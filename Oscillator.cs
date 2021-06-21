
using UnityEngine;
[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3 (10f, 10f, 10f);
    [SerializeField] float period = 2f;

    //remove from inspector later.
    float movementFactor; //0 for no movement, 1 for full movement.
    
    Vector3 startPos; 

    void Start()
    {   
        startPos = gameObject.transform.position;
    }


    void Update()
    {   //protect against NaN error
        if (period <= Mathf.Epsilon) { return; }
        float cycles = Time.time / period; //grows continually from 0

        const float tau = Mathf.PI * 2f; //about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau); //goes from -1 to +1
        movementFactor = rawSinWave / 2f + 0.5f; //omg math noooo OnO

        Vector3 displacement = movementVector * movementFactor;
        transform.position = startPos + displacement;
    }
}
