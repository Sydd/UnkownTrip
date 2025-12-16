using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
 [SerializeField] private SpriteRenderer spriteRenderer;
 [SerializeField] private List<Sprite> walkSprites;
 [SerializeField] private List<Sprite> attackSprites;
 [SerializeField] private List<Sprite> idleSprites;
 [SerializeField] private List<Sprite> currentAnimation;
 [SerializeField] private List<Sprite> attackDashAnimation;
 [SerializeField] private List<Sprite> hurtAnimation;
 [SerializeField] private List<Sprite> deadAnimation;
 [SerializeField] private Sprite dieSprite;

 [SerializeField] private float knockUpDistance = 1.0f;
 [SerializeField] private Transform myself;
     public int sortingOffset = 0;   // use this if you want manual fine tuning
    public float precision = 100f;  // how accurate the sorting is

    int count = 0;
    private bool facingRight = true;
    
    // Start is called before the first frame update
    void Start()
    {
        RunAnimation().Forget();
    }
    private async UniTask RunAnimation(){
        while (true){
            if (this == null) return;
            if (count >= currentAnimation.Count) 
            {
                await UniTask.Delay(100);
                continue;
            }
            spriteRenderer.sprite = currentAnimation[count];
            count = (count + 1) % currentAnimation.Count;
            await UniTask.Delay(100);
        }
    }
    public void SetIdle (){
        if (currentAnimation == idleSprites) return;
        count = 0;  
        currentAnimation = idleSprites;
    }
    public void SetWalk (){
        if (currentAnimation == walkSprites) return;
        count = 0;  
        currentAnimation = walkSprites;
    }
    public void SetAttack (){
        if (currentAnimation == attackSprites) return;
        count = 0;  
        currentAnimation = attackSprites;
    }

    public void SetAttackDash (){
        if (currentAnimation == attackDashAnimation) return;
        count = 0;
        currentAnimation = attackDashAnimation;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.MonsterGrowl);
    }

    public async UniTask PlayHurtAnimation()
    {
        Color originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        count = 0;
        currentAnimation = hurtAnimation;
        LeanTween.cancel(gameObject);

        // Turn red then back to original color
        if (spriteRenderer != null)
        {
            LeanTween.value(gameObject, originalColor, Color.red, 0.15f)
                .setOnUpdate((Color color) =>
                {
                    if (spriteRenderer != null)
                        spriteRenderer.color = color;
                })
                .setOnComplete(() =>
                {
                    LeanTween.value(gameObject, Color.red, originalColor, 0.15f)
                        .setOnUpdate((Color color) =>
                        {
                            if (spriteRenderer != null)
                                spriteRenderer.color = color;
                        });
                });
        }
        await UniTask.Delay(hurtAnimation.Count * 110);
        count = 0;
        currentAnimation = idleSprites;
    }
    
    public void Flip(bool shouldFaceRight)
    {
        if (facingRight != shouldFaceRight)
        {
            facingRight = shouldFaceRight;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

    void LateUpdate()
    {
        // Sorting based on world Z position
        spriteRenderer.sortingOrder = (int)(-transform.position.z * precision) + sortingOffset;
    }

    public void Knockback(Vector3 dmgPosition)
    {
        Vector3 direction = (myself.position - dmgPosition).normalized;
        Vector3 startPos = myself.position;
        Vector3 endPos = startPos + direction * knockUpDistance;
        float knockbackDuration = 0.3f;
        LeanTween.move(myself.gameObject, endPos, knockbackDuration).setEaseOutQuad();
    }
    internal async UniTask SetDie(Vector3 dmgPosition)
    {
        LeanTween.cancel(gameObject);
        Knockback(dmgPosition);
        count = 0;
        currentAnimation = deadAnimation;
        await UniTask.Delay(deadAnimation.Count * 100);
        currentAnimation = new List<Sprite> { dieSprite };
    }
}