﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogShowController : MonoBehaviour
{
    // Text:
    public Text textDisplay;
    public string[] sentences;
    private int index;
    public float typingSpeed;

    // Event:
    private GameObject dialog;
    private bool isDisplayed = false;
    private PlayerController playerInfo;
    private Coroutine typing;

    private void Awake()
    {
        dialog = GameObject.Find("UI").transform.Find("Dialog").gameObject;
        playerInfo = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        // Don't show new letters / make a sound if in pause-menu:
        if (Time.timeScale != 1) return;

        // Wait for player's input and go to next sentence:
        if (isDisplayed)
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                NextSentence();
    }

    public void StartDialog()
    {
        // Enable dialog element:
        textDisplay.text = "";
        isDisplayed = true;
        dialog.SetActive(true);

        // Start showing letters:
        typing = StartCoroutine(Type());
    }

    public IEnumerator Type()
    {
        // Speak up:
        gameObject.GetComponent<SoundController>().PlaySound(index);

        // Show letters one after another in certain gaps of time:
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void NextSentence() 
    {
        if (index < sentences.Length - 1)
        {
            index++;

            // Reset text:
            textDisplay.text = "";

            // Stop typing current sentence:
            StopCoroutine(typing);

            // Start typing next sentence:
            typing = StartCoroutine(Type());
        }
        else
        {
            // Hide and reset the text:
            dialog.SetActive(false);
            textDisplay.text = "";
            isDisplayed = false;

            // Update player's state (he can be attacked now):
            playerInfo.StopReading();

            // Stop making the sound (if is still hearable):
            gameObject.GetComponent<AudioSource>().Stop();
        }
    }
}
