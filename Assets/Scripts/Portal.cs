using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Action OnPlayerEnter;
    public bool isActive = true;
    [SerializeField] private SpriteAnimator portalParteVioleta;

    [SerializeField] private List<Sprite> idleVioletaSprites;
    
    private void Awake()
    {
        if (portalParteVioleta != null)
        {
            portalParteVioleta.spritesToAnimate = new List<Sprite>(idleVioletaSprites);
            Debug.Log($"Violeta sprites count: {idleVioletaSprites.Count}");
        }
    }
    
    private void Start()
    {
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isActive)
            {
                OnPlayerEnter?.Invoke();
            }
        }
    }
}