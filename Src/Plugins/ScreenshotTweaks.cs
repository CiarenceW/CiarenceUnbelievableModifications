using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace CiarenceUnbelievableModifications
{
	internal static class ScreenshotTweaks
	{
		/*private static void CopyToClipboard(Texture2D texture)
		{
			if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
			{
				var memStream = new System.IO.MemoryStream(texture.EncodeToPNG());

				PngBitmapDecoder pngBitmapDecoder = new PngBitmapDecoder(memStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

				BitmapFrame png = pngBitmapDecoder.Frames[0];

				Clipboard.SetImage(png);
			}
			else
			{
				Debug.LogWarning("Copy to clipboard is only supported on windows, sorry");
			}
		}*/
	}
}
