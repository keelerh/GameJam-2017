using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Assets.Gamelogic.Core;
using UnityEngine.SceneManagement;
 
namespace Assets.Gamelogic.Player
{
    // Add this MonoBehaviour on UnityWorker (server-side) workers only
    // [WorkerType(WorkerPlatform.UnityWorker)]
    public class Touching : MonoBehaviour
    {
        // Enable this MonoBehaviour only on the worker with write access for the entity's Health component
        // [Require] private Health.Writer HealthWriter;
        
		private void OnCollisionEnter(Collision other)
        {
            /*
             * Unity's OnTriggerEnter runs even if the MonoBehaviour is disabled, so non-authoritative UnityWorkers
             * must be protected against null writers
             */
            // if (HealthWriter == null)
            //     return;

            Debug.LogWarning("TOUCHING!!");

            if (other != null && other.gameObject.tag == "Player")
            {   
                Debug.LogWarning("TOUCHING PLAYER!");
            }
        }
    }
}