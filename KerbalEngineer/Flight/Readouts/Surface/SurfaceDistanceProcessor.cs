﻿//
//     Kerbal Engineer Redux
//
//     Copyright (C) 2017 fat-lobyte
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

#region Using Directives

using System;
using KerbalEngineer.Helpers;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class SurfaceDistanceProcessor : IUpdatable, IUpdateRequest
    {
        /// <summary>
        ///     Gets the current instance of the rendezvous processor.
        /// </summary>
        public static SurfaceDistanceProcessor Instance
        {
            get { return instance; }
        }


        /// <summary>
        ///     Gets whether a target or waypoint are active.
        /// </summary>
        public static bool ShowTargetDetails { get; private set; }
        public static bool ShowWaypointDetails { get; private set; }
        
        /// <summary>
        ///     Gets whether the target is in the same sphere-of-influence as us. If not, we don't calculate the bearing and distance.
        /// </summary>
        public static bool TargetInSameSOI { get; private set; }


        /// <summary>
        ///     Gets and sets whether the updatable object should be updated.
        /// </summary>
        public bool UpdateRequested { get; set; }

        private static readonly SurfaceDistanceProcessor instance = new SurfaceDistanceProcessor();

        /// <summary>
        ///     Gets the great-circle distance from the current vessel to the target on the surface.
        /// </summary>
        public static double SurfaceDistanceToTarget { get; private set; }

        /// <summary>
        ///     Gets the initial bearing on the great-circle from the origin position to the target position on the surface.
        /// </summary>
        public static double SurfaceBearingToTarget { get; private set; }

        /// <summary>
        ///     Gets the great-circle distance from the current vessel to the waypoint position on the surface.
        /// </summary>
        public static double SurfaceDistanceToWaypoint { get; private set; }

        /// <summary>
        ///     Gets the initial bearing on the great-circle current vessel to the waypoint position on the surface.
        /// </summary>
        public static double SurfaceBearingToWaypoint { get; private set; }

        /// <summary>
        ///     Request and update to calculate the details.
        /// </summary>
        public static void RequestUpdate()
        {
            instance.UpdateRequested = true;
        }

        /// <summary>
        ///     Updates the details by recalculating if requested.
        /// </summary>
        public void Update()
        {
            // get vessel and navigation waypoints
            global::Vessel targetVessel = FlightGlobals.fetch?.VesselTarget?.GetVessel();
            FinePrint.Waypoint navigationWaypoint = FlightGlobals.ActiveVessel?.navigationWaypoint;

            ShowTargetDetails = FlightGlobals.ActiveVessel != null && targetVessel != null;
            ShowWaypointDetails = FlightGlobals.ActiveVessel != null && navigationWaypoint != null;
            
            double originLat = FlightGlobals.ActiveVessel.latitude;
            double originLon = FlightGlobals.ActiveVessel.longitude;

            if (ShowTargetDetails)
            {
                if (targetVessel.mainBody != FlightGlobals.ActiveVessel.mainBody) { //todo - it's still probably useful to calculate these even when it's in a different SOI, at least the bearing, the distance won't work for either targets or waypoints; also check that navigationWaypoint.latitude/longitude are always in reference to the current SOI of the vessel
                    TargetInSameSOI = false;
                } else {
                    TargetInSameSOI = true;

                    double targetLat = targetVessel.mainBody.GetLatitude(targetVessel.GetWorldPos3D());
                    double targetLon = targetVessel.mainBody.GetLongitude(targetVessel.GetWorldPos3D());

                    SurfaceDistanceToTarget = CalcSurfaceDistance(FlightGlobals.ActiveVessel.mainBody.Radius,
                        originLat, originLon,
                        targetLat, targetLon);

                    SurfaceBearingToTarget = CalcSurfaceBearingToTarget(originLat, originLon,
                        targetLat, targetLon);
                }
            }

            if (ShowWaypointDetails)
            {
                SurfaceDistanceToWaypoint = CalcSurfaceDistance(FlightGlobals.ActiveVessel.mainBody.Radius,
                    originLat, originLon,
                    navigationWaypoint.latitude, navigationWaypoint.longitude);

                SurfaceBearingToWaypoint = CalcSurfaceBearingToTarget(originLat, originLon,
                    navigationWaypoint.latitude, navigationWaypoint.longitude);
            }
        }



        /// <summary>
        /// Calculate the shortest great-circle distance between two points on a sphere which are given by latitude and longitude.
        ///
        /// https://en.wikipedia.org/wiki/Haversine_formula
        /// </summary>
        /// <param name="bodyRadius"></param> Radius of the sphere in meters
        /// <param name="originLatitude"></param>Latitude of the origin of the distance, in degrees
        /// <param name="originLongitude"></param>Longitude of the origin of the distance, in degrees
        /// <param name="targetLatitude"></param>Latitude of the destination of the distance, in degrees
        /// <param name="targetLongitude"></param>Longitude of the destination of the distance, in degrees
        /// <returns>Distance between origin and source in meters</returns>
        private static double CalcSurfaceDistance(
            double bodyRadius,
            double originLatitude, double originLongitude,
            double targetLatitude, double targetLongitude)
        {
            double sin1 = Math.Sin(Units.DEG_TO_RAD * (originLatitude - targetLatitude) / 2);
            double sin2 = Math.Sin(Units.DEG_TO_RAD * (originLongitude - targetLongitude) / 2);
            double cos1 = Math.Cos(Units.DEG_TO_RAD * targetLatitude);
            double cos2 = Math.Cos(Units.DEG_TO_RAD * originLatitude);

            return 2 * bodyRadius * Math.Asin(Math.Sqrt(sin1 * sin1 + cos1 * cos2 * sin2 * sin2));
        }

        private static double CalcSurfaceBearingToTarget(
            double originLatitude, double originLongitude,
            double targetLatitude, double targetLongitude)
        {
            double olat = Units.DEG_TO_RAD * originLatitude,
                   olon = Units.DEG_TO_RAD * originLongitude,
                   tlat = Units.DEG_TO_RAD * targetLatitude,
                   tlon = Units.DEG_TO_RAD * targetLongitude;

            double y = Math.Sin(tlon - olon) * Math.Cos(tlat);
            double x = (Math.Cos(olat) * Math.Sin(tlat)) - (Math.Sin(olat) * Math.Cos(tlat) * Math.Cos(tlon - olon));
            double requiredBearing = Math.Atan2(y, x) * Units.RAD_TO_DEG;
            return (requiredBearing + 360.0) % 360.0;
        }
    }
}