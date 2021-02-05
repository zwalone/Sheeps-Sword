﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    // Event:
    private GameObject dialog;
    private GameObject mainCamera;

    private void Awake()
    {
        mainCamera = GameObject.Find("Main Camera");
        dialog = GameObject.Find("Dialog").gameObject;
        dialog.GetComponent<IntroDialogController>().StartDialog();
        GameObject.Find("Music").GetComponents<AudioSource>()[0].ignoreListenerPause = true;
    }

    public void EndScene()
    {
        StopCoroutine(mainCamera.GetComponent<CameraTrackController>().lightsOn);
        StartCoroutine(mainCamera.GetComponent<CameraTrackController>().LightsOff());
        StartCoroutine(VolumeDown());
        Invoke(nameof(SheepSound), 6.0f);
        Invoke(nameof(SheepSound), 8.0f);
        Invoke(nameof(SheepSound), 10.0f);
        Invoke(nameof(NewLevel), 12.0f);
    }

    private IEnumerator VolumeDown()
    {
        AudioSource music = GameObject.Find("Music").GetComponents<AudioSource>()[0];
        while (music.volume > 0)
        {
            music.volume -= 0.01f;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void SheepSound()
    {
        GameObject.Find("Music").GetComponents<AudioSource>()[1].Play();
    }

    private void NewLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
