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

        public static float time_to_drop;

        public static Color flashlight_color;

        private static float colour_a = 0f;
        private static float colour_b = 0.5f;
        private static float colour_c = 1f;

        private static float random_a;
        private static float random_b;
        private static float random_c;

        public static int disco_timescale;

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
                        if (verbose) Debug.LogFormat("{0} pressed", toggleKey);
                        LocalAimHandler.Flashlight.TogglePower();
                    }
                    vlb = LocalAimHandler.Flashlight.spot_light.transform.GetComponent<VLB.VolumetricLightBeam>();
                    vlb.colorFromLight = false;
                    vlb.color = new Color(vlb.color.r, vlb.color.g, vlb.color.b, 1 * (battery_life_remaining / max_battery_life));
                    if (!lah.IsHoldingGun)
                    {
                        if (lah.character_input.GetButtonDown(RewiredConsts.Action.Eject_Drop_Magazine)) drop_button_released_time = Time.time + time_to_drop;

                        if (lah.character_input.GetButtonUp(RewiredConsts.Action.Eject_Drop_Magazine)) drop_button_released_time = float.MaxValue;

                        if (Time.time >= drop_button_released_time && drop_button_released_time != float.MaxValue && lah.character_input.GetButton(RewiredConsts.Action.Eject_Drop_Magazine))
                        {
                            int handWithState2 = lah.GetHandWithState(LocalAimHandler.Hand.State.HoldingFlashlight);
                            lah.MoveInventoryItem(lah.hands[handWithState2].slot.contents[0], null);
                            drop_button_released_time = float.MaxValue;
                        }
                    }
                    UpdateFlashlightColours();
                }
            }
        }

        public static void UpdateFlashlightColours()
        {
            if (LocalAimHandler.Flashlight != null)
            {
                LocalAimHandler.Flashlight.spot_light.color = flashlight_color;
                LocalAimHandler.Flashlight.point_light.color = flashlight_color;
                LocalAimHandler.Flashlight.point_light2.color = flashlight_color;
                LocalAimHandler.Flashlight.spot_light.transform.GetComponent<VLB.VolumetricLightBeam>().color = new Color(flashlight_color.r, flashlight_color.g, flashlight_color.b);
            }
        }
        public static void Discolights()
        {
            if (LocalAimHandler.Flashlight != null)
            {
                if (LocalAimHandler.Flashlight.switch_on)
                {
                    if (colour_a == random_a) random_a = Random.Range(0f, 1f);
                    if (colour_b == random_b) random_b = Random.Range(0f, 1f);
                    if (colour_c == random_c) random_c = Random.Range(0f, 1f);

                    //if you set disco_timescale to 0 it changes lights every frame, also how the fuck? it's a division by zero how does it work
                    colour_a = Mathf.MoveTowards(colour_a, random_a, Time.deltaTime / disco_timescale);
                    colour_b = Mathf.MoveTowards(colour_b, random_b, Time.deltaTime / disco_timescale);
                    colour_c = Mathf.MoveTowards(colour_c, random_c, Time.deltaTime / disco_timescale);

                    Color rainbow_colour = new Color(colour_a, colour_b, colour_c);

                    flashlight_color = rainbow_colour;

                    UpdateFlashlightColours();
                }
            }
        }
    }
}
