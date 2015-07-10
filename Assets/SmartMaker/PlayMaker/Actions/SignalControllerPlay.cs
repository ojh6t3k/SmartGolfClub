using UnityEngine;
using System.Collections;
using System;
using SmartMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("SmartMaker")]
	[Tooltip("SignalController.Play()")]
	public class SignalControllerPlay : FsmStateAction
	{
		[RequiredField]
		public SignalController signalController;
		public FsmInt index;
		public FsmFloat multiplier;
		public FsmFloat speed;
		public FsmBool loop;

		public override void Reset()
		{
			signalController = null;
			// default axis to variable dropdown with None selected.
			index = new FsmInt { UseVariable = true };
			multiplier = new FsmFloat { UseVariable = true };
			speed = new FsmFloat { UseVariable = true };
			loop = new FsmBool { UseVariable = true };
		}
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(signalController != null)
			{
				if(!index.IsNone)
					signalController.index = index.Value;

				if(!multiplier.IsNone)
					signalController.multiplier = multiplier.Value;

				if(!speed.IsNone)
					signalController.speed = speed.Value;

				if(!loop.IsNone)
					signalController.loop = loop.Value;

				signalController.Play();
			}
			
			Finish();
		}
	}
}
