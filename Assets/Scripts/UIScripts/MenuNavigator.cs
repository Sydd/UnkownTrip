using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MenuNavigator : MonoBehaviour
{
    public enum ItemType
    {
        Button,
        Slider,
        Toggle
    }

    [System.Serializable]
    public class NavItem
    {
        public ItemType type;
        public Image image;

        public UnityEvent onPress;
        public Slider slider;
        public Toggle toggle;
    }

    [Header("Root panel of this menu")]
    [SerializeField] private GameObject menuRoot;

    [Header("Menu items (Top → Bottom)")]
    [SerializeField] private NavItem[] items;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color pressedColor = new Color(1f, 0.5f, 0.5f);

    [Header("Settings")]
    [SerializeField] private float inputCooldown = 0.25f;
    [SerializeField] private float sliderStep = 0.05f;

    private int index = 0;
    private float nextInput;

    private void OnEnable()
    {
        // Reset to first item when menu opens
        index = 0;
        HighlightItem(index);
    }

    private void Update()
    {
        // Prevent input when menu is not visible
        if (!menuRoot.activeInHierarchy)
            return;

        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        // Vertical navigation
        if (Time.time >= nextInput)
        {
            if (v > 0.5f) Move(-1);
            else if (v < -0.5f) Move(+1);
        }

        // Handle slider/toggle interactions
        HandleCurrentItem(h);

        // Submit button
        if (Input.GetButtonDown("Submit"))
            PressCurrent();
    }

    private void Move(int dir)
    {
        index += dir;

        if (index < 0) index = items.Length - 1;
        if (index >= items.Length) index = 0;

        HighlightItem(index);
        nextInput = Time.time + inputCooldown;
    }

    private void HighlightItem(int i)
    {
        for (int j = 0; j < items.Length; j++)
        {
            items[j].image.color = (j == i) ? selectedColor : normalColor;
        }
    }

    private void HandleCurrentItem(float h)
    {
        var item = items[index];

        switch (item.type)
        {
            case ItemType.Slider:
                if (h > 0.5f) item.slider.value += sliderStep;
                else if (h < -0.5f) item.slider.value -= sliderStep;
                break;

            case ItemType.Toggle:
                if (Input.GetButtonDown("Submit"))
                    item.toggle.isOn = !item.toggle.isOn;
                break;
        }
    }

    private void PressCurrent()
    {
        var item = items[index];

        // Flash pressed color
        item.image.color = pressedColor;
        Invoke(nameof(RestoreHighlight), 0.1f);

        if (item.type == ItemType.Button)
            item.onPress.Invoke();
    }

    private void RestoreHighlight()
    {
        HighlightItem(index);
    }
}
