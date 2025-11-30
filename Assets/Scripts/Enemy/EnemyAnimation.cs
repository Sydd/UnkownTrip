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
    public int sortingOffset = 0;   // use this if you want manual fine tuning
    public float precision = 100f;  // how accurate the sorting is

    int count = 0;
    private bool facingRight = true;
    
    // Start is called before the first frame update
    void Start()
    {
        RunWalkAnimation().Forget();
    }
    private async UniTask RunWalkAnimation(){
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
    }
    public void PlayHurtAnimation()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Vector3 originalScale = transform.localScale;
        Color originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        
        LeanTween.cancel(gameObject);
        
        // Scale down slightly
        LeanTween.scale(gameObject, originalScale * 0.9f, 0.15f).setOnComplete(() =>
        {
            // Scale back to original
            LeanTween.scale(gameObject, originalScale, 0.15f);
        });
        
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
}