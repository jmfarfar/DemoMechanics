using System;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    public Action<string> clickEvent;

    private void OnMouseDown()
    {
        clickEvent?.Invoke(gameObject.name);
    }
}
