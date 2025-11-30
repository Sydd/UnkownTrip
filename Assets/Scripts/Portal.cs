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
    [SerializeField] private SpriteAnimator portalParteMorada;
    [SerializeField] private SpriteAnimator portalParteAmarilla;
    [SerializeField] private SpriteAnimator portalParteTurquesa;

    [SerializeField] private List<Sprite> idleVioletaSprites;
    [SerializeField] private List<Sprite> idleMoradaSprites;
    [SerializeField] private List<Sprite> idleAmarillaSprites;
    [SerializeField] private List<Sprite> idleTurquesaSprites;
    
    private void Start()
    {
        Spawn().Forget();
    }
    
    private async UniTask Spawn()
    {
        portalParteAmarilla.spritesToAnimate = idleAmarillaSprites;
        portalParteMorada.spritesToAnimate = idleMoradaSprites;
        portalParteTurquesa.spritesToAnimate = idleTurquesaSprites;
        portalParteVioleta.spritesToAnimate = idleVioletaSprites;
        await UniTask.Delay(500);
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