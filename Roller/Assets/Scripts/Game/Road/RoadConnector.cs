using UnityEngine;
using System.Collections.Generic;
namespace Game
{
	public class RoadConnector : MonoBehaviour
	{
		// Node connectors
		public Transform s0;
		public Transform s1;
		public Transform s2;

		public Transform e0;
		public Transform e1;
		public Transform e2;

		public ConnectorTypes Type;

		// Node containers
		private List<Transform> startNodes = new List<Transform>();
		private List<Transform> endNodes = new List<Transform>();

		/// <summary>
		/// Init
		/// </summary>
		void Awake()
		{
			startNodes.Add(s0);
			startNodes.Add(s1);
			startNodes.Add(s2);
			endNodes.Add(e0);
			endNodes.Add(e1);
			endNodes.Add(e2);
		}

		/// <summary>
		/// Gets the starting node by given an index
		/// </summary>
		/// <returns>The end node by index.</returns>
		/// <param name="index">Index.</param>
		public Transform GetStartNodeByIndex(int index)
		{
			if(index < 0 || index >= startNodes.Count)
			{
				return null;
			}
			return startNodes[index];
		}

		/// <summary>
		/// Gets the ending node by given an index
		/// </summary>
		/// <returns>The end node by index.</returns>
		/// <param name="index">Index.</param>
		public Transform GetEndNodeByIndex(int index)
		{
			if(index < 0 || index >= endNodes.Count)
			{
				return null;
			}
			return endNodes[index];
		}
	}
}

