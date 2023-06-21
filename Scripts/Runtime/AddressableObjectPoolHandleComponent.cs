using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MPewsey.ObjectPool
{
    public class AddressableObjectPoolHandleComponent : MonoBehaviour
    {
        [SerializeField] private AssetReferenceGameObject _prefab;

        public AddressableObjectPoolHandle Handle { get; private set; }
        public AssetReferenceGameObject Prefab { get => _prefab; private set => _prefab = value; }

        private void Awake()
        {
            Handle = AddressableObjectPool.GetPoolHandle(Prefab);
        }

        public void ReturnObject()
        {
            Handle.ReturnObject(gameObject);
        }
    }
}