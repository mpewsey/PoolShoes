using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MPewsey.ObjectPool
{
    public struct AddressableObjectPoolHandle
    {
        private AssetReferenceGameObject Prefab { get; }
        private List<AddressableObjectPool> Pools { get; }

        public AddressableObjectPoolHandle(AssetReferenceGameObject prefab, List<AddressableObjectPool> pools)
        {
            Prefab = prefab;
            Pools = pools;
        }

        public AddressableObjectPool GetPool()
        {
            if (Pools.Count > 0)
                return Pools[0];

            return AddressableObjectPool.Create(Prefab);
        }

        public void EnsureCapacity(int capacity)
        {
            GetPool().EnsureCapacity(capacity);
        }

        public GameObject GetObject(Transform parent = null)
        {
            return GetPool().GetObject(parent);
        }

        public T GetObject<T>(Transform parent = null)
        {
            return GetPool().GetObject<T>(parent);
        }

        public void ReturnObject(GameObject obj)
        {
            if (Pools.Count > 0)
            {
                Pools[0].ReturnObject(obj);
                return;
            }

            Object.Destroy(obj);
        }

        public bool IsValid()
        {
            return Pools != null && Prefab != null && Prefab.RuntimeKeyIsValid();
        }
    }
}