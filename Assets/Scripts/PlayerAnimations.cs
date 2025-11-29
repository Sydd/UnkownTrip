using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Transform player;
    private Vector3 orignalScale;

    void Awake()
    {
        player = this.transform;
        orignalScale = player.localScale;
    }

    void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        PlayerState currentState = PlayerStatus.Instance.currentState;

        switch (currentState)
        {
            case PlayerState.Idle:
                player.localScale = orignalScale;
                break;
            case PlayerState.Moving:
                player.localScale = orignalScale * 1.1f;
                break;
            case PlayerState.Attacking:
                player.localScale = orignalScale * 1.5f;
                break;
            case PlayerState.Dash:
                player.localScale = orignalScale * 0.7f;
                break;
            default:
                player.localScale = orignalScale;
                break;
        }
    }
}