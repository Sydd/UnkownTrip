using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private Portal portal;
    [SerializeField] private FadeTool fadeTool;

    private void Awake()
    {
        fadeTool = FadeTool.Instance;
        portal.OnPlayerEnter += HandlePlayerEnterPortal;
    }

    private async void HandlePlayerEnterPortal()
    {
        
    }
}
public enum CurrentLevel
{
    BASE,
    AQUA,
    SPACE,
    WOODS
}