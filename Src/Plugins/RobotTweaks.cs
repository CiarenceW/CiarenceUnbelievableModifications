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
using Receiver2ModdingKit.ModInstaller;

namespace CiarenceUnbelievableModifications
{
    public static class RobotTweaks
    {
        public static Color colour_normal;
        public static Color colour_alert;
        public static Color colour_alert_shooting;

        private static float colour_a = 0f;
        private static float colour_b = 0.5f;
        private static float colour_c = 1f;

        private static float random_a;
        private static float random_b;
        private static float random_c;

        public static int disco_timescale;

        [HarmonyPatch(typeof(ReceiverCoreScript), "Awake")]
        [HarmonyPostfix]
        private static void PatchCoreAwake(ref ReceiverCoreScript __instance)
        {
            var bomb_bot = __instance.enemy_prefabs.bomb_bot.GetComponent<BombBotScript>();

            //this doesn't currently do anything, I think it used to talk and stuff and it was cool but it doesn't anymore. It just beeps. Capitalism. Also it used to soft-crash (is that a thing)
            bomb_bot.voice_filter = "event:/TextToSpeech/TextToSpeech - bomb bot";
        }

        //bear with me here but, wouldn't it be crazy if there were actual fucking tutorials for this sort of things?
        //like, AFAIK, people who use Harmony have all learned from looking at what other people have done
        //sure the ONE tutorial on the wiki helps a bit, but if you want to do something that isn't EXACTLY what the tutorial shows, you're fucked
        //anyways thanks Szikaka for being born with the knowledge of the gods or whatever
        //Inspired by https://www.youtube.com/watch?v=welzVVJD4ok, by Iwan :)
        [HarmonyPatch(typeof(TurretScript), "UpdateLight")]
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

        public static void Discolights()
        {
            if (colour_a == random_a) random_a = Random.Range(0f, 1f);
            if (colour_b == random_b) random_b = Random.Range(0f, 1f);
            if (colour_c == random_c) random_c = Random.Range(0f, 1f);

            //if you set disco_timescale to 0 it changes lights every frame, also how the fuck? it's a division by zero how does it work
            colour_a = Mathf.MoveTowards(colour_a, random_a, Time.deltaTime / disco_timescale);
            colour_b = Mathf.MoveTowards(colour_b, random_b, Time.deltaTime / disco_timescale); 
            colour_c = Mathf.MoveTowards(colour_c, random_c, Time.deltaTime / disco_timescale);

            colour_normal = new Color(colour_a, colour_b, colour_c);
        }
    }
}
