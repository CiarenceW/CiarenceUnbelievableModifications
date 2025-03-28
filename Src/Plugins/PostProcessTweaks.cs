﻿using BepInEx.Configuration;
using Receiver2;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;

namespace CiarenceUnbelievableModifications
{
	internal static class PostProcessTweaks
    {
        private static LocalAimHandler lah;
        private static ReceiverFog receiver_fog;
        public static bool verbose;

        internal static Receiver2ModdingKit.SettingsMenuManager.SettingsMenuEntry<float> motionBlurSlider;

        internal static Receiver2ModdingKit.SettingsMenuManager.SettingsMenuEntry<string> ssrDropDown;

        internal static MotionBlur motionBlur;

        internal static ScreenSpaceReflections ssr;

        public static Color east_beginner_colour = new Color(0.5f, 0.2f, 1f);
        public static Color west_beginner_colour = new Color(0.5f, 1f, 0.2f);

        public static Color east_sleeper_colour = new Color(0.1751f, 0.0753f, 0.3396f);
        public static Color west_sleeper_colour = new Color(0.1178f, 0.2453f, 0.0405f);

        public static Color east_sleepwalker_colour = new Color(0.1751f, 0.0753f, 0.3396f);
        public static Color west_sleepwalker_colour = new Color(0.1178f, 0.2453f, 0.0405f);

        public static Color east_fire_colour = new Color(1f, 0.5408f, 0f);
        public static Color west_fire_colour = new Color(1f, 0.398f, 0f);

        public static Color east_awake_colour = new Color(0.5f, 0.2f, 1f);
        public static Color west_awake_colour = new Color(0.5f, 1f, 0.2f);

        public static Color east_other_colour = new Color(0.5f, 0.2f, 1f);
        public static Color west_other_colour = new Color(0.5f, 1f, 0.2f);

        internal static void AddSettingsToStandardProfile()
        {
            CreateSettingsMenuEntries(); 
            //it didn't use to be like that but then there was a bug with events for custom settings menu buttons
            //so I had to move all of it and stuff and now I'm just lazy
        }

        public static Color GetCurrentColourEast()
        {
            //as it turns out, when you go from the dreaming to the compound, the WorldGenerationConfiguration.level_name doesn't get cleared.
            if (ReceiverCoreScript.Instance().game_mode.GetGameMode() != GameMode.RankingCampaign) 
                return east_other_colour;
            switch (ReceiverCoreScript.Instance().WorldGenerationConfiguration.level_name)
            {
                case "introduction":
                case "beginner":
                    return east_beginner_colour;
                case "sleeper":
                    return east_sleeper_colour;
                case "sleepwalker":
                    return east_sleepwalker_colour;
                case "dawned":
                    return east_fire_colour;
                case "awake":
                    return east_awake_colour;
                default:
                    return east_other_colour;
            }
        }

        public static Color GetCurrentColourWest()
        {
            if (ReceiverCoreScript.Instance().game_mode.GetGameMode() != GameMode.RankingCampaign)
                return west_other_colour;
            switch (ReceiverCoreScript.Instance().WorldGenerationConfiguration.level_name)
            {
                case "introduction":
                case "beginner":
                    return west_beginner_colour;
                case "sleeper":
                    return west_sleeper_colour;
                case "sleepwalker":
                    return west_sleepwalker_colour;
                case "dawned":
                    return west_fire_colour;
                case "awake":
                    return west_awake_colour;
                default:
                    return west_other_colour;
            }
        }

        public static void OnPlayerInitialize(ReceiverEventTypeVoid ev)
		{
			receiver_fog = null;

            List<PostProcessVolume> volumes = new List<PostProcessVolume>();
            PostProcessManager.instance.GetActiveVolumes(ReceiverCoreScript.Instance().player.lah.main_camera.GetComponent<PostProcessLayer>(), volumes);
            var fog_volumes = (from e in volumes where e.profile.HasSettings<ReceiverFog>() select e.profile.GetSetting<ReceiverFog>());

            //the last one is the one that is currently being used by the game
            if (fog_volumes.Count() > 0) receiver_fog = fog_volumes.Last();

            if (receiver_fog != null)
            {
                receiver_fog.eastColor.overrideState = true;
                receiver_fog.westColor.overrideState = true;
            }

            UpdateFogColour();
            /*LocalAimHandler.TryGetInstance(out lah);
            if (ReceiverCoreScript.Instance().game_mode.GetGameMode() == GameMode.RankingCampaign)
            {
                if (verbose) Debug.Log("receiver is receiving all over the place, guh damn");
                var highest_priority_volume = PostProcessManager.instance.GetHighestPriorityVolume(lah.main_camera.GetComponent<PostProcessLayer>());
                if (highest_priority_volume.name == "Blackout")
                {
                    receiver_fog = GlobalPostProcess.instance.default_fog_profile.GetSetting<ReceiverFog>();
                    receiver_fog.eastColor.overrideState = true;
                    receiver_fog.westColor.overrideState = true;
                }
                else
                {
                    receiver_fog = highest_priority_volume.profile.GetSetting<ReceiverFog>();
                }
                if (verbose) Debug.Log(receiver_fog.name);
            }
            if (ReceiverCoreScript.Instance().game_mode.GetGameMode() == GameMode.ReceiverMall)
            {
                if (verbose) Debug.Log("player is in the mall, he do be shopping doe");
                List<PostProcessVolume> volumes = new List<PostProcessVolume>();
                PostProcessManager.instance.GetActiveVolumes(lah.main_camera.GetComponent<PostProcessLayer>(), volumes, false, false);
                receiver_fog = volumes.First( e => e.name == "receiver_fog_override").profile.GetSetting<ReceiverFog>();
                if (verbose) Debug.Log(receiver_fog.name);
            }
            if (receiver_fog != null)
            {
                if (verbose) Debug.Log(receiver_fog.name);
                if (verbose) Debug.Log(receiver_fog.eastColor.value);
                if (verbose) Debug.Log(receiver_fog.westColor.value);
                UpdateFogColour();
            }*/
        }

        private static void CreateSettingsMenuEntries()
		{
			Receiver2ModdingKit.SettingsMenuManager.CreateSettingsMenuOption<bool>("Enable Screen Space Reflections", SettingsManager.configSSREnabled, 14).control.GetComponent(Type.GetType("Receiver2.ToggleComponent, Wolfire.Receiver2, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"));
            ssrDropDown = Receiver2ModdingKit.SettingsMenuManager.CreateSettingsMenuOption<string>("Screen Space Reflections Quality", SettingsManager.configSSRQuality, 15);
            var ssrDropDownComp = ssrDropDown.control.GetComponent<DropdownComponent>();
            ssrDropDownComp.OnChange.AddListener(value => ChangeSSRQualitySetting(ssrDropDownComp.SelectedIndex)); //I don't understand why this works and it makes me mad
            ssrDropDown.control.SetActive(SettingsManager.configSSREnabled.Value);
            ssrDropDown.label.SetActive(SettingsManager.configSSREnabled.Value);

            //do that here otherwise the fucking soften gun sounds thing get trued
            if (!GlobalPostProcess.instance.default_standard_profile.HasSettings<ScreenSpaceReflections>()) ssr = GlobalPostProcess.instance.default_standard_profile.AddSettings<ScreenSpaceReflections>();
            ssr.active = SettingsManager.configSSREnabled.Value;

            ssr.distanceFade.value = 0f;
            ssr.distanceFade.overrideState = true;

            ssr.resolution.value = ScreenSpaceReflectionResolution.Supersampled;
            ssr.resolution.overrideState = true;

            ssr.thickness.value = 20f;
            ssr.thickness.overrideState = true;

            ssr.maximumIterationCount.value = 256;
            ssr.maximumIterationCount.overrideState = true;

            ssr.maximumMarchDistance.value = 100f;
            ssr.maximumMarchDistance.overrideState = true;

            ssr.vignette.value = 0.2f;
            ssr.vignette.overrideState = true;

            ssr.preset.value = (ScreenSpaceReflectionPreset)(((AcceptableValueList<string>)SettingsManager.configSSRQuality.Description.AcceptableValues).AcceptableValues.ToList().FindIndex(value => value == SettingsManager.configSSRQuality.Value));
            ssr.preset.overrideState = true;

            var motionBlurToggle = Receiver2ModdingKit.SettingsMenuManager.CreateSettingsMenuOption<bool>("Enable Motion Blur", SettingsManager.configMotionBlurEnabled, 17).control.GetComponent(Type.GetType("Receiver2.ToggleComponent, Wolfire.Receiver2, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"));
            UnityEvent<bool> motionBlurToggleEvent = (UnityEvent<bool>)motionBlurToggle.GetType().GetField("OnChange").GetValue(motionBlurToggle);
            motionBlurSlider = Receiver2ModdingKit.SettingsMenuManager.CreateSettingsMenuOption<float>("Motion Blur Intensity", SettingsManager.configMotionBlurIntensity, 18);
            var motionBlurSliderComp = motionBlurSlider.control.GetComponent<SliderComponent>();
            motionBlurSliderComp.OnChange.AddListener(value => ChangeMotionBlurIntensity(motionBlurSliderComp.Value));
            motionBlurSlider.control.SetActive(SettingsManager.configMotionBlurEnabled.Value);
            motionBlurSlider.label.SetActive(SettingsManager.configMotionBlurEnabled.Value);

            //do that here otherwise master audio volume gets set
            if (!GlobalPostProcess.instance.default_standard_profile.HasSettings<MotionBlur>()) motionBlur = GlobalPostProcess.instance.default_standard_profile.AddSettings<MotionBlur>();
            motionBlur.active = SettingsManager.configMotionBlurEnabled.Value;
            motionBlur.sampleCount.value = 100;
            motionBlur.sampleCount.overrideState = true;
            motionBlur.shutterAngle.value = SettingsManager.configMotionBlurIntensity.Value;
            motionBlur.shutterAngle.overrideState = true;
        }

        internal static void ToggleMotionBlur()
        {
            GlobalPostProcess.instance.default_standard_profile.GetSetting<MotionBlur>().active = SettingsManager.configMotionBlurEnabled.Value;
            if (motionBlurSlider != null)
            {
                motionBlurSlider.control.SetActive(SettingsManager.configMotionBlurEnabled.Value);
                motionBlurSlider.label.SetActive(SettingsManager.configMotionBlurEnabled.Value);
            }
        }

        internal static void ToggleSSR()
        {
            GlobalPostProcess.instance.default_standard_profile.GetSetting<ScreenSpaceReflections>().active = SettingsManager.configSSREnabled.Value;
            if (ssrDropDown != null)
            {
                ssrDropDown.control.SetActive(SettingsManager.configSSREnabled.Value);
                ssrDropDown.label.SetActive(SettingsManager.configSSREnabled.Value);
            }
        }

        private static void ChangeSSRQualitySetting(int value)
        {
            var ssrQuality = GlobalPostProcess.instance.default_standard_profile.GetSetting<ScreenSpaceReflections>();
            ssrQuality.preset.value = new ParameterOverride<ScreenSpaceReflectionPreset>((ScreenSpaceReflectionPreset)value);
        }

        private static void ChangeMotionBlurIntensity(float value)
        {
            var motionBlurIntensity = GlobalPostProcess.instance.default_standard_profile.GetSetting<MotionBlur>();
            motionBlurIntensity.shutterAngle.value = value;
        }

        public static void UpdateFogColour()
        {
            if (receiver_fog != null)
            {
                if (verbose) Debug.Log("Updating fog colours");
                if (verbose) Debug.LogFormat("Current east fog color is {0}", receiver_fog.eastColor.value);
                if (verbose) Debug.LogFormat("Current west fog color is {0}", receiver_fog.westColor.value);
                if (verbose) Debug.LogFormat("Current custom east fog color is {0}", GetCurrentColourEast());
                if (verbose) Debug.LogFormat("Current custom west fog color is {0}", GetCurrentColourWest());
                receiver_fog.eastColor.value = GetCurrentColourEast();
                receiver_fog.westColor.value = GetCurrentColourWest();
                if (verbose) Debug.LogFormat("Current east fog color is {0}", receiver_fog.eastColor.value);
                if (verbose) Debug.LogFormat("Current west fog color is {0}", receiver_fog.westColor.value);
            }
        }
    }
}
