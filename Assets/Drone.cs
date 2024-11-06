using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;

[RequireComponent(typeof(Collider2D))]
public class Drone : MonoBehaviour
{
    public int Code { get; set; }
    public int Temperature { set; get; } = 0;
    public bool Destroyed { get; set; }

    Collider2D agentCollider;
    public Collider2D AgentCollider { get { return agentCollider; } }

    public float smoothTime = 0.3F;
    private Vector2 velocity = Vector2.zero;


    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider2D>();
        Code = Random.Range(1000, 9999);
    }

    public void Destroy()
    {
        Destroyed = true;
        gameObject.SetActive(false);
    }
    private void Update()
    {
        Temperature = (int)(Random.value * 100);
    }

    public void Move(Vector2 velocity)
    {
        transform.up = velocity;
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    public Vector2 CalcMove(List<Transform> context, Vector2 center, float socialradiusSq, float radiusSq)
    {
        Vector2 alignMotion = Align(context); //Align is for alignment
        Vector2 avoidMotion = AvoidOtherDrones(context, radiusSq); //AvoidOtherDrones is for avoidance
        Vector2 cohesiveMotion = EnsureCohesion(context); //EnsureCohesion is for alignment
        Vector2 radiusMotion = StayInRadius(center, radiusSq); //StayInRadius is for avoidance

        alignMotion.Normalize();
        avoidMotion.Normalize();
        cohesiveMotion.Normalize();
        radiusMotion.Normalize();

        Vector2 currentVelocity = Vector2.one;

        Vector2 motion = alignMotion * 0.2f + avoidMotion * 0.3f +
                         cohesiveMotion * 0.3f + radiusMotion * 0.2f;

        transform.up = Vector2.SmoothDamp(transform.up, motion, ref velocity, smoothTime);
        return motion;
    }

    private Vector2 Align(List<Transform> context)
    {
        //if no neighbors, maintain current alignment
        if (context.Count == 0)
            return transform.up;

        //add all points together and average
        Vector2 alignmentMove = Vector2.zero;
        foreach (Transform item in context)
        {
            alignmentMove += (Vector2)item.transform.up;
        }
        alignmentMove /= context.Count;

        return alignmentMove;
    }

    private Vector2 AvoidOtherDrones(List<Transform> context, float radiusSq)
    {
        //add all points together and average
        Vector2 avoidanceMove = Vector2.zero;
        int nAvoid = 0;
        foreach (Transform item in context)
        {
            if (Vector2.SqrMagnitude(item.position - transform.position) < radiusSq)
            {
                nAvoid++;
                avoidanceMove += (Vector2)(transform.position - item.position);
            }
        }
        if (nAvoid > 0)
            avoidanceMove /= nAvoid;

        return avoidanceMove;
    }

    private Vector2 EnsureCohesion(List<Transform> context)
    {
        //add all points together and average
        Vector2 cohesionMove = Vector2.zero;
        foreach (Transform item in context)
        {
            cohesionMove += (Vector2)item.position;
        }
        cohesionMove /= context.Count;

        //create offset from agent position
        cohesionMove -= (Vector2)transform.position;
        return cohesionMove;
    }

    private Vector2 StayInRadius(Vector2 center, float radius)
    {
        Vector2 centerOffset = center - (Vector2)transform.position;
        float t = centerOffset.magnitude / radius;
        if (t < 0.9f)
        {
            return Vector2.zero;
        }

        return centerOffset * t * t;
    }
}
