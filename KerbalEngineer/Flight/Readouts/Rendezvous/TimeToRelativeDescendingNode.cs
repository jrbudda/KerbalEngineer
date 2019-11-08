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
    public class TimeToRelativeDescendingNode : ReadoutModule
    {
        #region Constructors

        public TimeToRelativeDescendingNode()
        {
            this.Name = Localizer.Format("#KE_TimeToRelativeDescendingNode");//"Time to Rel. DN"
            this.Category = ReadoutCategory.GetCategory("Rendezvous");
            this.HelpString = Localizer.Format("#KE_TimeToRelativeDescendingNode_desc");//"Time until the vessel crosses the target's orbit, going south."
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (RendezvousProcessor.ShowDetails)
            {
                if (RendezvousProcessor.overrideANDN || RendezvousProcessor.overrideANDNRev)
                {
                    double time = RendezvousProcessor.TimeToPlane[1];

                    this.DrawLine("(L) " + TimeFormatter.ConvertToString(time), section.IsHud);
                }
                else
                {
                    this.DrawLine(TimeFormatter.ConvertToString(RendezvousProcessor.TimeToDescendingNode), section.IsHud);
                }
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