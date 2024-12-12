using FMOD.Studio;
using FMODUnity;
using HarmonyLib;
using Receiver2;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace CiarenceUnbelievableModifications
{
    internal class TapePlayerScriptTweaks
    {
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
            private static FieldInfo fmod_tape_contentFieldInfo;

            private static FieldInfo old_wear_valueFieldInfo;

            private static AccessTools.FieldRef<FMODTapeFileStream, FMOD.Sound> soundRef;

            [HarmonyPatch(typeof(TapePlayerScript), "LoadAudio")]
            [HarmonyPostfix]
            private static void LoadAudioPathtape(ref TapePlayerScript __instance)
            {
                if (Misc.TapeIsValid(__instance) && !Misc.HasRecording(__instance) && Misc.HasAudioFilePath(__instance))
                {
                    if (fmod_tape_contentFieldInfo == null) fmod_tape_contentFieldInfo = AccessTools.Field(typeof(TapePlayerScript), "fmod_tape_content");
                    var fmod_tape_content = (EventInstance)fmod_tape_contentFieldInfo.GetValue(__instance);

                    var extender = __instance.GetOrAddComponent<TapePlayerScriptExtender>();
                    extender.tapeStream = new FMODTapeFileStream();
                    fmod_tape_contentFieldInfo.SetValue(__instance, extender.tapeStream.LoadTapeContent(__instance.tape_instance_in_player.content));
                    float currentWear = __instance.tape_instance_in_player.content.GetCurrentWear();
                    fmod_tape_content.setParameterByName("DamagedMedia", currentWear, false);
                    if (old_wear_valueFieldInfo == null) old_wear_valueFieldInfo = AccessTools.Field(typeof(TapePlayerScript), "old_wear_value");
                    old_wear_valueFieldInfo.SetValue(__instance, currentWear);
                    if (soundRef == null) soundRef = AccessTools.FieldRefAccess<FMODTapeFileStream, FMOD.Sound>("sound");
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

            private static FieldInfo currentTapePlayerScriptFieldInfo;

            [HarmonyPatch(typeof(PlayerEquipment), nameof(PlayerEquipment.PutInSlot))]
            [HarmonyPostfix]
            private static void AssignMissingTapePlayerFields(InventoryItem item)
            {
                if (item is TapePlayerScript tapePlayer)
                {
                    if (currentTapePlayerScriptFieldInfo == null) currentTapePlayerScriptFieldInfo = AccessTools.Field(typeof(LocalAimHandler), "current_tape_player_script");

                    currentTapePlayerScriptFieldInfo.SetValue(LocalAimHandler.player_instance, tapePlayer);

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
            [HarmonyPatch(typeof(TapePlayerScript), nameof(TapePlayerScript.Stop))]
            [HarmonyPrefix] 
            private static bool CheckForGamePlayTape(TapePlayerScript __instance)
            {
                return __instance.tape_instance_in_player != null && __instance.tape_instance_in_player.content.stoppable;
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

            [HarmonyPatch(typeof(TapePlayerScript), nameof(TapePlayerScript.Stop))]
            [HarmonyPostfix]
            private static void StopTapeSubtitles(TapePlayerScript __instance)
            {
                if (__instance.TryGetComponent<TapePlayerScriptExtender>(out var extender))
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
        }

        internal static class Misc
        {
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
                ReceiverCoreScript receiverCoreScript;
                if (ReceiverCoreScript.TryGetInstance(out receiverCoreScript))
                {
                    SimpleTapePlayer tape_player = receiverCoreScript.tape_player;
                    subtitle_manager = ((tape_player != null) ? tape_player.subtitle_manager : null);
                }
                return subtitle_manager != null;
            }
        }
    }
}
