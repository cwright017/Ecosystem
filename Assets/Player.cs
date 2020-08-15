using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    NavMeshAgent agent;
    public float wanderTimer;
    public float wanderRadius;

    float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                Debug.Log("HIT: " + hit.transform.name);
                agent.destination = hit.point;
               
            }
        }

        timer += Time.deltaTime;

        if (timer > wanderTimer)
        {
            Vector3 dir = Random.insideUnitSphere * wanderRadius;
            dir += transform.position;

            NavMeshHit navHit;

            NavMesh.SamplePosition(dir, out navHit, wanderRadius, LayerMask.GetMask("Default"));

            Vector3 newPos = navHit.position;

            agent.SetDestination(newPos);
            timer = 0;
        }

    } 

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER: " + other.name);

        if(other.name == "Water")
        {
            //if (agent.pathStatus == NavMeshPathStatus.PathComplete)
            //{
                //Debug.Log("AT WATER");
                //agent.destination = agent.transform.position;
            //}
        }
    }
}