using DG.Tweening;
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
    [SerializeField] private Button _creditsBtn;
    [SerializeField] private Button _exitBtn;
    [SerializeField] private Button _mainMenuBtn;

    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _optionMenu;
    [SerializeField] private GameObject _creditsMenu;
    [SerializeField] private GameObject _menuBackground;
    [SerializeField] private GameObject _menuPortal;
    [SerializeField] private GameObject _menuForeground;
    [SerializeField] private GameObject _creditsImg;

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
        _creditsBtn.onClick.AddListener(ShowCreditsMenu);
        _creditsImg.GetComponent<Image>().DOFade(0f, 1f)
             .SetLoops(-1, LoopType.Yoyo)
             .SetEase(Ease.Linear);
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
            case MenuType.CreditsMenu:
                ShowSettingsMenu();
                break;
            case MenuType.HideAllMenus:
                HideAllMenus();
                break;
        }
    }
    public void StartGame()
    {
        _mainMenu.SetActive(false);
        _optionMenu.SetActive(false);
        _menuBackground.SetActive(false);
        _menuForeground.SetActive(false);
        _menuPortal.SetActive(false);
        SceneLoader.Instance.LoadScene("Main");
    }

    public void ShowMainMenu()
    {
        _mainMenu.SetActive(true);
        _optionMenu.SetActive(false);
        _menuBackground.SetActive(true);
        _menuForeground.SetActive(true);
        _creditsMenu.SetActive(false);
        _menuPortal.SetActive(true);
    }

    public void ShowSettingsMenu()
    {
        _optionMenu.SetActive(true);
        _mainMenu.SetActive(false);
        _menuBackground.SetActive(true);
        _menuForeground.SetActive(false);
        _menuForeground.SetActive(false);
        _creditsMenu.SetActive(false);
        _menuPortal.SetActive(false);
    }

    public void ShowCreditsMenu()
    {
        _optionMenu.SetActive(true);
        _mainMenu.SetActive(false);
        _menuBackground.SetActive(true);
        _menuForeground.SetActive(false);
        _creditsMenu.SetActive(true);
        _menuPortal.SetActive(false);
    }

    public void HideAllMenus()
    {
        _optionMenu.SetActive(false);
        _mainMenu.SetActive(false);
        _menuBackground.SetActive(false);
        _menuForeground.SetActive(false);
        _creditsMenu.SetActive(false);
        _menuPortal.SetActive(false);
    }

    public void ExitGame()
    {
        SceneLoader.Instance.QuitGame();
    }
}

public enum MenuType
{
    MainMenu,
    OptionsMenu,
    CreditsMenu,
    HideAllMenus,
}