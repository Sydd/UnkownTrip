using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public Action OnDamaged;
    public Action OnDeath;

    [Header("References")]
    [SerializeField] private PlayerAnimations playerAnimations;

    [Header("Stats")]
    [SerializeField] private int life = 3;

    [Header("Invulnerability")]
    [SerializeField] private float invulnerabilityTime = 1f;

    public static PlayerStatus Instance { get; private set; }

    public PlayerState currentState;
    public int CurrentHealth => life;

    private bool _isInvulnerable;
    private CancellationTokenSource _invulnerabilityCTS;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Hurt()
    {
        if (currentState == PlayerState.Dead) return;
        if (_isInvulnerable) return;

        life--;
        OnDamaged?.Invoke();

        if (life <= 0)
        {
            currentState = PlayerState.Dead;
            OnDeath?.Invoke();
            CancelInvulnerability();
        }
        else
        {
            playerAnimations.PlayHurtAnimation();
            StartInvulnerabilityAsync().Forget();
        }
    }

    private async UniTaskVoid StartInvulnerabilityAsync()
    {
        CancelInvulnerability();

        _invulnerabilityCTS = new CancellationTokenSource();
        _isInvulnerable = true;

        // Optional feedback
        playerAnimations?.PlayInvulnerabilityBlink();

        try
        {
            await UniTask.Delay(
                TimeSpan.FromSeconds(invulnerabilityTime),
                cancellationToken: _invulnerabilityCTS.Token
            );
        }
        catch (OperationCanceledException)
        {
            // ignored
        }

        _isInvulnerable = false;
    }

    private void CancelInvulnerability()
    {
        if (_invulnerabilityCTS == null) return;

        _invulnerabilityCTS.Cancel();
        _invulnerabilityCTS.Dispose();
        _invulnerabilityCTS = null;
        _isInvulnerable = false;
    }

    private void OnDestroy()
    {
        CancelInvulnerability();
    }
}

public enum PlayerState
{
    Idle,
    Moving,
    Attacking,
    Dead,
    Dash,
    AttackingCombo
}
