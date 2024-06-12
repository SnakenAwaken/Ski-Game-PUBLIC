using UnityEngine;
using System.Collections;
using System;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxHorizontalSpeed = 10f;
    public float boostSpeed = 10f;
    public float boostCooldown = 2f;
    public CameraShaker camShake;
    public Transform raycastOrigin; // GameObject used as the origin of the raycast
    public AudioSource collisionSound; // Reference to the AudioSource component

    private Rigidbody rb;
    private float sMov;
    private bool canBoost = true;
    private bool CanMove;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sMov = moveSpeed;
    }

    void FixedUpdate()
    {
        if (CanMove)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            Vector3 movement = new Vector3(horizontalInput, 0f, 0f).normalized;

            moveSpeed = sMov * -rb.velocity.z;

            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, 0f);
            Vector3 desiredHorizontalVelocity = horizontalVelocity + movement * moveSpeed;

            desiredHorizontalVelocity.x = Mathf.Clamp(desiredHorizontalVelocity.x, -maxHorizontalSpeed * -rb.velocity.z, maxHorizontalSpeed * -rb.velocity.z);

            rb.velocity = new Vector3(desiredHorizontalVelocity.x, rb.velocity.y, rb.velocity.z);

            if (rb.velocity != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(rb.velocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }

            if (Input.GetKeyDown(KeyCode.Space) && canBoost)
            {
                rb.velocity += new Vector3(0, 0, boostSpeed);
                StartCoroutine(BoostCooldown());
            }
        }

        // Perform raycast downwards
        RaycastHit hit;
        int layerMask = ~LayerMask.GetMask("Player"); // Ignore objects with "Player" tag
        if (Physics.Raycast(raycastOrigin.position, Vector3.down, out hit, 1f, layerMask))
        {
            CanMove = true; // If raycast hits something, enable movement
        }
        else
        {
            CanMove = false; // If raycast doesn't hit anything, disable movement
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ramp"))
        {
            rb.velocity += new Vector3(0, 0, boostSpeed * 1.2f);
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            rb.velocity += new Vector3(0, 12, -boostSpeed);
            Destroy(collision.gameObject);
            camShake.ShakeCamera(5f, 15f);
            if (collisionSound != null)
            {
                collisionSound.Play(); // Play the collision sound
            }
        }
    }

    IEnumerator BoostCooldown()
    {
        canBoost = false;
        yield return new WaitForSeconds(boostCooldown);
        canBoost = true;
    }
}
