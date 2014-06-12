using UnityEngine;
using Core;
using System.Collections.Generic;
namespace Game
{
	public class RoadSegment : BaseSplineRoad
	{
		public float ForwardDistanceToNextNode = 20.0f;

		// Horizontal offsets per node
		public float MinHorizontalOffset = -10.0f;
		public float MaxHorizontalOffset = 10.0f;
		
		// Vertical offsets per node
		public float MinVerticalOffset = -5.0f;
		public float MaxVerticalOffset = 5.0f;

		public RoadWidths Width;
		List<GameObject> collectables = new List<GameObject>();

		public void Populate(float targetLength, RoadConnector start, RoadConnector end, int index)
		{
			gameObject.name += "_" + index.ToString();

			// Add nodes at the end of the start connector
			SplineNode lastNode = null;
			float splineLength = Spline.Length;
			float time = 0;
			int nodeIndex = 0;

			// Add a node for each of the end nodes in the starting connector
			for(int i = 0; i < 3;  ++i)
			{
				time = Mathf.Clamp(splineLength / targetLength, 0.0f, 1.0f);
				lastNode = AddNode(Spline, lastNode, time, start.GetEndNodeByIndex(i).position, true, nodeIndex++);
				Spline.UpdateSpline();
				splineLength = Spline.Length;
			}

			// Add random nodes
			while(splineLength < targetLength)
			{
				time = Mathf.Clamp(splineLength / targetLength, 0.0f, 1.0f);
				lastNode = AddNode(Spline, lastNode, time, Vector3.zero, false, nodeIndex++);
				Spline.UpdateSpline();
				splineLength = Spline.Length;
			}

			// Update position of the end connector
			float distanceBetweenConnectorPoints = (end.GetStartNodeByIndex(2).position - end.GetStartNodeByIndex(0).position).magnitude;
			Vector3 endPosition = lastNode.transform.position;
			endPosition.z += distanceBetweenConnectorPoints + ForwardDistanceToNextNode;
			end.transform.position = endPosition;

			// Add nodes at end connector
			for(int i = 0; i < 3;  ++i)
			{
				time = Mathf.Clamp(splineLength / targetLength, 0.0f, 1.0f);
				lastNode = AddNode(Spline, lastNode, time, end.GetStartNodeByIndex(i).position, true, nodeIndex++);
				Spline.UpdateSpline();
				splineLength = Spline.Length;
			}

			// Update the road
			UpdateRoad();

			AddPerils();

			AddCollectables();
		}

		public void Reset()
		{
			foreach(GameObject collectable in collectables)
			{
				if(!collectable.activeSelf)
				{
					collectable.SetActive(true);
				}
				else
				{
					break;
				}
			}
		}

		/// <summary>
		/// Adds the perils.
		/// </summary>
		private void AddPerils()
		{
			// Lets not put them in the small roads for now
			if(Width != RoadWidths.LargeRoad)
			{
				return;
			}

			int side = 0;
			float increment = 0.2f;
			for(float i = 0; i < 1.0f; i += increment)
			{
				GameObject bumper = ObjectManager.Instance.GetLoadedGameObject("Game/Perils/Bumper");
				bumper.transform.parent = transform;
				Vector3 position = Spline.GetPositionOnSpline(i);
				//position += Spline.GetNormalToSpline(i) * 0.5f;

				// Ensure that we are on the edges
				Vector3 right = Vector3.right;
				if(i < 1.0f && (i + increment) < 1.0f)
				{
					// Calculate the forward vector
					Vector3 forward = (Spline.GetPositionOnSpline(i + increment) - position).normalized;
					right = Vector3.Cross(forward, Vector3.up);
				}
				position += right * ((side++ % 2) == 0 ? -2 : 2);
				bumper.transform.position = position;
			}
		}

		private void AddCollectables()
		{
			float quarterWidth = Mesh.xyScale.x * 0.25f;
			for(int i = 0; i < Spline.splineNodesArray.Count; ++i)
			{
				if(i == 0 || i == Spline.splineNodesArray.Count - 1)
				{
					continue;
				}
				SplineNode node = Spline.splineNodesArray[i];
				GameObject collectable = ObjectManager.Instance.GetLoadedGameObject("Game/Collectables/Collectable");
				Vector3 position = node.transform.position + Vector3.up;
				position.x += Random.Range(-quarterWidth, quarterWidth);
				collectable.transform.position = position;
				collectable.transform.parent = transform;
				collectables.Add(collectable);
			}
		}

		/// <summary>
		/// Adds the node.
		/// </summary>
		/// <returns>The node.</returns>
		/// <param name="lastNode">Last node.</param>
		/// <param name="offsetTime">Offset time.</param>
		private SplineNode AddNode(Spline spline, SplineNode lastNode, float offsetTime, Vector3 position, bool forceUsePosition, int nodeIndex)
		{
			// Validate offset between 0 and 1
			offsetTime = Mathf.Clamp(offsetTime, 0.0f, 1.0f);				
			
			// Add node
			SplineNode newNode = spline.AddSplineNode(lastNode).GetComponent<SplineNode>();

			// If we are not the first node, offset
			if(lastNode != null && !forceUsePosition)
			{
				position = lastNode.transform.position + Vector3.forward * ForwardDistanceToNextNode;

				// Horizontal
				position += Vector3.right * Random.Range(MinHorizontalOffset, MaxHorizontalOffset);
				
				// Vertical
				position += Vector3.up * Random.Range(MinVerticalOffset, MaxVerticalOffset);
			}
			
			// Set name
			newNode.gameObject.name = "Node_" + nodeIndex.ToString();;

			// Set parent and position
			newNode.transform.parent = transform;
			newNode.transform.position = position;
			
			return newNode;
		}
	}
}

