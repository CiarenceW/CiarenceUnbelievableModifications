using BepInEx;
using Receiver2;
using UnityEngine.Events;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Wolfire;
using HarmonyLib;
using SimpleJSON;

namespace CiarenceUnbelievableModifications
{
    public static class VictorianFix
    {
        private static TileScript victorian;
        private static bool patched;

        public static bool verbose;

        //there are a lot more things that could be done.
        //there's a way to add more spawns, however, doing that by code will be very time consuming.

        [HarmonyPatch(typeof(RuntimeTileLevelGenerator), "Awake")]
        [HarmonyPostfix]
        private static void PatchTileGenAwake(ref RuntimeTileLevelGenerator __instance)
        {
            if (patched) return;
            victorian = __instance.tile_prefabs.First(it => { return it is TileScript && it.name == "m_11_victorian"; });
            if (victorian != null)
            {
                var top_floor_collider = victorian.transform.Find("physical/Base/VictorianModule_Model/Collision/LoftFloorCollision_WithHatch");
                var top_floor_collider_pos = top_floor_collider.localPosition;
                top_floor_collider_pos.y = 8.37f;
                top_floor_collider.localPosition = top_floor_collider_pos;
                if(verbose) Debug.Log("Changed victorian top floor collider position");    
            }
            else
            {
                Debug.LogError("SHIT FUCKING VICTRORIAN IS NULLLLLLLL!!!!!!!!!!!!!!!!!!!!!!!");
            }
            patched = true;
        }

        /*private void OnInitialize(ReceiverEventTypeVoid ev)
        {
            Debug.Log("Player initialized");
            if (RuntimeTileLevelGenerator.instance != null)
            {
                RTLG = RuntimeTileLevelGenerator.instance;
                victorian = RTLG.tile_prefabs.First(it => { return it is TileScript && it.name == "m_11_victorian"; });
                if (victorian != null)
                {
                    var top_floor_collider = victorian.transform.Find("physical/Base/VictorianModule_Model/Collision/LoftFloorCollision_WithHatch");
                    var top_floor_collider_pos = top_floor_collider.localPosition;
                    Debug.Log(top_floor_collider_pos);
                    top_floor_collider_pos.y = 8.37f;
                    top_floor_collider.localPosition = top_floor_collider_pos;
                    Debug.Log("Changed victorian position");
                    Debug.Log(victorian.transform.Find("physical/Base/VictorianModule_Model/Collision/LoftFloorCollision_WithHatch").localPosition);
                }
                else
                {
                    Debug.LogError("SHIT FUCKING VICTRORIAN IS NULLLLLLLL!!!!!!!!!!!!!!!!!!!!!!!");
                }
            }
            else
            {
                Debug.LogError("RTLG is null");
            }
        }*/
    }
}
