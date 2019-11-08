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

using KerbalEngineer.Flight.Sections;

using KSP.Localization;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class Biome : ReadoutModule
    {
        #region Constructors

        public Biome()
        {
            this.Name = Localizer.Format("#KE_Biome");//"Biome"
            this.Category = ReadoutCategory.GetCategory("Surface");
            this.HelpString = Localizer.Format("#KE_Biome_desc");//"Shows the biome which the vessel is currently flying over."
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            var biome = ScienceUtil.GetExperimentBiome(FlightGlobals.ActiveVessel.mainBody, FlightGlobals.ActiveVessel.latitude, FlightGlobals.ActiveVessel.longitude);
            biome = ScienceUtil.GetBiomedisplayName(FlightGlobals.ActiveVessel.mainBody, biome);
            this.DrawLine(biome, section.IsHud);
        }

        #endregion
    }
}