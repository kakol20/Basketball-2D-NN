using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float maxWait = 5.0f;
    [SerializeField] private LayerMask exitTargetLayer;
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask topTargetLayer;

    private bool hitEntryTrigger = false;
    private bool hitExitTrigger = false;
    private bool hitTopTrigger = false;
    private float timeElapsed = 0f;
    private Rigidbody2D rb;

    public bool HitFloor { get; private set; }
    public bool HitTarget { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
        HitFloor = false;
        HitTarget = false;

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        //if (HitTarget)
        //{
        //    DebugGUI.LogPersistent("hitTarget", "Ball has hit target");
        //}
        //else
        //{
        //    DebugGUI.LogPersistent("hitTarget", "Ball has hit not target");
        //}
        timeElapsed += Time.deltaTime;

        if (rb.velocity.sqrMagnitude == 0 && rb.angularVelocity == 0 && timeElapsed >= maxWait) HitFloor = true;

        if (hitEntryTrigger && hitExitTrigger && hitTopTrigger) HitTarget = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == Mathf.Log(floorLayer.value, 2))
        {
            //DebugGUI.LogPersistent("hitFloor", "Ball has hit floor"); // for testing only

            HitFloor = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == Mathf.Log(targetLayer.value, 2) && !HitTarget)
        {
            hitEntryTrigger = true;
        }

        if (collision.gameObject.layer == Mathf.Log(exitTargetLayer.value, 2) && !HitTarget)
        {
            hitExitTrigger = true;
        }

        if (collision.gameObject.layer == Mathf.Log(topTargetLayer.value, 2) && HitTarget)
        {
            hitTopTrigger = true;
        }
    }

    public void Reset()
    {
        HitFloor = false;
        HitTarget = false;

        hitEntryTrigger = false;
        hitExitTrigger = false;
        hitTopTrigger = false;

        rb.angularVelocity = 0f;
        rb.velocity = Vector2.zero;

        timeElapsed = 0f;
    }
}