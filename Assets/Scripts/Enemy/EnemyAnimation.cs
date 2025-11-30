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
            if (currentAnimation[count] == null) 
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
    
    public void Flip(bool shouldFaceRight)
    {
        if (facingRight != shouldFaceRight)
        {
            facingRight = shouldFaceRight;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }
}