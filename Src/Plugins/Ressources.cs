using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace CiarenceUnbelievableModifications
{
	public class Ressources : MonoBehaviour
	{
		public static Ressources Instance
		{
			get;
			private set;
		}

		public void Awake()
		{
			Instance = this;
		}

		public PostProcessResources.ComputeShaders computeShaders = new PostProcessResources.ComputeShaders();

		public GameObject bombBotNewPrefab;

		internal GameObject bombBotOldPrefab;
	}
}
