using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public List<Sprite> spritesToAnimate;
    
    private void Start()
    {
        RunAnimation().Forget();
        StartScaleAnimation();
    }
    
    private void StartScaleAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        
        LeanTween.scale(gameObject, targetScale, 1f)
            .setEaseInOutSine()
            .setLoopPingPong();
    }
    
    private async UniTask RunAnimation()
    {
        // Wait until sprites are assigned
        while (spritesToAnimate == null || spritesToAnimate.Count == 0)
        {
            await UniTask.Yield();
        }
        
        int count = 0;
        while (true)
        {
            if (this == null) return;
            if (spritesToAnimate != null && spritesToAnimate.Count > 0) 
            {
                spriteRenderer.sprite = spritesToAnimate[count];
                count = (count + 1) % spritesToAnimate.Count;
            }
            await UniTask.Delay(250);
        }
    }
}
