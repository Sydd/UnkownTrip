using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] public static float attackCooldown = 0.4f;
    [SerializeField] private float dashDuration = .5f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float attackOffset = 1f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private PlayerMovement playerMovement;  
    private bool isDashOnCooldown = false;
    public Action<Vector3> enemyHurt;
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

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
        isDashOnCooldown = true;
        PlayerStatus.Instance.currentState = PlayerState.Dash;
        
        // Wait for dash duration
        await UniTask.Delay(System.TimeSpan.FromSeconds(dashDuration));
        
        PlayerStatus.Instance.currentState = PlayerState.Idle;
        
        // Wait for cooldown
        await UniTask.Delay(System.TimeSpan.FromSeconds(dashCooldown));
        AudioManager.Instance.PlaySFX(AudioManager.Instance.PlayerDash);
        isDashOnCooldown = false;
    }

    private bool CanDash()
    {
        return !isDashOnCooldown && (PlayerStatus.Instance.currentState == PlayerState.Idle || PlayerStatus.Instance.currentState == PlayerState.Moving);
    }
    Collider[] colliders = new Collider[10];
    async UniTask AttackAsync()
    {
        PlayerStatus.Instance.currentState = PlayerState.Attacking;
        await UniTask.Delay(System.TimeSpan.FromSeconds(attackCooldown) / 2);
        // Calculate attack position with offset based on facing direction
        Vector3 attackPosition = attackPoint.position;
        if (playerMovement != null)
        {
            float direction = playerMovement.lookingRight ? 1f : -1f;
            attackPosition += Vector3.right * attackOffset * direction;
        }

        Physics.OverlapSphereNonAlloc(attackPosition, attackRange, colliders, enemyLayer);

        foreach (Collider enemy in colliders)
        {
            // Deal damage to enemy
           if (enemy != null)
           {
               Enemy enemyScript = enemy.GetComponent<Enemy>();
               if (enemyScript != null && enemyScript.State != EnemyState.Hurt)
               {
                   enemyScript.TakeDamage(attackDamage).Forget();
                   enemyHurt?.Invoke(enemy.transform.position);
                   AudioManager.Instance.PlaySFX(AudioManager.Instance.PlayerHit);
               }
           }
        }

        // Wait for cooldown
        await UniTask.Delay(System.TimeSpan.FromSeconds(attackCooldown) / 2);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.SwordSlash);
        PlayerStatus.Instance.currentState = PlayerState.Idle;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        // Calculate attack position with offset for gizmo
        Vector3 attackPosition = attackPoint.position;
        if (playerMovement != null)
        {
            float direction = playerMovement.lookingRight ? 1f : -1f;
            attackPosition += Vector3.right * attackOffset * direction;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition, attackRange);
    }

    public bool CanAttack(){
       return  PlayerStatus.Instance.currentState == PlayerState.Idle || 
       PlayerStatus.Instance.currentState == PlayerState.Moving ;
    }
}