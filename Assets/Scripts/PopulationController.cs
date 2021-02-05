using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulationController : MonoBehaviour
{
    [SerializeField] private float basketX = 10f;

    [Header("Agents")]
    [SerializeField] private float maxForce = 10.0f;

    [SerializeField] private GameObject agentsPrefab;
    [SerializeField] private GameObject ballPrefab;

    private readonly float ySpawn = -3.7f;
    private List<GameObject> agentPopulation = new List<GameObject>();

    [Header("Spawn Area")]
    [SerializeField] private float maxX = 4f;

    [SerializeField] private float minX = -10f;
    [SerializeField] private int populationSize = 1;

    private float startX = 0f;

    [Header("Training")]
    [SerializeField] [Range(0, 1)] private float mutationRate = 0.1f;
    [SerializeField] private float incrementalPlacementLevel = 5f;
    [SerializeField] private float randomPlacementLevel = 25f;
    [SerializeField] private int seed = 1337;

    private int maxAttempts = 1;
    private int generation = 1;
    private int attempt = 1;

    private bool AllFinished()
    {
        foreach (GameObject item in agentPopulation)
        {
            if (!item.GetComponent<Agents>().IsFinished) return false;
        }

        return true;
    }

    private void CopyBest()
    {
        int half = agentPopulation.Count / 2;

        for (int i = 0; i < half; i++)
        {
            // ----- COPY BEST -----
            agentPopulation[i + half].GetComponent<Agents>().NN.CopyNetwork(agentPopulation[i].GetComponent<Agents>().NN);

            // ----- MUTATE -----
            float chance = Own.Random.Range();

            if (chance <= mutationRate)
            {
                agentPopulation[i + half].GetComponent<Agents>().NN.Mutate(-1f, 1f, mutationRate);
            }
        }
    }

    /// <summary>
    /// Detects if generation is finished and sorts the list
    /// </summary>
    /// <returns></returns>
    private bool GenFinished()
    {
        agentPopulation = agentPopulation.OrderByDescending(e => e.GetComponent<Agents>().Score).ToList();

        if (agentPopulation[0].GetComponent<Agents>().Attempts >= maxAttempts)
        {
            if (agentPopulation[0].GetComponent<Agents>().Score >= maxAttempts) maxAttempts++;

            DebugGUI.Graph("score", agentPopulation[0].GetComponent<Agents>().Score);

            generation++;

            DebugGUI.LogPersistent("generation", "Generation: " + generation.ToString("F0"));

            return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        Vector3 from = new Vector3(minX, 0f, 0f);
        Vector3 to = new Vector3(maxX, 0f, 0f);

        Gizmos.DrawLine(from, to);

        Gizmos.color = Color.green;
        //Gizmos.DrawIcon(new Vector3(basketX, 0f), "Basket X");
        Gizmos.DrawSphere(new Vector3(basketX, 0f), 0.25f);
    }

    private void SpawnAgents()
    {
        for (int i = 0; i < populationSize; i++)
        {
            float startX = (minX + maxX) / 2f;
            //float startX = maxX;
            Vector3 newPos = new Vector3(startX, ySpawn, 0f);
            agentPopulation.Add(Instantiate(agentsPrefab, newPos, transform.rotation, transform));

            agentPopulation[i].GetComponent<Agents>().CreateNetwork();
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        DebugGUI.LogPersistent("fps", "FPS: " + (1.0f / Time.deltaTime).ToString("F0"));
        DebugGUI.SetGraphProperties("score", "Score", 0, 0, 0, Color.red, true);

        Own.Random.Init(seed);

        startX = (minX + maxX) / 2f;

        SpawnAgents();

        foreach (GameObject item in agentPopulation)
        {
            item.GetComponent<Agents>().Init(ballPrefab, basketX, startX, minX, maxX);

            //item.GetComponent<Agents>().Move(minX, maxX, basketX);
            item.GetComponent<Agents>().Shoot(maxForce);
        }

        maxAttempts = 1;
        generation = 1;

        DebugGUI.LogPersistent("generation", "Generation: " + generation.ToString("F0"));
    }

    private void Shoot(GameObject agent)
    {
        agent.GetComponent<Agents>().Reset();

        if (maxAttempts > randomPlacementLevel)
        {
            agent.GetComponent<Agents>().RandomMove(minX, maxX, basketX);
        }
        else if (maxAttempts > incrementalPlacementLevel)
        {
            agent.GetComponent<Agents>().IncrementMove(minX, maxX, basketX);
        }

        agent.GetComponent<Agents>().Shoot(maxForce);
    }

    // Update is called once per frame
    private void Update()
    {
        DebugGUI.LogPersistent("fps", "FPS: " + (1.0f / Time.deltaTime).ToString("F0"));
        DebugGUI.LogPersistent("attempt", "Attempt: " + attempt.ToString());

        if (AllFinished())
        {
            if (GenFinished())
            {
                CopyBest();

                foreach (GameObject item in agentPopulation)
                {
                    item.GetComponent<Agents>().ResetGen();

                    Shoot(item);
                }

                attempt = 1;
            }
            else
            {
                foreach (GameObject item in agentPopulation)
                {
                    Shoot(item);
                }

                attempt++;
            }
        }
    }
}