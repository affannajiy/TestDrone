using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shuttle : MonoBehaviour
{
    public GameObject Bullet;
    public Vector3 dir;
    [Range(1f, 3f)] public float speed;
    [Range(5f, 15f)]
    public float rotDelta = 5;

    // Start is called before the first frame update
    void Start()
    {
        dir = Vector3.down;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += dir * speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            // Quaternion.AngleAxis(angle, axis) * start;

            dir = Quaternion.AngleAxis(rotDelta, Vector3.forward) * dir;
            transform.up = dir;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position += -dir * speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            dir = Quaternion.AngleAxis(-rotDelta, Vector3.forward) * dir;
            transform.up = dir;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            
            var bullet = Instantiate(Bullet, transform.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().dir = dir;
            bullet.GetComponent<Bullet>().Speed = speed * 2;
            
        }

    }
}
