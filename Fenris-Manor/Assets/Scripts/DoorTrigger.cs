using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DoorTrigger : MonoBehaviour {

    public CinemachineVirtualCamera initialCamera;
    public CinemachineVirtualCamera finalCamera;

    public GameObject roomSpawnPoint;

	public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        initialCamera.Priority = 0;
        finalCamera.Priority = 1;
        StartCoroutine(DisableControls());
               
        collision.gameObject.transform.position = roomSpawnPoint.transform.position;
    }

    IEnumerator DisableControls() {
        GameObject player = GameObject.Find("Player");
        PlayerPlatformerController controller;
        if (controller = player.GetComponent<PlayerPlatformerController>()) {
            Debug.Log("Controls Off");
            controller.enabled = false;
            yield return new WaitForSeconds(1);
            Debug.Log("Controls On");
            controller.enabled = true;

        }
    }
}
