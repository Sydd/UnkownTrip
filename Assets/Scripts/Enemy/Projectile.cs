using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetimeSeconds = 3f;
    [SerializeField] private float checkIntervalSeconds = 0.2f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float hitRadius = 0.5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Sprite> currentAnimation;
    private void Start()
    {
        RunAnimation().Forget();
    }
    private ProjectilePool pool;
    private float spawnTime;
    private float nextCheckTime;
    private bool active;
    private Vector3 moveDir = Vector3.forward;
    private Camera mainCam;

    public void SetPool(ProjectilePool p)
    {
        pool = p;
    }

    public void OnSpawned()
    {
        spawnTime = Time.time;
        nextCheckTime = spawnTime;
        active = true;
        if (moveDir.sqrMagnitude <= 0f)
        {
            moveDir = transform.right; // sprite default aims +X
        }
        mainCam = Camera.main;
        ApplyBillboardFacing();
    }

    public void Initialize(Vector3 direction)
    {
        moveDir = direction.sqrMagnitude > 0f ? direction.normalized : Vector3.forward;
        transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
    }

    void OnDisable()
    {
        active = false;
    }
    private async UniTask RunAnimation(){
        int count = 0;
        while (true){
            if (this == null) return;
            spriteRenderer.sprite = currentAnimation[count];
            count = (count + 1) % currentAnimation.Count;
            await UniTask.Delay(100);
        }
    }
    void Update()
    {
        if (!active) return;

        // Move in assigned direction each frame
        transform.position += moveDir * speed * Time.deltaTime;

        // Billboard to camera but keep sprite's +X aiming along moveDir
        ApplyBillboardFacing();

        // Periodic player check
        if (Time.time >= nextCheckTime)
        {
            nextCheckTime += checkIntervalSeconds;
            CheckForPlayer();
        }

        // Auto-return after lifetime
        if (Time.time - spawnTime >= lifetimeSeconds)
        {
            ReturnToPool();
        }
    }

    private void ApplyBillboardFacing()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return;

        Vector3 up = Vector3.up;
        Vector3 toCam = (mainCam.transform.position - transform.position).normalized;
        if (toCam.sqrMagnitude < 1e-6f) return;

        // Base billboard: face camera, keep global up
        Quaternion billboard = Quaternion.LookRotation(toCam, up);

        // Desired right axis is moveDir projected on horizontal plane
        Vector3 desiredRight = Vector3.ProjectOnPlane(moveDir, up).normalized;
        if (desiredRight.sqrMagnitude < 1e-6f)
        {
            transform.rotation = billboard;
            return;
        }

        // Current right after billboard
        Vector3 currentRight = billboard * Vector3.right;
        // Rotate around forward (toCam) to align right with desiredRight
        float angle = Vector3.SignedAngle(currentRight, desiredRight, toCam);
        transform.rotation = billboard * Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void CheckForPlayer()
    {
        // Simple sphere check for player hit
        Collider[] hits = new Collider[1];
        Physics.OverlapSphereNonAlloc(transform.position, hitRadius, hits, playerLayer);
        if (hits[0] != null)
        {
            // If PlayerStatus exists, apply damage/hurt
            var playerStatus = PlayerStatus.Instance;
            if (playerStatus != null)
            {
                playerStatus.Hurt();
            }
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (pool != null)
        {
            pool.ReturnProjectile(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}
