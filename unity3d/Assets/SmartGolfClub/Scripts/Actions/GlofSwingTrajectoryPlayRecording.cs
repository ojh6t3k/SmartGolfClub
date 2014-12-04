using UnityEngine;
using System;
using System.Collections;
using UnityRobot;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Smart Golf")]
	[Tooltip("Play Recording")]
	public class GolfSwingTrajectoryPlayRecording : FsmStateAction
	{
		[RequiredField]
		public GolfSwingTrajectory golfSwingTrajectory;
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(golfSwingTrajectory != null)
			{
				golfSwingTrajectory.PlayRecording();
			}
			
			Finish();
		}
	}
}
