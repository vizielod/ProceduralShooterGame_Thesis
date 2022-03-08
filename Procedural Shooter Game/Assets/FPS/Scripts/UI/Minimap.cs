using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform player;
    [SerializeField] private bool rotateWithPlayer;

    private void LateUpdate()
    {
        
        //Move minimap camera with the player
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
        
        //Rotate minimap camera with the player
        if (rotateWithPlayer)
        {
            transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }
    }
}
