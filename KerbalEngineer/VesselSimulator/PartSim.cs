// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2014 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 


namespace KerbalEngineer.VesselSimulator {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CompoundParts;
    using Extensions;
    using UnityEngine;

    public class PartSim {
        private static readonly Pool<PartSim> pool = new Pool<PartSim>(Create, Reset);

        private readonly List<AttachNodeSim> attachNodes = new List<AttachNodeSim>();

        public double realMass;
        public double baseMass;
        public double baseMassForCoM;
        public Vector3d centerOfMass;
        public double baseCost;
        public int decoupledInStage;
        public bool fuelCrossFeed;
        public List<PartSim> fuelTargets = new List<PartSim>();
        public List<PartSim> surfaceMountFuelTargets = new List<PartSim>();
        public bool hasModuleEngines;
        public bool hasMultiModeEngine;

        List<Part> chain = new List<Part>(); //prolly dont need a list, just the previous part but whatever.

        public bool hasVessel;
        public String initialVesselName;
        public int inverseStage;
        public int resPriorityOffset;
        public bool resPriorityUseParentInverseStage;
        public double resRequestRemainingThreshold;
        public bool isEngine;
        public bool isRCS;
        public bool isFuelLine;
        public bool isFuelTank;
        public bool isLanded;
        public bool isNoPhysics;
        public bool isSepratron;
        public float postStageMassAdjust;
        public int stageIndex;
        public String name;
        public String noCrossFeedNodeKey;
        public PartSim parent;
        public AttachModes parentAttach;
        public Part part; // This is only set while the data structures are being initialised
        public int partId = 0;
        public ResourceContainer resourceDrains = new ResourceContainer();
        public ResourceContainer resourceFlowStates = new ResourceContainer();
        public ResourceContainer resources = new ResourceContainer();
        // For RF residual calcs
        public ResourceContainer resourceCapacities = new ResourceContainer();
        public ResourceContainer residuals = new ResourceContainer();
        public double startMass = 0d;
        public double crewMassOffset = 0d;
        public String vesselName;
        public VesselType vesselType;
        public bool isEnginePlate;


        private static PartSim Create() {
            return new PartSim();
        }

        private static void Reset(PartSim partSim) {
            for (int i = 0; i < partSim.attachNodes.Count; i++) {
                partSim.attachNodes[i].Release();
            }
            partSim.attachNodes.Clear();
            partSim.fuelTargets.Clear();
            partSim.surfaceMountFuelTargets.Clear();
            partSim.resourceDrains.Reset();
            partSim.resourceFlowStates.Reset();
            partSim.resources.Reset();
            partSim.resourceCapacities.Reset();
            partSim.residuals.Reset();
            partSim.parent = null;
            partSim.baseCost = 0d;
            partSim.baseMass = 0d;
            partSim.baseMassForCoM = 0d;
            partSim.startMass = 0d;
            partSim.crewMassOffset = 0d;
        }

        public void Release() {
            pool.Release(this);
        }

        public static PartSim New(Part p, int id, double atmosphere, LogMsg log) {
            PartSim partSim = pool.Borrow();

            partSim.part = p;
            partSim.centerOfMass = p.transform.TransformPoint(p.CoMOffset);
            partSim.partId = id;
            partSim.name = p.partInfo.name;

            if (log != null) log.AppendLine("Create PartSim for ", partSim.name);

            partSim.parent = null;
            partSim.parentAttach = p.attachMode;
            partSim.fuelCrossFeed = p.fuelCrossFeed;
            partSim.noCrossFeedNodeKey = p.NoCrossFeedNodeKey;
            partSim.isEnginePlate = IsEnginePlate(p);
            if (partSim.isEnginePlate)
                partSim.noCrossFeedNodeKey = "bottom"; //sadly this only works in one direction.
            partSim.decoupledInStage = partSim.DecoupledInStage(p);
            partSim.isFuelLine = PartExtensions.HasModule<CModuleFuelLine>(p);
            partSim.isRCS = PartExtensions.HasModule<ModuleRCS>(p) || PartExtensions.HasModule<ModuleRCSFX>(p); //I don't think it checks inheritance.
            partSim.isSepratron = PartExtensions.IsSepratron(p);
            partSim.inverseStage = p.inverseStage;
            if (log != null) log.AppendLine("inverseStage = ", partSim.inverseStage);
            partSim.resPriorityOffset = p.resourcePriorityOffset;
            partSim.resPriorityUseParentInverseStage = p.resourcePriorityUseParentInverseStage;
            partSim.resRequestRemainingThreshold = p.resourceRequestRemainingThreshold;

            partSim.baseCost = PartExtensions.GetCostDry(p);

            if (log != null) log.AppendLine("Parent part = ", (p.parent == null ? "null" : p.parent.partInfo.name))
                                .AppendLine("physicalSignificance = ", p.physicalSignificance)
                                .AppendLine("PhysicsSignificance = ", p.PhysicsSignificance);

            // Work out if the part should have no physical significance
            // The root part is never "no physics"
            partSim.isNoPhysics = p.physicalSignificance == Part.PhysicalSignificance.NONE ||
                                    p.PhysicsSignificance == 1;

            if (PartExtensions.HasModule<LaunchClamp>(p)) {
                partSim.realMass = 0d;
                if (log != null) log.AppendLine("Ignoring mass of launch clamp");
            } else {
                partSim.crewMassOffset = PartExtensions.getCrewAdjustment(p);
                partSim.realMass = p.mass + partSim.crewMassOffset;

                if (log != null) log.AppendLine("Using part.mass of " + partSim.realMass);
            }

            partSim.postStageMassAdjust = 0f;
            if (log != null) log.AppendLine("Calculating postStageMassAdjust, prefabMass = ", p.prefabMass);
            int count = p.Modules.Count;
            for (int i = 0; i < count; i++) {
                if (log != null) log.AppendLine("Module: ", p.Modules[i].moduleName);
                IPartMassModifier partMassModifier = p.Modules[i] as IPartMassModifier;
                if (partMassModifier != null) {
                    if (log != null) log.AppendLine("ChangeWhen = ", partMassModifier.GetModuleMassChangeWhen());
                    if (partMassModifier.GetModuleMassChangeWhen() == ModifierChangeWhen.STAGED) {
                        float preStage = partMassModifier.GetModuleMass(p.prefabMass, ModifierStagingSituation.UNSTAGED);
                        float postStage = partMassModifier.GetModuleMass(p.prefabMass, ModifierStagingSituation.STAGED);
                        if (log != null) log.AppendLine("preStage = ", preStage, "   postStage = ", postStage);
                        partSim.postStageMassAdjust += (postStage - preStage);
                    }
                }
            }

            if (log != null) log.AppendLine("postStageMassAdjust = ", partSim.postStageMassAdjust);
            if (log != null) log.AppendLine("crewMassOffset = ", partSim.crewMassOffset);

            for (int i = 0; i < p.Resources.Count; i++) {
                PartResource resource = p.Resources[i];

                // Make sure it isn't NaN as this messes up the part mass and hence most of the values
                // This can happen if a resource capacity is 0 and tweakable
                if (!Double.IsNaN(resource.amount)) {
                    if (log != null) log.AppendLine(resource.resourceName, " = ", resource.amount);

                    partSim.resources.Add(resource.info.id, resource.amount);
                    partSim.resourceCapacities.Add(resource.info.id, resource.maxAmount);
                    partSim.resourceFlowStates.Add(resource.info.id, resource.flowState ? 1 : 0);
                } else {
                    if (log != null) log.AppendLine(resource.resourceName, " is NaN. Skipping.");
                }
            }

            partSim.hasVessel = (p.vessel != null);
            partSim.isLanded = partSim.hasVessel && p.vessel.Landed;
            if (partSim.hasVessel) {
                partSim.vesselName = p.vessel.vesselName;
                partSim.vesselType = p.vesselType;
            }
            partSim.initialVesselName = p.initialVesselName;

            partSim.hasMultiModeEngine = PartExtensions.HasModule<MultiModeEngine>(p);
            partSim.hasModuleEngines = PartExtensions.HasModule<ModuleEngines>(p);

            partSim.isEngine = partSim.hasMultiModeEngine || partSim.hasModuleEngines;

            if (log != null) log.AppendLine("Created ", partSim.name, ". Decoupled in stage ", partSim.decoupledInStage);

            return partSim;
        }

        public void CreateEngineSims(List<EngineSim> allEngines, double atmosphere, double mach, bool vectoredThrust, bool fullThrust, LogMsg log) {
            if (log != null) log.AppendLine("CreateEngineSims for ", this.name);
            List<ModuleEngines> cacheModuleEngines = part.FindModulesImplementing<ModuleEngines>();

            try {
                if (cacheModuleEngines.Count > 0) {
                    //find first active engine, assuming that two are never active at the same time
                    foreach (ModuleEngines engine in cacheModuleEngines) {
                        if (engine.isEnabled) {
                            if (log != null) log.AppendLine("Module: ", engine.moduleName);
                            EngineSim engineSim = EngineSim.New(
                                this,
                                engine,
                                atmosphere,
                                (float)mach,
                                vectoredThrust,
                                fullThrust,
                                log);
                            allEngines.Add(engineSim);
                        }
                    }
                }
            } catch {
                Debug.Log("[KER] Error Catch in CreateEngineSims");
            }
        }

        public void CreateRCSSims(List<RCSSim> allRCS, double atmosphere, double mach, bool vectoredThrust, bool fullThrust, LogMsg log) {
            if (log != null) log.AppendLine("CreateRCSSims for ", this.name);
            List<ModuleRCS> cacheModuleRCS = part.FindModulesImplementing<ModuleRCS>();

            try {
                if (cacheModuleRCS.Count > 0) {
                    //find first active engine, assuming that two are never active at the same time
                    foreach (ModuleRCS engine in cacheModuleRCS) {
                        if (engine.isEnabled) {
                            if (log != null) log.AppendLine("Module: ", engine.moduleName);
                            RCSSim engineSim = RCSSim.New(
                                this,
                                engine,
                                atmosphere,
                                (float)mach,
                                vectoredThrust,
                                fullThrust,
                                log);
                            allRCS.Add(engineSim);
                        }
                    }
                }
            } catch {
                Debug.Log("[KER] Error Catch in CreateRCSSims");
            }
        }

        public void DrainResources(double time, LogMsg log) {
            //if (log != null) log.Append("DrainResources(", name, ":", partId)
            //                    .AppendLine(", ", time, ")");
            for (int i = 0; i < resourceDrains.Types.Count; ++i) {
                int type = resourceDrains.Types[i];

                //if (log != null) log.AppendLine("draining ", (time * resourceDrains[type]), " ", ResourceContainer.GetResourceName(type));
                resources.Add(type, -time * resourceDrains[type]);
                //if (log != null) log.AppendLine(ResourceContainer.GetResourceName(type), " left = ", resources[type]);
            }
        }


        public String DumpPartAndParentsToLog(LogMsg log, String prefix) {
            if (log != null) {
                if (parent != null)
                    prefix = parent.DumpPartAndParentsToLog(log, prefix) + " ";

                DumpPartToLog(log, prefix);
            }

            return prefix;
        }

        public void DumpPartToLog(LogMsg log, String prefix, List<PartSim> allParts = null) {
            if (log == null)
                return;

            log.Append(prefix);
            log.Append(name);
            log.Append(":[id = ", partId, ", decouple = ", decoupledInStage);
            log.Append(", invstage = ", inverseStage);

            //log.Append(", vesselName = '", vesselName, "'");
            //log.Append(", vesselType = ", SimManager.GetVesselTypeString(vesselType));
            //log.Append(", initialVesselName = '", initialVesselName, "'");

            log.Append(", isNoPhys = ", isNoPhysics);
            log.buf.AppendFormat(", baseMass = {0}", baseMass);
            log.buf.AppendFormat(", baseMassForCoM = {0}", baseMassForCoM);

            log.Append(", fuelCF = {0}", fuelCrossFeed);
            log.Append(", noCFNKey = '{0}'", noCrossFeedNodeKey);

            log.Append(", isSep = {0}", isSepratron);

            try {
                for (int i = 0; i < resources.Types.Count; i++) {
                    int type = resources.Types[i];
                    log.buf.AppendFormat(", {0} = {1:g6}", ResourceContainer.GetResourceName(type), resources[type]);
                }
            } catch (Exception e) {
                log.Append("error dumping part resources " + e.ToString());
            }

            try {

                if (attachNodes.Count > 0) {
                    log.Append(", attached = <");
                    attachNodes[0].DumpToLog(log);
                    for (int i = 1; i < attachNodes.Count; i++) {
                        log.Append(", ");
                        if (attachNodes[i] != null) attachNodes[i].DumpToLog(log); //its u, isn't it?
                    }
                    log.Append(">");
                }
            } catch (Exception e) {
                log.Append("error dumping part nodes" + e.ToString());

            }

            try {
                if (surfaceMountFuelTargets.Count > 0) {
                    log.Append(", surface = <");
                    if (surfaceMountFuelTargets[0] != null) log.Append(surfaceMountFuelTargets[0].name, ":", surfaceMountFuelTargets[0].partId);
                    for (int i = 1; i < surfaceMountFuelTargets.Count; i++) {
                        if (surfaceMountFuelTargets[i] != null) log.Append(", ", surfaceMountFuelTargets[i].name, ":", surfaceMountFuelTargets[i].partId); //no it was u.
                    }
                    log.Append(">");
                }
            } catch (Exception e) {
                log.Append("error dumping part surface fuels " + e.ToString());
            }



            // Add more info here

            log.AppendLine("]");

            if (allParts != null) {
                String newPrefix = prefix + " ";
                for (int i = 0; i < allParts.Count; i++) {
                    PartSim partSim = allParts[i];
                    if (partSim.parent == this)
                        partSim.DumpPartToLog(log, newPrefix, allParts);
                }
            }
        }

        public bool EmptyOf(HashSet<int> types) {
            foreach (int type in types) {
                if (resources.HasType(type) && resourceFlowStates[type] != 0 && resources[type] > SimManager.RESOURCE_PART_EMPTY_THRESH)
                    return false;
            }

            return true;
        }

        public double GetMass(int currentStage, bool forCoM = false) {
            if (decoupledInStage >= currentStage)
                return 0d;

            double mass = forCoM ? baseMassForCoM : baseMass;

            for (int i = 0; i < resources.Types.Count; ++i) {
                mass += resources.GetResourceMass(resources.Types[i]);
            }

            if (postStageMassAdjust != 0.0 && currentStage <= inverseStage) {
                mass += postStageMassAdjust;
            }

            return mass;
        }

        public double GetCost(int currentStage) {
            if (decoupledInStage >= currentStage)
                return 0d;

            double cost = baseCost;

            for (int i = 0; i < resources.Types.Count; ++i) {
                cost += resources.GetResourceCost(resources.Types[i]);
            }

            return cost;
        }

        public void ReleasePart() {
            this.part = null;
        }

        // All functions below this point must not rely on the part member (it may be null)
        //

        public int GetResourcePriority() {
            return ((!resPriorityUseParentInverseStage || !(parent != null)) ? inverseStage : parent.inverseStage) * 10 + resPriorityOffset;
        }

        // This is a new function for STAGE_STACK_FLOW(_BALANCE)
        public void GetSourceSet(int type, List<PartSim> allParts, HashSet<PartSim> visited, HashSet<PartSim> allSources, LogMsg log, String indent) {
            // Initial version of support for new flow mode

            // Call a modified version of the old GetSourceSet code that adds all potential sources rather than stopping the recursive scan
            // when certain conditions are met
            int priMax = int.MinValue;
            GetSourceSet_Internal(type, allParts, visited, allSources, ref priMax, log, indent);
            if (log != null) log.AppendLine(allSources.Count, " parts with priority of ", priMax);
        }

        public void GetSourceSet_Internal(int type, List<PartSim> allParts, HashSet<PartSim> visited, HashSet<PartSim> allSources, ref int priMax, LogMsg log, String indent) {
            if (log != null) {
                log.Append(indent, "GetSourceSet_Internal(", ResourceContainer.GetResourceName(type), ") for ")
                    .AppendLine(name, ":", partId);
                indent += "  ";
            }

            // Rule 1: Each part can be only visited once, If it is visited for second time in particular search it returns as is.
            if (visited.Contains(this)) {
                if (log != null) log.Append(indent, "Nothing added, already visited (", name, ":")
                                    .AppendLine(partId + ")");
                return;
            }

            if (log != null) log.AppendLine(indent, "Adding this to visited");

            visited.Add(this);

            // Rule 2: Part performs scan on start of every fuel pipe ending in it. This scan is done in order in which pipes were installed.
            // Then it makes an union of fuel tank sets each pipe scan returned. If the resulting list is not empty, it is returned as result.
            //MonoBehaviour.print("for each fuel line");

            int lastCount = allSources.Count;

            for (int i = 0; i < this.fuelTargets.Count; i++) {
                PartSim partSim = this.fuelTargets[i];
                if (partSim != null) {
                    if (visited.Contains(partSim)) {
                        if (log != null) log.Append(indent, "Fuel target already visited, skipping (", partSim.name, ":")
                                            .AppendLine(partSim.partId, ")");
                    } else {
                        if (log != null) log.Append(indent, "Adding fuel target as source (", partSim.name, ":")
                                            .AppendLine(partSim.partId, ")");

                        partSim.GetSourceSet_Internal(type, allParts, visited, allSources, ref priMax, log, indent);
                    }
                }
            }

            if (fuelCrossFeed) {

                // check surface mounted fuel targets
                for (int i = 0; i < surfaceMountFuelTargets.Count; i++) {
                    PartSim partSim = this.surfaceMountFuelTargets[i];
                    if (partSim != null) {
                        if (visited.Contains(partSim)) {
                            if (log != null) log.Append(indent, "Surface part already visited, skipping (", partSim.name, ":")
                                                .AppendLine(partSim.partId, ")");
                        } else {
                            if (log != null) log.Append(indent, "Adding surface part as source (", partSim.name, ":")
                                                .AppendLine(partSim.partId, ")");

                            partSim.GetSourceSet_Internal(type, allParts, visited, allSources, ref priMax, log, indent);
                        }
                    }
                }

                lastCount = allSources.Count;
                //MonoBehaviour.print("for each attach node");
                for (int i = 0; i < this.attachNodes.Count; i++) {
                    AttachNodeSim attachSim = this.attachNodes[i];
                    if (attachSim.attachedPartSim != null) {
                        if (attachSim.nodeType == AttachNode.NodeType.Stack) {
                            if ((string.IsNullOrEmpty(noCrossFeedNodeKey) == false && attachSim.id.Contains(noCrossFeedNodeKey)) == false) {
                                if (visited.Contains(attachSim.attachedPartSim)) {
                                    if (log != null) log.Append(indent, "Attached part already visited, skipping (", attachSim.attachedPartSim.name, ":")
                                                        .AppendLine(attachSim.attachedPartSim.partId, ")");
                                } else {
                                    bool flg = true;

                                    if (attachSim.attachedPartSim.isEnginePlate) //y u make me do dis.
                                    {
                                        foreach (AttachNodeSim att in attachSim.attachedPartSim.attachNodes) {
                                            if (att.attachedPartSim == this && att.id == "bottom")
                                                flg = false;
                                        }
                                    }

                                    if (flg) {
                                        if (log != null) log.Append(indent, "Adding attached part as source  (", attachSim.attachedPartSim.name, ":")
                                                            .AppendLine(attachSim.attachedPartSim.partId, ")");

                                        attachSim.attachedPartSim.GetSourceSet_Internal(type, allParts, visited, allSources, ref priMax, log, indent);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // If the part is fuel container for searched type of fuel (i.e. it has capability to contain that type of fuel and the fuel 
            // type was not disabled) and it contains fuel, it adds itself.
            if (resources.HasType(type) && resourceFlowStates[type] > 0.0) {
                if (resources[type] > resRequestRemainingThreshold) {
                    // Get the priority of this tank
                    int pri = GetResourcePriority();
                    if (pri > priMax) {
                        // This tank is higher priority than the previously added ones so we clear the sources
                        // and set the priMax to this priority
                        allSources.Clear();
                        priMax = pri;
                    }
                    // If this is the correct priority then add this to the sources
                    if (pri == priMax) {
                        if (log != null) log.Append(indent, "Adding enabled tank as source (", name, ":")
                                            .AppendLine(partId, ")");

                        allSources.Add(this);
                    }
                } else {
                    if (log != null) log.Append(indent, name + " not enough " + ResourceContainer.GetResourceName(type))
                                        .AppendLine("  Requested = " + resRequestRemainingThreshold + " actual " + resources[type]);
                }
            } else {
                if (log != null) log.Append(indent, name + " not fuel tank or disabled. HasType = ", resources.HasType(type))
                                    .AppendLine("  FlowState = " + resourceFlowStates[type]);
            }
        }

        // This is the old recursive function for STACK_PRIORITY_SEARCH
        //public void GetSourceSet_Old(int type, bool includeSurfaceMountedParts, List<PartSim> allParts, HashSet<PartSim> visited, HashSet<PartSim> allSources, LogMsg log, String indent)
        //{
        //    if (log != null)
        //    {
        //        log.Append(indent, "GetSourceSet_Old(", ResourceContainer.GetResourceName(type), ") for ")
        //            .AppendLine(name, ":", partId);
        //        indent += "  ";
        //    }

        //    // Rule 1: Each part can be only visited once, If it is visited for second time in particular search it returns as is.
        //    if (visited.Contains(this))
        //    {
        //        if (log != null) log.Append(indent, "Returning empty set, already visited (", name, ":")
        //                            .AppendLine(partId + ")");
        //        return;
        //    }

        //    if (log != null) log.AppendLine(indent, "Adding this to visited");

        //    visited.Add(this);

        //    // Rule 2: Part performs scan on start of every fuel pipe ending in it. This scan is done in order in which pipes were installed.
        //    // Then it makes an union of fuel tank sets each pipe scan returned. If the resulting list is not empty, it is returned as result.
        //    //MonoBehaviour.print("for each fuel line");

        //    int lastCount = allSources.Count;

        //    for (int i = 0; i < this.fuelTargets.Count; i++)
        //    {
        //        PartSim partSim = this.fuelTargets[i];
        //        if (partSim != null)
        //        {
        //            if (visited.Contains(partSim))
        //            {
        //                if (log != null) log.Append(indent, "Fuel target already visited, skipping (", partSim.name, ":")
        //                                    .AppendLine(partSim.partId, ")");
        //            }
        //            else
        //            {
        //                if (log != null) log.Append(indent, "Adding fuel target as source (", partSim.name, ":")
        //                                    .AppendLine(partSim.partId, ")");

        //                partSim.GetSourceSet_Old(type, includeSurfaceMountedParts, allParts, visited, allSources, log, indent);
        //            }
        //        }
        //    }

        //    // check surface mounted fuel targets
        //    if (includeSurfaceMountedParts)
        //    {
        //        for (int i = 0; i < surfaceMountFuelTargets.Count; i++)
        //        {
        //            PartSim partSim = this.surfaceMountFuelTargets[i];
        //            if (partSim != null)
        //            {
        //                if (visited.Contains(partSim))
        //                {
        //                    if (log != null) log.Append(indent, "Fuel target already visited, skipping (", partSim.name, ":")
        //                                        .AppendLine(partSim.partId, ")");
        //                }
        //                else
        //                {
        //                    if (log != null) log.Append(indent, "Adding fuel target as source (", partSim.name, ":")
        //                                        .AppendLine(partSim.partId, ")");

        //                    partSim.GetSourceSet_Old(type, true, allParts, visited, allSources, log, indent);
        //                }
        //            }
        //        }
        //    }

        //    if (allSources.Count > lastCount)
        //    {
        //        if (log != null) log.Append(indent, "Returning ", (allSources.Count - lastCount), " fuel target sources (")
        //                            .AppendLine(this.name, ":", this.partId, ")");
        //        return;
        //    }


        //    // Rule 3: This rule has been removed and merged with rules 4 and 7 to fix issue with fuel tanks with disabled crossfeed

        //    // Rule 4: Part performs scan on each of its axially mounted neighbors. 
        //    //  Couplers (bicoupler, tricoupler, ...) are an exception, they only scan one attach point on the single attachment side,
        //    //  skip the points on the side where multiple points are. [Experiment]
        //    //  Again, the part creates union of scan lists from each of its neighbor and if it is not empty, returns this list. 
        //    //  The order in which mount points of a part are scanned appears to be fixed and defined by the part specification file. [Experiment]
        //    if (fuelCrossFeed)
        //    {
        //        lastCount = allSources.Count;
        //        //MonoBehaviour.print("for each attach node");
        //        for (int i = 0; i < this.attachNodes.Count; i++)
        //        {
        //            AttachNodeSim attachSim = this.attachNodes[i];
        //            if (attachSim.attachedPartSim != null)
        //            {
        //                if (attachSim.nodeType == AttachNode.NodeType.Stack)
        //                {
        //                    if ((string.IsNullOrEmpty(noCrossFeedNodeKey) == false && attachSim.id.Contains(noCrossFeedNodeKey)) == false)
        //                    {
        //                        if (visited.Contains(attachSim.attachedPartSim))
        //                        {
        //                            if (log != null) log.Append(indent, "Attached part already visited, skipping (", attachSim.attachedPartSim.name, ":")
        //                                                .AppendLine(attachSim.attachedPartSim.partId, ")");
        //                        }
        //                        else
        //                        {
        //                            if (log != null) log.Append(indent, "Adding attached part as source  (", attachSim.attachedPartSim.name, ":")
        //                                                .AppendLine(attachSim.attachedPartSim.partId, ")");

        //                            attachSim.attachedPartSim.GetSourceSet_Old(type, includeSurfaceMountedParts, allParts, visited, allSources, log, indent);
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        if (allSources.Count > lastCount)
        //        {
        //            if (log != null) log.Append(indent, "Returning " + (allSources.Count - lastCount) + " attached sources (")
        //                                .AppendLine(this.name, ":", this.partId, ")");
        //            return;
        //        }
        //    }

        //    // Rule 5: If the part is fuel container for searched type of fuel (i.e. it has capability to contain that type of fuel and the fuel 
        //    // type was not disabled [Experiment]) and it contains fuel, it returns itself.
        //    // Rule 6: If the part is fuel container for searched type of fuel (i.e. it has capability to contain that type of fuel and the fuel 
        //    // type was not disabled) but it does not contain the requested fuel, it returns empty list. [Experiment]
        //    if (resources.HasType(type) && resourceFlowStates[type] > 0.0)
        //    {
        //        if (resources[type] > SimManager.RESOURCE_MIN)
        //        {
        //            allSources.Add(this);

        //            if (log != null) log.Append(indent, "Returning enabled tank as only source (", name, ":")
        //                                .AppendLine(partId, ")");

        //            return;
        //        }
        //    }
        //    else
        //    {
        //        if (log != null) log.Append(indent, "Not fuel tank or disabled. HasType = ", resources.HasType(type))
        //                            .AppendLine("  FlowState = " + resourceFlowStates[type]);
        //    }

        //    // Rule 7: If the part is radially attached to another part and it is child of that part in the ship's tree structure, it scans its 
        //    // parent and returns whatever the parent scan returned. [Experiment] [Experiment]
        //    if (parent != null && parentAttach == AttachModes.SRF_ATTACH)
        //    {
        //        if (fuelCrossFeed)
        //        {
        //            if (visited.Contains(parent))
        //            {
        //                if (log != null) log.Append(indent, "Parent part already visited, skipping (", parent.name, ":")
        //                                    .AppendLine(parent.partId, ")");
        //            }
        //            else
        //            {
        //                lastCount = allSources.Count;
        //                this.parent.GetSourceSet_Old(type, includeSurfaceMountedParts, allParts, visited, allSources, log, indent);
        //                if (allSources.Count > lastCount)
        //                {
        //                    if (log != null) log.Append(indent, "Returning ", (allSources.Count - lastCount), " parent sources (")
        //                                        .AppendLine(this.name, ":", this.partId, ")");
        //                    return;
        //                }
        //            }
        //        }
        //    }

        //    // Rule 8: If all preceding rules failed, part returns empty list.
        //    if (log != null) log.Append(indent, "Returning empty set, no sources found (", name, ":")
        //                        .AppendLine(partId, ")");

        //    return;
        //}

        public double GetStartMass() {
            return startMass;
        }

        public void RemoveAttachedParts(HashSet<PartSim> partSims) {
            // Loop through the attached parts
            for (int i = 0; i < this.attachNodes.Count; i++) {
                AttachNodeSim attachSim = this.attachNodes[i];
                // If the part is in the set then "remove" it by clearing the PartSim reference
                if (partSims.Contains(attachSim.attachedPartSim)) {
                    attachSim.attachedPartSim = null;
                }
            }

            // Loop through the fuel targets (fuel line sources)
            for (int i = 0; i < this.fuelTargets.Count; i++) {
                PartSim fuelTargetSim = this.fuelTargets[i];
                // If the part is in the set then "remove" it by clearing the PartSim reference
                if (fuelTargetSim != null && partSims.Contains(fuelTargetSim)) {
                    this.fuelTargets[i] = null;
                }
            }

            // Loop through the surface attached fuel targets (surface attached parts for new flow modes)
            for (int i = 0; i < this.surfaceMountFuelTargets.Count; i++) {
                PartSim fuelTargetSim = this.surfaceMountFuelTargets[i];
                // If the part is in the set then "remove" it by clearing the PartSim reference
                if (fuelTargetSim != null && partSims.Contains(fuelTargetSim)) {
                    this.surfaceMountFuelTargets[i] = null;
                }
            }
        }

        public void SetupAttachNodes(Dictionary<Part, PartSim> partSimLookup, LogMsg log) {
            if (log != null) log.AppendLine("SetupAttachNodes for ", name, ":", partId);

            attachNodes.Clear();

            for (int i = 0; i < part.attachNodes.Count; ++i) {
                AttachNode attachNode = part.attachNodes[i];

                if (log != null) log.AppendLine("AttachNode ", attachNode.id, " = ", (attachNode.attachedPart != null ? attachNode.attachedPart.partInfo.name : "null"));

                if (attachNode.attachedPart != null && attachNode.id != "Strut") {
                    PartSim attachedSim;
                    if (partSimLookup.TryGetValue(attachNode.attachedPart, out attachedSim)) {
                        if (log != null) log.AppendLine("Adding attached node ", attachedSim.name, ":", attachedSim.partId);

                        attachNodes.Add(AttachNodeSim.New(attachedSim, attachNode.id, attachNode.nodeType));
                    } else {
                        if (log != null) log.AppendLine("No PartSim for attached part (", attachNode.attachedPart.partInfo.name, ")");
                    }
                }
            }

            for (int i = 0; i < part.fuelLookupTargets.Count; ++i) {
                Part p = part.fuelLookupTargets[i];

                if (p != null) {
                    PartSim targetSim;
                    if (partSimLookup.TryGetValue(p, out targetSim)) {
                        if (log != null) log.AppendLine("Fuel target: ", targetSim.name, ":", targetSim.partId);

                        fuelTargets.Add(targetSim);
                    } else {
                        if (log != null) log.AppendLine("No PartSim for fuel target (", p.name, ")");
                    }
                }
            }
        }

        public void SetupParent(Dictionary<Part, PartSim> partSimLookup, LogMsg log) {
            if (part.parent != null) {
                parent = null;
                if (partSimLookup.TryGetValue(part.parent, out parent)) {
                    if (log != null) log.AppendLine("Parent part is ", parent.name, ":", parent.partId);
                    if (part.attachMode == AttachModes.SRF_ATTACH && part.attachRules.srfAttach && part.fuelCrossFeed && part.parent.fuelCrossFeed) {
                        if (log != null) log.Append("Added (", name, ":", partId)
                                            .AppendLine(", ", parent.name, ":", parent.partId, ") to surface mounted fuel targets.");
                        parent.surfaceMountFuelTargets.Add(this);
                        surfaceMountFuelTargets.Add(parent);
                    }
                } else {
                    if (log != null) log.AppendLine("No PartSim for parent part (", part.parent.partInfo.name, ")");
                }
            }
        }

        public double TimeToDrainResource(LogMsg log) {
            //if (log != null) log.AppendLine("TimeToDrainResource(", name, ":", partId, ")");
            double time = double.MaxValue;

            for (int i = 0; i < resourceDrains.Types.Count; ++i) {
                int type = resourceDrains.Types[i];

                if (resourceDrains[type] > 0) {
                    double amount = resources[type];
                    if (residuals.HasType(type))
                        amount -= residuals[type];
                    time = Math.Min(time, amount / resourceDrains[type]);
                    //if (log != null) log.AppendLine("type = " + ResourceContainer.GetResourceName(type) + "  amount = " + resources[type] + "  rate = " + resourceDrains[type] + "  time = " + time);
                }
            }

            //if (time < double.MaxValue)
            //    if (log != null) log.Append("TimeToDrainResource(", name, ":", partId)
            //                        .AppendLine(") = ", time);
            return time;
        }

        public double CalcTimeToDrainResource(double consumptionRate, int resource, LogMsg log) {
            double time = double.MaxValue;

            if (consumptionRate > 0) {
                time = Math.Min(time, resources[resource] / consumptionRate);
            }

            return time;
        }

        private Vector3 CalculateThrustVector(List<Transform> thrustTransforms, LogMsg log) {
            if (thrustTransforms == null) {
                return Vector3.forward;
            }

            Vector3 thrustvec = Vector3.zero;
            for (int i = 0; i < thrustTransforms.Count; ++i) {
                Transform trans = thrustTransforms[i];

                if (log != null) log.buf.AppendFormat("Transform = ({0:g6}, {1:g6}, {2:g6})   length = {3:g6}\n", trans.forward.x, trans.forward.y, trans.forward.z, trans.forward.magnitude);

                thrustvec -= trans.forward;
            }

            if (log != null) log.buf.AppendFormat("ThrustVec  = ({0:g6}, {1:g6}, {2:g6})   length = {3:g6}\n", thrustvec.x, thrustvec.y, thrustvec.z, thrustvec.magnitude);

            thrustvec.Normalize();

            if (log != null) log.buf.AppendFormat("ThrustVecN = ({0:g6}, {1:g6}, {2:g6})   length = {3:g6}\n", thrustvec.x, thrustvec.y, thrustvec.z, thrustvec.magnitude);

            return thrustvec;
        }

        private int DecoupledInStage(Part thePart) {
            int stage = -1;
            Part original = thePart;

            if (original.parent == null)
                return stage; //root part is always present. Fixes phantom stage if root is stageable.

            chain.Clear();

            while (thePart != null) {

                chain.Add(thePart);

                if (thePart.inverseStage > stage) {

                    ModuleDecouple mdec = PartExtensions.GetModule<ModuleDecouple>(thePart);
                    ModuleDockingNode mdock = PartExtensions.GetModule<ModuleDockingNode>(thePart);
                    ModuleAnchoredDecoupler manch = PartExtensions.GetModule<ModuleAnchoredDecoupler>(thePart);

                    if (mdec != null) {
                        AttachNode att = thePart.FindAttachNode(mdec.explosiveNodeID);
                        if (mdec.isOmniDecoupler)
                            stage = thePart.inverseStage;
                        else {
                            if (att != null) {
                                if ((thePart.parent != null && att.attachedPart == thePart.parent) || PartExtensions.ContainedPart(att.attachedPart, chain))
                                    stage = thePart.inverseStage;
                            } else stage = thePart.inverseStage;
                        }
                    }

                    if (manch != null) //radial decouplers (ALSO REENTRY PODS BECAUSE REASONS!)
                    {
                        AttachNode att = thePart.FindAttachNode(manch.explosiveNodeID); // these stupid fuckers don't initialize in the Editor scene.
                        if (att != null) {
                            if ((thePart.parent != null && att.attachedPart == thePart.parent) || PartExtensions.ContainedPart(att.attachedPart, chain))
                                stage = thePart.inverseStage;
                        } else stage = thePart.inverseStage; //radial decouplers it seems the attach node ('surface') comes back null.
                    }

                    if (mdock != null) //docking port
                    {
                        if (original == thePart) {    //checking self, never leaves.

                        } else stage = thePart.inverseStage;
                    }

                }

                thePart = thePart.parent;
            }
            
            return stage;
        }

        private static bool IsEnginePlate(Part thePart) {
            ModuleDecouple mdec = PartExtensions.GetModule<ModuleDecouple>(thePart);
            if (mdec != null && mdec.IsStageable()) {
                ModuleDynamicNodes mdyn = PartExtensions.GetModule<ModuleDynamicNodes>(thePart);
                if (mdyn != null)
                    return true;
            }

            return false;
        }

        private bool IsSepratron() {
            if (!part.ActivatesEvenIfDisconnected) {
                return false;
            }

            IEnumerable<ModuleEngines> modList = part.Modules.OfType<ModuleEngines>();

            return modList.Any(module => module.throttleLocked);
        }
    }
}
