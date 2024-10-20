using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private static ObjectPool instance = null;
    public static ObjectPool Instance
    {
        get
        {
            if (instance == null) return null;
            return instance;
        }
    }

    class Pool
    {
        public GameObject Original { get; private set; }
        private Transform root;
        Queue<GameObject> poolQueue = new Queue<GameObject>();

        public void Init(GameObject original, int count = 10)
        {
            root = new GameObject { name = $"{original.name}Pool" }.transform;
            DontDestroyOnLoad(root);
            Original = original;
            original.transform.SetParent(root);
            for (int i = 0; i < count; i++) Push(Create());
        }

        GameObject Create()
        {
            GameObject go = Instantiate(Original);
            go.name = Original.name;
            return go;
        }

        public void Push(GameObject go)
        {
            go.transform.SetParent(root);
            go.SetActive(false);
            poolQueue.Enqueue(go);
        }

        public GameObject Pop(Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject go;
            if (poolQueue.Count > 0) go = poolQueue.Dequeue();
            else go = Create();
            go.transform.SetParent(parent);
            go.transform.localPosition = position;
            go.transform.localRotation = rotation;
            go.SetActive(true);
            return go;
        }
    }

    private Dictionary<string, Pool> objectPool = new Dictionary<string, Pool>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Update()
    {
        
    }

    public void CreatePool(string name)
    {
        if (objectPool.TryGetValue(name, out _)) return;
        Pool pool = new Pool();
        GameObject original = Instantiate(Resources.Load($"Prefabs/{name}") as GameObject);
        original.name = name;
        original.SetActive(false);
        pool.Init(original);
        objectPool.Add(name, pool);
    }

    public void Push(GameObject go)
    {
        Pool pool;
        if(!objectPool.TryGetValue(go.name, out pool))
        {
            Destroy(go);
            return;
        }
        pool.Push(go);
    }

    public GameObject Pop(string name, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        Pool pool;
        if(!objectPool.TryGetValue(name, out pool))
        {
            CreatePool(name);
            return objectPool[name].Pop(position, rotation, parent);
        }
        return pool.Pop(position, rotation, parent);
    }
}
