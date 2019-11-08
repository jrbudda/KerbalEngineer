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

using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Sections;

using KSP.Localization;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class TerminalVelocity : ReadoutModule
    {
        #region Constructors

        public TerminalVelocity()
        {
            this.Name = Localizer.Format("#KE_TerminalVelocity");//"Terminal Velocity"
            this.Category = ReadoutCategory.GetCategory("Surface");
            this.HelpString = Localizer.Format("#KE_TerminalVelocity_desc");//"Shows the velocity where the efforts of thrust and drag are equalled out."
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (AtmosphericProcessor.ShowDetails)
            {
                this.DrawLine(AtmosphericProcessor.TerminalVelocity.ToSpeed(), section.IsHud);
            }
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(AtmosphericProcessor.Instance);
        }

        public override void Update()
        {
            AtmosphericProcessor.RequestUpdate();
        }

        #endregion
    }
}