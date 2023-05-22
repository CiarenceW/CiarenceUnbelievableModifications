using BepInEx;
using Receiver2;
using UnityEngine.Events;
using UnityEngine;
using System.Reflection;
using HarmonyLib;
using Wolfire;
using BepInEx.Configuration;
using System;

namespace CiarenceUnbelievableModifications
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MainPlugin : BaseUnityPlugin
    {
        //all this kinda sucks, like, I probably should've tried to uniform everything and stuff. But I started doing all that at 3am and can't currently be arsed to make it all better. So oops.
        private ConfigEntry<bool> configVerboseDebugEnabled;
        private ConfigEntry<bool> configKilldroneColourOverride;
        private ConfigEntry<Color> configFogColourBeginnerEast;
        private ConfigEntry<Color> configFogColourBeginnerWest;
        private ConfigEntry<Color> configFogColourSleeperEast;
        private ConfigEntry<Color> configFogColourSleeperWest;
        private ConfigEntry<Color> configFogColourSleepwalkerEast;
        private ConfigEntry<Color> configFogColourSleepwalkerWest;
        private ConfigEntry<Color> configFogColourFireEast;
        private ConfigEntry<Color> configFogColourFireWest;
        private ConfigEntry<Color> configFogColourAwakeEast;
        private ConfigEntry<Color> configFogColourAwakeWest;
        private ConfigEntry<Color> configFogColourOtherEast;
        private ConfigEntry<Color> configFogColourOtherWest;
        private ConfigEntry<Color> configTripmineBeamColour;
        private ConfigEntry<bool> configFlashlightTweaks;
        private ConfigEntry<bool> configDiscoFlashlight;
        private ConfigEntry<Color> configFlashlightColour;
        private ConfigEntry<KeyCode> configFlashlightToggleKey;
        private ConfigEntry<bool> configDropGunEverywhere;
        private ConfigEntry<bool> configEnableTurretDiscoLights;
        private ConfigEntry<Color> configTurretColourNormal;
        private ConfigEntry<Color> configTurretColourAlert;
        private ConfigEntry<Color> configTurretColourAlertShooting;
        private ConfigEntry<Color> configDroneColourIdle;
        private ConfigEntry<Color> configDroneColourAlert;
        private ConfigEntry<Color> configDroneColourAttacking;
        private ConfigEntry<Color> configCameraColourIdle;
        private ConfigEntry<Color> configCameraColourAlert;
        private ConfigEntry<Color> configCameraColourAlarming;
        private ConfigEntry<int> configDiscoTimescale;
        private ConfigEntry<bool> configTurretAmmoBoxBoom;
        private ConfigEntry<bool> configGunTweaks;
        private ConfigEntry<bool> configRobotTweaks;
        private ConfigEntry<bool> configVictorianFix;
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            //Debug verbose config
            configVerboseDebugEnabled = Config.Bind("Debug",
                "VerboseEnabled",
                false,
                "Turns on a bunch of Debug.Log() to help with debugging");

            //Flashlight tweaks config
            configFlashlightTweaks = Config.Bind("General",
                "FlashlightTweaks",
                true,
                "Enable the flashlight tweaks");

            //Fog tweaks config
            configKilldroneColourOverride = Config.Bind("Custom campaign",
                "KilldroneColourOverride",
                true,
                "Enables custom colour for the killdrones on specific campaigns");

            //Fog Colour tweaks config
            configFogColourBeginnerEast = Config.Bind("Fog",
                "FogColourBeginnerEast",
                PostProcessTweaks.east_beginner_colour,
                "The color of the fog on the east side for beginner and intro");

            configFogColourBeginnerWest = Config.Bind("Fog",
                "FogColourBeginnerWest",
                PostProcessTweaks.west_beginner_colour,
                "The color of the fog on the west side for beginner and intro");

            configFogColourSleeperEast = Config.Bind("Fog",
                "FogColourSleeperEast",
                PostProcessTweaks.east_sleeper_colour,
                "The color of the fog on the east side for sleeper");

            configFogColourSleeperWest = Config.Bind("Fog",
                "FogColourSleeperWest",
                PostProcessTweaks.west_sleeper_colour,
                "The color of the fog on the west side for sleeper");

            configFogColourSleepwalkerEast = Config.Bind("Fog",
                "FogColourSleepwalkerEast",
                PostProcessTweaks.east_sleepwalker_colour,
                "The color of the fog on the east side for sleepwalker");

            configFogColourSleepwalkerWest = Config.Bind("Fog",
                "FogColourSleepwalkerWest",
                PostProcessTweaks.west_sleepwalker_colour,
                "The color of the fog on the west side for sleepwalker");

            configFogColourFireEast = Config.Bind("Fog",
                "FogColourFireEast",
                PostProcessTweaks.east_fire_colour,
                "The color of the fog on the east side for liminal");

            configFogColourFireWest = Config.Bind("Fog",
                "FogColourFireWest",
                PostProcessTweaks.west_fire_colour,
                "The color of the fog on the west side for liminal");

            configFogColourAwakeEast = Config.Bind("Fog",
                "FogColourAwakeEast",
                PostProcessTweaks.east_awake_colour,
                "The color of the fog on the east side for awake");

            configFogColourAwakeWest = Config.Bind("Fog",
                "FogColourAwakeWest",
                PostProcessTweaks.west_awake_colour,
                "The color of the fog on the west side for awake");

            configFogColourOtherEast = Config.Bind("Fog",
                "FogColourOtherEast",
                PostProcessTweaks.east_other_colour,
                "The color of the fog on the east side for the Compound, etc...");

            configFogColourOtherWest = Config.Bind("Fog",
                "FogColourOtherWest",
                PostProcessTweaks.west_other_colour,
                "The color of the fog on the west side for the Compound, etc...");

            //Tripmine beam colour
            configTripmineBeamColour = Config.Bind("Tripmine",
                "TripmineBeamColour",
                Color.red,
                "The colour of the tripmine's light beam");

            //Flashlight disco config
            configDiscoFlashlight = Config.Bind("Fun stuff",
                "DiscoFlashlight",
                false,
                "Enable the disco mode for the flashlight");

            //Flashlight custom colour config
            configFlashlightColour = Config.Bind("Flashlight colour",
                "FlashlightColour",
                Color.white,
                "The colour of the flashlight");

            //Flashlight toggle key config
            configFlashlightToggleKey = Config.Bind("Keybinds",
                "FlashlightToggleKey",
                KeyCode.X,
                "The key that toggles the flashlight while held");

            //DropGunEverywhere config
            configDropGunEverywhere = Config.Bind("General",
                "DropGunEverywhere",
                true,
                "Enable dropping the currently held gun in any scene");

            //Explobing burret config
            configTurretAmmoBoxBoom = Config.Bind("General",
                "TurretAmmoBoxBoom",
                true,
                "Enable shrapnel when shooting the turret's ammo box (if it still has ammo)");

            //Gun tweaks config
            configGunTweaks = Config.Bind("General",
                "GunTweaks",
                true,
                "Enable the gun fixes/changes/stuff, requires restart");

            //Robroes tweaks config
            configRobotTweaks = Config.Bind("General",
                "RobotTweaks",
                true,
                "Enable the fixes for the killdrones, requires restart");

            //Disco drone light config
            configEnableTurretDiscoLights = Config.Bind("Fun stuff",
                "TurretDiscoLights",
                false,
                "Makes turret lights disco");

            //Disco drone light timescale config
            configDiscoTimescale = Config.Bind("Fun stuff",
                "TurretDiscoLightsTimescale",
                5,
                new ConfigDescription("Speed at which the colours change for the disco modes", new AcceptableValueRange<int>(0, 30)));

            //Turret colour config
            configTurretColourNormal = Config.Bind("Turret colour",
                "TurretColourNormal",
                Color.blue,
                "Colour for the turret's camera");

            configTurretColourAlert = Config.Bind("Turret colour",
                "TurretColourAlert",
                Color.yellow,
                "Colour for the turret's camera while alert");

            configTurretColourAlertShooting = Config.Bind("Turret colour",
                "TurretColourAlertShooting",
                Color.red,
                "Colour for the turret's camera while alert and shooting");

            //Shock Drone colour config
            configDroneColourIdle = Config.Bind("Shock Drone colour",
                "ShockDroneColourIdle",
                Color.blue,
                "Colour for the shock drone's camera");

            configDroneColourAlert = Config.Bind("Shock Drone colour",
                "ShockDroneColourAlert",
                Color.yellow,
                "Colour for the shock drone's camera while alert");

            configDroneColourAttacking = Config.Bind("Shock Drone colour",
                "ShockDroneColourShocking",
                Color.red,
                "Colour for the shock drone's camera while zapping");

            //Camera colour config
            configCameraColourIdle = Config.Bind("Camera colour",
                "CameraColourNormal",
                Color.blue,
                "Colour for the camera's camera");

            configCameraColourAlert = Config.Bind("Camera colour",
                "CameraColourAlert",
                Color.yellow,
                "Colour for the camera's camera while alert");

            configCameraColourAlarming = Config.Bind("Camera colour",
                "CameraColourAlarming",
                Color.red,
                "Colour for the camera's camera while screaming loudly ow");

            //Victorian fix config
            configVictorianFix = Config.Bind("General",
                "VictorianFix",
                true,
                "Enable the victorian top floor collider fix, requires restart");

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

            configTurretAmmoBoxBoom.SettingChanged += (object sender, EventArgs args) =>
            {
                if (configTurretAmmoBoxBoom.Value) TurretAmmoBoxBoom.Enable();
                else TurretAmmoBoxBoom.Disable();
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

            configTripmineBeamColour.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.tripmine_beam_colour = configTripmineBeamColour.Value;
            };

            configEnableTurretDiscoLights.SettingChanged += (object sender, EventArgs args) =>
            {
                if (!configEnableTurretDiscoLights.Value)
                {
                    RobotTweaks.colour_normal = configTurretColourNormal.Value;

                    RobotTweaks.colour_idle_drone = configDroneColourIdle.Value;

                    RobotTweaks.colour_idle_camera = configDroneColourIdle.Value;
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
                if (!configKilldroneColourOverride.Value || (configKilldroneColourOverride.Value && !RobotTweaks.campaign_has_override)) RobotTweaks.colour_idle_drone = configDroneColourIdle.Value;
            };
            configDroneColourAlert.SettingChanged += (object sender, EventArgs args) =>
            {
                if (!configKilldroneColourOverride.Value || (configKilldroneColourOverride.Value && !RobotTweaks.campaign_has_override)) RobotTweaks.colour_alert_drone = configDroneColourAlert.Value;
            };
            configDroneColourAttacking.SettingChanged += (object sender, EventArgs args) =>
            {
                if (!configKilldroneColourOverride.Value || (configKilldroneColourOverride.Value && !RobotTweaks.campaign_has_override)) RobotTweaks.colour_attacking_drone = configDroneColourAttacking.Value;
            };


            configCameraColourIdle.SettingChanged += (object sender, EventArgs args) =>
            {
                if (!configKilldroneColourOverride.Value || (configKilldroneColourOverride.Value && !RobotTweaks.campaign_has_override)) RobotTweaks.colour_idle_camera = configCameraColourIdle.Value;
            };
            configCameraColourAlert.SettingChanged += (object sender, EventArgs args) =>
            {
                if (!configKilldroneColourOverride.Value || (configKilldroneColourOverride.Value && !RobotTweaks.campaign_has_override)) RobotTweaks.colour_alert_camera = configCameraColourAlert.Value;
            };
            configCameraColourAlarming.SettingChanged += (object sender, EventArgs args) =>
            {
                if (!configKilldroneColourOverride.Value || (configKilldroneColourOverride.Value && !RobotTweaks.campaign_has_override)) RobotTweaks.colour_alarming_camera = configCameraColourAlarming.Value;
            };


            configTurretColourNormal.SettingChanged += (object sender, EventArgs args) =>
            {
                if (!configKilldroneColourOverride.Value || (configKilldroneColourOverride.Value && !RobotTweaks.campaign_has_override)) RobotTweaks.colour_normal = configTurretColourNormal.Value;
            };
            configTurretColourAlert.SettingChanged += (object sender, EventArgs args) =>
            {
                if (!configKilldroneColourOverride.Value || (configKilldroneColourOverride.Value && !RobotTweaks.campaign_has_override)) RobotTweaks.colour_alert = configTurretColourAlert.Value;
            };
            configTurretColourAlertShooting.SettingChanged += (object sender, EventArgs args) =>
            {
                if (!configKilldroneColourOverride.Value || (configKilldroneColourOverride.Value && !RobotTweaks.campaign_has_override)) RobotTweaks.colour_alert_shooting = configTurretColourAlertShooting.Value;
            };

            FlashlightTweaks.flashlight_color = configFlashlightColour.Value;
            FlashlightTweaks.UpdateFlashlightColours();

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

            if (configTurretAmmoBoxBoom.Value) TurretAmmoBoxBoom.Enable();

            if (configDropGunEverywhere.Value) DropGunEverywhere.Enable();

            if (configGunTweaks.Value) Harmony.CreateAndPatchAll(typeof(GunTweaks));
            if (configRobotTweaks.Value) Harmony.CreateAndPatchAll(typeof(RobotTweaks));
            if (configVictorianFix.Value) Harmony.CreateAndPatchAll(typeof(VictorianFix));
            Harmony.CreateAndPatchAll(typeof(RobotTweaks.TurretLightUpdateTranspiler));
            Harmony.CreateAndPatchAll(typeof(RobotTweaks.TripmineUpdateTranspiler));

            ReceiverEvents.StartListening(ReceiverEventTypeVoid.PlayerInitialized, new UnityAction<ReceiverEventTypeVoid>(OnInitialize));
            ReceiverEvents.StartListening(ReceiverEventTypeVoid.PlayerInitialized, new UnityAction<ReceiverEventTypeVoid>(PostProcessTweaks.OnPlayerInitialize));
            ReceiverEvents.StartListening(ReceiverEventTypeVoid.PlayerInitialized, new UnityAction<ReceiverEventTypeVoid>(RobotTweaks.OnPlayerInitialize));

            Receiver2ModdingKit.ModdingKitCorePlugin.AddTaskAtCoreStartup(new Receiver2ModdingKit.ModdingKitCorePlugin.StartupAction(RobotTweaks.PatchBombBotPrefab));
        }

        private void OnInitialize(ReceiverEventTypeVoid ev)
        {
            if (configDropGunEverywhere.Value) DropGunEverywhere.Enable();
        }

        private void Update()
        {
            if (configDiscoFlashlight.Value) FlashlightTweaks.Discolights();
            if (configEnableTurretDiscoLights.Value) RobotTweaks.Discolights();
            if (configFlashlightTweaks.Value) FlashlightTweaks.UpdateFlashlight(configFlashlightToggleKey.Value);
        }
    }
}