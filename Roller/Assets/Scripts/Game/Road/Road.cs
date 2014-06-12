using UnityEngine;
using System.Collections.Generic;
using Core;
namespace Game
{
	public enum ConnectorTypes
	{
		SmallConnector,
		SmallDropoffConnector,
		MediumConnector,
		MediumDropoffConnector,
		LargeConnector,
		LargeDropoffConnector,
		SmallToLargeConnector,
		SmallToMediumConnector,
		MediumToSmallConnector,
		MediumToLargeConnector,
		LargeToSmallConnector,
		LargeToMediumConnector,
		LargeRotatingCylinderConnector,
		LargeSplitConnector,
		LargeToLargeTightrope
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

		public ConnectorTypes TestConnector = ConnectorTypes.LargeRotatingCylinderConnector;
		public bool UseTestConnector = false;

		public Transform StartPoint;
		public List<RoadConnectorData> ConnectorData;

		// Quick lookups. Filled in Awake()
		private Dictionary<ConnectorTypes, RoadWidths> connectorStartWidthMap = new Dictionary<ConnectorTypes, RoadWidths>();
		private Dictionary<ConnectorTypes, RoadWidths> connectorEndWidthMap = new Dictionary<ConnectorTypes, RoadWidths>();
		private Dictionary<RoadWidths, List<RoadConnectorData>> roadWidthToConnectorStartMap = new Dictionary<RoadWidths, List<RoadConnectorData>>();
		private Dictionary<RoadWidths, List<RoadConnectorData>> roadWidthToConnectorEndMap = new Dictionary<RoadWidths, List<RoadConnectorData>>();

		private List<RoadConnector> connectors;
		private List<RoadSegment> segments;

		// Helps add variety to our connector types
		private Dictionary<ConnectorTypes, bool> usedTypes = new Dictionary<ConnectorTypes, bool>();

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
					roadWidthToConnectorStartMap.Add(data.StartRoadWidth, new List<RoadConnectorData>());
				}
				
				// Add connector type if we don't have it already
				if(!roadWidthToConnectorStartMap[data.StartRoadWidth].Contains(data))
				{
					roadWidthToConnectorStartMap[data.StartRoadWidth].Add(data);
				}
				
				
				// Initialize list for this end width
				if(!roadWidthToConnectorEndMap.ContainsKey(data.EndRoadWidth))
				{
					roadWidthToConnectorEndMap.Add(data.EndRoadWidth, new List<RoadConnectorData>());
				}
				
				// Add connector type if we don't have it already
				if(!roadWidthToConnectorEndMap[data.StartRoadWidth].Contains(data))
				{
					roadWidthToConnectorEndMap[data.StartRoadWidth].Add(data);
				}
			}
		}

		public void Reset()
		{
			foreach(RoadSegment segment in segments)
			{
				segment.Reset();
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

			ObjectManager.Instance.LoadObject("Game/Perils/Bumper");
			ObjectManager.Instance.LoadObject("Game/Collectables/Collectable");
		}

		/// <summary>
		/// Populate this road
		/// </summary>
		public virtual void Populate()
		{
			Cleanup();

			// Instantiate connector and segment containers
			connectors = new List<RoadConnector>();
			segments = new List<RoadSegment>();

			// Determine if we are using a test connector
			//ConnectorTypes startingConnector = UseTestConnector ? TestConnector : ConnectorTypes.LargeConnector;

			RoadConnector lastConnector = CreateConnector(ConnectorTypes.LargeConnector);
			lastConnector.transform.parent = transform;
			lastConnector.transform.position = Vector3.zero;
			lastConnector.name += "_0";
			connectors.Add(lastConnector);

			// Add two major chunks
			for(int i = 0; i < SegmentCount; ++i)
			{
				RoadSegment segment = CreateRoad(GetRoadWidthForConnectorEnd(lastConnector.Type));
				segments.Add(segment);
				segment.transform.parent = transform;
				RoadConnector nextConnector = CreateConnector(GetRandomConnectorStartForRoadWidth(segment.Width));
				connectors.Add(nextConnector);
				nextConnector.name += "_" + (i + 1).ToString();
				nextConnector.transform.parent = transform;
				segment.Populate(TargetSegmentLength, lastConnector, nextConnector, i);
				lastConnector = nextConnector;
			}
		}

		/// <summary>
		/// Cleanup this instance.
		/// </summary>
		private void Cleanup()
		{
			if(connectors != null)
			{
				foreach(RoadConnector connector in connectors)
				{
					Destroy(connector.gameObject);
				}
				connectors = null;
			}
			if(segments != null)
			{
				foreach(RoadSegment segment in segments)
				{
					Destroy(segment.gameObject);
				}
				segments = null;
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
			if(UseTestConnector)
			{
				return TestConnector;
			}

			if(roadWidthToConnectorStartMap.ContainsKey(roadWidth) && roadWidthToConnectorStartMap[roadWidth].Count > 0)
			{
				ConnectorTypes type = GetRandomConnectorData(roadWidthToConnectorStartMap[roadWidth]).ConnectorType;
				usedTypes.Add(type, true);
				return type;
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
			if(UseTestConnector)
			{
				return TestConnector;
			}

			if(roadWidthToConnectorEndMap.ContainsKey(roadWidth) && roadWidthToConnectorEndMap[roadWidth].Count > 0)
			{
				ConnectorTypes type = GetRandomConnectorData(roadWidthToConnectorEndMap[roadWidth]).ConnectorType;
				usedTypes.Add(type, true);
				return type;
			}
			return ConnectorTypes.MediumConnector;
		}

		/// <summary>
		/// Gets the random connector data.
		/// </summary>
		/// <returns>The random connector data.</returns>
		/// <param name="roadConnectorList">Road connector list.</param>
		public RoadConnectorData GetRandomConnectorData(List<RoadConnectorData> roadConnectorList)
		{
			List<RoadConnectorData> usableList = new List<RoadConnectorData>();
			float totalWeight = 0;
			foreach(RoadConnectorData data in roadConnectorList)
			{
				if(!usedTypes.ContainsKey(data.ConnectorType))
				{
					totalWeight += data.SpawnWeighting;
					usableList.Add(data);
				}
			}

			// We have used all the available ones, go clean up the list
			if(usableList.Count == 0 || totalWeight <= Mathf.Epsilon)
			{
				usedTypes.Clear();
				usableList = roadConnectorList;
			}

			totalWeight = 0;
			foreach(RoadConnectorData data in usableList)
			{
				totalWeight += data.SpawnWeighting;
			}
			
			float random = Random.Range(0.0f, totalWeight);
			
			int index = -1;
			for(int i = 0; i< usableList.Count; ++i) 
			{
				if(random <= usableList[i].SpawnWeighting) 
				{ 
					index = i; 
					break; 
				}
				random -= usableList[i].SpawnWeighting;
			}
			
			if(index == -1) 
			{
				index = usableList.Count - 1;
			}
			
			return usableList[index];
		}
	}

	[System.Serializable]
	public class RoadConnectorData
	{
		public ConnectorTypes ConnectorType;
		public RoadWidths StartRoadWidth = RoadWidths.SmallRoad;
		public RoadWidths EndRoadWidth = RoadWidths.SmallRoad;
		public int SpawnWeighting = 100;
	}
}

