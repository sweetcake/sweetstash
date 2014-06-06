using UnityEngine;
namespace Game
{
	public class Road : MonoBehaviour
	{
		public Transform StartPoint;
		public DynamicSplineRoad dynamicRoad;
		public BranchingSplineRoad branchingRoad;

		public virtual void Init()
		{
			// Init our spline roads
			dynamicRoad.Populate();
			branchingRoad.Populate(dynamicRoad.Spline);
		}

	}
}

