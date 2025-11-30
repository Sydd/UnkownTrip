using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;
    public int sortingOffset = 0;   // use this if you want manual fine tuning
    public float precision = 100f;  // how accurate the sorting is
    public float maxYRotation = 45f; // max rotation angle in degrees
    
    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            Vector3 directionToCamera = mainCamera.transform.position - transform.position;
            directionToCamera.y = 0; // Keep it flat on the Y axis, no X rotation
            
            if (directionToCamera != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera);
                Vector3 euler = targetRotation.eulerAngles;
                
                // Clamp the Y rotation
                float yAngle = euler.y;
                if (yAngle > 180f) yAngle -= 360f; // Convert to -180 to 180 range
                yAngle = Mathf.Clamp(yAngle, -maxYRotation, maxYRotation);
                
                euler.y = yAngle;
                transform.rotation = Quaternion.Euler(euler);
            }
        }
        // Sorting based on world Z position
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = (int)(-transform.position.z * precision) + sortingOffset;
        }
    }
}