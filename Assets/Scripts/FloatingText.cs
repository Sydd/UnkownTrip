using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float floatHeight = 2f;
    [SerializeField] private float floatDuration = 1f;
    
    private List<TextMeshPro> textPool = new List<TextMeshPro>();
    private Camera mainCamera;
    private PlayerStatus playerStatus;
    [SerializeField]private PlayerAttack playerAttack;

    void Start()
    {
        mainCamera = Camera.main;
        playerStatus = PlayerStatus.Instance;
        if (playerAttack == null) playerAttack = GameObject.FindObjectOfType<PlayerAttack>();
        
        // Initialize pool
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewText();
        }
        
        // Subscribe to damage event
        if (playerStatus != null)
        {
            playerStatus.OnDamaged += HandleDamaged;
        }
        if (playerAttack != null)
        {
        //    playerAttack.enemyHurt += (Vector3 pos) => SpawnDamageText(10, pos);
        }
    }

    void OnDestroy()
    {
        if (playerStatus != null)
        {
            playerStatus.OnDamaged -= HandleDamaged;
        }
    }

    void Update()
    {
        // Make all active texts look at camera (billboard)
        foreach (var text in textPool)
        {
            if (text.gameObject.activeSelf && mainCamera != null)
            {
                text.transform.LookAt(mainCamera.transform);
                text.transform.Rotate(0, 180, 0);
            }
        }
    }

    private void HandleDamaged()
    {
      //  SpawnDamageText(10, playerStatus.transform.position);
    }

    private void SpawnDamageText(float damage, Vector3 position)
    {
        TextMeshPro text = GetAvailableText();
        
        if (text != null)
        {
            text.text = $"-{damage:F0}";
            text.color = Color.red;
            text.transform.position = position + Vector3.up;
            text.gameObject.SetActive(true);
            
            // Animate text going up and fading out
            Vector3 startPos = text.transform.position;
            Vector3 endPos = startPos + Vector3.up * floatHeight;
            
            LeanTween.cancel(text.gameObject);
            
            // Move up
            LeanTween.move(text.gameObject, endPos, floatDuration);
            
            // Fade out
            LeanTween.value(text.gameObject, 1f, 0f, floatDuration)
                .setOnUpdate((float alpha) =>
                {
                    Color color = text.color;
                    color.a = alpha;
                    text.color = color;
                })
                .setOnComplete(() =>
                {
                    text.gameObject.SetActive(false);
                });
        }
    }

    private TextMeshPro GetAvailableText()
    {
        // Find inactive text in pool
        foreach (var text in textPool)
        {
            if (!text.gameObject.activeSelf)
            {
                return text;
            }
        }
        
        // If no available text, create a new one
        return CreateNewText();
    }

    private TextMeshPro CreateNewText()
    {
        GameObject textObj;
        
        if (textPrefab != null)
        {
            textObj = Instantiate(textPrefab, transform);
        }
        else
        {
            // Create default TextMeshPro if no prefab is assigned
            textObj = new GameObject("FloatingText");
            textObj.transform.SetParent(transform);
            textObj.AddComponent<TextMeshPro>();
        }
        
        TextMeshPro textMesh = textObj.GetComponent<TextMeshPro>();
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontSize = 5;
        textObj.SetActive(false);
        
        textPool.Add(textMesh);
        return textMesh;
    }
}
