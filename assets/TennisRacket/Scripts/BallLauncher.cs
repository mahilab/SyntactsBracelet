using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLauncher : MonoBehaviour
{

    public GameObject ballPrefab;
    public float ballPeriod = 5;
    public float ballForce;
    public float ballLifeTime = 10;

    ObjectPool ballPool;
    float nextBall;

    // Start is called before the first frame update
    void Start()
    {
        ballPool = ObjectPool.CreateFor(ballPrefab, 20, false);
        nextBall = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (nextBall > ballPeriod) {
            StartCoroutine(LaunchBall());
            nextBall = 0;
        }
        nextBall += Time.deltaTime;        
    }

    IEnumerator LaunchBall() {
        GameObject ball = ballPool.GetPooledObject();
        if (ball != null) {
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            ballRb.transform.position = transform.position;
            ballRb.transform.rotation = transform.rotation;
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
            ballRb.AddForce(ballRb.transform.forward * ballForce);
            yield return new WaitForSeconds(ballLifeTime);
            ball.SetActive(false);
        }
        else
            yield return null;
    }
}
