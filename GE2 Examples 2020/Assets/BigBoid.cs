using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBoid : MonoBehaviour
{
    public Vector3 velocity = Vector3.zero;
    public Vector3 acceleration = Vector3.zero;
    public Vector3 force = Vector3.zero;

    public float mass = 1.0f;

    public float maxSpeed = 5;
    public float maxForce = 10;
    public float slowingDist = 1.0f;
    [Range(0.1f,1.0f)] [SerializeField] private float banking;

    public float speed = 0;
    public float playerForce = 100;
    public float damping = 1;

    public bool seekEnabled = false;
    public bool arriveEnabled = false;
    public bool steeringEnabled = false;
    public Vector3 target;
    public Transform targetTransform;
    public Vector3 PlayerSteering()
    {
        Vector3 f = Vector3.zero;
        f += Input.GetAxis("Vertical") * transform.forward * playerForce;
        Vector3 projectedRight = transform.right;
        projectedRight.y = 0;
        projectedRight.x = f.x;
        return f;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(target, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + force);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + velocity);
    }

    Vector3 Seek(Vector3 target)
    {
        Vector3 toTarget = target - transform.position;
        Vector3 desired = toTarget.normalized * maxSpeed;

        return desired - velocity;
    }

    Vector3 Arrive(Vector3 target)
    {
        Vector3 toTarget = target - transform.position;
        float dist = toTarget.magnitude;
        float ramped = dist / slowingDist * maxSpeed;
        float clamped = Mathf.Min(ramped, maxSpeed);
        Vector3 desired = (toTarget / dist) * clamped;
        return desired;
    }

    public Vector3 CalculateForce()
    {
        Vector3 force = Vector3.zero;
        if (seekEnabled)
        { 
               force += Seek(target);   
        }
        return force;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetTransform != null)
        {
            target = targetTransform.position;
        }
        if(arriveEnabled)
        {
            force += Arrive(target);
        }
        if(steeringEnabled)
        {
            force += PlayerSteering();
        }
        force = CalculateForce();
        acceleration = force / mass;
        velocity += acceleration * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
        speed = velocity.magnitude;
        if (speed > 0)
        {
            Vector3 tempUp = Vector3.Lerp(transform.up, Vector3.up + (acceleration * banking), Time.deltaTime * 3.0f);
            transform.LookAt(transform.position + velocity, tempUp);
            velocity -= (damping * velocity * Time.deltaTime);
        }
    }
}
