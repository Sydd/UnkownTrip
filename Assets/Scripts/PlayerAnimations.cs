using System;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private float hurtFadeDuration = 0.3f;
    private Transform player;
    private Vector3 orignalScale;
    private SpriteRenderer spriteRenderer;

    internal void PlayHurtAnimation()
    {
        if (spriteRenderer != null)
        {
            LeanTween.cancel(gameObject);
            
            // Fade to red
            LeanTween.value(gameObject, UpdateColor, spriteRenderer.color, Color.red, hurtFadeDuration)
                .setOnComplete(() =>
                {
                    // Fade back to white
                    LeanTween.value(gameObject, UpdateColor, spriteRenderer.color, Color.white, hurtFadeDuration);
                });
        }
    }
    
    private void UpdateColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }

    void Awake()
    {
        player = this.transform;
        orignalScale = player.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
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