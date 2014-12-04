using UnityEngine;
using System.Collections;
using System;
using UnityRobot;

namespace UnityRobot
{
	[AddComponentMenu("UnityRobot/AnalogPinDrag")]
	public class AnalogPinDrag : MonoBehaviour
	{
		public AnalogPin analogPin;
		[Range(0f, 1f)]
		public float dragMinRatio = 0.1f;
		[Range(0f, 1f)]
		public float dragMaxRatio = 0.9f;
		public float dragForceScaler = 1f;

		public EventHandler OnDragStart;
		public EventHandler OnDragMove;
		public EventHandler OnDragEnd;

		private bool _drag;
		private float _value;
		private float _preValue;
		private float _deltaTime;
		private float _preTime;

		// Use this for initialization
		void Start ()
		{
			_value = 0f;
			_preValue = _value;

			if(analogPin != null)
			{
				analogPin.OnStarted += OnStarted;
				analogPin.OnUpdated += OnUpdated;
				analogPin.OnStopped += OnStopped;
			}
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(analogPin != null)
			{
				if(analogPin.Started == true)
				{

				}
				else
				{
					if(_drag == true)
					{
						if(OnDragEnd != null)
							OnDragEnd(this, null);
					}
				}
			}		
		}

		private void OnStarted(object sender, EventArgs e)
		{
			_drag = false;
			_value = 0f;
			_preValue = _value;
		}

		private void OnUpdated(object sender, EventArgs e)
		{
			float curValue = analogPin.Value;
			if(_drag == true)
			{
				if(curValue > dragMinRatio && curValue < dragMaxRatio)
				{
					float time = Time.time;
					_deltaTime = time - _preTime;
					_preTime = time;
					_preValue = _value;
					_value = curValue;
					if(_value != _preValue)
					{
						if(OnDragMove != null)
							OnDragMove(this, null);
					}
				}
				else
				{
					_drag = false;
					if(OnDragEnd != null)
						OnDragEnd(this, null);
				}
			}
			else
			{
				if(curValue > dragMinRatio && curValue < dragMaxRatio)
				{
					_drag = true;
					_value = curValue;
					_preValue = _value;
					_preTime = Time.time;

					if(OnDragStart != null)
						OnDragStart(this, null);
				}
			}
		}

		private void OnStopped(object sender, EventArgs e)
		{
			_drag = false;
		}

		public bool isDragging
		{
			get
			{
				return _drag;
			}
		}

		public float Value
		{
			get
			{
				return _value;
			}
		}

		public float DragForce
		{
			get
			{
				float diff = _value - _preValue;
				float force = 0f;
				if(diff != 0f)
					force = diff / _deltaTime;
				return force * dragForceScaler;
			}
		}
	}
}
