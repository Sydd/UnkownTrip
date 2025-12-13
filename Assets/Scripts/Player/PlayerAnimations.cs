using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
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

    [Header("Invulnerability Blink")]
    [SerializeField] private float blinkDuration = 0.07f;
    [SerializeField] private int blinkCount = 8;
    [SerializeField] private float blinkAlpha = 0.3f;

    private Tween _blinkTween;
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
            AudioManager.Instance.PlaySFX(AudioManager.Instance.PlayerHit);
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
                await UniTask.WaitUntil(() => !runOnce);
                continue;
            }
            spriteRenderer.sprite = currentAnimation[count];
            count = (count + 1) % currentAnimation.Length;
            await UniTask.Delay(timeStep);
        }
    }



    public void PlayInvulnerabilityBlink()
    {
        StopInvulnerabilityBlink();

        _blinkTween = spriteRenderer
            .DOFade(blinkAlpha, blinkDuration)
            .SetLoops(blinkCount * 2, LoopType.Yoyo)
            .SetEase(Ease.Linear)
            .SetUpdate(true); // ignores Time.timeScale (important for hitstop)
    }

    public void StopInvulnerabilityBlink()
    {
        if (_blinkTween != null && _blinkTween.IsActive())
            _blinkTween.Kill();

        // Restore visibility
        Color color = spriteRenderer.color;
        color.a = 1f;
        spriteRenderer.color = color;
    }

    private void OnDisable()
    {
        StopInvulnerabilityBlink();
    }


    private PlayerState currentState;     
    private void UpdateAnimationState()
    {
        PlayerState newstate = PlayerStatus.Instance.currentState;
        if (currentState == newstate) return;
        currentState = newstate;
        count = 0;
        runOnce = false;
        timeStep = originalTimeStep;
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
               // timeStep = (int)(PlayerAttack.AttackCooldown * 1000 / attacking.Length);
                currentAnimation = attacking;
                break;
            case PlayerState.Dash:
                timeStep = originalTimeStep / 2;
                currentAnimation = dash;
                break;
            default:
                currentAnimation = idle;
                break;
        }
    }
}