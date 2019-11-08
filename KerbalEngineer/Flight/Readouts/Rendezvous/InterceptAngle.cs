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

namespace KerbalEngineer.Flight.Readouts.Rendezvous
{
    public class InterceptAngle : ReadoutModule
    {
        #region Constructors

        public InterceptAngle()
        {
            this.Name = Localizer.Format("#KE_InterceptAngle");//"Transfer Angle"
            this.Category = ReadoutCategory.GetCategory("Rendezvous");
            this.HelpString = Localizer.Format("#KE_InterceptAngle_desc");//"The phase angle for starting a Hohmann transfer."
            this.HelpString = string.Empty;
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (RendezvousProcessor.ShowDetails)
            {
                this.DrawLine(RendezvousProcessor.InterceptAngle.ToAngle(), section.IsHud);
            }
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(RendezvousProcessor.Instance);
        }

        public override void Update()
        {
            RendezvousProcessor.RequestUpdate();
        }

        #endregion
    }
}