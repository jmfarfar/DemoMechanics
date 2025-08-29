using System;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public Door door;
    private bool activated;

    private void OnTriggerEnter(Collider other)
    {
        // if the trigger is activated by the player, then open the door
        if (other.CompareTag("Player"))
        {
            activated = !activated;

            Debug.Log("Trigger activated");
            if (activated)
            {
                door.OpenDoor();
            }
            else
            {
                door.CloseDoor();
            }
        }
    }
}