﻿using UnityEngine;
using System.Collections;

namespace SmartGolf
{
	[AddComponentMenu("SmartGolf/ClubGeometry")]
	public class ClubGeometry : MonoBehaviour
	{
		public Transform characterCenter;
		public Transform characterUp;
		public Transform characterForward;

		public Transform clubCenter;
		public Transform clubUp;
		public Transform clubForward;
		public Transform clubFace;

		public bool displayDebug = true;

		private Vector3 _characterForward;
		private Vector3 _characterUp;
		private Vector3 _characterRight;

		private Vector3 _clubCenterOnRoll;
		private Vector3 _clubUpOnRoll;
		private Vector3 _clubShaftOnRoll;

		private Vector3 _clubCenterOnYaw;
		private Vector3 _clubUpOnYaw;
		private Vector3 _clubShaftOnYaw;

		private Vector3 _clubShaft;
		private Vector3 _clubForwardFrom;

		private float _rollAngle;
		private float _yawAngle;
		private float _clubAngle;


		// Use this for initialization
		void Start ()
		{
			Initialize();		
		}
		
		// Update is called once per frame
		void Update ()
		{
			_clubCenterOnRoll = GetPosiionOnPlane(clubCenter.position, characterCenter.position, _characterUp, _characterRight);
			_clubUpOnRoll = GetPosiionOnPlane(clubUp.position, characterCenter.position, _characterUp, _characterRight);
			Vector3 curVector = _clubCenterOnRoll - _clubUpOnRoll;
			_rollAngle += GetAngle(_clubShaftOnRoll, curVector, _characterForward);
			_clubShaftOnRoll = curVector;

			_clubCenterOnYaw = GetPosiionOnPlane(clubCenter.position, characterCenter.position, _characterForward, _characterRight);
			_clubUpOnYaw = GetPosiionOnPlane(clubUp.position, characterCenter.position, _characterForward, _characterRight);
			curVector = _clubCenterOnYaw - _clubUpOnYaw;
			_yawAngle += GetAngle(_clubShaftOnYaw, curVector, _characterUp);
			_clubShaftOnYaw = curVector;

			curVector = clubUp.position - clubCenter.position;
			Quaternion rot = Quaternion.FromToRotation(_clubShaft, curVector);
			_clubShaft = curVector;
			_clubForwardFrom = rot * _clubForwardFrom;
			curVector = clubForward.position - clubCenter.position;
			_clubAngle = GetAngle(_clubForwardFrom, curVector, _clubShaft);
		}

		void OnDrawGizmos()
		{
			if(displayDebug == false)
				return;

			if(characterCenter != null && characterUp != null && characterForward != null)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(characterCenter.position, characterForward.position);
				Gizmos.DrawLine(characterCenter.position, -characterUp.position);

				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(characterCenter.position, characterCenter.position + (_clubCenterOnRoll - _clubUpOnRoll));
				Gizmos.DrawLine(characterCenter.position, characterCenter.position + (_clubCenterOnYaw - _clubUpOnYaw));
			}

			if(clubCenter != null && clubUp != null && clubForward != null)
			{
				Gizmos.color = Color.magenta;
				Gizmos.DrawLine(clubCenter.position, clubUp.position);
				Gizmos.DrawLine(clubCenter.position, clubForward.position);

				Gizmos.color = Color.white;
				Gizmos.DrawLine(clubCenter.position, _clubForwardFrom + clubCenter.position);
			}

			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(_clubUpOnRoll, _clubCenterOnRoll);
			Gizmos.DrawLine(_clubUpOnYaw, _clubCenterOnYaw);
		}

		public void Initialize()
		{
			_characterUp = characterCenter.up;
			_characterForward = characterForward.position - characterCenter.position;
			_characterForward.Normalize();
			_characterRight = Vector3.Cross(_characterForward, _characterUp);

			_clubForwardFrom = clubForward.position - clubCenter.position;
			_clubShaft = clubUp.position - clubCenter.position;

			_clubShaftOnRoll = -_characterUp;
			_clubShaftOnYaw = _characterForward;

			_rollAngle = 0f;
			_yawAngle = 0f;
			_clubAngle = 0f;
		}

		public float rollAngle
		{
			get
			{
				return _rollAngle;
			}
		}

		public float yawAngle
		{
			get
			{
				return _yawAngle;
			}
		}

		public float clubAngle
		{
			get
			{
				return _clubAngle;
			}
		}

		public Vector3 clubDirUp
		{
			get
			{
				Vector3 dir = clubUp.position - clubCenter.position;
				return dir.normalized;
			}
		}

		public Vector3 clubDirForward
		{
			get
			{
				Vector3 dir = clubForward.position - clubCenter.position;
				return dir.normalized;
			}
		}

		public float clubLength
		{
			get
			{
				return Vector3.Distance(clubUp.position, clubCenter.position);
			}
		}

		public Vector3 EvaluateClubPosition(Vector3 fromPos, Vector3 upPos, Vector3 upDir, Vector3 forwardDir)
		{
			Quaternion rot = Quaternion.FromToRotation(clubDirUp, upDir);
			Vector3 center = rot * (clubCenter.position - clubUp.position);
			Vector3 pos = rot * (fromPos - clubUp.position);
			Vector3 forward = rot * (clubForward.position - clubUp.position);

			forward = forward - center;
			rot = Quaternion.FromToRotation(forward.normalized, forwardDir);
			pos = rot * (pos - center);

			return upPos + (-upDir * clubLength) + pos;
		}

		public Vector3 ToRelativePosition(Vector3 worldPos)
		{
			Vector3 offset = worldPos - characterCenter.position;
			return new Vector3(ScalarOnVector(offset, _characterRight)
			                   ,ScalarOnVector(offset, _characterUp)
			                   ,ScalarOnVector(offset, _characterForward));
		}

		public Vector3 ToRelativeDirection(Vector3 worldDir)
		{
			Vector3 dir = new Vector3(ScalarOnVector(worldDir, _characterRight)
			                          ,ScalarOnVector(worldDir, _characterUp)
			                          ,ScalarOnVector(worldDir, _characterForward));
			return dir.normalized;
		}

		public Vector3 ToWorldPosition(Vector3 relativePos)
		{
			return (relativePos.x * _characterRight + relativePos.y * _characterUp + relativePos.z * _characterForward) + characterCenter.position;
		}

		public Vector3 ToWorldDirection(Vector3 relativeDir)
		{
			Vector3 dir = relativeDir.x * _characterRight + relativeDir.y * _characterUp + relativeDir.z * _characterForward;
			return dir.normalized;
		}

		static public Vector3 GetPosiionOnPlane(Vector3 target, Vector3 root, Vector3 forward, Vector3 right)
		{
			Vector3 dir = target - root;
			Vector3 prjForward = Vector3.Project(dir, forward);
			Vector3 prjRight = Vector3.Project(dir, right);
			return (prjForward + prjRight) + root;
		}

		static public float GetAngle(Vector3 from, Vector3 to, Vector3 up)
		{
			from.Normalize();
			to.Normalize();
			up.Normalize();

			float angle = Vector3.Angle(from, to);
			if(Vector3.Dot(up, Vector3.Cross(to, from)) < 0f)
				angle = -angle;

			return angle;
		}

		static public float ScalarOnVector(Vector3 vector, Vector3 onNormal)
		{
			Vector3 proj = Vector3.Project(vector, onNormal);
			float scalar = proj.magnitude;
			if(Vector3.Dot(proj, onNormal) < 0f)
				scalar = -scalar;
			
			return scalar;
		}
	}
}
