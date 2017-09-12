using Assets.Gamelogic.Core;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;
using UnityEngine.SceneManagement;
using Improbable.Unity.Core.EntityQueries;
using Improbable;
using Improbable.Unity.Core;
using System.Collections;

namespace Assets.Gamelogic.Player
{
    // Add this MonoBehaviour on client workers only
    [WorkerType(WorkerPlatform.UnityClient)]
    public class PlayerInputController : MonoBehaviour
    {        

        [Require] private PlayerActions.Writer PlayerActionsWriter;
        private ActionFirer actionFirer;
		public AudioClip stabSound;
		public AudioClip operateSound;
        private AudioSource source;
		private TextMesh textMesh;

        void OnEnable()
        {
            actionFirer = GetComponent<ActionFirer>();
			SceneManager.UnloadSceneAsync(BuildSettings.SplashScreenScene);
			source = GetComponent<AudioSource>();

			textMesh = GetComponent<TextMesh>();//
			textMesh.text = "meow meow meow";
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {   
//                Debug.LogWarning("Completed task!");

                PlayerActionsWriter.Send(new PlayerActions.Update().AddOperateAction(new OperateAction()));
				source.PlayOneShot(operateSound,1f);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {   
//                Debug.LogWarning("Tranquilized chimp!");
                source.PlayOneShot(stabSound,1f);
                PlayerActionsWriter.Send(new PlayerActions.Update().AddStab(new Stab()));
            }
        }
    }
}




