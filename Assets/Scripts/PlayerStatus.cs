using System;
using UnityEngine;

public class PlayerStatus: MonoBehaviour

{
    public Action OnDamaged;

    [SerializeField] private PlayerAnimations playerAnimations;
    public int life = 100;
    public static PlayerStatus Instance { get; private set; }
    public PlayerState currentState;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Hurt()
    {
        OnDamaged?.Invoke();
        life -= 10;
        if (life <= 0)
        {
            currentState = PlayerState.Dead;
        }
        else
        {
            playerAnimations.PlayHurtAnimation();
        }
    }
}
public   enum PlayerState
    {
        Idle,
        Moving,
        Attacking,
        Dead,
        Dash
    }
