using UnityEngine;
using System;
using System.Collections;
using UnityRobot;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("UnityRobot")]
	[Tooltip("Listen Event from UnityRobot")]
	public class UnityRobotEvent : FsmStateAction
	{
		[Tooltip("Connected Event")]
		public FsmEvent connectedEvent;
		[Tooltip("Connection Failed Event")]
		public FsmEvent connectionFailedEvent;
		[Tooltip("Disconnected Event")]
		public FsmEvent disconnectedEvent;
		[Tooltip("Search Completed Event")]
		public FsmEvent searchCompletedEvent;
		[Tooltip("Updated Event")]
		public FsmEvent updatedEvent;

		[Tooltip("Event Target FSM")]
		public PlayMakerFSM targetFSM;

		private UnityRobot.UnityRobot _unityRobot;

		public override void Awake ()
		{
			base.Awake ();

			_unityRobot = Owner.GetComponent<UnityRobot.UnityRobot>();
			if(_unityRobot == null)
				Debug.LogWarning("There exist no UnityRobot!");
			else
			{
				_unityRobot.OnConnected += OnConnected;
				_unityRobot.OnConnectionFailed += OnConnectionFailed;
				_unityRobot.OnDisconnected += OnDisconnected;
				_unityRobot.OnSearchCompleted += OnSearchCompleted;
				_unityRobot.OnUpdated += OnUpdated;				
			}
		}

		public override void OnEnter()
		{
			base.OnEnter();
		}

		public override void OnExit ()
		{
			base.OnExit ();
		}

		void SendEvent(FsmEvent fsmEvent)
		{
			if(this.Entered == true)
			{
				if(targetFSM == null)
					Fsm.BroadcastEvent(fsmEvent);
				else
					targetFSM.Fsm.Event(fsmEvent);
				
				Fsm.Event(fsmEvent);
			}
		}
		
		void OnConnected(object sender, EventArgs e)
		{
			SendEvent(connectedEvent);
		}

		void OnConnectionFailed(object sender, EventArgs e)
		{
			SendEvent(connectionFailedEvent);
		}

		void OnDisconnected(object sender, EventArgs e)
		{
			SendEvent(disconnectedEvent);
		}

		void OnSearchCompleted(object sender, EventArgs e)
		{
			SendEvent(searchCompletedEvent);
		}

		void OnUpdated(object sender, EventArgs e)
		{
			SendEvent(updatedEvent);
		}
	}
}
