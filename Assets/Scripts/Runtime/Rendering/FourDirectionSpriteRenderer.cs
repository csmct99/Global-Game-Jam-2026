using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FourDirectionSpriteRenderer : MonoBehaviour
{
	#region Private Fields

	[Header("Directional Sprites")]
	[Tooltip("Sprite to display when parent is facing +Y (0 degrees)")]
	[SerializeField]
	private Sprite _spriteUp;

	[Tooltip("Sprite to display when parent is facing -X (90 degrees)")]
	[SerializeField]
	private Sprite _spriteLeft;

	[Tooltip("Sprite to display when parent is facing -Y (180 degrees)")]
	[SerializeField]
	private Sprite _spriteDown;

	[Tooltip("Sprite to display when parent is facing +X (270 degrees)")]
	[SerializeField]
	private Sprite _spriteRight;

	private SpriteRenderer _spriteRenderer;
	private Transform _parentTransform;
	private int _lastSpriteIndex = -1;

	#endregion

	#region MonoBehaviour Methods

	private void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_parentTransform = transform.parent;

		if (_parentTransform == null)
		{
			Debug.LogError($"[{nameof(FourDirectionSpriteRenderer)}] No parent transform found on {name}. Disabling component.");
			enabled = false;
		}
	}

	private void LateUpdate()
	{
		if (_parentTransform == null)
			return;

		// Visual indicator should remain upright regardless of parent rotation
		transform.rotation = Quaternion.identity;

		UpdateSprite();
	}

	#endregion

	#region Private Methods

	private void UpdateSprite()
	{
		float zRotation = _parentTransform.eulerAngles.z;
		int directionIndex = Mathf.FloorToInt((zRotation + 45f) / 90f) % 4;

		// Only swap the sprite if the direction has actually changed
		if (directionIndex == _lastSpriteIndex)
			return;

		_lastSpriteIndex = directionIndex;
		SetSpriteByIndex(directionIndex);
	}

	private void SetSpriteByIndex(int index)
	{
		Sprite newSprite = null;

		switch (index)
		{
			case 0:
				newSprite = _spriteUp;
				break;

			case 1:
				newSprite = _spriteLeft;
				break;

			case 2:
				newSprite = _spriteDown;
				break;

			case 3:
				newSprite = _spriteRight;
				break;
		}

		if (newSprite != null)
		{
			_spriteRenderer.sprite = newSprite;
		}
	}

	#endregion
}
