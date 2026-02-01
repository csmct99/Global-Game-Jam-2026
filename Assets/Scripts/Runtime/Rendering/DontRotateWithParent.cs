// DontRotateWithParent.cs
// 
// Description:
// 
// Author:
//  xxxcs
// 
// Copyright (C) Budge Studios Inc., 2026

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
