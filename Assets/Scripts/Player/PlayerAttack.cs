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
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float attackOffset = 1f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private PlayerMovement playerMovement;  
    [SerializeField] private GameObject dashEffect;

    [SerializeField] private float hitstopDuration = 0.05f;
    [SerializeField] private bool freezeAll = true;   // freeze whole game or just player
    [SerializeField] private bool freezePlayerMovement = true;
    private bool isDashOnCooldown = false;
    public Action<Vector3> enemyHurt;
    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        AttackCooldown = attackCooldown;
    }

    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Jump") && CanAttack() )
        {
            await AttackAsync();
        }
        if (Input.GetKeyDown(KeyCode.K) || Input.GetButtonDown("Fire1") && CanDash() )
        {
            await DashAsync();
        }
    }

    private async UniTask DashAsync()
    {
        isDashOnCooldown = true;
        dashEffect.SetActive(true);
        Vector3 dashposition = new Vector3(!playerMovement.lookingRight ? 1f : -1f, dashEffect.transform.localPosition.y, dashEffect.transform.localPosition.z);
        dashEffect.transform.localPosition = dashposition;
        dashEffect.transform.localScale = new Vector3(!playerMovement.lookingRight ? -3f : 3f, dashEffect.transform.localScale.y, dashEffect.transform.localScale.z);
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
               IEnemy enemyScript = enemy.GetComponent<EnemyMelee>();
               if (enemyScript == null)
               {
                   enemyScript = enemy.GetComponent<EnemyRanger>();
               }
               if (enemyScript != null)
               {
                   enemyScript.TakeDamage(attackDamage, transform.position).Forget();
                   enemyHurt?.Invoke(enemy.transform.position);
                   AudioManager.Instance.PlaySFX(AudioManager.Instance.MonsterHit);
               }

                await HitStopAsync();
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

    private async UniTask HitStopAsync()
    {
        float originalTimeScale = Time.timeScale;

        // Freeze ALL gameplay
        if (freezeAll)
            Time.timeScale = 0f;

        // Freeze only player movement logic
        if (freezePlayerMovement && playerMovement != null)
            playerMovement.enabled = false;

        // Wait in REAL TIME (so hitstop works even at timeScale = 0)
        await UniTask.Delay(TimeSpan.FromSeconds(hitstopDuration), ignoreTimeScale: true);

        // Restore
        if (freezeAll)
            Time.timeScale = originalTimeScale;

        if (freezePlayerMovement && playerMovement != null)
            playerMovement.enabled = true;
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