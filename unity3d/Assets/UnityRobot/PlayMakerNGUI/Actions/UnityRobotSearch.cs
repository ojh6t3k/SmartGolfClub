using UnityEngine;
using System;
using System.Collections;
using UnityRobot;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("UnityRobot")]
	[Tooltip("Search enable ports")]
	public class UnityRobotSearch : FsmStateAction
	{
		[RequiredField]
		public UnityRobot.UnityRobot unityRobot;

		public override void OnEnter()
		{
			base.OnEnter();

			if(unityRobot != null)
				unityRobot.PortSearch();

			Finish();
		}
	}
}
