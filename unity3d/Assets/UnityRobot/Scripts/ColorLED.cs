using UnityEngine;
using System.Collections;
using System;

namespace UnityRobot
{
	[AddComponentMenu("UnityRobot/ColorLED")]
	public class ColorLED : MonoBehaviour
	{
		public DigitalPin pwmRed;
		public DigitalPin pwmGreen;
		public DigitalPin pwmBlue;

		private float _calibrationRed = 0f;
		private float _calibrationGreen = 0f;
		private float _calibrationBlue = 0f;
		private Color _color;

		void Awake()
		{
			if(pwmRed != null)
				pwmRed.OnStarted += OnStartedRed;
			if(pwmGreen != null)
				pwmGreen.OnStarted += OnStartedGreen;
			if(pwmBlue != null)
				pwmBlue.OnStarted += OnStartedBlue;
		}

		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
		
		}

		public Color color
		{
			get
			{
				return _color;
			}
			set
			{
				if(_color != value)
				{
					_color = value;
					DoLEDControl();
				}
			}
		}

		public float calibrationRed
		{
			get
			{
				return _calibrationRed;
			}
			set
			{
				float newCalibration = Mathf.Clamp(value, -1f, 1f);
				if(_calibrationRed != newCalibration)
				{
					_calibrationRed = newCalibration;
					DoLEDControl();
				}
			}
		}

		public float calibrationGreen
		{
			get
			{
				return _calibrationGreen;
			}
			set
			{
				float newCalibration = Mathf.Clamp(value, -1f, 1f);
				if(_calibrationGreen != newCalibration)
				{
					_calibrationGreen = newCalibration;
					DoLEDControl();
				}
			}
		}

		public float calibrationBlue
		{
			get
			{
				return _calibrationBlue;
			}
			set
			{
				float newCalibration = Mathf.Clamp(value, -1f, 1f);
				if(_calibrationBlue != newCalibration)
				{
					_calibrationBlue = newCalibration;
					DoLEDControl();
				}
			}
		}

		private void DoLEDControl()
		{
			if(pwmRed != null)
			{
				if(pwmRed.Started == true)
					pwmRed.analogValue = _color.r + calibrationRed;
			}
			if(pwmGreen != null)
			{
				if(pwmGreen.Started == true)
					pwmGreen.analogValue = _color.g + calibrationGreen;
			}
			if(pwmBlue != null)
			{
				if(pwmBlue.Started == true)
					pwmBlue.analogValue = _color.b + calibrationBlue;
			}
		}

		private void OnStartedRed(object sender, EventArgs e)
		{
			DoLEDControl();
		}

		private void OnStartedGreen(object sender, EventArgs e)
		{
			DoLEDControl();
		}

		private void OnStartedBlue(object sender, EventArgs e)
		{
			DoLEDControl();
		}
	}
}