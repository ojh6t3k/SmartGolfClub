using UnityEngine;
using System.Collections;

namespace SmartGolf
{
	[AddComponentMenu("SmartGolf/ImpactZone")]
	public class ImpactZone : MonoBehaviour
	{
		public BoxCollider boundBox;
		public Transform ballPosition;
		public Transform hitDirection;

		private Vector3 _inPos;
		private Vector3 _impactPos;
		private Vector3 _outPos;

		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
		
		}

		public void AnalyzeCurve(SwingCurve curve)
		{
		}

		public Vector3 hitForce
		{
			get
			{
				return Vector3.zero;
			}
		}

		public float pathAngle
		{
			get
			{
				return 0f;
			}
		}

		public float attckAngle
		{
			get
			{
				return 0f;
			}
		}
	}
}
