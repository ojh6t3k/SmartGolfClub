using UnityEngine;
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

		private Vector3 GetPosiionOnPlane(Vector3 target, Vector3 root, Vector3 forward, Vector3 right)
		{
			Vector3 dir = target - root;
			Vector3 prjForward = Vector3.Project(dir, forward);
			Vector3 prjRight = Vector3.Project(dir, right);
			return (prjForward + prjRight) + root;
		}

		private float GetAngle(Vector3 from, Vector3 to, Vector3 up)
		{
			from.Normalize();
			to.Normalize();
			up.Normalize();

			float angle = Vector3.Angle(from, to);
			if(Vector3.Dot(up, Vector3.Cross(to, from)) < 0f)
				angle = -angle;

			return angle;
		}
	}
}
