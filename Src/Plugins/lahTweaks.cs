using HarmonyLib;
using Receiver2;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiarenceUnbelievableModifications
{
    internal static class lahTweaks //"naming rule violation" idc!!!!!!!!!!!!!!!!!!!!!!
    {
        [HarmonyPatch(typeof(LocalAimHandler), nameof(LocalAimHandler.IsTerminalFall), new[] {typeof(Vector3)})]
        [HarmonyPrefix]
        public static bool PatchIsTerminalFall(ref bool __result)
        {
            if (ConfigFiles.global.god)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
