using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongBall : MonoBehaviour
{
    public float launchVelocity = 1f;
    public float speedup = 0.05f;
    public AudioClip bounceSound;

    private Rigidbody rigid;
    private AudioSource aSource;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        aSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchBall();
        }

        if (transform.position.x < -5.5f)
        {
            LaunchBall();
            PongManager.Instance.PlayerScored(1);
        } else if (transform.position.x > 5.5f)
        {
            LaunchBall();
            PongManager.Instance.PlayerScored(0);
        }
    }

    public void LaunchBall()
    {
        transform.position = new Vector3(0, 1, 0);
        float eulerLaunch = Random.Range(-45f, 45f);
        Vector3 launchDir = Quaternion.AngleAxis(eulerLaunch, Vector3.up) * Vector3.right;
        if (Random.Range(-1f,1f) > 0)
        {
            launchDir = launchDir * launchVelocity * 1f;
        } else
        {
            launchDir = launchDir * launchVelocity * -1f;
        }

        rigid.velocity = launchDir;
    }

    public void OnCollisionExit(Collision collision)
    {
        aSource.PlayOneShot(bounceSound);
        if (collision.gameObject.tag == "Player")
        {
            float mag = rigid.velocity.magnitude + speedup;
            Vector3 dir = (rigid.velocity + collision.gameObject.GetComponent<Rigidbody>().velocity).normalized;
            rigid.velocity = dir * mag;
        } else
        {
            float mag = rigid.velocity.magnitude + speedup;
            rigid.velocity = rigid.velocity.normalized * mag;
        }
    }
}
