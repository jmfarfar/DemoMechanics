using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    public Action backToMenuEvent;

    public void BackToMainMenu()
    {
        backToMenuEvent?.Invoke();
    }
}
