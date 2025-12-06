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
	}

	private void RenderHearts(int currentHealth)
	{  
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (currentHealth -1 <= i)
             heartImages[i].sprite = heartFull;
            else 
             heartImages[i].sprite = heartEmpty;
        }
    }
}
