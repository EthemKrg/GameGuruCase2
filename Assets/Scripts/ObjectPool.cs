using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // Enum to define object types
    public enum ObjectType
    {
        Platform,
        Enemy,
        Projectile
    }

    [System.Serializable]
    public class ObjectTypePrefabPair
    {
        public ObjectType type;
        public GameObject prefab;
    }

    // List to map ObjectType to prefabs (visible in the Inspector)
    [SerializeField] private List<ObjectTypePrefabPair> prefabMapping;

    // Dictionary to hold multiple object pools for different object types
    private Dictionary<ObjectType, List<GameObject>> pools = new Dictionary<ObjectType, List<GameObject>>();

    /// <summary>
    /// Initializes a pool for a specific object type with a given size.
    /// </summary>
    /// <param name="type">The type of object to pool.</param>
    /// <param name="initialSize">The initial size of the pool.</param>
    public void InitializePool(ObjectType type, int initialSize)
    {
        if (!pools.ContainsKey(type))
        {
            pools[type] = new List<GameObject>();

            GameObject prefab = GetPrefabForType(type);
            if (prefab == null)
            {
                Debug.LogError($"No prefab assigned for ObjectType: {type}");
                return;
            }

            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false); // Deactivate the object initially
                pools[type].Add(obj);
            }
        }
    }

    /// <summary>
    /// Retrieves an inactive object from the pool for the given object type.
    /// If no inactive object is available, a new one is created.
    /// </summary>
    /// <param name="type">The type of object to retrieve from the pool.</param>
    /// <returns>A GameObject from the pool.</returns>
    public GameObject GetObject(ObjectType type)
    {
        if (pools.ContainsKey(type))
        {
            foreach (GameObject obj in pools[type])
            {
                if (!obj.activeInHierarchy)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }

            // If no inactive object is available, create a new one
            GameObject prefab = GetPrefabForType(type);
            if (prefab != null)
            {
                GameObject newObj = Instantiate(prefab);
                newObj.SetActive(true);
                pools[type].Add(newObj);
                return newObj;
            }
            else
            {
                Debug.LogError($"No prefab assigned for ObjectType: {type}");
                return null;
            }
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
    /// Gets the prefab associated with a specific ObjectType.
    /// </summary>
    /// <param name="type">The ObjectType to look up.</param>
    /// <returns>The associated prefab, or null if not found.</returns>
    private GameObject GetPrefabForType(ObjectType type)
    {
        foreach (var pair in prefabMapping)
        {
            if (pair.type == type)
            {
                return pair.prefab;
            }
        }

        return null;
    }
}
