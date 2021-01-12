using System.Collections.Generic;
using UnityEngine;

public class PopulationController : MonoBehaviour
{
    [Header("Spawn Area")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 4f;

    [Header("Agents")]
    [SerializeField] private float maxForce = 10.0f;
    [SerializeField] private GameObject agentsPrefab;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private int populationSize = 1;

    private float ySpawn = -3.7f;
    private List<GameObject> agentPopulation = new List<GameObject>();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        Vector3 from = new Vector3(minX, 0f, 0f);
        Vector3 to = new Vector3(maxX, 0f, 0f);

        Gizmos.DrawLine(from, to);
    }

    private void SpawnAgents()
    {
        for (int i = 0; i < populationSize; i++)
        {
            Vector3 newPos = new Vector3(Own.Random.Range(minX, maxX), ySpawn, 0f);
            agentPopulation.Add(Instantiate(agentsPrefab, newPos, transform.rotation));
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        DebugGUI.LogPersistent("fps", "FPS: " + (1.0f / Time.deltaTime).ToString("F0"));

        Own.Random.Init();

        SpawnAgents();

        foreach (GameObject item in agentPopulation)
        {
            item.GetComponent<Agents>().Init(ballPrefab);
            item.GetComponent<Agents>().Shoot(maxForce);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        DebugGUI.LogPersistent("fps", "FPS: " + (1.0f / Time.deltaTime).ToString("F0"));

        if (AllFinished())
        {
            foreach (GameObject item in agentPopulation)
            {
                item.GetComponent<Agents>().Reset();

                item.GetComponent<Agents>().Shoot(maxForce);
            }
        }
    }

    private bool AllFinished()
    {
        foreach (GameObject item in agentPopulation)
        {
            if (!item.GetComponent<Agents>().IsFinished) return false;
        }

        return true;
    }
}