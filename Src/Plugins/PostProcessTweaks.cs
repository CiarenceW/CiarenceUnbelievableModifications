using Receiver2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace CiarenceUnbelievableModifications
{
    public static class PostProcessTweaks
    {
        private static LocalAimHandler lah;
        private static ReceiverFog receiver_fog;
        public static bool verbose;

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
            LocalAimHandler.TryGetInstance(out lah);
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
                receiver_fog.eastColor.overrideState = true;
                receiver_fog.westColor.overrideState = true;
                if (verbose) Debug.Log(receiver_fog.name);
            }
            if (receiver_fog != null)
            {
                if (verbose) Debug.Log(receiver_fog.name);
                if (verbose) Debug.Log(receiver_fog.eastColor.value);
                if (verbose) Debug.Log(receiver_fog.westColor.value);
                UpdateFogColour();
            }
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
