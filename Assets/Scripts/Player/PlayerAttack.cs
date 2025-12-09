using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int attackDamage = 10;
    [SerializeField] public float attackCooldown = 0.4f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] public static float AttackCooldown = 0.4f;
    [SerializeField] private float dashDuration = .5f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float attackOffset = 1f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private PlayerMovement playerMovement;  
    private bool isDashOnCooldown = false;
    public Action<Vector3> enemyHurt;
    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        AttackCooldown = attackCooldown;
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
        AudioManager.Instance.PlaySFX(AudioManager.Instance.PlayerDash);
        // Wait for dash duration
        await UniTask.Delay(System.TimeSpan.FromSeconds(dashDuration));
        
        PlayerStatus.Instance.currentState = PlayerState.Moving;

        // Wait for cooldown
        await UniTask.Delay(System.TimeSpan.FromSeconds(dashCooldown));
        isDashOnCooldown = false;
    }

    private bool CanDash()
    {
        return !isDashOnCooldown && (PlayerStatus.Instance.currentState == PlayerState.Idle 
        || PlayerStatus.Instance.currentState == PlayerState.Moving || PlayerStatus.Instance.currentState == PlayerState.Attacking);
    }
    Collider[] colliders = new Collider[10];
    async UniTask AttackAsync()
    {
        PlayerStatus.Instance.currentState = PlayerState.Attacking;
        
        // Wait for half attack cooldown OR until dash is pressed
        var delayTask = UniTask.Delay(System.TimeSpan.FromSeconds(AttackCooldown / 2));
        await UniTask.WaitUntil(() => delayTask.Status == UniTaskStatus.Succeeded || PlayerStatus.Instance.currentState == PlayerState.Dash);
        if(PlayerStatus.Instance.currentState == PlayerState.Dash ) return;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.SwordSlash);
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
               if (enemyScript != null)
               {
                   enemyScript.TakeDamage(attackDamage, transform.position).Forget();
                   enemyHurt?.Invoke(enemy.transform.position);
                   AudioManager.Instance.PlaySFX(AudioManager.Instance.MonsterHit);
               }
           }
        }

        // Wait for remaining cooldown OR until dash is pressed
        var cooldownTask = UniTask.Delay(System.TimeSpan.FromSeconds(AttackCooldown / 2));
        await UniTask.WaitUntil(() => cooldownTask.Status == UniTaskStatus.Succeeded || PlayerStatus.Instance.currentState == PlayerState.Dash);        
        if (PlayerStatus.Instance.currentState != PlayerState.Dash)
        {
            PlayerStatus.Instance.currentState = PlayerState.Idle;
        }
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