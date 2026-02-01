// RandomRotationOnSpawn.cs
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
	public class RandomRotationOnSpawn : MonoBehaviour
	{
		private void Awake()
		{
			transform.localRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
		}
	}
}
