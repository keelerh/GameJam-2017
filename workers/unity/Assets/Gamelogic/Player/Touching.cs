using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Improbable;
using Assets.Gamelogic.Core;
using UnityEngine.SceneManagement;
using Improbable.Unity.Core.EntityQueries;
using Improbable.Unity.Core;
using System.Collections;
using UnityEngine.UI;

 
namespace Assets.Gamelogic.Player
{
	[WorkerType(WorkerPlatform.UnityClient)]
    public class Touching : MonoBehaviour
    {
        [Require] private PlayerActions.Writer PlayerActionsWriter;

		public AudioClip chompSound;
		private AudioSource source;

		private void OnEnable()
		{
		source = GetComponent<AudioSource>();
		}

		private void OnCollisionEnter(Collision other)
        {

			if (other != null && other.gameObject.tag == "Banana")
			{   
				SpatialOS.Commands.DeleteEntity(PlayerActionsWriter, other.gameObject.EntityId())
					.OnSuccess(entityId => Destroy(other.gameObject))
					.OnFailure(errorDetails => Debug.LogWarning("Failed to delete entity with error: " + errorDetails.ErrorMessage));

				int newBananas = PlayerActionsWriter.Data.bananas + 1;
				PlayerActionsWriter.Send(new PlayerActions.Update().SetBananas(newBananas));
				source.PlayOneShot(chompSound,1f);
			}

            if (other != null && other.gameObject.tag == "Player")
            {   

                 var newMap = PlayerActionsWriter.Data.touchMap;
                 newMap[other.gameObject.EntityId()] = true;
                 PlayerActionsWriter.Send(new PlayerActions.Update().SetTouchMap(newMap));
            }
        }

		private void OnCollisionExit(Collision other)
		{

			if (other != null && other.gameObject.tag == "Player")
			{   
				 var newMap = PlayerActionsWriter.Data.touchMap;
				 newMap[other.gameObject.EntityId()] = false;
				 PlayerActionsWriter.Send(new PlayerActions.Update().SetTouchMap(newMap));
			}
		}
    }
}