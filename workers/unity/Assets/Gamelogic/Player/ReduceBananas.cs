using UnityEngine;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using Improbable.Player;

namespace Assets.Gamelogic.Player
{   
	[WorkerType(WorkerPlatform.UnityClient)]
	public class ReduceBananas : MonoBehaviour
	{	
//		private ActionFirer actionFirer;
		[Require] private PlayerActions.Writer PlayerActionsWriter;

		private void OnEnable()
		{
//			actionFirer = GetComponent<ActionFirer>();
			InvokeRepeating("KillBanana", 0, 5.0f);
		}

		private void OnDisable()
		{
			CancelInvoke("KillBanana");
		}

		private void KillBanana()
		{
			int newBananas = PlayerActionsWriter.Data.bananas - 1;

			if (newBananas <= 0) {
//				Debug.Log("Trying to kill player " + this.gameObject.EntityId());
				PlayerActionsWriter.Send(new PlayerActions.Update().AddSuicide(new Suicide()));
				PlayerActionsWriter.Send(new PlayerActions.Update().SetBananas(0));
				return;
			}

			PlayerActionsWriter.Send(new PlayerActions.Update().SetBananas(newBananas));
		}
	}


}
