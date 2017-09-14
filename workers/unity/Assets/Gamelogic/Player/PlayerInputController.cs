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
using UnityEngine.UI;


namespace Assets.Gamelogic.Player
{
    // Add this MonoBehaviour on client workers only
    [WorkerType(WorkerPlatform.UnityClient)]
    public class PlayerInputController : MonoBehaviour
    {        

        [Require] private PlayerActions.Writer PlayerActionsWriter;
		[Require] private Health.Reader HealthReader;

		public AudioClip stabSound;
		public AudioClip operateSound;
        private AudioSource source;


		private Canvas scoreCanvasUI;
		private Text totalPointsGUI;

        void OnEnable()
        {
			SceneManager.UnloadSceneAsync(BuildSettings.SplashScreenScene);
			source = GetComponent<AudioSource>();

			GameObject tempObj = GameObject.Find("ScoreCanvas");
			scoreCanvasUI = tempObj.GetComponent<Canvas>();

			if (scoreCanvasUI != null) {
				totalPointsGUI = scoreCanvasUI.GetComponentInChildren<Text>();
			}
				
        }

        void Update()
        {	

			if (HealthReader.Data.currentHealth <= 0) {
				return;
			}

            if (Input.GetKeyDown(KeyCode.E))
            {   
                PlayerActionsWriter.Send(new PlayerActions.Update().AddOperateAction(new OperateAction()));
				source.PlayOneShot(operateSound,1f);
//				totalPointsGUI.text = "you are the murderer";
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {   
                source.PlayOneShot(stabSound,1f);
                PlayerActionsWriter.Send(new PlayerActions.Update().AddStab(new Stab()));
            }
        }
    }
}