using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;


public class Levitate : MonoBehaviour
{
    [Header("Movement")] [SerializeField] private float moveSpeed = 5f;

    [Header("Floating")] [SerializeField] private float hoverHeight = 2f;
    [SerializeField] private float frequency = 1.0f;
    [SerializeField] private float amplitude = 0.5f;
    [SerializeField] private float fallSpeed = 5f;

    [Header("Gravity")] [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float maxFallSpeed = 10f;

    [Header("Raycast detection")] [SerializeField]
    private float raycastLength = 5f;

    [SerializeField] private LayerMask groundLayer;

    // private Vector3 startPos;
    private Rigidbody rb;
    private float randomOffset;
    private float verticalVelocity = 0f;
    private float currentYVelocity;

    private bool wasGrounded = false;
    private float lastGroundY = 0f;

    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
        randomOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    private void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;
        Vector3 newPos = rb.position;

        if (moveDirection != Vector3.zero)
        {
            newPos += moveDirection * moveSpeed * Time.fixedDeltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 10f * Time.fixedDeltaTime));
        }

        bool grounded = Physics.Raycast(rb.position + Vector3.up, Vector3.down, out RaycastHit hit, raycastLength,
            groundLayer);

        if (grounded)
        {
            lastGroundY = hit.point.y;
            
            float groundY = lastGroundY + hoverHeight;
            float offsetY = Mathf.Sin(Time.time * frequency + randomOffset) * amplitude;
            float targetY = groundY + offsetY;

            if (!wasGrounded)
            {
                currentYVelocity = verticalVelocity;
            }

            newPos.y = Mathf.Lerp(rb.position.y, targetY, 10f * Time.fixedDeltaTime);
            verticalVelocity = 0f;
        }
        else
        {
            verticalVelocity -= gravity * Time.fixedDeltaTime;
            verticalVelocity = Mathf.Max(verticalVelocity, -maxFallSpeed);
            newPos.y -= fallSpeed * Time.fixedDeltaTime;
        }

        rb.MovePosition(newPos);
        wasGrounded = grounded;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up - Vector3.up * raycastLength);
    }
}