using Assets.Gamelogic.EntityTemplates;
using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Entity.Component;
using Improbable.Core;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;
using System;
using System.Collections;

namespace Assets.Gamelogic.Player
{
	[WorkerType(WorkerPlatform.UnityWorker)]
	public class HandleInstructionUpdate : MonoBehaviour {
		[Require] private Instructions.Writer instructionsWriter;

		public void OnEnable() 
		{
			instructionsWriter.CommandReceiver.OnUpdateInstructions.RegisterResponse (OnUpdateInstructions);
		}
		public void OnDisable()
		{
			instructionsWriter.CommandReceiver.OnUpdateInstructions.DeregisterResponse ();
		}

		private UpdateInstructionsResponse OnUpdateInstructions(UpdateInstructionsRequest request, ICommandCallerInfo callerinfo)
		{
			Debug.Log ("Going to handle the update instructions request");
			Debug.Log (request.instructionDescription);

			var update = new Instructions.Update();
			update.SetInstructionDescription(request.instructionDescription);
			instructionsWriter.Send(update);

			return new UpdateInstructionsResponse();
		}
	}
}