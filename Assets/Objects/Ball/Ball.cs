using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
    [SerializeField] private float maxWait = 5.0f;
    [SerializeField] private int IdealBounceAmount = 3;
    [SerializeField] private LayerMask exitTargetLayer;
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask midTargetLayer;
    [SerializeField] private LayerMask topTargetLayer;
    [SerializeField] private Color HitColor = Color.green;
    [SerializeField] private Color NoHitColor = Color.red;

    //private bool hitEntryTrigger = false;
    //private bool hitExitTrigger = false;
    //private bool hitTopTrigger = false;
    private float timeElapsed = 0f;

    private List<Target> hitOrder = new();
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    private enum Target {
        Top, Mid, Exit
    }

    public bool HitFloor { get; private set; }
    public bool HitTarget { get; private set; }
    public float ScoreOffset { get; private set; }

    private bool CountBounces = true;
    private int BounceCount = 0;

    public void Reset() {
        HitFloor = false;
        HitTarget = false;

        //hitEntryTrigger = false;
        //hitExitTrigger = false;
        //hitTopTrigger = false;
        hitOrder.Clear();

        rb.angularVelocity = 0f;
        rb.velocity = Vector2.zero;

        timeElapsed = 0f;

        CountBounces = true;
        BounceCount = 0;
        ScoreOffset = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == Mathf.Log(floorLayer.value, 2) && !HitFloor) {
            //DebugGUI.LogPersistent("hitFloor", "Ball has hit floor"); // for testing only

            HitFloor = true;

            rb.velocity /= 2f;
            rb.angularVelocity /= 2f;
        }

        if (CountBounces) BounceCount++;
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    //if (collision.gameObject.layer == Mathf.Log(targetLayer.value, 2) && !HitTarget)
    //    //{
    //    //    hitEntryTrigger = true;
    //    //}

    //    //if (collision.gameObject.layer == Mathf.Log(exitTargetLayer.value, 2) && !HitTarget)
    //    //{
    //    //    hitExitTrigger = true;
    //    //}

    //    //if (collision.gameObject.layer == Mathf.Log(topTargetLayer.value, 2) && HitTarget)
    //    //{
    //    //    hitTopTrigger = true;
    //    //}

    //    //if (collision.gameObject.layer == Mathf.Log(topTargetLayer.value, 2)) hitOrder.Add(Target.Top);
    //    //if (collision.gameObject.layer == Mathf.Log(midTargetLayer.value, 2)) hitOrder.Add(Target.Mid);
    //    //if (collision.gameObject.layer == Mathf.Log(exitTargetLayer.value, 2)) hitOrder.Add(Target.Exit);
    //}

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.layer == Mathf.Log(topTargetLayer.value, 2)) hitOrder.Add(Target.Top);
        if (collision.gameObject.layer == Mathf.Log(midTargetLayer.value, 2)) hitOrder.Add(Target.Mid);
        if (collision.gameObject.layer == Mathf.Log(exitTargetLayer.value, 2)) hitOrder.Add(Target.Exit);

        //ClosestDistAtHitTarget = 1f - (collision.gameObject.transform.position - transform.position).magnitude;
    }

    // Start is called before the first frame update
    private void Start() {
        HitFloor = false;
        HitTarget = false;

        rb = GetComponent<Rigidbody2D>();

        sprite = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update() {
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

        if (timeElapsed >= 24f) HitFloor = true;

        //if (hitEntryTrigger && hitExitTrigger && hitTopTrigger) HitTarget = true;

        if (hitOrder.Count >= 3) {
            // ball must hit top first
            if (hitOrder[0] == Target.Top) {
                int last = hitOrder.Count - 1;

                //if (hitOrder[last] == Target.Exit && hitOrder[last - 1] == Target.Mid) HitTarget = true; // doesn't matter what happens in between

                if (hitOrder[last] == Target.Mid) {
                    HitTarget = true;
                    CountBounces = false;

                    ScoreOffset = (IdealBounceAmount - BounceCount) / (float)IdealBounceAmount;
                }
            }
        }

        if (HitTarget) {
            sprite.color = HitColor;
        }
        else if (!HitTarget && HitFloor) {
            sprite.color = NoHitColor;
        }
        else {
            sprite.color = Color.white;
        }
    }
}