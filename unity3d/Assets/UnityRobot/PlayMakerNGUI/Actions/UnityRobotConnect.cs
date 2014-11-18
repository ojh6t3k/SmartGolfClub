using UnityEngine;
using System;
using System.Collections;
using UnityRobot;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("UnityRobot")]
	[Tooltip("Connect to UnityRobot")]
	public class UnityRobotConnect : FsmStateAction
	{
		[RequiredField]
		public UnityRobot.UnityRobot unityRobot;

		public override void OnEnter()
		{
			base.OnEnter();

			if(unityRobot != null)
				unityRobot.Connect();

			Finish();
		}
	}
}
