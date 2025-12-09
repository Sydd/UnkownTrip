using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int initialSize = 16;
    [SerializeField] private bool allowExpand = true;

    private readonly Queue<GameObject> pool = new Queue<GameObject>();
    private Transform poolParent;

    void Awake()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("ProjectilePool: projectilePrefab is not set.");
            enabled = false;
            return;
        }

        poolParent = new GameObject("ProjectilePool_Container").transform;
        poolParent.SetParent(transform);
        InitializePool(initialSize);
    }

    private void InitializePool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            var go = CreateProjectileInstance();
            pool.Enqueue(go);
        }
    }

    private GameObject CreateProjectileInstance()
    {
        var go = Instantiate(projectilePrefab, poolParent);
        go.SetActive(false);
        return go;
    }

    public GameObject GetProjectile(Vector3 position, Quaternion rotation)
    {
        GameObject go = null;

        // Reuse available instance or expand
        while (pool.Count > 0 && (go == null || go.activeSelf))
        {
            go = pool.Dequeue();
        }

        if (go == null)
        {
            if (!allowExpand && pool.Count == 0)
            {
                Debug.LogWarning("ProjectilePool: Pool exhausted and expansion disabled.");
                return null;
            }
            go = CreateProjectileInstance();
        }

        go.transform.SetPositionAndRotation(position, rotation);
        go.SetActive(true);
        return go;
    }

    public void ReturnProjectile(GameObject go)
    {
        if (go == null) return;
        go.SetActive(false);
        go.transform.SetParent(poolParent);
        pool.Enqueue(go);
    }
}
