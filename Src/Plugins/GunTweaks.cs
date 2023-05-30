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
using System.Reflection.Emit;

namespace CiarenceUnbelievableModifications
{
    public static class GunTweaks
    {
        public static void PatchDeaglesSpring()
        {
            var deagles = (from e in ReceiverCoreScript.Instance().gun_prefabs where e.GetComponent<GunScript>().weapon_group_name == "desert_eagle" select e);
            foreach (GameObject deagle in deagles)
            {
                //reflection-fest
                if (deagle != null)
                {
                    Transform mainspring = deagle.transform.Find("main_spring");
                    if (mainspring != null)
                    {
                        var mainspring_rot = mainspring.localRotation;
                        mainspring_rot.eulerAngles = new Vector3(5f, 0f, 20f);
                        mainspring.localRotation = mainspring_rot;
                        /*Quaternion rot_orig = (Quaternion)typeof(SpringCompressInstance).GetField("rot_orig", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(mainspring);
                        rot_orig.eulerAngles = new Vector3(5f, 0, 20f);
                        Debug.LogFormat("Patched {0}", deagle.GetComponent<GunScript>().InternalName);*/
                    }
                    else
                    {
                        Debug.LogError("hey, guess what, the mainspring is null.... oops");
                    }
                }
                else
                {
                    Debug.LogError("fucking deagle is null, can you believe it? fucking fuck.");
                }
            }
        }

        [HarmonyPatch(typeof(RuntimeTileLevelGenerator),
            nameof(RuntimeTileLevelGenerator.instance.InstantiateMagazine),
            new[] { typeof(Vector3), typeof(Quaternion), typeof(Transform), typeof(MagazineClass) }
            )]
        public static class InstantiateMagazineTranspiler
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase __originalMethod)
            {
                CodeMatcher codeMatcher = new CodeMatcher(instructions, generator).MatchForward(false, new CodeMatch(OpCodes.Call));

                LocalBuilder gun = generator.DeclareLocal(typeof(GunScript));
                LocalBuilder magazinePrefab = generator.DeclareLocal(typeof(MagazineScript));

                if (!codeMatcher.ReportFailure(__originalMethod, Debug.LogError))
                {
                    codeMatcher
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Nop))

                        .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ReceiverCoreScript), nameof(ReceiverCoreScript.Instance))))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Stloc_0))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ReceiverCoreScript), nameof(ReceiverCoreScript.player))))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ReceiverCoreScript.LocalPlayerInstance), nameof(ReceiverCoreScript.player.lah))))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloca_S, gun))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(LocalAimHandler), nameof(LocalAimHandler.TryGetGun))))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Pop))

                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_2))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(GunScript), nameof(GunScript.magazine_root_types))))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_0))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_2))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(GunScript), nameof(GunScript.magazine_root_types))))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldlen))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Conv_I4))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_1))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UnityEngine.Random), nameof(UnityEngine.Random.Range), new[] {typeof(Int32), typeof(Int32)})))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldelem_Ref))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_3))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloca_S, magazinePrefab))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(ReceiverCoreScript), nameof(ReceiverCoreScript.TryGetMagazinePrefabFromRoot))))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Pop))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(RuntimeTileLevelGenerator), nameof(RuntimeTileLevelGenerator.instance))))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_2))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_1))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(RuntimeTileLevelGenerator), nameof(RuntimeTileLevelGenerator.InstantiateMagazine), new[] { typeof(Vector3), typeof(Quaternion), typeof(Transform), typeof(MagazineScript) })))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Stloc_3))
                        .InsertBranchAndAdvance(OpCodes.Br_S, codeMatcher.Pos)
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_3))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ret))
                        ;
                }

                return codeMatcher.InstructionEnumeration();
            }
        }
    }
}
