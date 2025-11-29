using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeTool : MonoBehaviour
{
    public static FadeTool Instance { get; private set; }
    public SpriteRenderer fadeRenderer;
    [SerializeField] private float fadeDuration = 1f;
    
    private Camera mainCamera;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        mainCamera = Camera.main;
        
        // Position fade sprite in front of camera
        if (fadeRenderer != null && mainCamera != null)
        {
            // Make sprite transparent initially
            Color color = fadeRenderer.color;
            color.a = 0f;
            fadeRenderer.color = color;
            
            // Position in front of camera
            UpdateFadePosition();
        }
    }

    private void LateUpdate()
    {
        // Keep fade sprite in front of camera
        UpdateFadePosition();
    }

    private void UpdateFadePosition()
    {
        if (fadeRenderer != null && mainCamera != null)
        {
            // Position sprite in front of camera
            Vector3 cameraPos = mainCamera.transform.position;
            Vector3 cameraForward = mainCamera.transform.forward;
            fadeRenderer.transform.position = cameraPos + cameraForward * (mainCamera.nearClipPlane + 0.1f);
            fadeRenderer.transform.rotation = mainCamera.transform.rotation;
            
            // Scale sprite to cover camera view
            float distance = mainCamera.nearClipPlane + 0.1f;
            float height = 2f * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * distance;
            float width = height * mainCamera.aspect;
            fadeRenderer.transform.localScale = new Vector3(width, height, 1f);
        }
    }

    public void FadeIn()
    {
        if (fadeRenderer != null)
        {
            LeanTween.cancel(fadeRenderer.gameObject);
            
            Color targetColor = fadeRenderer.color;
            targetColor.a = 0f;
            
            LeanTween.value(fadeRenderer.gameObject, UpdateFadeAlpha, fadeRenderer.color.a, 0f, fadeDuration)
                .setOnUpdate((float alpha) =>
                {
                    if (fadeRenderer != null)
                    {
                        Color color = fadeRenderer.color;
                        color.a = alpha;
                        fadeRenderer.color = color;
                    }
                });
        }
    }

    public void FadeOut()
    {
        if (fadeRenderer != null)
        {
            LeanTween.cancel(fadeRenderer.gameObject);
            
            LeanTween.value(fadeRenderer.gameObject, UpdateFadeAlpha, fadeRenderer.color.a, 1f, fadeDuration)
                .setOnUpdate((float alpha) =>
                {
                    if (fadeRenderer != null)
                    {
                        Color color = fadeRenderer.color;
                        color.a = alpha;
                        fadeRenderer.color = color;
                    }
                });
        }
    }

    public void FadeOutAndIn()
    {
        if (fadeRenderer != null)
        {
            LeanTween.cancel(fadeRenderer.gameObject);
            
            // Fade to black
            LeanTween.value(fadeRenderer.gameObject, UpdateFadeAlpha, fadeRenderer.color.a, 1f, fadeDuration)
                .setOnUpdate((float alpha) =>
                {
                    if (fadeRenderer != null)
                    {
                        Color color = fadeRenderer.color;
                        color.a = alpha;
                        fadeRenderer.color = color;
                    }
                })
                .setOnComplete(() =>
                {
                    // Fade back to transparent
                    LeanTween.value(fadeRenderer.gameObject, UpdateFadeAlpha, 1f, 0f, fadeDuration)
                        .setOnUpdate((float alpha) =>
                        {
                            if (fadeRenderer != null)
                            {
                                Color color = fadeRenderer.color;
                                color.a = alpha;
                                fadeRenderer.color = color;
                            }
                        });
                });
        }
    }

    private void UpdateFadeAlpha(float alpha)
    {
        if (fadeRenderer != null)
        {
            Color color = fadeRenderer.color;
            color.a = alpha;
            fadeRenderer.color = color;
        }
    }
}
