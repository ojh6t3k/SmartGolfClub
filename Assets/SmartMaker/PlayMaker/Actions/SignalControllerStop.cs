using UnityEngine;
using System.Collections;
using System;
using SmartMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("SmartMaker")]
	[Tooltip("SignalController.Stop()")]
	public class SignalControllerStop : FsmStateAction
	{
		[RequiredField]
		public SignalController signalController;

		public override void Reset()
		{
			signalController = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			
			if(signalController != null)
				signalController.Stop();
			
			Finish();
		}
	}
}
