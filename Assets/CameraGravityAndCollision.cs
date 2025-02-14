using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class CameraGravityAndCollision : MonoBehaviour
{
    [Header("Gravity & Movement")]
    public float gravity = 9.81f;          // Gravity value
    public float climbSpeed = 7f;          // Speed for smoothing vertical movement
    public LayerMask collisionMask;        // For ground ray
    public float groundRayLength = 0.3f;   // Extra distance to check below the capsule

    [Header("Camera Shake")]
    public float shakeDuration = 0.2f;     // Duration
    public float shakeMagnitude = 0.05f;   // Magnitude

    [Header("Navigator / Terrain Follow")]
    public bool useNavigatorTerrainFollow = true;

    private CapsuleCollider capsuleCollider;
    private Rigidbody rb;
    private bool isGrounded;

    void Awake()
    {
        // Ensure a CapsuleCollider is present
        capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.height = 1.8f;
        capsuleCollider.radius = 0.3f;
        capsuleCollider.center = new Vector3(0, 0.9f, 0);

        // Ensure a Rigidbody is present for OnCollisionEnter to work
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;   // We'll apply gravity manually
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        // If the Navigator is not handling terrain, do custom gravity
        if (!useNavigatorTerrainFollow)
        {
            HandleGravity();
        }

        // (Optional) If you want to move horizontally, you can read Input here
        // and do e.g. rb.velocity = new Vector3(moveX, rb.velocity.y, moveZ);
    }

    private void HandleGravity()
    {
        // Cast a ray from slightly above the bottom of the capsule downwards
        Vector3 rayStart = transform.position + Vector3.down * (capsuleCollider.height * 0.5f - 0.05f);
        Ray downRay = new Ray(rayStart, Vector3.down);

        if (Physics.Raycast(downRay, out RaycastHit hit, groundRayLength, collisionMask))
        {
            isGrounded = true;

            // Smoothly move onto the ground
            float desiredY = hit.point.y + capsuleCollider.height * 0.5f;
            Vector3 targetPos = new Vector3(transform.position.x, desiredY, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * climbSpeed);

            // Zero out vertical velocity
            Vector3 vel = rb.velocity;
            vel.y = 0f;
            rb.velocity = vel;
        }
        else
        {
            // Apply downward gravity
            isGrounded = false;
            rb.velocity += Vector3.down * gravity * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Basic collision => camera shake
        StartCoroutine(CameraShake(shakeDuration, shakeMagnitude));
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(
                originalPos.x + offsetX,
                originalPos.y + offsetY,
                originalPos.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return camera to original local position
        transform.localPosition = originalPos;
    }
}
