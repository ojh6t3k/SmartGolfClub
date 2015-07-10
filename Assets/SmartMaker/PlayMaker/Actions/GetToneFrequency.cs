using UnityEngine;
using System.Collections;
using System;
using SmartMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("SmartMaker")]
	[Tooltip("Get ToneFrequency")]
	public class GetToneFrequency : FsmStateAction
	{
		public ToneFrequency toneFrequency;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storedValue;

		public override void Reset()
		{
			storedValue = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			
			if(storedValue != null)
			{
				storedValue.Value = (float)toneFrequency;
			}
			
			Finish();
		}
	}
}
