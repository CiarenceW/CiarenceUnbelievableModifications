using HarmonyLib;
using Receiver2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Receiver2ModdingKit;
using UnityEngine.SceneManagement;

namespace CiarenceUnbelievableModifications
{
    internal static class FPSLimiterTweaks
    {
        private static bool isLoading; //fps gets limited when loading, increasing loading times by a lot

        internal static void Initialize()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        internal static void CreateSettingsMenuEntries()
        {
            SettingsMenuManager.CreateSettingsMenuOption<bool>("Focus Lost FPS Limiter", SettingsManager.configLimitFPSFocusLostEnabled, 23);
            SettingsMenuManager.CreateSettingsMenuOption<float>("Focus Lost FPS Limit", SettingsManager.configLimitFPSFocusLostCount, 24);
        }
        internal static void ToggleFocusLostLimiter(bool focused)
        {
            if (SettingsManager.configLimitFPSFocusLostEnabled.Value)
            {
                if (isLoading)
                {
                    Application.targetFrameRate = -1;
                    return;
                }
                Application.targetFrameRate = (int)((focused) ? ((ConfigFiles.global.fps_limiter_active) ? ConfigFiles.global.fps_limit : -1) : SettingsManager.configLimitFPSFocusLostCount.Value); //evil double ternary because bored :D
            }
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            isLoading = false;
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            isLoading = true;
        }
    }
}
