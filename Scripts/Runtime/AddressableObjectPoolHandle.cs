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

        public GameObject GetObject(Transform parent)
        {
            if (Pools.Count > 0)
                return Pools[0].GetObject(parent);

            return AddressableObjectPool.Create(Prefab).GetObject(parent);
        }

        public T GetObject<T>(Transform parent)
        {
            if (Pools.Count > 0)
                return Pools[0].GetObject<T>(parent);

            return AddressableObjectPool.Create(Prefab).GetObject<T>(parent);
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
    }
}