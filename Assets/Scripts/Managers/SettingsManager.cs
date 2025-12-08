using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    private const float DefaultMasterVolume = 1f;
    private const float DefaultMusicVolume = 1f;
    private const float DefaultSfxVolume = 1f;
    private const int DefaultQuality = 2;
    private const int DefaultFullscreen = 1;
    private const int DefaultResolutionIndex = 0;
    private const int DefaultResolutionWidth = 1920;
    private const int DefaultResolutionHeight = 1080;


    public float MasterVolume { get; private set; }
    public float MusicVolume { get; private set; }
    public float SfxVolume { get; private set; }
    public int QualityLevel { get; private set; }
    public bool IsFullscreen { get; private set; }
    public int ResolutionIndex { get; private set; }
    public int ResolutionWidth { get; private set; }
    public int ResolutionHeight { get; private set; }


    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
        ApplySettings();
    }

    public void LoadSettings()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", DefaultMasterVolume);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", DefaultMusicVolume);
        SfxVolume = PlayerPrefs.GetFloat("SfxVolume", DefaultSfxVolume);
        QualityLevel = PlayerPrefs.GetInt("QualityLevel", DefaultQuality);
        IsFullscreen = PlayerPrefs.GetInt("Fullscreen", DefaultFullscreen) == 1;
        ResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", DefaultResolutionIndex);
    }

    public void ApplySettings()
    {
        AudioManager.Instance?.SetMusicVolume(MusicVolume);
        AudioManager.Instance?.SetSFXVolume(SfxVolume);

        //QualitySettings.SetQualityLevel(QualityLevel);
        Screen.fullScreen = IsFullscreen;
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        AudioManager.Instance?.SetMusicVolume(MusicVolume);
    }

    public void SetSfxVolume(float value)
    {
        SfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("SfxVolume", SfxVolume);
        AudioManager.Instance?.SetSFXVolume(SfxVolume);
    }

    public void SetQuality(int index)
    {
        QualityLevel = index;
        PlayerPrefs.SetInt("QualityLevel", index);
        //QualitySettings.SetQualityLevel(index);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        IsFullscreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        Screen.fullScreen = isFullscreen;
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }
}
