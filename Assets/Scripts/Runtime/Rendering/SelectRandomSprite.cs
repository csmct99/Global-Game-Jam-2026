using System;
using System.Collections.Generic;

using UnityEngine;

namespace Runtime.Rendering
{
	public class SelectRandomSprite : MonoBehaviour
	{
		public List<Sprite> _possibleSprites;
		public SpriteRenderer _spriteRenderer;

		private void Awake()
		{
			Randomize();
		}
		
		[ContextMenu("RADOMIZE SPRITE")]
		public void Randomize()
		{
			
			_spriteRenderer.sprite = _possibleSprites[UnityEngine.Random.Range(0, _possibleSprites.Count)];
		}
	}
}
