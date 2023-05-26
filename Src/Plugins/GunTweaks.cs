using BepInEx;
using HarmonyLib;
using Receiver2;
using UnityEngine;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiarenceUnbelievableModifications
{
    public static class GunTweaks
    {
        public static void PatchDeaglesSpring()
        {
            var deagles = (from e in ReceiverCoreScript.Instance().gun_prefabs where e.GetComponent<GunScript>().weapon_group_name == "desert_eagle" select e);
            foreach (GameObject deagle in deagles)
            {
                //reflection-fest
                if (deagle != null)
                {
                    Transform mainspring = deagle.transform.Find("main_spring");
                    if (mainspring != null)
                    {
                        var mainspring_rot = mainspring.localRotation;
                        mainspring_rot.eulerAngles = new Vector3(5f, 0f, 20f);
                        mainspring.localRotation = mainspring_rot;
                        /*Quaternion rot_orig = (Quaternion)typeof(SpringCompressInstance).GetField("rot_orig", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(mainspring);
                        rot_orig.eulerAngles = new Vector3(5f, 0, 20f);
                        Debug.LogFormat("Patched {0}", deagle.GetComponent<GunScript>().InternalName);*/
                    }
                    else
                    {
                        Debug.LogError("hey, guess what, the mainspring is null.... oops");
                    }
                }
                else
                {
                    Debug.LogError("fucking deagle is null, can you believe it? fucking fuck.");
                }
            }
        }
    }
}
