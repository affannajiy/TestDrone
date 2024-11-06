using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    internal Vector3 dir = Vector2.up;
    internal float Speed;
    [Range(2f, 10f)]
    public float DestroyTime = 2f;

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(DestroyTime);

        Destroy(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        if (DestroyTime > 0)
        {
            StartCoroutine(SelfDestruct());
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += dir * Speed * Time.deltaTime;
    }
}