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

        private ActionFirer actionFirer;
		public AudioClip stabSound;
		public AudioClip operateSound;
        private AudioSource source;


		private Canvas scoreCanvasUI;
		private Text totalPointsGUI;
//		private Text playerStatus;


//		public GameObject text;

        void OnEnable()
        {
            actionFirer = GetComponent<ActionFirer>();
			SceneManager.UnloadSceneAsync(BuildSettings.SplashScreenScene);
			source = GetComponent<AudioSource>();

			GameObject tempObj = GameObject.Find("ScoreCanvas");
			scoreCanvasUI = tempObj.GetComponent<Canvas>();

			if (scoreCanvasUI != null) {
//				Debug.LogWarning("We got here");
				totalPointsGUI = scoreCanvasUI.GetComponentInChildren<Text>();
//				scoreCanvasUI.enabled = false;
//				updateGUI(0);
			}
//			playerStatus = this.gameObject.GetComponentInChildren<Text>();
//			playerStatus.text = "player_" + this.gameObject.EntityId().ToString();

        }

        void Update()
        {	

			if (HealthReader.Data.currentHealth <= 0) {
				return;
			}

            if (Input.GetKeyDown(KeyCode.E))
            {   
//                Debug.LogWarning("Completed task!");
                PlayerActionsWriter.Send(new PlayerActions.Update().AddOperateAction(new OperateAction()));
				source.PlayOneShot(operateSound,1f);
				totalPointsGUI.text = "you are the murderer";
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




