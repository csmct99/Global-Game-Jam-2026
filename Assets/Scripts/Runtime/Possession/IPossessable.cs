using UnityEngine;

namespace Runtime
{
	public interface IPossessable
	{
		public void BeginPossess(MaskController mask);
		public void StopPossess(MaskController mask);

		public bool IsPossessed
		{
			get;
		}

		public GameObject GetGameObject()
		{
			return (this as MonoBehaviour)?.gameObject;
		}
	}
}
