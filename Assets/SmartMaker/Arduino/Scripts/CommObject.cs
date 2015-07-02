using UnityEngine;
using System.Collections;
using System;


namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Arduino/Internal/CommObject")]
	public class CommObject : MonoBehaviour
	{
		public ArduinoApp owner;
		public EventHandler OnOpened;
		public EventHandler OnOpenFailed;
		public EventHandler OnErrorClosed;

		public virtual void Open()
		{
		}

		public virtual void Close()
		{
		}

		public virtual void Write(byte[] bytes)
		{
		}

		public virtual byte[] Read()
		{
			return null;
		}

		public virtual bool IsOpen
		{
			get
			{
				return false;
			}
		}

		public virtual string[] SketchIncludes()
		{
			return null;
		}
		
		public virtual string SketchDeclaration()
		{
			return "";
		}

		public virtual string SketchSetup()
		{
			return "";
		}

		public virtual string SketchLoop()
		{
			return "";
		}
	}
}

