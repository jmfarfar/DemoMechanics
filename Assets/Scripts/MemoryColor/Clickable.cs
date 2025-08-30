using System;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    public Action<GameObject> clickEvent;

    private void OnMouseDown()
    {
        clickEvent?.Invoke(gameObject);
    }
}
