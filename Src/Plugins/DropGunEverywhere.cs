using Receiver2;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CiarenceUnbelievableModifications
{
    internal static class DropGunEverywhere
    {
        private static LocalAimHandler lah;
        public static float time_to_drop = 1f;
        public static void Enable()
        {
            if (LocalAimHandler.TryGetInstance(out lah))
            {
                lah.allow_gun_drop = true;
            }
        }

        //I FUCKING LOVE TRANSPILERS
        [HarmonyPatch(typeof(LocalAimHandler), "Update")]
        public static class DropButtonTimeOffsetTranspiler
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase __originalMethod)
            {
                //allow_gun_drop is only present once in the whole Update method, so we'll only look for that
                CodeMatcher codeMatcher = new CodeMatcher(instructions, generator).MatchForward(false, new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(LocalAimHandler), "allow_gun_drop")));

                if (!codeMatcher.ReportFailure(__originalMethod, Debug.LogError))
                {
                    codeMatcher
                        //there's 6 instructions after it that we don't care about, so we'll skip them
                        .Advance(6)

                        //since the field that we're trying to replace the thing with is a static, we only have to set Ldsfld
                        .SetInstruction(new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(DropGunEverywhere), nameof(time_to_drop))));
                }

                return codeMatcher.InstructionEnumeration();
            }
        }

        public static void Disable()
        {
            LevelObject level_object = (LevelObject)typeof(ReceiverCoreScript).GetField("level_object", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ReceiverCoreScript.Instance());
            lah.allow_gun_drop = level_object.allow_gun_drop;
        }
    }
}