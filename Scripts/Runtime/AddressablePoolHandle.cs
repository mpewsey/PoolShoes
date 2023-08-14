using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MPewsey.PoolShoes
{
    public struct AddressablePoolHandle
    {
        private AssetReferenceGameObject Prefab { get; }
        private List<AddressablePool> Pools { get; }

        public AddressablePoolHandle(AssetReferenceGameObject prefab, List<AddressablePool> pools)
        {
            Prefab = prefab;
            Pools = pools;
        }

        public AddressablePool GetPool()
        {
            if (Pools.Count > 0)
                return Pools[0];

            return AddressablePool.Create(Prefab);
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