using Receiver2;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using System.Security.Cryptography;
using UnityEngine.UIElements;
using SimpleJSON;
using System.Reflection;
using System.CodeDom;
using System.Collections.Generic;
using System;
using System.Reflection.Emit;
using ImGuiNET;
using Receiver2ModdingKit.Helpers;

namespace CiarenceUnbelievableModifications
{
    internal static class rtlgTweaks //SHUT UP!!!!!!!! I DON'T OBEY BY YOUR RULES!!!!!!! NAMING VIOLATIONS MEAN *NOTHING* TO ME
    {
        private static bool patched;
        public static bool verbose;

        internal static class TileFix
        {
            private static TileScript victorian;

            //there are a lot more things that could be done.
            //there's a way to add more spawns, however, doing that by code will be very time consuming.

            internal static void ChangeVictorianTopFloorCollision()
            {
                victorian = GetTilePrefab(ReceiverTileType.M_11Victorian);
                if (victorian != null)
                {
                    var top_floor_collider = victorian.transform.Find("physical/Base/VictorianModule_Model/Collision/LoftFloorCollision_WithHatch");
                    var top_floor_collider_pos = top_floor_collider.localPosition;
                    top_floor_collider_pos.y = 8.37f;
                    top_floor_collider.localPosition = top_floor_collider_pos;
                    if (verbose) Debug.Log("Changed victorian top floor collider position");
                }
                else
                {
                    Debug.LogError("SHIT FUCKING VICTRORIAN IS NULLLLLLLL!!!!!!!!!!!!!!!!!!!!!!!");
                }
            }
            private static TileScript twoTowers;

            internal static void ChangeTwoTowersWalkwayLayer() //fixes bullets being able to shoot through the base of the tile
            {
                twoTowers = GetTilePrefab(ReceiverTileType.M_10TwoTowers);
                twoTowers.transform.Find("physical/Base/Architecture/10_tower_walkramp").gameObject.layer = 0; 
            }
        }

        /*internal static class GapAndCatwalkFix
        {
            private static TileScript gapAndCatwalk;

            internal static void AssignMissingProbesBottom()
            {
                if (patched) return;
                gapAndCatwalk = GetTilePrefab(ReceiverTileType.M_05GapAndCatwalk);

                if (gapAndCatwalk != null)
                {
                    gapAndCatwalk.transform.Find("m_05_gap_and_catwalk(Clone)/physical/Bottom Middle Windows(Clone)/Objects/r2_module5_window_small (2)/")
                }
            }
        }*/

        public static TileScript GetTilePrefab(ReceiverTileType tileType)
        {
            return (from e in RuntimeTileLevelGenerator.instance.tile_prefabs where e.tile_type == tileType select e).FirstOrDefault();
        }

        internal static void OnInitialize(ReceiverEventTypeVoid ev)
        {
            if (RuntimeTileLevelGenerator.instance != null && SettingsManager.configVictorianFix.Value)
            {
                if (patched) return;
                TileFix.ChangeTwoTowersWalkwayLayer();
                TileFix.ChangeVictorianTopFloorCollision();
                patched = true;
            }
        }

        internal static class TapePlayerScriptCompatibilityPatch
        {
			internal delegate ActiveItem SpawnTapeDelegate(RuntimeTileLevelGenerator instance, TileRoom tile_room, Vector3 position, Quaternion rotation, short local_ammo_spawn_id, string item_spawn_name, JSONObject persistent_data);

			internal static SpawnTapeDelegate spawnTapeDelegate = AccessTools.MethodDelegate<SpawnTapeDelegate>(AccessTools.Method(typeof(RuntimeTileLevelGenerator), "SpawnTape"));

            [HarmonyPatch(typeof(RuntimeTileLevelGenerator), "SpawnTapeGroup")]
            [HarmonyPrefix]
            private static bool YeetSpawnTapeGroupIfTapePlayerScript(RuntimeTileLevelGenerator __instance, ref ActiveItem __result, TileRoom tile_room, Vector3 position, Quaternion rotation, short local_ammo_spawn_id, string item_spawn_name, JSONObject persistent_data)
            {
                if ((LocalAimHandler.player_instance != null && !LocalAimHandler.player_instance.simplified_tape_player) || (ReceiverCoreScript.Instance().CurrentLoadout != null && !ReceiverCoreScript.Instance().CurrentLoadout.simplified_tape_player))
                {
                    __result = spawnTapeDelegate(__instance, tile_room, position, rotation, local_ammo_spawn_id, item_spawn_name, persistent_data);
                    return false;
                }

                return true;
            }

            internal delegate GameObject InstantiateTapeGroupDelegate(RuntimeTileLevelGenerator instance, Vector3 vector3, Quaternion quaternion, Transform transform);

            internal static InstantiateTapeGroupDelegate instantiateTapeDelegate = AccessTools.MethodDelegate<InstantiateTapeGroupDelegate>(AccessTools.Method(typeof(RuntimeTileLevelGenerator), "InstantiateTape"));

            [HarmonyPatch(typeof(RuntimeTileLevelGenerator), "InstantiateTapeGroup")]
            [HarmonyPrefix]
            private static bool YeetInstatiateTapeGroupIfTapePlayerScript(RuntimeTileLevelGenerator __instance, ref GameObject __result, Vector3 position, Quaternion rotation, Transform parent)
            {
                if ((LocalAimHandler.player_instance != null && !LocalAimHandler.player_instance.simplified_tape_player) || (ReceiverCoreScript.Instance().CurrentLoadout != null && !ReceiverCoreScript.Instance().CurrentLoadout.simplified_tape_player))
                {
                    __result = instantiateTapeDelegate(__instance, position, rotation, parent);
                    return false;
                }

                return true;
            }

			[HarmonyPatch(typeof(RuntimeTileLevelGenerator), "GenerateTapeGroupData")]
			[HarmonyPostfix]
			private static void GenerateTapeData(RuntimeTileLevelGenerator __instance, ref JSONObject __result)
			{
				var rcs = ReceiverCoreScript.Instance();
				Debug.Log(rcs.CurrentLoadout);
				if (!rcs.CurrentLoadout.simplified_tape_player)
				{
					JSONObject persistentData = new JSONObject();
					persistentData.Add("tape_id_string", rcs.tape_loadout_asset.GetPrioritizedTapeID((TapeGroupID)__result["tape_group_id"].AsInt, rcs.session_data.picked_up_tapes_string.ToArray<string>()));

					__result = persistentData;
				}
			}

			//idk what this is
            public static JSONObject TapeGroupDataToNormalTapeData(JSONObject tapeGroupData)
            {
                tapeGroupData["tape_id_string"] = new JSONObject();

                var value = tapeGroupData["tape_group_id"].AsInt;

                var rcs = ReceiverCoreScript.Instance();

                var rpgm = ReceiverCoreScript.Instance().game_mode as RankingProgressionGameMode;

                bool flag = rcs.WorldGenerationConfiguration.forced_tape_sequence.Count > rpgm.LevelTapeCollected;
                string text;
                if (flag)
                {
                    text = rcs.WorldGenerationConfiguration.forced_tape_sequence[rpgm.LevelTapeCollected];
                }
                else
                {
                    text = rcs.tape_loadout_asset.GetPrioritizedTapeID((TapeGroupID)value, rcs.session_data.picked_up_tapes_string.ToArray<string>());
                    if (string.IsNullOrEmpty(text))
                    {
                        rcs.session_data.picked_up_tapes_string.Clear();
                        text = rcs.tape_loadout_asset.GetPrioritizedTapeID((TapeGroupID)value, rcs.session_data.picked_up_tapes_string.ToArray<string>());
                    }
                }
                TapeContent tape = rcs.tape_loadout_asset.GetTape(text);/*
                bool flag2 = tape.tape_group == TapeGroupID.MindControl;
                bool flag3 = rpgm.CanBeMindcontrolled() || flag;
                if (flag2 && flag3)
                {
                    RankingProgressionGameMode.can_unlock_threat_attack_note = true;
                    rpgm.progression_data.has_picked_up_mindcontrol_tape = true;
                }
                bool flag4 = tape.tape_group == TapeGroupID.SneakBot;
                bool flag5 = rpgm.CanBeStalked() || flag;
                if (flag4 && flag5)
                {
                    RankingProgressionGameMode.can_unlock_sneak_bot_note = true;
                    rpgm.progression_data.has_picked_up_sneakbot_tape = true;
                }
                if (string.IsNullOrEmpty(text) || (flag2 && !flag3) || (flag4 && !flag5))
                {
                    HashSet<TapeGroupID> hashSet = new HashSet<TapeGroupID>(from x in rcs.WorldGenerationConfiguration.tape_groups
                                                                            select x.tape_group_id into x
                                                                            where x != TapeGroupID.MindControl && x != TapeGroupID.SneakBot
                                                                            select x);
                    text = rcs.tape_loadout_asset.GetPrioritizedTapeID(hashSet, rcs.session_data.picked_up_tapes_string.ToArray<string>());
                    global::UnityEngine.Debug.LogWarning("Tape group empty, falling back to other tape groups in configuration");
                    flag2 = false;
                }
                if (flag2 && !ConfigFiles.profile.enable_threat_echoes)
                {
                    text = rcs.tape_loadout_asset.GetTape(text).unlocks_tape_string;
                }*/
                global::UnityEngine.Debug.LogWarning("No more valid tapes can be picked up in play session");

                tapeGroupData["tape_id_string"] = text;

                return null;
            }
        }

        internal static class SpawnCompatibleMagsTweak
        {
            [HarmonyPatch(typeof(RuntimeTileLevelGenerator),
            nameof(RuntimeTileLevelGenerator.instance.InstantiateMagazine),
            new[] { typeof(Vector3), typeof(Quaternion), typeof(Transform), typeof(MagazineClass) }
            )]
            [HarmonyPrefix]
            private static bool InstantiateMagazine(ref GameObject __result, ref Vector3 position, ref Quaternion rotation, ref Transform parent, ref MagazineClass magazine_class)
            {
                var RCS = ReceiverCoreScript.Instance();
                MagazineScript magazinePrefab;
                var gun = RCS.GetGunPrefab(RCS.CurrentLoadout.gun_internal_name);
                RCS.TryGetMagazinePrefabFromRoot(gun.magazine_root_types[UnityEngine.Random.Range(0, gun.magazine_root_types.Length)], magazine_class, out magazinePrefab);
                __result = RuntimeTileLevelGenerator.instance.InstantiateMagazine(position, rotation, parent, magazinePrefab);
                return false;
            }
        }
    }
}
