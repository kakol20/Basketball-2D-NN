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
    private List<GameObject> agentPopulation = new();
    private Agents bestAgent;

    [Header("Spawn Area")]
    [SerializeField] private float maxX = 4f;
    [SerializeField] private float minX = -10f;
    [SerializeField] private int populationSize = 1;

    private float startX = 0f;

    [Header("Training")]
    [SerializeField][Range(0, 1)] private float mutationRate = 0.1f;
    [SerializeField] private float incrementalPlacementLevel = 5f;
    [SerializeField] private float randomPlacementLevel = 25f;
    //[SerializeField] private int seed = 1337;

    private int maxAttempts = 1;
    private int generation = 1;
    private int attempt = 1;

    /// <summary>
    /// Checks if all agents finish shooting
    /// </summary>
    /// <returns></returns>
    private bool AllFinished()
    {
        foreach (GameObject item in agentPopulation)
        {
            if (!item.GetComponent<Agents>().IsFinished) return false;
        }

        return true;
    }

    /// <summary>
    /// Copy best top half
    /// </summary>
    private void CopyBest()
    {
        int half = agentPopulation.Count / 2;

        for (int i = 0; i < half; i++)
        {
            Agents iHalf = agentPopulation[i + half].GetComponent<Agents>();

            // ----- COPY BEST -----
            iHalf.NN.CopyNetwork(agentPopulation[i].GetComponent<Agents>().NN);

            // ----- MUTATE -----
            iHalf.GetComponent<Agents>().NN.Mutate(-1f, 1f, mutationRate);
        }
    }

    /// <summary>
    /// Copy outlier if the best is an outlier to worst agents
    /// </summary>
    /// <returns></returns>
    private bool CopyOutlier()
    {
        float limit = (populationSize / 2f) - 1f;

        // ----- QUARTILES -----
        float Q3 = GetScore((1 / 3f) * limit);
        float Q1 = GetScore(limit);

        float IQR = Q3 - Q1;

        // ----- CHECK OUTLIER -----
        float outlier = Q3 + 1.5f * IQR;

        if (GetScore(0) > outlier)
        {
            // ----- COPY OUTLIER TO WORST AGENTS -----
            for (int i = agentPopulation.Count / 2; i < agentPopulation.Count; i++)
            {
                Agents iAgent = agentPopulation[i].GetComponent<Agents>();
                iAgent.NN.CopyNetwork(bestAgent.NN);

                iAgent.NN.Mutate(-1f, 1f, mutationRate);
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Detects if generation is finished and sorts the list
    /// </summary>
    /// <returns></returns>
    private bool GenFinished()
    {
        agentPopulation = agentPopulation.OrderByDescending(e => e.GetComponent<Agents>().Score).ToList();

        bestAgent = agentPopulation.First().GetComponent<Agents>();

        if (bestAgent.Attempts >= maxAttempts)
        {
            if (GetScore(0) >= maxAttempts) maxAttempts++;

            DebugGUI.Graph("score", GetScore(0));

            generation++;

            DebugGUI.LogPersistent("generation", "Generation: " + generation.ToString("F0"));
            DebugGUI.LogPersistent("bestScore", "Last Score: " + bestAgent.Score.ToString("F2"));

            return true;
        }

        return false;
    }

    /// <summary>
    /// Get floating point of score based on float index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private float GetScore(float index)
    {
        if (index % 1 != 0)
        {
            int tmp1 = Mathf.FloorToInt(index);
            int tmp2 = Mathf.CeilToInt(index);

            return (agentPopulation[tmp1].GetComponent<Agents>().Score + (float)agentPopulation[tmp2].GetComponent<Agents>().Score) / 2f;
        }
        else
        {
            return agentPopulation[(int)index].GetComponent<Agents>().Score;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        Vector3 from = new(minX, 0f, 0f);
        Vector3 to = new(maxX, 0f, 0f);

        Gizmos.DrawLine(from, to);

        Gizmos.color = Color.green;
        //Gizmos.DrawIcon(new Vector3(basketX, 0f), "Basket X");
        Gizmos.DrawSphere(new Vector3(basketX, 0f), 0.25f);
    }

    private void Shoot(Agents agent)
    {
        agent.Reset();

        if (maxAttempts > randomPlacementLevel)
        {
            agent.RandomMove(minX, maxX, basketX);
        }
        else if (maxAttempts > incrementalPlacementLevel)
        {
            agent.IncrementMove(minX, maxX, basketX);
        }

        agent.Shoot(maxForce);
    }

    private void SpawnAgents()
    {
        for (int i = 0; i < populationSize; i++)
        {
            float startX = (minX + maxX) / 2f;
            //float startX = maxX;
            Vector3 newPos = new(startX, ySpawn, 0f);
            agentPopulation.Add(Instantiate(agentsPrefab, newPos, transform.rotation, transform));

            agentPopulation[i].GetComponent<Agents>().CreateNetwork();
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        DebugGUI.LogPersistent("fps", "FPS: " + (1.0f / Time.deltaTime).ToString("F0"));
        DebugGUI.SetGraphProperties("score", "Score", 0, 0, 0, Color.red, true);

        Own.Random.Init(StaticManager.Seed);

        startX = (minX + maxX) / 2f;

        SpawnAgents();

        foreach (GameObject item in agentPopulation)
        {
            Agents l_agent = item.GetComponent<Agents>();
            l_agent.Init(ballPrefab, basketX, startX, minX, maxX);

            //item.GetComponent<Agents>().Move(minX, maxX, basketX);
            l_agent.Shoot(maxForce);
        }

        maxAttempts = 1;
        generation = 1;

        DebugGUI.LogPersistent("generation", "Generation: " + generation.ToString("F0"));
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
                if (!CopyOutlier()) CopyBest();

                foreach (GameObject item in agentPopulation)
                {
                    Agents agent = item.GetComponent<Agents>();
                    agent.ResetGen();

                    Shoot(agent);
                }

                attempt = 1;
            }
            else
            {
                foreach (GameObject item in agentPopulation)
                {
                    Shoot(item.GetComponent<Agents>());
                }

                attempt++;
            }
        }
    }
}