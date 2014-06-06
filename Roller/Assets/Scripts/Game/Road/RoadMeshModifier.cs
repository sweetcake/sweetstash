using UnityEngine;
using System.Collections.Generic;
namespace Game
{
	public class RoadMeshModifier : BaseRoadMeshModifier
	{
		#region implemented abstract members of SplineMeshModifier

		public float[] PathScales;
		public int Repeat = 1;

		// Max time at which to use the desired scale
		private float[] pathScaleTimeCeiling;

		void Awake()
		{
			Init ();
		}

		protected void Init()
		{
			if(PathScales == null || PathScales.Length == 0)
			{
				return;
			}
			
			int entries = PathScales.Length * Repeat;
			
			pathScaleTimeCeiling = new float[entries];
			float segmentLength = 1.0f / (float)entries;
			float currentSegmentMax = segmentLength;
			
			// Repeat count
			for(int i = 0; i < entries; ++i)
			{
				pathScaleTimeCeiling[i] = currentSegmentMax;
				currentSegmentMax += segmentLength;
			}
			
			for(float i = 0; i <= 1.0f; i += 0.01f)
			{
				GetScale(i);
			}
		}

		protected float GetScale(float time)
		{
			if(PathScales == null || PathScales.Length == 0 || pathScaleTimeCeiling == null)
			{
				return 1.0f;
			}

			for(int i = 0; i < pathScaleTimeCeiling.Length; ++i)
			{
				// Found the entry
				if(time <= pathScaleTimeCeiling[i])
				{
					int index = i % PathScales.Length;
					return PathScales[index];
				}
			}
			return 1.0f;
		}

		public override Vector3 ModifyVertex( SplineMesh splineMesh, Vector3 vertex, float splineParam )
		{
			return vertex * GetScale(splineParam);
		}
		
		#endregion
	}

	[System.Serializable]
	public class ScaleSetting
	{
		public float Scale = 1.0f;
		public float Percentage = 1.0f;
	}
}

