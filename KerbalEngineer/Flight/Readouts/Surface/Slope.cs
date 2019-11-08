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

using KerbalEngineer.Flight.Sections;
using KerbalEngineer.Helpers;

using UnityEngine;

using KSP.Localization;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class Slope : ReadoutModule
    {
        #region Constructors

        public Slope()
        {
            this.Name = Localizer.Format("#KE_Slope");//"Slope"
            this.Category = ReadoutCategory.GetCategory("Surface");
            this.HelpString = Localizer.Format("#KE_Slope_desc");//"Shows the slope of the terrain below your vessel."
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            this.DrawLine(this.GetSlopeAngleAndHeading(), section.IsHud);
        }

        #endregion

        #region Methods: private

        private string GetSlopeAngleAndHeading()
        {
            try
            {
                var result = "--° @ ---°";
                var mainBody = FlightGlobals.ActiveVessel.mainBody;
                var rad = (FlightGlobals.ActiveVessel.CoM - mainBody.position).normalized;
                RaycastHit hit;
                if (Physics.Raycast(FlightGlobals.ActiveVessel.CoM, -rad, out hit, Mathf.Infinity, 1 << 15)) // Just "Local Scenery" please
                {
                    var norm = hit.normal;
                    norm = norm.normalized;
                    var raddotnorm = Vector3d.Dot(rad, norm);
                    if (raddotnorm > 1.0)
                    {
                        raddotnorm = 1.0;
                    }
                    else if (raddotnorm < 0.0)
                    {
                        raddotnorm = 0.0;
                    }
                    var slope = Math.Acos(raddotnorm) * 180 / Math.PI;
                    result = Units.ToAngle(slope, 1);
                    if (slope < 0.05)
                    {
                        result += " @ ---°";
                    }
                    else
                    {
                        var side = Vector3d.Cross(rad, norm).normalized;
                        var east = Vector3d.Cross(rad, Vector3d.up).normalized;
                        var north = Vector3d.Cross(rad, east).normalized;
                        var sidedoteast = Vector3d.Dot(side, east);
                        var direction = Math.Acos(sidedoteast) * 180 / Math.PI;
                        var sidedotnorth = Vector3d.Dot(side, north);
                        if (sidedotnorth < 0)
                        {
                            direction = 360 - direction;
                        }
                        result += " @ " + Units.ToAngle(direction, 1);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex, "Surface->Slope->GetSlopeAngleAndHeading");
                return "--° @ ---°";
            }
        }

        #endregion
    }
}