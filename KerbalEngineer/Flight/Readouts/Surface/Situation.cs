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

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class Situation : ReadoutModule
    {
        #region Constructors

        public Situation()
        {
            this.Name = "Situation";
            this.ShortName = "Sit";
            this.Category = ReadoutCategory.GetCategory("Surface");
            this.HelpString = "Shows the vessel's current scientific situation. (Landed, Splashed, Flying Low/High, In Space Low/High)";
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            switch (ScienceUtil.GetExperimentSituation(FlightGlobals.ActiveVessel))
            {
                case ExperimentSituations.SrfLanded:
                    this.DrawLine("Landed", section);
                    break;

                case ExperimentSituations.SrfSplashed:
                    this.DrawLine("Splashed", section);
                    break;

                case ExperimentSituations.FlyingLow:
                    this.DrawLine("Flying Low", section);
                    break;

                case ExperimentSituations.FlyingHigh:
                    this.DrawLine("Flying High", section);
                    break;

                case ExperimentSituations.InSpaceLow:
                    this.DrawLine("In Space Low", section);
                    break;

                case ExperimentSituations.InSpaceHigh:
                    this.DrawLine("In Space High", section);
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
            return FlightGlobals.currentMainBody.bodyDisplayName.LocalizeRemoveGender().EndsWith("s") ? FlightGlobals.currentMainBody.bodyDisplayName.LocalizeRemoveGender() + "\'" : FlightGlobals.currentMainBody.bodyDisplayName.LocalizeRemoveGender() + "\'s";
        }

        #endregion
    }
}