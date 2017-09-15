using Assets.Gamelogic.EntityTemplates;
using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Entity.Component;
using Improbable.Core;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Assets.Gamelogic.Player
{
	[WorkerType(WorkerPlatform.UnityClient)]
	public class ShowLatestInstructions : MonoBehaviour
	{
		[Require] private Instructions.Reader instructionsReader;
		[Require] private PlayerActions.Writer playerActionsWriter;

		private Canvas scoreCanvasUI;
		private Text totalPointsGUI;

		private string latestInstructions;

		private void OnEnable()
		{
			GameObject tempObj = GameObject.Find("ScoreCanvas");
			scoreCanvasUI = tempObj.GetComponent<Canvas>();
			if (scoreCanvasUI != null) {
				totalPointsGUI = scoreCanvasUI.GetComponentInChildren<Text>();
			}

			instructionsReader.ComponentUpdated.Add(OnComponentUpdated);
			totalPointsGUI.text = instructionsReader.Data.instructionDescription;
		}

		private void OnDisable()
		{
			instructionsReader.ComponentUpdated.Remove(OnComponentUpdated);
		}

		private void OnComponentUpdated(Instructions.Update update)
		{
			Debug.LogError (instructionsReader.Data.instructionDescription);
			totalPointsGUI.text = instructionsReader.Data.instructionDescription;
		}
	}
}