using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using HutongGames.PlayMaker;

namespace SmartMaker.PlayMaker
{
	[AddComponentMenu("SmartMaker/PlayMaker/ArduinoAppProxy")]
	public class ArduinoAppProxy : MonoBehaviour
	{
		public readonly string builtInOnConnected = "ARDUINO APP / ON CONNECTED";
		public readonly string builtInOnConnectionFailed = "ARDUINO APP / ON CONNECTION FAILED";
		public readonly string builtInOnDisconnected = "ARDUINO APP / ON DISCONNECTED";

		public string eventOnConnected = "ARDUINO APP / ON CONNECTED";
		public string eventOnConnectionFailed = "ARDUINO APP / ON CONNECTION FAILED";
		public string eventOnDisconnected = "ARDUINO APP / ON DISCONNECTED";

		private PlayMakerFSM _fsm;
		private ArduinoApp _arduinoApp;
		private FsmEventTarget _fsmEventTarget;

		// Use this for initialization
		void Start ()
		{
			_fsm = FindObjectOfType<PlayMakerFSM>();
			if(_fsm == null)
				_fsm = gameObject.AddComponent<PlayMakerFSM>();

			_arduinoApp = GetComponent<ArduinoApp>();
			if(_arduinoApp != null)
			{
				_arduinoApp.OnConnected.AddListener(OnConnected);
				_arduinoApp.OnConnectionFailed.AddListener(OnConnectionFailed);
				_arduinoApp.OnDisconnected.AddListener(OnDisconnected);
			}

			_fsmEventTarget = new FsmEventTarget();
			_fsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
			_fsmEventTarget.excludeSelf = false;
		}
		
		// Update is called once per frame
		void Update ()
		{
		
		}

		private void OnConnected()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnConnected);
		}

		private void OnConnectionFailed()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnConnectionFailed);
		}

		private void OnDisconnected()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnDisconnected);
		}
	}
}
