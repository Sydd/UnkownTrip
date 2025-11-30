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
    
    private void Awake()
    {
        // Set sprites before Start() is called on SpriteAnimators
        if (portalParteAmarilla != null)
        {
            portalParteAmarilla.spritesToAnimate = new List<Sprite>(idleAmarillaSprites);
            Debug.Log($"Amarilla sprites count: {idleAmarillaSprites.Count}");
        }
        if (portalParteMorada != null)
        {
            portalParteMorada.spritesToAnimate = new List<Sprite>(idleMoradaSprites);
            Debug.Log($"Morada sprites count: {idleMoradaSprites.Count}");
        }
        if (portalParteTurquesa != null)
        {
            portalParteTurquesa.spritesToAnimate = new List<Sprite>(idleTurquesaSprites);
            Debug.Log($"Turquesa sprites count: {idleTurquesaSprites.Count}");
        }
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