using System;
using System.Collections;
using Unity.Mathematics;
using UnityEditorInternal;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float maxAngle = 90f;
    public float velocity = 3f;

    private bool isOpen;
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    private void Start()
    {
        initialRotation = transform.rotation;
        targetRotation = Quaternion.Euler(transform.eulerAngles.x,
            transform.eulerAngles.y + maxAngle,
            transform.eulerAngles.z
        );
    }

    void Update()
    {
        // To Open the door by pressing the SPACE button
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     isOpen = !isOpen;
        //
        //     if (isOpen)
        //     {
        //         targetRotation = Quaternion.Euler(transform.eulerAngles.x,
        //             transform.eulerAngles.y + maxAngle,
        //             transform.eulerAngles.z
        //         );
        //     }
        //     else
        //     {
        //         targetRotation = initialRotation;
        //     }
        // }
        //
        // transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * velocity);
        
        // Open the door when the trigger is activated.
        Quaternion target = isOpen ? targetRotation : initialRotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * velocity);
    }

    public void OpenDoor()
    {
        isOpen = true;
    }

    public void CloseDoor()
    {
        isOpen = false;
    }
}