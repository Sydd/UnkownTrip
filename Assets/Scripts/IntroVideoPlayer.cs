using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroVideoPlayer : MonoBehaviour
{
    [Header("Video Player")]
    public VideoPlayer videoPlayer;

    [Header("Main Menu Scene")]
    public string mainMenuSceneName = "MainMenu";

    private bool hasSkipped = false;

    private void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        videoPlayer.loopPointReached += OnVideoFinished; // Fired when MP4 ends
        videoPlayer.Play();
    }

    private void Update()
    {
        // If player pressed any key, skip the intro.
        if (!hasSkipped && Input.anyKeyDown)
        {
            hasSkipped = true;
            LoadMainMenu();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
