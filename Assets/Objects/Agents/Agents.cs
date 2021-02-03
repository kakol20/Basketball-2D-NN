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

    private NeuralNetwork neuralNetwork;

    private bool grounded = true;
    private float distanceToBasket = 0f;
    private Rigidbody2D rb;

    // shooting
    private GameObject currentBall;
    public bool IsFinished { get; private set; } = false;
    private bool HasShot = false;
    public void CreateNetwork()
    {
        neuralNetwork = new NeuralNetwork(layers);

        neuralNetwork.Randomise(0f, 1f);
    }

    public void Init(GameObject ballPrefab)
    {
        currentBall = Instantiate(ballPrefab, transform.position, transform.rotation, transform);
    }
    public void Move(float minX, float maxX, float basketX)
    {
        float newX = Own.Random.Range(minX, maxX);

        float distance = Mathf.Abs(basketX - newX);

        float maxDistance = Mathf.Max(Mathf.Abs(basketX - minX), Mathf.Abs(basketX - maxX));

        distanceToBasket = Own.Math.Map(newX, 0f, maxDistance, 0f, 1f);

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
    public void Shoot(float maxForce)
    {
        if (!HasShot)
        {
            currentBall.transform.position = transform.position;

            //float x = Own.Random.Range(0f, 1f) * maxForce;
            //float y = Own.Random.Range(0f, 1f) * maxForce;

            // add inputs
            List<float> input = new List<float>();
            input.Add(distanceToBasket);

            neuralNetwork.FeedForward(input.ToArray(), ActivationFunction.Sigmoid); // feed forward

            float x = neuralNetwork.Output[0] * maxForce;
            float y = neuralNetwork.Output[1] * maxForce;

            currentBall.GetComponent<Rigidbody2D>().AddForce(new Vector2(x, y));

            HasShot = true;
            IsFinished = false;
        }
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
            }
        }
    }
}