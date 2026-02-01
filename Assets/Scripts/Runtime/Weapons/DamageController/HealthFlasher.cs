using System;
using System.Collections;

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

	[SerializeField]
	private float _takeDamageFlashDuration = 0.5f;

	[SerializeField]
	private float _lowHealthFlashDuration = 0.2f;

	[SerializeField]
	[Range(0f, 1f)]
	private float _lowHealthThreshold = 0.2f;

	[SerializeField]
	private float _frequency = 1f;

	[SerializeField]
	private Color _flashHighColor = Color.white;

	[SerializeField]
	private Color _flashHurtHighColor = Color.red;

	private Coroutine _flashCoroutine;

	#endregion

	#region MonoBehaviour Methods

	private void Awake()
	{
		_startColor = _spriteRenderer.color;
		_flashHighColor = Color.Lerp(_startColor, _flashHighColor, 0.5f);

		_damageController.OnTookDamage += OnTookDamage;
	}

	private void Update()
	{
		if (_damageController.HealthAsPercent <= _lowHealthThreshold)
		{
			_spriteRenderer.color = CalculateFlashColor(_startColor);
		}
	}

	private void OnDestroy()
	{
		if (_damageController != null)
		{
			_damageController.OnTookDamage -= OnTookDamage;
		}
	}

	private void OnValidate()
	{
		if (_damageController == null)
			_damageController = GetComponent<DamageController>();
	}

	#endregion

	#region Public Methods

	public void FlashOnce(float duration)
	{
		if (_flashCoroutine != null)
			StopCoroutine(_flashCoroutine);
		_flashCoroutine = StartCoroutine(FlashOnceCoroutine(duration));
	}

	#endregion

	#region Private Methods

	private void OnTookDamage(float damage)
	{
		FlashOnce(_takeDamageFlashDuration);
	}

	private Color CalculateFlashColor(Color startColor)
	{
		return Color.Lerp(startColor, _flashHighColor, Mathf.Sin(Time.time * _frequency));
	}

	private IEnumerator FlashOnceCoroutine(float duration)
	{
		float elapsed = 0f;
		while (elapsed < duration)
		{
			float t = Mathf.Sin((elapsed / duration) * Mathf.PI);
			_spriteRenderer.color = Color.Lerp(_startColor, _flashHurtHighColor, t);
			elapsed += Time.deltaTime;
			yield return null;
		}

		_spriteRenderer.color = _startColor;
		_flashCoroutine = null;
	}

	#endregion
}
