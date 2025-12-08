using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

    [Header("Graphics Settings")]
    [SerializeField] private Toggle _fullscreenToggle;

    private Resolution[] _resolutions;

    private void Start()
    {
        LoadCurrentSettings();

        // Hook up listeners
        _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        _sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        _fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
    }


    private void LoadCurrentSettings()
    {
        _musicVolumeSlider.value = SettingsManager.Instance.MusicVolume;
        _sfxVolumeSlider.value = SettingsManager.Instance.SfxVolume;

        _fullscreenToggle.isOn = Screen.fullScreen;
    }

    private void OnMusicVolumeChanged(float value)
    {
        SettingsManager.Instance.SetMusicVolume(value);
    }

    private void OnSfxVolumeChanged(float value)
    {
        SettingsManager.Instance.SetSfxVolume(value);
    }

    private void OnFullscreenToggled(bool isFullscreen)
    {
        SettingsManager.Instance.SetFullscreen(isFullscreen);
    }

    public void OnApplyButtonPressed()
    {
        SettingsManager.Instance.SaveSettings();
    }

    public void OnBackButtonPressed()
    {
        gameObject.SetActive(false);
    }
}

