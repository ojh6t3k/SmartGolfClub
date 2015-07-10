using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using HutongGames.PlayMaker;

namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/PlayMaker/AppActionProxy")]
	public class AppActionProxy : MonoBehaviour
	{
		public readonly string builtInOnStarted = "APP ACTION / ON STARTED";
		public readonly string builtInOnExcuted = "APP ACTION / ON EXCUTED";
		public readonly string builtInOnStopped = "APP ACTION / ON STOPPED";
		
		public string eventOnStarted = "APP ACTION / ON STARTED";
		public string eventOnExcuted = "APP ACTION / ON EXCUTED";
		public string eventOnStopped = "APP ACTION / ON STOPPED";
		
		private PlayMakerFSM _fsm;
		private AppAction _appAction;
		private FsmEventTarget _fsmEventTarget;
		
		// Use this for initialization
		void Start ()
		{
			_fsm = FindObjectOfType<PlayMakerFSM>();
			if(_fsm == null)
				_fsm = gameObject.AddComponent<PlayMakerFSM>();
			
			_appAction = GetComponent<AppAction>();
			if(_appAction != null)
			{
				_appAction.OnStarted.AddListener(OnStarted);
				_appAction.OnExcuted.AddListener(OnExcuted);
				_appAction.OnStopped.AddListener(OnStopped);
			}
			
			_fsmEventTarget = new FsmEventTarget();
			_fsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
			_fsmEventTarget.excludeSelf = false;
		}
		
		// Update is called once per frame
		void Update ()
		{
			
		}
		
		private void OnStarted()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnStarted);
		}
		
		private void OnExcuted()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnExcuted);
		}
		
		private void OnStopped()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnStopped);
		}
	}
}
