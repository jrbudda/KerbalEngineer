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

    public class ElectricCharge : ReadoutModule
    {
        #region Constructors

        public ElectricCharge() {
            this.Name = "Electric Charge";
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = "Current and maximum electric charge in the vessel.";
            this.IsDefault = false;
        }

        #endregion

        #region Methods

        public override void Draw(Unity.Flight.ISectionModule section) {
            //PartResourceDefinition definition = PartResourceLibrary.Instance.GetDefinition(PartResourceLibrary.ElectricityHashcode); //Seems unnecessary?
            List<Part> parts = FlightGlobals.ActiveVessel.parts;

            double currentEC = 0, maxEC = 0;
            foreach (Part part in parts) {
                foreach (PartResource resource in part.Resources) {
                    if (resource.info.id == PartResourceLibrary.ElectricityHashcode) {
                        currentEC += resource.amount;
                        maxEC += resource.maxAmount;
                    }
                }
            }

            int decimals = section.IsHud ? HudDecimalPlaces : DecimalPlaces;
            if (decimals < 0) decimals = 1;
            this.DrawLine(currentEC.ToString("F" + decimals) + " / " + maxEC.ToString("F" + decimals), section);
        }

        #endregion
    }
}