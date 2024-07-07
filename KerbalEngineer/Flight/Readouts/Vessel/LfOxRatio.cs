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

    using Helpers;
    using System;
    using System.Collections.Generic;

    #endregion

    public class LfOxRatio : ReadoutModule
    {
        #region Constructors

        public LfOxRatio() {
            this.Name = "LF:Ox Ratio";
            this.ShortName = "LF:Ox";
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = "Ratio of Liquid Fuel to Oxidizer in the vessel. More than 100% means you have more LF than oxidizer for standard rocket engines, so you have extra for LF-only engines. The number in parentheses is how much excess LF you have.";
            this.IsDefault = false;
        }

        #endregion

        #region Methods

        public override void Draw(Unity.Flight.ISectionModule section) {
            PartResourceDefinition lfDefinition = PartResourceLibrary.Instance.GetDefinition("LiquidFuel"),
                                   oxDefinition = PartResourceLibrary.Instance.GetDefinition("Oxidizer");
            List<Part> parts = FlightGlobals.ActiveVessel.parts;

            double currentLF = 0, currentOx = 0;
            foreach (Part part in parts) {
                foreach (PartResource resource in part.Resources) {
                    if (resource.info.id == lfDefinition.id) currentLF += resource.amount;
                    else if (resource.info.id == oxDefinition.id) currentOx += resource.amount;
                }
            }

            bool usingShortName = UsingShortName(section);
            if (currentOx <= double.Epsilon) this.DrawLine(usingShortName ? "No Ox" : "No Oxidizer", section);
            else {
                const double FUEL_MIX = 440.0 / 360.0;
                double ratio = currentLF * FUEL_MIX / currentOx;
                if (usingShortName) {
                    if (ratio >= 10.0) {
                        this.DrawLine("A Lot", section);
                        return;
                    }
                } else {
                    if (ratio > 90.0) {
                        this.DrawLine("Over 9000%", section);
                        return;
                    }
                }

                int excess = (int)(currentLF - (currentOx / FUEL_MIX));

                int decimals = section.IsHud ? HudDecimalPlaces : DecimalPlaces;
                if (decimals < 0) decimals = 1;
                this.DrawLine(Units.ToPercent(ratio, decimals) + (excess > 0 ? " (+" : " (") + excess + ")", section);
            }
        }

        #endregion
    }
}