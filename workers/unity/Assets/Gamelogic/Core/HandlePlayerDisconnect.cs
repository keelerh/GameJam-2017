using Improbable;
using Improbable.Entity.Component;
using Improbable.Core;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;
using System;
using System.Collections;

namespace Assets.Gamelogic.Core
{
	[WorkerType(WorkerPlatform.UnityWorker)]
	public class HandlePlayerDisconnect : MonoBehaviour {
		[Require] private PlayerCreation.Writer playerCreationWriter;

		public void OnEnable() 
		{
			playerCreationWriter.CommandReceiver.OnHandlePlayerDisconnect.RegisterResponse (OnHandlePlayerDisconnect);
		}
		public void OnDisable()
		{
			playerCreationWriter.CommandReceiver.OnHandlePlayerDisconnect.DeregisterResponse ();
		}

		private HandlePlayerDisconnectResponse OnHandlePlayerDisconnect(HandlePlayerDisconnectRequest request,  ICommandCallerInfo callerinfo) {
			int numberOfPlayersConnected = playerCreationWriter.Data.numberOfPlayersConnected - 1;
			Debug.Log ("HANDLING A PLAYER DISCONNECT");
			var update = new PlayerCreation.Update ();
			update.SetNumberOfPlayersConnected (numberOfPlayersConnected);
			if (numberOfPlayersConnected == 0) {
				update.SetCanAddPlayers (true);
				update.SetNumberOfPlayersAlive(0);
			}
			playerCreationWriter.Send (update);

			return new HandlePlayerDisconnectResponse ();
		}
	}
}