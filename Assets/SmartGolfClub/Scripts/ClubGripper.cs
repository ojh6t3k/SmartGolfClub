using UnityEngine;
using System.Collections;

namespace SmartGolf
{
	[AddComponentMenu("SmartGolf/ClubGripper")]
	public class ClubGripper : MonoBehaviour
	{
		public Transform rightAt;
		public Transform rightTo1;
		public Transform rightTo2;
		public Transform leftAt;
		public Transform leftTo1;
		public Transform leftTo2;

		private Animator _animator;

		// Use this for initialization
		void Start ()
		{
			_animator = GetComponent<Animator>();		
		}
		
		// Update is called once per frame
		void Update ()
		{
		
		}

		void OnAnimatorIK(int layerIndex)
		{
			if(rightAt != null)
			{
				_animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
				_animator.SetIKPosition(AvatarIKGoal.RightHand, rightAt.position);
				if(rightTo1 != null && rightTo2 != null)
				{
					Vector3 forward1 = rightTo1.position - rightAt.position;
					Vector3 forward2 = rightTo2.position - rightAt.position;
					Vector3 up = Vector3.Cross(forward2, forward1);
					_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
					_animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(forward1, up));
				}
			}

			if(leftAt != null)
			{
				_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
				_animator.SetIKPosition(AvatarIKGoal.LeftHand, leftAt.position);

				if(leftTo1 != null && leftTo2 != null)
				{
					Vector3 forward1 = leftTo1.position - leftAt.position;
					Vector3 forward2 = leftTo2.position - leftAt.position;
					Vector3 up = Vector3.Cross(forward1, forward2);
					_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
					_animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.LookRotation(forward1, up));
				}
			}
		}
	}
}
