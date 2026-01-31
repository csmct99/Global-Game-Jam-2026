using UnityEngine;

public class SmoothFollowTarget : MonoBehaviour
{
	#region Private Fields

	[SerializeField]
	private float _interpolationSpeed = 0.1f;

	[SerializeField]
	private Transform _target;

	#endregion

	#region MonoBehaviour Methods

	public void Update()
	{
		Vector2 newPos = Vector2.Lerp(transform.position, _target.position, _interpolationSpeed);

		transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
	}

	#endregion
}
