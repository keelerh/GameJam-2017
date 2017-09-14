using Assets.Gamelogic.EntityTemplates;
using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Entity.Component;
using Improbable.Core;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Core.EntityQueries;
using Improbable.Unity.Visualizer;
using UnityEngine;
using System;
using System.Collections;

namespace Assets.Gamelogic.Core
{
	[WorkerType(WorkerPlatform.UnityWorker)]
	public class HandlePlayerDeath : MonoBehaviour {
		[Require] private PlayerCreation.Writer playerCreationWriter;

		public void OnEnable() 
		{
			playerCreationWriter.CommandReceiver.OnHandlePlayerDeath.RegisterResponse (OnHandlePlayerDeath);
		}
		public void OnDisable()
		{
			playerCreationWriter.CommandReceiver.OnHandlePlayerDeath.DeregisterResponse ();
		}

		private HandlePlayerDeathResponse OnHandlePlayerDeath (HandlePlayerDeathRequest request, ICommandCallerInfo callerinfo)
		{
			EntityId dyingId = request.dyingPlayerId;
			EntityId killerId = playerCreationWriter.Data.killerId;
			int numberOfPlayersAlive = playerCreationWriter.Data.numberOfPlayersAlive - 1;

			if (dyingId.Equals(killerId)) {
				var playerQuery = Query.HasComponent<Health>().ReturnOnlyEntityIds();
				SpatialOS.WorkerCommands.SendQuery (playerQuery)
					.OnSuccess (result => {
						var playerEntityIds = result.Entities.Keys.GetEnumerator();

						int i=0;
						while(i<numberOfPlayersAlive) {
							EntityId curId = playerEntityIds.Current;
							if(curId.Id != 0) {
								if(curId==killerId) {
									SendDeathCommand("You have been rekt.", curId);
								} else {
									SendDeathCommand("The rogue monkey has been rekt.  You win!", curId);
								}
								i++;
							}
							playerEntityIds.MoveNext();
						}
					})
					.OnFailure (response => {
						Debug.Log ("All players query failed.  Trying again...");
						OnHandlePlayerDeath(request, callerinfo);
					});
			} else {
				SendDeathCommand ("You have been rekt.", dyingId);
				if (numberOfPlayersAlive == 1) {
					SendDeathCommand ("All the other monkeys are rekt.  You win!", killerId);
				}
			}

			var update = new PlayerCreation.Update ();
			update.SetNumberOfPlayersAlive (numberOfPlayersAlive);
			playerCreationWriter.Send (update);

			return new HandlePlayerDeathResponse();
		}

		private void SendDeathCommand(string message, EntityId target)
		{
			SpatialOS.Commands.SendCommand (playerCreationWriter, Instructions.Commands.UpdateInstructions.Descriptor, new UpdateInstructionsRequest (message), target)
				.OnFailure (response => {
					Debug.Log ("UpdateInstructions failed with error message " + response.ErrorMessage);
					SendDeathCommand(message, target);
				});			
		}
	}
}
