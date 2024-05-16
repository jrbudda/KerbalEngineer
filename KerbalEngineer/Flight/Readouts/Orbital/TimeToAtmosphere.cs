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

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class TimeToAtmosphere : ReadoutModule
    {
        //private LogMsg log = new LogMsg();
        
        #region Constructors

        public TimeToAtmosphere()
        {
            this.Name = "Time to Atmosphere";
            this.Category = ReadoutCategory.GetCategory("Orbital");
            this.HelpString = "Shows the time until the vessel enters or leaves the atmosphere.";
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            String str;
            Orbit orbit = FlightGlobals.ship_orbit;

			if (orbit.referenceBody.atmosphere && orbit.PeA < orbit.referenceBody.atmosphereDepth && orbit.ApA > orbit.referenceBody.atmosphereDepth)
            {
                double tA = orbit.TrueAnomalyAtRadius(orbit.referenceBody.atmosphereDepth + orbit.referenceBody.Radius);
                //log.buf.AppendFormat("tA = {0}\n", tA);
                double utTime = Planetarium.GetUniversalTime();
                //log.buf.AppendFormat("utTime = {0}\n", utTime);
                double timeAtRad1 = orbit.GetUTforTrueAnomaly(tA, orbit.period * 0.5);
                //log.buf.AppendFormat("timeAtRad1 = {0}\n", timeAtRad1);
                if (timeAtRad1 < utTime)
                {
                    timeAtRad1 += orbit.period;
                    //log.buf.AppendFormat("timeAtRad1 = {0}\n", timeAtRad1);
                }
                double timeAtRad2 = orbit.GetUTforTrueAnomaly(-tA, orbit.period * 0.5);
                //log.buf.AppendFormat("timeAtRad2 = {0}\n", timeAtRad2);
                if (timeAtRad2 < utTime)
                {
                    timeAtRad2 += orbit.period;
                    //log.buf.AppendFormat("timeAtRad2 = {0}\n", timeAtRad2);
                }
                double time = Math.Min(timeAtRad1, timeAtRad2) - utTime;
                //log.buf.AppendFormat("time = {0}\n", time);

                if (Double.IsNaN(time))
                {
                    str = "---s";
                    //log.buf.AppendLine("time is NaN");
                }
                else
                {
                    str = TimeFormatter.ConvertToString(time);
                    //log.buf.AppendFormat("str = {0}\n", str);
                }
            }
            else
            {
                str = "---s";
                //log.buf.AppendLine("no atmosphere, pe > atmosphere, or ap < atmosphere");
            }

            //log.Flush();
            this.DrawLine(str, section);
        }

        #endregion
    }
}