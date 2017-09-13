using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.Player
{
	// Add this MonoBehaviour on client workers only
	[WorkerType(WorkerPlatform.UnityClient)]
	public class PlayerGUI : MonoBehaviour
	{
//		[Require] private ShipControls.Writer ShipControlsWriter;
//		[Require] private Score.Reader ScoreReader;

		private Canvas scoreCanvasUI;
		private Text totalPointsGUI;

		private void Awake()
		{
			if (scoreCanvasUI != null) {
				totalPointsGUI = scoreCanvasUI.GetComponentInChildren<Text>();
				scoreCanvasUI.enabled = false;
				updateGUI(0);
			}
		}

		private void OnEnable()
		{
			// Register callback for when components change
//			ScoreReader.NumberOfPointsUpdated.Add(OnNumberOfPointsUpdated);
			updateGUI(100);
		}

		private void OnDisable()
		{
			// Deregister callback for when components change
//			ScoreReader.NumberOfPointsUpdated.Remove(OnNumberOfPointsUpdated);
		}

		// Callback for whenever one or more property of the Score component is updated
		private void OnNumberOfPointsUpdated(int numberOfPoints)
		{
			updateGUI(numberOfPoints);
		}

		void updateGUI(int score)
		{

			Debug.LogWarning("GUI UPDATE");
			if (scoreCanvasUI != null) {
				if (score > 0)
				{
					scoreCanvasUI.enabled = true;
					totalPointsGUI.text = score.ToString();
				}
				else
				{
					scoreCanvasUI.enabled = false;
				}
			}
		}
	}
}