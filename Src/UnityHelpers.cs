using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CiarenceUnbelievableModifications
{
    public static class UnityHelpers
    {
        public static bool TryFind(this Transform transform, string path, out Transform outTransform)
        {
            return (outTransform = transform.Find(path)) != null;
        }

        public static bool HasComponent<T>(this Transform transform) where T : Component
        {
            return transform.GetComponent<T>() != null;
        }

        public static bool HasComponent<T>(this Component component) where T : Component
        {
            return component.GetComponent<T>() != null;
        }

        public static T AddComponent<T>(this Transform transform) where T : Component
        {
            return transform.gameObject.AddComponent<T>();
        }

        public static T AddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Transform transform) where T : Component
        {
            return (transform.TryGetComponent<T>(out var result)) ? result : transform.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return (component.TryGetComponent<T>(out var result)) ? result : component.AddComponent<T>();
        }

		public static int ToInt (this bool boolean)
		{
			return boolean ? 1 : 0;
		}
    }
}
