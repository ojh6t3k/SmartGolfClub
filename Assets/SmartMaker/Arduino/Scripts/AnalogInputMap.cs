using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Arduino/Utility/AnalogInputMap")]
	public class AnalogInputMap : AppActionUtil
	{
		public AnalogInput analogInput;
		public float multiplier = 1f;
		public AnimationCurve mapCurve;

		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
		
		}
		
		public float Value
		{
			get
			{
				if(analogInput == null)
					return 0f;

				return mapCurve.Evaluate(analogInput.Value) * multiplier;
			}
		}
	}
}
