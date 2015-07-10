using UnityEngine;
using System.Collections;
using System;
using SmartMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("SmartMaker")]
	[Tooltip("ArduinoApp.Connect()")]
	public class ArduinoAppConnect : FsmStateAction
	{
		[RequiredField]
		public ArduinoApp arduinoApp;

		public override void Reset()
		{
			arduinoApp = null;
		}
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(arduinoApp != null)
				arduinoApp.Connect();
			
			Finish();
		}
	}
}
