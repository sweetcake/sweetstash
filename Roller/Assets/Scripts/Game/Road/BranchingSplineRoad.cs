using UnityEngine;
namespace Game
{
	public class BranchingSplineRoad : BaseSplineRoad
	{
		public override void Populate (Spline originalSpline)
		{

			int counter = 0;
			SplineNode lastNode = gameObject.transform.FindChild("Node2").gameObject.GetComponent<SplineNode>();
			foreach(SplineNode originalNode in originalSpline.splineNodesArray)
			{
				if(originalNode.name == "Node0" || originalNode.name == "Node1" || originalNode.name == "Node2")
				{
					continue;
				}

				// Add node
				SplineNode newNode = Spline.AddSplineNode(lastNode).GetComponent<SplineNode>();

				// Set name
				newNode.gameObject.name = originalNode.name;
				
				Vector3 position = originalNode.transform.position;

				if(counter++ % 5 == 3)
				{
					float direction = Random.value > 0.5f ? -1.0f : 1.0f;
					position += Vector3.right * direction * 10.0f;
				}

				// Set position
				newNode.transform.parent = transform;
				newNode.transform.position = position;

				// Update spline
				Spline.UpdateSpline();

				// Copy new node
				lastNode = newNode;
			}

			base.Populate(originalSpline);
		}
	}
}

