using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stepDistance = 1f;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private float viewDistance = 10f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask playerLayer;
    private PlayerStatus playerStatus;
    private Transform player;
    private bool isMoving = false;
    bool attacking = false;
    // Start is called before the first frame update
    void Start()
    {
        playerStatus = PlayerStatus.Instance;
        player = playerStatus.transform;
    }

    // Update is called once per frame
    async void Update()
    {
        if (!isMoving && !attacking && player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > stopDistance)
            {
                await MoveTowardPlayerAsync(); 
            }
            if (distanceToPlayer <= stopDistance )
            {
                await AttackPlayerAsync();
            }
        }
    }
    private async UniTask MoveTowardPlayerAsync()
    {
        isMoving = true;
        
        Vector3 startPosition = transform.position;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 targetPosition = startPosition + directionToPlayer * stepDistance;
        
        // Check for walls before moving
        if (Physics.Raycast(startPosition, directionToPlayer, viewDistance, wallLayer))
        {
            isMoving = false;
            return;
        }
        
        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = distance / moveSpeed;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            if (this == null) break;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            await UniTask.Yield();
        }
        if (transform != null)
        transform.position = targetPosition;
        isMoving = false;
    }

    private async UniTask AttackPlayerAsync()
    {
        bool animating = true;
        float attackDistance = stopDistance * 1.2f;
        bool damaged = false;
        attacking = true;
        Collider[] hitEnemies = new Collider[1];
        Vector3 directionToPlayer = (player.position - transform.position).normalized * attackDistance;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + directionToPlayer;
        Vector3 originalScale = transform.localScale;        
        LeanTween.scale(gameObject, Vector3.one * 1.5f, 0.5f).setEasePunch().setOnComplete(() => animating = false);
        await UniTask.WaitUntil(() => !animating);
        float elapsed = 0f;
        float duration = .3f;
        while (elapsed < duration)
        {
            if (this == null) break;
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            if (!damaged)
            {
                Physics.OverlapSphereNonAlloc(transform.position, attackRange, hitEnemies, playerLayer);
                if (hitEnemies[0] != null && playerStatus.currentState!= PlayerState.Dash )
                {
                    playerStatus.Hurt();
                    damaged = true;
                }
            }
            await UniTask.Yield();
        }
        if (this == null) return;
        transform.localScale = originalScale;
        await UniTask.WaitForSeconds(0.5f);
        attacking = false;
    }

    void OnDrawGizmos()
    {
        // Draw ray in red if blocked, green if clear
        // Draw attack range sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
public enum EnemyState
{
    Idle,
    Moving,
    Attacking

}