using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameObject Player;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == Player)
        {
            collider.GetComponent<PlayerController>().Respawn_Position = transform.position;

            Destroy(gameObject);
        }
    }
}
