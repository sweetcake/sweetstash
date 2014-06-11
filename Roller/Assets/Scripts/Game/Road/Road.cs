using UnityEngine;
using System.Collections.Generic;
using Core;
namespace Game
{
	public enum ConnectorTypes
	{
		SmallConnector,
		MediumConnector,
		LargeConnector,
		SmallToLargeConnector,
		SmallToMediumConnector,
		MediumToSmallConnector,
		MediumToLargeConnector,
		LargeToSmallConnector,
		LargeToMediumConnector
	}

	// Used by connectors
	public enum RoadWidths
	{
		SmallRoad,
		MediumRoad,
		LargeRoad
	}

	public class Road : MonoBehaviour
	{
		public int SegmentCount = 100;
		public int TargetSegmentLength = 25;

		public Transform StartPoint;
		public List<RoadConnectorData> ConnectorData;

		// Quick lookups. Filled in Awake()
		private Dictionary<ConnectorTypes, RoadWidths> connectorStartWidthMap = new Dictionary<ConnectorTypes, RoadWidths>();
		private Dictionary<ConnectorTypes, RoadWidths> connectorEndWidthMap = new Dictionary<ConnectorTypes, RoadWidths>();
		private Dictionary<RoadWidths, List<ConnectorTypes>> roadWidthToConnectorStartMap = new Dictionary<RoadWidths, List<ConnectorTypes>>();
		private Dictionary<RoadWidths, List<ConnectorTypes>> roadWidthToConnectorEndMap = new Dictionary<RoadWidths, List<ConnectorTypes>>();

		/// <summary>
		/// Initialization
		/// </summary>
		void Awake()
		{
			InitializeMaps();
			LoadAssets();
		}

		/// <summary>
		/// Initializes the maps.
		/// </summary>
		private void InitializeMaps()
		{
			// Fill in our maps for quick lookups
			foreach(RoadConnectorData data in ConnectorData)
			{
				if(!connectorStartWidthMap.ContainsKey(data.ConnectorType))
				{
					connectorStartWidthMap.Add(data.ConnectorType, data.StartRoadWidth);
				}
				
				if(!connectorEndWidthMap.ContainsKey(data.ConnectorType))
				{
					connectorEndWidthMap.Add(data.ConnectorType, data.EndRoadWidth);
				}
				
				// Initialize list for this width
				if(!roadWidthToConnectorStartMap.ContainsKey(data.StartRoadWidth))
				{
					roadWidthToConnectorStartMap.Add(data.StartRoadWidth, new List<ConnectorTypes>());
				}
				
				// Add connector type if we don't have it already
				if(!roadWidthToConnectorStartMap[data.StartRoadWidth].Contains(data.ConnectorType))
				{
					roadWidthToConnectorStartMap[data.StartRoadWidth].Add(data.ConnectorType);
				}
				
				
				// Initialize list for this end width
				if(!roadWidthToConnectorEndMap.ContainsKey(data.EndRoadWidth))
				{
					roadWidthToConnectorEndMap.Add(data.EndRoadWidth, new List<ConnectorTypes>());
				}
				
				// Add connector type if we don't have it already
				if(!roadWidthToConnectorEndMap[data.StartRoadWidth].Contains(data.ConnectorType))
				{
					roadWidthToConnectorEndMap[data.StartRoadWidth].Add(data.ConnectorType);
				}
			}
		}

		/// <summary>
		/// Loads assets that we will use later
		/// </summary>
		private void LoadAssets()
		{
			// Roads
			foreach(RoadWidths width in EnumUtil.GetValues<RoadWidths>())
			{
				ObjectManager.Instance.LoadObject(GetRoadPath(width));
			}

			// Connector types
			foreach(ConnectorTypes connectorType in EnumUtil.GetValues<ConnectorTypes>())
			{
				ObjectManager.Instance.LoadObject(GetConnectorPath(connectorType));
			}
		}

		/// <summary>
		/// Populate this road
		/// </summary>
		public virtual void Populate()
		{
			// Randomly create our first connector
			RoadConnector lastConnector = CreateConnector(ConnectorTypes.LargeConnector);//GetRandomConnectorType());
			lastConnector.transform.parent = transform;
			lastConnector.transform.position = Vector3.zero;
			lastConnector.name += "_0";

			// Add two major chunks
			for(int i = 0; i < SegmentCount; ++i)
			{
				RoadSegment segment = CreateRoad(GetRoadWidthForConnectorEnd(lastConnector.Type));
				segment.transform.parent = transform;
				RoadConnector nextConnector = CreateConnector(GetRandomConnectorStartForRoadWidth(segment.Width));
				nextConnector.name += "_" + (i + 1).ToString();
				nextConnector.transform.parent = transform;
				segment.Populate(TargetSegmentLength, lastConnector, nextConnector, i);
				lastConnector = nextConnector;
			}
		}

		/// <summary>
		/// Returns a new connector from the given type
		/// </summary>
		/// <returns>The connector.</returns>
		/// <param name="type">Type.</param>
		private RoadConnector CreateConnector(ConnectorTypes type)
		{
			return ObjectManager.Instance.GetLoadedGameObject(GetConnectorPath(type)).GetComponent<RoadConnector>();
		}

		/// <summary>
		/// Gets the connector  asset path.
		/// </summary>
		/// <returns>The connector path.</returns>
		/// <param name="type">Type.</param>
		private string GetConnectorPath(ConnectorTypes type)
		{
			return "Game/Roads/Connectors/" + type.ToString();
		}

		/// <summary>
		/// Creates the road.
		/// </summary>
		/// <returns>The road.</returns>
		/// <param name="width">Width.</param>
		private RoadSegment CreateRoad(RoadWidths width)
		{
			return ObjectManager.Instance.GetLoadedGameObject(GetRoadPath(width)).GetComponent<RoadSegment>();
		}

		/// <summary>
		/// Gets the road asset path.
		/// </summary>
		/// <returns>The road path.</returns>
		/// <param name="width">Width.</param>
		private string GetRoadPath(RoadWidths width)
		{
			return "Game/Roads/" + width.ToString();
		}

		/// <summary>
		/// Gets the road width for connector start.
		/// </summary>
		/// <returns>The road width for connector start.</returns>
		/// <param name="connectorType">Connector type.</param>
		private RoadWidths GetRoadWidthForConnectorStart(ConnectorTypes connectorType)
		{
			if(connectorStartWidthMap.ContainsKey(connectorType))
			{
				return connectorStartWidthMap[connectorType];
			}
			return RoadWidths.MediumRoad;
		}

		/// <summary>
		/// Gets the road width for connector end.
		/// </summary>
		/// <returns>The road width for connector end.</returns>
		/// <param name="connectorType">Connector type.</param>
		private RoadWidths GetRoadWidthForConnectorEnd(ConnectorTypes connectorType)
		{
			if(connectorStartWidthMap.ContainsKey(connectorType))
			{
				return connectorEndWidthMap[connectorType];
			}
			return RoadWidths.MediumRoad;
		}

		/// <summary>
		/// Gets a random connector type that starts at the given width
		/// </summary>
		/// <returns>The random connector start for road width.</returns>
		/// <param name="roadWidth">Road width.</param>
		private ConnectorTypes GetRandomConnectorStartForRoadWidth(RoadWidths roadWidth)
		{
			if(roadWidthToConnectorStartMap.ContainsKey(roadWidth) && roadWidthToConnectorStartMap[roadWidth].Count > 0)
			{
				return roadWidthToConnectorStartMap[roadWidth][Random.Range(0, roadWidthToConnectorStartMap[roadWidth].Count)];
			}
			return ConnectorTypes.MediumConnector;
		}

		/// <summary>
		/// Gets a random connector type that ends at the given width
		/// </summary>
		/// <returns>The random connector start for road width.</returns>
		/// <param name="roadWidth">Road width.</param>
		private ConnectorTypes GetRandomConnectorEndForRoadWidth(RoadWidths roadWidth)
		{
			if(roadWidthToConnectorEndMap.ContainsKey(roadWidth) && roadWidthToConnectorEndMap[roadWidth].Count > 0)
			{
				return roadWidthToConnectorEndMap[roadWidth][Random.Range(0, roadWidthToConnectorEndMap[roadWidth].Count)];
			}
			return ConnectorTypes.MediumConnector;
		}

		/// <summary>
		/// Gets the random connector.
		/// </summary>
		/// <returns>The random connector.</returns>
		private ConnectorTypes GetRandomConnectorType()
		{
			System.Array types = System.Enum.GetValues(typeof(ConnectorTypes));
			return (ConnectorTypes)types.GetValue(Random.Range(0, types.Length));
		}
	}

	[System.Serializable]
	public class RoadConnectorData
	{
		public ConnectorTypes ConnectorType;
		public RoadWidths StartRoadWidth = RoadWidths.SmallRoad;
		public RoadWidths EndRoadWidth = RoadWidths.SmallRoad;
	}
}

