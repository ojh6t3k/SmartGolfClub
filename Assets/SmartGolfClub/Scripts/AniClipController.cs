using UnityEngine;
using System.Collections;

namespace SmartGolf
{
	[AddComponentMenu("SmartGolf/AniClipController")]
	public class AniClipController : MonoBehaviour
	{
		public Animator animator;
		public string state;
		public int layer;
		[Range(0f, 1f)]
		public float normalizedTime = 0f;

		private float _normalizedTime;

		void Awake()
		{
		}

		void Start()
		{
		}

		void Update()
		{
			if(animator != null)
			{
				if(_normalizedTime != normalizedTime)
				{
					animator.Play(state, layer, normalizedTime);
					_normalizedTime = normalizedTime;
				}
			}
		}

		void OnEnable()
		{
			if(animator != null)
			{
				animator.speed = 0f;
				animator.Play(state, layer, normalizedTime);
				_normalizedTime = normalizedTime;
			}
		}

		void OnDisable()
		{
			if(animator != null)
			{
				animator.speed = 1f;
			}
		}
	}
}
