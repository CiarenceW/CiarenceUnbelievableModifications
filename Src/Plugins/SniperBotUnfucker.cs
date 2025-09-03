using FMOD;
using FMOD.Studio;
using FMODUnity;
using HarmonyLib;
using Receiver2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CiarenceUnbelievableModifications
{
	internal static class SniperBotUnfucker
	{
		static Dictionary<string, string> pissDic = new Dictionary<string, string>()
			{
				{ "Target spotted", "targetSpotted.mp3" },
				{ "Eyes on target", "eyesontarget.mp3" },
				{ "I've got visual", "ivegotvisual.mp3" },
				{ "Target acquired", "targetacquired.mp3" },
				{ "I see the target", "iseethetarget.mp3" },
				{ "Target lost", "targetlost.mp3" },
				{ "Target has disappeared", "targethasdisappeared.mp3" },
				{ "I've lost visual", "ivelostvisual.mp3" },
				{ "Target occluded", "targetoccluded.mp3" },
				{ "I can't see the target", "icantseethetarget.mp3" },
				{ "Aiming", "aiming.mp3" },
				{ "Calculating ballistics", "calculatingballistics.mp3" },
				{ "Going to take the shot", "goingtotaketheshot.mp3" },
				{ "Firing", "firing.mp3" },
				{ "Taking the shot", "takingtheshot.mp3" },
				{ "Tango down", "tangodown.mp3" },
				{ "Enemy defeated", "enemydefeated.mp3" },
				{ "Good hit", "goodhit.mp3" },
				{ "Good kill", "goodkill.mp3" },
				{ "On target", "ontarget.mp3" },
				{ "Target is down", "targetisdown.mp3" },
				{ "I'm melting!", "immelting.mp3" },
				{ "Oh no, I'm dead", "ohnoimdead.mp3" },
				{ "Someone hit my killswitch", "someonehitmykillswitch.mp3" },
				{ "Goodbye", "goodbye.mp3" },
				{ "Changing position", "changingposition.mp3" },
				{ "Relocation", "relocation.mp3" },
				{ "Shifting perspective", "shiftingperspective.mp3" }
			};

		static string SnipeSoundFolder 
		{
			get;
		} = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "sniperBotVoiceLines");

		internal static void UnfuckVoiceFilter()
		{
			ReceiverCoreScript.Instance().enemy_prefabs.sniper_bot.GetComponent<SniperBot>().voice_filter = "event:/TextToSpeech/TextToSpeech - radio";
		}

		[HarmonyPatch(typeof(SniperBot), nameof(SniperBot.Say))]
		[HarmonyPostfix]
		private static void MakeSniperBotActuallyFuckingSaySomething(SniperBot __instance, string text, ref EventInstance ___speech_instance)
		{
			var eventInstance = FMODUnity.RuntimeManager.CreateInstance(__instance.voice_filter);
			GCHandle handle = GCHandle.Alloc(Path.Combine(SnipeSoundFolder, pissDic[text]), GCHandleType.Pinned);
			eventInstance.set3DAttributes(LocalAimHandler.player_instance.main_camera.transform.position.To3DAttributes());
			eventInstance.setUserData(GCHandle.ToIntPtr(handle));
			eventInstance.setCallback(FuckYou, EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND | EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND | EVENT_CALLBACK_TYPE.DESTROYED);
			eventInstance.start();
			__instance.SpeechCallback(eventInstance);
		}

		private static RESULT FuckYou(EVENT_CALLBACK_TYPE type, EventInstance instance, IntPtr parameterPtr)
		{
			instance.getUserData(out var intPtr);
			GCHandle gchandle = GCHandle.FromIntPtr(intPtr);
			string text = gchandle.Target as string;

			if (type == EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND)
			{
				PROGRAMMER_SOUND_PROPERTIES pROGRAMMER_SOUND_PROPERTIES = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));
				if (RuntimeManager.CoreSystem.createSound(text, MODE.DEFAULT, out var sound) == RESULT.OK)
				{
					pROGRAMMER_SOUND_PROPERTIES.sound = sound.handle;
					Marshal.StructureToPtr<PROGRAMMER_SOUND_PROPERTIES>(pROGRAMMER_SOUND_PROPERTIES, parameterPtr, false);
				}
			}
			else if (type == EVENT_CALLBACK_TYPE.DESTROYED)
			{
				gchandle.Free();
			}
			else if (type == EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND)
			{
				PROGRAMMER_SOUND_PROPERTIES programmer_SOUND_PROPERTIES = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));
				Sound sound = default(Sound);
				sound.handle = programmer_SOUND_PROPERTIES.sound;
				sound.release();
			}

			return RESULT.OK;
		}
	}
}
