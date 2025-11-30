using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScroller : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.5f, 0.5f);
    [SerializeField] private string textureProperty = "_MainTex";

    private Material _material;
    private Vector2 _currentOffset;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        if (targetRenderer != null)
        {
            _material = targetRenderer.material;
        }
    }

    private void Update()
    {
        if (_material == null) { return; }

        _currentOffset += scrollSpeed * Time.deltaTime;
        _material.SetTextureOffset(textureProperty, _currentOffset);
    }
}
