using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using HutongGames.PlayMaker;

namespace SmartGolf
{
	[AddComponentMenu("SmartGolf/PlayMaker/SwingMapperProxy")]
	public class SwingMapperProxy : MonoBehaviour
	{
		public SwingMapper swingMapper;
		
		public readonly string builtInOnReplayStarted = "SWING MAPPER / ON REPLAY STARTED";
		public readonly string builtInOnReplayStopped = "SWING MAPPER / ON REPLAY STOPPED";
		
		public string eventOnReplayStarted = "SWING MAPPER / ON REPLAY STARTED";
		public string eventOnReplayStopped = "SWING MAPPER / ON REPLAY STOPPED";
		
		private PlayMakerFSM _fsm;
		private FsmEventTarget _fsmEventTarget;
		
		// Use this for initialization
		void Start ()
		{
			_fsm = FindObjectOfType<PlayMakerFSM>();
			if(_fsm == null)
				_fsm = gameObject.AddComponent<PlayMakerFSM>();
			
			if(swingMapper != null)
			{
				swingMapper.OnReplayStarted.AddListener(OnReplayStarted);
				swingMapper.OnReplayStopped.AddListener(OnReplayStopped);
			}
			
			_fsmEventTarget = new FsmEventTarget();
			_fsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
			_fsmEventTarget.excludeSelf = false;
		}
		
		// Update is called once per frame
		void Update ()
		{
			
		}
		
		private void OnReplayStarted()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnReplayStarted);
		}
		
		private void OnReplayStopped()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnReplayStopped);
		}
	}
}
