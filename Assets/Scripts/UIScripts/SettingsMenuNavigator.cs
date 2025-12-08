using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SettingsMenuNavigator : MonoBehaviour
{
    public enum ItemType
    {
        Slider,
        Toggle,
        Button
    }

    [System.Serializable]
    public class SettingsItem
    {
        public ItemType type;
        public Image image;

        public Slider slider;   // if type == Slider
        public Toggle toggle;   // if type == Toggle
        public UnityEvent onPress; // if type == Button
    }

    [Header("Settings Items (Top → Bottom)")]
    [SerializeField] private SettingsItem[] items;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color pressedColor = new Color(1f, 0.5f, 0.5f);

    [Header("Navigation Settings")]
    [SerializeField] private float inputCooldown = 0.25f;
    [SerializeField] private float sliderStep = 0.05f;

    private int index = 0;
    private float nextInputTime;

    private void Start()
    {
        Highlight(index);
    }

    private void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (Time.time >= nextInputTime)
        {
            if (vertical > 0.5f)
                Move(-1);        // Up

            else if (vertical < -0.5f)
                Move(+1);        // Down
        }

        HandleCurrentItem(horizontal);

        if (Input.GetButtonDown("Submit"))
            PressCurrent();

    }

    private void Move(int direction)
    {
        index += direction;

        if (index < 0) index = items.Length - 1;
        if (index >= items.Length) index = 0;

        Highlight(index);

        nextInputTime = Time.time + inputCooldown;
    }

    private void Highlight(int idx)
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].image.color = (i == idx) ? selectedColor : normalColor;
        }
    }

    private void HandleCurrentItem(float horizontal)
    {
        SettingsItem item = items[index];

        switch (item.type)
        {
            case ItemType.Slider:
                if (horizontal > 0.5f)
                    item.slider.value += sliderStep;
                else if (horizontal < -0.5f)
                    item.slider.value -= sliderStep;
                break;

            case ItemType.Toggle:
                if (Input.GetButtonDown("Submit"))
                    item.toggle.isOn = !item.toggle.isOn;
                break;

            case ItemType.Button:
                // only submits, no horizontal movement
                break;
        }
    }

    private void PressCurrent()
    {
        SettingsItem item = items[index];

        // Flash pressed color
        item.image.color = pressedColor;
        Invoke(nameof(RestoreHighlight), 0.1f);

        // Trigger event only for buttons
        if (item.type == ItemType.Button)
            item.onPress.Invoke();
    }

    private void RestoreHighlight()
    {
        Highlight(index);
    }
}
