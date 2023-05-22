using BepInEx;
using HarmonyLib;
using Receiver2;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using VLB;

namespace CiarenceUnbelievableModifications
{
    public static class RobotTweaks
    {
        public static Color colour_normal;
        public static Color colour_alert;
        public static Color colour_alert_shooting;

        public static Color tripmine_beam_colour;

        public static Color colour_idle_turret;
        public static Color colour_alert_turret;
        public static Color colour_attacking_turret;

        public static Color colour_idle_drone;
        public static Color colour_alert_drone;
        public static Color colour_attacking_drone;

        public static Color colour_idle_camera;
        public static Color colour_alert_camera;
        public static Color colour_alarming_camera;

        public readonly static Color tc_colour_idle = Color.red;
        public readonly static Color tc_colour_alert = Color.white;
        public readonly static Color tc_colour_attack = new Color(1f, 0.25f, 0f);

        public static Color colour_override_idle;
        public static Color colour_override_alert;
        public static Color colour_override_attack;

        private static float colour_a = 0f;
        private static float colour_b = 0.5f;
        private static float colour_c = 1f;

        private static float random_a;
        private static float random_b;
        private static float random_c;

        public static int disco_timescale;

        public static bool verbose;

        public static bool campaign_has_override;

        [HarmonyPatch(typeof(ReceiverCoreScript), "Awake")]
        [HarmonyPostfix]
        private static void PatchCoreAwake(ref ReceiverCoreScript __instance)
        {
            var bomb_bot = __instance.enemy_prefabs.bomb_bot.GetComponent<BombBotScript>();

            //this doesn't currently do anything, I think it used to talk and stuff and it was cool but it doesn't anymore. It just beeps. Capitalism. Also, if you don't change it to a valid event like the one below, it used to soft-crash (is that a thing)
            bomb_bot.voice_filter = "event:/TextToSpeech/TextToSpeech - bomb bot";

            //AccessTools.Field(typeof(LightPart), "passive_color").SetValue(__instance.enemy_prefabs.shock_drone.GetComponent<ShockDrone>().light_part, new Color(1f, 0f, 1f, 1f));
            //AccessTools.Field(typeof(LightPart), "light_color").SetValue(__instance.enemy_prefabs.shock_drone.GetComponent<ShockDrone>().light_part, new Color(1f, 0f, 1f, 1f));
        }

        //bear with me here but, wouldn't it be crazy if there were actual fucking tutorials for this sort of things?
        //like, AFAIK, people who use Harmony have all learned from looking at what other people have done
        //sure the ONE tutorial on the wiki helps a bit, but if you want to do something that isn't EXACTLY what the tutorial shows, you're fucked
        //anyways thanks Szikaka for being born with the knowledge of the gods or whatever
        //Inspired by https://www.youtube.com/watch?v=welzVVJD4ok, by Iwan :)
        [HarmonyPatch(typeof(TurretScript), "UpdateLight")]
        public static class TurretLightUpdateTranspiler
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase __originalMethod)
            {
                CodeMatcher codeMatcher = new CodeMatcher(instructions, generator).MatchForward(false, new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Light), "set_color")));

                if (!codeMatcher.ReportFailure(__originalMethod, Debug.LogError))
                {
                    codeMatcher
                        .SetAndAdvance(OpCodes.Ldfld, AccessTools.Field(typeof(RobotTweaks), "colour_normal"))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Light), "set_color", new System.Type[] { typeof(Color) })));
                }


                codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Light), "set_color")));

                if (!codeMatcher.ReportFailure(__originalMethod, Debug.LogError))
                {
                    codeMatcher
                        .SetAndAdvance(OpCodes.Ldfld, AccessTools.Field(typeof(RobotTweaks), "colour_alert_shooting"))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Light), "set_color", new System.Type[] { typeof(Color) })));
                }


                codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Light), "set_color")));

                if (!codeMatcher.ReportFailure(__originalMethod, Debug.LogError))
                {
                    codeMatcher
                        .SetAndAdvance(OpCodes.Ldfld, AccessTools.Field(typeof(RobotTweaks), "colour_alert"))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Light), "set_color", new System.Type[] { typeof(Color) })));
                }

                return codeMatcher.InstructionEnumeration();
            }
        }

        //I don't have a good experience with trying to change the colour of all instances of a light, so I'm using the tried and tested transpiler to do it class-level.
        [HarmonyPatch(typeof(TripMineBot), "Update")]
        public static class TripmineUpdateTranspiler
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase __originalMethod)
            {
                CodeMatcher codeMatcher = new CodeMatcher(instructions, generator).MatchForward(false, new CodeMatch(OpCodes.Ldsfld, AccessTools.Field(typeof(LocalAimHandler), "player_instance")));

                if (!codeMatcher.ReportFailure(__originalMethod, Debug.LogError))
                {
                    codeMatcher
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(TripMineBot), "light_beam")))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(RobotTweaks), "tripmine_beam_colour")))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(VolumetricLightBeam), "color")))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(TripMineBot), "light_beam")))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(VolumetricLightBeam), nameof(VolumetricLightBeam.UpdateAfterManualPropertyChange))));
                }
                return codeMatcher.InstructionEnumeration();
            }
        }

        [HarmonyPatch(typeof(LightPart), "Update")]
        [HarmonyPostfix]
        //can you tell from this that I'm going insane?
        private static void PatchLightPartUpdate(ref LightPart __instance)
        {
            var light_color = typeof(LightPart).GetField("light_color", BindingFlags.Instance | BindingFlags.NonPublic);
            var shockDroneScript = __instance.transform.parent.parent.GetComponent<ShockDrone>();
            if (shockDroneScript != null)
            {
                var state = (ShockDroneState)typeof(ShockDrone).GetField("state", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(shockDroneScript);

                if (verbose) Debug.LogFormat("State is {0}", state);
                switch (state)
                {
                    case ShockDroneState.Idle:
                        light_color.SetValue(__instance, colour_normal);
                        return;
                    case ShockDroneState.Alert:
                    case ShockDroneState.TrackPlayer:
                    case ShockDroneState.Ramming:
                        light_color.SetValue(__instance, colour_alert);
                        return;
                    case ShockDroneState.Attacking:
                        light_color.SetValue(__instance, colour_alert_shooting);
                        return;
                    case ShockDroneState.Standby:
                        var passive_colour = colour_normal;
                        passive_colour.a = 0f;
                        light_color.SetValue(__instance, passive_colour);
                        return;
                    default:
                        light_color.SetValue(__instance, colour_normal);
                        return;
                }
            }
            var securityCameraScript = __instance.transform.parent.parent.GetComponent<SecurityCamera>();
            if (securityCameraScript != null)
            {
                var state = (SecurityCameraState)typeof(SecurityCamera).GetField("state", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(securityCameraScript);

                if (verbose) Debug.LogFormat("State is {0}", state);
                switch (state)
                {
                    case SecurityCameraState.Idle:
                        light_color.SetValue(__instance, colour_normal);
                        return;
                    case SecurityCameraState.Alert:
                        light_color.SetValue(__instance, colour_alert);
                        return;
                    case SecurityCameraState.Alarm:
                        light_color.SetValue(__instance, colour_alert_shooting);
                        return;
                    case SecurityCameraState.Off:
                        var passive_colour = colour_normal;
                        passive_colour.a = 0f;
                        light_color.SetValue(__instance, passive_colour);
                        return;
                    default:
                        light_color.SetValue(__instance, colour_normal);
                        return;
                }
            }
            if (verbose) Debug.LogError("fucksguhuogsrgksgsfsgehlfse");
            //var light_colour = (Color)AccessTools.Field(typeof(LightPart), "light_color").GetValue(__instance);
            //light_colour = new Color(0f, 1f, 1f, 1f);
            //Debug.Log("THIS PISS OF SHIT DOESN'T FUKING WORKY!!!!!");
        }

        public static void Discolights()
        {
            if (!campaign_has_override)
            {
                if (colour_a == random_a) random_a = Random.Range(0f, 1f);
                if (colour_b == random_b) random_b = Random.Range(0f, 1f);
                if (colour_c == random_c) random_c = Random.Range(0f, 1f);

                //if you set disco_timescale to 0 it changes lights every frame, also how the fuck? it's a division by zero how does it work
                colour_a = Mathf.MoveTowards(colour_a, random_a, Time.deltaTime / disco_timescale);
                colour_b = Mathf.MoveTowards(colour_b, random_b, Time.deltaTime / disco_timescale);
                colour_c = Mathf.MoveTowards(colour_c, random_c, Time.deltaTime / disco_timescale);

                Color rainbow_colour = new Color(colour_a, colour_b, colour_c);

                colour_normal = rainbow_colour;
                colour_idle_drone = rainbow_colour;
                colour_idle_camera = rainbow_colour;
            }
        }
    }
}
