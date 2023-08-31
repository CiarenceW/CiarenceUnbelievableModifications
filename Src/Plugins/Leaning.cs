using HarmonyLib;
using Receiver2;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiarenceUnbelievableModifications
{
    public static class Leaning
    {
        private static bool leaning;
        private static LeaningDirection leaningDirection;
        private static float leaningRotation = 30f;
        private static Transform lean_pivot;

        public static void Initialize()
        {
            lean_pivot = UnityEngine.Object.Instantiate(new GameObject("lean_pivot"), ReceiverCoreScript.Instance().player_prefab.transform).transform;
            lean_pivot.localPosition = Vector3.zero;
            lean_pivot.localRotation = Quaternion.identity;
        }

        [HarmonyPatch(typeof(LocalAimHandler), nameof(LocalAimHandler.SetUpCamera))]
        [HarmonyPostfix]
        private static void PatchSetupCamera()
        {
            LocalAimHandler.player_instance.main_camera.transform.parent = lean_pivot;
            lean_pivot.parent = LocalAimHandler.player_instance.transform;
        }

        [HarmonyPatch(typeof(LocalAimHandler), nameof(LocalAimHandler.UpdateCameraAndPlayerTransformation))]
        [HarmonyPostfix]
        private static void PatchUpdateCamera()
        {
            if (leaningDirection == LeaningDirection.Forward) leaning = false;
            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.E))
            {
                if (!leaning) leaning = true;
                leaningDirection = LeaningDirection.Forward;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (!leaning)
                {
                    leaning = true;
                    leaningDirection = LeaningDirection.Left;
                }
                else if (leaningDirection == LeaningDirection.Left)
                {
                    leaning = false;
                    leaningDirection = LeaningDirection.None;
                }
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                if (!leaning)
                {
                    leaning = true;
                    leaningDirection = LeaningDirection.Right;
                }
                else if (leaningDirection == LeaningDirection.Right)
                {
                    leaning = false;
                    leaningDirection = LeaningDirection.None;
                }
            }

            float x = 0f;
            float z = 0f;
            if (leaning)
            {
                switch (leaningDirection)
                {
                    case LeaningDirection.Left:
                        x = leaningRotation;
                        break;
                    case LeaningDirection.Right:
                        x = -leaningRotation;
                        break;
                    case LeaningDirection.Forward:
                        z = -leaningRotation;
                        break;
                }
            }
            lean_pivot.eulerAngles = new Vector3(Mathf.MoveTowardsAngle(lean_pivot.eulerAngles.x, x, Time.deltaTime * 200f), 0f, Mathf.MoveTowardsAngle(lean_pivot.eulerAngles.z, z, Time.deltaTime * 200f));
        }

        private enum LeaningDirection
        {
            None,
            Left,
            Right,
            Forward
        }
    }
}
