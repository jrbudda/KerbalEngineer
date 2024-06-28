// 
//     Kerbal Engineer Redux
// 
// Extension methods are bad

namespace KerbalEngineer.Extensions
{
    using System;
    using System.Collections.Generic;
    using CompoundParts;

    public static class PartExtensions
    {
        //private static Part cachePart;
        //private static PartModule cachePartModule;
        //private static PartResource cachePartResource;

        /// <summary>
        ///     Gets whether the part contains a specific resource.
        /// </summary>
        public static bool ContainsResource(Part part, int resourceId)
        {
            return part.Resources.Contains(resourceId);
        }

        /// <summary>
        ///     Gets whether the part contains resources.
        /// </summary>
        public static bool ContainsResources(Part part)
        {
            for (int i = 0; i < part.Resources.dict.Count; ++i)
            {
                if (part.Resources.dict.At(i).amount > 0.0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Gets whether the part has fuel.
        /// </summary>
        /* not used
        public static bool EngineHasFuel(Part part)
        {
            PartModule cachePartModule = GetModule<ModuleEngines>(part);
            if (cachePartModule != null)
            {
                return (cachePartModule as ModuleEngines).getFlameoutState;
            }

            cachePartModule = GetModuleMultiModeEngine(part);
            if (cachePartModule != null)
            {
                return (cachePartModule as ModuleEnginesFX).getFlameoutState;
            }

            return false;
        }
        */
        /// <summary>
        ///     Gets the cost of the part excluding resources.
        /// </summary>
        public static double GetCostDry(Part part)
        {
            return part.partInfo.cost - GetResourceCostMax(part) + part.GetModuleCosts(0.0f);
        }

        /// <summary>
        ///     Gets the cost of the part including maximum resources.
        /// </summary>
        public static double GetCostMax(Part part)
        {
            return part.partInfo.cost + part.GetModuleCosts(0.0f);
        }

        /// <summary>
        ///     Gets the cost of the part modules
        ///     Same as stock but without mem allocation
        /// </summary>
        public static double GetModuleCostsNoAlloc(Part part, float defaultCost)
        {
            float cost = 0f;
            for (int i = 0; i < part.Modules.Count; i++)
            {
                PartModule pm = part.Modules[i];
                if (pm is IPartCostModifier)
                    cost += (pm as IPartCostModifier).GetModuleCost(defaultCost, ModifierStagingSituation.CURRENT);
            }
            return cost;
        }

        /// <summary>
        ///     Gets the cost of the part including resources.
        /// </summary>
        public static double GetCostWet(Part part)
        {
            return part.partInfo.cost - GetResourceCostInverted(part) + PartExtensions.GetModuleCostsNoAlloc(part,0.0f); // part.GetModuleCosts allocate 44B per call. 
        }

        /// <summary>
        ///     Gets the dry mass of the part.
        /// </summary>
        public static double GetDryMass(Part part)
        {
            return (part.physicalSignificance == Part.PhysicalSignificance.FULL) ? part.mass + PartExtensions.getCrewAdjustment(part) : 0d;
        }

        public static double getCrewAdjustment(Part part)
        {
            //if (HighLogic.LoadedSceneIsEditor && PhysicsGlobals.KerbalCrewMass != 0 && ShipConstruction.ShipManifest != null)
            //{ //fix weird stock behavior with physics setting.

            //    var crewlist = ShipConstruction.ShipManifest.GetAllCrew(false);

            //    int crew = 0;

            //    foreach (var crewmem in crewlist)
            //    {
            //        if (crewmem != null) crew++;
            //    }

            //    if (crew > 0)
            //    {
            //        var pcm = ShipConstruction.ShipManifest.GetPartCrewManifest(part.craftID);

            //        int actualCrew = 0;

            //        foreach (var crewmem in pcm.GetPartCrew())
            //        {
            //            if (crewmem != null)
            //                actualCrew++;
            //        }

            //        if (actualCrew < crew)
            //        {
            //            return -PhysicsGlobals.KerbalCrewMass * (crew - actualCrew);
            //        }

            //    }
            //}

            return 0;
        }
   

        /// <summary>
        ///     Gets the maximum thrust of the part if it's an engine.
        /// </summary>
        /* not used
        public static double GetMaxThrust(Part part)
        {
            PartModule cachePartModule = GetModule<ModuleEngines>(part);
            if (cachePartModule != null)
            {
                return (cachePartModule as ModuleEngines).maxThrust;
            }

            cachePartModule = GetModuleMultiModeEngine(part) ?? GetModule<ModuleEnginesFX>(part);
            if (cachePartModule != null)
            {
                return (cachePartModule as ModuleEnginesFX).maxThrust;
            }

            return 0.0;
        }
        */

        /// <summary>
        ///     Gets the first typed PartModule in the part's module list.
        /// </summary>
        public static T GetModule<T>(Part part) where T : PartModule
        {
            for (int i = 0; i < part.Modules.Count; i++)
            {
                PartModule pm = part.Modules[i];
                if (pm is T)
                    return (T)pm;
            }
            return null;
        }

        /// <summary>
        ///     Gets a typed PartModule.
        /// </summary>
        public static T GetModule<T>(Part part, string className) where T : PartModule
        {
            return part.Modules[className] as T;
        }

        /// <summary>
        ///     Gets a typed PartModule.
        /// </summary>
        public static T GetModule<T>(Part part, int classId) where T : PartModule
        {
            return part.Modules[classId] as T;
        }

        /// <summary>
        ///     Gets a ModuleAlternator typed PartModule.
        /// </summary>
        public static ModuleAlternator GetModuleAlternator(Part part)
        {
            return GetModule<ModuleAlternator>(part);
        }

        /// <summary>
        ///     Gets a ModuleDeployableSolarPanel typed PartModule.
        /// </summary>
        public static ModuleDeployableSolarPanel GetModuleDeployableSolarPanel(Part part)
        {
            return GetModule<ModuleDeployableSolarPanel>(part);
        }

        /// <summary>
        ///     Gets a ModuleEngines typed PartModule.
        /// </summary>
        public static ModuleEngines GetModuleEngines(Part part)
        {
            return GetModule<ModuleEngines>(part);
        }

/*        public static ModuleEnginesFX GetModuleEnginesFx(Part part)
        {
            return GetModule<ModuleEnginesFX>(part);
        }*/

        /// <summary>
        ///     Gets a ModuleGenerator typed PartModule.
        /// </summary>
        public static ModuleGenerator GetModuleGenerator(Part part)
        {
            return GetModule<ModuleGenerator>(part);
        }

        /// <summary>
        ///     Gets a ModuleGimbal typed PartModule.
        /// </summary>
        public static ModuleGimbal GetModuleGimbal(Part part)
        {
            return GetModule<ModuleGimbal>(part);
        }

        /// <summary>
        ///     Gets the current selected ModuleEnginesFX.
        /// </summary>
        public static ModuleEngines GetModuleMultiModeEngine(Part part)
        {
            ModuleEngines moduleEngines;
            MultiModeEngine multiMod = GetModule<MultiModeEngine>(part);
            if (multiMod != null)
            {
                string mode = multiMod.mode;
                for (int i = 0; i < part.Modules.Count; ++i)
                {
                    moduleEngines = part.Modules[i] as ModuleEngines;
                    if (moduleEngines != null && moduleEngines.engineID == mode)
                    {
                        return moduleEngines;
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///     Gets a ModuleParachute typed PartModule.
        /// </summary>
        public static ModuleParachute GetModuleParachute(Part part)
        {
            return GetModule<ModuleParachute>(part);
        }

        public static ModuleRCS GetModuleRcs(Part part)
        {
            return GetModule<ModuleRCS>(part);
        }

        /// <summary>
        ///     Gets a typed list of PartModules.
        /// </summary>
        public static List<T> GetModules<T>(Part part) where T : PartModule
        {
            List<T> list = new List<T>();
            for (int i = 0; i < part.Modules.Count; ++i)
            {
                T module = part.Modules[i] as T;
                if (module != null)
                {
                    list.Add(module);
                }
            }
            return list;
        }

        public static ProtoModuleDecoupler GetProtoModuleDecoupler(Part part)
        {
            PartModule cachePartModule = GetModule<ModuleDecouple>(part);
            if (cachePartModule == null)
            {
                cachePartModule = GetModule<ModuleAnchoredDecoupler>(part);
            }
            if (cachePartModule != null)
            {
                return new ProtoModuleDecoupler(cachePartModule);
            }

            return null;
        }

        /// <summary>
        ///     Gets a generic proto engine for the current engine module attached to the part.
        /// </summary>
        public static ProtoModuleEngine GetProtoModuleEngine(Part part)
        {
            PartModule cachePartModule = GetModule<ModuleEngines>(part);
            if (cachePartModule != null)
            {
                return new ProtoModuleEngine(cachePartModule);
            }

            cachePartModule = GetModuleMultiModeEngine(part) ?? GetModule<ModuleEnginesFX>(part);
            if (cachePartModule != null)
            {
                return new ProtoModuleEngine(cachePartModule);
            }

            return null;
        }

        /// <summary>
        ///     Gets the cost of the part's contained resources.
        /// </summary>
        public static double GetResourceCost(Part part)
        {
            double cost = 0.0;
            for (int i = 0; i < part.Resources.dict.Count; ++i)
            {
                PartResource cachePartResource = part.Resources.dict.At(i);
                cost = cost + (cachePartResource.amount * cachePartResource.info.unitCost);
            }
            return cost;
        }

        /// <summary>
        ///     Gets the cost of the part's contained resources, inverted.
        /// </summary>
        public static double GetResourceCostInverted(Part part)
        {
            double sum = 0;
            for (int i = 0; i < part.Resources.dict.Count; i++)
            {
                PartResource r = part.Resources.dict.At(i);
                sum += (r.maxAmount - r.amount) * r.info.unitCost;
            }
            return sum;
        }

        /// <summary>
        ///     Gets the cost of the part's maximum contained resources.
        /// </summary>
        public static double GetResourceCostMax(Part part)
        {
            double cost = 0.0;
            for (int i = 0; i < part.Resources.dict.Count; ++i)
            {
                PartResource cachePartResource = part.Resources.dict.At(i);
                cost = cost + (cachePartResource.maxAmount * cachePartResource.info.unitCost);
            }
            return cost;
        }

        /// <summary>
        ///     Gets the current specific impulse for the engine.
        /// </summary>
        /* not used
        public static double GetSpecificImpulse(Part part, float atmosphere)
        {
            PartModule cachePartModule = GetModule<ModuleEngines>(part);
            if (cachePartModule != null)
            {
                return (cachePartModule as ModuleEngines).atmosphereCurve.Evaluate(atmosphere);
            }

            cachePartModule = GetModuleMultiModeEngine(part) ?? GetModule<ModuleEnginesFX>(part);
            if (cachePartModule != null)
            {
                return (cachePartModule as ModuleEnginesFX).atmosphereCurve.Evaluate(atmosphere);
            }

            return 0.0;
        }
        */

        /// <summary>
        ///     Gets the total mass of the part including resources.
        /// </summary>
        public static double GetWetMass(Part part)
        {
            return (part.physicalSignificance == Part.PhysicalSignificance.FULL) ? part.mass + part.GetResourceMass() + getCrewAdjustment(part) : part.GetResourceMass();
        }

        /// <summary>
        ///     Gets whether the part contains a PartModule.
        /// </summary>
        public static bool HasModule<T>(Part part) where T : PartModule
        {
            for (int i = 0; i < part.Modules.Count; i++)
            {
                if (part.Modules[i] is T)
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     Gets whether the part contains a PartModule conforming to the supplied predicate.
        /// </summary>
        public static bool HasModule<T>(Part part, Func<T, bool> predicate) where T : PartModule
        {
            for (int i = 0; i < part.Modules.Count; i++)
            {
                PartModule pm = part.Modules[i];
                if (pm is T && predicate(pm as T))
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     Gets whether the part contains a PartModule.
        /// </summary>
        public static bool HasModule(Part part, string className)
        {
            return part.Modules.Contains(className);
        }

        /// <summary>
        ///     Gets whether the part contains a PartModule.
        /// </summary>
        public static bool HasModule(Part part, int moduleId)
        {
            return part.Modules.Contains(moduleId);
        }

        /// <summary>
        ///     Gets whether the part has a one shot animation.
        /// </summary>
        public static bool HasOneShotAnimation(Part part)
        {
            PartModule cachePartModule = GetModule<ModuleAnimateGeneric>(part);
            return cachePartModule != null && (cachePartModule as ModuleAnimateGeneric).isOneShot;
        }

        /// <summary>
        ///     Gets whether the part is a command module.
        /// </summary>
        public static bool IsCommandModule(Part part)
        {
            return HasModule<ModuleCommand>(part);
        }

        /// <summary>
        ///     Gets whether the part is decoupled in a specified stage.
        /// </summary>
        public static bool IsDecoupledInStage(Part part, int stage)
        {
            if ((IsDecoupler(part) || IsLaunchClamp(part)) && part.inverseStage == stage)
            {
                return true;
            }
            if (part.parent == null)
            {
                return false;
            }
            return IsDecoupledInStage(part.parent, stage);
        }

        /// <summary>
        ///     Gets whether the part is a decoupler.
        /// </summary>
        public static bool IsDecoupler(Part part)
        {
            return HasModule<ModuleDecouple>(part) || HasModule<ModuleAnchoredDecoupler>(part);
        }

        /// <summary>
        ///     Gets whether the part is an active engine.
        /// </summary>
        public static bool IsEngine(Part part)
        {
            return HasModule<ModuleEngines>(part);
        }

        /// <summary>
        ///     Gets whether the part is a fuel line.
        /// </summary>
        public static bool IsFuelLine(Part part)
        {
            return HasModule<CModuleFuelLine>(part);
        }

        /// <summary>
        ///     Gets whether the part is a generator.
        /// </summary>
        public static bool IsGenerator(Part part)
        {
            return HasModule<ModuleGenerator>(part);
        }

        /// <summary>
        ///     Gets whether the part is a launch clamp.
        /// </summary>
        public static bool IsLaunchClamp(Part part)
        {
            return HasModule<LaunchClamp>(part);
        }

        /// <summary>
        ///     Gets whether the part is a parachute.
        /// </summary>
        public static bool IsParachute(Part part)
        {
            return HasModule<ModuleParachute>(part);
        }

        /// <summary>
        ///     Gets whether the part is considered a primary part on the vessel.
        /// </summary>
        public static bool IsPrimary(Part part, List<Part> partsList, PartModule module)
        {
            for (int i = 0; i < partsList.Count; i++)
            {
                var vesselPart = partsList[i];
                if (!HasModule(vesselPart, module.ClassID))
                {
                    continue;
                }
                if (vesselPart == part)
                {
                    return true;
                }
                break;
            }

            return false;
        }

        public static bool IsRcsModule(Part part)
        {
            return HasModule<ModuleRCS>(part);
        }

        /// <summary>
        ///     Gets whether the part is a sepratron.
        /// </summary>
        public static bool IsSepratron(Part part)
        {
            for (int i = 0; i < part.Modules.Count; i++)
            {
                if (part.Modules[i] is ModuleEngines)
                {
                    if ((part.Modules[i] as ModuleEngines).throttleLocked)
                        return true;
                }
            }
            return false;
        }

        public static bool ContainedPart(Part part, List<Part> chain)
        {
            for (int i = 0; i < chain.Count; i++)
            {
                if (chain[i] == part)
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Gets whether the part is a deployable solar panel.
        /// </summary>
        public static bool IsSolarPanel(Part part)
        {
            return HasModule<ModuleDeployableSolarPanel>(part);
        }

        /// <summary>
        ///     Gets whether the part is a solid rocket motor.
        /// </summary>
        public static bool IsSolidRocket(Part part)
        {
            return (PartExtensions.HasModule<ModuleEngines>(part) && PartExtensions.GetModuleEngines(part).throttleLocked);
        }

        public class ProtoModuleDecoupler
        {
            private readonly PartModule module;

            public ProtoModuleDecoupler(PartModule module)
            {
                this.module = module;

                if (this.module is ModuleDecouple)
                {
                    SetModuleDecouple();
                }
                else if (this.module is ModuleAnchoredDecoupler)
                {
                    SetModuleAnchoredDecoupler();
                }
            }

            public double EjectionForce { get; private set; }
            public bool IsOmniDecoupler { get; private set; }
            public bool IsStageEnabled { get; private set; }

            private void SetModuleAnchoredDecoupler()
            {
                ModuleAnchoredDecoupler decoupler = module as ModuleAnchoredDecoupler;
                if (decoupler == null)
                {
                    return;
                }

                EjectionForce = decoupler.ejectionForce;
                IsStageEnabled = decoupler.stagingEnabled;
            }

            private void SetModuleDecouple()
            {
                ModuleDecouple decoupler = module as ModuleDecouple;
                if (decoupler == null)
                {
                    return;
                }

                EjectionForce = decoupler.ejectionForce;
                IsOmniDecoupler = decoupler.isOmniDecoupler;
                IsStageEnabled = decoupler.stagingEnabled;
            }
        }

        // needs updating to handle multi-mode engines and engines with multiple ModuleEngines correctly.
        // It currently just shows the stats of the currently active module for multi-mode engines and just 
        // the first ModuleEngines for engines with multiple modules.
        // It should really show all the modes for multi-mode engines as separate sections.
        // For other engines with multiple ModuleEngines it should combine the separate modules into a single set of data
        // The constructor should be changed to take the Part itself.  It can be called if HasModule<ModuleEngines>() is true.
        public class ProtoModuleEngine
        {
            private readonly PartModule module;

            public ProtoModuleEngine(PartModule module)
            {
                this.module = module;

                if (module is ModuleEngines)
                {
                    SetModuleEngines();
                }
            }

            public double MaximumThrust { get; private set; }
            public double MinimumThrust { get; private set; }
            public List<Propellant> Propellants { get; private set; }

            public float GetSpecificImpulse(float atmosphere)
            {
                if (module is ModuleEngines)
                {
                    return (module as ModuleEngines).atmosphereCurve.Evaluate(atmosphere);
                }
                return 0.0f;
            }

            private void SetModuleEngines()
            {
                ModuleEngines engine = module as ModuleEngines;
                if (engine == null)
                {
                    return;
                }

                MaximumThrust = engine.MaxThrustOutputVac();
                MinimumThrust = engine.MaxThrustOutputAtm();
                Propellants = engine.propellants;
            }
        }
    }
}