using UnityEngine;
using Cinemachine;

public class CameraShaker : MonoBehaviour
{
    // Reference to the Cinemachine Virtual Camera
    public CinemachineVirtualCamera virtualCamera;

    // Shake duration and intensity
    private float shakeTimer;
    private float shakeIntensity;

    // Smoothly decrease shake intensity
    private float shakeDamping = 2f;

    // Shake the camera with given duration and intensity
    public void ShakeCamera(float duration, float intensity)
    {
        // Set shake parameters
        shakeTimer = duration;
        shakeIntensity = intensity;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            // Reduce the shake timer
            shakeTimer -= Time.deltaTime;

            // Generate random noise for camera shake
            Vector3 shakeAmount = Random.insideUnitSphere * shakeIntensity;

            // Apply the shake to the camera's noise profile
            CinemachineBasicMultiChannelPerlin noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            noise.m_AmplitudeGain = shakeAmount.magnitude;

            // Smoothly decrease shake intensity over time
            shakeIntensity = Mathf.Lerp(shakeIntensity, 0f, shakeDamping * Time.deltaTime);

            if (shakeTimer <= 0)
            {
                // Reset camera shake
                noise.m_AmplitudeGain = 0f;
            }
        }
    }
}
