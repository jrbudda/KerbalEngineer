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

using KerbalEngineer.Flight.Sections;
using KerbalEngineer.Helpers;

using KSP.Localization;

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode
{
    public class NodeTimeToHalfBurn : ReadoutModule
    {
        #region Constructors

        public NodeTimeToHalfBurn()
        {
            this.Name = Localizer.Format("#KE_NodeTimeToHalfBurn");//"Time to Manoeuvre Burn"
            this.Category = ReadoutCategory.GetCategory("Orbital");
            this.HelpString = Localizer.Format("#KE_NodeTimeToHalfBurn_desc");//"Time until the Manoeuvre should be started."
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (!ManoeuvreProcessor.ShowDetails)
            {
                return;
            }

            this.DrawLine("Time to Node Burn", TimeFormatter.ConvertToString(ManoeuvreProcessor.UniversalTime - ManoeuvreProcessor.HalfBurnTime - Planetarium.GetUniversalTime()), section.IsHud);
        }

        public override void Reset()
        {
            ManoeuvreProcessor.Reset();
        }

        public override void Update()
        {
            ManoeuvreProcessor.RequestUpdate();
        }

        #endregion
    }
}