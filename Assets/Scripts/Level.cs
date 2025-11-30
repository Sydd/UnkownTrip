using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public List<Enemy> enemiesInLevel = new List<Enemy>();
    public Action<Level>   OnLevelCleared;
    public Transform portalPosition;
    public Transform playerSpawnPoint;
    public AudioClip levelMusic;
    public string levelName;
    private void Start()
    {
        foreach (var enemy in enemiesInLevel)
        {
            enemy.OnDie += () => HandleEnemyDeath(enemy);
        }
    }

    public void LoadLevel()
    {
        
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        enemiesInLevel.Remove(enemy);
        enemy.OnDie = null;
        GameObject.Destroy(enemy.gameObject);
        if (enemiesInLevel.Count == 0)
        {
            OnLevelCleared?.Invoke(this);
        }
    }

}
