using DG.Tweening;
using UnityEngine;

public class Props : MonoBehaviour
{
    public bool _isSolid;

    [Header("Wobble Settings")]
    [SerializeField] private float wobbleAmount = 0.2f;
    [SerializeField] private float wobbleDuration = 0.3f;
    [SerializeField] private int wobbleVibrato = 10;
    [SerializeField] private float wobbleElasticity = 1f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }


    public void OnTriggerEnter(Collider other)
    {
        if (!_isSolid && (other.gameObject.tag == "Player" || other.gameObject.tag == "enemy")) 
        {
            PlayWobble();
            Debug.Log("asd");
        }
    }

    public void PlayWobble()
    {
        // Make sure scale is reset
        transform.localScale = originalScale;
        GetComponent<Transform>().
        transform.DOPunchScale(
            new Vector3(wobbleAmount, wobbleAmount, wobbleAmount),
            wobbleDuration,
            wobbleVibrato,
            wobbleElasticity
        );
    }
    }
