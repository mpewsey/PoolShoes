# Object Pool

An object pool for addressable objects in Unity.

## Installation

To add the package to a project, in Unity, select `Window > Package Manager`.

![Unity Package Manager](https://user-images.githubusercontent.com/23442063/163601100-191d8699-f4fd-42cc-96d4-f6aa5a8ae29b.png)

Select `Add package from git URL...` and paste the following URL:

```
https://github.com/mpewsey/ObjectPool.git
```

RECOMMENDED: To lock into a specific version, append `#{VERSION_TAG}` to the end of the URL. For example:

```
https://github.com/mpewsey/ObjectPool.git#v1.0.0
```

## Example Usage

```ObjectPoolExample.cs
public class ObjectPoolExample : MonoBehaviour
{
    public AssetReferenceGameObject prefab;
    private AddressableObjectPoolHandle PoolHandle { get; set; }
    private List<GameObject> GameObjects { get; } = new List<GameObject>();

    private void Awake()
    {
        // Cache a pool handle. This handle can perform checkout and return operations
        // on any existing pools for the specified prefab and will create a pool for
        // the prefab if one doesn't already exist.
        PoolHandle = AddressableObjectPool.GetPoolHandle(prefab);
    }

    private void CheckoutObjects()
    {
        // Check out some objects from the object pool.
        for (int i = 0; i < 10; i++)
        {
            var obj = PoolHandle.GetObject(transform);
            GameObjects.Add(obj);
        }
    }

    private void ReturnObjects()
    {
        // Return any objects stored in the GameObjects list.
        foreach (var obj in GameObjects)
        {
            PoolHandle.ReturnObject(obj);
        }

        GameObjects.Clear();
    }
}
```
