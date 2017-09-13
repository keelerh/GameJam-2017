using Assets.Gamelogic.Utils;
using UnityEngine;
using Assets.Gamelogic.Core;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine.SceneManagement;
using Improbable.Unity.Core.EntityQueries;
using Improbable;
using Improbable.Unity.Core;
using System.Collections;
using UnityEngine.UI;


namespace Assets.Gamelogic.UI
{
	[WorkerType(WorkerPlatform.UnityClient)]
	public class BillboardRotation : MonoBehaviour
	{
		public bool FixVerticalAxis = false;
		private Text playerStatus;

		void OnEnable()
		{
			playerStatus = this.gameObject.GetComponentInChildren<Text>();
//			playerStatus.text = "player_" + this.transform.parent.gameObject.EntityId().ToString();
			playerStatus.text = "player_" + this.transform.parent.gameObject.transform.name;
		}

		private void LateUpdate()
		{
			if (FixVerticalAxis)
			{
				transform.forward = Camera.main.transform.forward.FlattenVector();
			}
			else
			{
				transform.forward = Camera.main.transform.forward;
			}
		}
	}
}