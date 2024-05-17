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

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    #region Using Directives

    using System;
    using UnityEngine;

    #endregion

    public class AttitudeProcessor : IUpdatable, IUpdateRequest
    {
        public const double INVALID_ANGLE = -9000000.0;

        #region Fields

        private static readonly AttitudeProcessor instance = new AttitudeProcessor();

        private Vector3 centreOfMass = Vector3.zero;

        private double heading;
        private double headingRate;
        private Vector3 north = Vector3.zero;
        private double pitch;
        private double pitchRate;
        private double previousHeading;
        private double previousPitch;
        private double previousRoll;
        private double roll;
        private double rollRate;
        private Quaternion surfaceRotation;
        private Vector3 up = Vector3.zero;

        #endregion

        #region Properties

        public static double Heading
        {
            get { return instance.heading; }
        }

        public static double HeadingRate
        {
            get { return instance.headingRate; }
        }

        public static AttitudeProcessor Instance
        {
            get { return instance; }
        }

        public static double Pitch
        {
            get { return instance.pitch; }
        }

        public static double PitchRate
        {
            get { return instance.pitchRate; }
        }

        public static double Roll
        {
            get { return instance.roll; }
        }

        public static double RollRate
        {
            get { return instance.rollRate; }
        }
        
        public static double DisplacementAngle { get; private set; }
        public static double AttackAngle { get; private set; }
        public static double SideslipAngle { get; private set; }

        public bool UpdateRequested { get; set; }

        #endregion

        #region Methods

        public static void RequestUpdate()
        {
            instance.UpdateRequested = true;
        }

        public void Update()
        {
            var vessel = FlightGlobals.ActiveVessel;

            this.surfaceRotation = this.GetSurfaceRotation(vessel);

            this.previousHeading = this.heading;
            this.previousPitch = this.pitch;
            this.previousRoll = this.roll;

            // This code was derived from MechJeb2's implementation for getting the vessel's surface relative rotation.
            this.heading = this.surfaceRotation.eulerAngles.y;
            this.pitch = this.surfaceRotation.eulerAngles.x > 180.0f
                ? 360.0f - this.surfaceRotation.eulerAngles.x
                : -this.surfaceRotation.eulerAngles.x;
            this.roll = this.surfaceRotation.eulerAngles.z > 180.0f
                ? 360.0f - this.surfaceRotation.eulerAngles.z
                : -this.surfaceRotation.eulerAngles.z;

            this.headingRate = this.heading - this.previousHeading;
            this.pitchRate = this.pitch - this.previousPitch;
            this.rollRate = this.roll - this.previousRoll;


            //Also stolen from MechJeb2
            
            var surfaceVelocity = vessel.obt_velocity - vessel.mainBody.getRFrmVel(vessel.CoMD);
            var surfaceSpeed = surfaceVelocity.magnitude;
            
            if (surfaceSpeed < 0.05) DisplacementAngle = AttackAngle = SideslipAngle = INVALID_ANGLE;
            else {
                // Displacement Angle, angle between surface velocity and the ship-nose vector (KSP "up" vector) -- ignores roll of the craft (0 to 180 degrees)
                double tempAoD = UtilMath.Rad2Deg *
                                 Math.Acos(Mathf.Clamp(Vector3.Dot(vessel.ReferenceTransform.up, surfaceVelocity.normalized), -1, 1));
                DisplacementAngle = double.IsNaN(tempAoD) ? INVALID_ANGLE : tempAoD;

                // Angle of Attack, angle between surface velocity and the ship-nose vector (KSP "up" vector) in the plane that has no ship-right/left in it (-180 to +180 degrees)
                var srfProj = Vector3.ProjectOnPlane(surfaceVelocity.normalized, vessel.ReferenceTransform.right);
                double tempAoA = UtilMath.Rad2Deg * Math.Atan2(Vector3.Dot(srfProj.normalized, vessel.ReferenceTransform.forward),
                                                                Vector3.Dot(srfProj.normalized, vessel.ReferenceTransform.up));
                AttackAngle = double.IsNaN(tempAoA) ? INVALID_ANGLE : tempAoA;

                // Angle of Sideslip, angle between surface velocity and the ship-nose vector (KSP "up" vector) in the plane that has no ship-top/bottom in it (KSP "forward"/"back"; -180 to +180 degrees)
                srfProj = Vector3.ProjectOnPlane(surfaceVelocity.normalized, vessel.ReferenceTransform.forward);
                double tempAoS = UtilMath.Rad2Deg * Math.Atan2(Vector3.Dot(srfProj.normalized, vessel.ReferenceTransform.right),
                                                               Vector3.Dot(srfProj.normalized, vessel.ReferenceTransform.up));
                SideslipAngle = double.IsNaN(tempAoA) ? INVALID_ANGLE : tempAoS;
            }
        }

        private Quaternion GetSurfaceRotation(global::Vessel vessel)
        {
            // This code was derived from MechJeb2's implementation for getting the vessel's surface relative rotation.
            this.centreOfMass = vessel.CoMD;
            this.up = (this.centreOfMass - vessel.mainBody.position).normalized;
            this.north = Vector3.ProjectOnPlane((vessel.mainBody.position + vessel.mainBody.transform.up * (float)vessel.mainBody.Radius) - this.centreOfMass, this.up).normalized;

            return Quaternion.Inverse(Quaternion.Euler(90.0f, 0.0f, 0.0f) * Quaternion.Inverse(vessel.transform.rotation) * Quaternion.LookRotation(this.north, this.up));
        }

        #endregion
    }
}