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
    using System.Text;
    using Editor;
    using Helpers;
    using UnityEngine;

    public class RCSSim {
        private static readonly Pool<RCSSim> pool = new Pool<RCSSim>(Create, Reset);

        public readonly ResourceContainer resourceConsumptions = new ResourceContainer();
        public readonly ResourceContainer resourceFlowModes = new ResourceContainer();

        public double actualThrust = 0;
        public bool isActive = false;
        public double isp = 0;
        public PartSim partSim;
        public List<AppliedForce> appliedForces = new List<AppliedForce>();
        public float maxMach;
        public bool isFlamedOut;
        public bool dontDecoupleActive = true;

        public double thrust = 0;

        // Add thrust vector to account for directional losses
        public Vector3 thrustVec;

        private static RCSSim Create() {
            return new RCSSim();
        }

        private static void Reset(RCSSim engineSim) {
            engineSim.resourceConsumptions.Reset();
            engineSim.resourceFlowModes.Reset();
            engineSim.partSim = null;
            engineSim.actualThrust = 0;
            engineSim.isActive = false;
            engineSim.isp = 0;
            for (int i = 0; i < engineSim.appliedForces.Count; i++) {
                engineSim.appliedForces[i].Release();
            }
            engineSim.appliedForces.Clear();
            engineSim.thrust = 0;
            engineSim.maxMach = 0f;
            engineSim.isFlamedOut = false;
        }

        public void Release() {
            pool.Release(this);
        }

        public static RCSSim New(PartSim theEngine,
                                    ModuleRCS engineMod,
                                    double atmosphere,
                                    float machNumber,
                                    bool vectoredThrust,
                                    bool fullThrust,
                                    LogMsg log) {
            double maxFuelFlow = engineMod.maxFuelFlow;
            // double minFuelFlow = engineMod.minFuelFlow;
            float thrustPercentage = engineMod.thrustPercentage;
            List<Transform> thrustTransforms = engineMod.thrusterTransforms;
            //   List<float> thrustTransformMultipliers = engineMod.th
            Vector3 vecThrust = CalculateThrustVector(vectoredThrust ? thrustTransforms : null, log);
            FloatCurve atmosphereCurve = engineMod.atmosphereCurve;
            //   bool atmChangeFlow = engineMod.at
            //   FloatCurve atmCurve = engineMod.useAtmCurve ? engineMod.atmCurve : null;
            //   FloatCurve velCurve = engineMod.useVelCurve ? engineMod.velCurve : null;
            //    float currentThrottle = engineMod.currentThrottle;
            double IspG = engineMod.G;
            //    bool throttleLocked = engineMod.throttleLocked || fullThrust;
            List<Propellant> propellants = engineMod.propellants;
            bool active = engineMod.moduleIsEnabled;
            //    float resultingThrust = engineMod.resultingThrust;
            bool isFlamedOut = engineMod.flameout;

            RCSSim engineSim = pool.Borrow();

            engineSim.isp = 0.0;
            engineSim.maxMach = 0.0f;
            engineSim.actualThrust = 0.0;
            engineSim.partSim = theEngine;
            engineSim.isActive = active;
            engineSim.thrustVec = vecThrust;
            engineSim.isFlamedOut = isFlamedOut;
            engineSim.resourceConsumptions.Reset();
            engineSim.resourceFlowModes.Reset();
            engineSim.appliedForces.Clear();

            double flowRate = 0.0;
            if (engineSim.partSim.hasVessel) {
                if (log != null) log.AppendLine("hasVessel is true");

                engineSim.isp = atmosphereCurve.Evaluate((float)atmosphere);
                engineSim.thrust = GetThrust(maxFuelFlow, engineSim.isp);
                engineSim.actualThrust = engineSim.isActive ? engineSim.thrust : 0.0;

                if (log != null) {
                    log.buf.AppendFormat("isp     = {0:g6}\n", engineSim.isp);
                    log.buf.AppendFormat("thrust  = {0:g6}\n", engineSim.thrust);
                    log.buf.AppendFormat("actual  = {0:g6}\n", engineSim.actualThrust);
                }

                if (true) {
                    if (log != null) log.AppendLine("throttleLocked is true, using thrust for flowRate");
                    flowRate = GetFlowRate(engineSim.thrust, engineSim.isp);
                } else {
                    //              if (currentThrottle > 0.0f && engineSim.partSim.isLanded == false)
                    //              {
                    //// TODO: This bit doesn't work for RF engines
                    //if (log != null) log.AppendLine("throttled up and not landed, using actualThrust for flowRate");
                    //                  flowRate = GetFlowRate(engineSim.actualThrust, engineSim.isp);
                    //              }
                    //              else
                    //              {
                    //                  if (log != null) log.AppendLine("throttled down or landed, using thrust for flowRate");
                    //                  flowRate = GetFlowRate(engineSim.thrust, engineSim.isp);
                    //              }
                }
            } else {
                if (log != null) log.buf.AppendLine("hasVessel is false");
                engineSim.isp = atmosphereCurve.Evaluate((float)atmosphere);
                engineSim.thrust = GetThrust(maxFuelFlow, engineSim.isp);
                engineSim.actualThrust = 0d;
                if (log != null) {
                    log.buf.AppendFormat("isp     = {0:g6}\n", engineSim.isp);
                    log.buf.AppendFormat("thrust  = {0:g6}\n", engineSim.thrust);
                    log.buf.AppendFormat("actual  = {0:g6}\n", engineSim.actualThrust);
                    log.AppendLine("no vessel, using thrust for flowRate");
                }

                flowRate = GetFlowRate(engineSim.thrust, engineSim.isp);
            }

            if (log != null) log.buf.AppendFormat("flowRate = {0:g6}\n", flowRate);

            float flowMass = 0f;
            for (int i = 0; i < propellants.Count; ++i) {
                Propellant propellant = propellants[i];
                if (!propellant.ignoreForIsp)
                    flowMass += propellant.ratio * ResourceContainer.GetResourceDensity(propellant.id);
            }

            if (log != null) log.buf.AppendFormat("flowMass = {0:g6}\n", flowMass);

            for (int i = 0; i < propellants.Count; ++i) {
                Propellant propellant = propellants[i];

                if (propellant.ignoreForIsp || propellant.name == "ElectricCharge" || propellant.name == "IntakeAir" || || DeltaVGlobals.PropellantsToIgnore.Contains(propellant.name)) {
                    continue;
                }

                double consumptionRate = propellant.ratio * flowRate / flowMass;
                if (log != null) log.buf.AppendFormat(
                        "Add consumption({0}, {1}:{2:d}) = {3:g6}\n",
                        ResourceContainer.GetResourceName(propellant.id),
                        theEngine.name,
                        theEngine.partId,
                        consumptionRate);
                engineSim.resourceConsumptions.Add(propellant.id, consumptionRate);
                engineSim.resourceFlowModes.Add(propellant.id, (double)propellant.GetFlowMode());
            }

            for (int i = 0; i < thrustTransforms.Count; i++) {
                Transform thrustTransform = thrustTransforms[i];
                Vector3d direction = thrustTransform.forward.normalized;
                Vector3d position = thrustTransform.position;

                AppliedForce appliedForce = AppliedForce.New(direction * engineSim.thrust, position);
                engineSim.appliedForces.Add(appliedForce);
            }

            return engineSim;
        }

        private static Vector3 CalculateThrustVector(List<Transform> thrustTransforms, LogMsg log) {
            if (thrustTransforms == null) {
                return Vector3.forward;
            }

            Vector3 thrustvec = Vector3.zero;
            for (int i = 0; i < thrustTransforms.Count; ++i) {
                Transform trans = thrustTransforms[i];

                if (log != null) log.buf.AppendFormat("Transform = ({0:g6}, {1:g6}, {2:g6})   length = {3:g6}\n", trans.forward.x, trans.forward.y, trans.forward.z, trans.forward.magnitude);

                thrustvec -= (trans.forward);
            }

            if (log != null) log.buf.AppendFormat("ThrustVec  = ({0:g6}, {1:g6}, {2:g6})   length = {3:g6}\n", thrustvec.x, thrustvec.y, thrustvec.z, thrustvec.magnitude);

            thrustvec.Normalize();

            if (log != null) log.buf.AppendFormat("ThrustVecN = ({0:g6}, {1:g6}, {2:g6})   length = {3:g6}\n", thrustvec.x, thrustvec.y, thrustvec.z, thrustvec.magnitude);

            return thrustvec;
        }

        public static double GetExhaustVelocity(double isp) {
            return isp * Units.GRAVITY;
        }

        public static float GetFlowModifier(bool atmChangeFlow, FloatCurve atmCurve, double atmDensity, FloatCurve velCurve, float machNumber, ref float maxMach) {
            float flowModifier = 1.0f;
            if (atmChangeFlow) {
                flowModifier = (float)(atmDensity / 1.225);
                if (atmCurve != null) {
                    flowModifier = atmCurve.Evaluate(flowModifier);
                }
            }
            if (velCurve != null) {
                flowModifier = flowModifier * velCurve.Evaluate(machNumber);
                maxMach = velCurve.maxTime;
            }
            if (flowModifier < float.Epsilon) {
                flowModifier = float.Epsilon;
            }
            return flowModifier;
        }

        public static double GetFlowRate(double thrust, double isp) {
            return thrust / GetExhaustVelocity(isp);
        }

        public static float GetThrottlePercent(float currentThrottle, float thrustPercentage) {
            return currentThrottle * GetThrustPercent(thrustPercentage);
        }

        public static double GetThrust(double flowRate, double isp) {
            return flowRate * GetExhaustVelocity(isp);
        }

        public static float GetThrustPercent(float thrustPercentage) {
            return thrustPercentage * 0.01f;
        }

        public void DumpEngineToLog(LogMsg log) {
            if (log != null) log.buf.AppendFormat("RCS: [thrust = {0:g6}, actual = {1:g6}, isp = {2:g6}\n", thrust, actualThrust, isp);
        }

        // A dictionary to hold a set of parts for each resource
        Dictionary<int, HashSet<PartSim>> sourcePartSets = new Dictionary<int, HashSet<PartSim>>();

        Dictionary<int, HashSet<PartSim>> stagePartSets = new Dictionary<int, HashSet<PartSim>>();

        HashSet<PartSim> visited = new HashSet<PartSim>();

        public void DumpSourcePartSets(LogMsg log, String msg) {
            if (log == null)
                return;

            log.AppendLine("DumpSourcePartSets ", msg);
            foreach (int type in sourcePartSets.Keys) {
                log.AppendLine("SourcePartSet for ", ResourceContainer.GetResourceName(type));
                HashSet<PartSim> sourcePartSet = sourcePartSets[type];
                if (sourcePartSet.Count > 0) {
                    foreach (PartSim partSim in sourcePartSet) {
                        log.AppendLine("Part ", partSim.name, ":", partSim.partId);
                    }
                } else {
                    log.AppendLine("No parts");
                }
            }
        }

        //public bool SetResourceDrains(LogMsg log, List<PartSim> allParts)
        //{
        //    //DumpSourcePartSets(log, "before clear");
        //    foreach (HashSet<PartSim> sourcePartSet in sourcePartSets.Values)
        //    {
        //        sourcePartSet.Clear();
        //    }
        //    //DumpSourcePartSets(log, "after clear");

        //    for (int index = 0; index < this.resourceConsumptions.Types.Count; index++)
        //    {
        //        int type = this.resourceConsumptions.Types[index];

        //        HashSet<PartSim> sourcePartSet;
        //        if (!sourcePartSets.TryGetValue(type, out sourcePartSet))
        //        {
        //            sourcePartSet = new HashSet<PartSim>();
        //            sourcePartSets.Add(type, sourcePartSet);
        //        }

        //        switch ((ResourceFlowMode)this.resourceFlowModes[type])
        //        {
        //            case ResourceFlowMode.NO_FLOW:
        //                if (partSim.resourceDrains[type] > SimManager.RESOURCE_MIN && partSim.resourceFlowStates[type] != 0)
        //                {
        //                    sourcePartSet.Add(partSim);
        //                }
        //                break;

        //            case ResourceFlowMode.ALL_VESSEL:
        //            case ResourceFlowMode.ALL_VESSEL_BALANCE:
        //                for (int i = 0; i < allParts.Count; i++)
        //                {
        //                    PartSim aPartSim = allParts[i];
        //                    if (aPartSim.resourceDrains[type] > SimManager.RESOURCE_MIN && aPartSim.resourceFlowStates[type] != 0)
        //                    {
        //                        sourcePartSet.Add(aPartSim);
        //                    }
        //                }
        //                break;

        //            case ResourceFlowMode.STAGE_PRIORITY_FLOW:
        //            case ResourceFlowMode.STAGE_PRIORITY_FLOW_BALANCE:

        //                if (log != null) log.Append("Find ", ResourceContainer.GetResourceName(type), " sources for ", partSim.name)
        //                                    .AppendLine(":" , partSim.partId);
        //                foreach (HashSet<PartSim> stagePartSet in stagePartSets.Values)
        //                {
        //                    stagePartSet.Clear();
        //                }
        //                var maxStage = -1;

        //                for (int i = 0; i < allParts.Count; i++)
        //                {
        //                    var aPartSim = allParts[i];
        //                    //if (log != null) log.Append(aPartSim.name, ":" + aPartSim.partId, " contains ", aPartSim.resources[type])
        //                    //                  .AppendLine((aPartSim.resourceFlowStates[type] == 0) ? " (disabled)" : "");
        //                    if (aPartSim.resources[type] <= SimManager.RESOURCE_MIN || aPartSim.resourceFlowStates[type] == 0)
        //                    {
        //                        continue;
        //                    }

        //                    int stage = aPartSim.inverseStage;
        //                    if (stage > maxStage)
        //                    {
        //                        maxStage = stage;
        //                    }

        //                    HashSet<PartSim> tempPartSet;
        //                    if (!stagePartSets.TryGetValue(stage, out tempPartSet))
        //                    {
        //                        tempPartSet = new HashSet<PartSim>();
        //                        stagePartSets.Add(stage, tempPartSet);
        //                    }
        //                    tempPartSet.Add(aPartSim);
        //                }

        //                for (int j = maxStage; j >= -1; j--)
        //                {
        //                    //if (log != null) log.AppendLine("Testing stage ", j);
        //                    HashSet<PartSim> stagePartSet;
        //                    if (stagePartSets.TryGetValue(j, out stagePartSet) && stagePartSet.Count > 0)
        //                    {
        //                        //if (log != null) log.AppendLine("Not empty");
        //                        // We have to copy the contents of the set here rather than copying the set reference or 
        //                        // bad things (tm) happen
        //                        foreach (PartSim aPartSim in stagePartSet)
        //                        {
        //                            sourcePartSet.Add(aPartSim);
        //                        }
        //                        break;
        //                    }
        //                }
        //                break;

        //            case ResourceFlowMode.STACK_PRIORITY_SEARCH:
        //            case ResourceFlowMode.STAGE_STACK_FLOW:
        //            case ResourceFlowMode.STAGE_STACK_FLOW_BALANCE:
        //                visited.Clear();

        //                if (log != null) log.Append("Find ", ResourceContainer.GetResourceName(type), " sources for ", partSim.name)
        //                                    .AppendLine(":", partSim.partId);

        //                partSim.GetSourceSet(type, true, allParts, visited, sourcePartSet, log, "");
        //                break;

        //            default:
        //                if (log != null) log.Append("SetResourceDrains(", partSim.name, ":", partSim.partId)
        //                                    .AppendLine(") Unexpected flow type for ", ResourceContainer.GetResourceName(type), ")");
        //                break;
        //        }

        //        if (log != null && sourcePartSet.Count > 0)
        //        {
        //            log.AppendLine("Source parts for ", ResourceContainer.GetResourceName(type), ":");
        //            foreach (PartSim partSim in sourcePartSet)
        //            {
        //                log.AppendLine(partSim.name, ":", partSim.partId);
        //            }
        //        }

        //        //DumpSourcePartSets(log, "after " + ResourceContainer.GetResourceName(type));
        //    }

        //    // If we don't have sources for all the needed resources then return false without setting up any drains
        //    for (int i = 0; i < this.resourceConsumptions.Types.Count; i++)
        //    {
        //        int type = this.resourceConsumptions.Types[i];
        //        HashSet<PartSim> sourcePartSet; 
        //        if (!sourcePartSets.TryGetValue(type, out sourcePartSet) || sourcePartSet.Count == 0)
        //        {
        //            if (log != null) log.AppendLine("No source of ", ResourceContainer.GetResourceName(type));
        //            isActive = false;
        //            return false;
        //        }
        //    }

        //    // Now we set the drains on the members of the sets and update the draining parts set
        //    for (int i = 0; i < this.resourceConsumptions.Types.Count; i++)
        //    {
        //        int type = this.resourceConsumptions.Types[i];
        //        HashSet<PartSim> sourcePartSet = sourcePartSets[type];
        //        ResourceFlowMode mode = (ResourceFlowMode)resourceFlowModes[type];
        //        double consumption = resourceConsumptions[type];
        //        double amount = 0d;
        //        double total = 0d;
        //        if (mode == ResourceFlowMode.ALL_VESSEL_BALANCE ||
        //            mode == ResourceFlowMode.STAGE_PRIORITY_FLOW_BALANCE ||
        //            mode == ResourceFlowMode.STAGE_STACK_FLOW_BALANCE ||
        //            mode == ResourceFlowMode.STACK_PRIORITY_SEARCH)
        //        {
        //            foreach (PartSim partSim in sourcePartSet)
        //                total += partSim.resources[type];
        //        }
        //        else
        //            amount = consumption / sourcePartSet.Count;

        //        // Loop through the members of the set 
        //        foreach (PartSim partSim in sourcePartSet)
        //        {
        //            if (total != 0d)
        //                amount = consumption * partSim.resources[type] / total;

        //            if (log != null) log.Append("Adding drain of ", amount, " ", ResourceContainer.GetResourceName(type))
        //                                .AppendLine(" to ", partSim.name, ":", partSim.partId);

        //            partSim.resourceDrains.Add(type, amount);
        //        }
        //    }
        //    return true;
        //}

    }
}
