using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool scale = true;
    [SerializeField] private bool deactivateAfterAnimation = false;
    [SerializeField] private int step = 250;
    public List<Sprite> spritesToAnimate;
    private Vector3 originalScale;

    private void OnEnable()
    {
        originalScale = transform.localScale;
        RunAnimation().Forget();
        if (scale) StartScaleAnimation();
    }

    private void OnDisable()
    {
        // Cancel any tweens on disable to avoid lingering animations
        LeanTween.cancel(gameObject);
        transform.localScale = originalScale;

    }
    
    private void StartScaleAnimation()
    {
        Vector3 targetScale = originalScale * 1.5f;
        
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
        bool running = true;
        while (running)
        {
            if (this == null) return;
            if (!gameObject.activeInHierarchy) return;
            if (spritesToAnimate != null && spritesToAnimate.Count > 0) 
            {
                spriteRenderer.sprite = spritesToAnimate[count];
                // Advance frame
                int next = (count + 1) % spritesToAnimate.Count;
                // If we completed a full cycle and should deactivate, do it reliably
                if (deactivateAfterAnimation && next == 0)
                {
                    gameObject.SetActive(false);
                    return;
                }
                count = next;
            }
            await UniTask.Delay(step);
        }
    }
}
