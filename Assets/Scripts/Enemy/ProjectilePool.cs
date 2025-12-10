using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int initialSize = 16;
    [SerializeField] private bool allowExpand = true;

    private readonly Queue<Projectile> pool = new Queue<Projectile>();
    private Transform poolParent;
    public static ProjectilePool Instance;

    void Awake()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("ProjectilePool: projectilePrefab is not set.");
            enabled = false;
            return;
        }
        Instance = this;

        poolParent = new GameObject("ProjectilePool_Container").transform;
        poolParent.SetParent(transform);
        InitializePool(initialSize);
    }

    private void InitializePool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            var p = CreateProjectileInstance();
            pool.Enqueue(p);
        }
    }

    private Projectile CreateProjectileInstance()
    {
        var p = Instantiate(projectilePrefab, poolParent);
        p.gameObject.SetActive(false);
        p.SetPool(this);
        return p;
    }

    public Projectile GetProjectile(Vector3 position, Quaternion rotation)
    {
        Projectile proj = null;

        // Reuse available instance or expand
        while (pool.Count > 0 && (proj == null || proj.gameObject.activeSelf))
        {
            proj = pool.Dequeue();
        }

        if (proj == null)
        {
            if (!allowExpand && pool.Count == 0)
            {
                Debug.LogWarning("ProjectilePool: Pool exhausted and expansion disabled.");
                return null;
            }
            proj = CreateProjectileInstance();
        }

        proj.transform.SetPositionAndRotation(position, rotation);
        proj.gameObject.SetActive(true);
        proj.OnSpawned();
        return proj;
    }

    public void ReturnProjectile(Projectile proj)
    {
        if (proj == null) return;
        proj.gameObject.SetActive(false);
        proj.transform.SetParent(poolParent);
        pool.Enqueue(proj);
    }
}
