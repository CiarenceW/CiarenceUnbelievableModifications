using UnityEngine;
using HarmonyLib;
using Receiver2;

namespace CiarenceUnbelievableModifications
{
    public static class InventoryGlintColourRandomizer
    {
        internal static readonly Color BaseGlintColour = new Color(1, 0.9266f, 0.5047f);

        [HarmonyPatch(typeof(InventoryItem), nameof(InventoryItem.Awake))]
        [HarmonyPostfix]
        private static void PatchInventoryItemAwake(ref InventoryItem __instance)
        {
            if (SettingsManager.configInventoryGlintColourEnabled.Value == true)
            {
                if (__instance.glint_renderer != null && __instance.glint_renderer.material != null)
                {
                    Color randomColor = UnityEngine.Random.ColorHSV(0, 1, 0, 1, 0.7f, 1);
                    if (SettingsManager.Verbose) Debug.Log(__instance.InternalName + "'s new Random Colour: " + randomColor);
                    __instance.glint_renderer.material.SetColor("_GlintColor", randomColor);
                }
            }
        }
    }
}
