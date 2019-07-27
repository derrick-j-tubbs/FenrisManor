using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DoorTrigger : MonoBehaviour {

    public CinemachineVirtualCamera initialCamera;
    public CinemachineVirtualCamera finalCamera;

    public GameObject roomSpawnPoint;
    protected PlayerController playerController;
    protected PlayerPlatformerController platformerController;

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
        if ((platformerController = player.GetComponent<PlayerPlatformerController>()) && 
                (playerController = player.GetComponent<PlayerController>())) {
            Debug.Log("Controls Off");
            ToggleControls(false);
            yield return new WaitForSeconds(1);
            Debug.Log("Controls On");
            ToggleControls(true);

        }
    }

    void ToggleControls(bool state) {
        platformerController.enabled = state;
        playerController.setIsClimbing(!state);
    }
}
