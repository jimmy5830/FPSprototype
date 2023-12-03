using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.SceneManagement;

public class RunnerAgent : Agent
{
    private Animator anim;
    private CharacterController controller;

    public float speed;
    private float ySpeed;

    private string sceneName;
    public GameObject goal;

    private int checkCnt;

    protected override void Awake()
    {
        base.Awake();
        sceneName = "TestEnv1";
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        anim.SetBool("IsRunning", true);
        ySpeed = 0f;
        checkCnt = 0;
    }

    // Update is called once per frame
    void Update()
    {
        AddReward(-0.001f);
        ySpeed += Physics.gravity.y * Time.deltaTime;
        if (controller.isGrounded)
        {
            ySpeed = -0.1f;
            anim.SetBool("IsGrounded", true);
            anim.SetBool("IsFalling", false);
        }

        if (ySpeed < -3.0f)
            anim.SetBool("IsFalling", true);

        if (transform.position.y <= -4.0f)
        {
            SetReward(-50.0f + 3.0f * checkCnt);
            EndEpisode();
            SceneManager.LoadScene(sceneName);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // int Discrete = actions.DiscreteActions[0];
        // Decide direction
        float Continuous = 30.0f * Mathf.Clamp(actions.ContinuousActions[0], -1.0f, 1.0f);
        //Debug.Log("Continuous Action : " + Continuous);

        // Turn and move
        Debug.Log(12 * Continuous);
        transform.Rotate(0f, Continuous, 0f);
        controller.Move((transform.forward * speed + ySpeed * new Vector3(0, 1f, 0)) * Time.deltaTime);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(gameObject.transform.position);
        sensor.AddObservation(gameObject.transform.eulerAngles.y);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == goal)
        {
            AddReward(50f);
            EndEpisode();
            Debug.Log("Arrive");
            SceneManager.LoadScene(sceneName);
        }
        if (collision.gameObject.CompareTag("CheckPoint"))
        {
            Destroy(collision.gameObject);
            AddReward(1.0f);
            checkCnt = checkCnt + 1;
            Debug.Log("Touch " + checkCnt + " CheckPoint");
        }
    }
}
