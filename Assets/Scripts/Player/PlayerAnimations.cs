using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private float hurtFadeDuration = 0.3f;
    private Transform player;
    private Vector3 orignalScale;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] idle;
    [SerializeField] private Sprite[] moving;
    [SerializeField] private Sprite[] attacking;
    [SerializeField] private Sprite[] dash;
    [SerializeField] private int timeStep = 100;
    int originalTimeStep;
    private Sprite[] currentAnimation;        
    int count = 0;
    bool runOnce;
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
    private void Start()
    {
        currentAnimation=idle;
        originalTimeStep = timeStep;
        RunAnimations().Forget();
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
    }

    void Update()
    {
        UpdateAnimationState();
    }

    async UniTask RunAnimations(){
        while (PlayerStatus.Instance.currentState != PlayerState.Dead){
            if (runOnce && count == currentAnimation.Length)
            {
                timeStep = originalTimeStep;
                await UniTask.Delay(timeStep);
                continue;
            }
            spriteRenderer.sprite = currentAnimation[count];
            count = (count + 1) % currentAnimation.Length;
            await UniTask.Delay(timeStep);
        }
    }

    private PlayerState currentState;     
    private void UpdateAnimationState()
    {
        PlayerState newstate = PlayerStatus.Instance.currentState;
        if (currentState == newstate) return;
        currentState = newstate;
        count = 0;
        runOnce = false;
        switch (currentState)
        {
            case PlayerState.Idle:
                currentAnimation = idle;
                // player.localScale = orignalScale;
                break;
            case PlayerState.Moving:
                currentAnimation = moving;
                break;
            case PlayerState.Attacking:
                runOnce = true;
                timeStep = (int)(PlayerAttack.attackCooldown * 1000 / attacking.Length);
                currentAnimation = attacking;
                break;
            case PlayerState.Dash:
                currentAnimation = dash;
                break;
            default:
                currentAnimation = idle;
                break;
        }
    }
}