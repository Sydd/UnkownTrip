using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class EnemyMelee : MonoBehaviour, IEnemy
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stepDistance = 1f;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private float viewDistance = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int life = 50;
    [SerializeField] private EnemyAnimation enemyAnimations;
    private bool isAttackOnCoolDown = false;
    public EnemyState State { get; set; } = EnemyState.Idle;

    public Action OnDie { get; set; }
    Vector3 originalScale;
    private bool isAttacking = false;
    private bool isMoving = false;

    private PlayerStatus playerStatus;
    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        playerStatus = PlayerStatus.Instance;
        player = playerStatus.transform;
        enemyAnimations.SetIdle();
        originalScale = transform.localScale;        
    }

    async void Update()
    {
        if (State == EnemyState.Dying ) return;
        if (State == EnemyState.Idle && !isAttacking && !isMoving && player != null)
        {   
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > stopDistance)
            {
                await MoveTowardPlayerAsync(); 
            }
            else if (distanceToPlayer <= stopDistance && !isAttackOnCoolDown )
            {
                await AttackPlayerAsync();
            } 
            else {
                enemyAnimations.SetIdle(); 
            }
        }
    }
    private async UniTask MoveTowardPlayerAsync()
    {
        if (isMoving) return;
        isMoving = true;
        
        Vector3 startPosition = transform.position;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 targetPosition = startPosition + directionToPlayer * stepDistance;
        
        // Check for walls before moving
        if (Physics.Raycast(startPosition, directionToPlayer, viewDistance, wallLayer))
        {
            if (ShouldKeepRoutine()) State = EnemyState.Idle;
            enemyAnimations.SetIdle();
            isMoving = false;
            return;
        }
       // Flip to face player
        bool shouldFaceRight = directionToPlayer.x > 0;
        enemyAnimations.Flip(shouldFaceRight);
        enemyAnimations.SetWalk();
        State = EnemyState.Moving;
        
        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = distance / moveSpeed;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            if (!ShouldKeepRoutine())
            {
                isMoving = false;
                return;
            }
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            if (this == null) break;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            await UniTask.Yield();
        }
        if (this == null)
        {
            isMoving = false;
            return;
        }
        transform.position = targetPosition;
        if (ShouldKeepRoutine()) State = EnemyState.Idle;
        isMoving = false;
    }

    private async UniTask AttackPlayerAsync()
    {   
        if (isAttacking) return;
        isAttacking = true;
        float attackDistance = stopDistance * 1.2f;
        Vector3 directionToPlayer = (player.position - transform.position).normalized * attackDistance;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + directionToPlayer;
        if (Physics.Raycast(startPosition, directionToPlayer, viewDistance, wallLayer))
        {
            if (ShouldKeepRoutine()) State = EnemyState.Idle;
            enemyAnimations.SetIdle();
            isAttacking = false;
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
        LeanTween.scale(gameObject, Vector3.one * 1.5f, 0.5f).setEasePunch().setOnComplete(() => animating = false);
        await UniTask.WaitUntil(() => !animating || State == EnemyState.Hurt || State == EnemyState.Dying);
        if (this == null) return;
        transform.localScale = originalScale;
        if (!ShouldKeepRoutine()){
            isAttacking = false;
            return;
        }
        float elapsed = 0f;
        float duration = .3f;

        while (elapsed < duration)
        {
            if (this == null)
            {
                isAttacking = false;
                break;
            }
            if (!ShouldKeepRoutine()){
                transform.localScale = originalScale;
                isAttacking = false;
                return;
            }
            if (State == EnemyState.Hurt){
                transform.localScale = originalScale;
                isAttacking = false;
                return;
            }
            enemyAnimations.SetAttackDash();
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
        if (this == null)
        {
            isAttacking = false;
            return;
        }
        enemyAnimations.SetIdle();
        await UniTask.WaitForSeconds(0.5f);
        if (ShouldKeepRoutine()) State = EnemyState.Idle;
        isAttacking = false;
        isAttackOnCoolDown = true;
        await UniTask.Delay((int)(attackCooldown * 1000));
        isAttackOnCoolDown = false;
    }

    void OnDrawGizmos()
    {
        // Draw ray in red if blocked, green if clear
        // Draw attack range sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public async UniTask TakeDamage(int attackDamage, Vector3 position)
    {
        if (State == EnemyState.Dying) return;
        State = EnemyState.Hurt;
        enemyAnimations.SetIdle();
        life -= attackDamage;
        if (life <= 0)
        {
            State = EnemyState.Dying;
            // Stop any ongoing tweens and actions
            isAttacking = false;
            isMoving = false;
            if (gameObject != null)
            {
                LeanTween.cancel(gameObject);
            }
            OnDie?.Invoke();
            await enemyAnimations.SetDie(position);
            await UniTask.Delay(1000);
            Destroy(gameObject);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.MonsterDeath);
            return;
        }
        else
        {
            await enemyAnimations.PlayHurtAnimation(position);
        }
        await UniTask.WaitForSeconds(2);
        if (State == EnemyState.Dying) return;
        State = EnemyState.Idle;
        enemyAnimations.SetIdle();
    }

    private bool ShouldKeepRoutine(){
        return State != EnemyState.Hurt && State != EnemyState.Dying;
    }
}

public interface IEnemy
{
     EnemyState State { get; set; }
     UniTask TakeDamage(int attackDamage, Vector3 position);
     Action OnDie { get; set; }
}

public enum EnemyState
{
    Idle,
    Moving,
    Attacking,
    Hurt,
    Dying
}