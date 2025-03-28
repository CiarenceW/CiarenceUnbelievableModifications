using HarmonyLib;
using Receiver2;
using UnityEngine;
using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TMPro;

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

        [HarmonyPatch(typeof(LocalAimHandler), nameof(LocalAimHandler.SetCharacterInput))]
        [HarmonyPostfix]
        private static void AddTapePlayerHolsterKey(LocalAimHandler __instance, CharacterInput ch, int player_index)
        {
            var keyboardMap = (from e in ReInput.players.GetPlayer(player_index).controllers.maps.GetAllMaps(ControllerType.Keyboard) where e.id == 10 select e).First();

            if (keyboardMap.ButtonMapsWithAction(60).Count() == 0)
            {
                keyboardMap.CreateElementMap(60, Pole.Positive, KeyCode.J, ModifierKeyFlags.None);
            }

            var tapePlayerMap = (from e in ReInput.players.GetPlayer(player_index).controllers.maps.GetAllMaps(ControllerType.Keyboard) where e.id == 13 select e).First();

            if (tapePlayerMap.ButtonMapsWithAction(58).Count() == 0)
            {
                tapePlayerMap.CreateElementMap(58, Pole.Positive, KeyCode.W, ModifierKeyFlags.None);
            }
        }

        [HarmonyPatch(typeof(LocalAimHandler), "InitWeaponSlot")]
        [HarmonyPrefix]
        private static void ChangeTapePlayerSlotPosition(LocalAimHandler __instance, LocalAimHandler.WeaponSlot slot, int slot_index)
        {
            var slotCheck = (from e in PlayerGUI.Instance.inventory_slots where e.name == "TapePlayerSlot" select e);

            if (__instance.simplified_tape_player)
            {
                if (slotCheck.Count() != 0) slotCheck.First().gameObject.SetActive(false);
                return;
            }
            if (slot_index != 6) return;

            var tapePlayer = __instance.GetItemInInventorySlot(6);

            if (slotCheck.Count() == 0)
            {
                var tapePlayerSlot = Component.Instantiate(PlayerGUI.Instance.holster_slot, PlayerGUI.Instance.holster_slot.parent);

                tapePlayerSlot.SetSiblingIndex(0);

                tapePlayerSlot.name = "TapePlayerSlot";

                var textTMPThing = tapePlayerSlot.Find("GUI Line Text/Layout Group/Text");

                if (textTMPThing.TryGetComponent<LocalizedTextMesh>(out var KILL))
                {
                    KILL.SetLocaleUIString(LocaleUIString.M_CBM_TAPE_PLAYER);
                }

                PlayerGUI.Instance.inventory_slots.Add(tapePlayerSlot);

				updateWeaponSlotTransformRef.Invoke(__instance, slot, slot_index);
			}
        }

		private static UpdateWeaponSlotTransformDelegate updateWeaponSlotTransformRef = AccessTools.MethodDelegate<UpdateWeaponSlotTransformDelegate>(AccessTools.Method(typeof(LocalAimHandler), "UpdateWeaponSlotTransform"));
		private delegate void UpdateWeaponSlotTransformDelegate(LocalAimHandler instance, LocalAimHandler.WeaponSlot slot, int slot_index);

        [HarmonyPatch(typeof(LocalAimHandler), "InitWeaponSlot")]
        [HarmonyPostfix]
        private static void MakeBackgroundRotCooler(LocalAimHandler __instance, LocalAimHandler.WeaponSlot slot, int slot_index)
        {
			if (slot_index == 6)
			{
				slot.background.transform.localEulerAngles = new Vector3(23.3928f, 24.0709f, 51.6015f);

				updateWeaponSlotTransformRef.Invoke(__instance, slot, slot_index);

				PlayerGUI.Instance.inventory_slots[slot_index].gameObject.SetActive(true);
			}
        }
    }
}
