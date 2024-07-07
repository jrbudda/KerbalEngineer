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

using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Sections;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class SuicideBurnAltitude : ReadoutModule
    {
        #region Constructors

        public SuicideBurnAltitude()
        {
            this.Name = "Suicide Burn Alt.";
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = "Shows the sea level altitude when to start a suicide burn.";
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (!SimulationProcessor.ShowDetails || !Surface.ImpactProcessor.ShowDetails)
            {
                return;
            }

            this.DrawLine(Surface.ImpactProcessor.SuicideAltitude.ToDistance(section.IsHud ? HudDecimalPlaces : DecimalPlaces), section);
        }

        public override void Reset()
        {
           
        }

        public override void Update()
        {
            Surface.ImpactProcessor.RequestUpdate();
        }

        #endregion
    }
}