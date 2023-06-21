using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MPewsey.ObjectPool
{
    public class AddressableObjectPool : MonoBehaviour
    {
        private static Dictionary<object, List<AddressableObjectPool>> Pools { get; } = new Dictionary<object, List<AddressableObjectPool>>();

        [SerializeField] private int _initialCapacity;
        [SerializeField] private AssetReferenceGameObject _prefab;

        private AsyncOperationHandle<GameObject> Handle { get; set; }
        private Stack<GameObject> Pool { get; } = new Stack<GameObject>();
        public int InitialCapacity { get => _initialCapacity; set => _initialCapacity = Mathf.Max(value, 0); }
        public AssetReferenceGameObject Prefab { get => _prefab; private set => _prefab = value; }

        private void OnValidate()
        {
            InitialCapacity = InitialCapacity;
        }

        private void Awake()
        {
            RegisterPool();
            InitializePool();
        }

        private void OnDestroy()
        {
            UnregisterPool();
            ReleaseHandle();
        }

        public int Count()
        {
            return Pool.Count;
        }

        private void ReleaseHandle()
        {
            if (Handle.IsValid())
            {
                Addressables.Release(Handle);
                Handle = new AsyncOperationHandle<GameObject>();
            }
        }

        private void InitializePool()
        {
            if (Prefab.RuntimeKeyIsValid())
            {
                Handle = Prefab.LoadAssetAsync<GameObject>();
                Handle.Completed += OnLoadComplete;
            }
        }

        private void OnLoadComplete(AsyncOperationHandle<GameObject> handle)
        {
            var prefab = handle.Result;

            while (Pool.Count < InitialCapacity)
            {
                var obj = Instantiate(prefab, transform);
                obj.SetActive(false);
                Pool.Push(obj);
            }
        }

        private void RegisterPool()
        {
            if (Prefab.RuntimeKeyIsValid())
            {
                var key = Prefab.RuntimeKey;

                if (!Pools.TryGetValue(key, out var pools))
                {
                    pools = new List<AddressableObjectPool>();
                    Pools.Add(key, pools);
                }

                pools.Add(this);
            }
        }

        private void UnregisterPool()
        {
            if (Prefab.RuntimeKeyIsValid() && Pools.TryGetValue(Prefab.RuntimeKey, out var pools))
                pools.Remove(this);
        }

        public GameObject GetObject(Transform parent)
        {
            if (Pool.Count > 0)
            {
                var obj = Pool.Pop();
                obj.transform.SetParent(parent);
                obj.SetActive(true);
                return obj;
            }

            var prefab = Handle.IsDone ? Handle.Result : Handle.WaitForCompletion();
            return Instantiate(prefab, parent);
        }

        public T GetObject<T>(Transform parent)
        {
            return GetObject(parent).GetComponent<T>();
        }

        public void ReturnObject(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            Pool.Push(obj);
        }

        public static AddressableObjectPoolHandle GetPoolHandle(AssetReferenceGameObject prefab)
        {
            if (!prefab.RuntimeKeyIsValid())
                throw new System.ArgumentException($"Prefab runtime key is not valid: {prefab}.");

            var key = prefab.RuntimeKey;

            if (!Pools.TryGetValue(key, out var pools))
            {
                pools = new List<AddressableObjectPool>();
                Pools.Add(key, pools);
            }

            return new AddressableObjectPoolHandle(prefab, pools);
        }

        public static AddressableObjectPool Create(AssetReferenceGameObject prefab, int initialCapacity = 0)
        {
            var obj = new GameObject("Addressable Object Pool");
            obj.SetActive(false);
            DontDestroyOnLoad(obj);
            var pool = obj.AddComponent<AddressableObjectPool>();
            pool.InitialCapacity = initialCapacity;
            pool.Prefab = prefab;
            obj.SetActive(true);
            return pool;
        }
    }
}