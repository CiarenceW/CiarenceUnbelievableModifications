using FMOD.Studio;
using FMODUnity;
using HarmonyLib;
using Receiver2;
using Receiver2ModdingKit.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.WSA.Input;

namespace CiarenceUnbelievableModifications
{
    internal class TapePlayerScriptTweaks
    {
		static KeybindingComponent tapePlayerEntry;

		internal static void CreateTapePlayerBindingEntries()
		{
			var contentMenu = ReceiverCoreScript.Instance().transform.Find("Menus/Overlay Menu Canvas/Aspect Ratio Fitter/New Pause Menu/Backdrop1/Sub-Menu Layout Group/New Keybinding Menu/ScrollableContent Variant/Viewport/Content/");

			var suicideEntry = contentMenu.Find("Suicide");

			var tapePlayerEntryGo = GameObject.Instantiate(suicideEntry.gameObject, contentMenu);

			tapePlayerEntryGo.name = "Toggle Tape Player";

			tapePlayerEntryGo.transform.SetSiblingIndex(19);

			tapePlayerEntry = tapePlayerEntryGo.GetComponent<KeybindingComponent>();

			tapePlayerEntry.action_id = RewiredConsts.Action.Toggle_Tape_Player;
		}

        internal static class Transpilers
        {
            [HarmonyPatch(typeof(TapePlayerScript), "LoadAudio")]
            [HarmonyTranspiler]
            private static IEnumerable<CodeInstruction> PatchShitPath(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase __originalMethod)
            {
                CodeMatcher codeMatcher = new CodeMatcher(instructions, generator)
                    .MatchForward(true,
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(TapePlayerScript), "HasRecording")),
                    new CodeMatch(OpCodes.Brfalse)
                    );

                if (!codeMatcher.ReportFailure(__originalMethod, Debug.LogError))
                {
                    var branchInstruction = codeMatcher.Operand;

                    codeMatcher
                        .Advance(1)
                        .InsertAndAdvance(
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Misc), nameof(Misc.HasAudioFilePath))),
                        new CodeInstruction(OpCodes.Brtrue, branchInstruction)
                        );
                }

                return codeMatcher.InstructionEnumeration();
            }
        }

        internal static class Patches
        {
			//counter gets updated before current loadout is assigned, so it'll be the tape picked up number, instead of the consumed number, I decided to switch the numbers around because you're more likely to have consumed tapes rather than picked up some
			[HarmonyPatch(typeof(RankingProgressionGameMode), "get_LevelTapeCollected")]
			[HarmonyPrefix]
			private static bool ReplacePickedUpCountByConsumedCount(RankingProgressionGameMode __instance, ref int __result)
			{
				if (ReceiverCoreScript.Instance().CurrentLoadout != null && ReceiverCoreScript.Instance().CurrentLoadout.simplified_tape_player)
				{
					return true;
				}

				__result = __instance.progression_data.regular_tapes_consumed - (__instance.TapeTarget - __instance.LevelTapeTarget);
				return false;
			}

			[HarmonyPatch(typeof(TapeScript), nameof(TapeScript.FixedUpdate))]
			[HarmonyPrefix]
			private static bool DisableStupidFuckingAutoSleep()
			{
				return false;
			}

			//there's another tape_player action whose action_id is 17, but it doesn't seem to actually do anything, the one used by the LocalAimHandler is 60, so we'll just patch that
			[HarmonyPatch(typeof(ControllerBindingScript), nameof(ControllerBindingScript.RewiredActionToButtonLabel))]
			[HarmonyPrefix]
			private static bool FixTapePlayerLocaleId(int action_id, ref LocaleUIString __result)
			{
				if (action_id == RewiredConsts.Action.Toggle_Tape_Player)
				{
					__result = LocaleUIString.M_CBM_TAPE_PLAYER;
					return false;
				}

				return true;
			}

            private static AccessTools.FieldRef<TapePlayerScript, EventInstance> fmod_tape_contentRef = AccessTools.FieldRefAccess<TapePlayerScript, EventInstance>("fmod_tape_content");

            private static AccessTools.FieldRef<TapePlayerScript, float> old_wear_valueRef = AccessTools.FieldRefAccess<TapePlayerScript, float>("old_wear_value");

            private static AccessTools.FieldRef<FMODTapeFileStream, FMOD.Sound> soundRef = AccessTools.FieldRefAccess<FMODTapeFileStream, FMOD.Sound>("sound");

            [HarmonyPatch(typeof(TapePlayerScript), "LoadAudio")]
            [HarmonyPostfix]
            private static void LoadAudioPathtape(TapePlayerScript __instance)
            {
                if (Misc.TapeIsValid(__instance) && !Misc.HasRecording(__instance) && Misc.HasAudioFilePath(__instance))
                {
                    ref var fmod_tape_content = ref fmod_tape_contentRef.Invoke(__instance);

                    var extender = __instance.GetOrAddComponent<TapePlayerScriptExtender>();
                    extender.tapeStream = new FMODTapeFileStream();
                    fmod_tape_content = extender.tapeStream.LoadTapeContent(__instance.tape_instance_in_player.content);
                    float currentWear = __instance.tape_instance_in_player.content.GetCurrentWear();
                    fmod_tape_content.setParameterByName("DamagedMedia", currentWear, false);

                    old_wear_valueRef.Invoke(__instance) = currentWear;

                    soundRef.Invoke(extender.tapeStream).getLength(out uint num, FMOD.TIMEUNIT.MS);

                    __instance.tape_instance_in_player.content.UpdateTimelineLength((int)num);
                    fmod_tape_content.setPaused(__instance.tape_player_state != TapePlayerScript.TapePlayerState.Playing);
                    fmod_tape_content.setPitch(0.1f);
                    fmod_tape_content.start();
                    fmod_tape_content.setTimelinePosition(__instance.tape_instance_in_player.content.timeline_position);
                }
            }

            [HarmonyPatch(typeof(TapePlayerScript), nameof(TapePlayerScript.TapeIsValid))]
            [HarmonyPrefix]
            private static bool MakeTapeValidYouFuck(TapePlayerScript __instance, ref bool __result)
            {
                __result = Misc.TapeIsValid(__instance);
                return false;
            }

            private static AccessTools.FieldRef<LocalAimHandler, TapePlayerScript> currentTapePlayerScriptFieldInfo = AccessTools.FieldRefAccess<LocalAimHandler, TapePlayerScript>("current_tape_player_script");

            [HarmonyPatch(typeof(PlayerEquipment), nameof(PlayerEquipment.PutInSlot))]
            [HarmonyPostfix]
            private static void AssignMissingTapePlayerFields(InventoryItem item)
            {
                if (item is TapePlayerScript tapePlayer)
                {
                    currentTapePlayerScriptFieldInfo.Invoke(LocalAimHandler.player_instance) = tapePlayer;

                    tapePlayer.transform.localPosition = Vector3.zero;
                }
            }

            [HarmonyPatch(typeof(TapePlayerScript), nameof(TapePlayerScript.Play))]
            [HarmonyPostfix]
            private static void SetTapeSubtitles(TapePlayerScript __instance)
            {
                if (__instance.TryGetComponent<TapePlayerScriptExtender>(out var extender))
                {
                    if (extender.TryGetSubtitleManager(out var subtitle_manager))
                    {
                        if (__instance.tape_instance_in_player == null) return;
                        subtitle_manager.SetTapePlayingPosition(__instance.tape_instance_in_player.content.tape_id_string, 0, true);
                    }
                }
            }

            [HarmonyPatch(typeof(TapePlayerScript), nameof(TapePlayerScript.FastForward))]
            [HarmonyPatch(typeof(TapePlayerScript), nameof(TapePlayerScript.Rewind))]
			[HarmonyPrefix]
			private static bool CheckForGamePlayTape(TapePlayerScript __instance)
			{
				return Misc.ShouldntBlockPause(__instance);
			}

			[HarmonyPatch(typeof(TapePlayerScript), nameof(TapePlayerScript.Stop))]
            [HarmonyPrefix] 
            private static bool CheckForGamePlayTapeStop(TapePlayerScript __instance, bool force_stop)
            {
				return Misc.ShouldntBlockPause(__instance) || force_stop;
            }

            [HarmonyPatch(typeof(TapePlayerScript), "UpdateTape")]
            [HarmonyPostfix]
            private static void UpdateTapeSubtitles(TapePlayerScript __instance)
            {
                if (__instance.TryGetComponent<TapePlayerScriptExtender>(out var extender))
                {
                    if (extender.TryGetSubtitleManager(out var subtitle_manager))
                    {
                        if (__instance.tape_instance_in_player == null) return;
                        if (__instance.tape_instance_in_player.content.timeline_position > 1)
                        {
                            subtitle_manager.SetTapePlayingPosition(__instance.tape_instance_in_player.content.tape_id_string, __instance.tape_instance_in_player.content.timeline_position, false);
                        }
                    }
                }
			}

			//threat tapes have flags that make the event trigger x seconds before end, instead of x seconds after start, the simplified tape player supports this, but not the cool tape player :)
			[HarmonyPatch(typeof(TapePlayerScript), "UpdateTape")]
			[HarmonyTranspiler]
			private static IEnumerable<CodeInstruction> TranspileCountFromEndOfTapeFlagIntoTapePlayer(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase __originalMethod)
			{
				CodeMatcher codeMatcher = new CodeMatcher(instructions, generator)
					.MatchForward(true, new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(TapeTimestampEventContainer), nameof(TapeTimestampEventContainer.timestampSeconds))));

				var brTrueLabel = generator.DefineLabel();

				if (!codeMatcher.ReportFailure(__originalMethod, MainPlugin.Logger.LogError))
				{
					codeMatcher
						.CreateLabelAt(codeMatcher.Pos + 1, out var stlocLabel)
						.InsertAndAdvance(
						new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(TapeTimestampEventContainer), nameof(TapeTimestampEventContainer.countFromEndOfTape))),
						new CodeInstruction(OpCodes.Brtrue_S, brTrueLabel),
						new CodeInstruction(OpCodes.Ldloc_S, 11),
						new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(TapeTimestampEventContainer), nameof(TapeTimestampEventContainer.timestampSeconds))),
						new CodeInstruction(OpCodes.Br_S, stlocLabel),
						new CodeInstruction(OpCodes.Ldarg_0).WithLabels(brTrueLabel))
						.InsertAndAdvance(
						new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(TapePlayerScript), nameof(TapePlayerScript.tape_instance_in_player))),
						new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(TapeScript), nameof(TapeScript.content))),
						new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(TapeContent), nameof(TapeContent.timeline_length))),
						new CodeInstruction(OpCodes.Conv_R4),
						new CodeInstruction(OpCodes.Ldc_R4, 1000f),
						new CodeInstruction(OpCodes.Div),
						new CodeInstruction(OpCodes.Ldloc_S, 11))
						.Advance(1)
						.InsertAndAdvance(new CodeInstruction(OpCodes.Sub));
				}

				return codeMatcher.InstructionEnumeration();
			}

			[HarmonyPatch(typeof(TapePlayerScript), nameof(TapePlayerScript.Stop))]
            [HarmonyPostfix]
            private static void StopTapeSubtitles(TapePlayerScript __instance)
            {
                if (__instance.tape_player_state == TapePlayerScript.TapePlayerState.Stopped && __instance.TryGetComponent<TapePlayerScriptExtender>(out var extender))
                {
                    if (extender.TryGetSubtitleManager(out var subtitle_manager))
                    {
                        if (__instance.tape_instance_in_player == null) return;
                        subtitle_manager.SetTapeStop();
                    }
                }
            }

            [HarmonyPatch(typeof(TapePlayerScript), nameof(TapePlayerScript.Awake))]
            [HarmonyPostfix]
            private static void TryAddExtender(TapePlayerScript __instance)
            {
                if (!__instance.TryGetComponent<TapePlayerScriptExtender>(out var _))
                {
                    __instance.gameObject.AddComponent<TapePlayerScriptExtender>();
                }
            }

			[HarmonyPatch(typeof(TapeScript), nameof(TapeScript.Start))]
			[HarmonyPostfix]
			private static void RandomiseTapeBodyColour(TapeScript __instance, ref MaterialPropertyBlock ___body_prop_block)
			{
				if (ReceiverCoreScript.Instance().game_mode != null && ReceiverCoreScript.Instance().game_mode.GetGameMode() == GameMode.RankingCampaign)
				{
					___body_prop_block = new MaterialPropertyBlock();
					__instance.body_renderer.GetPropertyBlock(___body_prop_block);
					___body_prop_block.SetColor("_BodyColor", UnityEngine.Random.ColorHSV());
					___body_prop_block.SetColor("_LabelColor", UnityEngine.Random.ColorHSV());
					__instance.body_renderer.SetPropertyBlock(___body_prop_block);
				}
			}
        }

        internal static class Misc
        {
			internal static bool ShouldntBlockPause(TapePlayerScript __instance)
			{
				return __instance.tape_instance_in_player != null && (__instance.tape_instance_in_player.content.stoppable || __instance.tape_instance_in_player.content.is_consumed || __instance.tape_instance_in_player.ended);
			}

            internal static bool HasRecording(TapePlayerScript tapePlayerScript)
            {
                return tapePlayerScript.tape_instance_in_player != null && tapePlayerScript.tape_instance_in_player.content != null && tapePlayerScript.tape_instance_in_player.content.HasRecording;
            }

            internal static bool HasText(TapePlayerScript tapePlayerScript)
            {
                return tapePlayerScript.tape_instance_in_player != null && tapePlayerScript.tape_instance_in_player.content != null && tapePlayerScript.tape_instance_in_player.content.HasText;
            }

            internal static bool TapeIsValid(TapePlayerScript tapePlayerScript)
            {
                return tapePlayerScript.tape_instance_in_player != null && (HasRecording(tapePlayerScript) || HasText(tapePlayerScript) || HasAudioFilePath(tapePlayerScript));
            }

            internal static bool HasAudioFilePath(TapePlayerScript tapePlayerScript)
            {
                return tapePlayerScript.tape_instance_in_player != null && tapePlayerScript.tape_instance_in_player.content != null && tapePlayerScript.tape_instance_in_player.content.HasAudioFilePath;
            }
        }

        public class TapePlayerScriptExtender : MonoBehaviour
        {
            public TapePlayerScript tapePlayer;

            public FMODTapeFileStream tapeStream;

            public TapePlayerScript.TapePlayerState lastTapePlayerState;

            private void Start()
            {
                tapePlayer = GetComponent<TapePlayerScript>();

                ReceiverEvents.StartListening(ReceiverEventTypeVoid.OnGamePaused, new UnityAction<ReceiverEventTypeVoid>(this.OnPauseEvent));
                ReceiverEvents.StartListening(ReceiverEventTypeVoid.OnGameResumed, new UnityAction<ReceiverEventTypeVoid>(this.OnPauseEvent));
            }

            private void OnPauseEvent(ReceiverEventTypeVoid ev)
            {
                if (ev == ReceiverEventTypeVoid.OnGamePaused)
                {
                    lastTapePlayerState = tapePlayer.tape_player_state;
                    tapePlayer.Stop(true);
                }
                else if (ev == ReceiverEventTypeVoid.OnGameResumed)
                {
                    switch (lastTapePlayerState)
                    {
                        case TapePlayerScript.TapePlayerState.Stopped:
                            {
                                break;
                            }
                        case TapePlayerScript.TapePlayerState.Playing:
                            {
                                tapePlayer.Play();
                                break;
                            }
                        case TapePlayerScript.TapePlayerState.Rewinding:
                            {
                                tapePlayer.Rewind();
                                break;
                            }
                        case TapePlayerScript.TapePlayerState.FastForwarding:
                            {
                                tapePlayer.FastForward();
                                break;
                            }
                    }
                }
            }

            public void OnDestroy()
            {
                if (tapeStream != null)
                {
                    tapeStream.Release();
                    tapeStream = null;

                    ReceiverEvents.StopListening(ReceiverEventTypeVoid.OnGamePaused, new UnityAction<ReceiverEventTypeVoid>(this.OnPauseEvent));
                    ReceiverEvents.StopListening(ReceiverEventTypeVoid.OnGameResumed, new UnityAction<ReceiverEventTypeVoid>(this.OnPauseEvent));
                }
            }

            public bool TryGetSubtitleManager(out SubtitleManager subtitle_manager)
            {
                subtitle_manager = null;
				if (ReceiverCoreScript.TryGetInstance(out var receiverCoreScript))
				{
					SimpleTapePlayer tape_player = receiverCoreScript.tape_player;
					subtitle_manager = ((tape_player != null) ? tape_player.subtitle_manager : null);
				}
				return subtitle_manager != null;
            }
        }
    }
}
