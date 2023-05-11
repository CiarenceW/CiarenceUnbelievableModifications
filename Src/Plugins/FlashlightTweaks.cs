using BepInEx;
using Receiver2;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace CiarenceUnbelievableModifications
{
    public static class FlashlightTweaks
    {
        private static LocalAimHandler lah;
        private static float drop_button_released_time;
        private static float battery_life_remaining = float.MaxValue;
        private static float max_battery_life = 19800f;
        private static VLB.VolumetricLightBeam vlb;
        public static bool verbose;

        public static void UpdateFlashlight(KeyCode toggleKey)
        {
            if (LocalAimHandler.TryGetInstance(out lah))
            {
                if (lah.IsHoldingFlashlight)
                {
                    if (battery_life_remaining == float.MaxValue)
                    {
                        battery_life_remaining = (float)typeof(FlashlightScript).GetField("battery_life_remaining", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(LocalAimHandler.Flashlight);
                    }
                    if (Input.GetKeyDown(toggleKey))
                    {
                        if(verbose) Debug.LogFormat("{0} pressed", toggleKey);
                        LocalAimHandler.Flashlight.TogglePower();
                    }
                    vlb = LocalAimHandler.Flashlight.spot_light.transform.GetComponent<VLB.VolumetricLightBeam>();
                    vlb.colorFromLight = false;
                    vlb.color = new Color(vlb.color.r, vlb.color.g, vlb.color.b, 1 * (battery_life_remaining / max_battery_life));
                    if (!lah.IsHoldingGun)
                    {
                        if (lah.character_input.GetButtonDown(RewiredConsts.Action.Eject_Drop_Magazine)) drop_button_released_time = Time.time + 0.5f;

                        if (lah.character_input.GetButtonUp(RewiredConsts.Action.Eject_Drop_Magazine)) drop_button_released_time = float.MaxValue;

                        if (Time.time >= drop_button_released_time && drop_button_released_time != float.MaxValue && lah.character_input.GetButton(RewiredConsts.Action.Eject_Drop_Magazine))
                        {
                            int handWithState2 = lah.GetHandWithState(LocalAimHandler.Hand.State.HoldingFlashlight);
                            lah.MoveInventoryItem(lah.hands[handWithState2].slot.contents[0], null);
                            drop_button_released_time = float.MaxValue;
                        }
                    }
                }
            }
        }

    }
}
