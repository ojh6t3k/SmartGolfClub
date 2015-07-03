using UnityEngine;
using System.Collections;

namespace SmartGolf
{
	[AddComponentMenu("SmartGolf/SwingAnalyzer")]
	public class SwingAnalyzer : MonoBehaviour
	{
		public Transform characterRoot;
		public Transform characterForward;
		public Transform clubRoot;
		public Transform clubForward;
		public Transform clubUp;

		public float rollAngle;
		public float pitchAngle;
		public float yawAngle;

		private Vector3 _characterForward;
		private Vector3 _characterUp;
		private Vector3 _characterRight;
		private Vector3 _clubRootOnForward;
		private Vector3 _clubUpOnForward;
		private Vector3 _clubRootOnUp;
		private Vector3 _clubUpOnUp;
		private Vector3 _clubForward;
		private Vector3 _clubFromForward;
		private Vector3 _clubFromDirection;
		private Vector3 _clubOnUp;
		private Vector3 _clubOnForward;

		// Use this for initialization
		void Start ()
		{
			Initialize();		
		}
		
		// Update is called once per frame
		void Update ()
		{
			_clubRootOnForward = GetPosiionOnPlane(clubRoot.position, characterRoot.position, _characterForward, _characterUp);
			_clubUpOnForward = GetPosiionOnPlane(clubUp.position, characterRoot.position, _characterForward, _characterUp);
			_clubRootOnUp = GetPosiionOnPlane(clubRoot.position, characterRoot.position, _characterUp, _characterRight);
			_clubUpOnUp = GetPosiionOnPlane(clubUp.position, characterRoot.position, _characterUp, _characterRight);

			Vector3 clubToDirection = clubRoot.position - clubUp.position;
			Quaternion rot = Quaternion.FromToRotation(_clubFromDirection, clubToDirection);
			_clubFromForward = (rot * _clubForward) + clubRoot.position;

			Vector3 curVector = _clubRootOnUp - _clubUpOnUp;
			rollAngle += GetAngle(_clubOnUp, curVector, _characterForward);
			_clubOnUp = curVector;

			curVector = _clubRootOnUp - _clubUpOnUp;
			pitchAngle += GetAngle(_clubOnForward, curVector, _characterRight);
			_clubOnForward = curVector;
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(characterRoot.position, characterForward.position);

			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(clubRoot.position, clubUp.position);

			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(_clubUpOnForward, _clubRootOnForward);

			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(_clubUpOnUp, _clubRootOnUp);

			Gizmos.color = Color.white;
			Gizmos.DrawLine(clubRoot.position, _clubFromForward);

			Gizmos.color = Color.red;
			Gizmos.DrawLine(clubRoot.position, clubForward.position);
		}

		public void Initialize()
		{
			_characterUp = characterRoot.up;
			_characterForward = characterForward.position - characterRoot.position;
			_characterForward.Normalize();
			_characterRight = Vector3.Cross(_characterForward, _characterUp);

			_clubForward = clubForward.position - clubRoot.position;
			_clubFromDirection = clubRoot.position - clubUp.position;
			_clubFromDirection.Normalize();

			_clubOnUp = -_characterUp;
			_clubOnForward = _characterForward;

			rollAngle = 0f;
			pitchAngle = 0f;
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
