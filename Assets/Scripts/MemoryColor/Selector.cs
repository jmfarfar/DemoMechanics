using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;

public class Selector : MonoBehaviour
{
    private const int TURN_LIMIT = 5;

    [field: SerializeField] private GameObject Red { get; set; }
    [field: SerializeField] private GameObject Blue { get; set; }
    [field: SerializeField] private GameObject Green { get; set; }
    [field: SerializeField] private GameObject Yellow { get; set; }

    private Queue<GameObject> colorsSelectedByCPU;
    private Queue<GameObject> colorsSelectedByPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // change this to get objects with the Find method.
        Red.GetComponent<Clickable>().clickEvent += RedSelected;
        Blue.GetComponent<Clickable>().clickEvent += BlueSelected;
        Green.GetComponent<Clickable>().clickEvent += GreenSelected;
        Yellow.GetComponent<Clickable>().clickEvent += YellowSelected;

        colorsSelectedByPlayer = new Queue<GameObject>();
        colorsSelectedByCPU = new Queue<GameObject>();

        SelectRandomColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (colorsSelectedByCPU.Count <= TURN_LIMIT)
        {
            if (colorsSelectedByPlayer.Count.Equals(colorsSelectedByCPU.Count))
            {
                if (colorsSelectedByPlayer.SequenceEqual(colorsSelectedByCPU))
                {
                    // win
                    Debug.Log("Good!");
                    colorsSelectedByPlayer.Clear(); // remove all selections from player
                    SelectRandomColor(); // add a new color in the queue
                }
                else
                {
                    Debug.Log("Fail! Better luck next time!");
                    colorsSelectedByPlayer.Clear();
                    colorsSelectedByCPU.Clear();
                    SelectRandomColor();
                }
            }
        }
        else
        {
            Debug.Log("You have beaten the game");
            colorsSelectedByPlayer.Clear();
            colorsSelectedByCPU.Clear();
            SelectRandomColor();
        }
    }

    // coroutine to display the colors on the queue to the player to memorize
    IEnumerator DisplayColorsOnQueue()
    {
        yield return new WaitForSeconds(2f);

        foreach (var color in colorsSelectedByCPU)
        {
            Debug.Log(color.gameObject.name);
            new WaitForSeconds(1f);
        }
    }

    // select a random color and add it to the queue
    private void SelectRandomColor()
    {
        var color = UnityEngine.Random.Range(0, 4) switch
        {
            0 => Red,
            1 => Blue,
            2 => Green,
            3 => Yellow,
            _ => throw new ArgumentOutOfRangeException(null, "The value is out of Range")
        };

        colorsSelectedByCPU.Enqueue(color);

        StartCoroutine(DisplayColorsOnQueue());
    }

    public void RedSelected(string arg)
    {
        colorsSelectedByPlayer.Enqueue(Red);
    }

    public void BlueSelected(string arg)
    {
        colorsSelectedByPlayer.Enqueue(Blue);
    }

    public void GreenSelected(string arg)
    {
        colorsSelectedByPlayer.Enqueue(Green);
    }

    public void YellowSelected(string arg)
    {
        colorsSelectedByPlayer.Enqueue(Yellow);
    }

    private void OnDestroy()
    {
        Red.GetComponent<Clickable>().clickEvent -= RedSelected;
        Blue.GetComponent<Clickable>().clickEvent -= BlueSelected;
        Green.GetComponent<Clickable>().clickEvent -= GreenSelected;
        Yellow.GetComponent<Clickable>().clickEvent -= YellowSelected;
    }
}