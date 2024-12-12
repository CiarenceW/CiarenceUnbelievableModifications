using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using Receiver2;
using System.Net;
using Wolfire;
using System;

namespace CiarenceUnbelievableModifications
{
    internal static class TapeLocatron3000
    {
        internal static float light_distance = 10f;
        internal static float time_occluded = 0f;

        [HarmonyPatch(typeof(RuntimeTileLevelGenerator), "Update")]
        [HarmonyPostfix]
        private static void PatchRuntimeTileGenUpdate(ref RuntimeTileLevelGenerator __instance, List<(int, ActiveItem)> ___tapes_closest_to_player)
        {
            if (LocalAimHandler.player_instance != null)
            {
                if (SettingsManager.configTapeLocatron3000Enabled.Value == false) return;

                foreach (var closest_tape in ___tapes_closest_to_player)
                {
                    var tape = closest_tape.Item2;
                    if (LocalAimHandler.player_instance.main_camera != null)
                    {
                        ApplyIndividualDroneLightNoRayCast(tape.position, (LocalAimHandler.player_instance.GetCameraPos() - tape.position).normalized, SettingsManager.configTapeLocatron3000ColourLos.Value, SettingsManager.configTapeLocatron3000ColourOccluded.Value, light_distance);
                    }
                }
            }
        }
        public static void ApplyIndividualDroneLightNoRayCast(Vector3 origin, Vector3 light_direction, Color color_los, Color color_occluded, float light_range)
        {
            Vector3 vector = LocalAimHandler.player_instance.main_camera.transform.position - origin;
            float num = Vector3.Magnitude(vector);
            if (num > light_range)
            {
                return;
            }

            Vector3 vector2 = Vector3.Normalize(vector);
            float num2 = Vector3.Dot(vector2, light_direction);
            float num3 = Mathf.Lerp(0f, 1f, (num2 - 0.7f) / 0.3f);
            num3 *= 1f - num / light_range;
            if (num3 > 0f)
            {
                Color color = color_los;
                if (!(!Physics.Raycast(origin, vector2, out var hitInfo, num, ReceiverCoreScript.Instance().layer_mask_vision) || StochasticVision.CheckHitPlayer(hitInfo, LocalAimHandler.player_instance)))
                {
                    color = color_occluded;
                }

                //if (color == color_occluded)
                //    time_occluded += Time.deltaTime * 10f;
                //else
                //    time_occluded -= Time.deltaTime * 10f;
                //time_occluded = Mathf.Clamp01(time_occluded);

                //color = Color.Lerp(color_los, color_occluded, time_occluded);

                color *= AudioManager.Instance().GetComponent<MusicScript>().Mystical;
                float num4 = 4f;
                num3 = Mathf.Pow(num3, 4f);
                num3 *= num4;
                float num5 = Vector3.Dot(-vector2, LocalAimHandler.player_instance.main_camera.transform.right);
                float num6 = Vector3.Dot(-vector2, LocalAimHandler.player_instance.main_camera.transform.up);
                if (num5 > 0f)
                {
                    ScreenEffect.Flash(ScreenEffect.ScreenFlashEvent.Type.AddGradient, new Color(color.r, color.g, color.b, num3 * num5), 0f, 0f, ScreenFlash.Dir.Right);
                }

                if (num5 < 0f)
                {
                    ScreenEffect.Flash(ScreenEffect.ScreenFlashEvent.Type.AddGradient, new Color(color.r, color.g, color.b, num3 * (0f - num5)), 0f, 0f, ScreenFlash.Dir.Left);
                }

                if (num6 > 0f)
                {
                    ScreenEffect.Flash(ScreenEffect.ScreenFlashEvent.Type.AddGradient, new Color(color.r, color.g, color.b, num3 * num6), 0f, 0f, ScreenFlash.Dir.Up);
                }

                if (num6 < 0f)
                {
                    ScreenEffect.Flash(ScreenEffect.ScreenFlashEvent.Type.AddGradient, new Color(color.r, color.g, color.b, num3 * (0f - num6)), 0f, 0f, ScreenFlash.Dir.Down);
                }
            }
        }
    }
}
