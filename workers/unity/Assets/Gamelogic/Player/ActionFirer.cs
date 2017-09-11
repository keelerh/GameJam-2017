using UnityEngine;
using Improbable.Player;
using Improbable.Unity.Visualizer;
using Improbable.Unity;
using Assets.Gamelogic.Core;
using UnityEngine.SceneManagement;


namespace Assets.Gamelogic.Player
{
    // This MonoBehaviour will be enabled on both client and server-side workers
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class ActionFirer : MonoBehaviour
    {
        [Require] private PlayerActions.Reader PlayerActionsReader;
        [Require] private Health.Writer HealthWriter;

        private void Start()
        {
            // actionFirer = gameObject.GetComponent<ActionFirer>();
        }
        	
        private void OnEnable()
        {
            PlayerActionsReader.StabTriggered.Add(OnStab);
            PlayerActionsReader.OperateActionTriggered.Add(OnOperateAction);
        }

        private void OnDisable()
        {
            PlayerActionsReader.StabTriggered.Remove(OnStab);
            PlayerActionsReader.OperateActionTriggered.Remove(OnOperateAction);
        }

        private void OnStab(Stab stab)
        {   
            Debug.LogWarning("Stab action fired!");
            int newHealth = HealthWriter.Data.currentHealth - 250;
            HealthWriter.Send(new Health.Update().SetCurrentHealth(newHealth));
        }

        private void OnOperateAction(OperateAction operateAction)
        {   
            Debug.LogWarning("Operate action fired!");
        }

        // public void AttemptToStab(Vector3 direction)
        // {
        // }
    }
}