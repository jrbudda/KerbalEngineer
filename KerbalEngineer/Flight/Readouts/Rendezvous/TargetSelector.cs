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

using UnityEngine;

using KSP.Localization;

#endregion

namespace KerbalEngineer.Flight.Readouts.Rendezvous {
    public class TargetSelector : ReadoutModule {
        #region Fields

        private string searchQuery = string.Empty;
        private string searchText = string.Empty;
        private int targetCount;
        private ITargetable targetObject;
        private float typeButtonWidth;
        private bool typeIsBody;
        private bool usingSearch;
        private VesselType vesselType = VesselType.Unknown;

        #endregion

        #region Initialisation

        public TargetSelector() {
            this.Name = Localizer.Format("#KE_TargetSelector");//"Target Selector"
            this.Category = ReadoutCategory.GetCategory("Rendezvous");
            this.HelpString = Localizer.Format("#KE_TargetSelector_desc");//"A tool to allow easy browsing, searching and selection of targets."
            this.IsDefault = true;
        }

        #endregion

        #region Drawing

        #region Methods: public

        /// <summary>
        ///     Draws the target selector structure.
        /// </summary>
        public override void Draw(Unity.Flight.ISectionModule section) {
            if (!HighLogic.LoadedSceneIsFlight) {
                this.DrawTarget(section);
                return;
            }

            if (FlightGlobals.fetch.VesselTarget == null) {
                if (this.vesselType == VesselType.Unknown && !this.typeIsBody) {
                    this.DrawSearch();
                    if (this.searchQuery.Length == 0) {
                        this.DrawTypes();
                    } else {
                        this.DrawTargetList();
                    }
                } else {
                    this.DrawBackToTypes();
                    this.DrawTargetList();
                }
            } else {
                this.DrawTarget(section);
            }

            if (this.targetObject != FlightGlobals.fetch.VesselTarget) {
                this.targetObject = FlightGlobals.fetch.VesselTarget;
                this.ResizeRequested = true;
            }
        }

        #endregion

        #region Methods: private

        /// <summary>
        ///     Draws the back to types button.
        /// </summary>
        private void DrawBackToTypes() {
            if (GUILayout.Button(Localizer.Format("#KE_BacktoTypebutton"), this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {//"Go Back to Type Selection"
                this.typeIsBody = false;
                this.vesselType = VesselType.Unknown;
                this.ResizeRequested = true;
            }

            GUILayout.Space(3f);
        }

        /// <summary>
        ///     Draws targetable moons.
        /// </summary>
        private int DrawMoons() {
            var count = 0;

            foreach (var body in FlightGlobals.Bodies) {
                if (FlightGlobals.ActiveVessel.mainBody != body.referenceBody || body == Planetarium.fetch.Sun) {
                    continue;
                }

                if (this.searchQuery.Length > 0 && !body.bodyName.ToLower().Contains(this.searchQuery)) {
                    continue;
                }

                count++;
                if (GUILayout.Button(body.bodyName, this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {
                    this.SetTargetAs(body);
                }
            }
            return count;
        }

        /// <summary>
        ///     Draws the targetable planets.
        /// </summary>
        private int DrawPlanets() {
            var count = 0;
            foreach (var body in FlightGlobals.Bodies) {
                if (FlightGlobals.ActiveVessel.mainBody == Planetarium.fetch.Sun || FlightGlobals.ActiveVessel.mainBody.referenceBody != body.referenceBody || body == Planetarium.fetch.Sun || body == FlightGlobals.ActiveVessel.mainBody) {
                    continue;
                }

                if (this.searchQuery.Length > 0 && !body.bodyName.ToLower().Contains(this.searchQuery)) {
                    continue;
                }

                count++;
                if (GUILayout.Button(body.GetName(), this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {
                    this.SetTargetAs(body);
                }
            }
            return count;
        }

        /// <summary>
        ///     Draws the search bar.
        /// </summary>
        private void DrawSearch() {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.Format("#KE_SEARCH"), this.FlexiLabelStyle, GUILayout.Width(60.0f * GuiDisplaySize.Offset));//"SEARCH:"

            this.searchText = GUILayout.TextField(this.searchText, this.TextFieldStyle);

            if (this.searchText.Length > 0 || this.searchQuery.Length > 0) {
                this.searchQuery = this.searchText.ToLower();

                if (!this.usingSearch) {
                    this.usingSearch = true;
                    this.ResizeRequested = true;
                }
            } else if (this.usingSearch) {
                this.usingSearch = false;
                this.ResizeRequested = true;
            }

            GUILayout.EndHorizontal();
        }

        private bool wasMapview;

        /// <summary>
        ///     Draws the target information when selected.
        /// </summary>
        private void DrawTarget(Unity.Flight.ISectionModule section) {
            ITargetable target = Flight.Readouts.Rendezvous.RendezvousProcessor.activeTarget;

            this.ResizeRequested = true;

            if (target != null) {

                if (HighLogic.LoadedSceneIsFlight) {
                    if (GUILayout.Button(Localizer.Format("#KE_BacktoTypebutton"), this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {//"Go Back to Target Selection"
                        FlightGlobals.fetch.SetVesselTarget(null);
                    }
                } else {
                    if (RendezvousProcessor.TrackingStationSource != target)
                        if (GUILayout.Button(Localizer.Format("#KE_Target", RendezvousProcessor.nameForTargetable(target)), this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {//"Use " +  + " As Reference"
                            RendezvousProcessor.TrackingStationSource = target;
                        }
                }

                if (HighLogic.LoadedSceneIsFlight) {
                    var act = FlightGlobals.ActiveVessel;

                    if (act == null) return; //wat

                    if (!(target is CelestialBody) && GUILayout.Button(Localizer.Format("#KE_SwitchtoTarget"), this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {
                        FlightEngineerCore.SwitchToVessel(target.GetVessel(), act);//"Switch to Target"
                    }

                    bool focusable = (target is CelestialBody || target is global::Vessel);

                    if (focusable) {
                        MapObject targMo = null;

                        if (target is global::Vessel)
                            targMo = ((global::Vessel)(target)).mapObject;
                        else
                            targMo = ((CelestialBody)(target)).MapObject;

                        bool shouldFocus = targMo != null && (targMo != PlanetariumCamera.fetch.target || !MapView.MapIsEnabled);

                        if (shouldFocus && GUILayout.Button(Localizer.Format("#KE_FocusTarget"), this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {//"Focus Target"
                            wasMapview = MapView.MapIsEnabled;
                            MapView.EnterMapView();
                            PlanetariumCamera.fetch.SetTarget(targMo);
                        }
                    }

                    bool switchBack = PlanetariumCamera.fetch.target != act.mapObject;

                    if (switchBack && MapView.MapIsEnabled && GUILayout.Button(Localizer.Format("#KE_FocusVessel"), this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {//"Focus Vessel"
                        PlanetariumCamera.fetch.SetTarget(act.mapObject);
                        if (!wasMapview) MapView.ExitMapView();
                    }

                    if (FlightCamera.fetch.mode != FlightCamera.Modes.LOCKED && !MapView.MapIsEnabled && GUILayout.Button(Localizer.Format("#KE_LookatTarget"), this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {//"Look at Target"
                        var pcam = PlanetariumCamera.fetch;
                        var fcam = FlightCamera.fetch;

                        Vector3 from = new Vector3();

                        if (target is global::Vessel && ((global::Vessel)target).LandedOrSplashed) {
                            from = ((global::Vessel)target).GetWorldPos3D();
                        } else {
                            //I don't think it's possible to target the sun so this should always work but who knows.
                            if (target.GetOrbit() != null)
                                from = target.GetOrbit().getTruePositionAtUT(Planetarium.GetUniversalTime());
                        }


                        Vector3 to = FlightGlobals.fetch.activeVessel.GetWorldPos3D();

                        //  float pdist = pcam.Distance; 
                        float fdist = fcam.Distance;

                        Vector3 n = (from - to).normalized;

                        if (!n.IsInvalid()) {
                            //   pcam.SetCamCoordsFromPosition(n * -pdist); //this does weird stuff
                            fcam.SetCamCoordsFromPosition(n * -fdist);
                        }

                    }
                }

                GUILayout.Space(3f);

                this.DrawLine(Localizer.Format("#KE_SelectedTarget"), RendezvousProcessor.nameForTargetable(target), section.IsHud);//"Selected Target"

                try {

                    if (RendezvousProcessor.sourceDisplay != null) {
                        if (RendezvousProcessor.landedSamePlanet || RendezvousProcessor.overrideANDN)
                            this.DrawLine(Localizer.Format("#KE_RefOrbit"), Localizer.Format("#KE_Landedon", RendezvousProcessor.activeVessel.GetOrbit().referenceBody.GetName()), section.IsHud);//"Ref Orbit""Landed on <<1>>"
                        else
                            this.DrawLine(Localizer.Format("#KE_RefOrbit"), RendezvousProcessor.sourceDisplay, section.IsHud);//"Ref Orbit"
                    }

                    if (RendezvousProcessor.targetDisplay != null) {
                        if (RendezvousProcessor.landedSamePlanet || RendezvousProcessor.overrideANDNRev)
                            this.DrawLine(Localizer.Format("#KE_TargetOrbit"), Localizer.Format("#KE_Landedon", target.GetOrbit().referenceBody.GetName()), section.IsHud);//"Target Orbit""Landed on " 
                        else
                            this.DrawLine(Localizer.Format("#KE_TargetOrbit"), RendezvousProcessor.targetDisplay, section.IsHud);//"Target Orbit"
                    }

                } catch (System.Exception) {
                    Debug.Log(" target " + target + " " + RendezvousProcessor.activeVessel + " " + target.GetOrbit() + " " + RendezvousProcessor.overrideANDN + " " + RendezvousProcessor.overrideANDNRev + " " + RendezvousProcessor.landedSamePlanet);
                }

            }
        }

        /// <summary>
        ///     Draws the target list.
        /// </summary>
        private void DrawTargetList() {
            var count = 0;

            if (this.searchQuery.Length == 0) {
                if (this.typeIsBody) {
                    GUILayout.Label(Localizer.Format("#KE_LocalBodies"), this.FlexiLabelStyle, GUILayout.Width(this.ContentWidth));//"Local Bodies"
                    count += this.DrawMoons();
                    GUILayout.Label(Localizer.Format("#KE_RemoteBodies"), this.FlexiLabelStyle, GUILayout.Width(this.ContentWidth));//"Remote Bodies"
                    count += this.DrawPlanets();
                } else {
                    GUILayout.Label(this.vesselType.ToString(), this.FlexiLabelStyle, GUILayout.Width(this.ContentWidth));
                    count += this.DrawVessels();
                }
            } else {
                GUILayout.Label(Localizer.Format("#KE_SearchResults"), this.FlexiLabelStyle, GUILayout.Width(this.ContentWidth));//"Search Results"
                count += this.DrawVessels();
                count += this.DrawMoons();
                count += this.DrawPlanets();
            }

            if (count == 0) {
                this.DrawMessageLine(Localizer.Format("#KE_Notargets"));//"No targets found!"
            }

            if (count != this.targetCount) {
                this.targetCount = count;
                this.ResizeRequested = true;
            }
        }

        /// <summary>
        ///     Draws the button list of target types.
        /// </summary>
        private void DrawTypes() {
            this.typeButtonWidth = Mathf.Round(this.ContentWidth * 0.5f);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localizer.Format("#KE_CelestialBodies"), this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {//"Celestial Bodies"
                this.SetTypeAsBody();
            }
            if (GUILayout.Button(Localizer.Format("#KE_Debris"), this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {//"Debris"
                this.SetTypeAs(VesselType.Debris);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localizer.Format("#KE_Probes"), this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {//"Probes"
                this.SetTypeAs(VesselType.Probe);
            }
            if (GUILayout.Button(Localizer.Format("#KE_Relays"), this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {//"Relays"
                this.SetTypeAs(VesselType.Relay);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localizer.Format("#KE_Rovers"), this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {//"Rovers"
                this.SetTypeAs(VesselType.Rover);
            }
            if (GUILayout.Button(Localizer.Format("#KE_Landers"), this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {//"Landers"
                this.SetTypeAs(VesselType.Lander);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localizer.Format("#KE_Ships"), this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {//"Ships"
                this.SetTypeAs(VesselType.Ship);
            }
            if (GUILayout.Button(Localizer.Format("#KE_Planes"), this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {//"Planes"
                this.SetTypeAs(VesselType.Plane);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localizer.Format("#KE_Stations"), this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {//"Stations"
                this.SetTypeAs(VesselType.Station);
            }
            if (GUILayout.Button(Localizer.Format("#KE_Bases"), this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {//"Bases"
                this.SetTypeAs(VesselType.Base);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localizer.Format("#KE_EVAs"), this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {//"EVAs"
                this.SetTypeAs(VesselType.EVA);
            }
            if (GUILayout.Button(Localizer.Format("#KE_Flags"), this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {//"Flags"
                this.SetTypeAs(VesselType.Flag);
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///     Draws targetable vessels.
        /// </summary>
        private int DrawVessels() {
            var count = 0;
            foreach (var vessel in FlightGlobals.Vessels) {
                if (vessel == FlightGlobals.ActiveVessel || (this.searchQuery.Length == 0 && vessel.vesselType != this.vesselType)) {
                    continue;
                }

                if (this.searchQuery.Length == 0) {
                    count++;

                    if (GUILayout.Button(vessel.GetName(), this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {
                        this.SetTargetAs(vessel);
                    }
                } else if (vessel.vesselName.ToLower().Contains(this.searchQuery)) {
                    count++;
                    if (GUILayout.Button(vessel.GetName(), this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {
                        this.SetTargetAs(vessel);
                    }
                }
            }
            return count;
        }

        private void SetTargetAs(ITargetable target) {
            FlightGlobals.fetch.SetVesselTarget(target);
            //this.targetObject = target;
            this.ResizeRequested = true;
        }

        private void SetTypeAs(VesselType vesselType) {
            this.vesselType = vesselType;
            this.ResizeRequested = true;
        }

        private void SetTypeAsBody() {
            this.typeIsBody = true;
            this.ResizeRequested = true;
        }

        public override void Update() {
            RendezvousProcessor.RequestUpdate();
        }

        #endregion

        #endregion
    }
}