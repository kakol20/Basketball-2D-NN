using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float maxWait = 5.0f;
    [SerializeField] private LayerMask exitTargetLayer;
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask midTargetLayer;
    [SerializeField] private LayerMask topTargetLayer;

    //private bool hitEntryTrigger = false;
    //private bool hitExitTrigger = false;
    //private bool hitTopTrigger = false;
    private float timeElapsed = 0f;
    private List<Target> hitOrder = new List<Target>();
    private Rigidbody2D rb;

    private enum Target
    {
        Top, Mid, Exit
    }

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

        //if (hitEntryTrigger && hitExitTrigger && hitTopTrigger) HitTarget = true;

        if (hitOrder.Count >= 3)
        {
            if (hitOrder[0] == Target.Top && hitOrder[1] == Target.Mid && hitOrder[2] == Target.Exit) HitTarget = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == Mathf.Log(floorLayer.value, 2) && !HitFloor)
        {
            //DebugGUI.LogPersistent("hitFloor", "Ball has hit floor"); // for testing only

            HitFloor = true;

            rb.velocity /= 2f;
            rb.angularVelocity /= 2f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.gameObject.layer == Mathf.Log(targetLayer.value, 2) && !HitTarget)
        //{
        //    hitEntryTrigger = true;
        //}

        //if (collision.gameObject.layer == Mathf.Log(exitTargetLayer.value, 2) && !HitTarget)
        //{
        //    hitExitTrigger = true;
        //}

        //if (collision.gameObject.layer == Mathf.Log(topTargetLayer.value, 2) && HitTarget)
        //{
        //    hitTopTrigger = true;
        //}

        if (!HitFloor)
        {
            if (collision.gameObject.layer == Mathf.Log(topTargetLayer.value, 2)) hitOrder.Add(Target.Top);
            if (collision.gameObject.layer == Mathf.Log(midTargetLayer.value, 2)) hitOrder.Add(Target.Mid);
            if (collision.gameObject.layer == Mathf.Log(exitTargetLayer.value, 2)) hitOrder.Add(Target.Exit);
        }
    }

    public void Reset()
    {
        HitFloor = false;
        HitTarget = false;

        //hitEntryTrigger = false;
        //hitExitTrigger = false;
        //hitTopTrigger = false;
        hitOrder.Clear();

        rb.angularVelocity = 0f;
        rb.velocity = Vector2.zero;

        timeElapsed = 0f;
    }
}