using UnityEngine;

public class ItemBoxRotator : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 100f * Time.deltaTime, 0f);
    }
}
