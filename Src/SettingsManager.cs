﻿using BepInEx.Configuration;
using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Reflection;
using Receiver2;
using UnityEngine.Rendering.PostProcessing;
using HarmonyLib;
using Receiver2ModdingKit;

namespace CiarenceUnbelievableModifications
{
    internal static class SettingsManager
    {
        //internal static ConfigFile config = new ConfigFile(Path.Combine(Paths.ConfigPath, typeof(SettingsManager).Assembly.GetName().Name) + ".cfg", true, (BepInPlugin)Attribute.GetCustomAttribute(typeof(MainPlugin), typeof(BepInPlugin)));
        internal static ConfigFile config;

        public static bool Verbose
        {
            get 
            {
                return configVerboseDebugEnabled.Value;
            }
        }

        const string debugCatName = "Debug";
        const string generalCatName = "General";
        const string fogCatName = "Fog";
        const string flashlightCatName = "Flashlight";
        const string killdroneCatName = "Killdrone Colour";
        const string tripmineColCatName = killdroneCatName + " | Tripmines";
        const string turretColCatName = killdroneCatName + " | Turrets";
        const string shockdroneColCatName = killdroneCatName + " | Shock Drones";
        const string cameraColCatName = killdroneCatName + " | Cameras";
        const string dropGunEverywhereCatName = "Drop gun everywhere";
        const string funStuffCatName = "Fun stuff";
        const string tapeLocatron3000CatName = "Tape Locatron 3000";
        const string itemGlintColourCatName = "Item Glint Colour";
        const string bombBotTweaksCatName = "Bomb Bot Tweaks";
		const string hudScalingCatName = "HUD Scaling";
		const string droneLightDisabler = "Drone Light Disabler";

        internal static ConfigEntry<bool> configSSREnabled;
        internal static ConfigEntry<string> configSSRQuality;

        internal static ConfigEntry<bool> configMotionBlurEnabled;
        internal static ConfigEntry<float> configMotionBlurIntensity;

        internal static ConfigEntry<bool> configVerboseDebugEnabled;

        internal static ConfigEntry<bool> configKilldroneColourOverride;

        internal static ConfigEntry<Color> configFogColourBeginnerEast;
        internal static ConfigEntry<Color> configFogColourBeginnerWest;
        internal static ConfigEntry<Color> configFogColourSleeperEast;
        internal static ConfigEntry<Color> configFogColourSleeperWest;
        internal static ConfigEntry<Color> configFogColourSleepwalkerEast;
        internal static ConfigEntry<Color> configFogColourSleepwalkerWest;
        internal static ConfigEntry<Color> configFogColourFireEast;
        internal static ConfigEntry<Color> configFogColourFireWest;
        internal static ConfigEntry<Color> configFogColourAwakeEast;
        internal static ConfigEntry<Color> configFogColourAwakeWest;
        internal static ConfigEntry<Color> configFogColourOtherEast;
        internal static ConfigEntry<Color> configFogColourOtherWest;

        internal static ConfigEntry<Color> configTripmineBeamColour;
        internal static ConfigEntry<Color> configTripmineBeamTriggeredColour;

        internal static ConfigEntry<bool> configFlashlightTweaks;
        internal static ConfigEntry<bool> configDiscoFlashlight;
        internal static ConfigEntry<float> configTimeToDropFlashlight;
        internal static ConfigEntry<Color> configFlashlightColour;
        internal static ConfigEntry<KeyCode> configFlashlightToggleKey;

        internal static ConfigEntry<bool> configDropGunEverywhere;

        internal static ConfigEntry<float> configTimeToDropGun;

        internal static ConfigEntry<KeyCode> configForceXrayKey;

        internal static ConfigEntry<bool> configEnableTurretDiscoLights;

        internal static ConfigEntry<Color> configTurretColourNormal;
        internal static ConfigEntry<Color> configTurretColourAlert;
        internal static ConfigEntry<Color> configTurretColourAlertShooting;
        internal static ConfigEntry<Color> configDroneColourIdle;
        internal static ConfigEntry<Color> configDroneColourAlert;
        internal static ConfigEntry<Color> configDroneColourAttacking;
        internal static ConfigEntry<Color> configCameraColourIdle;
        internal static ConfigEntry<Color> configCameraColourAlert;
        internal static ConfigEntry<Color> configCameraColourAlarming;

        internal static ConfigEntry<int> configDiscoTimescale;

        internal static ConfigEntry<bool> configTurretAmmoBoxBoom;

        internal static ConfigEntry<bool> configGunTweaks;

        internal static ConfigEntry<bool> configRobotTweaks;

        internal static ConfigEntry<bool> configVictorianFix;

        internal static ConfigEntry<bool> configSpawnCompatibleMags;

        internal static ConfigEntry<bool> configLimitFPSFocusLostEnabled;
        internal static ConfigEntry<float> configLimitFPSFocusLostCount;

        internal static ConfigEntry<bool> configTapeLocatron3000Enabled;
        internal static ConfigEntry<Color> configTapeLocatron3000ColourOccluded;
        internal static ConfigEntry<Color> configTapeLocatron3000ColourLos;

        internal static ConfigEntry<bool> configReplaceBombBotModel;
        internal static ConfigEntry<bool> configBombBotModelSpeechReplacer;
        internal static ConfigEntry<string> initiatingSelfDestructAudioPath;
        internal static ConfigEntry<string> threateningPostureAudioPath;
        internal static ConfigEntry<string> threatOutOfSightAudioPath;
        internal static ConfigEntry<string> threatAlleviatedAudioPath;

        internal static ConfigEntry<bool> configInventoryGlintColourEnabled;

		internal static ConfigEntry<bool> configEnableHUDScaling;
		internal static ConfigEntry<UnityEngine.UI.CanvasScaler.ScaleMode> configHUDScaleMode;
		internal static ConfigEntry<float> configHUDScaleFactor;

		internal static ConfigEntry<bool> configEnableDroneLightDisabler;
		internal static ConfigEntry<bool> configOnlyDisablePointLights; //point lights are the most expensive
		internal static ConfigEntry<float> configLightFadeDistance;
		internal static ConfigEntry<float> configLightCutoffDistance;

        internal static void InitializeAndBindSettings()
        {
            config = MainPlugin.config;

            configVerboseDebugEnabled = config.Bind(debugCatName,
                "VerboseEnabled",
                false,
                "Turns on a bunch of Debug.Log() to help with debugging");

            //Flashlight tweaks config
            configFlashlightTweaks = config.Bind(generalCatName,
                "FlashlightTweaks",
                true,
                "Enable the flashlight tweaks");

            //Custom campaign override colour config
            configKilldroneColourOverride = config.Bind(killdroneCatName,
                "KilldroneColourOverride",
                true,
                "Enables custom colour for the killdrones on specific campaigns");

            configKilldroneColourOverride.SettingChanged += (object sender, EventArgs args) =>
            {
                if (ReceiverCoreScript.Instance() != null && ReceiverCoreScript.Instance().enemy_prefabs != null) RobotTweaks.SetOverrideColours(configKilldroneColourOverride.Value);
            };

            //Fog Colour tweaks config
            configFogColourBeginnerEast = config.Bind(fogCatName,
                "FogColourBeginnerEast",
                PostProcessTweaks.east_beginner_colour,
                "The color of the fog on the east side for beginner and intro");

            configFogColourBeginnerWest = config.Bind(fogCatName,
                "FogColourBeginnerWest",
                PostProcessTweaks.west_beginner_colour,
                "The color of the fog on the west side for beginner and intro");

            configFogColourSleeperEast = config.Bind(fogCatName,
                "FogColourSleeperEast",
                PostProcessTweaks.east_sleeper_colour,
                "The color of the fog on the east side for sleeper");

            configFogColourSleeperWest = config.Bind(fogCatName,
                "FogColourSleeperWest",
                PostProcessTweaks.west_sleeper_colour,
                "The color of the fog on the west side for sleeper");

            configFogColourSleepwalkerEast = config.Bind(fogCatName,
                "FogColourSleepwalkerEast",
                PostProcessTweaks.east_sleepwalker_colour,
                "The color of the fog on the east side for sleepwalker");

            configFogColourSleepwalkerWest = config.Bind(fogCatName,
                "FogColourSleepwalkerWest",
                PostProcessTweaks.west_sleepwalker_colour,
                "The color of the fog on the west side for sleepwalker");

            configFogColourFireEast = config.Bind(fogCatName,
                "FogColourFireEast",
                PostProcessTweaks.east_fire_colour,
                "The color of the fog on the east side for liminal");

            configFogColourFireWest = config.Bind(fogCatName,
                "FogColourFireWest",
                PostProcessTweaks.west_fire_colour,
                "The color of the fog on the west side for liminal");

            configFogColourAwakeEast = config.Bind(fogCatName,
                "FogColourAwakeEast",
                PostProcessTweaks.east_awake_colour,
                "The color of the fog on the east side for awake");

            configFogColourAwakeWest = config.Bind(fogCatName,
                "FogColourAwakeWest",
                PostProcessTweaks.west_awake_colour,
                "The color of the fog on the west side for awake");

            configFogColourOtherEast = config.Bind(fogCatName,
                "FogColourOtherEast",
                PostProcessTweaks.east_other_colour,
                "The color of the fog on the east side for the Compound, etc...");

            configFogColourOtherWest = config.Bind(fogCatName,
                "FogColourOtherWest",
                PostProcessTweaks.west_other_colour,
                "The color of the fog on the west side for the Compound, etc...");

            //Flashlight disco config
            configDiscoFlashlight = config.Bind(funStuffCatName,
                "DiscoFlashlight",
                false,
                "Enable the disco mode for the flashlight");

            //Flashlight custom colour config
            configFlashlightColour = config.Bind(flashlightCatName,
                "FlashlightColour",
                Color.white,
                "The colour of the flashlight");

            //Flashlight toggle key config
            configFlashlightToggleKey = config.Bind(flashlightCatName,
                "FlashlightToggleKey",
                KeyCode.X,
                "The key that toggles the flashlight while held");

            configTimeToDropFlashlight = config.Bind(flashlightCatName,
                "TimeToDropFlashlight",
                0.5f,
                new ConfigDescription("the amount of time you have to hold \"E\" to drop the flashlight if it's currently in your hands", new AcceptableValueRange<float>(0f, 1f)));

            //DropGunEverywhere config
            configDropGunEverywhere = config.Bind(generalCatName,
                "DropGunEverywhere",
                true,
                "Enable dropping the currently held gun in any scene");

            //Time to drop gun config
            configTimeToDropGun = config.Bind(dropGunEverywhereCatName,
                "TimeToDropGun",
                0.5f,
                new ConfigDescription("The amount of time you have to hold \"E\" to drop the gun that is currently in your hands", new AcceptableValueRange<float>(0f, 1f)));

            //Explobing burret config
            configTurretAmmoBoxBoom = config.Bind(generalCatName,
                "TurretAmmoBoxBoom",
                false,
                "Enable shrapnel when shooting the turret's ammo box (if it still has ammo)");

            configTurretAmmoBoxBoom.SettingChanged += (object sender, EventArgs args) =>
            {
                if (configTurretAmmoBoxBoom.Value) TurretAmmoBoxBoom.Enable();
                else TurretAmmoBoxBoom.Disable();
            };

            //Gun tweaks config
            configGunTweaks = config.Bind(generalCatName,
                "GunTweaks",
                true,
                "Enable the gun fixes/changes/stuff, requires restart");

            #region RobotTweaks
            //Robroes tweaks config
            configRobotTweaks = config.Bind(generalCatName,
                "RobotTweaks",
                true,
                "Enable the fixes for the killdrones, requires restart");

            //Disco drone light config
            configEnableTurretDiscoLights = config.Bind(funStuffCatName,
                "TurretDiscoLights",
                false,
                "Makes turret lights disco");

            //Disco drone light timescale config
            configDiscoTimescale = config.Bind(funStuffCatName,
                "TurretDiscoLightsTimescale",
                5,
                new ConfigDescription("Speed at which the colours change for the disco modes", new AcceptableValueRange<int>(0, 30)));
			#endregion

			#region BombBotTweaks

			configReplaceBombBotModel = config.Bind(bombBotTweaksCatName,
                "ReplaceBombBotModel",
                true,
                "Replaces the placeholder Bomb Bot model with the cool final one");

            configBombBotModelSpeechReplacer = config.Bind(bombBotTweaksCatName,
                "BombBotModelReplaceSpeech",
                false,
                "Allows you to replace the voice clips that the bomb bot plays by setting your own paths");

            initiatingSelfDestructAudioPath = config.Bind(bombBotTweaksCatName,
                "InitiatingSelfDestructAudioPath",
                Application.streamingAssetsPath + "/Sounds/TTSVoiceClips/" + "InitiatingSelfDestruct.wav",
                "The path of the InitiatingSelfDestruct voice clip");

            threateningPostureAudioPath = config.Bind(bombBotTweaksCatName,
                "ThreateningPostureAudioPath",
                Application.streamingAssetsPath + "/Sounds/TTSVoiceClips/" + "ThreateningPosture.wav",
                "The path of the ThreateningPosture voice clip");

            threatOutOfSightAudioPath = config.Bind(bombBotTweaksCatName,
                "ThreatOutOfSightAudioPath",
                Application.streamingAssetsPath + "/Sounds/TTSVoiceClips/" + "ThreatOutOfSight.wav",
                "The path of the ThreatOutOfSight voice clip");

            threatAlleviatedAudioPath = config.Bind(bombBotTweaksCatName,
                "ThreatAlleviatedAudioPath",
                Application.streamingAssetsPath + "/Sounds/TTSVoiceClips/" + "ThreatAlleviated.wav",
                "The path of the ThreatAlleviated voice clip");

			#endregion

			#region HUDScalingTweaks
			configEnableHUDScaling = config.Bind(hudScalingCatName,
				"EnableHUDScaling",
				true,
				"Enable HUD Scaling based on factor or screen resolution");

			configHUDScaleMode = config.Bind(hudScalingCatName,
				"HUDScaleMode",
				UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize,
				"Selects which mode to scale the HUD with, ScaleWithScreenSize or ConstantPixelSize recommended");

			configHUDScaleFactor = config.Bind(hudScalingCatName,
				"HUDScaleFactor",
				1.3f,
				"The factor by which the screen will be scaled in ConstantPixelSize mode");

			if (configEnableHUDScaling.Value)
			{
				ModdingKitEvents.AddTaskAtCoreStartup(new ModdingKitEvents.StartupAction(HUDScalingTweak.SetHUDScaleFactor));
				ModdingKitEvents.AddTaskAtCoreStartup(new ModdingKitEvents.StartupAction(HUDScalingTweak.ChangeHUDScaleMode));
			}
			#endregion

			#region TurretColourConfig
			//Turret colour config
			configTurretColourNormal = config.Bind(turretColCatName,
                "TurretColourNormal",
                Color.blue,
                "Colour for the turret's camera");

            configTurretColourAlert = config.Bind(turretColCatName,
                "TurretColourAlert",
                Color.yellow,
                "Colour for the turret's camera while alert");

            configTurretColourAlertShooting = config.Bind(turretColCatName,
                "TurretColourAlertShooting",
                Color.red,
                "Colour for the turret's camera while alert and shooting");
            #endregion

            #region ShockDroneColourConfig
            //Shock Drone colour config
            configDroneColourIdle = config.Bind(shockdroneColCatName,
                "ShockDroneColourIdle",
                Color.blue,
                "Colour for the shock drone's camera");

            configDroneColourAlert = config.Bind(shockdroneColCatName,
                "ShockDroneColourAlert",
                Color.yellow,
                "Colour for the shock drone's camera while alert");

            configDroneColourAttacking = config.Bind(shockdroneColCatName,
                "ShockDroneColourShocking",
                Color.red,
                "Colour for the shock drone's camera while zapping");
            #endregion

            #region SecurityCameraColourConfig
            //Camera colour config
            configCameraColourIdle = config.Bind(cameraColCatName,
                "CameraColourNormal",
                Color.blue,
                "Colour for the camera's camera");

            configCameraColourAlert = config.Bind(cameraColCatName,
                "CameraColourAlert",
                Color.yellow,
                "Colour for the camera's camera while alert");

            configCameraColourAlarming = config.Bind(cameraColCatName,
                "CameraColourAlarming",
                Color.red,
                "Colour for the camera's camera while screaming loudly ow");
            #endregion

            #region TripmineColour
            //Tripmine beam colour
            configTripmineBeamColour = config.Bind(tripmineColCatName,
                "TripmineBeamColour",
                Color.red,
                "The colour of the tripmine's light beam");

            configTripmineBeamTriggeredColour = config.Bind(tripmineColCatName,
                "TripmineBeamTriggeredColour",
                Color.cyan,
                "The colour of the tripmine's light beam when triggered");
            #endregion

            //Victorian fix config
            configVictorianFix = config.Bind(generalCatName,
                "TileFix",
                true,
                "Enable the victorian top floor collider fix, and the Two Towers walkramp fix, requires restart");

            #region MotionBlur
            configMotionBlurEnabled = config.Bind("Graphics",
                "Enable Motion Blur",
                false,
                "Should Motion Blur be enabled");

            configMotionBlurIntensity = config.Bind("Graphics",
                "Motion Blur Intensity",
                100f,
                new ConfigDescription("Intensity of the Motion Blur (sometimes known as shutter angle)", new AcceptableValueRange<float>(0f, 1000f)));
            #endregion

            #region SSR
            configSSREnabled = config.Bind("Graphics",
                "Enable SSR",
                false,
                "Should Screen Space Reflections be enabled");

            configSSRQuality = config.Bind("Graphics",
                "SSR Quality",
                "Medium",
                new ConfigDescription("Quality of the Screen Space Reflections", new AcceptableValueList<string>("Lower", "Low", "Medium", "High", "Higher", "Ultra", "Overkill", "Receiver 2 Bespoke")));
            #endregion

            #region SpawnAllCompatibleMags
            configSpawnCompatibleMags = config.Bind(generalCatName,
                "Spawn Compatible Mags",
                true,
                "Spawns every compatible mag for current gun in the Dreaming");

            configSpawnCompatibleMags.SettingChanged += (s, e) =>
            {
                if (configSpawnCompatibleMags.Value == true)
                    Harmony.CreateAndPatchAll(typeof(rtlgTweaks.SpawnCompatibleMagsTweak), MainPlugin.SpawnCompatibleMagsHID);
                else
                    Harmony.UnpatchID(MainPlugin.SpawnCompatibleMagsHID);
            };
            #endregion

            #region FocusLostFPSLimit
            configLimitFPSFocusLostEnabled = config.Bind(generalCatName,
                "Limit FPS on Focus Lost",
                true,
                "Enables the frame limiter when application is alt-tabbed");

            configLimitFPSFocusLostCount = config.Bind(generalCatName,
                "Focus lost FPS limit",
                30f,
                new ConfigDescription("What the FPS limit should be set at", new AcceptableValueRange<float>(15, 480)));
            #endregion

            #region TapeLocatron
            configTapeLocatron3000Enabled = config.Bind(tapeLocatron3000CatName,
                "Enable Tape Locatron 3000",
                false,
                "Enables the Tape Locatron 3000 ");

            configTapeLocatron3000ColourOccluded = config.Bind(tapeLocatron3000CatName,
                "Tape Locatron 3000 Occluded Colour",
                Color.green,
                "Colour for tapes that aren't in your direct line of sight");

            configTapeLocatron3000ColourLos = config.Bind(tapeLocatron3000CatName,
                "Tape Locatron 3000 Los Colour",
                Color.magenta,
                "Colour for tapes that are in your direct line of sight");
            #endregion

            #region Glint_colour
            //Item Glint Colour config
            configInventoryGlintColourEnabled = config.Bind(itemGlintColourCatName,
                "Enable Random Glint Colour",
                false,
                "Enables the random glint colour for Inventory Items (rounds, mags, tapes)");

            configInventoryGlintColourEnabled.SettingChanged += (s, e) =>
            {
                var inventoryItems = UnityEngine.Object.FindObjectsOfType<InventoryItem>();
                if (configInventoryGlintColourEnabled.Value == true)
                {
                    Harmony.CreateAndPatchAll(typeof(InventoryGlintColourRandomizer), MainPlugin.InventoryGlintColourRandomizerHID);
                    for (int i = 0; i < inventoryItems.Length; i++)
                    {
                        if (inventoryItems[i].glint_renderer != null && inventoryItems[i].glint_renderer.material != null)
                        {
                            inventoryItems[i].glint_renderer.material.SetColor("_GlintColor", UnityEngine.Random.ColorHSV(0, 1, 0, 1, 0.7f, 1));
                        }
                    }
                }
                else
                {
                    Harmony.UnpatchID(MainPlugin.InventoryGlintColourRandomizerHID);

                    for (int i = 0; i < inventoryItems.Length; i++)
                    {
                        if (inventoryItems[i].glint_renderer != null && inventoryItems[i].glint_renderer.material != null)
                        {
                            inventoryItems[i].glint_renderer.material.SetColor("_GlintColor", InventoryGlintColourRandomizer.BaseGlintColour);
                        }
                    }
                }
            };
			#endregion

			configEnableDroneLightDisabler = config.Bind(droneLightDisabler,
				"Enable Drone Light Disabler",
				false,
				"Disables killdrones' lights when they're too far away, there's barely any visual difference, since they still have their light beams, but you'll notice your fps increase :)");

			configOnlyDisablePointLights = config.Bind(droneLightDisabler,
				"Only Disable Point Lights",
				false,
				"Only disable drone's point lights, point lights are the most expensive type of lights to render");

			configLightFadeDistance = config.Bind(droneLightDisabler,
				"Light Fade Distance",
				15f,
				"The distance at which the lights will begin to fade out");

			configLightCutoffDistance = config.Bind(droneLightDisabler,
				"Light Cutoff Distance",
				20f,
				"The distance at which the lights will be disabled");

			configMotionBlurEnabled.SettingChanged += (object sender, EventArgs args) =>
            {
                PostProcessTweaks.ToggleMotionBlur();
            };

            configSSREnabled.SettingChanged += (object sender, EventArgs args) =>
            {
                PostProcessTweaks.ToggleSSR();
            };

            configVerboseDebugEnabled.SettingChanged += (object sender, EventArgs args) =>
            {
                FlashlightTweaks.verbose = configVerboseDebugEnabled.Value;
                TurretAmmoBoxBoom.verbose = configVerboseDebugEnabled.Value;
                RobotTweaks.verbose = configVerboseDebugEnabled.Value;
                PostProcessTweaks.verbose = configVerboseDebugEnabled.Value;
            };

            configDropGunEverywhere.SettingChanged += (object sender, EventArgs args) =>
            {
                if (configDropGunEverywhere.Value) DropGunEverywhere.Enable();
                else DropGunEverywhere.Disable();
            };

            configFogColourBeginnerEast.SettingChanged += (object sender, EventArgs args) =>
            {
                PostProcessTweaks.east_beginner_colour = configFogColourBeginnerEast.Value;
                PostProcessTweaks.UpdateFogColour();
            };

            configFogColourBeginnerWest.SettingChanged += (object sender, EventArgs args) =>
            {
                PostProcessTweaks.west_beginner_colour = configFogColourBeginnerWest.Value;
                PostProcessTweaks.UpdateFogColour();
            };

            configFogColourSleeperEast.SettingChanged += (object sender, EventArgs args) =>
            {
                PostProcessTweaks.east_sleeper_colour = configFogColourSleeperEast.Value;
                PostProcessTweaks.UpdateFogColour();
            };

            configFogColourSleeperWest.SettingChanged += (object sender, EventArgs args) =>
            {
                PostProcessTweaks.west_sleeper_colour = configFogColourSleeperWest.Value;
                PostProcessTweaks.UpdateFogColour();
            };

            configFogColourSleepwalkerEast.SettingChanged += (object sender, EventArgs args) =>
            {
                PostProcessTweaks.east_sleepwalker_colour = configFogColourSleepwalkerEast.Value;
                PostProcessTweaks.UpdateFogColour();
            };

            configFogColourSleepwalkerWest.SettingChanged += (object sender, EventArgs args) =>
            {
                PostProcessTweaks.west_sleepwalker_colour = configFogColourSleepwalkerWest.Value;
                PostProcessTweaks.UpdateFogColour();
            };

            configFogColourFireEast.SettingChanged += (object sender, EventArgs args) =>
            {
                PostProcessTweaks.east_fire_colour = configFogColourFireEast.Value;
                PostProcessTweaks.UpdateFogColour();
            };

            configFogColourFireWest.SettingChanged += (object sender, EventArgs args) =>
            {
                PostProcessTweaks.west_fire_colour = configFogColourFireWest.Value;
                PostProcessTweaks.UpdateFogColour();
            };

            configFogColourAwakeEast.SettingChanged += (object sender, EventArgs args) =>
            {
                PostProcessTweaks.east_awake_colour = configFogColourAwakeEast.Value;
                PostProcessTweaks.UpdateFogColour();
            };

            configFogColourAwakeWest.SettingChanged += (object sender, EventArgs args) =>
            {
                PostProcessTweaks.west_awake_colour = configFogColourAwakeWest.Value;
                PostProcessTweaks.UpdateFogColour();
            };

            configFogColourOtherEast.SettingChanged += (object sender, EventArgs args) =>
            {
                PostProcessTweaks.east_other_colour = configFogColourOtherEast.Value;
                PostProcessTweaks.UpdateFogColour();
            };

            configFogColourOtherWest.SettingChanged += (object sender, EventArgs args) =>
            {
                PostProcessTweaks.west_other_colour = configFogColourOtherWest.Value;
                PostProcessTweaks.UpdateFogColour();
            };

            configTimeToDropGun.SettingChanged += (object sender, EventArgs args) =>
            {
                DropGunEverywhere.time_to_drop = configTimeToDropGun.Value;
            };

            configTimeToDropFlashlight.SettingChanged += (object sender, EventArgs args) =>
            {
                FlashlightTweaks.time_to_drop = configTimeToDropFlashlight.Value;
            };

            configTripmineBeamColour.SettingChanged += (object sender, EventArgs args) =>
            {
                if (CanChangeKillDroneLights()) RobotTweaks.tripmine_beam_colour = configTripmineBeamColour.Value;
            };

            configTripmineBeamTriggeredColour.SettingChanged += (object sender, EventArgs args) =>
            {
                if (CanChangeKillDroneLights()) RobotTweaks.tripmine_beam_colour_triggered = configTripmineBeamTriggeredColour.Value;
            };

            configEnableTurretDiscoLights.SettingChanged += (object sender, EventArgs args) =>
            {
                if (!configEnableTurretDiscoLights.Value)
                {
                    RobotTweaks.colour_normal = configTurretColourNormal.Value;

                    RobotTweaks.colour_idle_drone = configDroneColourIdle.Value;

                    RobotTweaks.colour_idle_camera = configCameraColourIdle.Value;

                    if (ReceiverCoreScript.Instance() != null && ReceiverCoreScript.Instance().enemy_prefabs != null) RobotTweaks.UpdateColourPartLight();
                }
            };

            configDiscoTimescale.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.disco_timescale = configDiscoTimescale.Value;
                FlashlightTweaks.disco_timescale = configDiscoTimescale.Value;
            };

            configFlashlightColour.SettingChanged += (object sender, EventArgs args) =>
            {
                FlashlightTweaks.flashlight_color = configFlashlightColour.Value;
                FlashlightTweaks.UpdateFlashlightColours();
            };

            configDiscoFlashlight.SettingChanged += (object sender, EventArgs args) =>
            {
                if (!configDiscoFlashlight.Value) FlashlightTweaks.flashlight_color = configFlashlightColour.Value;
                FlashlightTweaks.UpdateFlashlightColours();
            };

            configDroneColourIdle.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.OnChangeKilldroneLightColour();
                if (ReceiverCoreScript.TryGetInstance(out var _)) RobotTweaks.UpdateLightPartDefaultColour(configDroneColourIdle.Value, ReceiverEntityType.Drone);
                if (CanChangeKillDroneLights()) RobotTweaks.colour_idle_drone = configDroneColourIdle.Value;
            };
            configDroneColourAlert.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.OnChangeKilldroneLightColour();
                if (CanChangeKillDroneLights()) RobotTweaks.colour_alert_drone = configDroneColourAlert.Value;
            };
            configDroneColourAttacking.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.OnChangeKilldroneLightColour();
                if (CanChangeKillDroneLights()) RobotTweaks.colour_attacking_drone = configDroneColourAttacking.Value;
            };


            configCameraColourIdle.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.OnChangeKilldroneLightColour();
                if (ReceiverCoreScript.TryGetInstance(out var _)) RobotTweaks.UpdateLightPartDefaultColour(configCameraColourIdle.Value, ReceiverEntityType.SecurityCamera);
                if (CanChangeKillDroneLights()) RobotTweaks.colour_idle_camera = configCameraColourIdle.Value;
            };
            configCameraColourAlert.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.OnChangeKilldroneLightColour();
                if (CanChangeKillDroneLights()) RobotTweaks.colour_alert_camera = configCameraColourAlert.Value;
            };
            configCameraColourAlarming.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.OnChangeKilldroneLightColour();
                if (CanChangeKillDroneLights()) RobotTweaks.colour_alarming_camera = configCameraColourAlarming.Value;
            };


            configTurretColourNormal.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.OnChangeKilldroneLightColour();
                if (CanChangeKillDroneLights()) RobotTweaks.colour_normal = configTurretColourNormal.Value;
            };
            configTurretColourAlert.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.OnChangeKilldroneLightColour();
                if (CanChangeKillDroneLights()) RobotTweaks.colour_alert = configTurretColourAlert.Value;
            };
            configTurretColourAlertShooting.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.OnChangeKilldroneLightColour();
                if (CanChangeKillDroneLights()) RobotTweaks.colour_alert_shooting = configTurretColourAlertShooting.Value;
            };

			configEnableHUDScaling.SettingChanged += (object sender, EventArgs args) =>
			{
				if (!configEnableHUDScaling.Value)
				{
					PlayerGUI.Instance.canvas.GetComponent<UnityEngine.UI.CanvasScaler>().uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ConstantPixelSize;
					PlayerGUI.Instance.canvas.GetComponent<UnityEngine.UI.CanvasScaler>().scaleFactor = 1f;
				}
				else
				{
					HUDScalingTweak.SetHUDScaleFactor();
					HUDScalingTweak.ChangeHUDScaleMode();
				}
			};

            //Fog custom colours
            PostProcessTweaks.east_other_colour = configFogColourOtherEast.Value;
            PostProcessTweaks.west_other_colour = configFogColourOtherWest.Value;
            PostProcessTweaks.east_beginner_colour = configFogColourBeginnerEast.Value;
            PostProcessTweaks.west_beginner_colour = configFogColourBeginnerWest.Value;
            PostProcessTweaks.east_sleeper_colour = configFogColourSleeperEast.Value;
            PostProcessTweaks.west_sleeper_colour = configFogColourSleeperWest.Value;
            PostProcessTweaks.east_sleepwalker_colour = configFogColourSleepwalkerEast.Value;
            PostProcessTweaks.west_sleepwalker_colour = configFogColourSleepwalkerWest.Value;
            PostProcessTweaks.east_fire_colour = configFogColourFireEast.Value;
            PostProcessTweaks.west_fire_colour = configFogColourFireWest.Value;
            PostProcessTweaks.east_awake_colour = configFogColourAwakeEast.Value;
            PostProcessTweaks.west_awake_colour = configFogColourAwakeWest.Value;

            DropGunEverywhere.time_to_drop = configTimeToDropGun.Value;

            FlashlightTweaks.time_to_drop = configTimeToDropFlashlight.Value;

            FlashlightTweaks.flashlight_color = configFlashlightColour.Value;
            FlashlightTweaks.UpdateFlashlightColours();

            RobotTweaks.tripmine_beam_colour = configTripmineBeamColour.Value;
            RobotTweaks.tripmine_beam_colour_triggered = configTripmineBeamTriggeredColour.Value;

            RobotTweaks.tripmine_beam_colour_normal = configTripmineBeamColour.Value;
            RobotTweaks.tripmine_beam_colour_triggered_normal = configTripmineBeamTriggeredColour.Value;

            RobotTweaks.colour_idle_drone = configDroneColourIdle.Value;
            RobotTweaks.colour_alert_drone = configDroneColourAlert.Value;
            RobotTweaks.colour_attacking_drone = configDroneColourAttacking.Value;

            RobotTweaks.colour_idle_camera = configCameraColourIdle.Value;
            RobotTweaks.colour_alert_camera = configCameraColourAlert.Value;
            RobotTweaks.colour_alarming_camera = configCameraColourAlarming.Value;

            RobotTweaks.colour_normal = configTurretColourNormal.Value;
            RobotTweaks.colour_alert = configTurretColourAlert.Value;
            RobotTweaks.colour_alert_shooting = configTurretColourAlertShooting.Value;

            RobotTweaks.colour_idle_turret = configTurretColourNormal.Value;
            RobotTweaks.colour_alert_turret = configTurretColourAlert.Value;
            RobotTweaks.colour_attacking_turret = configTurretColourAlertShooting.Value;

            RobotTweaks.disco_timescale = configDiscoTimescale.Value;
            FlashlightTweaks.disco_timescale = configDiscoTimescale.Value;
        }

        private static bool CanChangeKillDroneLights()
        {
            return !configKilldroneColourOverride.Value || (configKilldroneColourOverride.Value && !RobotTweaks.campaign_has_override);
        }
    }
}
