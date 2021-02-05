using System.Collections.Generic;
using UnityEngine;

public class Agents : MonoBehaviour
{
    [SerializeField] private float jumpForce = 320.0f;
    [SerializeField] private float maxHorizontalVelocity = 1.34f;
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private LayerMask floorLayer;

    [Header("Neural Network")]
    [SerializeField] private int[] layers = { 1, 3, 2 };

    //private NeuralNetwork NeuralNetwork;
    public NeuralNetwork NN { get; private set; }

    private bool grounded = true;
    private float distanceToBasket = 0f;
    private Rigidbody2D rb;

    // shooting
    private GameObject currentBall;

    private bool HasShot = false;

    public int Attempts { get; private set; }
    public bool IsFinished { get; private set; } = false;
    public int Score { get; private set; }

    public void CreateNetwork()
    {
        NN = new NeuralNetwork(layers);

        NN.Randomise(-1f, 1f);
    }

    /// <summary>
    /// Incrementally change X value by 1
    /// </summary>
    /// <param name="minX"></param>
    /// <param name="maxX"></param>
    /// <param name="basketX"></param>
    public void IncrementMove(float minX, float maxX, float basketX)
    {
        float newX = transform.position.x + 1;

        if (newX > maxX) newX = minX;

        CalculateDistanceToBasket(basketX, newX, minX, maxX);

        Vector3 newPos = transform.position;
        newPos.x = newX;

        transform.position = newPos;
    }

    public void Init(GameObject ballPrefab, float basketX, float startX, float minX, float maxX)
    {
        currentBall = Instantiate(ballPrefab, transform.position, transform.rotation, transform);

        // calculate distance
        CalculateDistanceToBasket(basketX, startX, minX, maxX);
    }

    /// <summary>
    /// Move at random X Values
    /// </summary>
    /// <param name="minX"></param>
    /// <param name="maxX"></param>
    /// <param name="basketX"></param>
    public void RandomMove(float minX, float maxX, float basketX)
    {
        float newX = Own.Random.Range(minX, maxX);

        CalculateDistanceToBasket(basketX, newX, minX, maxX);

        Vector3 newPos = transform.position;
        newPos.x = newX;

        transform.position = newPos;
    }

    public void Reset()
    {
        IsFinished = false;

        //Destroy(currentBall);
        currentBall.GetComponent<Ball>().Reset();
    }

    public void ResetGen()
    {
        Score = 0;
        Attempts = 0;
    }

    public void Shoot(float maxForce)
    {
        if (!HasShot)
        {
            currentBall.transform.position = transform.position;

            //float x = Own.Random.Range(0f, 1f) * maxForce;
            //float y = Own.Random.Range(0f, 1f) * maxForce;

            // add inputs
            List<float> input = new List<float>
            {
                distanceToBasket
            };

            NN.FeedForward(input.ToArray(), ActivationFunction.Tanh); // feed forward

            float x = Own.Math.Map(NN.Output[0], -1f, 1f, 0f, 1f, true) * maxForce;
            float y = Own.Math.Map(NN.Output[1], -1f, 1f, 0f, 1f, true) * maxForce;

            currentBall.GetComponent<Rigidbody2D>().AddForce(new Vector2(x, y));

            HasShot = true;
            IsFinished = false;
        }
    }

    private void CalculateDistanceToBasket(float basketX, float newX, float minX, float maxX)
    {
        float distance = Mathf.Abs(basketX - newX);

        float maxDistance = Mathf.Max(Mathf.Abs(basketX - minX), Mathf.Abs(basketX - maxX));

        distanceToBasket = Own.Math.Map(distance, 0f, maxDistance, -1f, 1f);
    }
    private void FixedUpdate()
    {
        ManualMove();
    }

    private void ManualMove()
    {
        //DebugGUI.LogPersistent("hAxis", "Horizontal Axis: " + Input.GetAxis("Horizontal").ToString("F2"));
        //DebugGUI.LogPersistent("vAxis", "Vertical Axis: " + Input.GetAxis("Vertical").ToString("F2"));

        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.5f) rb.AddRelativeForce(new Vector2(1, 0) * speed * Mathf.Round(Input.GetAxis("Horizontal")));

        if (Input.GetAxis("Vertical") > 0 && grounded) rb.AddRelativeForce(new Vector2(0, 1) * jumpForce);

        Vector2 oldVelocity = rb.velocity;
        oldVelocity.y = 0;

        if (Mathf.Abs(oldVelocity.x) > maxHorizontalVelocity)
        {
            oldVelocity.Normalize();
            rb.velocity = new Vector2(oldVelocity.x * maxHorizontalVelocity, rb.velocity.y);
        }
    }

    //private void MoveToMouse()
    //{
    //    Vector3 newPos = Input.mousePosition;

    //    newPos.z = Camera.main.nearClipPlane;
    //    newPos = Camera.main.ScreenToWorldPoint(newPos);

    //    transform.position = newPos;
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == Mathf.Log(floorLayer.value, 2)) grounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == Mathf.Log(floorLayer.value, 2)) grounded = false;
    }

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (HasShot)
        {
            if (currentBall.GetComponent<Ball>().HitFloor)
            {
                // reset

                IsFinished = true;
                HasShot = false;

                Attempts++;

                if (currentBall.GetComponent<Ball>().HitTarget) Score++;
            }
        }
    }
}