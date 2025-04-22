using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // Enum to define object types
    public enum ObjectType
    {
        Platform,
        Coin,
        Diamond
    }

    [System.Serializable]
    public class ObjectTypePrefabPair
    {
        public int poolSize; // Maximum size of the pool
        public ObjectType type;
        public GameObject prefab;
    }

    // List to map ObjectType to prefabs (visible in the Inspector)
    [SerializeField] private List<ObjectTypePrefabPair> prefabMapping;

    // Dictionary to hold multiple object pools for different object types
    private Dictionary<ObjectType, List<GameObject>> pools = new Dictionary<ObjectType, List<GameObject>>();

    void Start()
    {
        InitializePool(ObjectType.Platform);
    }


    /// <summary>
    /// Initializes a pool for a specific object type with a given size.
    /// </summary>
    /// <param name="type">The type of object to pool.</param>
    public void InitializePool(ObjectType type)
    {
        if (!pools.ContainsKey(type))
        {
            ObjectTypePrefabPair pair = GetPrefabPairForType(type);
            if (pair == null)
            {
                Debug.LogError($"No prefab assigned for ObjectType: {type}");
                return;
            }

            pools[type] = new List<GameObject>();

            for (int i = 0; i < pair.poolSize; i++)
            {
                GameObject obj = Instantiate(pair.prefab);
                obj.SetActive(false); // Deactivate the object initially
                pools[type].Add(obj);
            }
        }
    }

    /// <summary>
    /// Retrieves an inactive object from the pool for the given object type.
    /// If no inactive object is available, it reuses the oldest object in the pool.
    /// </summary>
    /// <param name="type">The type of object to retrieve from the pool.</param>
    /// <returns>A GameObject from the pool.</returns>
    public GameObject GetObject(ObjectType type)
    {
        if (pools.ContainsKey(type))
        {
            // Try to find an inactive object
            foreach (GameObject obj in pools[type])
            {
                if (!obj.activeInHierarchy)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }

            // If no inactive object is available, reuse the oldest object
            GameObject oldestObject = pools[type][0];
            pools[type].RemoveAt(0); // Remove it from the front
            pools[type].Add(oldestObject); // Add it to the back
            oldestObject.SetActive(true);
            return oldestObject;
        }
        else
        {
            Debug.LogError($"No pool found for ObjectType: {type}");
            return null;
        }
    }

    /// <summary>
    /// Returns an object to the pool by deactivating it.
    /// </summary>
    /// <param name="obj">The GameObject to return to the pool.</param>
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    /// <summary>
    /// Gets the prefab pair associated with a specific ObjectType.
    /// </summary>
    /// <param name="type">The ObjectType to look up.</param>
    /// <returns>The associated ObjectTypePrefabPair, or null if not found.</returns>
    private ObjectTypePrefabPair GetPrefabPairForType(ObjectType type)
    {
        foreach (var pair in prefabMapping)
        {
            if (pair.type == type)
            {
                return pair;
            }
        }

        return null;
    }
}
