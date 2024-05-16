// Copyright (C) 2015 CYBUTEK
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU
// General Public License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program. If not,
// see <http://www.gnu.org/licenses/>.

namespace KerbalEngineer.Flight.Readouts {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Miscellaneous;
    using Orbital;
    using Orbital.ManoeuvreNode;
    using Rendezvous;
    using Settings;
    using Surface;
    using Thermal;
    using Vessel;
    using Body;
    using AltitudeSeaLevel = Surface.AltitudeSeaLevel;
    using ApoapsisHeight = Orbital.ApoapsisHeight;
    using OrbitalPeriod = Orbital.OrbitalPeriod;
    using PeriapsisHeight = Orbital.PeriapsisHeight;
    using SemiMajorAxis = Orbital.SemiMajorAxis;
    using SemiMinorAxis = Orbital.SemiMinorAxis;
    using TimeToApoapsis = Orbital.TimeToApoapsis;
    using TimeToPeriapsis = Orbital.TimeToPeriapsis;
    using Sections;

    public static class ReadoutLibrary {
        private static List<ReadoutModule> readouts = new List<ReadoutModule>();

        /// <summary>
        /// Sets up and populates the readout library with the stock readouts.
        /// </summary>
        static ReadoutLibrary() {
            try {
                ReadoutCategory.SetCategory("Orbital", "Readout for orbital manovoeures.");
                ReadoutCategory.SetCategory("Surface", "Surface and atmospheric readouts.");
                ReadoutCategory.SetCategory("Vessel", "Vessel performance statistics.");
                ReadoutCategory.SetCategory("Rendezvous", "Readouts for rendezvous manovoeures.");
                ReadoutCategory.SetCategory("Thermal", "Thermal characteristics readouts.");
                ReadoutCategory.SetCategory("Body", "Characteristics of the current SOI.");
                ReadoutCategory.SetCategory("Miscellaneous", "Miscellaneous readouts.");
                ReadoutCategory.Selected = ReadoutCategory.GetCategory("Orbital");

                // Orbital
                readouts.Add(new ApoapsisHeight());
                readouts.Add(new PeriapsisHeight());
                readouts.Add(new TimeToApoapsis());
                readouts.Add(new TimeToPeriapsis());
                readouts.Add(new Inclination());
                readouts.Add(new TimeToEquatorialAscendingNode());
                readouts.Add(new TimeToEquatorialDescendingNode());
                readouts.Add(new AngleToEquatorialAscendingNode());
                readouts.Add(new AngleToEquatorialDescendingNode());
                readouts.Add(new Eccentricity());
                readouts.Add(new OrbitalSpeed());
                readouts.Add(new OrbitalPeriod());
                readouts.Add(new LongitudeOfAscendingNode());
                readouts.Add(new LongitudeOfPeriapsis());
                readouts.Add(new ArgumentOfPeriapsis());
                readouts.Add(new TrueAnomaly());
                readouts.Add(new MeanAnomaly());
                readouts.Add(new MeanAnomalyAtEpoc());
                readouts.Add(new EccentricAnomaly());
                readouts.Add(new SemiMajorAxis());
                readouts.Add(new SemiMinorAxis());
                readouts.Add(new AngleToPrograde());
                readouts.Add(new AngleToRetrograde());
                readouts.Add(new NodeProgradeDeltaV());
                readouts.Add(new NodeNormalDeltaV());
                readouts.Add(new NodeRadialDeltaV());
                readouts.Add(new NodeTotalDeltaV());
                readouts.Add(new NodeBurnTime());
                readouts.Add(new NodeHalfBurnTime());
                readouts.Add(new NodeTimeToManoeuvre());
                readouts.Add(new NodeTimeToHalfBurn());
                readouts.Add(new NodeAngleToPrograde());
                readouts.Add(new NodeAngleToRetrograde());
                readouts.Add(new PostBurnApoapsis());
                readouts.Add(new PostBurnPeriapsis());
                readouts.Add(new PostBurnInclination());
                readouts.Add(new PostBurnRealtiveInclination());
                readouts.Add(new PostBurnPeriod());
                readouts.Add(new PostBurnEccentricity());
                readouts.Add(new SpeedAtApoapsis());
                readouts.Add(new SpeedAtPeriapsis());
                readouts.Add(new TimeToAtmosphere());
                readouts.Add(new TripTotalDeltaV());

                // Surface
                readouts.Add(new AltitudeSeaLevel());
                readouts.Add(new AltitudeTerrain());
                readouts.Add(new VerticalSpeed());
                readouts.Add(new VerticalAcceleration());
                readouts.Add(new HorizontalSpeed());
                readouts.Add(new HorizontalAcceleration());
                readouts.Add(new MachNumber());
                readouts.Add(new Latitude());
                readouts.Add(new Longitude());
                readouts.Add(new GeeForce());
                readouts.Add(new TerminalVelocity());
                readouts.Add(new AtmosphericEfficiency());
                readouts.Add(new AtmosphericPressure());
                readouts.Add(new Biome());
                readouts.Add(new Situation());
                readouts.Add(new Slope());
                readouts.Add(new ImpactTime());
                readouts.Add(new ImpactLatitude());
                readouts.Add(new ImpactLongitude());
                readouts.Add(new ImpactMarker());
                readouts.Add(new ImpactAltitude());
                readouts.Add(new ImpactBiome());

                // Vessel
                readouts.Add(new Name());
                readouts.Add(new DeltaVStaged());
                readouts.Add(new DeltaVCurrent());
                readouts.Add(new DeltaVTotal());
                readouts.Add(new DeltaVCurrentTotal());
                readouts.Add(new SpecificImpulse());
                readouts.Add(new Mass());
                readouts.Add(new Thrust());
                readouts.Add(new ThrustToWeight());
                readouts.Add(new ThrustOffsetAngle());
                readouts.Add(new ThrustTorque());
                readouts.Add(new SurfaceThrustToWeight());
                readouts.Add(new Gravity());
                readouts.Add(new Acceleration());
                readouts.Add(new SuicideBurnAltitude());
                readouts.Add(new SuicideBurnDistance());
                readouts.Add(new SuicideBurnDeltaV());
                readouts.Add(new SuicideBurnCountdown());
                readouts.Add(new SuicideBurnLength());
                readouts.Add(new IntakeAirUsage());
                readouts.Add(new IntakeAirDemand());
                readouts.Add(new IntakeAirSupply());
                readouts.Add(new IntakeAirDemandSupply());
                readouts.Add(new PartCount());
                readouts.Add(new Throttle());
                readouts.Add(new Heading());
                readouts.Add(new Pitch());
                readouts.Add(new Roll());
                readouts.Add(new HeadingRate());
                readouts.Add(new PitchRate());
                readouts.Add(new RollRate());
                readouts.Add(new RCSDeltaV());
                readouts.Add(new RCSIsp());
                readouts.Add(new RCSThrust());
                readouts.Add(new RCSTWR());

                // Rendezvous
                readouts.Add(new TargetSelector());
                readouts.Add(new PhaseAngle());
                readouts.Add(new InterceptAngle());
                readouts.Add(new TimeToTransferAngleTime());
                readouts.Add(new RelativeVelocity());
                readouts.Add(new RelativeSpeed());
                readouts.Add(new RelativeInclination());
                readouts.Add(new TimeToRelativeAscendingNode());
                readouts.Add(new TimeToRelativeDescendingNode());
                readouts.Add(new AngleToRelativeAscendingNode());
                readouts.Add(new AngleToRelativeDescendingNode());
                readouts.Add(new Rendezvous.AltitudeSeaLevel());
                readouts.Add(new Rendezvous.ApoapsisHeight());
                readouts.Add(new Rendezvous.PeriapsisHeight());
                readouts.Add(new Rendezvous.TimeToApoapsis());
                readouts.Add(new Rendezvous.TimeToPeriapsis());
                readouts.Add(new Distance());
                readouts.Add(new Rendezvous.OrbitalPeriod());
                readouts.Add(new Rendezvous.SemiMajorAxis());
                readouts.Add(new Rendezvous.SemiMinorAxis());
                readouts.Add(new Rendezvous.TimeTilClosestApproach());
                readouts.Add(new Rendezvous.SeparationAtClosestApproach());
                readouts.Add(new Rendezvous.SpeedAtClosestApproach());
                readouts.Add(new TargetLatitude());
                readouts.Add(new TargetLongitude());

                // Thermal
                readouts.Add(new InternalFlux());
                readouts.Add(new ConvectionFlux());
                readouts.Add(new RadiationFlux());
                readouts.Add(new CriticalPart());
                readouts.Add(new CriticalTemperature());
                readouts.Add(new CriticalSkinTemperature());
                readouts.Add(new CriticalThermalPercentage());
                readouts.Add(new HottestPart());
                readouts.Add(new HottestTemperature());
                readouts.Add(new HottestSkinTemperature());
                readouts.Add(new CoolestPart());
                readouts.Add(new CoolestTemperature());
                readouts.Add(new CoolestSkinTemperature());

                // Body
                readouts.Add(new BodyName());
                readouts.Add(new HasAtmosphere());
                readouts.Add(new HasOxygen());
                readouts.Add(new MinOrbitHeight());
                readouts.Add(new HighAtmosphereHeight());
                readouts.Add(new LowSpaceHeight());
                readouts.Add(new HighSpaceHeight());
                readouts.Add(new GeostationaryHeight());
                readouts.Add(new CurrentSoi());
                readouts.Add(new BodyRotationPeriod());
                readouts.Add(new BodyOrbitalPeriod());
                readouts.Add(new EscapeVelocity());
                readouts.Add(new BodyMass());
                readouts.Add(new BodyRadius());
                readouts.Add(new BodyGravity());

                // Misc
                readouts.Add(new Separator());
                readouts.Add(new ClearSeparator());
                readouts.Add(new Crosshair());
                readouts.Add(new GuiSizeAdjustor());
                readouts.Add(new SimulationDelay());
                readouts.Add(new VectoredThrustToggle());
                readouts.Add(new SystemTime());
                readouts.Add(new SystemTime24());
                readouts.Add(new SystemDateTime());
                readouts.Add(new LogSimToggle());

                LoadHelpStrings();
                LoadReadoutConfig();
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        /// <summary>
        /// Gets and sets the available readout modules.
        /// </summary>
        public static List<ReadoutModule> Readouts {
            get {
                return readouts;
            }
            set {
                readouts = value;
            }
        }

        /// <summary>
        /// Gets a list of readout modules which are associated with the specified category.
        /// </summary>
        public static List<ReadoutModule> GetCategory(ReadoutCategory category) {
            return readouts.Where(r => r.Category == category).ToList();
        }

        /// <summary>
        /// Gets a readout module with the specified name or class name. (Returns null if not found.)
        /// </summary>
        public static ReadoutModule GetReadout(string name) {
            return readouts.FirstOrDefault(r => r.Name == name || r.GetType().Name == name || r.Category + "." + r.GetType().Name == name);
        }

        /// <summary>
        /// Resets all the readout modules.
        /// </summary>
        public static void Reset() {
            foreach (ReadoutModule readout in readouts) {
                readout.Reset();
            }
        }

        /// <summary>
        /// Loads the help strings from file.
        /// </summary>
        private static void LoadHelpStrings() {
            try {
                SettingHandler handler = SettingHandler.Load("HelpStrings.xml");
                foreach (ReadoutModule readout in readouts) {
                    readout.HelpString = handler.GetSet(readout.Category + "." + readout.GetType().Name, readout.HelpString);
                }
                handler.Save("HelpStrings.xml");
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        /// <summary>
        /// Loads config
        /// </summary>
        private static void LoadReadoutConfig() {
            try {
                SettingHandler handler = SettingHandler.Load("ReadoutsConfig.xml", new Type[] { typeof(ReadoutModuleConfigNode)});
                foreach (ReadoutModule readout in readouts) {
                    ReadoutModuleConfigNode r = handler.Get<ReadoutModuleConfigNode>(readout.Name, null);
                    if (r != null) {
                        readout.ValueStyle.normal.textColor = r.TextColor;
                        readout.DecimalPlaces = r.DecimalPlaces;
                        readout.HudDecimalPlaces = r.HudDecimalPlaces;
                        readout.CharacterLimit = r.CharacterLimit;
                        readout.HudCharacterLimit = r.HudCharacterLimit;
                    }
                }
                handler.Save("ReadoutsConfig.xml");
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        public static void RemoveReadoutConfig(ReadoutModule readout) {
            try {
                SettingHandler handler = SettingHandler.Load("ReadoutsConfig.xml", new Type[] { typeof(ReadoutModuleConfigNode)});
                var r = handler.Get<ReadoutModuleConfigNode>(readout.Name, null);

                if (r == null) {
                    return;
                }

                handler.Items.Remove(handler.Items.Find(i => i.Name == readout.Name));

                handler.Save("ReadoutsConfig.xml");
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }


        public static void SaveReadoutConfig(ReadoutModule readout) {
            try {
                SettingHandler handler = SettingHandler.Load("ReadoutsConfig.xml", new Type[] { typeof(ReadoutModuleConfigNode)});
                var r = handler.Get<ReadoutModuleConfigNode>(readout.Name, null);

                if (r == null) {
                    r = new ReadoutModuleConfigNode();
                }

                r.Name = readout.Name;
                r.TextColor = readout.ValueStyle.normal.textColor;
                r.DecimalPlaces = readout.DecimalPlaces;
                r.HudDecimalPlaces = readout.HudDecimalPlaces;
                r.CharacterLimit = readout.CharacterLimit;
                r.HudCharacterLimit = readout.HudCharacterLimit;

                handler.Set(r.Name, r);
                handler.Save("ReadoutsConfig.xml");
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

    }
}