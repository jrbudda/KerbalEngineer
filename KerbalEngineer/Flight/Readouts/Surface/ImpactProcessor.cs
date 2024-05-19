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

#region Using Directives

using System;

using UnityEngine;
using KerbalEngineer.Helpers;

#endregion

// The calculations and functional code in this processor were generously developed by mic_e.


namespace KerbalEngineer.Flight.Readouts.Surface {
    public class ImpactProcessor : IUpdatable, IUpdateRequest {
        #region Instance

        #region Fields

        private static readonly ImpactProcessor instance = new ImpactProcessor();

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the current instance of the impact processor.
        /// </summary>
        public static ImpactProcessor Instance {
            get { return instance; }
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the altitude of the impact coordinates.
        /// </summary>
        public static double Altitude { get; private set; }

        /// <summary>
        ///     Gets the biome of the impact coordinates.
        /// </summary>
        public static string Biome { get; private set; }

        /// <summary>
        ///     Gets the latitude of the impact coordinates.
        /// </summary>
        public static double Latitude { get; private set; }

        /// <summary>
        ///     Gets the longitude of the impact coordinates.
        /// </summary>
        public static double Longitude { get; private set; }

        /// <summary>
        ///     Gets whether the details are ready to be shown.
        /// </summary>
        public static bool ShowDetails { get; private set; }

        /// <summary>
        ///     Gets the time to impact.
        /// </summary>
        public static double Time { get; private set; }

        #endregion

        #region IUpdatable Members


        public void Update() {

            if (FlightEngineerCore.gamePaused)
                return;

            Vector3d futurepos = new Vector3d();
            var body = FlightGlobals.ActiveVessel.mainBody;
            var vessel = FlightGlobals.ActiveVessel;

            double terrainAltitude = 0;
            bool impactHappening = false;
            double impactLatitude = 0;
            double impactLongitude = 0;
            double impactTime = 0;
            double impacttheta = 0;

            SuicideAltitude = 0;
            SuicideCountdown = 0;
            SuicideDeltaV = 0;
            SuicideDistance = 0;
            SuicideLength = 0;
            ShowDetails = false;

            bool debugImpact = false;

            if (vessel.Landed || body.pqsController == null)
                return;

            if (vessel.orbit.PeA >= body.minOrbitalDistance) {
                //periapsis must be lower min dist;
                if (debugImpact) Debug.Log("no impact: periapse > min alt " + vessel.orbit.PeA + body.minOrbitalDistance);
                return;
            }

            if ((vessel.orbit.eccentricity >= 1) && (vessel.orbit.timeToPe <= 0)) {
                //if currently escaping, we still need to be before periapsis
                if (debugImpact) Debug.Log("no impact: escaping and passed periapse");
                return;
            }

            //if ((vessel.orbit.eccentricity < 1) && (vessel.orbit.ApA <= impactAltitude)) {
            //    //apoapsis must be higher than impact alt
            //    impactHappening = false;
            //    Debug.Log("no impact: ap < alt " + impactAltitude + " " + vessel.orbit.ApA);
            //}

            //do impact site calculations
            impactHappening = false;
            impactTime = 0;
            impactLongitude = 0;
            impactLatitude = 0;
            terrainAltitude = 0;

            var e = vessel.orbit.eccentricity;
            var a = vessel.orbit.semiMajorAxis;
            var r = body.Radius * 0.9999; //avoid floating point errors.

            //get current position direction vector
            var currentpos = vessel.orbit.getRelativePositionFromTrueAnomaly(vessel.orbit.trueAnomaly); // this.RadiusDirection(vessel.orbit.trueAnomaly * Units.RAD_TO_DEG);
            //calculate longitude in inertial reference frame from that
            var currentirflong = 180 * Math.Atan2(currentpos.x, currentpos.y) / Math.PI;

            int side = 1;

            double startangle = vessel.GetOrbit().trueAnomaly * Units.RAD_TO_DEG;
            if (startangle > 0) startangle = -360 + startangle;

            double endangle = startangle + 360;

            if (vessel.orbit.PeR <= body.Radius) { //only search down to sea level.
                double costheta = a / r / e - a * e / r - 1 / e;

                if (costheta < -1d)
                    costheta = -1d;
                else if (costheta > 1d)
                    costheta = 1d;

                endangle = -180 * Math.Acos(costheta) / Math.PI;
                endangle += Math.Abs(endangle - startangle) / 10; //avoid errors at 0;
            }

            //} else if (vessel.orbit.PeR <= body.minOrbitalDistance && vessel.orbit.ApR > body.minOrbitalDistance) {
            //    //search to min dist exit.
            //    r = body.minOrbitalDistance;
            //    double costheta = a / r * e - a * e / r - 1 / e;
            //    if (costheta < -1d)
            //        costheta = -1d;
            //    else if (costheta > 1d)
            //        costheta = 1d;
            //    endangle = -180 * Math.Acos(costheta) / Math.PI;

            //    if (vessel.orbit.timeToPe < 0) {
            //        if (endangle < 0) endangle *= -1;
            //    }

            //} else { //search all the way around. ap and pe both below min dist.
            //    endangle = startangle + 360; 
            //}

            double interval = Math.Abs(endangle - startangle) / 36;
            int it = 0;
            int itf = 0;

            if (debugImpact) Debug.Log("Impact pre " + side + " " + startangle + ">" + endangle + " " + interval + " " + terrainAltitude);
            bool ok = false;

            do {
                ok = false;
                it += 1;
                //-164 + 1.8*-1 ; -165 > -147
                for (impacttheta = startangle + interval * side; side == 1 ? impacttheta <= endangle : impacttheta >= endangle; impacttheta += interval * side) {
                    itf += 1;
                    double tARads = Math.PI * impacttheta / 180;
                    futurepos = vessel.orbit.getRelativePositionFromTrueAnomaly(tARads); //this.RadiusDirection(impacttheta);

                    if (futurepos.magnitude > body.minOrbitalDistance)
                        continue;

                    //calculate time to impact
                    impactTime = vessel.orbit.GetDTforTrueAnomaly(tARads, 0);
                    //calculate position vector of impact site
                    futurepos = vessel.orbit.getRelativePositionFromTrueAnomaly(tARads); //this.RadiusDirection(impacttheta);
                    //calculate longitude of impact site in inertial reference frame
                    var impactirflong = 180 * Math.Atan2(futurepos.x, futurepos.y) / Math.PI;
                    var deltairflong = impactirflong - currentirflong;
                    //get body rotation until impact
                    var bodyrot = 360 * impactTime / body.rotationPeriod;
                    //get current longitude in body coordinates
                    var currentlong = vessel.longitude;
                    //finally, calculate the impact longitude in body coordinates
                    impactLongitude = this.NormAngle(currentlong - deltairflong - bodyrot);
                    //calculate impact latitude from impact position
                    impactLatitude = 180 * Math.Asin(futurepos.z / futurepos.magnitude) / Math.PI;
                    //calculate the actual altitude of the impact site
                    //altitude for long/lat code stolen from some ISA MapSat forum post; who knows why this works, but it seems to.
                    var rad = QuaternionD.AngleAxis(impactLongitude, Vector3d.down) * QuaternionD.AngleAxis(impactLatitude, Vector3d.forward) * Vector3d.right;
                    terrainAltitude = body.pqsController.GetSurfaceHeight(rad) - body.pqsController.radius;

                    double shipalt = futurepos.magnitude - body.Radius;

                    if ((terrainAltitude < 0) && body.ocean) {
                        terrainAltitude = 0; //sploosh.
                    }

                    double delta = shipalt - terrainAltitude;

                    if (debugImpact) Debug.Log("Impact iteration " + currentpos + " " + futurepos + " " + side + " " + startangle + ">" + endangle + " " + impacttheta + " " + interval + " " + shipalt + " " + terrainAltitude + " " + delta);

                    if ((side * delta < 0)) {
                        impactHappening = true;
                        side *= -1;
                        startangle = impacttheta;
                        endangle = impacttheta + side * interval;
                        interval = Math.Abs(endangle - startangle) / 20.0;
                        endangle += interval * side;
                        if (debugImpact) Debug.Log("Impact Switch! " + startangle + " > " + endangle + " " + interval);
                        ok = true;
                        break;
                    } else if (delta == 0) { //i guess it could happen.
                        if (debugImpact) Debug.Log("Impact Zero! " + startangle + " > " + endangle + " " + interval);
                        impactHappening = true;
                        interval = 0;
                        ok = true;
                        break;
                    }

                }

                if (!ok) {
                    if (debugImpact) Debug.Log("bad loop");
                    break;
                }

            } while (interval > 0.00001 && impactHappening && it < 36);

            if (debugImpact) Debug.Log("Impact calc! iterations " + impactHappening + " " + it + " " + itf );

            if (impactHappening) {
                ShowDetails = true;
                Time = impactTime;
                Longitude = impactLongitude;
                Latitude = impactLatitude;
                Altitude = terrainAltitude;

                try {
                    Biome = ScienceUtil.GetExperimentBiome(body, impactLatitude, impactLongitude);
                    Biome = ScienceUtil.GetBiomedisplayName(body, Biome);
                } catch (Exception) { //this gets spammy with misbehaving mod planets.
                    Biome = "<failed>";
                }

                double m_Gravity = body.gravParameter / Math.Pow(body.Radius + terrainAltitude, 2.0);
                double m_Acceleration = Vessel.SimulationProcessor.LastStage.totalMass > 0 ? Vessel.SimulationProcessor.LastStage.thrust / Vessel.SimulationProcessor.LastStage.totalMass : 0;

                bool debugBurn = false;

                if (m_Acceleration == 0 || m_Acceleration < m_Gravity) {
                    //lol u dead, boy
                    if (debugBurn) Debug.Log("insuffucuent thrust " + m_Acceleration);
                    return;
                }

                double brakingDist = 0;
                double burnTime = 0;
                Vector3d srfb = new Vector3d();
                double shipalt = 0;

                brakingDist = getBrakingDistanceForDT(vessel, 0, body, out shipalt, out burnTime, out srfb, false);
                //get instant breaking dist as sanity check.

                int iterations = 0;

                if (brakingDist < shipalt) { //only search if it's actually possible to stop.
                    double tstart = 0;
                    double tend = Time;
                    double lastt = 0;
                    double brakedelta = double.MaxValue;
                    double btime = 0;
                    Vector3d srf = new Vector3d();
                    double bdist = 0;

                    do { //dat binary search doe. Is there a closed form solution to this? idk. brute force!
                        iterations++;
                        double t = tstart + (tend - tstart) / 2;

                        if (Math.Abs(lastt - t) < 0.05) {
                            if (debugBurn) getBrakingDistanceForDT(vessel, lastt, body, out shipalt, out btime, out srf, true);
                            break; //that should be quite enough, tyvm.
                        }

                        lastt = t;

                        bdist = getBrakingDistanceForDT(vessel, t, body, out shipalt, out btime, out srf, false);

                        if (bdist <= shipalt) {
                            if (shipalt - bdist < brakedelta ) {
                                brakedelta = shipalt - bdist;
                                brakingDist = bdist;
                                burnTime = btime;
                                srfb = srf;
                            }
                            tstart = t; //search forward.
                            if (debugBurn) Debug.Log("forward: t " + t + " shipalt " + shipalt + " srf " + srf + " bdist " + bdist);

                        } else {
                            tend = t; //search backwards.
                           if (debugBurn) Debug.Log("backward: t " + t + " shipalt " + shipalt + " srf " + srf + " bdist " + bdist);
                        }


                    } while (true);
                } else {
                    if (debugBurn) Debug.Log("Can't stop, won't stop. srf:" + srfb + " " + shipalt);
                }

                if (debugBurn) Debug.Log("breaking dist " + brakingDist + " " + iterations);

                SuicideDeltaV = srfb.magnitude;
                SuicideAltitude = terrainAltitude + brakingDist;
                SuicideLength = burnTime;

                r = SuicideAltitude + body.Radius;

                double costheta = a / r / e - a * e / r - 1 / e;
                if (costheta < -1d)
                    costheta = -1d;
                else if (costheta > 1d)
                    costheta = 1d;

                double burntA = -Math.Acos(costheta);

                if (debugBurn) Debug.Log("burnpos " + costheta + " " + vessel.orbit.pos + " " + vessel.orbit.getRelativePositionFromTrueAnomaly(burntA));

                SuicideCountdown = vessel.orbit.GetDTforTrueAnomaly(burntA, double.MaxValue);

                if (SuicideCountdown < 0 && vessel.orbit.trueAnomaly > 0)
                    SuicideCountdown = vessel.orbit.period + SuicideCountdown;

                //this next bit is suprisingly tricky. Orbital arc distance given 2 true anomalies.
                //from http://mathforum.org/kb/servlet/JiveServlet/download/130-2391290-7852023-766514/PERIMETER%20OF%20THE%20ELLIPTICAL%20ARC%20A%20GEOMETRIC%20METHOD.pdf
                //this only works right if you're after apoapsis (same quadrant), but you should be using countdown anyway.
                double Eburn = vessel.orbit.GetEccentricAnomaly(burntA); //should always be a smallish negative number.
                double EShip = vessel.orbit.GetEccentricAnomaly(vessel.orbit.trueAnomaly); //why is orbit.E 0 ????
                if (EShip > 0)
                    EShip = -2 * Math.PI + EShip;
                double chord = (vessel.orbit.pos - vessel.orbit.getRelativePositionFromTrueAnomaly(burntA)).magnitude;
                double dTheta = Eburn - EShip;
                double Rc = Math.Abs(chord / 2 / Math.Sin(dTheta / 2));
                SuicideDistance = Rc * dTheta;

                if (debugBurn) Debug.Log("dist debug eburn " + Eburn + " eship " + EShip + " ch " + chord + " dt " + dTheta + " rc " + Rc);


            } else {
                ShowDetails = false;
            }
        }


        public static double SuicideDeltaV { get; private set; }
        public static double SuicideDistance { get; private set; }
        public static double SuicideAltitude { get; private set; }
        public static double SuicideLength { get; private set; }
        public static double SuicideCountdown { get; private set; }


        private double getBrakingDistanceForDT(global::Vessel vessel, double t, CelestialBody body, out double shipalt, out double burntime, out Vector3d srf, bool debug) {
            Vector3d posFromNow = vessel.GetOrbit().getRelativePositionAtUT(Planetarium.GetUniversalTime() +  t);
            double shipradius = posFromNow.magnitude;
            shipalt = shipradius - body.Radius;

            //hmm. use instant or surface gravity? instant will underestimate brake dist. Surface will overestimate.
            double m_Gravity = body.gravParameter / Math.Pow(shipradius - shipalt / 2, 2.0);
            //use the average!

            srf = GetSrfAtDT(vessel, t, body);
            var upAxis = posFromNow.normalized.xzy;
            double ivertDeltaV = -Vector3d.Dot(srf, upAxis);

            if (ivertDeltaV <= 0) {
                if (debug) {
                    Debug.Log("calc brake: going up (land) " + srf + " " + upAxis + " " + posFromNow + " " + ivertDeltaV);
                }
                burntime = 0;
                return 0;
            }

            double ihDeltaV = Math.Sqrt(srf.sqrMagnitude - ivertDeltaV * ivertDeltaV);
            double loss = ivertDeltaV == 0 ? 0 : Math.Cos(Math.Atan(ihDeltaV / ivertDeltaV)); //assume ship is always surface retrograde
            double accel = Vessel.SimulationProcessor.LastStage.totalMass > 0 ? Vessel.SimulationProcessor.LastStage.thrust / Vessel.SimulationProcessor.LastStage.totalMass : 0;
            double vaccel = accel * loss - m_Gravity;
            double haccel = accel * (1 - loss);
            double netaccel = Math.Sqrt(vaccel * vaccel + haccel * haccel);
            burntime = srf.magnitude / netaccel;
            double vburnTime = ivertDeltaV / vaccel;
            double bdist = ivertDeltaV * vburnTime - (0.5) * (vaccel) * vburnTime * vburnTime;
            if (debug) Debug.Log("calc brake " + " g " + m_Gravity + " a " + accel + " loss " + loss + " vi " +ivertDeltaV + " hi " + ihDeltaV + " b " + burntime + " vb " + vburnTime );
            return bdist;
        }


        #endregion


        public static bool ShowMarker = true;

        public static void drawImpact(Color color) {
            if (ShowDetails && ShowMarker && FlightGlobals.fetch !=null && FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.mainBody != null )
                Drawing.DebugDrawing.DrawGroundMarker(FlightGlobals.ActiveVessel.mainBody, Latitude, Longitude, color, MapView.MapIsEnabled, 0, 0);
        }

        #region IUpdateRequest Members

        public bool UpdateRequested { get; set; }

        #endregion

        #region Methods: public

        public static void RequestUpdate() {
            Vessel.SimulationProcessor.RequestUpdate();
            instance.UpdateRequested = true;
        }

        #endregion

        #region Calculations

        #region Methods: public

        public static double ACosh(double x) {
            return (Math.Log(x + Math.Sqrt((x * x) - 1.0)));
        }

        #endregion

        #region Methods: private

        private Vector3d GetSrfAtDT(global::Vessel vessel, double deltaTime, CelestialBody body) {
            Vector3d obti = vessel.GetOrbit().getOrbitalVelocityAtUT(Planetarium.GetUniversalTime() + deltaTime).xzy;
            Vector3d rposFromNow = vessel.GetOrbit().getRelativePositionAtUT(Planetarium.GetUniversalTime() + deltaTime).xzy;
            Vector3d refi = Vector3d.Cross(body.angularVelocity, rposFromNow);
            return obti - refi;
        }

        private double NormAngle(double ang) {
            if (ang > 180) {
                ang -= 360 * Math.Ceiling((ang - 180) / 360);
            }
            if (ang <= -180) {
                ang -= 360 * Math.Floor((ang + 180) / 360);
            }

            return ang;
        }

        private Vector3d RadiusDirection(double theta) {
            theta = Math.PI * theta / 180;
            var omega = Math.PI * FlightGlobals.ActiveVessel.orbit.argumentOfPeriapsis / 180;
            var incl = Math.PI * FlightGlobals.ActiveVessel.orbit.inclination / 180;

            var costheta = Math.Cos(theta);
            var sintheta = Math.Sin(theta);
            var cosomega = Math.Cos(omega);
            var sinomega = Math.Sin(omega);
            var cosincl = Math.Cos(incl);
            var sinincl = Math.Sin(incl);

            Vector3d result;

            result.x = cosomega * costheta - sinomega * sintheta;
            result.y = cosincl * (sinomega * costheta + cosomega * sintheta);
            result.z = sinincl * (sinomega * costheta + cosomega * sintheta);

            return result;
        }

        private double TimeToPeriapsis(double theta) {
            var e = FlightGlobals.ActiveVessel.orbit.eccentricity;
            var a = FlightGlobals.ActiveVessel.orbit.semiMajorAxis;
            var rp = FlightGlobals.ActiveVessel.orbit.PeR;
            var mu = FlightGlobals.ActiveVessel.mainBody.gravParameter;

            if (e == 1.0) {
                var D = Math.Tan(Math.PI * theta / 360.0);
                var M = D + D * D * D / 3.0;
                return (Math.Sqrt(2.0 * rp * rp * rp / mu) * M);
            }
            if (a > 0) {
                var cosTheta = Math.Cos(Math.PI * theta / 180.0);
                var cosE = (e + cosTheta) / (1.0 + e * cosTheta);
                var radE = Math.Acos(cosE);
                var M = radE - e * Math.Sin(radE);
                return (Math.Sqrt(a * a * a / mu) * M);
            }
            if (a < 0) {
                var cosTheta = Math.Cos(Math.PI * theta / 180.0);
                var coshF = (e + cosTheta) / (1.0 + e * cosTheta);
                var radF = ACosh(coshF);
                var M = e * Math.Sinh(radF) - radF;
                return (Math.Sqrt(-a * a * a / mu) * M);
            }

            return 0;
        }

        #endregion

        #endregion
    }
}