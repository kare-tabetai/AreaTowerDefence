using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Allow : Actor
{
    [SerializeField]
    float velocity = 1;
    [SerializeField]
    int damage = 100;
    [SerializeField, Tooltip("刺さってから消えるまでの時間")]
    float lostTime = 5;

    bool stuck;
    Rigidbody rb;
    float timer;

    public override void Initialize(int playerNum)
    {
        base.Initialize(playerNum);
        var forward = transform.forward;
        rb = GetComponent<Rigidbody>();
        rb.AddForce(forward * velocity, ForceMode.Impulse);
    }

    void Start ()
	{
		
	}

	void Update ()
	{
        if (stuck)
        {
            timer += Time.deltaTime;
            if (lostTime <= timer) { Destroy(gameObject); }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Stage"
            && other.tag != "Actor") { return; }
        if (other.tag == "Stage")
        {
            stuck = true;
            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            return;
        }
        var hpActor = other.GetComponent<HpActor>();
        if (hpActor == null) { return; }
        if (hpActor.PlayerNumber == PlayerNumber) { return; }

        hpActor.Damage(damage, this);
        stuck = true;
        transform.parent = other.transform;
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
}
