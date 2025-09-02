using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;

public class Selector : MonoBehaviour
{
    private const int TURN_LIMIT = 5;

    // Buttons
    [field: SerializeField] private GameObject Red { get; set; }
    [field: SerializeField] private GameObject Blue { get; set; }
    [field: SerializeField] private GameObject Green { get; set; }
    [field: SerializeField] private GameObject Yellow { get; set; }

    // Materials
    [SerializeField] private Material RedHighlight;
    [SerializeField] private Material BlueHighlight;
    [SerializeField] private Material GreenHighlight;
    [SerializeField] private Material YellowHighlight;

    // Sounds
    [field: SerializeField] private AudioClip RedSound { get; set; }
    [field: SerializeField] private AudioClip BlueSound { get; set; }
    [field: SerializeField] private AudioClip GreenSound { get; set; }
    [field: SerializeField] private AudioClip YellowSound { get; set; }

    private Queue<GameObject> colorsSelectedByCPU;
    private Queue<GameObject> colorsSelectedByPlayer;

    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        Red.GetComponent<Clickable>().clickEvent += ButtonSelected;
        Blue.GetComponent<Clickable>().clickEvent += ButtonSelected;
        Green.GetComponent<Clickable>().clickEvent += ButtonSelected;
        Yellow.GetComponent<Clickable>().clickEvent += ButtonSelected;

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

    private void PlaySound(GameObject color)
    {
        if (color == Red)
            audioSource.PlayOneShot(RedSound);
        else if (color == Blue)
            audioSource.PlayOneShot(BlueSound);
        else if (color == Green)
            audioSource.PlayOneShot(GreenSound);
        else if (color == Yellow)
            audioSource.PlayOneShot(YellowSound);
    }

    // coroutine to display the colors on the queue to the player to memorize
    IEnumerator DisplayColorsOnQueue()
    {
        // disable the collider from buttons
        DisableButtons();

        yield return new WaitForSeconds(2f);

        foreach (var color in colorsSelectedByCPU)
        {
            var renderer = color.GetComponent<Renderer>();
            var originalMat = renderer.material;

            // For some reason I cannot use the HighlightSelected() method here
            renderer.material = GetHighlightMaterial(color);
            PlaySound(color);
            yield return new WaitForSeconds(0.5f);
            renderer.material = originalMat;
            yield return new WaitForSeconds(0.5f);
        }

        // enable the colliders of buttons
        EnableButtons();
    }

    private void DisableButtons()
    {
        Red.GetComponent<BoxCollider>().enabled = false;
        Blue.GetComponent<BoxCollider>().enabled = false;
        Green.GetComponent<BoxCollider>().enabled = false;
        Yellow.GetComponent<BoxCollider>().enabled = false;
    }

    private void EnableButtons()
    {
        Red.GetComponent<BoxCollider>().enabled = true;
        Blue.GetComponent<BoxCollider>().enabled = true;
        Green.GetComponent<BoxCollider>().enabled = true;
        Yellow.GetComponent<BoxCollider>().enabled = true;
    }

    private IEnumerator HighlightSelected(GameObject color)
    {
        var renderer = color.GetComponent<Renderer>();
        var originalMat = renderer.material;

        renderer.material = GetHighlightMaterial(color);
        yield return new WaitForSeconds(0.5f);
        renderer.material = originalMat;
        yield return new WaitForSeconds(0.5f);
    }

    private Material GetHighlightMaterial(GameObject color)
    {
        var material = color.gameObject.name switch
        {
            "Red" => RedHighlight,
            "Blue" => BlueHighlight,
            "Green" => GreenHighlight,
            "Yellow" => YellowHighlight,
            _ => throw new ArgumentException("This button does not exist")
        };

        return material;
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

    public void ButtonSelected(GameObject sender)
    {
        GameObject selected = sender.gameObject.name switch
        {
            "Red" => Red,
            "Blue" => Blue,
            "Green" => Green,
            "Yellow" => Yellow,
            _ => throw new ArgumentException("This button is not valid")
        };

        colorsSelectedByPlayer.Enqueue(selected);
        PlaySound(selected);
        StartCoroutine(HighlightSelected(selected));
    }

    private void OnDestroy()
    {
        Red.GetComponent<Clickable>().clickEvent -= ButtonSelected;
        Blue.GetComponent<Clickable>().clickEvent -= ButtonSelected;
        Green.GetComponent<Clickable>().clickEvent -= ButtonSelected;
        Yellow.GetComponent<Clickable>().clickEvent -= ButtonSelected;
    }
}