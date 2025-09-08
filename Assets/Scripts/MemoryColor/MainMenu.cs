using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private CinemachineCamera zoomInCamera;
    [SerializeField] private CinemachineCamera zoomOutCamera;

    [SerializeField] private GameObject gameMenu;

    public void SwitchCamera()
    {
        if (zoomInCamera.IsLive)
        {
            zoomOutCamera.Priority.Value = 10;
            zoomInCamera.Priority.Value = 0;
            this.gameObject.SetActive(false);
            gameMenu.SetActive(true);
            
        }
        else if(zoomOutCamera.IsLive)
        {
            zoomOutCamera.Priority.Value = 0;
            zoomInCamera.Priority.Value = 10;
            this.gameObject.SetActive(true);
            gameMenu.SetActive(false);
        }
    }
}