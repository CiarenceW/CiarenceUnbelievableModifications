using HarmonyLib;
using Receiver2;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using VLB;
using UnityEditor.PackageManager;
using UnityEngine.Rendering.PostProcessing;
using System.Threading.Tasks;

namespace CiarenceUnbelievableModifications
{
    public static class RobotTweaks
    {
        public static Color tripmine_beam_colour;
        public static Color tripmine_beam_colour_triggered;

        public static Color tripmine_beam_colour_normal;
        public static Color tripmine_beam_colour_triggered_normal;

        public static Color colour_normal;
        public static Color colour_alert;
        public static Color colour_alert_shooting;

        public static Color colour_idle_turret;
        public static Color colour_alert_turret;
        public static Color colour_attacking_turret;

        public static Color colour_idle_drone;
        public static Color colour_alert_drone;
        public static Color colour_attacking_drone;

        public static Color colour_idle_camera;
        public static Color colour_alert_camera;
        public static Color colour_alarming_camera;

        public static Color tc_tripmine_beam_colour = Color.blue;
        public static Color tc_tripmine_beam_colour_triggered = Color.red;

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

        private static FieldInfo current_light_mode;

        private static FieldInfo part;

        private static FieldInfo light_color;

        private static MethodInfo SetLightMode;

        internal static void SetUpLightPartFieldReflections()
        {
            current_light_mode = typeof(LightPart).GetField("current_light_mode", BindingFlags.Instance | BindingFlags.NonPublic);

            part = typeof(LightPart).GetField("part", BindingFlags.Instance | BindingFlags.NonPublic);

            light_color = typeof(LightPart).GetField("light_color", BindingFlags.Instance | BindingFlags.NonPublic);

            SetLightMode = typeof(LightPart).GetMethod("SetLightMode", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static void PatchBombBotPrefab()
        {
            var bomb_bot = ReceiverCoreScript.Instance().enemy_prefabs.bomb_bot.GetComponent<BombBotScript>();

            //this doesn't currently do anything, I think it used to talk and stuff and it was cool but it doesn't anymore. It just beeps. Capitalism.
            //Also, if you don't change it to a valid event like the one below, it used to soft-crash (is that a thing)
            bomb_bot.voice_filter = "event:/TextToSpeech/TextToSpeech - bomb bot";

            //AccessTools.Field(typeof(LightPart), "passive_color").SetValue(__instance.enemy_prefabs.shock_drone.GetComponent<ShockDrone>().light_part, new Color(1f, 0f, 1f, 1f));
            //AccessTools.Field(typeof(LightPart), "light_color").SetValue(__instance.enemy_prefabs.shock_drone.GetComponent<ShockDrone>().light_part, new Color(1f, 0f, 1f, 1f));
        }

        internal static void PatchPowerLeechPrefab()
        {
            var power_leech = ReceiverCoreScript.Instance().enemy_prefabs.power_leech_bot.GetComponent<PowerLeechBot>();

            power_leech.occlusion_component = ReceiverCoreScript.Instance().enemy_prefabs.turret.GetComponent<TurretScript>().occlusion_component;
        }

        internal static void OnChangeKilldroneLightColour()
        {
            UpdateColourPartLight();
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
                        //.CreateLabelAt(0, out Label not_triggered_label)

                        //sets the tripmine colour to its idle colour
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(TripMineBot), "light_beam")))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(RobotTweaks), "tripmine_beam_colour")))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(VolumetricLightBeam), "color")))

                        //is the tripmine triggered? if not, skip next block
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(TripMineBot), "triggered")))
                        //label if tripmine is not triggered
                        .InsertBranchAndAdvance(OpCodes.Brfalse_S, codeMatcher.Pos)

                        //if tripmine is triggered, sets colour to triggered colour
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(TripMineBot), "light_beam")))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(RobotTweaks), "tripmine_beam_colour_triggered")))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(VolumetricLightBeam), "color")))

                        .Advance(2) //Advancing twice to place this after the first instruction, needed for the label for the laser to update.

                        //Volumetric Light Beam needs to have this method called to update the colour. the base method for the tripmine actually calls it, but only if the tripmine is triggered and not incapacitated
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(TripMineBot), "light_beam")))
                        .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(VolumetricLightBeam), nameof(VolumetricLightBeam.UpdateAfterManualPropertyChange))));

                }
                return codeMatcher.InstructionEnumeration();
            }
        }

        #region LightPartPatches

        [HarmonyPatch(typeof(LightPart), "SetLightMode")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> TranspileSetLightMode(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase __originalMethod)
        {
            CodeMatcher codeMatcher = new CodeMatcher(instructions, generator).MatchForward(true, new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(LightPart), "passive_color")));
            
            if (!codeMatcher.ReportFailure(__originalMethod, Debug.LogError))
            {
                codeMatcher.SetOperandAndAdvance(AccessTools.Field(typeof(LightPart), "part"));
                codeMatcher.Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RobotTweaks), nameof(GetPassiveColour))));
            }

            codeMatcher.MatchForward(true, new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(LightPart), "alert_color")));

            if (!codeMatcher.ReportFailure(__originalMethod, Debug.LogError))
            {
                codeMatcher.SetOperandAndAdvance(AccessTools.Field(typeof(LightPart), "part"));
                codeMatcher.Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RobotTweaks), nameof(GetAlertColour))));
            }

            codeMatcher.MatchForward(true, new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(LightPart), "aggressive_color")));

            if (!codeMatcher.ReportFailure(__originalMethod, Debug.LogError))
            {
                codeMatcher.SetOperandAndAdvance(AccessTools.Field(typeof(LightPart), "part"));
                codeMatcher.Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RobotTweaks), nameof(GetAggressiveColour))));
            }

            codeMatcher.MatchForward(true, new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(LightPart), "passive_color")));

            if (!codeMatcher.ReportFailure(__originalMethod, Debug.LogError))
            {
                codeMatcher.SetOperandAndAdvance(AccessTools.Field(typeof(LightPart), "part"));
                codeMatcher.Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RobotTweaks), nameof(GetPassiveColour))));
            }

            return codeMatcher.InstructionEnumeration();
        }

        //needed to update the light's colour when first spawned in, other is blue until light mode change
        [HarmonyPatch(typeof(LightPart), "Awake")]
        [HarmonyPostfix]
        private static void PatchLightPartAwake(LightPart __instance)
        {
            /*
            var passive_colour = GetPassiveColour(__instance.part);
            UpdateColourPartLight(__instance);
            light_color.SetValue(__instance, passive_colour);
            __instance.spot_light.color = passive_colour;
            __instance.beam.color = passive_colour;
            if (verbose) Debug.Log(__instance.spot_light.color);
            if (verbose) Debug.Log(__instance.light_color);
            */
        }

        [HarmonyPatch(typeof(SecurityCamera), "Start")]
        [HarmonyPrefix]
        private static void PatchSecurityCameraStart(ref SecurityCamera __instance)
        {
            light_color.SetValue(__instance.light_part, colour_idle_camera);
        }

        [HarmonyPatch(typeof(ShockDrone), "Start")]
        [HarmonyPrefix]
        private static void PatchShockDroneStart(ref ShockDrone __instance)
        {
            light_color.SetValue(__instance.light_part, colour_idle_drone);
        }

        internal static void UpdateColourPartLight(LightPart instance = null)
        {
            if (instance != null)
            {
                SetLightMode.Invoke(instance, new object[] { current_light_mode.GetValue(instance) });
                return;
            }

            var lightParts = Component.FindObjectsOfType<LightPart>();
            for (int i = 0; i < lightParts.Length; i++)
            {
                SetLightMode.Invoke(lightParts[i], new object[] { current_light_mode.GetValue(lightParts[i]) });
            }
        }

        [HarmonyPatch(typeof(LightPart), "UpdateLightMode")]
        [HarmonyPrefix]
        private static void PatchLightPartUpdate(ref LightPart __instance)
        {
            if (SettingsManager.configEnableTurretDiscoLights.Value == true && (!campaign_has_override || campaign_has_override && SettingsManager.configKilldroneColourOverride.Value == false))
                UpdateColourPartLight(__instance);
        }

        internal static void UpdateLightPartDefaultColour(Color lightColour, ReceiverEntityType entityType = ReceiverEntityType.Unknown)
        {
            var enemy_prefabs = ReceiverCoreScript.Instance().enemy_prefabs;
            if (entityType == ReceiverEntityType.Unknown)
            {
                light_color.SetValue(enemy_prefabs.shock_drone.GetComponent<ShockDrone>().light_part, lightColour);
                light_color.SetValue(enemy_prefabs.security_camera.GetComponent<SecurityCamera>().light_part, lightColour);
                return;
            }

            var prefab = enemy_prefabs.GetPrefab(entityType);
            if (entityType == ReceiverEntityType.Drone) light_color.SetValue(prefab.GetComponent<ShockDrone>().light_part, lightColour);
            if (entityType == ReceiverEntityType.SecurityCamera)light_color.SetValue(prefab.GetComponent<SecurityCamera>().light_part, lightColour);
        }

        public static Color GetPassiveColour(RobotPart part)
        {
            if (campaign_has_override && SettingsManager.configKilldroneColourOverride.Value == true) 
                return colour_override_idle;

            if (part.robot is ShockDrone)
                return colour_idle_drone;

            return colour_idle_camera;
        }

        public static Color GetAlertColour(RobotPart part)
        {
            if (campaign_has_override && SettingsManager.configKilldroneColourOverride.Value == true)
                return colour_override_alert;

            if (part.robot is ShockDrone)
                return colour_alert_drone;

            return colour_alert_camera;
        }

        public static Color GetAggressiveColour(RobotPart part)
        {
            if (campaign_has_override && SettingsManager.configKilldroneColourOverride.Value == true)
                return colour_override_attack;

            if (part.robot is ShockDrone)
                return colour_attacking_drone;

            return colour_alarming_camera;
        }

        #endregion

        //for the turrets' camera colours when in TC, you can't change from which field the thing is read during runtime I think with Transpilers.
        public static void OnPlayerInitialize(ReceiverEventTypeVoid ev)
        {
            if (ReceiverCoreScript.Instance().game_mode.GetGameMode() != GameMode.RankingCampaign) return;

            FieldInfo campaign_name = typeof(RankingProgressionGameMode).GetField("campaign_name", BindingFlags.Instance | BindingFlags.NonPublic);
            RankingProgressionGameMode rpgm = ReceiverCoreScript.Instance().game_mode.GetComponent<RankingProgressionGameMode>();
            if (verbose) Debug.Log(campaign_name.GetValue(rpgm));
            if ((string)campaign_name.GetValue(rpgm) != "bozo_torture_campaign")
            {
                campaign_has_override = false;
                colour_normal = colour_idle_turret;
                colour_alert = colour_alert_turret;
                colour_alert_shooting = colour_attacking_turret;
                tripmine_beam_colour = tripmine_beam_colour_normal;
                if (verbose) Debug.LogFormat("{0} <- {1}", tripmine_beam_colour, tripmine_beam_colour_normal);
                tripmine_beam_colour_triggered = tripmine_beam_colour_triggered_normal;
                if (verbose) Debug.LogFormat("{0} <- {1}", tripmine_beam_colour_triggered, tripmine_beam_colour_triggered_normal);
                return;
            }
            campaign_has_override = true;
            SetOverrideColours(true);
            if (verbose) Debug.Log("player is bozo");
        }

        internal static void SetOverrideColours(bool enable)
        {
            if (enable && campaign_has_override && SettingsManager.configKilldroneColourOverride.Value == true)
            {
                colour_override_idle = tc_colour_idle;
                colour_override_alert = tc_colour_alert;
                colour_override_attack = tc_colour_attack;
                colour_normal = colour_override_idle;
                colour_alert = colour_override_alert;
                colour_alert_shooting = colour_override_attack;
                tripmine_beam_colour = tc_tripmine_beam_colour;
                tripmine_beam_colour_triggered = tc_tripmine_beam_colour_triggered;
                UpdateLightPartDefaultColour(colour_override_idle);
            }
            else
            {
                colour_normal = colour_idle_turret;
                colour_alert = colour_alert_turret;
                colour_alert_shooting = colour_attacking_turret;
                tripmine_beam_colour = tripmine_beam_colour_normal;
                tripmine_beam_colour_triggered = tripmine_beam_colour_triggered_normal;
                UpdateLightPartDefaultColour(SettingsManager.configDroneColourIdle.Value, ReceiverEntityType.Drone);
                UpdateLightPartDefaultColour(SettingsManager.configCameraColourIdle.Value, ReceiverEntityType.SecurityCamera);
            }
            UpdateColourPartLight();
        }

        public static void Discolights()
        {
            if (!campaign_has_override || campaign_has_override && SettingsManager.configKilldroneColourOverride.Value == false)
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
                UpdateLightPartDefaultColour(rainbow_colour);
            }
        }
    }
}