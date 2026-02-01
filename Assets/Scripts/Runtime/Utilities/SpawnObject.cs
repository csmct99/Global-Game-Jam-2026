using System;

using UnityEngine;

namespace Runtime.Utilities
{
	public class SpawnObject : MonoBehaviour
	{
		#region Public Fields

		public GameObject ObjectToSpawn;

		#endregion

		#region Public Methods

		public void Spawn()
		{
			if (Application.isPlaying && ObjectToSpawn != null)
			{
				Instantiate(ObjectToSpawn, transform.position, Quaternion.identity, parent: null);
			}
		}

		#endregion
	}
}
