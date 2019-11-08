﻿// 
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
using KSP.Localization;

#endregion

namespace KerbalEngineer.Flight.Readouts.Miscellaneous
{
    public class SystemTime24 : ReadoutModule
    {


        #region Constructors

        public SystemTime24()
        {
            this.Name = Localizer.Format("#KE_SystemTime24");//"System Time"
            this.Category = ReadoutCategory.GetCategory("Miscellaneous");
            this.HelpString = Localizer.Format("#KE_SystemTime24_desc");//"Shows the System Time in 24 hour format"
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            this.DrawLine(DateTime.Now.ToString("HH:mm:ss"), section.IsHud);
        }

        #endregion

    }
}