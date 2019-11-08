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

namespace KerbalEngineer.Flight.Readouts.Rendezvous
{
    public class TimeToPeriapsis : ReadoutModule
    {
        #region Constructors

        public TimeToPeriapsis()
        {
            this.Name = Localizer.Format("#KE_TimeToPeriapsis");//"Time to Periapsis"
            this.Category = ReadoutCategory.GetCategory("Rendezvous");
            this.HelpString = Localizer.Format("#KE_TimeToPeriapsis_Rendezdesc");//"Shows the time until the target reaches periapsis, the lowest point of the orbit."
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (RendezvousProcessor.ShowDetails)
            {
                this.DrawLine(TimeFormatter.ConvertToString(RendezvousProcessor.TimeToPeriapsis), section.IsHud);
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