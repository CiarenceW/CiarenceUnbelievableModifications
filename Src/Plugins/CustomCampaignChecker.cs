using Receiver2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CiarenceUnbelievableModifications
{
    internal static class CustomCampaignChecker
    {
        public static bool ShouldOverrideColour
        {
            get { return (campaign_has_override && SettingsManager.configKilldroneColourOverride.Value == true); }
        }

        internal static bool campaign_has_override;

        //for the turrets' camera colours when in TC, you can't change from which field the thing is read during runtime I think with Transpilers.
        public static void OnPlayerInitialize(ReceiverEventTypeVoid ev)
        {
            if (ReceiverCoreScript.Instance().game_mode.GetGameMode() != GameMode.RankingCampaign) return;

            FieldInfo campaign_name = typeof(RankingProgressionGameMode).GetField("campaign_name", BindingFlags.Instance | BindingFlags.NonPublic);
            RankingProgressionGameMode rpgm = ReceiverCoreScript.Instance().game_mode.GetComponent<RankingProgressionGameMode>();
            if (SettingsManager.Verbose) Debug.Log(campaign_name.GetValue(rpgm));
            if ((string)campaign_name.GetValue(rpgm) != "bozo_torture_campaign")
            {
                campaign_has_override = false;
                RobotTweaks.SetOverrideColours(false);
                return;
            }
            campaign_has_override = true;
            RobotTweaks.SetOverrideColours(true);
            if (SettingsManager.Verbose) Debug.Log("player is bozo");
        }
    }
}
