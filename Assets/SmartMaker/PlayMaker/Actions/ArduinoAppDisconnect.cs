using UnityEngine;
using System.Collections;
using System;
using SmartMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("SmartMaker")]
	[Tooltip("ArduinoApp.Disconnect()")]
	public class ArduinoAppDisconnect : FsmStateAction
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
				arduinoApp.Disconnect();
			
			Finish();
		}
	}
}
