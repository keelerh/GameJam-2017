using UnityEngine;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using Improbable.Player;
using UnityEngine.UI;

namespace Assets.Gamelogic.Player
{   
	[WorkerType(WorkerPlatform.UnityClient)]
	public class ReduceBananas : MonoBehaviour
	{	
//		private ActionFirer actionFirer;
		[Require] private PlayerActions.Writer PlayerActionsWriter;
		[Require] private Instructions.Reader InstructionsReader;

		private Canvas scoreCanvasUI2;
		private Text bananaGUI;

		private void OnEnable()
		{
//			actionFirer = GetComponent<ActionFirer>();
			InvokeRepeating("KillBanana", 0, 5.0f);

			GameObject tempObj = GameObject.Find("ScoreCanvas2");
			scoreCanvasUI2 = tempObj.GetComponent<Canvas>();

			if (scoreCanvasUI2 != null) {
				bananaGUI = scoreCanvasUI2.GetComponentInChildren<Text>();
			}
		}

		private void OnDisable()
		{
			CancelInvoke("KillBanana");
		}

		private void KillBanana()
		{	
			int newBananas = PlayerActionsWriter.Data.bananas - 1;

			if (InstructionsReader.Data.instructionDescription == "You are the rogue monkey") {
				newBananas++;
			}

			if (newBananas < 0) {
//				Debug.Log("Trying to kill player " + this.gameObject.EntityId());
				PlayerActionsWriter.Send(new PlayerActions.Update().AddSuicide(new Suicide()));
				PlayerActionsWriter.Send(new PlayerActions.Update().SetBananas(0));
				return;
			}

			bananaGUI.text = newBananas + " bananas";

			PlayerActionsWriter.Send(new PlayerActions.Update().SetBananas(newBananas));
		}
	}


}
