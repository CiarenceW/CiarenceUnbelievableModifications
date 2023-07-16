using BepInEx;
using Receiver2;
using UnityEngine.Events;
using UnityEngine;
using HarmonyLib;
using BepInEx.Configuration;
using System;

namespace CiarenceUnbelievableModifications
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MainPlugin : BaseUnityPlugin
    {
        //all this kinda sucks, like, I probably should've tried to uniform everything and stuff. But I started doing all that at 3am and can't currently be arsed to make it all better. So oops.
        internal static ConfigFile config;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            config = Config;

            SettingsManager.InitializeAndBindSettings();

            if (SettingsManager.configTurretAmmoBoxBoom.Value) TurretAmmoBoxBoom.Enable();

            if (SettingsManager.configDropGunEverywhere.Value) DropGunEverywhere.Enable();

            if (SettingsManager.configGunTweaks.Value) Harmony.CreateAndPatchAll(typeof(GunTweaks));
            if (SettingsManager.configRobotTweaks.Value) Harmony.CreateAndPatchAll(typeof(RobotTweaks));
            if (SettingsManager.configVictorianFix.Value) Harmony.CreateAndPatchAll(typeof(rtlgTweaks));
            Harmony.CreateAndPatchAll(typeof(PostProcessTweaks));
            Harmony.CreateAndPatchAll(typeof(lahTweaks));
            Harmony.CreateAndPatchAll(typeof(DropGunEverywhere.DropButtonTimeOffsetTranspiler));
            Harmony.CreateAndPatchAll(typeof(RobotTweaks.TurretLightUpdateTranspiler));
            Harmony.CreateAndPatchAll(typeof(RobotTweaks.TripmineUpdateTranspiler));
            Harmony.CreateAndPatchAll(typeof(MenuManagerTweaks));

            ReceiverEvents.StartListening(ReceiverEventTypeVoid.PlayerInitialized, new UnityAction<ReceiverEventTypeVoid>(OnInitialize));
            ReceiverEvents.StartListening(ReceiverEventTypeVoid.PlayerInitialized, new UnityAction<ReceiverEventTypeVoid>(PostProcessTweaks.OnPlayerInitialize));
            ReceiverEvents.StartListening(ReceiverEventTypeVoid.PlayerInitialized, new UnityAction<ReceiverEventTypeVoid>(RobotTweaks.OnPlayerInitialize));
            ReceiverEvents.StartListening(ReceiverEventTypeVoid.PlayerInitialized, new UnityAction<ReceiverEventTypeVoid>(rtlgTweaks.OnInitialize));

            Receiver2ModdingKit.ModdingKitCorePlugin.AddTaskAtCoreStartup(new Receiver2ModdingKit.ModdingKitCorePlugin.StartupAction(RobotTweaks.PatchBombBotPrefab));
            Receiver2ModdingKit.ModdingKitCorePlugin.AddTaskAtCoreStartup(new Receiver2ModdingKit.ModdingKitCorePlugin.StartupAction(RobotTweaks.PatchPowerLeechPrefab));
            Receiver2ModdingKit.ModdingKitCorePlugin.AddTaskAtCoreStartup(new Receiver2ModdingKit.ModdingKitCorePlugin.StartupAction(GunTweaks.PatchDeaglesSpring));
            Receiver2ModdingKit.ModdingKitCorePlugin.AddTaskAtCoreStartup(new Receiver2ModdingKit.ModdingKitCorePlugin.StartupAction(GunTweaks.PatchHiPointCatchMagSlideAmount));
            Receiver2ModdingKit.ModdingKitCorePlugin.AddTaskAtCoreStartup(new Receiver2ModdingKit.ModdingKitCorePlugin.StartupAction(PostProcessTweaks.AddSettingsToStandardProfile));
        }

        private void OnInitialize(ReceiverEventTypeVoid ev)
        {
            if (SettingsManager.configDropGunEverywhere.Value) DropGunEverywhere.Enable();
        }

        public GameObject InstantiateMagazine(Vector3 position, Quaternion rotation, Transform parent, MagazineClass magazine_class)
        {
            var RCS = ReceiverCoreScript.Instance();
            RCS.player.lah.TryGetGun(out GunScript gun);
            RCS.TryGetMagazinePrefabFromRoot(gun.magazine_root_types[UnityEngine.Random.Range(0, gun.magazine_root_types.Length)], magazine_class, out MagazineScript magazinePrefab);
            return RuntimeTileLevelGenerator.instance.InstantiateMagazine(position, rotation, parent, magazinePrefab);
        }

        private void Update()
        {
            if (SettingsManager.configDiscoFlashlight.Value) FlashlightTweaks.Discolights();
            if (SettingsManager.configEnableTurretDiscoLights.Value) RobotTweaks.Discolights();
            if (SettingsManager.configFlashlightTweaks.Value) FlashlightTweaks.UpdateFlashlight(SettingsManager.configFlashlightToggleKey.Value);
        }
    }
}