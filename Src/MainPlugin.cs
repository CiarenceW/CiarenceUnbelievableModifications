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
        private ConfigEntry<KeyCode> configFlashlightToggleKey;
        private ConfigEntry<bool> configDropGunEverywhere;
        private ConfigEntry<bool> configEnableTurretDiscoLights;
        private ConfigEntry<Color> configTurretColourNormal;
        private ConfigEntry<Color> configTurretColourAlert;
        private ConfigEntry<Color> configTurretColourAlertShooting;
        private ConfigEntry<int> configDiscoTimescale;
        private ConfigEntry<bool> configTurretAmmoBoxBoom;
        private ConfigEntry<bool> configGunTweaks;
        private ConfigEntry<bool> configRobotTweaks;
        private ConfigEntry<bool> configVictorianFix;
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");


            configVerboseDebugEnabled = Config.Bind("Debug",
                "VerboseEnabled",
                false,
                "Turns on a bunch of Debug.Log() to help with debugging");

            configFlashlightTweaks = Config.Bind("General",
                "FlashlightTweaks",
                true,
                "Enable the flashlight tweaks");

            configFlashlightToggleKey = Config.Bind("Keybinds",
                "FlashlightToggleKey",
                KeyCode.X,
                "The key that toggles the flashlight while held");

            configDropGunEverywhere = Config.Bind("General",
                "DropGunEverywhere",
                true,
                "Enable dropping the currently held gun in any scene");

            configTurretAmmoBoxBoom = Config.Bind("General",
                "TurretAmmoBoxBoom",
                true,
                "Enable shrapnel when shooting the turret's ammo box (if it still has ammo)");

            configGunTweaks = Config.Bind("General",
                "GunTweaks",
                true,
                "Enable the gun fixes/changes/stuff, requires restart");

            configRobotTweaks = Config.Bind("General",
                "RobotTweaks",
                true,
                "Enable the fixes for the killdrones, requires restart");

            configEnableTurretDiscoLights = Config.Bind("Fun stuff",
                "TurretDiscoLights",
                false,
                "Makes turret lights disco");

            configDiscoTimescale = Config.Bind("Fun stuff",
                "TurretDiscoLightsTimescale",
                5,
                new ConfigDescription("Colour for the turret's camera while alert and shooting", new AcceptableValueRange<int>(0, 30)));

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

            configVictorianFix = Config.Bind("General",
                "VictorianFix",
                true,
                "Enable the victorian top floor collider fix, requires restart");

            configVerboseDebugEnabled.SettingChanged += (object sender, EventArgs args) =>
            {
                FlashlightTweaks.verbose = configVerboseDebugEnabled.Value;
                TurretAmmoBoxBoom.verbose = configVerboseDebugEnabled.Value;
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
                    RobotTweaks.colour_alert = configTurretColourAlert.Value;
                    RobotTweaks.colour_alert_shooting = configTurretColourAlertShooting.Value;
                }
            };

            configDiscoTimescale.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.disco_timescale = configDiscoTimescale.Value;
            };

            configTurretColourNormal.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.colour_normal = configTurretColourNormal.Value;
                Debug.Log(RobotTweaks.colour_normal);
            };

            configTurretColourAlert.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.colour_alert = configTurretColourAlert.Value;
                Debug.Log(RobotTweaks.colour_alert);
            };

            configTurretColourAlertShooting.SettingChanged += (object sender, EventArgs args) =>
            {
                RobotTweaks.colour_alert_shooting = configTurretColourAlertShooting.Value;
                Debug.Log(RobotTweaks.colour_alert_shooting);
            };

            RobotTweaks.colour_normal = configTurretColourNormal.Value;
            RobotTweaks.colour_alert = configTurretColourAlert.Value;
            RobotTweaks.colour_alert_shooting = configTurretColourAlertShooting.Value;
            RobotTweaks.disco_timescale = configDiscoTimescale.Value;

            if (configTurretAmmoBoxBoom.Value) TurretAmmoBoxBoom.Enable();
            else TurretAmmoBoxBoom.Disable();

            if (configDropGunEverywhere.Value) DropGunEverywhere.Enable();
            else DropGunEverywhere.Disable();

            if (configGunTweaks.Value) Harmony.CreateAndPatchAll(typeof(GunTweaks));
            if (configRobotTweaks.Value) Harmony.CreateAndPatchAll(typeof(RobotTweaks));
            if (configVictorianFix.Value) Harmony.CreateAndPatchAll(typeof(VictorianFix));

            ReceiverEvents.StartListening(ReceiverEventTypeVoid.PlayerInitialized, new UnityAction<ReceiverEventTypeVoid>(OnInitialize));
        }

        private void OnInitialize(ReceiverEventTypeVoid ev)
        {
            if (configDropGunEverywhere.Value) DropGunEverywhere.Enable();
        }

        private void Update()
        {
            if (configEnableTurretDiscoLights.Value) RobotTweaks.Discolights();
            if (configFlashlightTweaks.Value) FlashlightTweaks.UpdateFlashlight(configFlashlightToggleKey.Value);
        }
    }
}
