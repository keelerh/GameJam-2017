using UnityEngine;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using Improbable.Player;
using Improbable;
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
using Assets.Gamelogic.EntityTemplates;
using Improbable.Core;

namespace Assets.Gamelogic.Core
{   
	[WorkerType(WorkerPlatform.UnityWorker)]
	public class BananaSpawner : MonoBehaviour
	{	
		[Require] private PlayerCreation.Writer HealthWriter;

		private void OnEnable()
		{
			InvokeRepeating("CreateBanana", 0, 1.3f);
		}

		private void OnDisable()
		{
			CancelInvoke("CreateBanana");
		}

		private void CreateBanana()
		{	
			Debug.Log("CREATING BANANA");
			var x = Random.Range(-35,30);
			var z = Random.Range(-35,30);
			var bananaCoordinates = new Vector3(x,0.3f,z);
//			Debug.LogError(HealthWriter.HasAuthority);
			SpatialOS.Commands.CreateEntity(HealthWriter, EntityTemplateFactory.CreateBananaTemplate(bananaCoordinates))
				.OnSuccess(entityId => Debug.Log("Created entity with ID: " + entityId))
				.OnFailure(errorDetails => Debug.LogError("Failed to create entity with error: " + errorDetails.ErrorMessage));
		}
	}


}
