using Assets.Gamelogic.EntityTemplates;
using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Entity.Component;
using Improbable.Core;
using Improbable.Unity.Core.EntityQueries;
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
    public class PlayerCreatingBehaviour : MonoBehaviour
    {
        [Require] private PlayerCreation.Writer PlayerCreationWriter;

        private void OnEnable()
        {
            PlayerCreationWriter.CommandReceiver.OnCreatePlayer.RegisterResponse(OnCreatePlayer);

			var update = new PlayerCreation.Update ();
			update.SetCanAddPlayers (true);
			PlayerCreationWriter.Send (update);
        }
			
        private void OnDisable()
        {
            PlayerCreationWriter.CommandReceiver.OnCreatePlayer.DeregisterResponse();
        }

        private CreatePlayerResponse OnCreatePlayer(CreatePlayerRequest request, ICommandCallerInfo callerinfo)
        {
            CreatePlayerWithReservedId(callerinfo.CallerWorkerId);
            return new CreatePlayerResponse();
        }

        private void CreatePlayerWithReservedId(string clientWorkerId)
        {
            SpatialOS.Commands.ReserveEntityId(PlayerCreationWriter)
                .OnSuccess(reservedEntityId => CreatePlayer(clientWorkerId, reservedEntityId.ReservedEntityId))
                .OnFailure(failure => OnFailedReservation(failure, clientWorkerId));
        }

        private void OnFailedReservation(ICommandErrorDetails response, string clientWorkerId)
        {
            Debug.LogError("Failed to Reserve EntityId for Player: " + response.ErrorMessage + ". Retrying...");
            CreatePlayerWithReservedId(clientWorkerId);
        }

        private void CreatePlayer(string clientWorkerId, EntityId entityId)
        {
            int numberOfPlayersConnected = PlayerCreationWriter.Data.numberOfPlayersConnected;
			bool canAddPlayers = PlayerCreationWriter.Data.canAddPlayers;

			if (canAddPlayers == false) {
                Debug.LogError("No more players can connect.");
            } else {
                var playerEntityTemplate = EntityTemplateFactory.CreatePlayerTemplate(clientWorkerId);

				SpatialOS.WorkerCommands.CreateEntity(entityId, playerEntityTemplate)
					.OnSuccess(result => OnSuccessfulPlayerCreation(numberOfPlayersConnected, entityId))
                    .OnFailure(failure => OnFailedPlayerCreation(failure, clientWorkerId, entityId));
			}
        }

        private void OnFailedPlayerCreation(ICommandErrorDetails response, string clientWorkerId, EntityId entityId)
        {
            Debug.LogError("Failed to Create Player Entity: " + response.ErrorMessage + ". Retrying...");
            CreatePlayer(clientWorkerId, entityId);
        }

		private void OnSuccessfulPlayerCreation(int numberOfPlayersConnected, EntityId entityId)
		{
			numberOfPlayersConnected = numberOfPlayersConnected + 1;

			var update = new PlayerCreation.Update();
			update.SetNumberOfPlayersConnected(numberOfPlayersConnected);
			update.SetNumberOfPlayersAlive (numberOfPlayersConnected);

			if (numberOfPlayersConnected == SimulationSettings.RequiredNumberOfPlayers) {
				var playerQuery = Query.HasComponent<Health>().ReturnOnlyEntityIds();
				SpatialOS.WorkerCommands.SendQuery (playerQuery)
					.OnSuccess (result => {
						var playerEntityIds = result.Entities.Keys.GetEnumerator();
						int randIndex = UnityEngine.Random.Range (0, numberOfPlayersConnected);
						EntityId killerId = new EntityId(0);

						int i=0;
						while(i<numberOfPlayersConnected) {
							EntityId curId = playerEntityIds.Current;
							if(curId.Id != 0) {
								if(i == randIndex) {
									killerId = curId;
									SendInitialInstructions("You are the rogue monkey", curId);
								} else {
									SendInitialInstructions("You are innocent", curId);
								}
								i++;
							}
							playerEntityIds.MoveNext();
						}
							
						update.SetKillerId (killerId);
						update.SetCanAddPlayers (false);
						PlayerCreationWriter.Send (update);
					})
					.OnFailure (response => {
						Debug.Log ("All players query failed.  Trying again...");
						SpatialOS.WorkerCommands.SendQuery (playerQuery);
				});
			} else {
				PlayerCreationWriter.Send (update);
			}
	    }

		private void SendInitialInstructions(string message, EntityId target)
		{
			SpatialOS.Commands.SendCommand (PlayerCreationWriter, Instructions.Commands.UpdateInstructions.Descriptor, new UpdateInstructionsRequest (message), target)
				.OnFailure (response => {
				Debug.Log ("UpdateInstructions failed with error message " + response.ErrorMessage);
				SendInitialInstructions (message, target);
			});
		}
	}
}
