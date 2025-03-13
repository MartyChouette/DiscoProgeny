using UnityEngine;

public class Drunkeness : MonoBehaviour
{
    public float swaySpeed = 2f; 
    public int swayAmount = 0; 

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;

        transform.position = initialPosition + new Vector3(sway, 0, 0); 
    }
}
