using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Tooltip("The GameObject prefab to be pooled.")]
    public GameObject pooledObject = null;

    [Tooltip("The size which the pool will on itializaed to on Start.")]
    public int startSize = 100;

    [Tooltip("Is the pool allowed to grow if no pooled objects are available when requested?")]
    public bool canGrow = true;

    [Tooltip("Should this pool destroy all of its pooled objects when it is destroyed?")]
    public bool destroyPooledObjects = true;

    [Tooltip("This current size of the pool.")]
    public int size = 0;

    List<GameObject> pooledObjects;
    bool initialized = false;

    static Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();
    
    void Awake() {
        TryAutoInitialize();
    }

    void Start() {
        TryAutoInitialize();
    }

    public GameObject GetPooledObject(bool activate = true) {      
        foreach (GameObject obj in pooledObjects) {
            if (obj != null && !obj.activeInHierarchy) {
                obj.transform.parent = transform;
                obj.SetActive(activate);
                return obj;
            }
        }
        if (canGrow) {
            GameObject obj = AddPooledObject();
            obj.SetActive(activate);
            return obj;
        }
        return null;
    }

    public GameObject AddPooledObject() {
        GameObject obj = (GameObject)Instantiate(pooledObject);
        obj.transform.parent = transform;
        obj.SetActive(false);
        pooledObjects.Add(obj);
        size++;
        return obj;
    }

    public void AddPooledObjects(int count) {
        for (int i = 0; i < count; ++i) 
            AddPooledObject();
    }

    public void ReturnAllPooledObjects() {
        foreach (GameObject obj in pooledObjects) {
            if (obj != null) {
                obj.transform.parent = transform;
                obj.SetActive(false);
            }
        }
    }

    void OnDestroy() {
        if (initialized)
        {
            if (destroyPooledObjects)
            {
                foreach (GameObject obj in pooledObjects)
                    Destroy(obj);
            }
            else
            {
                foreach (GameObject obj in pooledObjects)
                {
                    if (obj.transform.parent = this.transform)
                        obj.transform.parent = null;
                }
            }
            pools.Remove(this.pooledObject.name);
        }
    }

    void TryAutoInitialize() {
        if (!initialized && pooledObject != null) {
            if (!Initialize()) {
                Debug.LogError("ObjectPool for pooled object \"" + pooledObject.name + "\" already exists! This ObjectPool will be destroyed.");
                Destroy(this);
            }
        }
    }

    bool Initialize() {
        if (pools.ContainsKey(this.pooledObject.name)) 
            return false;
        else {
            pools.Add(this.pooledObject.name, this);
            pooledObjects = new List<GameObject>();
            AddPooledObjects(startSize);
            this.initialized = true;
            return true;
        }
    }

    public static ObjectPool CreateFor(GameObject pooledObject, int startSize = 100, bool canGrow = true, bool destroyPooledObjects = true) {
        if (ExistsFor(pooledObject)) {
            Debug.LogError("ObjectPool for pooled object \"" + pooledObject.name + "\" already exists! Use GetFor instead.");
            return null;
        }
        GameObject go = new GameObject("ObjectPool_" + pooledObject.name);
        go.transform.Reset();
        go.transform.parent = null;
        ObjectPool objectPool = go.AddComponent<ObjectPool>();
        objectPool.pooledObject = pooledObject;
        objectPool.startSize = startSize;
        objectPool.canGrow   = canGrow;
        objectPool.destroyPooledObjects = destroyPooledObjects;
        objectPool.Initialize();
        return objectPool;        
    }

    public static ObjectPool GetFor(GameObject pooledObject, bool canCreate = true) {
        ObjectPool pool;
        if (pools.TryGetValue(pooledObject.name, out pool)) 
            return pool;
        else if (canCreate)
            return CreateFor(pooledObject);        
        else
            return null;
    }

    public static bool ExistsFor(GameObject pooledObject) {
        return pools.ContainsKey(pooledObject.name);
    }
}
