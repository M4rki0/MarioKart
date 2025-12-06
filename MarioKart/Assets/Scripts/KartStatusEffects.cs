using UnityEngine;

public class KartStatusEffects : MonoBehaviour
{
    public float spinDuration = 1.5f;
    public float knockbackForce = 800f;
    
    private bool isStunned = false;
    private float spinTimer = 0f;
    private Rigidbody rb;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStunned)
        {
            spinTimer += Time.deltaTime;
            if (spinTimer >= spinDuration)
            {
                isStunned = false;
                spinTimer = 0f;
            }
        }
    }

    public bool IsStunned()
    {
        return isStunned;
    }

    public void SpinOut()
    {
        if (isStunned) return;

        isStunned = true;
        spinTimer = 0f;

        StartCoroutine(SpinEffect());
    }

    public void KnockBack(Vector3 sourcePosition)
    {
        Vector3 direction = (transform.position - sourcePosition).normalized;
        rb.AddForce(direction * knockbackForce);
        SpinOut();
    }

    private System.Collections.IEnumerator SpinEffect()
    {
        float time = 0f;
        while (time < spinDuration)
        {
            transform.Rotate(0f, 720f * Time.deltaTime, 0f);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
