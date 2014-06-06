using UnityEngine;
namespace Core
{
	public static class MathUtils
	{
		public static bool IsZero(float value)
		{
			return value < Mathf.Epsilon && value > -Mathf.Epsilon;
		}

		public static bool IsEqual(float a, float b)
		{
			return a < b + Mathf.Epsilon && a > b - Mathf.Epsilon;
		}
	}
}

