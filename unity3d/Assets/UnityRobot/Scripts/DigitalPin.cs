using UnityEngine;
using System.Collections;
using System;

namespace UnityRobot
{
	[AddComponentMenu("UnityRobot/DigitalPin")]
	public class DigitalPin : UnityModule
	{
		public enum Mode
		{
			OUTPUT = 0,
			INPUT = 1,
			INPUT_PULLUP = 2,
			PWM = 3
		}

		public EventHandler OnChangedMode;
		public EventHandler OnChangedDigitalInput;

		[SerializeField]
		private Mode _mode;

		private byte _newValue;

		[SerializeField]
		private byte _value;

		protected override void OnAwake ()
		{
			if(_mode == Mode.OUTPUT)
				outputOnly = true;
			else if(_mode == Mode.INPUT)
				outputOnly = false;
			else if(_mode == Mode.INPUT_PULLUP)
				outputOnly = false;
			else if(_mode == Mode.PWM)
				outputOnly = true;
		}
		
		protected override void OnUpdate ()
		{
		}

		protected override void OnModuleStart ()
		{
			_newValue = _value;
		}

		protected override void OnModuleStop ()
		{
		}

		protected override void OnAction ()
		{
			if(_mode == Mode.INPUT || _mode == Mode.INPUT_PULLUP)
			{
				if(_newValue != _value)
				{
					_value = _newValue;
					if(OnChangedDigitalInput != null)
						OnChangedDigitalInput(this, null);
				}
			}
		}

		protected override void OnPop ()
		{
			if(_mode == Mode.INPUT || _mode == Mode.INPUT_PULLUP)
				Pop(ref _newValue);
		}
		
		protected override void OnPush ()
		{
			Push((byte)_mode);
			if(_mode == Mode.OUTPUT || _mode == Mode.PWM)
				Push(_value);
		}

		public Mode mode
		{
			get
			{
				return _mode;
			}
			set
			{
				if(_mode != value)
				{
					_mode = value;
					if(_mode == Mode.OUTPUT || _mode == Mode.PWM)
					{
						_newValue = _value;
						outputOnly = true;
					}
					else if(_mode == Mode.INPUT || _mode == Mode.INPUT_PULLUP)
					{
						outputOnly = false;
					}
					canUpdate = true;

					if(OnChangedMode != null)
						OnChangedMode(this, null);
				}
			}
		}

		public bool digitalValue
		{
			get
			{
				if(_value == 0)
					return false;
				else
					return true;
			}
			set
			{
				if(_mode == Mode.OUTPUT)
				{
					byte bValue = 0;
					if(value == true)
						bValue = 1;

					if(_value != bValue)
					{
						_value = bValue;
						canUpdate = true;
					}
				}
			}
		}

		public float analogValue
		{
			get
			{
				if(_value == 0)
					return 0;
				else
					return (float)_value / 255f;
			}
			set
			{
				if(_mode == Mode.PWM)
				{
					int iValue = (int)(value * 255f);
					iValue = Mathf.Clamp(iValue, 0, 255);
					if(_value != (byte)iValue)
					{
						_value = (byte)iValue;
						canUpdate = true;
					}
				}
			}
		}
	}
}
