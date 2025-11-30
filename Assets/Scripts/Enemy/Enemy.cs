using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stepDistance = 1f;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private float viewDistance = 10f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int life = 50;
    [SerializeField] private EnemyAnimation enemyAnimations;
    public EnemyState State = EnemyState.Idle;

    public Action OnDie;

    private PlayerStatus playerStatus;
    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        playerStatus = PlayerStatus.Instance;
        player = playerStatus.transform;
        enemyAnimations.SetIdle();
    }

    // Update is called once per frame
    async void Update()
    {
        if (State == EnemyState.Idle && player != null)
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
        enemyAnimations.SetWalk();
        State = EnemyState.Moving;
        
        Vector3 startPosition = transform.position;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 targetPosition = startPosition + directionToPlayer * stepDistance;
        
        // Flip to face player
        bool shouldFaceRight = directionToPlayer.x > 0;
        enemyAnimations.Flip(shouldFaceRight);
        
        // Check for walls before moving
        if (Physics.Raycast(startPosition, directionToPlayer, viewDistance, wallLayer))
        {
            State = EnemyState.Idle;
            enemyAnimations.SetIdle();
            return;
        }
        
        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = distance / moveSpeed;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            if (State == EnemyState.Hurt) break;
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            if (this == null) break;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            await UniTask.Yield();
        }
        if (this == null) return;
        transform.position = targetPosition;
        State = EnemyState.Idle;
    }

    private async UniTask AttackPlayerAsync()
    {   
        float attackDistance = stopDistance * 1.2f;
        Vector3 directionToPlayer = (player.position - transform.position).normalized * attackDistance;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + directionToPlayer;
        if (Physics.Raycast(startPosition, directionToPlayer, viewDistance, wallLayer))
        {
            State = EnemyState.Idle;
            enemyAnimations.SetIdle();
            return;
        }
        State = EnemyState.Attacking;
        enemyAnimations.SetAttack();
        
        // Flip to face player
        bool shouldFaceRight = directionToPlayer.x > 0;
        enemyAnimations.Flip(shouldFaceRight);
        
        bool animating = true;
        bool damaged = false;
        Collider[] hitEnemies = new Collider[1];

        Vector3 originalScale = transform.localScale;        
        LeanTween.scale(gameObject, Vector3.one * 1.5f, 0.5f).setEasePunch().setOnComplete(() => animating = false);
        await UniTask.WaitUntil(() => !animating);
        float elapsed = 0f;
        float duration = .3f;
        while (elapsed < duration)
        {
            if (this == null) break;
            if (State == EnemyState.Hurt){
                transform.localScale = originalScale;
                return;
            }
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
        State = EnemyState.Idle;
        enemyAnimations.SetIdle();
    }

    void OnDrawGizmos()
    {
        // Draw ray in red if blocked, green if clear
        // Draw attack range sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    internal async UniTask TakeDamage(int attackDamage)
    {
        State = EnemyState.Hurt;
        enemyAnimations.SetIdle();
        life -= attackDamage;
        if (life <= 0)
        {
            OnDie?.Invoke();
        }
        else
        {
            PlayHurtAnimation();
        }
        await UniTask.WaitForSeconds(PlayerAttack.attackCooldown);
        State = EnemyState.Idle;
        enemyAnimations.SetIdle();
    }

    private void PlayHurtAnimation()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Vector3 originalScale = transform.localScale;
        Color originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        
        LeanTween.cancel(gameObject);
        
        // Scale down slightly
        LeanTween.scale(gameObject, originalScale * 0.9f, 0.15f).setOnComplete(() =>
        {
            // Scale back to original
            LeanTween.scale(gameObject, originalScale, 0.15f);
        });
        
        // Turn red then back to original color
        if (spriteRenderer != null)
        {
            LeanTween.value(gameObject, originalColor, Color.red, 0.15f)
                .setOnUpdate((Color color) =>
                {
                    if (spriteRenderer != null)
                        spriteRenderer.color = color;
                })
                .setOnComplete(() =>
                {
                    LeanTween.value(gameObject, Color.red, originalColor, 0.15f)
                        .setOnUpdate((Color color) =>
                        {
                            if (spriteRenderer != null)
                                spriteRenderer.color = color;
                        });
                });
        }
    }
}
public enum EnemyState
{
    Idle,
    Moving,
    Attacking,
    Hurt
}