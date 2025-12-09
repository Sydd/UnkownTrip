using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public List<EnemyMelee> enemiesInLevel = new List<EnemyMelee>();
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

    private void HandleEnemyDeath(EnemyMelee enemy)
    {
        enemiesInLevel.Remove(enemy);
        enemy.OnDie = null;
        if (enemiesInLevel.Count == 0)
        {
            OnLevelCleared?.Invoke(this);
        }
    }

}
