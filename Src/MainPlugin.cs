using BepInEx;
using Receiver2;
using UnityEngine.Events;
using UnityEngine;
using HarmonyLib;
using BepInEx.Configuration;
using System;
using Receiver2ModdingKit;
using Receiver2ModdingKit.Logging;
using Extensions = Receiver2ModdingKit.Extensions;

namespace CiarenceUnbelievableModifications
{
	[BepInProcess("Receiver2")]
	[BepInDependency("pl.szikaka.receiver_2_modding_kit", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("Ciarencew.CiarencesUnbelievableModifications", "CiarencesUnbelievableModifications", "1.10.0")]
    public class MainPlugin : BaseUnityPlugin
    {
        //all this kinda sucks, like, I probably should've tried to uniform everything and stuff. But I started doing all that at 3am and can't currently be arsed to make it all better. So oops.
        internal static ConfigFile config;

        //standard HID naming: [AcronymOfTweakName]-HID
        internal const string InventoryGlintColourRandomizerHID = "IGCR-HID";
        internal const string SpawnCompatibleMagsHID = "SCM-HID";

		internal static new ExtendedManualLogSource Logger
		{
			get;
			private set;
		}

        private void Awake()
        {
			// Plugin startup logic
			Logger = new ExtendedManualLogSource(this.GetBepInAttribute().Name);

			Receiver2ModdingKit.Extensions.BackgroundColor = ConsoleColor.DarkRed;
            Logger.LogMessageWithColor($"Plugin CiarencesUnbelievableModifications is loaded!", ConsoleColor.DarkYellow);
			Receiver2ModdingKit.Extensions.BackgroundColor = ConsoleColor.Black;

			// July 14th https://en.wikipedia.org/wiki/Bastille_Day
			DateTimeOffset cestTime = new DateTimeOffset(DateTime.UtcNow);
			cestTime = cestTime.ToOffset(new TimeSpan(2, 0, 0));
			if (cestTime.Day == 14 && cestTime.Month == 7)
			{
				Logger.LogMessageWithColor($"Happy", ConsoleColor.DarkBlue);
				Logger.LogMessageWithColor($"French", ConsoleColor.White);
				Logger.LogMessageWithColor($"Day", ConsoleColor.Red);
			}

            config = Config;

			if (Receiver2ModdingKit.Helpers.AssetHelper.FindAssetBundle<Ressources>(out var ressources))
			{
				UnityEngine.Object.DontDestroyOnLoad(Instantiate(ressources, this.transform.parent));
			}
			else
			{
				Logger.LogFatal("Didn't find Ressources thing, are you not on Windows, MacOSX, or Linux?");
			}

            SettingsManager.InitializeAndBindSettings();

            if (SettingsManager.configTurretAmmoBoxBoom.Value) TurretAmmoBoxBoom.Enable();

            if (SettingsManager.configDropGunEverywhere.Value) DropGunEverywhere.Enable();

            if (SettingsManager.configGunTweaks.Value) Harmony.CreateAndPatchAll(typeof(GunTweaks));
            if (SettingsManager.configRobotTweaks.Value) Harmony.CreateAndPatchAll(typeof(RobotTweaks));
            if (SettingsManager.configSpawnCompatibleMags.Value) Harmony.CreateAndPatchAll(typeof(rtlgTweaks.SpawnCompatibleMagsTweak), SpawnCompatibleMagsHID);
            Harmony.CreateAndPatchAll(typeof(lahTweaks));
            Harmony.CreateAndPatchAll(typeof(DropGunEverywhere.DropButtonTimeOffsetTranspiler));
            Harmony.CreateAndPatchAll(typeof(RobotTweaks.TurretLightUpdateTranspiler));
            Harmony.CreateAndPatchAll(typeof(RobotTweaks.TripmineUpdateTranspiler));
            Harmony.CreateAndPatchAll(typeof(MenuManagerTweaks));
            Harmony.CreateAndPatchAll(typeof(TapeLocatron3000));
            Harmony.CreateAndPatchAll(typeof(TurretAmmoBoxBoom));
            Harmony.CreateAndPatchAll(typeof(SpecialEventsTweaks.HalloweenTweaks));
            Harmony.CreateAndPatchAll(typeof(RobotTweaks.BombBotPatch.Transpilers));
            Harmony.CreateAndPatchAll(typeof(RobotTweaks.BombBotPatch.Patches));
            Harmony.CreateAndPatchAll(typeof(TapePlayerScriptTweaks.Patches));
            Harmony.CreateAndPatchAll(typeof(TapePlayerScriptTweaks.Transpilers));
            Harmony.CreateAndPatchAll(typeof(rtlgTweaks.TapePlayerScriptCompatibilityPatch));
			Harmony.CreateAndPatchAll(typeof(UnlockManager.Transpilers));
			Harmony.CreateAndPatchAll(typeof(DroneLightDisabler));
			Harmony.CreateAndPatchAll(typeof(SniperBotUnfucker));
			if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Direct3D12) Harmony.CreateAndPatchAll(typeof(RessourcesHandler.ComputeShadersHandler));
            if (SettingsManager.configInventoryGlintColourEnabled.Value == true) Harmony.CreateAndPatchAll(typeof(InventoryGlintColourRandomizer), InventoryGlintColourRandomizerHID);
            //Harmony.CreateAndPatchAll(typeof(Leaning));

            ReceiverEvents.StartListening(ReceiverEventTypeVoid.PlayerInitialized, new UnityAction<ReceiverEventTypeVoid>(OnInitialize));
            ReceiverEvents.StartListening(ReceiverEventTypeVoid.PlayerInitialized, new UnityAction<ReceiverEventTypeVoid>(PostProcessTweaks.OnPlayerInitialize));
            ReceiverEvents.StartListening(ReceiverEventTypeVoid.PlayerInitialized, new UnityAction<ReceiverEventTypeVoid>(CustomCampaignChecker.OnPlayerInitialize));
            ReceiverEvents.StartListening(ReceiverEventTypeVoid.PlayerInitialized, new UnityAction<ReceiverEventTypeVoid>(rtlgTweaks.OnInitialize));

			AddTasksAtCoreStartup(
				RobotTweaks.BombBotPatch.PatchBombBotPrefab,
				RobotTweaks.PatchPowerLeechPrefab,
				GunTweaks.PatchDeaglesSpring,
				GunTweaks.PatchHiPointCatchMagSlideAmount,
				PostProcessTweaks.AddSettingsToStandardProfile,
				FPSLimiterTweaks.Initialize,
				Leaning.Initialize,
				RessourcesHandler.BombBotReplacerHandler.ReplaceBombBotPrefab,
				TapePlayerScriptTweaks.CreateTapePlayerBindingEntries,
				SniperBotUnfucker.UnfuckVoiceFilter
				);
        }

        private void AddTasksAtCoreStartup(params ModdingKitEvents.StartupAction[] startupActions)
        {
            for ( int i = 0; i < startupActions.Length; i++)
            {
                ModdingKitEvents.AddTaskAtCoreStartup(startupActions[i]);
            }
        }

        private void OnInitialize(ReceiverEventTypeVoid ev)
        {
            if (SettingsManager.configDropGunEverywhere.Value) DropGunEverywhere.Enable();
        }

        private void OnApplicationFocus(bool focused)
        {
            FPSLimiterTweaks.ToggleFocusLostLimiter(focused);
        }

        private void Update()
        {
            if (SettingsManager.configDiscoFlashlight.Value && ReceiverCoreScript.Instance() != null) FlashlightTweaks.Discolights();
            if (SettingsManager.configEnableTurretDiscoLights.Value && ReceiverCoreScript.Instance() != null) RobotTweaks.Discolights();
            if (SettingsManager.configFlashlightTweaks.Value && ReceiverCoreScript.Instance() != null) FlashlightTweaks.UpdateFlashlight(SettingsManager.configFlashlightToggleKey.Value);
        }
    }
}