using System;

using UnityEngine;

namespace Runtime.Rendering
{
	public class RandomRotationOnSpawn : MonoBehaviour
	{
		#region MonoBehaviour Methods

		private void Awake()
		{
			transform.localRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
		}

		#endregion
	}
}
