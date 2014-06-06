using UnityEngine;
namespace Game
{
	public class BranchingRoadMeshModifier : RoadMeshModifier
	{
		public override Vector3 ModifyVertex( SplineMesh splineMesh, Vector3 vertex, float splineParam )
		{
			return vertex * GetScale(splineParam) * 0.5f;
		}
	}
}

