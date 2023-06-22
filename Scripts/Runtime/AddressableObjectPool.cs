using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MPewsey.ObjectPool
{
    public class AddressableObjectPool : MonoBehaviour
    {
        private static Dictionary<object, List<AddressableObjectPool>> Pools { get; } = new Dictionary<object, List<AddressableObjectPool>>();

        private AssetReferenceGameObject Prefab { get; set; }
        private AsyncOperationHandle<GameObject> Handle { get; set; }
        private Stack<GameObject> Pool { get; } = new Stack<GameObject>();

        private void OnDestroy()
        {
            UnregisterPool();
            ReleaseHandle();
        }

        private void Initialize(AssetReferenceGameObject prefab)
        {
            Prefab = prefab;
            Handle = prefab.LoadAssetAsync();
            RegisterPool();
            DontDestroyOnLoad(gameObject);
        }

        public void Clear()
        {
            while (Pool.Count > 0)
            {
                Destroy(Pool.Pop());
            }
        }

        public void EnsureCapacity(int capacity)
        {
            if (Handle.IsDone)
            {
                var prefab = Handle.Result;

                while (Pool.Count < capacity)
                {
                    var obj = Instantiate(prefab, transform);
                    obj.SetActive(false);
                    Pool.Push(obj);
                }

                return;
            }

            Handle.Completed += handle =>
            {
                var prefab = handle.Result;

                while (Pool.Count < capacity)
                {
                    var obj = Instantiate(prefab, transform);
                    obj.SetActive(false);
                    Pool.Push(obj);
                }
            };
        }

        private void ReleaseHandle()
        {
            if (Handle.IsValid())
            {
                Addressables.Release(Handle);
                Handle = new AsyncOperationHandle<GameObject>();
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

        public GameObject GetObject(Transform parent = null)
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

        public T GetObject<T>(Transform parent = null)
        {
            return GetObject(parent).GetComponent<T>();
        }

        public void ReturnObject(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            Pool.Push(obj);
        }

        private static void AssertPrefabIsValid(AssetReferenceGameObject prefab)
        {
            if (!prefab.RuntimeKeyIsValid())
                throw new System.ArgumentException($"Prefab runtime key is not valid: {prefab}.");
        }

        public static AddressableObjectPoolHandle GetPoolHandle(AssetReferenceGameObject prefab)
        {
            AssertPrefabIsValid(prefab);
            var key = prefab.RuntimeKey;

            if (!Pools.TryGetValue(key, out var pools))
            {
                pools = new List<AddressableObjectPool>();
                Pools.Add(key, pools);
            }

            return new AddressableObjectPoolHandle(prefab, pools);
        }

        public static AddressableObjectPool Create(AssetReferenceGameObject prefab)
        {
            AssertPrefabIsValid(prefab);
            var obj = new GameObject("Addressable Object Pool");
            var pool = obj.AddComponent<AddressableObjectPool>();
            pool.Initialize(prefab);
            return pool;
        }
    }
}