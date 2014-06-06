using UnityEngine;
namespace Game
{
	public class BaseRoadMeshModifier : SplineMeshModifier
	{
		#region implemented abstract members of SplineMeshModifier

		public override Vector3 ModifyVertex (SplineMesh splineMesh, Vector3 vertex, float splineParam)
		{
			return vertex;
		}

		public override Vector2 ModifyUV (SplineMesh splineMesh, Vector2 uvCoord, float splineParam)
		{
			return uvCoord;
		}

		public override Vector3 ModifyNormal (SplineMesh splineMesh, Vector3 normal, float splineParam)
		{
			return normal;
		}

		public override Vector4 ModifyTangent (SplineMesh splineMesh, Vector4 tangent, float splineParam)
		{
			return tangent;
		}

		#endregion


	}
}

