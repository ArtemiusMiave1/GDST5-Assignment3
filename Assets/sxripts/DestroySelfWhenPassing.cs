using UnityEngine;

public class DestroySelfWhenPassing : MonoBehaviour
{
    public float destroyZ = -50f;

    void Update()
    {
        if (transform.position.z < destroyZ)
        {
            Destroy(gameObject);
        }
    }
}