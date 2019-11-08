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
using KerbalEngineer.Helpers;
using KSP.Localization;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class SuicideBurnLength : ReadoutModule
    {
        #region Constructors

        public SuicideBurnLength()
        {
            this.Name = Localizer.Format("#KE_SuicideBurnLength");//"Suicide Burn Length"
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = Localizer.Format("#KE_SuicideBurnLength_desc");//"Shows the duration of the suicide burn."
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (!SimulationProcessor.ShowDetails || !Surface.ImpactProcessor.ShowDetails) {
                return;
            }

            this.DrawLine(TimeFormatter.ConvertToString(Surface.ImpactProcessor.SuicideLength), section.IsHud);
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