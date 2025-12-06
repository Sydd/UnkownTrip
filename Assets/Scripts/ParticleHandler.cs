using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHandler : MonoBehaviour
{
    PlayerAttack playerAttack;
    private void Start()
    {
        playerAttack = GameObject.FindObjectOfType<PlayerAttack>();
        if (playerAttack != null)
        {
            playerAttack.enemyHurt += (Vector3 pos) => 
            { 
               string spawn =  UnityEngine.Random.Range(0,2) == 0 ? "HitEffect" : "HitEffect2";
                ParticlesPool.Instance.Spawn(spawn, pos, Quaternion.identity,0.5f);
            };
        }   
    }
}
