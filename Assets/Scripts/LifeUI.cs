using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeUI : MonoBehaviour
{
	[SerializeField] private Image[] heartImages;
	[SerializeField] private Sprite heartFull;
	[SerializeField] private Sprite heartHalf;
	[SerializeField] private Sprite heartEmpty;
	[SerializeField] private PlayerStatus playerStatus;

void Start()
	{
		RenderHearts(playerStatus.CurrentHealth);
	}
	void OnEnable()
	{
		// If PlayerStatus exposes an OnDamaged or OnHealthChanged event, subscribe
		// These null-conditional subscriptions are safe if events exist
		playerStatus.OnDamaged += OnPlayerDamaged;
	}

	void OnDisable()
	{
		playerStatus.OnDamaged -= OnPlayerDamaged;
	}


	// Event handlers
	private void OnPlayerDamaged()
	{
        RenderHearts(playerStatus.CurrentHealth);

        int index = Mathf.Clamp(playerStatus.CurrentHealth, 0, heartImages.Length - 1);

        heartImages[index].rectTransform.DOShakePosition(0.5f, 20f, 20, 90f);
    }

    private void RenderHearts(int currentHealth)
	{  
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (currentHealth > i)
             heartImages[i].sprite = heartFull;
            else 
             heartImages[i].sprite = heartEmpty;
        }
    }
}
