using UnityEngine;
using System.Collections;
using System;
using SmartGolf;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Smart Golf")]
	[Tooltip("BallSimulator.Play()")]
	public class BallSimulatorPlay : FsmStateAction
	{
		[RequiredField]
		public BallSimulator ballSimulator;
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(ballSimulator != null)
				ballSimulator.Play();
			
			Finish();
		}
	}
}
