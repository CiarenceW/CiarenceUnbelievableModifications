using HarmonyLib;
using Receiver2;
using Receiver2ModdingKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering.PostProcessing;

namespace CiarenceUnbelievableModifications
{
	internal class RessourcesHandler
	{
		internal class BombBotReplacerHandler
		{
			internal static void ReplaceBombBotPrefab()
			{
				Ressources.Instance.bombBotNewPrefab.GetComponent<BombBotScript>().fade_overlay_prefab = ReceiverCoreScript.Instance().enemy_prefabs.bomb_bot.GetComponent<BombBotScript>().fade_overlay_prefab;
				Ressources.Instance.bombBotOldPrefab = ReceiverCoreScript.Instance().enemy_prefabs.bomb_bot;
				ReceiverCoreScript.Instance().enemy_prefabs.bomb_bot = Ressources.Instance.bombBotNewPrefab;

				CiarenceUnbelievableModifications.MainPlugin.Logger.LogDebugWithColor("Replaced Bomb Bot Prefab", ConsoleColor.Magenta);
			}
		}

		internal class ComputeShadersHandler
		{
			[HarmonyPatch(typeof(PostProcessRenderContext), "get_resources")]
			[HarmonyPostfix]
			private static void SwitchFromBadComputeShadersToCoolComputeShaders(PostProcessResources __result)
			{
				__result.computeShaders = Ressources.Instance.computeShaders;
			}
		}
	}
}
