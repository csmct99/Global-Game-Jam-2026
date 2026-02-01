using System;
using System.Collections.Generic;

using UnityEngine;

namespace Runtime.Rendering
{
	public class SelectRandomSprite : MonoBehaviour
	{
		#region Public Fields

		public List<Sprite> _possibleSprites;
		public SpriteRenderer _spriteRenderer;

		#endregion

		#region MonoBehaviour Methods

		private void Awake()
		{
			Randomize();
		}

		#endregion

		#region Public Methods

		[ContextMenu("RADOMIZE SPRITE")]
		public void Randomize()
		{
			_spriteRenderer.sprite = _possibleSprites[UnityEngine.Random.Range(0, _possibleSprites.Count)];
		}

		#endregion
	}
}
