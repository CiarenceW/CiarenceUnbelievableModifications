using HarmonyLib;
using Receiver2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace CiarenceUnbelievableModifications
{
    internal class SpecialEventsTweaks
    {
        internal static class HalloweenTweaks
        {
            /*internal static bool IsHalloweenActive()
            {
                return SpecialEvent.forced_events.Contains(SpecialEvent.EventType.Halloween) || SpecialEventRange.HalloweenEventRange.IsToday();
            }*/

            [HarmonyPatch(typeof(Pumpkin), "Start")]
            [HarmonyPrefix]
            private static void PatchPumpkinAwake(ref Pumpkin __instance)
            {
                if (RobotTweaks.campaign_has_override) //awful, need to refactor ASAP
                {
                    __instance.intact_object.GetComponentInChildren<Light>().color = Color.red;
                    var pumpkinMat = __instance.intact_object.GetComponentInChildren<MeshRenderer>().material;
                    pumpkinMat.color = (Probability.Chance(0.2f)) ? Color.black : pumpkinMat.color;
                    pumpkinMat.SetColor("_EmissionColor", Color.red);
                }
            }
        }

/*        //for some fucking reason pretty much everything inside SpecialEvent is private, I can't even check if an event is currently active because of it
        internal class SpecialEventRange
        {
            public static readonly EventRange HalloweenEventRange = new EventRange
            {
                start_date = new EventDate(10, 18),
                end_date = new EventDate(11, 4)
            };

            // Token: 0x020004A6 RID: 1190
            public enum EventType
            {
                // Token: 0x04002014 RID: 8212
                Halloween
            }

            // Token: 0x020004A7 RID: 1191
            public class EventDate
            {
                // Token: 0x06001AEC RID: 6892 RVA: 0x0001689F File Offset: 0x00014A9F
                public EventDate(int month, int day)
                {
                    this.month = month;
                    this.day = day;
                }

                // Token: 0x06001AED RID: 6893 RVA: 0x000168B5 File Offset: 0x00014AB5
                public EventDate(DateTime date_time)
                {
                    this.month = date_time.Month;
                    this.day = date_time.Day;
                }

                // Token: 0x06001AEE RID: 6894 RVA: 0x000168D7 File Offset: 0x00014AD7
                public static bool operator >(EventDate a, EventDate b)
                {
                    return a.month > b.month || (a.month == b.month && a.day > b.day);
                }

                // Token: 0x06001AEF RID: 6895 RVA: 0x00016907 File Offset: 0x00014B07
                public static bool operator <(EventDate a, EventDate b)
                {
                    return a.month < b.month || (a.month == b.month && a.day < b.day);
                }

                // Token: 0x06001AF0 RID: 6896 RVA: 0x00016937 File Offset: 0x00014B37
                public static bool operator >=(EventDate a, EventDate b)
                {
                    return a.month > b.month || (a.month == b.month && a.day >= b.day);
                }

                // Token: 0x06001AF1 RID: 6897 RVA: 0x0001696A File Offset: 0x00014B6A
                public static bool operator <=(EventDate a, EventDate b)
                {
                    return a.month < b.month || (a.month == b.month && a.day <= b.day);
                }

                // Token: 0x04002015 RID: 8213
                public int month;

                // Token: 0x04002016 RID: 8214
                public int day;
            }

            // Token: 0x020004A8 RID: 1192
            public class EventRange
            {
                // Token: 0x06001AF2 RID: 6898 RVA: 0x000B1820 File Offset: 0x000AFA20
                public bool IsToday()
                {
                    EventDate eventDate = new EventDate(DateTime.Today);
                    if (this.end_date.month < this.start_date.month)
                    {
                        this.end_date.month += 12;
                    }
                    return this.start_date <= eventDate && this.end_date >= eventDate;
                }

                // Token: 0x04002017 RID: 8215
                public EventDate start_date;

                // Token: 0x04002018 RID: 8216
                public EventDate end_date;
            }
        }*/
    }
}
