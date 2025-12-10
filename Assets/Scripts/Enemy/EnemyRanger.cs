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
    [SerializeField] private int life = 30;
    [Header("Floating Effect")]
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatPeriod = 1.5f;
    [SerializeField] private float floatSwayAmplitude = 0.15f;
    [SerializeField] private float floatSwayPeriod = 2.0f;
    [SerializeField] private Transform floatTarget; // optional: assign visual child
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
        transform.position = new Vector3(transform.position.x, playerStatus.transform.position.y, transform.position.z);
        // Start floating effect
        var target = floatTarget == null ? transform : floatTarget;
        float originalY = target.localPosition.y;
        float originalX = target.localPosition.x;
        LeanTween.moveLocalY(target.gameObject, originalY + floatAmplitude, floatPeriod)
             .setEaseInOutSine()
             .setLoopPingPong();
        LeanTween.moveLocalX(target.gameObject, originalX + floatSwayAmplitude, floatSwayPeriod)
             .setEaseInOutSine()
             .setLoopPingPong();
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
       life -= attackDamage;
        if (life <= 0)
        {
            State = EnemyState.Dying;
            // Stop any ongoing tweens and actions
            isAttacking = false;
            if (gameObject != null)
            {
                LeanTween.cancel(gameObject);
            }
            if (floatTarget != null)
            {
                LeanTween.cancel(floatTarget.gameObject);
            }
            OnDie?.Invoke();
            await enemyAnimations.SetDie(position);
            await UniTask.Delay(1000);
            Destroy(gameObject);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.MonsterDeath);
            return;
        }        
        else{
            await enemyAnimations.PlayHurtAnimation(position);
        }
        await UniTask.WaitForSeconds(2);
        if (State == EnemyState.Dying) return;
        State = EnemyState.Idle;
        enemyAnimations.SetIdle();
    }
}
