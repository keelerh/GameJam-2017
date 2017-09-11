using Assets.Gamelogic.Core;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;
using UnityEngine.SceneManagement;
using Improbable.Unity.Core.EntityQueries;
using Improbable;
using Improbable.Unity.Core;

namespace Assets.Gamelogic.Player
{
    // Add this MonoBehaviour on client workers only
    [WorkerType(WorkerPlatform.UnityClient)]
    public class PlayerInputController : MonoBehaviour
    {        

        [Require] private PlayerActions.Writer PlayerActionsWriter;
        private ActionFirer actionFirer;

        void OnEnable()
        {
            actionFirer = GetComponent<ActionFirer>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {   
                Debug.LogWarning("Completed task!");

                PlayerActionsWriter.Send(new PlayerActions.Update().AddOperateAction(new OperateAction()));
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {   
                Debug.LogWarning("Tranquilized chimp!");

                PlayerActionsWriter.Send(new PlayerActions.Update().AddStab(new Stab()));
            }
        }
    }
}




