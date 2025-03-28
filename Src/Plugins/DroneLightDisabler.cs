using HarmonyLib;
using Receiver2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CiarenceUnbelievableModifications
{
    public static class DroneLightDisabler
	{
		[HarmonyPatch(typeof(TurretScript), nameof(TurretScript.Update))]
		[HarmonyPostfix]
		private static void DynamicallyDisableTurretLightsBasedOnDistance(Receiver2.TurretScript __instance, UnityEngine.Light ___gun_pivot_camera_light, UnityEngine.Light ___gun_pivot_camera_light_point, AIState ___ai_state)
		{
			if (__instance.camera_alive && ___ai_state == AIState.Idle && LocalAimHandler.player_instance && SettingsManager.configEnableDroneLightDisabler.Value == true)
			{
				var sqr = Vector3.Distance(LocalAimHandler.player_instance.transform.position, __instance.transform.position);
				if (sqr < SettingsManager.configLightCutoffDistance.Value)
				{
					if (sqr < SettingsManager.configLightFadeDistance.Value)
					{
						if (SettingsManager.configOnlyDisablePointLights.Value == false)
							___gun_pivot_camera_light.intensity = 1.25f;

						___gun_pivot_camera_light_point.intensity = 1.25f;
					}
					else
					{
						var smoothIntensity = Mathf.Lerp(1.25f, 0, (sqr - SettingsManager.configLightFadeDistance.Value) * (1 / (SettingsManager.configLightCutoffDistance.Value - SettingsManager.configLightFadeDistance.Value)));

						if (SettingsManager.configOnlyDisablePointLights.Value == false)
							___gun_pivot_camera_light.intensity = smoothIntensity;

						___gun_pivot_camera_light_point.intensity = smoothIntensity;
					}

					___gun_pivot_camera_light.enabled = true;
					___gun_pivot_camera_light_point.enabled = true;
				}
				else
				{
					if (SettingsManager.configOnlyDisablePointLights.Value == false)
						___gun_pivot_camera_light.enabled = false;

					___gun_pivot_camera_light_point.enabled = false;
				}
			}
		}

		[HarmonyPatch(typeof(LightPart), "Update")]
		[HarmonyPostfix]
		private static void DynamicallyDisableLightPartBasedOnDistance(LightPart __instance, LightPart.LightMode ___current_light_mode, RobotPart ___part)
		{
			if (__instance.battery.Alive && ___part.Alive && ___current_light_mode == LightPart.LightMode.Passive && LocalAimHandler.player_instance && SettingsManager.configEnableDroneLightDisabler.Value == true)
			{
				var sqr = Vector3.Distance(LocalAimHandler.player_instance.transform.position, __instance.transform.position);
				if (sqr < SettingsManager.configLightCutoffDistance.Value)
				{
					if (sqr < SettingsManager.configLightFadeDistance.Value)
					{
						if (SettingsManager.configOnlyDisablePointLights.Value == false)
							__instance.spot_light.intensity = 3f;

						if (__instance.point_light)
							__instance.point_light.intensity = 3f;
					}
					else
					{
						var smoothIntensity = Mathf.Lerp(1.25f, 0, (sqr - SettingsManager.configLightFadeDistance.Value) * (1 / (SettingsManager.configLightCutoffDistance.Value - SettingsManager.configLightFadeDistance.Value)));

						if (SettingsManager.configOnlyDisablePointLights.Value == false)
							__instance.spot_light.intensity = smoothIntensity;

						if (__instance.point_light)
							__instance.point_light.intensity = smoothIntensity;
					}

					__instance.spot_light.enabled = true;

					if (__instance.point_light)
						__instance.point_light.enabled = true;
				}
				else
				{
					if (SettingsManager.configOnlyDisablePointLights.Value == false)
						__instance.spot_light.enabled = false;

					if (__instance.point_light)
						__instance.point_light.enabled = false;
				}
			}
		}
	}
}
