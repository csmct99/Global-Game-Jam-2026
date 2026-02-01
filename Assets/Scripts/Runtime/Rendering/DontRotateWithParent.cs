using System;

using UnityEngine;

namespace Runtime.Rendering
{
	public class DontRotateWithParent : MonoBehaviour
	{
		private void LateUpdate()
		{
			transform.rotation = Quaternion.identity;
		}
	}
}
