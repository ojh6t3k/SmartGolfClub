using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using HutongGames.PlayMaker;

namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/PlayMaker/AnalogInputDragProxy")]
	public class AnalogInputDragProxy : MonoBehaviour
	{
		public AnalogInputDrag analogInputDrag;

		public readonly string builtInOnDragStart = "ANALOG INPUT DRAG / ON DRAG START";
		public readonly string builtInOnDragMove = "ANALOG INPUT DRAG / ON DRAG MOVE";
		public readonly string builtInOnDragEnd = "ANALOG INPUT DRAG / ON DRAG END";
		
		public string eventOnDragStart = "ANALOG INPUT DRAG / ON DRAG START";
		public string eventOnDragMove = "ANALOG INPUT DRAG / ON DRAG MOVE";
		public string eventOnDragEnd = "ANALOG INPUT DRAG / ON DRAG END";
		
		private PlayMakerFSM _fsm;
		private FsmEventTarget _fsmEventTarget;
		
		// Use this for initialization
		void Start ()
		{
			_fsm = FindObjectOfType<PlayMakerFSM>();
			if(_fsm == null)
				_fsm = gameObject.AddComponent<PlayMakerFSM>();
			
			if(analogInputDrag != null)
			{
				analogInputDrag.OnDragStart.AddListener(OnDragStart);
				analogInputDrag.OnDragMove.AddListener(OnDragMove);
				analogInputDrag.OnDragEnd.AddListener(OnDragEnd);
			}
			
			_fsmEventTarget = new FsmEventTarget();
			_fsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
			_fsmEventTarget.excludeSelf = false;
		}
		
		// Update is called once per frame
		void Update ()
		{
			
		}
		
		private void OnDragStart()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnDragStart);
		}
		
		private void OnDragMove()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnDragMove);
		}
		
		private void OnDragEnd()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnDragEnd);
		}
	}
}
