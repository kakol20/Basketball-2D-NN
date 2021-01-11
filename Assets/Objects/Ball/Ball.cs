using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask exitTargetLayer;

    public bool HitFloor { get; private set; }
    public bool HitTarget { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
        HitFloor = false;
        HitTarget = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (HitTarget)
        {
            DebugGUI.LogPersistent("hitTarget", "Ball has hit target");
        }
        else
        {
            DebugGUI.LogPersistent("hitTarget", "Ball has hit not target");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == Mathf.Log(floorLayer.value, 2))
        {
            DebugGUI.LogPersistent("hitFloor", "Ball has hit floor"); // for testing only

            HitFloor = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == Mathf.Log(targetLayer.value, 2) && !HitTarget)
        {
            HitTarget = true;
        }

        if (collision.gameObject.layer == Mathf.Log(exitTargetLayer.value, 2) && HitTarget)
        {
            HitTarget = false;
        }
    }
}