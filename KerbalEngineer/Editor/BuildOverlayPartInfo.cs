// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2016 CYBUTEK
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
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  

namespace KerbalEngineer.Editor
{
    using System;
    using System.Collections.Generic;
    using Extensions;
    using Helpers;
    using KeyBinding;
    using KSP.UI.Screens;
    using Unity;
    using UnityEngine;
    using KeyBinding = global::KeyBinding;
    using KSP.Localization;

    public class BuildOverlayPartInfo : MonoBehaviour
    {
        private static bool clickToOpen = true;
        private static bool namesOnly;
        private static bool visible = true;

        private readonly List<PartInfoItem> infoItems = new List<PartInfoItem>();

        private Rect position;
        private Part selectedPart;
        private bool showInfo;
        private bool skipFrame;
        private PointerHoverDetector stageUiPointerHoverDetector;

        public static bool ClickToOpen
        {
            get
            {
                return clickToOpen;
            }

            set
            {
                clickToOpen = value;
            }
        }

        public static bool Hidden { get; set; }

        public static bool NamesOnly
        {
            get
            {
                return namesOnly;
            }

            set
            {
                namesOnly = value;
            }
        }

        public static bool Visible
        {
            get
            {
                return visible;
            }

            set
            {
                visible = value;
            }
        }

        protected void OnGUI()
        {
            try
            {
                if (!Visible || Hidden || selectedPart == null || (EditorPanels.Instance.IsMouseOver() && IsPointerOverStaging() == false))
                {
                    return;
                }

                position = GUILayout.Window(GetInstanceID(), position, Window, string.Empty, BuildOverlay.WindowStyle);
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }

        protected void Update()
        {
            try
            {
                if (!Visible || Hidden || EditorLogic.RootPart == null || (EditorPanels.Instance.IsMouseOver() && IsPointerOverStaging() == false))
                {
                    return;
                }

                position.x = Mathf.Clamp(Input.mousePosition.x + 16.0f, 0.0f, Screen.width - position.width);
                position.y = Mathf.Clamp(Screen.height - Input.mousePosition.y, 0.0f, Screen.height - position.height);
                if (position.x < Input.mousePosition.x + 20.0f)
                {
                    position.y = Mathf.Clamp(position.y + 20.0f, 0.0f, Screen.height - position.height);
                }

                if (position.x < Input.mousePosition.x + 16.0f && position.y < Screen.height - Input.mousePosition.y)
                {
                    position.x = Input.mousePosition.x - 3 - position.width;
                }

                RaycastHit rayHit;
                Part part = null;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayHit))
                {
                    // MyLogger.Log("Raycast returned true");
                    part = rayHit.transform.GetComponent<Part>();
                }
                else
                {
                    // MyLogger.Log("Raycast returned false");
                    part = EditorLogic.fetch.ship.parts.Find(p => p.HighlightActive) ?? EditorLogic.SelectedPart;
                }

                if (part != null)
                {
                    if (!part.Equals(selectedPart))
                    {
                        selectedPart = part;
                        ResetInfo();
                    }

                    if (NamesOnly || skipFrame)
                    {
                        skipFrame = false;
                        return;
                    }

                    if (!showInfo && Input.GetKeyDown(KeyBinder.PartInfoShowHide))
                    {
                        showInfo = true;
                    }
                    else if (ClickToOpen && showInfo && Input.GetKeyDown(KeyBinder.PartInfoShowHide))
                    {
                        ResetInfo();
                    }

                    if (showInfo)
                    {
                        PartInfoItem.Release(infoItems);
                        infoItems.Clear();
                        SetCostInfo();
                        SetMassItems();
                        SetResourceItems();
                        SetEngineInfo();
                        SetAlternatorInfo();
                        SetGimbalInfo();
                        SetRcsInfo();
                        SetParachuteInfo();
                        SetSasInfo();
                        SetReactionWheelInfo();
                        SetSolarPanelInfo();
                        SetGeneratorInfo();
                        SetDecouplerInfo();
                        SetTransmitterInfo();
                        SetScienceExperimentInfo();
                        SetScienceContainerInfo();
                        SetSingleActivationInfo();
                    }
                }
                else
                {
                    selectedPart = null;
                }
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }

        private bool IsPointerOverStaging()
        {
            if (stageUiPointerHoverDetector == null)
            {
                stageUiPointerHoverDetector = StageManager.Instance.scrollRect.gameObject.AddOrGetComponent<PointerHoverDetector>();
            }

            if (stageUiPointerHoverDetector != null)
            {
                return stageUiPointerHoverDetector.IsPointerHovering;
            }

            return false;
        }

        private void ResetInfo()
        {
            showInfo = !clickToOpen;
            skipFrame = true;
            position.width = namesOnly || clickToOpen ? 0.0f : 200.0f;
            position.height = 0.0f;
        }

        private void SetAlternatorInfo()
        {
            ModuleAlternator moduleAlternator = selectedPart.GetModule<ModuleAlternator>();
            if (moduleAlternator != null)
            {
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_Alternator")));//"Alternator"
                for (int i = 0; i < moduleAlternator.resHandler.outputResources.Count; ++i)
                {
                    var moduleResource = moduleAlternator.resHandler.outputResources[i];
                    infoItems.Add(PartInfoItem.Create("\t" + moduleResource.name, moduleResource.rate.ToRate()));
                }
            }
        }

        private void SetCostInfo()
        {
            infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_Cost"), Units.ConcatF(selectedPart.GetCostDry(), selectedPart.GetCostWet())));//"Cost"
        }

        private void SetDecouplerInfo()
        {
            var protoModuleDecoupler = selectedPart.GetProtoModuleDecoupler();
            if (protoModuleDecoupler != null)
            {
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_EjectionForce"), protoModuleDecoupler.EjectionForce.ToForce()));//"Ejection Force"
                if (protoModuleDecoupler.IsOmniDecoupler)
                {
                    infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_Omni_directional")));//"Omni-directional"
                }
            }
        }

        private void SetEngineInfo()
        {
            var protoModuleEngine = selectedPart.GetProtoModuleEngine();
            if (protoModuleEngine != null)
            {
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_Thrust"), Units.ToForce(protoModuleEngine.MinimumThrust, protoModuleEngine.MaximumThrust)));//"Thrust"
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_Isp"), Units.ConcatF(protoModuleEngine.GetSpecificImpulse(1.0f), protoModuleEngine.GetSpecificImpulse(0.0f)) + "s"));//"Isp"
                if (protoModuleEngine.Propellants.Count > 0)
                {
                    infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_Propellants")));//"Propellants"

                    float totalRatio = 0.0f;
                    for (int i = 0; i < protoModuleEngine.Propellants.Count; ++i)
                    {
                        totalRatio = totalRatio + protoModuleEngine.Propellants[i].ratio;
                    }

                    for (int i = 0; i < protoModuleEngine.Propellants.Count; ++i)
                    {
                        var propellant = protoModuleEngine.Propellants[i];
                        infoItems.Add(PartInfoItem.Create("\t" + propellant.name, (propellant.ratio / totalRatio).ToPercent()));
                    }
                }
            }
        }

        private void SetGeneratorInfo()
        {
            var moduleGenerator = selectedPart.GetModule<ModuleGenerator>();
            if (moduleGenerator != null)
            {
                if (moduleGenerator.resHandler.inputResources.Count > 0)
                {
                    infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_GeneratorInput")));//"Generator Input"
                    for (int i = 0; i < moduleGenerator.resHandler.inputResources.Count; ++i)
                    {
                        var generatorResource = moduleGenerator.resHandler.inputResources[i];
                        infoItems.Add(PartInfoItem.Create("\t" + generatorResource.name, generatorResource.rate.ToRate()));
                    }
                }

                if (moduleGenerator.resHandler.outputResources.Count > 0)
                {
                    infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_GeneratorOnput")));//"Generator Output"
                    for (int i = 0; i < moduleGenerator.resHandler.outputResources.Count; ++i)
                    {
                        var generatorResource = moduleGenerator.resHandler.outputResources[i];
                        infoItems.Add(PartInfoItem.Create("\t" + generatorResource.name, generatorResource.rate.ToRate()));
                    }
                }

                if (moduleGenerator.isAlwaysActive)
                {
                    infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_GeneratorisAlwaysActive")));//"Generator is Always Active"
                }
            }
        }

        private void SetGimbalInfo()
        {
            var moduleGimbal = selectedPart.GetModule<ModuleGimbal>();
            if (moduleGimbal != null)
            {
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_ThrustVectoring"), moduleGimbal.gimbalRange.ToString("F2")));//"Thrust Vectoring"
            }
        }

        private void SetMassItems()
        {
            if (selectedPart.physicalSignificance == Part.PhysicalSignificance.FULL)
            {
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_Mass"), Units.ToMass(selectedPart.GetDryMass(), selectedPart.GetWetMass())));//"Mass"
            }
        }

        private void SetParachuteInfo()
        {
            var moduleParachute = selectedPart.GetModule<ModuleParachute>();
            if (moduleParachute != null)
            {
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_DeployedDrag"), Units.ConcatF(moduleParachute.semiDeployedDrag, moduleParachute.fullyDeployedDrag)));//"Deployed Drag"
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_DeploymentAltitude"), moduleParachute.deployAltitude.ToDistance()));//"Deployment Altitude"
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_DeploymentPressure"), moduleParachute.minAirPressureToOpen.ToString("F2")));//"Deployment Pressure"
            }
        }

        private void SetRcsInfo()
        {
            var moduleRcs = selectedPart.GetModule<ModuleRCS>();
            if (moduleRcs != null)
            {
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_ThrusterPower"), moduleRcs.thrusterPower.ToForce()));//"Thruster Power"
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_SpecificImpulse"), Units.ConcatF(moduleRcs.atmosphereCurve.Evaluate(1.0f), moduleRcs.atmosphereCurve.Evaluate(0.0f)) + "s"));//"Specific Impulse"
            }
        }

        private void SetReactionWheelInfo()
        {
            var moduleReactionWheel = selectedPart.GetModule<ModuleReactionWheel>();
            if (moduleReactionWheel != null)
            {
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_ReactionWheelInfo")));//"Reaction Wheel Torque"
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_ReactionWheelInfo_Pitch"), moduleReactionWheel.PitchTorque.ToTorque()));//"\tPitch"
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_ReactionWheelInfo_Roll"), moduleReactionWheel.RollTorque.ToTorque()));//"\tRoll"
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_ReactionWheelInfo_Yaw"), moduleReactionWheel.YawTorque.ToTorque()));//"\tYaw"
                for (int i = 0; i < moduleReactionWheel.resHandler.inputResources.Count; ++i)
                {
                    var moduleResource = moduleReactionWheel.resHandler.inputResources[i];
                    infoItems.Add(PartInfoItem.Create("\t" + moduleResource.name, moduleResource.rate.ToRate()));
                }
            }
        }

        private void SetResourceItems()
        {
            bool visibleResources = false;
            for (int i = 0; i < selectedPart.Resources.dict.Count; ++i)
            {
                if (selectedPart.Resources.dict.At(i).hideFlow == false)
                {
                    visibleResources = true;
                    break;
                }
            }

            if (visibleResources)
            {
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_Resources")));//"Resources"
                for (int i = 0; i < selectedPart.Resources.dict.Count; ++i)
                {
                    var partResource = selectedPart.Resources.dict.At(i);

                    if (partResource.hideFlow == false)
                    {
                        infoItems.Add(partResource.GetDensity() > 0
                                          ? PartInfoItem.Create("\t" + partResource.info.name, "(" + partResource.GetMass().ToMass() + ") " + partResource.amount.ToString("F1"))
                                          : PartInfoItem.Create("\t" + partResource.info.name, partResource.amount.ToString("F1")));
                    }
                }
            }
        }

        private void SetSasInfo()
        {
            if (selectedPart.HasModule<ModuleSAS>())
            {
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_SASEquiped")));//"SAS Equiped"
            }
        }

        private void SetScienceContainerInfo()
        {
            if (selectedPart.HasModule<ModuleScienceContainer>())
            {
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_ScienceContainer")));//"Science Container"
            }
        }

        private void SetScienceExperimentInfo()
        {
            var moduleScienceExperiment = selectedPart.GetModule<ModuleScienceExperiment>();
            if (moduleScienceExperiment != null)
            {
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_ScienceExperiment"), moduleScienceExperiment.experimentActionName));//"Science Experiment"
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_TransmitEfficiency"), moduleScienceExperiment.xmitDataScalar.ToPercent()));//"\tTransmit Efficiency"
                if (moduleScienceExperiment.rerunnable == false)
                {
                    infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_SingleUsage")));//"\tSingle Usage"
                }
            }
        }

        private void SetSingleActivationInfo()
        {
            if (selectedPart.HasModule<ModuleAnimateGeneric>(m => m.isOneShot))
            {
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_Partinfo_SingleActivation")));//"Single Activation"
            }
        }

        private void SetSolarPanelInfo()
        {
            var moduleDeployableSolarPanel = selectedPart.GetModule<ModuleDeployableSolarPanel>();
            if (moduleDeployableSolarPanel != null)
            {
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_SolarPanelInfo_ChargeRate"), moduleDeployableSolarPanel.chargeRate.ToRate()));//"Charge Rate"
                if (moduleDeployableSolarPanel.isBreakable)
                {
                    infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_SolarPanelInfo_Breakable")));//"Breakable"
                }

                if (moduleDeployableSolarPanel.trackingBody == Sun.Instance)
                {
                    infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_SolarPanelInfo_SunTracking")));//"Sun Tracking"
                }
            }
        }

        private void SetTransmitterInfo()
        {
            var moduleDataTransmitter = selectedPart.GetModule<ModuleDataTransmitter>();
            if (moduleDataTransmitter != null)
            {
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_TransmitterInfo_PacketSize"), moduleDataTransmitter.packetSize.ToString("F2") + " Mits"));//"Packet Size"
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_TransmitterInfo_Bandwidth"), (moduleDataTransmitter.packetInterval * moduleDataTransmitter.packetSize).ToString("F2") + "Mits/sec"));//"Bandwidth"

                // TODO: allow for multiple consumed resources
                infoItems.Add(PartInfoItem.Create(moduleDataTransmitter.GetConsumedResources()[0].name, Localizer.Format("#KE_TransmitterInfo_Packet", moduleDataTransmitter.packetResourceCost.ToString("F2"))));// + "/Packet")
            }
        }

        private void Window(int windowId)
        {
            try
            {
                GUILayout.Label(selectedPart.partInfo.title, BuildOverlay.TitleStyle);
                if (showInfo)
                {
                    for (int i = 0; i < infoItems.Count; ++i)
                    {
                        var partInfoItem = infoItems[i];
                        GUILayout.Space(2.0f);
                        GUILayout.BeginHorizontal();
                        if (partInfoItem.Value != null)
                        {
                            GUILayout.Label(partInfoItem.Name + ":", BuildOverlay.NameStyle);
                            GUILayout.Space(25.0f);
                            GUILayout.Label(partInfoItem.Value, BuildOverlay.ValueStyle);
                        }
                        else
                        {
                            GUILayout.Label(partInfoItem.Name, BuildOverlay.NameStyle);
                        }

                        GUILayout.EndHorizontal();
                    }
                }
                else if (clickToOpen && namesOnly == false)
                {
                    GUILayout.Space(2.0f);
                    GUILayout.Label(Localizer.Format("",KeyBinder.PartInfoShowHide), BuildOverlay.NameStyle);//"Click [" +  + "] to show more info..."
                }
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }
    }
}