using UnityEngine;
using System;
using System.Collections;
using UnityRobot;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("UnityRobot")]
	[Tooltip("Play ToneModule")]
	public class ToneModulePlay : FsmStateAction
	{
		[Tooltip("Tone Frequency")]
		public FsmProperty toneFrequency;
		[Tooltip("Play Time(msec)")]
		public FsmInt durationTime;

		public override void Reset()
		{
			toneFrequency = new FsmProperty {setProperty = true};
		}
		
		public override void OnEnter()
		{
			base.OnEnter();

			ToneModule toneModule = (ToneModule)toneFrequency.TargetObject.Value;
			if(toneModule != null)
			{
				toneFrequency.SetValue();
				toneModule.Play(durationTime.Value);
			}
			
			Finish();
		}
	}
}
