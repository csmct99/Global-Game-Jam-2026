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

	#endregion

	#region MonoBehaviour Methods

	private void Awake()
	{
		_startColor = _spriteRenderer.color;
		_modifiedColor = new Color(_startColor.r, _startColor.g, _startColor.b, _startColor.a);
	}

	private void Update()
	{
		_spriteRenderer.color = DesaturateColor(_startColor, _damageController.HealthAsPercent);
	}

	private void OnValidate()
	{
		if (_damageController == null)
			_damageController = GetComponent<DamageController>();
	}

	#endregion

	#region Private Methods

	private Color DesaturateColor(Color color, float saturation)
	{
		_modifiedColor.r = color.r * saturation;
		_modifiedColor.g = color.g * saturation;
		_modifiedColor.b = color.b * saturation;

		return _modifiedColor;
	}

	#endregion
}
