using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameOverImage : MonoBehaviour
{
    [SerializeField] private Image[] imageslist;
    [SerializeField] private Image background,background2;
    public static GameOverImage instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        DeactivateAll();
        TurnOffBackground();   
    }
    public void TurnOnBackground()
    {
        background.gameObject.SetActive(true);
        background2.gameObject.SetActive(true);
    }
    public void TurnOffBackground()
    {
        background.gameObject.SetActive(false);
        background2.gameObject.SetActive(false);
    }
    public async UniTask FadeIn()
    {
        
        // Ensure objects are active when starting fade in
        SetActiveAll(true);
        float duration = 1f;
        float elapsed = 0f;

        // Ensure starting alpha is 0 for all images
        for (int i = 0; i < imageslist.Length; i++)
        {
            if (imageslist[i] == null) continue;
            var c = imageslist[i].color;
            c.a = 0f;
            imageslist[i].color = c;
        }

        // Animate alpha to 1 over duration
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            for (int i = 0; i < imageslist.Length; i++)
            {
                if (imageslist[i] == null) continue;
                var c = imageslist[i].color;
                c.a = t;
                imageslist[i].color = c;
            }
            await UniTask.Yield();
        }
    }

    public async UniTask FadeOut()
    {
        float duration = 1f;
        float elapsed = 0f;

        // Ensure starting alpha is 1 for all images
        for (int i = 0; i < imageslist.Length; i++)
        {
            if (imageslist[i] == null) continue;
            var c = imageslist[i].color;
            c.a = 1f;
            imageslist[i].color = c;
        }

        // Animate alpha to 0 over duration
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            for (int i = 0; i < imageslist.Length; i++)
            {
                if (imageslist[i] == null) continue;
                var c = imageslist[i].color;
                c.a = 1f - t;
                imageslist[i].color = c;
            }
            await UniTask.Yield();
        }

        // Deactivate after fade out completes
        SetActiveAll(false);
    }

    public void ActivateAll()
    {
        for (int i = 0; i < imageslist.Length; i++)
        {
            if (imageslist[i] == null) continue;
            imageslist[i].gameObject.SetActive(true);
        }
    }

    public void DeactivateAll()
    {
        for (int i = 0; i < imageslist.Length; i++)
        {
            if (imageslist[i] == null) continue;
            imageslist[i].gameObject.SetActive(false);
        }
    }

    public void SetActiveAll(bool isActive)
    {
        for (int i = 0; i < imageslist.Length; i++)
        {
            if (imageslist[i] == null) continue;
            imageslist[i].gameObject.SetActive(isActive);
        }
    }
}
