using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageMenuNavigator : MonoBehaviour
{
    [System.Serializable]
    public class MenuItem
    {
        public Image image;         // The image used as a button
        public UnityEvent onSelect; // Action executed when pressing Submit
    }

    [Header("Menu Items (Top → Bottom)")]
    [SerializeField] private MenuItem[] menuItems;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color pressedColor = new Color(1f, 0.5f, 0.5f);

    [Header("Navigation Settings")]
    [SerializeField] private float inputCooldown = 0.25f;

    private int _currentIndex = 0;
    private float _nextInput;

    private void Start()
    {
        HighlightItem(_currentIndex);
    }

    private void Update()
    {
        float vertical = Input.GetAxis("Vertical");

        if (Time.time >= _nextInput)
        {
            if (vertical > 0.5f)
                MoveSelection(-1);
            else if (vertical < -0.5f)
                MoveSelection(+1);
        }

        if (Input.GetButtonDown("Submit"))
        {
            PressCurrentItem();
        }
    }

    private void MoveSelection(int direction)
    {
        _currentIndex += direction;

        if (_currentIndex < 0) _currentIndex = menuItems.Length - 1;
        if (_currentIndex >= menuItems.Length) _currentIndex = 0;

        HighlightItem(_currentIndex);

        _nextInput = Time.time + inputCooldown;
    }

    private void HighlightItem(int index)
    {
        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItems[i].image.color = (i == index) ? selectedColor : normalColor;
        }
    }

    private void PressCurrentItem()
    {
        // Flash pressed color
        menuItems[_currentIndex].image.color = pressedColor;

        // Restore highlight after a moment
        Invoke(nameof(RestoreHighlight), 0.1f);

        // Trigger assigned action
        menuItems[_currentIndex].onSelect.Invoke();
    }

    private void RestoreHighlight()
    {
        HighlightItem(_currentIndex);
    }
}
