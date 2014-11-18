using UnityEngine;
using System.Collections;
using System;

namespace UnityRobot
{
	[AddComponentMenu("UnityRobot/AnalogPin")]
	public class AnalogPin : UnityModule
	{
		public int maxValue = 1023;

		protected ushort _newValue;
		protected ushort _value;


		protected override void OnAwake ()
		{
			outputOnly = false;
		}

		protected override void OnUpdate ()
		{
		}

		protected override void OnModuleStart ()
		{
			_newValue = 0;
			_value = 0;
		}

		protected override void OnModuleStop ()
		{

		}

		protected override void OnAction ()
		{
			if(_newValue != _value)
			{
				_value = _newValue;
			}
		}

		protected override void OnPop ()
		{
			Pop(ref _newValue);
		}
		
		protected override void OnPush ()
		{
		}

		public int RawValue
		{
			get
			{
				return (int)_value;
			}
		}

		public float Value
		{
			get
			{
				return (float)_value / maxValue;
			}
		}
	}
}
