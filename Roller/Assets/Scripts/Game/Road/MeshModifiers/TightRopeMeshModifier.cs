using UnityEngine;
namespace Game
{
	public class TightRopeMeshModifier : BaseRoadMeshModifier
	{
		public float StartingScale = 1.0f;
		public float EndingScale = 1.0f;
		public float TightRopeScale = 1.0f;
		public float TightRopeStart = 0.2f;
		public float TightRopeEndTime = 0.8f;

		public override Vector3 ModifyVertex (SplineMesh splineMesh, Vector3 vertex, float splineParam)
		{
			if(splineParam <= TightRopeStart)
			{
				float time = Mathf.Clamp(splineParam / TightRopeStart, 0.0f, 1.0f);
				vertex.x *= Mathf.Lerp(StartingScale, TightRopeScale, time);
			}
			else if(splineParam >= TightRopeEndTime)
			{
				float time = Mathf.Clamp((splineParam - TightRopeEndTime) / (1 - TightRopeEndTime), 0.0f, 1.0f);
				vertex.x *= Mathf.Lerp(TightRopeScale, EndingScale, time);
			}
			else
			{
				vertex.x *= TightRopeScale;
			}
			return vertex;
		}
	}
}

