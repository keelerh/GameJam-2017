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
 
namespace Assets.Gamelogic.Player
{
    // Add this MonoBehaviour on client workers only
    [WorkerType(WorkerPlatform.UnityClient)]
    public class DeathBehaviour : MonoBehaviour
    {
        // Inject access to the entity's Health component
        [Require] private Health.Reader HealthReader;
		// also need an auxilary writer to uniquely identify the worker when it issues commands
		[Require] private PlayerActions.Writer PlayerActionsWriter;
 
        private bool alreadyDead = false;
 
        private void OnEnable()
        {
            alreadyDead = false;
            // Register callback for when components change
            HealthReader.CurrentHealthUpdated.Add(OnCurrentHealthUpdated);
        }
 
        private void OnDisable()
        {
            // Deregister callback for when components change
            HealthReader.CurrentHealthUpdated.Remove(OnCurrentHealthUpdated);
        }
 
        // Callback for whenever the CurrentHealth property of the Health component is updated
        private void OnCurrentHealthUpdated(int currentHealth)
        {
            if (!alreadyDead && currentHealth <= 0)
			{
				EntityId dyingEntityId = gameObject.EntityId();
				FindPlayerCreatorAndSendHandlePlayerDeathRequest (dyingEntityId);
                alreadyDead = true;
            }
        }

		private void FindPlayerCreatorAndSendHandlePlayerDeathRequest(EntityId dyingEntityId) {
			var playerCreatorQuery = Query.HasComponent<PlayerCreation>().ReturnOnlyEntityIds();
			SpatialOS.WorkerCommands.SendQuery(playerCreatorQuery)
				.OnSuccess(result => {
					if (result.EntityCount < 1)
					{
						Debug.LogError("Failed to find PlayerCreator. SpatialOS probably hadn't finished loading the initial snapshot. Try again in a few seconds.");
						return;
					}
					var playerCreatorEntity = result.Entities.First.Value.Key;
					SpatialOS.Commands.SendCommand (PlayerActionsWriter, PlayerCreation.Commands.HandlePlayerDeath.Descriptor, new HandlePlayerDeathRequest (dyingEntityId), playerCreatorEntity)
						.OnFailure(response => FindPlayerCreatorAndSendHandlePlayerDeathRequest(dyingEntityId));
				})
				.OnFailure(response => FindPlayerCreatorAndSendHandlePlayerDeathRequest(dyingEntityId));
		}
   }
}