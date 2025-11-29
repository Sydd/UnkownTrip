using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _player;

    void Update()
    {
        this.gameObject.transform.position = _player.transform.position;
    }
}
