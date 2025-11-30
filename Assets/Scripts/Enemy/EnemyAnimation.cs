using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
 [SerializeField] private SpriteRenderer spriteRenderer;
 [SerializeField] private List<Sprite> walkSprites;
 [SerializeField] private List<Sprite> attackSprites;
 [SerializeField] private List<Sprite> idleSprites;
 [SerializeField] private List<Sprite> currentAnimation;
 
    private bool facingRight = true;
    
    // Start is called before the first frame update
    void Start()
    {
        RunWalkAnimation().Forget();
    }
    private async UniTask RunWalkAnimation(){
        int count = 0;
        while (true){
            spriteRenderer.sprite = currentAnimation[count];
            count = (count + 1) % currentAnimation.Count;
            await UniTask.Delay(250);
        }
    }
    public void SetIdle (){
        currentAnimation = idleSprites;
    }
    public void SetWalk (){
        currentAnimation = walkSprites;
    }
    public void SetAttack (){
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