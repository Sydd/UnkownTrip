using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private Portal portal;
    [SerializeField] private FadeTool fadeTool;
    [SerializeField] private List<Level> levels;
    [SerializeField] private bool onLevel = false;
    [SerializeField] private EntryBase entryBase;
    [SerializeField] private PlayerStatus playerStatus;
    private void Awake()
    {
        fadeTool = FadeTool.Instance;
        portal.OnPlayerEnter += async () => await HandlePlayerEnterPortal();
        foreach (var level in levels)
        {
            level.OnLevelCleared += async (lvl) => await OnLevelCleared(lvl);
        }
        playerStatus.OnDeath += async () => await HandlePlayerDeath();
    }

    private async UniTask HandlePlayerDeath()
    {
        await UniTask.Delay(1000);
        fadeTool.FadeIn();
        await UniTask.Delay(1000);
        SceneManager.LoadScene(0);
    }

    private async UniTask OnLevelCleared(Level level)
    {
        await UniTask.Delay(1000);
        portal.transform.position = level.portalPosition.position;
        portal.gameObject.SetActive(true);
        await UniTask.Delay(1000);
        if (levels.Count > 0) portal.isActive = true;
    }
    private async UniTask HandlePlayerEnterPortal()
    {
        portal.isActive = false;
        fadeTool.FadeIn();
        await UniTask.Delay(1000);
        if (onLevel)
        {
            entryBase.gameObject.SetActive(true);
            portal.transform.position = entryBase.portalSpawnPoint.position;
            playerStatus.transform.position = entryBase.playerSpawnPoint.position;
            fadeTool.FadeOut();
            onLevel = false;
        }
        else
        {
            entryBase.gameObject.SetActive(false);
            Level nextLevel = GetRandomLevel();
            nextLevel.gameObject.SetActive(true);   
            playerStatus.transform.position = nextLevel.playerSpawnPoint.position;
            onLevel = true;
        }

        fadeTool.FadeOut();
    }

    private Level GetRandomLevel(){
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        var num = UnityEngine.Random.Range(0, levels.Count);
        Level level = levels[num];
        levels.RemoveAt(num);
        return level;
    }
}