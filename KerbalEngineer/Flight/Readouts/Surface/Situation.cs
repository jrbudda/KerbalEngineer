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
    public class Situation : ReadoutModule
    {
        #region Constructors

        public Situation()
        {
            this.Name = Localizer.Format("#KE_Situation");//"Situation"
            this.Category = ReadoutCategory.GetCategory("Surface");
            this.HelpString = Localizer.Format("#KE_Situation_desc");//"Shows the vessel's current scientific situation. (Landed, Splashed, Flying Low/High, In Space Low/High)"
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            switch (ScienceUtil.GetExperimentSituation(FlightGlobals.ActiveVessel))
            {
                case ExperimentSituations.SrfLanded:
                    this.DrawLine(Localizer.Format("#KE_Landed"), section.IsHud);//"Landed"
                    break;

                case ExperimentSituations.SrfSplashed:
                    this.DrawLine(Localizer.Format("#KE_Splashed"), section.IsHud);//"Splashed"
                    break;

                case ExperimentSituations.FlyingLow:
                    this.DrawLine(Localizer.Format("#KE_FlyingLow"), section.IsHud);//"Flying Low"
                    break;

                case ExperimentSituations.FlyingHigh:
                    this.DrawLine(Localizer.Format("#KE_FlyingHigh"), section.IsHud);//"Flying High"
                    break;

                case ExperimentSituations.InSpaceLow:
                    this.DrawLine(Localizer.Format("#KE_SpaceLow"), section.IsHud);//"In Space Low"
                    break;

                case ExperimentSituations.InSpaceHigh:
                    this.DrawLine(Localizer.Format("#KE_SpaceHigh"), section.IsHud);//"In Space High"
                    break;
            }
        }

        #endregion

        #region Methods: private

        private static string GetBiome()
        {
            return ScienceUtil.GetExperimentBiomeLocalized(FlightGlobals.ActiveVessel.mainBody, FlightGlobals.ActiveVessel.latitude, FlightGlobals.ActiveVessel.longitude);
        }

        private static string GetBodyPlural()
        {
            return FlightGlobals.currentMainBody.bodyName.EndsWith("s") ? FlightGlobals.currentMainBody.bodyName + "\'" : FlightGlobals.currentMainBody.bodyName + "\'s";
        }

        #endregion
    }
}