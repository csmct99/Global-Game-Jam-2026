using System;

using UnityEngine;

namespace Runtime.Utilities
{
	public class SpawnObject : MonoBehaviour
	{
		
		public GameObject ObjectToSpawn;

		public void Spawn()
		{
			if (Application.isPlaying && ObjectToSpawn != null)
			{

				Instantiate(ObjectToSpawn, transform.position, Quaternion.identity, parent: null);
			}
		}
	}
}
