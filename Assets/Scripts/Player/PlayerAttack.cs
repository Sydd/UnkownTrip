using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int attackDamage = 10;
    [SerializeField] public float attackDuration = 0.4f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] public static float AtttackDuration = 0.4f;
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
        AtttackDuration = attackDuration;
    }

    async void Update()
    {
        if  (AttackInputDetected() && CanAttack() )
        {
            await AttackAsync();
        }
        if (DashInputDetected() && CanDash() )
        {
            await DashAsync();
        }
    }

    private bool DashInputDetected()
    {
        if (Input.GetKeyDown(KeyCode.K) || Input.GetButtonDown("Fire1")) return true;
        return false;
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
        string preResult = await ResultAsync(AtttackDuration / 2, ignoreAttackInput: true);
        if(preResult == "Dash") return;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.SwordSlash);
        await DoDamage();

        string result = await ResultAsync(AtttackDuration / 2);
        if (result != "Attack") {
            PlayerStatus.Instance.currentState = PlayerState.Idle;
            return ;
        }
        preResult = await ResultAsync(AtttackDuration / 2, ignoreAttackInput: true);
        if(preResult == "Dash") return;
        PlayerStatus.Instance.currentState = PlayerState.AttackingCombo;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.SwordSlash);
        await DoDamage();
        preResult = await ResultAsync(AtttackDuration / 2, ignoreAttackInput: true);
        if(preResult == "Dash") return;
        PlayerStatus.Instance.currentState = PlayerState.Idle;
    }

    private async UniTask DoDamage(){
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
    }
    private async UniTask<string> ResultAsync(float secondsToAwait, bool ignoreAttackInput = false)
    {
        float elapsedTime = 0f;
        while (elapsedTime < secondsToAwait)
        {
            if (DashInputDetected())
            {
                return "Dash";
            }
            if (AttackInputDetected() && !ignoreAttackInput)
            {
                return "Attack";
            }

            elapsedTime +=Time.deltaTime;
            await UniTask.Yield();
        }
        return "Timeout";
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

    private bool CanAttack(){
       return  PlayerStatus.Instance.currentState == PlayerState.Idle || 
       PlayerStatus.Instance.currentState == PlayerState.Moving ;
    }
    private bool AttackInputDetected()
    {
        if (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Jump")) return true;
        return false;
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

}