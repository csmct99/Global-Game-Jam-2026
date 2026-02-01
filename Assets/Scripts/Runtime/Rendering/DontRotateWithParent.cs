using System;

using UnityEngine;

namespace Runtime.Rendering
{
	public class DontRotateWithParent : MonoBehaviour
	{
		#region MonoBehaviour Methods

		private void LateUpdate()
		{
			transform.rotation = Quaternion.identity;
		}

		#endregion
	}
}
