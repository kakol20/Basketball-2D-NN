using UnityEngine;

public class ManualAgents : MonoBehaviour
{
    [SerializeField] private LayerMask floorLayer;
    private bool grounded = true;
    [SerializeField] private float jumpForce = 320.0f;
    private Rigidbody2D rigidbody;
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float maxHorizontalVelocity = 1.34f;

    private void FixedUpdate()
    {
        ManualMove();
    }

    private void ManualMove()
    {
        //DebugGUI.LogPersistent("hAxis", "Horizontal Axis: " + Input.GetAxis("Horizontal").ToString("F2"));
        //DebugGUI.LogPersistent("vAxis", "Vertical Axis: " + Input.GetAxis("Vertical").ToString("F2"));

        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.5f) rigidbody.AddRelativeForce(new Vector2(1, 0) * speed * Mathf.Round(Input.GetAxis("Horizontal")));

        if (Input.GetAxis("Vertical") > 0 && grounded) rigidbody.AddRelativeForce(new Vector2(0, 1) * jumpForce);

        Vector2 oldVelocity = rigidbody.velocity;
        oldVelocity.y = 0;

        if (Mathf.Abs(oldVelocity.x) > maxHorizontalVelocity)
        {
            oldVelocity.Normalize();
            rigidbody.velocity = new Vector2(oldVelocity.x * maxHorizontalVelocity, rigidbody.velocity.y);
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
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}