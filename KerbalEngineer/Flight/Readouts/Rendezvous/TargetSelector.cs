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
using static System.Collections.Specialized.BitVector32;

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
            this.Name = "Target Selector";
            this.Category = ReadoutCategory.GetCategory("Rendezvous");
            this.HelpString = "A tool to allow easy browsing, searching and selection of targets.";
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
            if (GUILayout.Button("Go Back to Type Selection", this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {
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

                if (this.searchQuery.Length > 0 && !body.bodyDisplayName.LocalizeRemoveGender().ToLower().Contains(this.searchQuery)) {
                    continue;
                }

                count++;
                if (GUILayout.Button(body.bodyDisplayName.LocalizeRemoveGender(), this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {
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

                if (this.searchQuery.Length > 0 && !body.bodyDisplayName.LocalizeRemoveGender().ToLower().Contains(this.searchQuery)) {
                    continue;
                }

                count++;
                if (GUILayout.Button(body.GetDisplayName().LocalizeRemoveGender(), this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {
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
            GUILayout.Label("SEARCH:", this.FlexiLabelStyle, GUILayout.Width(60.0f * GuiDisplaySize.Offset));

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
                    if (GUILayout.Button("Go Back to Target Selection", this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {
                        FlightGlobals.fetch.SetVesselTarget(null);
                    }
                } else {
                    if (RendezvousProcessor.TrackingStationSource != target)
                        if (GUILayout.Button("Use " + RendezvousProcessor.nameForTargetable(target) + " As Reference", this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {
                            RendezvousProcessor.TrackingStationSource = target;
                        }
                }

                if (HighLogic.LoadedSceneIsFlight) {
                    var act = FlightGlobals.ActiveVessel;

                    if (act == null) return; //wat

                    if (!(target is CelestialBody) && GUILayout.Button("Switch to Target", this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {
                        FlightEngineerCore.SwitchToVessel(target.GetVessel(), act);
                    }

                    bool focusable = (target is CelestialBody || target is global::Vessel);

                    if (focusable) {
                        MapObject targMo = null;

                        if (target is global::Vessel)
                            targMo = ((global::Vessel)(target)).mapObject;
                        else
                            targMo = ((CelestialBody)(target)).MapObject;

                        bool shouldFocus = targMo != null && (targMo != PlanetariumCamera.fetch.target || !MapView.MapIsEnabled);

                        if (shouldFocus && GUILayout.Button("Focus Target", this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {
                            wasMapview = MapView.MapIsEnabled;
                            MapView.EnterMapView();
                            PlanetariumCamera.fetch.SetTarget(targMo);
                        }
                    }

                    bool switchBack = PlanetariumCamera.fetch.target != act.mapObject;

                    if (switchBack && MapView.MapIsEnabled && GUILayout.Button("Focus Vessel", this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {
                        PlanetariumCamera.fetch.SetTarget(act.mapObject);
                        if (!wasMapview) MapView.ExitMapView();
                    }

                    if (FlightCamera.fetch.mode != FlightCamera.Modes.LOCKED && !MapView.MapIsEnabled && GUILayout.Button("Look at Target", this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {
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

                this.DrawLine("Selected Target", RendezvousProcessor.nameForTargetable(target), section);

                try {

                    if (RendezvousProcessor.sourceDisplay != null) {
                        if (RendezvousProcessor.landedSamePlanet || RendezvousProcessor.overrideANDN)
                            this.DrawLine("Ref Orbit", "Landed on " + RendezvousProcessor.activeVessel.GetOrbit().referenceBody.GetDisplayName().LocalizeRemoveGender(), section);
                        else
                            this.DrawLine("Ref Orbit", RendezvousProcessor.sourceDisplay, section);
                    }

                    if (RendezvousProcessor.targetDisplay != null) {
                        if (RendezvousProcessor.landedSamePlanet || RendezvousProcessor.overrideANDNRev)
                            this.DrawLine("Target Orbit", "Landed on " + target.GetOrbit().referenceBody.GetDisplayName().LocalizeRemoveGender(), section);
                        else
                            this.DrawLine("Target Orbit", RendezvousProcessor.targetDisplay, section);
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
                    GUILayout.Label("Local Bodies", this.FlexiLabelStyle, GUILayout.Width(this.ContentWidth));
                    count += this.DrawMoons();
                    GUILayout.Label("Remote Bodies", this.FlexiLabelStyle, GUILayout.Width(this.ContentWidth));
                    count += this.DrawPlanets();
                } else {
                    GUILayout.Label(this.vesselType.ToString(), this.FlexiLabelStyle, GUILayout.Width(this.ContentWidth));
                    count += this.DrawVessels();
                }
            } else {
                GUILayout.Label("Search Results", this.FlexiLabelStyle, GUILayout.Width(this.ContentWidth));
                count += this.DrawVessels();
                count += this.DrawMoons();
                count += this.DrawPlanets();
            }

            if (count == 0) {
                this.DrawMessageLine("No targets found!", Unity.Flight.OOPSux.DEFAULT_SECTION_WIDTH);
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
            if (GUILayout.Button("Celestial Bodies", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {
                this.SetTypeAsBody();
            }
            if (GUILayout.Button("Debris", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {
                this.SetTypeAs(VesselType.Debris);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Probes", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {
                this.SetTypeAs(VesselType.Probe);
            }
            if (GUILayout.Button("Relays", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {
                this.SetTypeAs(VesselType.Relay);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Rovers", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {
                this.SetTypeAs(VesselType.Rover);
            }
            if (GUILayout.Button("Landers", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {
                this.SetTypeAs(VesselType.Lander);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Ships", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {
                this.SetTypeAs(VesselType.Ship);
            }
            if (GUILayout.Button("Planes", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {
                this.SetTypeAs(VesselType.Plane);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Stations", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {
                this.SetTypeAs(VesselType.Station);
            }
            if (GUILayout.Button("Bases", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {
                this.SetTypeAs(VesselType.Base);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("EVAs", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {
                this.SetTypeAs(VesselType.EVA);
            }
            if (GUILayout.Button("Flags", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth))) {
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

                    if (GUILayout.Button(vessel.GetDisplayName().LocalizeRemoveGender(), this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {
                        this.SetTargetAs(vessel);
                    }
                } else if (vessel.vesselName.ToLower().Contains(this.searchQuery)) {
                    count++;
                    if (GUILayout.Button(vessel.GetDisplayName().LocalizeRemoveGender(), this.ButtonStyle, GUILayout.Width(this.ContentWidth))) {
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