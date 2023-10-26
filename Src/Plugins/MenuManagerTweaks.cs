using Receiver2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ImGuiNET;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;

namespace CiarenceUnbelievableModifications
{
    internal static class MenuManagerTweaks
    {
        internal static void TriggerEventLoadLevelSections()
        {
            if (ImGui.BeginMenu("Trigger event"))
            {
                foreach (object obj3 in Enum.GetValues(typeof(ReceiverEventType)))
                {
                    ReceiverEventType receiverEventType = (ReceiverEventType)obj3;
                    if (ImGui.MenuItem(receiverEventType.ToString()))
                    {
                        ReceiverEventManager.Instance.TriggerEvent(receiverEventType);
                    }
                }
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Load Level"))
            {
                string[] array5 = LevelManagerScript.Instance.level_list.GetSceneNames().ToArray();
                for (int l = 0; l < array5.Length; l++)
                {
                    if (ImGui.MenuItem(array5[l]))
                    {
                        ReceiverCoreScript.Instance().StartGameMode(GameMode.TestDrive);
                        ReceiverCoreScript.Instance().FadeAndLoad(LevelManagerScript.Instance.level_list.GetIDs()[l]);
                        ReceiverCoreScript.Instance().menu_manager.CloseMenu();
                    }
                }
                ImGui.EndMenu();
            }
        }
        [HarmonyPatch(typeof(MenuManagerScript), "UpdateDeveloperMenu")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> UpdateDeveloperMenuTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase __originalMethod)
        {
            CodeMatcher codeMatcher = new CodeMatcher(instructions, generator).MatchForward(false, new CodeMatch(OpCodes.Ldstr, "Special Events"));

            if (!codeMatcher.ReportFailure(__originalMethod, Debug.LogError))
            {
                codeMatcher
                    .SetAndAdvance(OpCodes.Call, AccessTools.Method(typeof(MenuManagerTweaks), nameof(TriggerEventLoadLevelSections)))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldstr, "Special Events"));
            }

            return codeMatcher.InstructionEnumeration();
        }
    }
}
