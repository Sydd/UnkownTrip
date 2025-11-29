using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }

    [SerializeField] private Button _startBtn;
    [SerializeField] private Button _optionBtn;
    [SerializeField] private Button _exitBtn;
    [SerializeField] private Button _mainMenuBtn;

    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _optionMenu;
    [SerializeField] private GameObject _menuBackground;

    private void Awake()
    {
        if (Instance != null && SceneLoader.Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _startBtn.onClick.AddListener(StartGame);
        _optionBtn.onClick.AddListener(ShowSettingsMenu);
        _exitBtn.onClick.AddListener(ExitGame);
        _mainMenuBtn.onClick.AddListener(ShowMainMenu);
    }

    public void ShowMenu(MenuType menu)
    {
        switch (menu)
        {
            case MenuType.MainMenu:
                ShowMainMenu();
                break;
            case MenuType.OptionsMenu:
                ShowSettingsMenu();
                break;
            case MenuType.HideAllMenus:
                HideAllMenus();
                break;
        }
    }
    private void StartGame()
    {
        _mainMenu.SetActive(false);
        _optionMenu.SetActive(false);
        _menuBackground.SetActive(true);
        SceneLoader.Instance.LoadScene("Main");
        Debug.Log("asd");
    }

    private void ShowMainMenu()
    {
        _mainMenu.SetActive(true);
        _optionMenu.SetActive(false);
        _menuBackground.SetActive(true);
    }

    private void ShowSettingsMenu()
    {
        _optionMenu.SetActive(true);
        _mainMenu.SetActive(false);
        _menuBackground.SetActive(true);
    }

    private void HideAllMenus()
    {
        _optionMenu.SetActive(false);
        _mainMenu.SetActive(false);
        _menuBackground.SetActive(false);
    }

    private void ExitGame()
    {
        SceneLoader.Instance.QuitGame();
    }
}

public enum MenuType
{
    MainMenu,
    OptionsMenu,
    HideAllMenus,
}