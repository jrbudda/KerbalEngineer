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

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Readouts.Miscellaneous
{
    public class SystemDateTime : ReadoutModule
    {


        #region Constructors

        public SystemDateTime()
        {
            this.Name = "System Time";
            this.Category = ReadoutCategory.GetCategory("Miscellaneous");
            this.HelpString = "Shows the System Date/Time in ISO 8601 format";
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            this.DrawLine(DateTime.Now.ToString("yyyy-MM-dd, HH:mm:ss"), section.Width, section.IsHud);
        }

        #endregion

    }
}