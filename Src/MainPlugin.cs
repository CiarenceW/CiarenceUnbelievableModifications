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
                RobotTweaks.colour_idle_drone = configDroneColourIdle.Value;
            };
            configDroneColourAlert.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.colour_alert_drone = configDroneColourAlert.Value;
            };
            configDroneColourAttacking.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.colour_attacking_drone = configDroneColourAttacking.Value;
            };


            configCameraColourIdle.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.colour_idle_camera = configCameraColourIdle.Value;
            };
            configCameraColourAlert.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.colour_alert_camera = configCameraColourAlert.Value;
            };
            configCameraColourAlarming.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.colour_alarming_camera = configCameraColourAlarming.Value;
            };


            configTurretColourNormal.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.colour_normal = configTurretColourNormal.Value;
            };
            configTurretColourAlert.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.colour_alert = configTurretColourAlert.Value;
            };
            configTurretColourAlertShooting.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.colour_alert_shooting = configTurretColourAlertShooting.Value;
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
            RobotTweaks.disco_timescale = configDiscoTimescale.Value;
            FlashlightTweaks.disco_timescale = configDiscoTimescale.Value;

            if (configTurretAmmoBoxBoom.Value) TurretAmmoBoxBoom.Enable();
            else TurretAmmoBoxBoom.Disable();

            if (configDropGunEverywhere.Value) DropGunEverywhere.Enable();
            else DropGunEverywhere.Disable();

            if (configGunTweaks.Value) Harmony.CreateAndPatchAll(typeof(GunTweaks));
            if (configRobotTweaks.Value) Harmony.CreateAndPatchAll(typeof(RobotTweaks));
            if (configVictorianFix.Value) Harmony.CreateAndPatchAll(typeof(VictorianFix));
            Harmony.CreateAndPatchAll(typeof(RobotTweaks.TurretLightUpdateTranspiler));
            Harmony.CreateAndPatchAll(typeof(RobotTweaks.DroneSetLightModeTranspiler));

            ReceiverEvents.StartListening(ReceiverEventTypeVoid.PlayerInitialized, new UnityAction<ReceiverEventTypeVoid>(OnInitialize));
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
