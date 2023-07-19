using Receiver2;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using System.Security.Cryptography;
using static UnityEngine.UIElements.StyleVariableResolver;
using UnityEngine.UIElements;

namespace CiarenceUnbelievableModifications
{
    internal static class rtlgTweaks //SHUT UP!!!!!!!! I DON'T OBEY BY YOUR RULES!!!!!!! NAMING VIOLATIONS MEAN *NOTHING* TO ME
    {
        private static bool patched;
        public static bool verbose;

        internal static class VictorianFix
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

        internal static class TwoTowersFix
        {
            private static TileScript twoTowers;

            internal static void ChangeTwoTowersWalkwayLayer() //fixes bullets being able to shoot through the base of the tile
            {
                twoTowers = GetTilePrefab(ReceiverTileType.M_10TwoTowers);
                twoTowers.transform.Find("physical/Base/Architecture/10_tower_walkramp").gameObject.layer = 0; 
            }
        }

        public static TileScript GetTilePrefab(ReceiverTileType tileType)
        {
            return (from e in RuntimeTileLevelGenerator.instance.tile_prefabs where e.tile_type == tileType select e).FirstOrDefault();
        }

        internal static void OnInitialize(ReceiverEventTypeVoid ev)
        {
            if (RuntimeTileLevelGenerator.instance != null && !SettingsManager.configVictorianFix.Value)
            {
                if (patched) return;
                TwoTowersFix.ChangeTwoTowersWalkwayLayer();
                VictorianFix.ChangeVictorianTopFloorCollision();
                patched = true;
            }
        }

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
