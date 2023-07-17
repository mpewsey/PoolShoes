using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MPewsey.ObjectPool
{
    public class AddressablePoolHandler : MonoBehaviour
    {
        [SerializeField] private AssetReferenceGameObject _prefab;

        public AddressablePoolHandle Handle { get; private set; }
        public AssetReferenceGameObject Prefab { get => _prefab; private set => _prefab = value; }

        private void Awake()
        {
            Handle = AddressablePool.GetPoolHandle(Prefab);
        }

        public void ReturnObject()
        {
            Handle.ReturnObject(gameObject);
        }
    }
}