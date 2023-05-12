using BepInEx;
using HarmonyLib;
using Receiver2;
using UnityEngine;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiarenceUnbelievableModifications
{
    public static class GunTweaks
    {
        //only fixes the main_spring on the deagles for now.

        [HarmonyPatch(typeof(ReceiverCoreScript), "Awake")]
        [HarmonyPostfix]
        private static void PatchCoreAwake(ref ReceiverCoreScript __instance)
        {
			var bomb_bot = __instance.enemy_prefabs.bomb_bot.GetComponent<BombBotScript>();
			bomb_bot.voice_filter = "event:/TextToSpeech/TextToSpeech - bomb bot";
        }
    }
}