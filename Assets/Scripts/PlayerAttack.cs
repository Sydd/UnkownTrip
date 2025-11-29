using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform attackPoint;

    private bool isAttacking = false;
    private bool isDashActive = false;
    private bool isDashOnCooldown = false;

    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && CanAttack() )
        {
            await AttackAsync();
        }
        if (Input.GetKeyDown(KeyCode.K) && CanDash() )
        {
            await DashAsync();
        }
    }

    private async UniTask DashAsync()
    {
        isDashActive = true;
        isDashOnCooldown = true;
        PlayerStatus.Instance.currentState = PlayerState.Dash;
        
        // Wait for dash duration
        await UniTask.Delay(System.TimeSpan.FromSeconds(dashDuration));
        
        isDashActive = false;
        PlayerStatus.Instance.currentState = PlayerState.Idle;
        
        // Wait for cooldown
        await UniTask.Delay(System.TimeSpan.FromSeconds(dashCooldown));
        
        isDashOnCooldown = false;
    }

    private bool CanDash()
    {
        return !isDashOnCooldown && (PlayerStatus.Instance.currentState == PlayerState.Idle || PlayerStatus.Instance.currentState == PlayerState.Moving);
    }
    async UniTask AttackAsync()
    {
        PlayerStatus.Instance.currentState = PlayerState.Attacking;

        // Perform attack
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            // Deal damage to enemy
           // enemy.GetComponent<Enemy>()?.TakeDamage(attackDamage);
        }

        // Wait for cooldown
        await UniTask.Delay(System.TimeSpan.FromSeconds(attackCooldown));

        PlayerStatus.Instance.currentState = PlayerState.Idle;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public bool CanAttack(){
       return  PlayerStatus.Instance.currentState == PlayerState.Idle || PlayerStatus.Instance.currentState == PlayerState.Moving;
    }
}