using UnityEngine;
using System.Timers;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
	public class ObjectManager 
	{
		//Singleton
		private static ObjectManager _instance;
		private static bool _allowInstantiation = false;
		
		//Prefab containers
		private Dictionary<string, GameObject> _objects = new Dictionary<string, GameObject>();
		
		// Object pools
		private Dictionary<string, ObjectPool> _objectPools = new Dictionary<string, ObjectPool>();
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Core.ObjectManager"/> class.
		/// </summary>
		public ObjectManager ()
		{
			if(!_allowInstantiation)
			{
				throw new UnityException("Attempting to re-intantiate ObjectManager singleton");
			}
		}
		
		#if UNITY_EDITOR
		/// <summary>
		/// Spews the state to a file in the temp folder
		/// </summary>
		public void SpewState()
		{
			string path = System.IO.Path.Combine("../", "ObjectPool-" + DateUtils.GetTimestamp() + ".txt");
			path = path.Replace(" ", string.Empty);
			List<string> includes = new List<string>{
				//				"JubblyWubbly",
				//				"Fruit",
				//				"Particles",
				//				"GameChunks",
				//				"ParallaxLayer",
				//				"FloatingText"
			};
			using (System.IO.StreamWriter file = System.IO.File.CreateText(path)) 
			{
				List<ObjectPool> spewPool = new List<ObjectPool>();
				int longestKeyLength = 0;
				foreach(ObjectPool op in _objectPools.Values)
				{
					if(includes.Count == 0)
					{
						if(op.Key.Length > longestKeyLength)
						{
							longestKeyLength = op.Key.Length;
						}
						spewPool.Add(op);
					}
					else
					{
						foreach(string includeName in includes)
						{
							if(op.Key.Contains(includeName))
							{
								if(op.Key.Length > longestKeyLength)
								{
									longestKeyLength = op.Key.Length;
								}
								spewPool.Add(op);
							}
						}
					}
				}
				
				foreach(ObjectPool op in spewPool)
				{
					file.WriteLine(op.Key.PadRight(longestKeyLength, '-') + 
					               " : InitialCnt:" + op.InitialPoolSize.ToString().PadLeft(3, '_')  + 
					               " | CurrentCnt = " + op.Objects.Count.ToString().PadLeft(3, '_') + 
					               " | MaxInPool = " + op.MaxInPool.ToString().PadLeft(3, '_') + 
					               " | Alloc = " + op.TotalAllocations.ToString().PadLeft(3, '_') + 
					               " | Delivered = " + op.TotalDelivered.ToString().PadLeft(3, '_') + 
					               " | Recycled = " + op.TotalRecycled.ToString().PadLeft(3, '_'));
				}
				file.Close();
			}
		}
		#endif
		
		/// <summary>
		/// Loads the object.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <param name="createPool">If set to <c>true</c> create pool.</param>
		/// <param name="poolSize">Pool size.</param>
		public GameObject LoadObject(string path, bool createPool = false, int poolSize = 0)
		{
			if(_objects.ContainsKey(path))
			{
				LogAndTryDebugBreak("ObjectManager - trying to load object that has already been loaded : " + path);
				return null;
			}
			
			try
			{
				// Load object
				GameObject prefab = Resources.Load(path, typeof(GameObject)) as GameObject;
				//LogAndTryDebugBreak("ObjectManager::LoadObject - loaded object at - " + path);
				if(prefab != null)
				{
					// Add game object to map
					_objects.Add(path, prefab);
					
					// Pool objects
					if(createPool && poolSize > 0)
					{
						CreatePool(path, poolSize);
					}
					
					return prefab;
				}
				else
				{
					LogAndTryDebugBreak("ObjectManager - Tried to load invalid gameobject at: " + path);
				}
			}
			catch(UnityException e)
			{
				LogAndTryDebugBreak("ObjectManager - Tried to load invalid gameobject at: " + path + ". Message: " + e.Message);
			}
			
			return null;
		}
		
		/// <summary>
		/// Gets the loaded game object.
		/// </summary>
		/// <returns>The loaded game object.</returns>
		/// <param name="path">Path.</param>
		/// <param name="fromPool">If set to <c>true</c> from pool.</param>
		public GameObject GetLoadedGameObject(string path, bool fromPool = true)
		{
			try
			{
				if(fromPool && _objectPools.ContainsKey(path))
				{
					return _objectPools[path].GetNext();
				}
				else
				{
					if(!_objects.ContainsKey(path))
					{
						LogAndTryDebugBreak("ObjectManager - trying to get loaded game object that is not loaded : " + path);
						return null;
					}
					else
					{
						//LogAndTryDebugBreak("ObjectManager - instantiating object at : " + path);
						return Instantiate(_objects[path]);
					}
				}
			}
			catch(UnityException e)
			{
				LogAndTryDebugBreak("ObjectManager - Could not load game object: " + path + " Error: " + e.Message);
				return null;
			}
		}
		
		/// <summary>
		/// Creates the pool of the given prefab
		/// </summary>
		/// <param name="prefab">Prefab.</param>
		/// <param name="poolSize">Pool size.</param>
		public void CreatePool(string path, int poolSize, GameObject prefab = null)
		{
			if(poolSize <= 0)
			{
				LogAndTryDebugBreak("ObjectManager - trying to pool objects with zero or negative count: " + path);
				return;
			}
			
			if(string.IsNullOrEmpty(path))
			{
				LogAndTryDebugBreak("ObjectManager - Trying to create a pool with an un-named prefab. Please add unique name: " + path);
				return;
			}
			
			// If this already exists, don't create another pool
			if(_objectPools.ContainsKey(path))
			{
				Debug.Log("ObjectManager - Trying to create a pool That already exists: " + path);
				return;
			}
			
			// Try to use prefab as loaded object
			if(prefab != null && !_objects.ContainsKey(path))
			{
				_objects.Add(path, prefab);
			}
			
			// Create our pool
			_objectPools.Add(path, new ObjectPool(path, poolSize));
		}
		
		/// <summary>
		/// Instantiate the specified prefab.
		/// </summary>
		/// <param name="prefab">Prefab.</param>
		public GameObject Instantiate(GameObject prefab)
		{
			GameObject go = GameObject.Instantiate(prefab) as GameObject;
			if(go != null)
			{
				go.name = go.name.Remove(go.name.IndexOf("(Clone)"));
			}
			return go;
		}
		
		/// <summary>
		/// Returns gameobject to pool as long as there is a pool to return it to
		/// </summary>
		/// <param name="go">Go.</param>
		public bool ReturnToPool(GameObject go)
		{
			// Get the path 
			string path = GetGameObjectPath(go);
			
			// There is no pool
			if(string.IsNullOrEmpty(path) || !_objectPools.ContainsKey(path))
			{
				LogAndTryDebugBreak("ObjectManager - attempted to return gameobject " + path + " to pool that has no pool");
				return false;
			}
			
			// Object is already in the pool
			if(_objectPools[path].Contains(go))
			{
				LogAndTryDebugBreak("ObjectManager - attempting to return gameobject: " + path + " to the pool more than once.");
				return true;
			}
			
			_objectPools[path].Recycle(go);
			return true;
		}
		
		/// <summary>
		/// Gets the game object path.
		/// </summary>
		/// <param name="go">Go.</param>
		private string GetGameObjectPath(GameObject go)
		{
			return go.GetComponent<PoolableObject>().AssetPath;
		}
		
		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear()
		{
			_objects.Clear();
			_objectPools.Clear();
		}
		
		/// <summary>
		/// Tries the break.
		/// </summary>
		private void LogAndTryDebugBreak(string log)
		{
			Debug.Log(log);
		}
		
		/// <summary>
		/// Singleton accessor
		/// </summary>
		/// <value>The instance.</value>
		public static ObjectManager Instance
		{
			get {
				if(_instance == null)
				{
					_allowInstantiation = true;
					_instance = new ObjectManager();
					_allowInstantiation = false;
				}
				return _instance;
			}
		}
		
		///Private class to manage objects in pool
		private class ObjectPool
		{
			public string Key;
			public int InitialPoolSize = 0;
			public int MaxInPool = 0;
			public int TotalAllocations = 0;
			public int TotalDelivered = 0;
			public int TotalRecycled = 0;
			public Stack<GameObject> Objects;
			
			/// <summary>
			/// Initializes a new instance of the <see cref="Core.ObjectManager+ObjectPool"/> class.
			/// </summary>
			/// <param name="key">Key.</param>
			/// <param name="initialPoolSize">Initial pool size.</param>
			public ObjectPool(string key, int initialPoolSize)
			{
				Key = key;
				InitialPoolSize = initialPoolSize;
				MaxInPool = initialPoolSize;
				Objects = new Stack<GameObject>(InitialPoolSize);
				for(int i = 0; i < InitialPoolSize; ++i)
				{
					Objects.Push(InstantiatePoolableObject());
				}
			}
			
			/// <summary>
			/// Instantiates the poolable object and returns it
			/// </summary>
			/// <returns>The poolable object.</returns>
			private GameObject InstantiatePoolableObject()
			{
				++TotalAllocations;
				GameObject go = ObjectManager.Instance.GetLoadedGameObject(Key, false);
				PostInstantiate(go);
				return go;
			}
			
			/// <summary>
			/// Returns true if go is already in the pool
			/// </summary>
			/// <param name="go">Go.</param>
			public bool Contains(GameObject go)
			{
				return Objects.Contains(go);
			}
			
			/// <summary>
			/// Gets the next.
			/// </summary>
			/// <returns>The next.</returns>
			public GameObject GetNext()
			{
				// Get from pool, or instantiate
				GameObject go = Objects.Count > 0 ? Objects.Pop() : InstantiatePoolableObject();
				
				// Activate
				go.SetActive(true);
				
				// Reset poolable components in object
				foreach(PoolableObject ro in go.GetComponents<PoolableObject>())
				{
					ro.Reset();
				}
				
				++TotalDelivered;
				return go;
			}
			
			/// <summary>
			/// Recycle the specified go.
			/// </summary>
			/// <param name="go">Go.</param>
			public void Recycle(GameObject go)
			{
				go.SetActive(false);
				Objects.Push(go);
				UpdateMax();
				++TotalRecycled;
			}
			
			/// <summary>
			/// Updates the max.
			/// </summary>
			private void UpdateMax()
			{
				if(Objects.Count > MaxInPool)
				{
					MaxInPool = Objects.Count;
				}
			}
			
			/// <summary>
			/// Post processing on new game objects
			/// </summary>
			/// <param name="go">Go.</param>
			private void PostInstantiate(GameObject go)
			{
				if(go == null)
				{
					Debug.Log("ObjectPool - PostInstantiate called on null object");
					return;
				}
				
				// Set name
				SetAssetPath(go);
				
				go.SetActive(false);
			}
			
			/// <summary>
			/// Tries the set game object path.
			/// </summary>
			/// <param name="go">Go.</param>
			/// <param name="path">Path.</param>
			private void SetAssetPath(GameObject go)
			{
				PoolableObject po = go.GetComponent<PoolableObject>();
				if(po == null)
				{
					po = go.AddComponent<PoolableObject>();
				}
				po.AssetPath = Key;
			}
			
			/// <summary>
			/// Gets a value indicating whether this instance is empty.
			/// </summary>
			/// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
			public bool IsEmpty
			{
				get { return Objects.Count == 0; }
			}
		}
	}
}
