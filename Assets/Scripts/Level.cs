using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public List<IEnemy> enemiesInLevel = new List<IEnemy>();
    public Action<Level>   OnLevelCleared;
    public Transform portalPosition;
    public Transform playerSpawnPoint;
    public Transform enemyContainer;
    public AudioClip levelMusic;
    public string levelName;
    public Material levelMaterial;
    private void Start()
    {
        enemiesInLevel = new List<IEnemy>();
        enemiesInLevel.AddRange(enemyContainer.GetComponentsInChildren<EnemyRanger>());
        enemiesInLevel.AddRange(enemyContainer.GetComponentsInChildren<EnemyMelee>());
        foreach (var enemy in enemiesInLevel)
        {
            enemy.OnDie += () => HandleEnemyDeath(enemy);
        }
    }

    public void LoadLevel()
    {
        RenderSettings.skybox = levelMaterial;
        DynamicGI.UpdateEnvironment();
    }

    private void HandleEnemyDeath(IEnemy enemy)
    {
        enemiesInLevel.Remove(enemy);
        enemy.OnDie = null;
        if (enemiesInLevel.Count == 0)
        {
            OnLevelCleared?.Invoke(this);
        }
    }

}
