using Receiver2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiarenceUnbelievableModifications
{
	internal static class HUDScalingTweak
	{
		public static void ChangeHUDScaleMode()
		{
			PlayerGUI.Instance.canvas.GetComponent<UnityEngine.UI.CanvasScaler>().uiScaleMode = SettingsManager.configHUDScaleMode.Value;
		}

		public static void SetHUDScaleFactor()
		{
			PlayerGUI.Instance.canvas.GetComponent<UnityEngine.UI.CanvasScaler>().scaleFactor = SettingsManager.configHUDScaleFactor.Value;
		}
	}
}
