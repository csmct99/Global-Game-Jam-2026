using System;

using UnityEngine;

[RequireComponent(typeof(DamageController))]
public class HealthFlasher : MonoBehaviour
{
	#region Private Fields

	[SerializeField]
	private DamageController _damageController;

	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	private Color _startColor;
	private Color _modifiedColor;

	[SerializeField]
	private float _frequency = 1f;
	[SerializeField]
	private Color _flashHighColor = Color.white;

	#endregion

	#region MonoBehaviour Methods

	private void Awake()
	{
		_startColor = _spriteRenderer.color;
		_modifiedColor = new Color(_startColor.r, _startColor.g, _startColor.b, _startColor.a);

		_flashHighColor = Color.Lerp(_startColor, _flashHighColor, 0.5f);
	}

	private void Update()
	{
		_spriteRenderer.color = CalculateFlashColor(_startColor,  1 - _damageController.HealthAsPercent);
	}

	private void OnValidate()
	{
		if (_damageController == null)
			_damageController = GetComponent<DamageController>();
	}

	private Color CalculateFlashColor(Color startColor, float deathPercent)
	{
		return Color.Lerp(startColor, _flashHighColor,  Mathf.Sin(Time.time * _frequency * deathPercent));
	}

	#endregion

}
