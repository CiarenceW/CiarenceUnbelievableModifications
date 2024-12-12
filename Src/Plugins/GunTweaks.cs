using Receiver2;
using UnityEngine;
using System.Linq;

namespace CiarenceUnbelievableModifications
{
    internal static class GunTweaks
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

        public static void PatchHiPointCatchMagSlideAmount() //that's a mouthful
        {
            var hipoints = (from e in ReceiverCoreScript.Instance().gun_prefabs where e.GetComponent<GunScript>().gun_model == GunModel.HiPoint select e.GetComponent<GunScript>());
            foreach (GunScript hipoint in hipoints)
            {
                if (hipoint != null)
                {
                    hipoint.magazine_catch_mag_slide_animation = "";
                    hipoint.magazine_catch_mag_slide_amount = 0.8025f; //I like this number
                }
            }
        }
    }
}
