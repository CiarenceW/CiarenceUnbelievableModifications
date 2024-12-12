using HarmonyLib;
using Receiver2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using ImGuiNET;
using UnityEngine.Rendering.PostProcessing;
using Receiver2ModdingKit.Helpers;
using System.Runtime.CompilerServices;
using Receiver2.ChallengeDome;
using Receiver2.ShootingBooth;

namespace CiarenceUnbelievableModifications
{
	internal static class UnlockManager
	{
		internal static ReceiverCoreScript rcs = ReceiverCoreScript.Instance();

		private static HashSet<string> opened_doors_hashset = new HashSet<string>()
		{
			"MALL_LOBBY_LIFT",
			"MALL_LOBBY_METALGATE",
			"MALL_BALL_CLOSET_DOOR",
			"MALL_KEYCARD_DOOR_RED",
			"MALL_KEYCARD_DOOR_GREEN",
			"MALL_KEYCARD_DOOR_BLUE",
			"MALL_COURTYARD_METAL_GATE",
			"MALL_LOBBY_SECRETDOOR",
			"MALL_HOOP_STORAGE_ROOM_DOOR",
			"MALL_LOBBY_MAINTENANCE",
			"MALL_SENSORY_TANK",
			"MALL_SHOOTING_RANGE_LOOSE_AC"
		};

		private static HashSet<string> mall_tape_cassette_collection_to_unlock = new HashSet<string>()
		{
			"IS_Baseline_Alone",
			"LowcapMags",
			"WeirdBuildings",
			"Beginner_Balance",
			"IS_Sleeper_Deception",
			"Sig226_Controls",
			"SuicideTape3",
			"FlashlightIdentification",
			"Sleeper_TheLost",
			"IS_Sleeper_Organization",
			"Weaknesses",
			"MindTech",
			"ExtraRound",
			"Beginner_GunSafety",
			"IS_Baseline_Connection",
			"IS_Sleepwalker_WhyTapes",
			"HipointAppearance",
			"GlockTutorial",
			"Sig226_MK25",
			"IS_Sleeper_NoMindKill",
			"Beginner_Focus",
			"SuicideTape2",
			"DeathGenericGrit",
			"DetectiveRecoil",
			"SuicideTape6",
			"Beginner_Dedication",
			"SAATutorial",
			"Mindkill",
			"Beginner_Threat",
			"Sleeper_MindTechActivation",
			"Liminal_Cleartape",
			"BreathControl",
			"MurphysLaw",
			"Beginner_TerminatingThoughts",
			"DeathGenericResilience",
			"FirearmOperation",
			"IS_Awake_Analogy",
			"IS_Baseline_Online",
			"SAA_Appearance",
			"IS_Sleepwalker_AdvancedDreaming",
			"DeathMissed",
			"Beginner_SelfDefense",
			"GunSafety2",
			"SuicideTape2Post",
			"PerpetualSet",
			"Sleeper_FirearmExercise",
			"IS_Awake_AdvancedThreading",
			"Sleepwalker_Severing",
			"SightPicture",
			"Suicide",
			"FailureToFeed",
			"Liminal_MindTechManifestation",
			"Sleepwalker_MindTechCleartape",
			"DeathGenericMotivation",
			"InitiationTapeA",
			"GlockAuto",
			"Sleeper_PhysicalIsolation",
			"RealityB",
			"InformationOverlaod",
			"Sleepwalker_MindKillContingency",
			"M1911Safety",
			"Reflections",
			"IS_Sleepwalker_MindKillDefeated",
			"GlockAppearance",
			"Suicide2",
			"IS_Sleeper_Checkmate",
			"Beginner_Inversion",
			"SuicideTape4",
			"IFeelYou",
			"SuicideTape7Post",
			"Introduction_NotADream",
			"IS_Baseline_IsolationSet",	
			"DeathGenericFreeWill",
			"IS_Liminal_Habit",
			"Sleeper_Fear",
			"Sig226_Seals",
			"M10_tape2",
			"TheClearTape",
			"Liminal_Systems",
			"Beginner_Anxiety",
			"CreepyBot",
			"HipointQuality",
			"Sleepwalker_RepeatedListenings",
			"ScreamingTape",
			"Belief",
			"GunSafety4",
			"History1911",
			"Beginner_IntroToBeginner",
			"DeathOutOfAmmo",
			"SuicideTape6Post",
			"Balloons",
			"M10Tutorial",
			"M9Competition2",
			"M10_tape3",
			"IS_Liminal_OnlyOneSkill",
			"IS_Sleepwalker_MindKillDefeated",
			"IS_Baseline_IsolationSet",
			"FlashlightNecessity",
			"MindKillCont",
			"SuicideTape7",
			"Liminal_Potential",
			"Amnesia",
			"IS_Awake_AdvancedThreading",
			"Sig226Tutorial",
			"M1911Tutorial",
			"M1911Appearance",
			"SuicideTape1",
			"Sleeper_Cycle",
			"HiPointTutorial",
			"IS_Sleepwalker_Receivings",
			"OutwardsThought",
			"InformationCuration",
			"Beginner_Trauma",
			"Liminal_IntroToLiminals",
			"SuicideTape5Post",
			"DeagleFlash",
			"IS_Sleeper_Deception",
			"GunSafety3",
			"IS_Awake_RealityC",
			"Familiar",
			"Sleeper_IntroToSleeping",
			"SAA_SlipHammer",
			"Sleeper_InnerFormation",
			"Killdrones",
			"M9Design",
			"Sleepwalker_Minthought",
			"DeagleTutorial",
			"WhyNow",
			"Sleepwalker_MindTech",
			"Sleepwalker_RealityB",
			"RevolverCylinderDirection",
			"Awake_EndingDInsightTwo",
			"GunSafety1",
			"PressCheck",
			"HipointControls",
			"SuicideTape5",
			"M99mm",
			"Sleeper_Antipattern",
			"MindfulRoomClearing",
			"IS_Sleeper_NoMindKill",
			"FlashlightStance",
			"SuicideHelp",
			"IS_Awake_PracticalAdvantage",
			"Sleepwalker_TheCall",
			"Sleeper_Overload",
			"Sleeper_WhatIsMindtech",
			"Liminal_BigBang",
			"Beginner_PhysicalTraining",
			"IS_Awake_RealityC",
			"SuicideTape4Post",
			"Liminal_Undermined",
			"Sleepwalker_Awake",
			"M9Appearance",
			"Doublefeed",
			"DetectiveColt",
			"IS_Sleeper_Checkmate",
			"Beginner_MindBodyLink",
			"IS_Baseline_Online",
			"SAA_Safety",
			"Beginner_OriginOfTheMind",
			"BerettaM9Tutorial",
			"UpsideDown",
			"DeathGenericFrustration",
			"IS_Liminal_OriginalIdeas",
			"Sleeper_FirstSteps",
			"DeathEasyJam",
			"Liminal_DarkEnergy",
			"IS_Baseline_Alone",
			"Awake",
			"IS_Liminal_Damage",
			"ClearTape",
			"Liminal_CounterDreaming",
			"Awake_EndingDInsightThree",
			"IS_Awake_PracticalAdvantage",
			"SuicideTape1Post",
			"Stovepipe",
			"GlockSafety",
			"Beginner_Phobia",
			"MurphysLaw2",
			"M10_tape4",
			"IS_Sleepwalker_Receivings",
			"Beginner_MentalTraining",
			"Suicide3",
			"IS_Sleeper_Organization",
			"WhyTapes",
			"MentalHygiene",
			"Sleepwalker_Preparation",
			"DetectiveTutorial",
			"SuicideTape3Post",
			"RepeatedListenings",
			"DeagleAppearance",
			"DeagleControls",
			"DetectiveAccuracy",
			"DeathBadHit",
			"Awake_EndingDInsightOne"
		};

		internal static HashSet<string> trophies_to_unlock = new HashSet<string>()
		{
			"wolfire.statue_bomb_bot",
			"wolfire.statue_hand",
			"wolfire.statue_turret",
			"wolfire.statue_camera",
			"wolfire.statue_ball",
			"wolfire.statue_music_head",
			"wolfire.statue_beretta_m9",
			"wolfire.statue_receiver_man",
			"wolfire.statue_desert_eagle",
			"wolfire.statue_arcade",
			"wolfire.statue_diskette",
			"wolfire.statue_polaroid",
			"wolfire.statue_model10",
			"wolfire.statue_hipoint",
			"wolfire.statue_sig226",
			"wolfire.statue_glock17",
			"wolfire.statue_detective_special",
			"wolfire.statue_saa",
			"wolfire.statue_1911",
			"wolfire.statue_sneak_bot",
			"wolfire.statue_cassette"
		};

		internal static HashSet<string> challenges_to_unlock = new HashSet<string>()
		{
			"metal_targets_big",
			"metal_targets_random",
			"LimitedShotChallenge.s10",
			"metal_targets_small",
			"metal_targets_perifery",
			"triple_precision",
			"drone_circle",
			"MultidistanceScore.t30",
			"turrets_easy",
			"drone_attack",
			"turrets_eliminate",
			"TimedScoreChallenge.t30",
			"turrets_hard"
		};

		internal static HashSet<string> music_head_solutions_to_unlock = new HashSet<string>()
		{
			"Solution4",
			"Solution13",
			"Solution6",
			"Solution8",
			"Solution7",
			"Solution11",
			"Solution15",
			"Solution10",
			"Solution14",
			"Solution12"
		};

		internal static class Methods
		{
			internal static void OpenUnlockManagerMenu()
			{
				if (ImGui.Button("Unlock everything (Cut Content not included)"))
				{
					UnlockAllDoors();
					UnlockAllTapes();
					UnlockAllTrophies();

					rcs.PlayerData.finished_intro = true;

					rcs.MallData.UnlockAllMallItems();

					rcs.MallData.UnlockFlashlight();

					UnlockAllChallenges();

					UnlockAllAchievements();

					UnlockAllMusicHeadSolutions();

					UnlockAllNotes();

					UnlockAllHelpEntries();
				}

				if (ImGui.Button("Lock everything (and reset progress)"))
				{
					rcs.ResetProgress();
					rcs.Restart(true, true);
				}
			}

			static Dictionary<BaseAchievement, AchievementID> achievements_things;

			internal static void UnlockAllAchievements()
			{
				if(achievements_things == null)
				{
					achievements_things = AccessTools.FieldRefAccess<AchievementManager, Dictionary<BaseAchievement, AchievementID>>("active_achievements").Invoke(AchievementManager.Instance);
				}

				foreach (var achievement_id in achievements_things.ToList())
				{
					if (!AchievementManager.Instance.IsUnlocked(achievement_id.Value))
					{ 
						AchievementManager.Instance.UnlockAchievement(achievement_id.Key);	
					}
				}
			}

			static HashSet<string> unlocked_music_head_solutions;

			internal static void UnlockAllMusicHeadSolutions()
			{
				var things = UnityEngine.Object.FindObjectOfType<CubeHeadMusicManager>();

				if (things)
				{
					things.UnlockAllTargets();
				}
				else
				{
					if (unlocked_music_head_solutions == null)
					{
						unlocked_music_head_solutions = AccessTools.FieldRefAccess<PersistentMallData, HashSet<string>>("unlocked_music_head_solutions").Invoke(rcs.MallData);
					}

					foreach (var thing in music_head_solutions_to_unlock)
					{
						unlocked_music_head_solutions.Add(thing);
					}
				}
			}

			internal static void UnlockAllChallenges()
			{
				MallChallengeComputer[] array6 = global::UnityEngine.Object.FindObjectsOfType<MallChallengeComputer>();
				for (int i = 0; i < array6.Length; i++)
				{
					array6[i].UnlockAllChallenges();
				}

				ShootingBoothScript[] array7 = global::UnityEngine.Object.FindObjectsOfType<ShootingBoothScript>();
				for (int i = 0; i < array7.Length; i++)
				{
					array7[i].UnlockAllChallenges();
				}

				if (array6.Length == 0)
				{
					foreach (string challenge_id in challenges_to_unlock)
					{
						rcs.MallData.SetChallengeUnlocked(challenge_id);
					}
				}
			}

			internal static void UnlockAllNotes()
			{
				foreach (NoteData noteData in rcs.note_loadout_asset.notes)
				{
					rcs.PlayerData.picked_up_diary_ids_string.Add(noteData.id.ToString());
				}
			}

			internal static void UnlockAllHelpEntries()
			{
				foreach (HelpEntryData helpEntryData in rcs.help_entry_loadout_asset.help_entries)
				{
					rcs.PlayerData.unlocked_help_entry_ids_string.Add(helpEntryData.id.ToString());
				}
			}

			internal static void UnlockAllDoors()
			{
				foreach (string door_id in opened_doors_hashset)
				{
					rcs.MallData.SetDoorOpened(door_id, true);
				}

				DoorBase[] array5 = global::UnityEngine.Object.FindObjectsOfType<DoorBase>();
				for (int i = 0; i < array5.Length; i++)
				{
					array5[i].ForceUnlockedState();
				}
			}

			internal static void UnlockAllTapes()
			{
				foreach (string tape_id in mall_tape_cassette_collection_to_unlock)
				{
					rcs.MallData.UnlockMallTape(tape_id);
					rcs.PlayerData.picked_up_tape_ids_string.Add(tape_id);
				}
			}

			internal static void UnlockAllTrophies()
			{
				rcs.PlayerData.trophy_progress_deagle = new HashSet<string>()
				{
					"drone_circle.desert_eagle",
					"drone_attack.desert_eagle"
				};

				rcs.PlayerData.trophy_progress_sig = new HashSet<string>()
				{
					"turrets_hard.sig226",
					"turrets_easy.sig226",
					"turrets_elminate.sig226"
				};

				foreach (string trophy_id in trophies_to_unlock)
				{
					TrophyUnlockManager.UnlockTrophy(trophy_id);
				}
			}
		}

		internal static class Transpilers
		{
			[HarmonyPatch(typeof(MenuManagerScript), "UpdateDeveloperMenu")]
			[HarmonyTranspiler]
			private static IEnumerable<CodeInstruction> AddUnlockManagerButtonToDebugMenu(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase __originalMethod)
			{
				//allow_gun_drop is only present once in the whole Update method, so we'll only look for that
				CodeMatcher codeMatcher = new CodeMatcher(instructions, generator).MatchForward(false, new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ImGui), nameof(ImGui.EndMainMenuBar))));

				if (!codeMatcher.ReportFailure(__originalMethod, MainPlugin.Logger.LogError))
				{
					var labelsssss = codeMatcher
						.Instruction
						.ExtractLabels();

					codeMatcher
						.CreateLabelAt(codeMatcher.Pos, out var endLabel)
						.Insert(
							new CodeInstruction(OpCodes.Ldstr, "Unlock Manager"),
							new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ImGui), nameof(ImGui.BeginMenu), new Type[] { typeof(string) })),
							new CodeInstruction(OpCodes.Brfalse_S, endLabel),
							new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UnlockManager.Methods), nameof(UnlockManager.Methods.OpenUnlockManagerMenu))),
							new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ImGui), nameof(ImGui.EndMenu))))
						.AddLabels(labelsssss)
						;
				}

				return codeMatcher.InstructionEnumeration();
			}
		}
	}
}
