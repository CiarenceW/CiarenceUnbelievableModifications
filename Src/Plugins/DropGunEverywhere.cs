using Receiver2;
using UnityEngine.Events;
using System.Reflection;
using UnityEngine;
using BepInEx;

namespace CiarenceUnbelievableModifications
{
    public static class DropGunEverywhere
    {
        private static LocalAimHandler lah;
        public static void Enable()
        {
            if (LocalAimHandler.TryGetInstance(out lah))
            {
                lah.allow_gun_drop = true;
            }
        }

        public static void Disable()
        {
            LevelObject level_object = (LevelObject)typeof(ReceiverCoreScript).GetField("level_object", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ReceiverCoreScript.Instance());
            lah.allow_gun_drop = level_object.allow_gun_drop;
        }
    }
}