using System;
using UnityEngine;
namespace Core
{
	public class PoolableObject : MonoBehaviour
	{
		public string AssetPath = null;

		/// <summary>
		/// Reset this instance.
		/// </summary>
		public virtual void Reset() {}

		/// <summary>
		/// Recycle this instance.
		/// </summary>
		public virtual bool Recycle() 
		{
			return ObjectManager.Instance.ReturnToPool(gameObject);
		}
	}
}

