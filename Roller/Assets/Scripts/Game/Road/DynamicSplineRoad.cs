using UnityEngine;
using System.Collections.Generic;
namespace Game
{
	public class DynamicSplineRoad : BaseSplineRoad
	{
		public float ForwardDistanceToNextNode = 50.0f;

		// Horizontal offsets per node
		public float StartingMinHorizontalOffset = -50.0f;
		public float StartingMaxHorizontalOffset = 50.0f;
		public float EndingMinHorizontalOffset = -50.0f;
		public float EndingMaxHorizontalOffset = 50.0f;

		// Vertical offsets per node
		public float StartingMinVerticalOffset = -10.0f;
		public float StartingMaxVerticalOffset = 10.0f;
		public float EndingMinVerticalOffset = -10.0f;
		public float EndingMaxVerticalOffset = 10.0f;

		public float MaxSplineLength = 5000.0f;

		public float PercentageOfNarrowSections = 0.5f;

		public override void Populate()
		{
			SplineNode lastNode = gameObject.transform.FindChild("Node2").gameObject.GetComponent<SplineNode>();
			float splineLength = Spline.Length;
			while(splineLength < MaxSplineLength)
			{
				float time = Mathf.Clamp(splineLength / MaxSplineLength, 0.0f, 1.0f);

				// Insert node
				lastNode = AddNode(Spline, lastNode, time);

				// Update spline
				Spline.UpdateSpline();

				// Recalc length
				splineLength = Spline.Length;
			}
			base.Populate();
		}

		/// <summary>
		/// Adds the node.
		/// </summary>
		/// <returns>The node.</returns>
		/// <param name="lastNode">Last node.</param>
		/// <param name="offsetTime">Offset time.</param>
		private SplineNode AddNode(Spline spline, SplineNode lastNode, float offsetTime)
		{
			// Validate offset between 0 and 1
			offsetTime = Mathf.Clamp(offsetTime, 0.0f, 1.0f);

			// Get last node position
			Vector3 lastPosition = lastNode == null ? new Vector3(0, 0, -ForwardDistanceToNextNode) : lastNode.transform.position;						

			// Add node
			SplineNode newNode = spline.AddSplineNode(lastNode).GetComponent<SplineNode>();
			
			// Offsets
			Vector3 position = lastPosition + Vector3.forward * ForwardDistanceToNextNode;

			// Horizontal
			float min = Mathf.Lerp(StartingMinHorizontalOffset, EndingMinHorizontalOffset, offsetTime);
			float max = Mathf.Lerp(StartingMaxHorizontalOffset, EndingMaxHorizontalOffset, offsetTime);
			position += Vector3.right * Random.Range(min, max);

			// Vertical
			min = Mathf.Lerp(StartingMinVerticalOffset, EndingMinVerticalOffset, offsetTime);
			max = Mathf.Lerp(StartingMaxVerticalOffset, EndingMaxVerticalOffset, offsetTime);
			position += Vector3.up * Random.Range(min, max);

			// Set parent
			newNode.transform.parent = transform;

			// Set name
			newNode.gameObject.name = "Node" + (spline.splineNodesArray.Count - 1);

			// Set position
			newNode.transform.position = position;

			return newNode;
		}
	}
}

