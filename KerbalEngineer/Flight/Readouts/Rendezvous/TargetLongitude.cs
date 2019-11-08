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

namespace KerbalEngineer.Flight.Readouts.Surface
{
    using Helpers;
    using Rendezvous;
    using Sections;
    using KSP.Localization;

    public class TargetLongitude : ReadoutModule
    {
        public TargetLongitude()
        {
            Name = Localizer.Format("#KE_TargetLongitude");//"Target Longitude"
            Category = ReadoutCategory.GetCategory("Rendezvous");
            HelpString = Localizer.Format("#KE_TargetLongitude_desc");//"Shows the target vessel's longitude around a celestial body. Longitude is the angle from the bodies prime meridian."
            IsDefault = false;
        }

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (!RendezvousProcessor.ShowDetails) return;

            var target = Rendezvous.RendezvousProcessor.targetVessel;
            if (target != null)
            {
                var vessel = target.GetVessel();
                if(vessel == null)
                {
                    DrawLine("N/A", section.IsHud);
                } else
                {
                    double longitude = AngleHelper.Clamp180(vessel.longitude);
                    DrawLine(Units.ToAngleDMS(longitude) + (longitude < 0.0 ? " W" : " E"), section.IsHud);
                }
            }
        }

        public override void Update() {
            RendezvousProcessor.RequestUpdate();
        }
    }
}