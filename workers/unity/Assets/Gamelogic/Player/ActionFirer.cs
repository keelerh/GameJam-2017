using UnityEngine;
using Improbable.Core;
using Improbable.Player;
using Improbable.Unity.Visualizer;
using Improbable.Unity;
using Improbable.Unity.Core;
using Assets.Gamelogic.Core;
using UnityEngine.SceneManagement;
using Improbable.Entity.Component;
using Improbable;

namespace Assets.Gamelogic.Player
{
    // This MonoBehaviour will be enabled on server-side workers
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class ActionFirer : MonoBehaviour
    {
		[Require] private PlayerActions.Reader PlayerActionsReader;
        [Require] private Health.Writer HealthWriter;
//		[Require] private PlayerCreation.Reader PlayerCreationReader;
        	
        private void OnEnable()
        {
            PlayerActionsReader.StabTriggered.Add(OnStab);
            PlayerActionsReader.OperateActionTriggered.Add(OnOperateAction);
			PlayerActionsReader.SuicideTriggered.Add(OnSuicide);
			HealthWriter.CommandReceiver.OnTakeDamage.RegisterResponse(TakeDamage);
        }

        private void OnDisable()
        {
            PlayerActionsReader.StabTriggered.Remove(OnStab);
            PlayerActionsReader.OperateActionTriggered.Remove(OnOperateAction);
			PlayerActionsReader.SuicideTriggered.Remove(OnSuicide);
			HealthWriter.CommandReceiver.OnTakeDamage.DeregisterResponse();
        }

        private void OnStab(Stab stab)
        {   
			var touchMap = PlayerActionsReader.Data.touchMap;
			foreach(var item in touchMap)
			{
				if (item.Value) {
					AttackPlayer(item.Key);
				}
			}
        }

		private void OnSuicide(Suicide suicide)
		{   
			int newHealth = HealthWriter.Data.currentHealth - 1000;
			HealthWriter.Send(new Health.Update().SetCurrentHealth(newHealth));
		}

        private void OnOperateAction(OperateAction operateAction)
        {   
//            Debug.LogWarning("Operate action fired!");
        }

		private DamageResponse TakeDamage(DamageRequest request, ICommandCallerInfo callerInfo)
		{

			int newHealth = HealthWriter.Data.currentHealth - 1000;
			HealthWriter.Send(new Health.Update().SetCurrentHealth(newHealth));

			return new DamageResponse(1000);
		}


		public void AttackPlayer(EntityId playerEntityId) 
		{	
			EntityId playerId = gameObject.EntityId();
			EntityId killerId = SpatialOS.GetLocalEntityComponent<PlayerCreation>(new EntityId(1)).Get().Value.killerId;
//			long playerEntityIdCopy = playerEntityId.Id;

			SpatialOS.Commands.SendCommand(HealthWriter, Health.Commands.TakeDamage.Descriptor, new DamageRequest(1000), playerEntityId)
				.OnSuccess(response => {
					if(playerId != killerId && playerEntityId != killerId) {
						HealthWriter.Send(new Health.Update().SetCurrentHealth(-10));
					}
				})
				.OnFailure(OnDamageRequestFailure);
		}

		void OnDamageRequestSuccess(DamageResponse response)
		{
			Debug.Log("Take damage command succeeded; dealt damage: " + response.dealtDamage);
		}

		void OnDamageRequestFailure(ICommandErrorDetails response)
		{
			Debug.LogError("Failed to send take damage command with error: " + response.ErrorMessage);
		}
    }
}