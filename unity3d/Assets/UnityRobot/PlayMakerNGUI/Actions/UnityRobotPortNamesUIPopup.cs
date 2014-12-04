using UnityEngine;
using System;
using System.Collections;
using UnityRobot;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("UnityRobot")]
	[Tooltip("Connect to UnityRobot")]
	public class UnityRobotPortNamesUIPopup : FsmStateAction
	{
		[RequiredField]
		public UnityRobot.UnityRobot unityRobot;
		[RequiredField]
		public UIPopupList uiPopupList;

		public override void OnEnter()
		{
			base.OnEnter();

			if(unityRobot != null && uiPopupList != null)
			{
				uiPopupList.Clear();
				if(unityRobot.portNames.Count > 0)
				{
					string currentPort = unityRobot.portName;
					int index = -1;
					for(int i=0; i<unityRobot.portNames.Count; i++)
					{
						string portName = unityRobot.portNames[i];
						uiPopupList.AddItem(portName);
						if(currentPort.Equals(portName) == true)
							index = i;
					}
					if(index >= 0)
						uiPopupList.value = currentPort;
					else
						uiPopupList.value = unityRobot.portNames[0];
				}
			}
			
			Finish();
		}
	}
}
