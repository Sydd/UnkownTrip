using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWobbleEffect : MonoBehaviour
{
    [Header("Wobble Settings")]
    [SerializeField] private float wobbleScale = 1.1f;
    [SerializeField] private float wobbleDuration = 0.6f;
    [SerializeField] private Ease wobbleEase = Ease.InOutSine;

    private RectTransform _rectTransform;
    private Tween _wobbleTween;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        StartWobbleLoop();
    }

    private void OnDisable()
    {
        _wobbleTween?.Kill();
    }

    private void StartWobbleLoop()
    {
        _wobbleTween?.Kill();

        _rectTransform.localScale = Vector3.one;

        _wobbleTween = _rectTransform
            .DOScale(wobbleScale, wobbleDuration)
            .SetEase(wobbleEase)
            .SetLoops(-1, LoopType.Yoyo);
    }

}
