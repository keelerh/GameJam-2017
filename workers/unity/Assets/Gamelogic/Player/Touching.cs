using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Improbable;
using Assets.Gamelogic.Core;
using UnityEngine.SceneManagement;
 
namespace Assets.Gamelogic.Player
{
    public class Touching : MonoBehaviour
    {
        [Require] private PlayerActions.Writer PlayerActionsWriter;

		private void OnCollisionEnter(Collision other)
        {
//            Debug.LogWarning("Touching!");

            if (other != null && other.gameObject.tag == "Player")
            {   
//                Debug.LogWarning("Touching Player!");

                 var newMap = PlayerActionsWriter.Data.touchMap;
                 newMap[other.gameObject.EntityId()] = true;
                 PlayerActionsWriter.Send(new PlayerActions.Update().SetTouchMap(newMap));
            }
        }

		private void OnCollisionExit(Collision other)
		{
//			Debug.LogWarning("Not Touching!");

			if (other != null && other.gameObject.tag == "Player")
			{   
//				Debug.LogWarning("Not Touching Player!");

				 var newMap = PlayerActionsWriter.Data.touchMap;
				 newMap[other.gameObject.EntityId()] = false;
				 PlayerActionsWriter.Send(new PlayerActions.Update().SetTouchMap(newMap));
			}
		}
    }
}