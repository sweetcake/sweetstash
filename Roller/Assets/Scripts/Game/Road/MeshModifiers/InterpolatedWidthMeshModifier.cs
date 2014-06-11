using UnityEngine;
namespace Game
{
	public class InterpolatedWidthMeshModifier : BaseRoadMeshModifier
	{
		public float StartingScale = 1.0f;
		public float EndingScale = 1.0f;

		public override Vector3 ModifyVertex (SplineMesh splineMesh, Vector3 vertex, float splineParam)
		{
			vertex.x *= Mathf.Lerp(StartingScale, EndingScale, splineParam);
			return vertex;
		}
	}
}

