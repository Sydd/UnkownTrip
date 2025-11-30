using UnityEngine;

public class CameraFollow2_5D : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offsets")]
    public Vector3 offset = new Vector3(0, 5, -10);    // posición base
   // public float zLock = -10f;                         // mantiene la cámara fija en Z para 2.5D

    [Header("Smooth Follow")]
    public float followSpeed = 5f;                     // qué tan rápido sigue la cámara
    public float deadZoneRadius = 0.5f;                // zona donde no se mueve la cámara

    [Header("Look Ahead")]
    public float lookAheadDistance = 2f;               // cuánto mira hacia adelante
    public float lookAheadSmooth = 5f;

    [Header("Clamp (Opcional)")]
    public bool clampEnabled = false;
    public Vector3 minBounds;
    public Vector3 maxBounds;

    private Vector3 currentLookAhead;
    private Vector3 targetLastPos;

    void Start()
    {
        if(target != null)
            targetLastPos = target.position;
    }

    void LateUpdate()
    {
        if(target == null) return;

        // ------------------------------
        // LOOK AHEAD dinámico
        // ------------------------------
        Vector3 targetMovement = target.position - targetLastPos;

        Vector3 lookAhead = new Vector3(
            Mathf.Sign(targetMovement.x) * lookAheadDistance,
            0f,
            0f
        );

        currentLookAhead = Vector3.Lerp(
            currentLookAhead,
            lookAhead,
            Time.deltaTime * lookAheadSmooth
        );

        targetLastPos = target.position;

        // ------------------------------
        // DEAD ZONE
        // ------------------------------
        Vector3 targetPos = target.position + offset + currentLookAhead;
        //targetPos.z = zLock; // aseguramos cámara fija en Z (ideal 2.5D)

        Vector3 diff = targetPos - transform.position;

        if(diff.magnitude < deadZoneRadius)
        {
            // el personaje está dentro del círculo, no mover la cámara
            return;
        }

        // ------------------------------
        // FOLLOW SUAVE
        // ------------------------------
        Vector3 desired = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * followSpeed
        );

        // ------------------------------
        // CLAMP opcional
        // ------------------------------
        if(clampEnabled)
        {
            desired.x = Mathf.Clamp(desired.x, minBounds.x, maxBounds.x);
            desired.y = Mathf.Clamp(desired.y, minBounds.y, maxBounds.y);
            desired.z = Mathf.Clamp(desired.z, minBounds.z, maxBounds.z);
        }

        transform.position = desired;
    }

    void OnDrawGizmosSelected()
    {
        // dibuja zona muerta
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, deadZoneRadius);
    }
}
