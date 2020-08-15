using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    NavMeshAgent agent;

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

        Vector3 pos = agent.transform.position;
        Debug.Log("TILE: " + pos);
        if (TerrainData.IsShoreTile((int)pos.x, (int)pos.z))
        {
            Debug.Log("AT SHORE");
            agent.destination = pos;
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