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
        InitializeResolutions();
        LoadCurrentSettings();

        // Hook up listeners
        _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        _sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        _fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
    }

    private void InitializeResolutions()
    {
        _resolutions = Screen.resolutions;

        var options = new System.Collections.Generic.List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height;
            options.Add(option);

            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
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

