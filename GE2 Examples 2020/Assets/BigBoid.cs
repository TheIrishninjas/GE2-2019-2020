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
    public int i;

    public float speed = 0;
    public float dist;

    public bool seekEnabled = false;
    public Vector3 target;
    public Transform[] targetTransforms;

    public bool arriveEnabled = false;
    public bool controlEnabled = false;
    private bool arrived = false;
    public float slowingDistance = 10;
    public float changingDistance = 4.0f;

    [Range(0.0f, 0.2f)]
    public float banking = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        targetTransforms = GameObject.Find("Targets").GetComponentsInChildren<Transform>();
    }

    public Vector3 Control()
    {
        Vector3 f = Vector3.zero;
        f.x = Input.GetAxis("Horizontal");
        f.y = Input.GetAxis("Vertical");
        return f;
    }
    public void OnDrawGizmos()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(target, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + acceleration);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + velocity);

        Gizmos.color = Color.magenta;
        for (int i = 0; i < targetTransforms.Length; i++)
        {
            Gizmos.DrawWireSphere(targetTransforms[i].position, slowingDistance);
        }
    }

    Vector3 Arrive(Vector3 target)
    {
        Vector3 toTarget = target - transform.position;
        dist = toTarget.magnitude;

        float ramped = (dist / slowingDistance) * maxSpeed;
        float clamped = Mathf.Min(ramped, maxSpeed);
        Vector3 desired = (toTarget / dist) * clamped;

        return desired - velocity;
    }

    Vector3 Seek(Vector3 target)
    {
        Vector3 toTarget = target - transform.position;
        Vector3 desired = toTarget.normalized * maxSpeed;

        return desired - velocity;
    }

    public Vector3 CalculateForce()
    {
        Vector3 force = Vector3.zero;
        if (seekEnabled)
        {
            force += Seek(target);
        }
        if (arriveEnabled)
        {
            force += Arrive(target);
        }
        if(controlEnabled)
        {
            force += Control();
        }
        return force;
    }

    public void Switching()
    {
        if (dist <= changingDistance && !arrived)
        {
            arrived = true;
        }
        else
        {
            arrived = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        print(i);
        if (target != null)
        {
            target = targetTransforms[i].position;
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
            //transform.forward = velocity;
        }
        Switching();
        if(arrived)
        {
            i += 1;
            arrived = false;
        }
        else if(!arrived)
        {

        }
        if(i > (targetTransforms.Length - 1))
        {
            i = 0;
        }
    }
}
