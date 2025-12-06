using UnityEngine;

public class Banana : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Banana triggered with: " + other.name);
        var kart = other.GetComponent<KartStatusEffects>();
        if (kart == null)
        {
            Debug.Log("No KartStatusEffects found on " + other.name);
        }
        else
        {
            
        }
        {
            Debug.Log("Spinning out kart: " + other.name);
            kart.SpinOut();
            Destroy(gameObject);
        }
    }
}
