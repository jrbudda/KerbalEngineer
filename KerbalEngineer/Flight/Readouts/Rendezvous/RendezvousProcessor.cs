// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2015 CYBUTEK
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

namespace KerbalEngineer.Flight.Readouts.Rendezvous {
    using System;
    using Extensions;
    using Helpers;
    using UnityEngine;
    using KSP.Localization;

    public class RendezvousProcessor : IUpdatable, IUpdateRequest {
        private static readonly RendezvousProcessor instance = new RendezvousProcessor();

        /// <summary>
        ///     Gets the target's altitude above its reference body.
        /// </summary>
        public static double AltitudeSeaLevel { get; private set; }

        /// <summary>
        ///     Gets the angle from the origin position to the ascending node.
        /// </summary>
        public static double AngleToAscendingNode { get; private set; }

        /// <summary>
        ///     Gets the angle from the origin position to the descending node.
        /// </summary>
        public static double AngleToDescendingNode { get; private set; }

        /// <summary>
        ///     Gets the target's apoapsis above its reference body.
        /// </summary>
        public static double ApoapsisHeight { get; private set; }

        /// <summary>
        ///     Gets the distance from the origin position to the target position.
        /// </summary>
        public static double Distance { get; private set; }

        /// <summary>
        ///     Gets the current instance of the rendezvous processor.
        /// </summary>
        public static RendezvousProcessor Instance {
            get {
                return instance;
            }
        }

        /// <summary>
        ///     Gets the difference in angle from the origin position to where it is most efficient to burn for an encounter.
        /// </summary>
        public static double InterceptAngle { get; private set; }

        /// <summary>
        ///     Gets the orbital period of the target orbit.
        /// </summary>
        public static double OrbitalPeriod { get; private set; }

        /// <summary>
        ///     Gets the target's periapsis above its reference body.
        /// </summary>
        public static double PeriapsisHeight { get; private set; }

        /// <summary>
        ///     Gets the difference in angle from the origin position to the target position based on a common reference.
        /// </summary>
        public static double PhaseAngle { get; private set; }

        /// <summary>
        ///     Gets the phase angle for a Hohmann transfer.
        /// </summary>
        public static double TransferAngle { get; private set; }

        /// <summary>
        ///     Gets the time until the transfer angle is the phase angle
        /// </summary>
        public static double TimeToTransferAngle { get; private set; }


        /// <summary>
        ///     Gets the angular difference between the origin and target orbits.
        /// </summary>
        public static double RelativeInclination { get; private set; }

        /// <summary>
        ///     Gets the relative orbital speed between the vessel and target.
        /// </summary>
        public static double RelativeSpeed { get; private set; }

        /// <summary>
        ///     Gets the relative orbital velocity between the vessel and target.
        /// </summary>
        public static double RelativeVelocity { get; private set; }

        /// <summary>
        ///     Gets the semi-major axis of the target orbit.
        /// </summary>
        public static double SemiMajorAxis { get; private set; }

        /// <summary>
        ///     Gets the semi-minor axis of the target orbit.
        /// </summary>
        public static double SemiMinorAxis { get; private set; }

        /// <summary>
        ///     Gets whether the details are ready to be shown.
        /// </summary>
        public static bool ShowDetails { get; private set; }

        /// <summary>
        ///     Gets the target's time to apoapsis.
        /// </summary>
        public static double TimeToApoapsis { get; private set; }

        /// <summary>
        ///     Gets the time it will take to reach the ascending node.
        /// </summary>
        public static double TimeToAscendingNode { get; private set; }

        /// <summary>
        ///     Gets the time it will take to reach the descending node.
        /// </summary>
        public static double TimeToDescendingNode { get; private set; }

        /// <summary>
        ///     Gets the target's time to periapsis.
        /// </summary>
        public static double TimeToPeriapsis { get; private set; }

        /// <summary>
        ///     Gets time for landed ship to intersect target orbital plane in system.
        /// </summary>
        public static double[] TimeToPlane { get; private set; } = new double[2];

        /// <summary>
        ///     Gets angle for landed ship to intersect target orbital plane in system.
        /// </summary>
        public static double[] AngleToPlane { get; private set; } = new double[2];


        /// <summary>
        ///     If the target body is ascending at the point of planar intersect.
        /// </summary>
        public static bool TimeToPlaneisAsc { get; private set; }

        /// <summary>
        ///     Is the Ship landed.
        /// </summary>
        public static bool isLanded { get; private set; }

        /// <summary>
        ///     Is the Ship and target landed on same body.
        /// </summary>
        public static bool landedSamePlanet { get; private set; }

        /// <summary>
        ///    The current ship's reference body rotation period.
        /// </summary>
        public static double bodyRotationPeriod { get; private set; }

        /// <summary>
        ///    The display string of what's actually being calculated.
        /// </summary>
        public static string targetDisplay { get; private set; }

        /// <summary>
        ///    The display string of what's actually being calculated.
        /// </summary>
        public static string sourceDisplay { get; private set; }


        /// <summary>
        ///     Gets the time of the next closest approach.
        /// </summary>
        public static double TimeTilEncounter { get; private set; }

        /// <summary>
        ///     Gets the distance at next closest approach
        /// </summary>
        public static double SeparationAtEncounter { get; private set; }

        /// <summary>
        ///     Gets the relative speed at next closest approach
        /// </summary>
        public static double SpeedAtEncounter { get; private set; }

        /// <summary>
        ///     Gets and sets whether the updatable object should be updated.
        /// </summary>
        public bool UpdateRequested { get; set; }

        /// <summary>
        ///     Request and update to calculate the details.
        /// </summary>
        public static void RequestUpdate() {
            instance.UpdateRequested = true;
        }


        public static bool overrideANDN = false;

        public static bool overrideANDNRev = false;

        public static global::Vessel targetVessel = null;
        public static global::Vessel activeVessel = null;


        public static ITargetable TrackingStationSource = null;

        public static ITargetable activeTarget = null;


        /// <summary>
        ///     Updates the details by recalculating if requested.
        /// </summary>
        public void Update() {

            ITargetable target = null;
            global::Vessel vessel = null;
            landedSamePlanet = false;
            isLanded = false;
            overrideANDN = false;
            overrideANDNRev = false;
            
            if (HighLogic.LoadedSceneIsFlight) {
                if (FlightGlobals.fetch == null ||
                    FlightGlobals.fetch.VesselTarget == null ||
                    FlightGlobals.ActiveVessel == null ||
                    FlightGlobals.ActiveVessel.targetObject == null ||
                    FlightGlobals.ActiveVessel.targetObject.GetOrbit() == null ||
                    FlightGlobals.ship_orbit == null ||
                    FlightGlobals.ship_orbit.referenceBody == null) {
                    ShowDetails = false;
                    return;
                } else {
                    target = FlightGlobals.ActiveVessel.targetObject;
                    vessel = FlightGlobals.ActiveVessel;
                }
                TrackingStationSource = null;
            } else if (HighLogic.LoadedScene == GameScenes.TRACKSTATION) {

                if (PlanetariumCamera.fetch.target.type == MapObject.ObjectType.CelestialBody)
                    target = PlanetariumCamera.fetch.target.celestialBody;
                else if (PlanetariumCamera.fetch.target.type == MapObject.ObjectType.Vessel)
                    target = PlanetariumCamera.fetch.target.vessel;

                if (TrackingStationSource != null)
                    vessel = TrackingStationSource.GetVessel();
                else
                    TrackingStationSource = target;
            }

            activeTarget = target;

            if (target == null) {
                ShowDetails = false;
                return;
            }

            ShowDetails = true;

            var actualSourceOrbit = vessel != null ? vessel.orbit : TrackingStationSource.GetOrbit();
            var actualTargetOrbit = target.GetOrbit();

            if (target is global::Vessel)
                targetVessel = (global::Vessel)target;
            else
                targetVessel = null;

            activeVessel = vessel;

            if (actualSourceOrbit == null) {
                //  Debug.Log("Source orbit is null!");
                TrackingStationSource = null;
                ShowDetails = false;
                return;
            }

            if (actualTargetOrbit == null) {
                // Debug.Log("Target orbit is null!");
                ShowDetails = false;
                return;
            }

            isLanded = vessel != null && vessel.LandedOrSplashed;
            bool targetLanded = (target is global::Vessel && ((global::Vessel)target).LandedOrSplashed);
            landedSamePlanet = isLanded && targetLanded && actualSourceOrbit.referenceBody == actualTargetOrbit.referenceBody;

            var originOrbit = isLanded ? actualSourceOrbit.referenceBody.orbit : actualSourceOrbit;
            var targetOrbit = targetLanded ? actualTargetOrbit.referenceBody.orbit : actualTargetOrbit;

            if ((!isLanded && !targetLanded) || (actualSourceOrbit.referenceBody != actualTargetOrbit.referenceBody))
                findConcentricParents(ref originOrbit, ref targetOrbit);

            if (originOrbit == targetOrbit && !landedSamePlanet) {
                targetOrbit = actualTargetOrbit;
                originOrbit = null;
            }
            
            { //These are not 'rendezvous' calculations, just raw data about the target object.
                AltitudeSeaLevel = targetOrbit.altitude;
                ApoapsisHeight = targetOrbit.ApA;
                PeriapsisHeight = targetOrbit.PeA;
                TimeToApoapsis = targetOrbit.timeToAp;
                TimeToPeriapsis = targetOrbit.timeToPe;
                SemiMajorAxis = targetOrbit.semiMajorAxis;
                SemiMinorAxis = targetOrbit.semiMinorAxis;
                OrbitalPeriod = targetOrbit.period;
                RelativeInclination = targetOrbit.inclination;
            }

            { //Set everything else 0 incase some are not valid.
                TimeToAscendingNode = 0;
                TimeToDescendingNode = 0;
                AngleToAscendingNode = 0;
                AngleToDescendingNode = 0;

                RelativeVelocity = 0;
                RelativeSpeed = 0;
                PhaseAngle = 0;
                InterceptAngle = 0;
                TimeToTransferAngle = 0;

                AngleToPlane[0] = 0;
                TimeToPlane[0] = 0;
                AngleToPlane[1] = 0;
                TimeToPlane[1] = 0;
                Distance = 0;

                TimeTilEncounter = double.NaN;
                SeparationAtEncounter = double.NaN;
                SpeedAtEncounter = double.NaN;

                Orbital.ManoeuvreNode.ManoeuvreProcessor.PostBurnRelativeInclination = 0;

            }


            if (originOrbit != null) { //Actually calculating an encounter with 2 different things.

                overrideANDN = isLanded && actualSourceOrbit.referenceBody == targetOrbit.referenceBody;
                overrideANDNRev = targetLanded && !isLanded && actualTargetOrbit.referenceBody == actualSourceOrbit.referenceBody && target.GetVessel() != null;
                bodyRotationPeriod = actualSourceOrbit.referenceBody.rotationPeriod;

                if (landedSamePlanet) { //this should only occur when landed targeting something else landed on same body.

                    AltitudeSeaLevel = target.GetVessel().altitude;
                    ApoapsisHeight = 0;
                    PeriapsisHeight = 0;
                    TimeToApoapsis = 0;
                    TimeToPeriapsis = 0;
                    SemiMajorAxis = 0;
                    SemiMinorAxis = 0;
                    Distance = Vector3d.Distance(target.GetVessel().GetWorldPos3D(), vessel.GetWorldPos3D());
                    OrbitalPeriod = bodyRotationPeriod;

                } else if (overrideANDN) {//launching

                    if (vessel != null) {
                        AngleToPlane = CalcAngleToPlane(vessel.GetOrbit().referenceBody, vessel.latitude, vessel.longitude, targetOrbit);
                        TimeToPlane[0] = (AngleToPlane[0] / 360) * vessel.GetOrbit().referenceBody.rotationPeriod;
                        TimeToPlane[1] = (AngleToPlane[1] / 360) * vessel.GetOrbit().referenceBody.rotationPeriod;
                    }

                    RelativeInclination = targetOrbit.inclination;
                    PhaseAngle = OrbitExtensions.GetPhaseAngle(actualSourceOrbit,actualTargetOrbit); //this works for some reason.

                } else if (overrideANDNRev) { //landing

                    global::Vessel tgt = target.GetVessel();

                    if (vessel != null) {
                        AngleToPlane = CalcAngleToPlane(vessel.GetOrbit().referenceBody, tgt.latitude, tgt.longitude, originOrbit);
                        TimeToPlane[0] = (AngleToPlane[0] / 360) * vessel.GetOrbit().referenceBody.rotationPeriod;
                        TimeToPlane[1] = (AngleToPlane[1] / 360) * vessel.GetOrbit().referenceBody.rotationPeriod;
                    }

                    RelativeInclination = originOrbit.inclination;
                    Distance = Vector3d.Distance(target.GetVessel().GetWorldPos3D(), vessel.GetWorldPos3D());
                    AltitudeSeaLevel = tgt.altitude;
                    PhaseAngle = OrbitExtensions.GetPhaseAngle(actualSourceOrbit, actualTargetOrbit); //this works for some reason.


                } else { //standard 2 orbits

                    RelativeInclination = OrbitExtensions.GetRelativeInclination(originOrbit, targetOrbit);



                    if (FlightGlobals.ActiveVessel == null || FlightGlobals.ActiveVessel.patchedConicSolver == null ||
                        FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes == null ||
                        FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.Count == 0) {
                    } else {
                        var node = FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes[0];
                        if (node != null && node.nextPatch != null)
                            Orbital.ManoeuvreNode.ManoeuvreProcessor.PostBurnRelativeInclination = OrbitExtensions.GetRelativeInclination(node.nextPatch, FlightGlobals.ActiveVessel.targetObject.GetOrbit());
                    }



                    RelativeSpeed = originOrbit.GetRelativeVel().magnitude - target.GetObtVelocity().magnitude;
                    RelativeVelocity = (originOrbit.GetRelativeVel() - target.GetObtVelocity()).magnitude;

                    // FlightGlobals.ship_tgtVelocity = FlightGlobals.ship_obtVelocity - this.VesselTarget.GetObtVelocity();
                    // FlightGlobals.ship_tgtSpeed = FlightGlobals.ship_tgtVelocity.magnitude;

                    PhaseAngle = OrbitExtensions.GetPhaseAngle(originOrbit, targetOrbit);
                    InterceptAngle = CalcInterceptAngle(targetOrbit, originOrbit);

                    double tspd = 360 / targetOrbit.period;
                    double sspd = 360 / originOrbit.period;

                    if (PhaseAngle < 0) {
                        double diff = InterceptAngle - PhaseAngle;
                        if (diff < 0) diff += 360;
                        if (diff > 340) diff -= 360;
                        TimeToTransferAngle = sspd == tspd ? 0 : diff / (tspd - sspd);
                    } else {
                        double diff = PhaseAngle - InterceptAngle;
                        if (diff < 0) diff += 360;
                        if (diff > 340) diff -= 360;
                        TimeToTransferAngle = sspd == tspd ? 0 : diff / (sspd - tspd);
                    }


                    TimeToAscendingNode = OrbitExtensions.GetTimeToVector(originOrbit, GetAscendingNode(targetOrbit, originOrbit));
                    TimeToDescendingNode = OrbitExtensions.GetTimeToVector(originOrbit, GetDescendingNode(targetOrbit, originOrbit));
                    AngleToAscendingNode = OrbitExtensions.GetAngleToVector(originOrbit, GetAscendingNode(targetOrbit, originOrbit));
                    AngleToDescendingNode = OrbitExtensions.GetAngleToVector(originOrbit, GetDescendingNode(targetOrbit, originOrbit));

                    Distance = Vector3d.Distance(targetOrbit.pos, originOrbit.pos);


                    //From OrbitTargeter
                    double tOne = 0;
                    if (target is CelestialBody) {
                        Vector3d relativePositionAtUT = originOrbit.getRelativePositionAtUT(originOrbit.closestTgtApprUT);
                        Vector3d relativePositionAtUT2 = targetOrbit.getRelativePositionAtUT(originOrbit.closestTgtApprUT);
                        double separation = (relativePositionAtUT - relativePositionAtUT2).magnitude;
                        tOne = originOrbit.closestTgtApprUT;
                    } else {
                        double num3 = 0.0;
                        double num4 = 0.0;
                        double num5 = 0.0;
                        double eVs = 0.0;
                        double num6 = 0.0;
                        double eVs2 = 0.0;
                        int iterations = 0;
                        int num7 = Orbit.FindClosestPoints(originOrbit, targetOrbit, ref num3, ref num4, ref num5, ref eVs, ref num6, ref eVs2, 0.0001, 20, ref iterations);
                        tOne = originOrbit.StartUT + originOrbit.GetDTforTrueAnomaly(num5, 0.0);
                        double tTwo = originOrbit.StartUT + originOrbit.GetDTforTrueAnomaly(num6, 0.0);
                        if (tOne > tTwo) {
                            UtilMath.SwapValues(ref tOne, ref tTwo);
                            UtilMath.SwapValues(ref num5, ref num6);
                            UtilMath.SwapValues(ref eVs, ref eVs2);
                        }
                    }

                   if (tOne > originOrbit.StartUT) {
                        TimeTilEncounter = tOne - originOrbit.StartUT;
                        SeparationAtEncounter = (originOrbit.getPositionAtUT(tOne) - targetOrbit.getPositionAtUT(tOne)).magnitude;
                        SpeedAtEncounter = Math.Abs(originOrbit.getOrbitalSpeedAt(tOne) - targetOrbit.getOrbitalSpeedAt(tOne));
                   }

                }
            }

            if (actualTargetOrbit != targetOrbit) {
                targetDisplay = findNameForOrbit(targetOrbit, target);
            } else {
                targetDisplay = null;
            }

            if (originOrbit == null)
                sourceDisplay = "N/A";
            else if (actualSourceOrbit != originOrbit || HighLogic.LoadedScene == GameScenes.TRACKSTATION) {
                sourceDisplay = findNameForOrbit(originOrbit, vessel != null ? vessel : TrackingStationSource);
            } else {
                sourceDisplay = null;
            }

        }

        public string findNameForOrbit(Orbit orbit, ITargetable start) {
            if (start.GetOrbit() == orbit || start.GetOrbit() == null)
                return nameForTargetable(start);
            else
                return (findNameForOrbit(orbit, start.GetOrbit().referenceBody));
        }


        public void findConcentricParents(ref Orbit source, ref Orbit target) {
            Orbit o = target;
            while (true) {
                while (true) {
                    //Debug.Log("Compare " + source.referenceBody.GetName() + " to " + target.referenceBody.GetName());

                    if (source.referenceBody == target.referenceBody)
                        return;
                    if (target.referenceBody.orbit == null) //the sun!
                        break;
                    target = target.referenceBody.orbit;
                }

                if (source.referenceBody.orbit == null) //the sun!
                    return;

                source = source.referenceBody.orbit;
                target = o;
            }
        }

        public static string nameForTargetable(ITargetable tgt) {
            MapObject mo = null;

            if (tgt is CelestialBody)
                mo = ((CelestialBody)tgt).MapObject;
            else if (tgt is global::Vessel)
                mo = ((global::Vessel)tgt).mapObject;

            if (mo == null || mo.Discoverable == null)
                return Localizer.Format("<<1>>", tgt.GetDisplayName().LocalizeRemoveGender());
            else
                return Localizer.Format("<<1>>", mo.GetDisplayName().LocalizeRemoveGender());
        }

        private double CalcInterceptAngle(Orbit targetOrbit, Orbit originOrbit) {
            double originRadius = (originOrbit.semiMinorAxis + originOrbit.semiMajorAxis) * 0.5;
            double targetRadius = (targetOrbit.semiMinorAxis + targetOrbit.semiMajorAxis) * 0.5;
            double angle = 180.0 * (1.0 - Math.Pow((originRadius + targetRadius) / (2.0 * targetRadius), 1.5));
            return RelativeInclination < 90.0 ? angle : 360.0 - (180.0 - angle);
        }

        private Vector3d GetAscendingNode(Orbit targetOrbit, Orbit originOrbit) {
            return Vector3d.Cross(targetOrbit.GetOrbitNormal(), originOrbit.GetOrbitNormal());
        }

        private Vector3d GetDescendingNode(Orbit targetOrbit, Orbit originOrbit) {
            return Vector3d.Cross(originOrbit.GetOrbitNormal(), targetOrbit.GetOrbitNormal());
        }

        //From MechJeb2
        //Computes the time required for the given launch location to rotate under the target orbital plane. 
        //If the latitude is too high for the launch location to ever actually rotate under the target plane,
        //returns the time of closest approach to the target plane.
        //I have a wonderful proof of this formula which this comment is too short to contain.
        private double[] CalcAngleToPlane(CelestialBody launchBody, double launchLatitude, double launchLongitude, Orbit target) {
            double[] o = new double[2];

            double inc = Math.Abs(Vector3d.Angle(SwappedOrbitNormal(target), launchBody.angularVelocity));
            Vector3d b = Vector3d.Exclude(launchBody.angularVelocity, SwappedOrbitNormal(target)).normalized; // I don't understand the sign here, but this seems to work
            b *= launchBody.Radius * Math.Sin(Units.DEG_TO_RAD * launchLatitude) / Math.Tan(Units.DEG_TO_RAD * inc);
            Vector3d c = Vector3d.Cross(SwappedOrbitNormal(target), launchBody.angularVelocity).normalized;
            double cMagnitudeSquared = Math.Pow(launchBody.Radius * Math.Cos(Units.DEG_TO_RAD * launchLatitude), 2) - b.sqrMagnitude;
            if (cMagnitudeSquared < 0) cMagnitudeSquared = 0;
            c *= Math.Sqrt(cMagnitudeSquared);
            Vector3d a1 = b + c;
            Vector3d a2 = b - c;

            Vector3d longitudeVector = launchBody.GetSurfaceNVector(0, launchLongitude);

            double angle1 = Math.Abs(Vector3d.Angle(longitudeVector, a1));
            if (Vector3d.Dot(Vector3d.Cross(longitudeVector, a1), launchBody.angularVelocity) < 0) angle1 = 360 - angle1;

            double angle2 = Math.Abs(Vector3d.Angle(longitudeVector, a2));
            if (Vector3d.Dot(Vector3d.Cross(longitudeVector, a2), launchBody.angularVelocity) < 0) angle2 = 360 - angle2;

            o[0] = Math.Min(angle1, angle2);
            o[1] = Math.Max(angle1, angle2) - 360;

            return o;
        }

        //normalized vector perpendicular to the orbital plane
        //convention: as you look down along the orbit normal, the satellite revolves counterclockwise
        public static Vector3d SwappedOrbitNormal(Orbit o) {
            return -SwapYZ(o.GetOrbitNormal()).normalized;
        }

        //can probably be replaced with Vector3d.xzy?
        public static Vector3d SwapYZ(Vector3d v) {
            return v.xzy;
        }
    }
}