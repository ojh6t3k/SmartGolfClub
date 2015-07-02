using UnityEngine;
using System.Collections;
using System;

namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Arduino/AppActions/AnalogInput")]
	public class AnalogInput : AppAction
	{
		public int pin;
		public int resolution = 1024;
		public bool filter = false;
		[Range(0f, 1f)]
		public float minValue = 0f;
		[Range(0f, 1f)]
		public float maxValue = 1f;
		public bool smooth = false;
		[Range(0f, 1f)]
		public float sensitivity = 0.5f;

		protected ushort _newValue;
		protected ushort _value;

		private float _sensitivity;
		private int _sampleNum = 100;
		private ArrayList _originValues = new ArrayList();
		private ArrayList _values = new ArrayList();
		// Kalman filter's parameter
		private float _Q;
		private float _R;
		private float _P;
		private float _X;
		private float _K;
		private float _originValue;
		private float _filterValue;


		void Awake()
		{
		}
		
		// Use this for initialization
		void Start ()
		{
			Reset();
		}
		
		// Update is called once per frame
		void Update ()
		{
			
		}

		public float Value
		{
			get
			{
				if(filter == true)
					return _filterValue;
				else
					return _originValue;
			}
		}

		public float MappingValue
		{
			get
			{
				if(filter == true)
					return _filterValue;
				else
					return _originValue;
			}
		}

		public float[] OriginValues
		{
			get
			{
				return (float[])_originValues.ToArray(typeof(float));
			}
		}
		
		public float[] FilterValues
		{
			get
			{
				return (float[])_values.ToArray(typeof(float));
			}
		}

		public override string SketchDeclaration()
		{
			return string.Format("{0} {1}({2:d}, A{3:d});", this.GetType().Name, SketchVarName, id, pin);
		}

		public override string SketchVarName
		{
			get
			{
				return string.Format("aInput{0:d}", id);
			}
		}

		protected override void OnActionStart ()
		{
			_newValue = _value;
			_originValue = (float)_value / (float)(resolution - 1);
		}
		
		protected override void OnActionStop ()
		{
			
		}
		
		protected override void OnActionExcute ()
		{
			if(_newValue != _value)
			{
				_value = _newValue;
				_originValue = (float)_value / (float)(resolution - 1);
			}

			if(filter == true)
			{
				if(_originValues.Count >= _sampleNum)
					_originValues.RemoveAt(0);
				if(_values.Count >= _sampleNum)
					_values.RemoveAt(0);

				_filterValue = Mathf.Clamp(_originValue, minValue, maxValue);
				_filterValue = (_filterValue - minValue) / (maxValue - minValue);

				if(smooth == true)
				{
					if(_sensitivity != sensitivity)
						FilterReset();

					_K = (_P + _Q) / (_P + _Q + _R);
					_P = _R * (_P + _Q) / (_R + _P + _Q);
					_filterValue = _X + (_filterValue - _X) * _K;
					_X = _filterValue;
				}
				else
					FilterReset();

				_originValues.Add(_originValue);
				_values.Add(_filterValue);
			}
		}
		
		protected override void OnPop ()
		{
			Pop(ref _newValue);
		}
		
		protected override void OnPush ()
		{
		}

		public override float signalValue
		{
			get
			{
				return Value;
			}
			set
			{
			}
		}

		public void Reset()
		{
			FilterReset();
			
			_originValues.Clear();
			
			_values.Clear();
			_values.Add(0f);
			_values.Add(0f);
			_values.Add(0f);
		}
		
		private void FilterReset()
		{
			_sensitivity = sensitivity;
			_Q = 0.00001f + (0.001f * sensitivity);
			_R = 0.01f;
			_P = 1f;
			_X = 0f;
			_K = 0f;
		}
	}
}
