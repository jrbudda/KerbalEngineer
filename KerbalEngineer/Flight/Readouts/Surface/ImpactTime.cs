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
using KerbalEngineer.Helpers;
using KSP.Localization;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class ImpactTime : ReadoutModule
    {
        #region Constructors

        public ImpactTime()
        {
            this.Name = Localizer.Format("#KE_ImpactTime");//"Impact Time"
            this.Category = ReadoutCategory.GetCategory("Surface");
            this.HelpString = Localizer.Format("#KE_ImpactTime_desc");//"Shows time until the vessel impacts the central object."
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (ImpactProcessor.ShowDetails)
            {
                this.DrawLine(TimeFormatter.ConvertToString(ImpactProcessor.Time), section.IsHud);
            }
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(ImpactProcessor.Instance);
        }

        public override void Update()
        {
            ImpactProcessor.RequestUpdate();
        }

        #endregion
    }
}