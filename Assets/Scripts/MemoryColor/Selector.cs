using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Random = UnityEngine.Random;

public class Selector : MonoBehaviour
{
    private const int TURN_LIMIT = 5;

    private readonly string[] correctAns = { "Good!", "Marvelous!", "Fantastic!", "Excellent!", "Great!", "Awesome!" };
    private readonly string[] incorrectAns = { "Oww!", "Fail!", "Wrong!", "Are you even trying!?" };


    // Buttons
    [field: SerializeField] private GameObject Red { get; set; }
    [field: SerializeField] private GameObject Blue { get; set; }
    [field: SerializeField] private GameObject Green { get; set; }
    [field: SerializeField] private GameObject Yellow { get; set; }

    // Base Materials
    [SerializeField] private Material RedBase;
    [SerializeField] private Material BlueBase;
    [SerializeField] private Material GreenBase;
    [SerializeField] private Material YellowBase;

    // Highlighted Materials
    [SerializeField] private Material RedHighlight;
    [SerializeField] private Material BlueHighlight;
    [SerializeField] private Material GreenHighlight;
    [SerializeField] private Material YellowHighlight;

    // Sounds
    [field: SerializeField] private AudioClip RedSound { get; set; }
    [field: SerializeField] private AudioClip BlueSound { get; set; }
    [field: SerializeField] private AudioClip GreenSound { get; set; }
    [field: SerializeField] private AudioClip YellowSound { get; set; }

    // Feedback messages
    [SerializeField] private TextMeshProUGUI message;

    private Queue<GameObject> colorsSelectedByCPU;
    private Queue<GameObject> bkp_colorsSelectedByCPU;
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
        bkp_colorsSelectedByCPU = new Queue<GameObject>();

        SelectRandomColor();
    }

    private IEnumerator DisplayWrongMessage()
    {
        message.SetText(incorrectAns[Random.Range(0, incorrectAns.Length)]);
        yield return new WaitForSeconds(1f);
        message.SetText(string.Empty);
    }

    private IEnumerator DisplayGoodMessage()
    {
        message.SetText(correctAns[Random.Range(0, correctAns.Length)]);
        yield return new WaitForSeconds(1f);
        message.SetText(string.Empty);
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
            // var originalMat = renderer.material;

            // For some reason I cannot use the HighlightSelected() method here
            renderer.material = GetHighlightMaterial(color);
            PlaySound(color);
            yield return new WaitForSeconds(0.5f);
            renderer.material = GetBaseMaterial(color);
            yield return new WaitForSeconds(0.5f);
        }

        // enable the colliders of buttons
        EnableButtons();
    }

    private void DisableButtons()
    {
        Red.GetComponent<MeshCollider>().enabled = false;
        Blue.GetComponent<MeshCollider>().enabled = false;
        Green.GetComponent<MeshCollider>().enabled = false;
        Yellow.GetComponent<MeshCollider>().enabled = false;
    }

    private void EnableButtons()
    {
        Red.GetComponent<MeshCollider>().enabled = true;
        Blue.GetComponent<MeshCollider>().enabled = true;
        Green.GetComponent<MeshCollider>().enabled = true;
        Yellow.GetComponent<MeshCollider>().enabled = true;
    }

    private IEnumerator HighlightSelected(GameObject color)
    {
        var renderer = color.GetComponent<Renderer>();
        // var originalMat = renderer.material;

        renderer.material = GetHighlightMaterial(color);
        yield return new WaitForSeconds(0.5f);
        renderer.material = GetBaseMaterial(color);
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

    private Material GetBaseMaterial(GameObject color)
    {
        var material = color.gameObject.name switch
        {
            "Red" => RedBase,
            "Blue" => BlueBase,
            "Green" => GreenBase,
            "Yellow" => YellowBase,
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
        bkp_colorsSelectedByCPU.Enqueue(color);

        StartCoroutine(DisplayColorsOnQueue());
    }

    public void ButtonSelected(GameObject sender)
    {
        colorsSelectedByPlayer.Enqueue(sender);
        PlaySound(sender);
        StartCoroutine(HighlightSelected(sender));

        // here we may verify if the button selected is the next on the queue from the CPU
        if (sender == colorsSelectedByCPU.Dequeue())
        {
            // every selection was correct
            if (colorsSelectedByCPU.Count <= 0)
            {
                // check if we have reached the limit of turns in the game
                if (bkp_colorsSelectedByCPU.Count >= TURN_LIMIT)
                {
                    Debug.Log("You have beaten the game");
                    StartCoroutine(DisplayGoodMessage());
                    colorsSelectedByPlayer.Clear();
                    colorsSelectedByCPU.Clear();
                    bkp_colorsSelectedByCPU.Clear();
                    SelectRandomColor();
                }
                // just display a good message and continue with the game
                else
                {
                    // win, there are no more elements on the queue and all matched with the player's selection
                    Debug.Log("Good!");
                    StartCoroutine(DisplayGoodMessage());
                    colorsSelectedByPlayer.Clear(); // remove all selections from player
                    colorsSelectedByCPU = new Queue<GameObject>(bkp_colorsSelectedByCPU);
                    SelectRandomColor(); // add a new color in the queue
                }
            }
        }
        else
        {
            // lose, a player's selection didnt match the next element on the queue
            Debug.Log("Fail! Better luck next time!");
            StartCoroutine(DisplayWrongMessage());
            colorsSelectedByPlayer.Clear();
            colorsSelectedByCPU.Clear();
            bkp_colorsSelectedByCPU.Clear();
            SelectRandomColor();
        }
    }

    private void OnDestroy()
    {
        Red.GetComponent<Clickable>().clickEvent -= ButtonSelected;
        Blue.GetComponent<Clickable>().clickEvent -= ButtonSelected;
        Green.GetComponent<Clickable>().clickEvent -= ButtonSelected;
        Yellow.GetComponent<Clickable>().clickEvent -= ButtonSelected;
    }
}