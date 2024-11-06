using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DroneSwarm : MonoBehaviour
{
    public Drone dronePrefab;
    List<Drone> drones = new List<Drone>();


    [Range(10, 5000)]
    public int startingCount = 250;
    const float DroneDensity = 0.08f;

    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float maxSpeed = 5f;
    [Range(1f, 10f)]
    public float neighborRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    [Range(1f, 10f)]
    public float bulletRadius = 0.2f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;

    // Start is called before the first frame update
    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        for (int i = 0; i < startingCount; i++)
        {
            Drone newDrone = Instantiate(
                dronePrefab,
                UnityEngine.Random.insideUnitCircle * startingCount * DroneDensity,
                Quaternion.Euler(Vector3.forward * UnityEngine.Random.Range(0f, 360f)),
                transform
                );
            newDrone.name = "Drone " + i;
            drones.Add(newDrone);
        }
    }


    // Update is called once per frame
    void Update()
    {
        foreach (Drone drone in this.drones)
        {
            // decide on next movement direction
            List<Transform> context = GetNearbyObjects(drone);
            Vector2 move = drone.CalcMove(context, Vector2.zero, squareAvoidanceRadius, 10);  //behavior.CalculateMove(drone, context, this);
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }
            drone.Move(move);
        }

        ShootDrones();

        CleanUpDrones();
    }

    void ShootDrones()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Bullet thebullet = bullet.GetComponent<Bullet>();
            List<Transform> destroyed = GetHitByBullet(thebullet);

            foreach (Transform drone in destroyed)
            {
                GameObject go = drone.gameObject;
                Drone droneDestroyed = go.GetComponent<Drone>();
                droneDestroyed.Destroy();
            }
        }
    }

    List<Transform> GetNearbyObjects(Drone drone)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(drone.transform.position, neighborRadius);
        foreach (Collider2D c in contextColliders)
        {
            if (c != drone.AgentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }

    void CleanUpDrones()
    {
        List<Drone> whatremains = new List<Drone>();
        foreach (Drone drone in drones)
        {
            if (drone.Destroyed == true)
                Destroy(drone.gameObject);
            else
                whatremains.Add(drone);
        }
        drones = whatremains;
    }

    List<Transform> GetHitByBullet(Bullet bullet)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(bullet.transform.position,
                                                                bulletRadius);

        Debug.Log("Number of nearby drones = " + contextColliders.Length);
        foreach (Collider2D c in contextColliders)
        {
            if (c.tag == bullet.tag)
                continue;

            context.Add(c.transform);

            GameObject gobj = c.transform.gameObject;
        }
        return context;
    }

}