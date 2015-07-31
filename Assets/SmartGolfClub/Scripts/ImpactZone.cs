using UnityEngine;
using System.Collections;

namespace SmartGolf
{
	[AddComponentMenu("SmartGolf/ImpactZone")]
	public class ImpactZone : MonoBehaviour
	{
		public Transform boundBox;
		public Transform ball;
		public SwingCurve swingCurve;

		public bool displayDebug = true;

		private bool _analyzed = false;
		private float _impactTime;
		private Vector3 _inPos;
		private Vector3 _outPos;
		private Vector3 _hitDir;
		private float _pathAngle;
		private float _faceAngle;
		private float _attackAngle;
		private float _velocity;

		// Use this for initialization
		void Start ()
		{
		}
		
		// Update is called once per frame
		void Update ()
		{		
		}

		void OnDrawGizmos()
		{
			if(displayDebug == false)
				return;

			if(boundBox != null && ball != null)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(ball.position + transform.forward * (boundBox.localScale.z * 0.5f)
				                ,ball.position - transform.forward * (boundBox.localScale.z * 0.5f));

				Gizmos.color = Color.red;
				Gizmos.DrawLine(ball.position + transform.right * (boundBox.localScale.x * 0.5f)
				                ,ball.position - transform.right * (boundBox.localScale.x * 0.5f));
			}

			if(_analyzed == true)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(_inPos, _outPos);

				Gizmos.color = Color.white;
				Gizmos.DrawLine(ball.position
				                ,ball.position + _hitDir * boundBox.localScale.z);
			}
		}

		public void MoveTo(Vector3 pos, Vector3 dir)
		{
			pos.y = transform.position.y;
			dir = Vector3.Project(dir, transform.forward) + Vector3.Project(dir, transform.right);

			transform.position = pos;
			transform.rotation = Quaternion.FromToRotation(transform.forward, dir) * transform.rotation;
		}

		public void AnalyzeCurve()
		{
			_analyzed = false;
			_impactTime = 0f;
			_pathAngle = 0f;
			_faceAngle = 0f;
			_attackAngle = 0f;
			_velocity = 0f;
			_hitDir = Vector3.zero;

			if(swingCurve == null)
				return;
			if(swingCurve.dataEnabled == false)
				return;

			Vector3 pos = Vector3.zero;
			float time = swingCurve.topTime;
			Vector3 upPos = ball.position + transform.up * (boundBox.localScale.y * 0.5f);
			_inPos = ball.position - transform.forward * (boundBox.localScale.z * 0.5f);
			_outPos = ball.position + transform.forward * (boundBox.localScale.z * 0.5f);

			SearchSwingCurve(ref pos, ref time, 0.1f, upPos, -transform.up);
			SearchSwingCurve(ref pos, ref time, -0.01f, _inPos, -transform.forward);
			SearchSwingCurve(ref pos, ref time, 0.0001f, _inPos, transform.forward);

			Vector3 offset = pos - _inPos;
			_inPos = _inPos + Vector3.Project(offset, transform.right) + Vector3.Project(offset, transform.up);

			SearchSwingCurve(ref pos, ref time, 0.0001f, ball.position, transform.forward);
			SearchSwingCurve(ref pos, ref time, -0.00001f, ball.position, -transform.forward);
			_impactTime = time;

			SearchSwingCurve(ref pos, ref time, 0.0001f, _outPos, transform.forward);
			SearchSwingCurve(ref pos, ref time, -0.00001f, _outPos, -transform.forward);

			offset = pos - _outPos;
			_outPos = _outPos + Vector3.Project(offset, transform.right) + Vector3.Project(offset, transform.up);

			Vector3 clubFace = swingCurve.clubGeometry.clubFace.position;
			Vector3 clubFaceForward = clubFace + swingCurve.clubGeometry.clubFace.forward;
			Vector3 impactClubUpPos = swingCurve.EvaluateClubUpPosition(_impactTime);
			Vector3 impactClubUpDir = swingCurve.EvaluateClubUpDirection(_impactTime);
			Vector3 impactClubForwardDir = swingCurve.EvaluateClubForwardDirection(_impactTime);

			clubFace = swingCurve.clubGeometry.EvaluateClubPosition(clubFace, impactClubUpPos, impactClubUpDir, impactClubForwardDir);
			clubFaceForward = swingCurve.clubGeometry.EvaluateClubPosition(clubFaceForward, impactClubUpPos, impactClubUpDir, impactClubForwardDir);
			_hitDir = clubFaceForward - clubFace;
			_hitDir.Normalize();

			_velocity = swingCurve.clubVelocities.Evaluate(_impactTime);

			Vector3 proj = Vector3.Project(_hitDir, transform.forward) + Vector3.Project(_hitDir, transform.right);
			_faceAngle = ClubGeometry.GetAngle(transform.forward, proj,transform.up);

			proj = Vector3.Project(_hitDir, transform.forward) + Vector3.Project(_hitDir, transform.up);
			_attackAngle = ClubGeometry.GetAngle(transform.forward, proj,transform.right);

			offset = _outPos - _inPos;
			offset.Normalize();
			proj = Vector3.Project(offset, transform.forward) + Vector3.Project(offset, transform.right);
			_pathAngle = ClubGeometry.GetAngle(transform.forward, proj,transform.up);

			_analyzed = true;
		}

		private void SearchSwingCurve(ref Vector3 pos, ref float time, float unitTime, Vector3 origin, Vector3 normal)
		{
			if(time < swingCurve.topTime || time > swingCurve.finishTime)
				return;

			while(true)
			{
				pos = swingCurve.EvaluateClubFacePosition(time);
				Vector3 proj = Vector3.Project(pos - origin, normal);
				if(Vector3.Dot(proj, normal) >= 0f)
					return;

				time += unitTime;
				if(time < swingCurve.topTime || time > swingCurve.finishTime)
					return;
			}
		}

		public bool analyzed
		{
			get
			{
				return _analyzed;
			}
		}

		public float impactTime
		{
			get
			{
				return _impactTime;
			}
		}

		public float pathAngle
		{
			get
			{
				return _pathAngle;
			}
		}

		public float faceAngle
		{
			get
			{
				return _faceAngle;
			}
		}

		public float attackAngle
		{
			get
			{
				return _attackAngle;
			}
		}

		public float velocity
		{
			get
			{
				return _velocity / 1000f;
			}
		}

		public Vector3 hitDirection
		{
			get
			{
				return _hitDir;
			}
		}

		public Vector3 hitForce
		{
			get
			{
				return _hitDir * velocity;
			}
		}
	}
}
