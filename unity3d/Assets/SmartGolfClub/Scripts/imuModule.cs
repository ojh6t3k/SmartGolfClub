using UnityEngine;
using System.Collections;
using System;

namespace UnityRobot
{
	public class imuModule : UnityModule
	{
		public GameObject target;
		public Vector3 offsetAngle = Vector3.zero;

		private Quaternion _targetStartRotation;
		private Quaternion _rotation;
		private Quaternion _clibRotation;
		private Quaternion _curRotation;
		private Quaternion _fromRotation;
		private Quaternion _toRotation;
		private float _time;
		private float _refTime;

		protected short _qX;
		protected short _qY;
		protected short _qZ;
		protected short _qW;
		protected ushort _intervalTime;
		
		
		protected override void OnAwake ()
		{
			outputOnly = false;

			if(target != null)
				_targetStartRotation = target.transform.localRotation;
			else
				_targetStartRotation = Quaternion.identity;
			_clibRotation = Quaternion.identity;
			_curRotation = Quaternion.identity;
			_toRotation = Quaternion.identity;
			_time = 1f;
		}
		
		protected override void OnUpdate ()
		{
			if(_time < 1f)
			{
				_time += (Time.deltaTime / _refTime);
				_curRotation = Quaternion.Lerp(_fromRotation, _toRotation, _time);
			}
			else
				_curRotation = _toRotation;

			if(target != null)
				target.transform.localRotation = _targetStartRotation * _curRotation;
		}
		
		protected override void OnModuleStart ()
		{
			_clibRotation = Quaternion.identity;
			_curRotation = Quaternion.identity;
			_toRotation = Quaternion.identity;
			_time = 1f;
		}
		
		protected override void OnModuleStop ()
		{
			if(target != null)
				target.transform.localRotation = Quaternion.identity;
		}
		
		protected override void OnAction ()
		{
			_fromRotation = _toRotation;
			_toRotation = Quaternion.Euler(-offsetAngle) * _clibRotation * _rotation;
			_time = 0f;
		}
		
		protected override void OnPop ()
		{
			Pop(ref _qX);
			Pop(ref _qY);
			Pop(ref _qZ);
			Pop(ref _qW);
			Pop(ref _intervalTime);

			_refTime = (float)_intervalTime / 1000f;
			_rotation = new Quaternion((float)_qX * -0.0001f
			                        ,(float)_qZ * -0.0001f
			                        ,(float)_qY * -0.0001f
			                        ,(float)_qW * 0.0001f);

			//Debug.Log(_rotation.eulerAngles);
		}
		
		protected override void OnPush ()
		{
		}
		
		public Quaternion Rotation
		{
			get
			{
				return _curRotation;
			}
		}

		public float IntervalTime
		{
			get
			{
				return (float)_intervalTime * 0.001f;
			}
		}

		public void Calibration()
		{
			_clibRotation = Quaternion.Inverse(_rotation);
		}
	}
}
