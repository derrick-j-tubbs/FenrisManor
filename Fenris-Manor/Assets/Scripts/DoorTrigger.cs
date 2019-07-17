using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour {

    public GameObject initialCamera;
    public GameObject finalCamera;

    public GameObject roomSpawnPoint;

	public void OnTriggerEnter2D(Collider2D collision)
    {
        //print(collision.gameObject.tag);
        if (collision.gameObject.tag != "Player")
            return;
 
        initialCamera.gameObject.SetActive(false);
        finalCamera.gameObject.SetActive(true);
       
        collision.gameObject.transform.position = roomSpawnPoint.transform.position;
    }
}
