using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 movementVector = new Vector3(5, 0, 0);
    [SerializeField] private float period = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Simple Sine wave movement
        float cycles = Time.time / period;
        const float tau = Mathf.PI * 2;
        float rawSinWave = Mathf.Sin(cycles * tau);

        Vector3 offset = movementVector * rawSinWave;
        transform.position = startPos + offset;
    }
}