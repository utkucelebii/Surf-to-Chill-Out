using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset;
    private Vector3 pos;
    private Vector3 rot;


    private void Start()
    {
        pos = player.position + offset;
        
        transform.position = pos;

        rot = new Vector3(30, 0, 0);

        transform.rotation = Quaternion.Euler(rot);
    }

    private void FixedUpdate()
    {
        pos = player.position + offset;
        if (!player.GetComponent<Player>().slide)
        {
            pos.y -= 5;
            pos.z += 5;
        }
        transform.position = Vector3.Lerp(transform.position, pos, 0.125f);

        transform.rotation = Quaternion.Euler(rot);
    }
}
