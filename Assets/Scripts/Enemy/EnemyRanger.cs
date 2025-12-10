using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRanger : MonoBehaviour, IEnemy
{
    private PlayerStatus playerStatus;
    private Transform player;
    [SerializeField] private EnemyAnimation enemyAnimations;
    [SerializeField] private float projectileSpawnOffset = 0.5f;
    Vector3 originalScale;
    public EnemyState State { get; set; } = EnemyState.Idle;
    private bool isAttacking = false;
    private bool isAttackOnCoolDown = false;
    public Action OnDie { get; set; }
    void Start()
    {
        playerStatus = PlayerStatus.Instance;
        player = playerStatus.transform;
        enemyAnimations.SetIdle();
        originalScale = transform.localScale;        
    }
    // Update is called once per frame
async void Update()
    {
        if (State == EnemyState.Dying) return;
        if (State == EnemyState.Idle && !isAttacking  && player != null)
        {
            if (!isAttackOnCoolDown )
            {
                await AttackPlayerAsync();
            } 
            else {
                enemyAnimations.SetIdle(); 
            }
        }
    }

    private async UniTask AttackPlayerAsync()
    {
        if (isAttacking) return;
        isAttacking = true;
        State = EnemyState.Attacking;
        enemyAnimations.SetAttack();
        // Fire projectiles in 4 directions (X ±, Z ±)

        Vector3 origin = transform.position + Vector3.up * projectileSpawnOffset;
        Vector3[] dirs = new Vector3[] { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
        await UniTask.Delay(500); // brief wind-up
        for (int i = 0; i < dirs.Length; i++)
        {
            if (State == EnemyState.Dying) break;
            Quaternion rot = Quaternion.LookRotation(dirs[i], Vector3.up);
            var proj = ProjectilePool.Instance.GetProjectile(origin, rot);
            if (proj != null)
            {
                proj.Initialize(dirs[i]);
            }
        }

        // Brief wind-down
        await UniTask.Delay(500);
        if (State == EnemyState.Dying)
        {
            isAttacking = false;
            return;
        }
        isAttackOnCoolDown = true;
        if (State != EnemyState.Dying) State = EnemyState.Idle;
        isAttacking = false;
        // Start cooldown
        await UniTask.Delay(2000);
        isAttackOnCoolDown = false;
    }
    public async UniTask TakeDamage(int attackDamage, Vector3 position)
    {
        if (State == EnemyState.Dying) return;
        State = EnemyState.Hurt;
        enemyAnimations.SetIdle();
        // Play hurt animation
        if (gameObject != null)
        {
            await enemyAnimations.PlayHurtAnimation(position);
        }
        await UniTask.WaitForSeconds(2);
        if (State == EnemyState.Dying) return;
        State = EnemyState.Idle;
        enemyAnimations.SetIdle();
    }
}
