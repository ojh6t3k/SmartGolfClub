using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace UnityRobot
{
	[AddComponentMenu("UnityRobot/UnityModule")]
	public class UnityModule : MonoBehaviour
	{
		[HideInInspector]
		public UnityRobot owner;

		public int id;

		public EventHandler OnStarted;
		public EventHandler OnStopped;
		public EventHandler OnUpdated;

		private List<byte> _dataBytes = new List<byte>();
		private const int _maxNumBytes = 116;
		private byte _enableFlush;
		private bool _updated;
		private bool _started = false;

		protected bool outputOnly = true;
		protected bool canUpdate = false;


		void Awake()
		{
			if(this.enabled == true)
				_enableFlush = 1;
			else
				_enableFlush = 0;

			OnAwake();
		}


		// Update is called once per frame
		void Update ()
		{
			if(_started == true)
			{
				OnUpdate();
			}
		}

		void OnEnable()
		{
			_enableFlush = 1;
			if(outputOnly == false)
				canUpdate = true;
		}

		void OnDisable()
		{
			_enableFlush = 0;
			if(outputOnly == false)
				canUpdate = true;
		}

		public byte[] dataBytes
		{
			get
			{
				if(canUpdate == false)
					return null;

				canUpdate = false;
				_dataBytes.Clear();
				OnPush();
				if(outputOnly == false)
					Push(_enableFlush);

				if(_dataBytes.Count == 0)
					return null;
				else
					return _dataBytes.ToArray();
			}
			set
			{
				_dataBytes.Clear();
				_dataBytes.AddRange(value);
				OnPop();
				_updated = true;
			}
		}

		public bool Started
		{
			get
			{
				return _started;
			}
		}

		public void ModuleStart()
		{
			canUpdate = true;
			_updated = false;
			_started = true;

			OnModuleStart();

			if(OnStarted != null)
				OnStarted(this, null);
		}

		public void Action()
		{
			if(_updated == true)
			{
				OnAction();
				_updated = false;

				if(OnUpdated != null)
					OnUpdated(this, null);
			}
		}

		public void ModuleStop()
		{
			_started = false;

			OnModuleStop();
			
			if(OnStopped != null)
				OnStopped(this, null);
		}

		protected bool Push(byte value)
		{
			if((_maxNumBytes - _dataBytes.Count) < 1)
				return false;

			_dataBytes.Add(value);
			return true;
		}

		protected bool Push(ushort value)
		{
			if((_maxNumBytes - _dataBytes.Count) < 2)
				return false;
			
			_dataBytes.Add((byte)(value & 0xFF));
			_dataBytes.Add((byte)((value >> 8) & 0xFF));
			return true;
		}

		protected bool Push(short value)
		{
			ushort binary = 0;
			if(value < 0)
			{
				value *= -1;
				binary = (ushort)value;
				binary |= (ushort)0x8000;
			}
			else
				binary = (ushort)value;
			
			return Push(binary);
		}

		protected bool Push(byte[] value)
		{
			if((_maxNumBytes - _dataBytes.Count) < value.Length)
				return false;

			_dataBytes.AddRange(value);
			return true;
		}

		protected bool Pop(ref byte value)
		{
			if(_dataBytes.Count < 1)
				return false;
			value = _dataBytes[0];
			_dataBytes.RemoveAt(0);
			return true;
		}
		
		protected bool Pop(ref ushort value)
		{
			if(_dataBytes.Count < 2)
				return false;

			value = (ushort)(((_dataBytes[1] << 8) & 0xFF00) | (_dataBytes[0] & 0xFF));
			_dataBytes.RemoveRange(0, 2);
			return true;
		}
		
		protected bool Pop(ref short value)
		{
			ushort binary = 0;
			if(Pop(ref binary) == false)
				return false;

			value = (short)(binary & 0x7FFF);
			if((binary & 0x8000) == 0x8000)
				value *= -1;
			return true;
		}
		
		protected bool Pop(ref byte[] value, int count)
		{
			if(_dataBytes.Count < count)
				return false;

			value = _dataBytes.GetRange(0, count).ToArray();
			_dataBytes.RemoveRange(0, count);
			return true;
		}

		protected virtual void OnAwake()
		{
		}

		protected virtual void OnUpdate()
		{
		}

		protected virtual void OnModuleStart()
		{
		}

		protected virtual void OnModuleStop()
		{
		}
		
		protected virtual void OnAction()
		{
		}
		
		protected virtual void OnPush()
		{
		}
		
		protected virtual void OnPop()
		{
		}
	}
}