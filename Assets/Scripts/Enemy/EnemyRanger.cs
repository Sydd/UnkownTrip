using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRanger : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private Transform player;
    [SerializeField] private EnemyAnimation enemyAnimations;
    Vector3 originalScale;
    public EnemyState State = EnemyState.Idle;
    private bool isAttacking = false;
    private bool isAttackOnCoolDown = false;
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
        // Face the player
        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        // Simulate attack duration
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
}
