using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.SceneManagement;

public class Controller : Agent
{
    public float speed;
    private float ySpeed;
    public GameObject goal;
    private bool bDetected;
    //public GameObject checkPoints;
    private Vector3 goalPos;
    //private float dist;
    private string sceneName;
    private int checkCnt;
    private int hp;

    private Animator anim;
    private CharacterController controller;

    private void Awake()
    {
        base.Awake();
        sceneName = "Env3";
        goalPos = goal.transform.position;

    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        anim.SetBool("IsRunning", true);
        ySpeed = 0f;
        checkCnt = 0;
        hp = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (StepCount >= MaxStep - 100)
        {
            EndEpisode();
            SceneManager.LoadScene(sceneName);
        }

        AddReward(-0.001f);
        ySpeed += Physics.gravity.y * Time.deltaTime;
        if (controller.isGrounded)
        {
            ySpeed = -0.1f;
            anim.SetBool("IsGrounded", true);
            anim.SetBool("IsFalling", false);
        }

        if (ySpeed < -3f)
            anim.SetBool("IsFalling", true);

        if (transform.position.y <= -3f)
        {
            //SetReward(-50.0f - (transform.position - goalPos).magnitude + 3.0f * checkCnt);
            Debug.Log("Runner has fallen off.");
            SetReward(-50.0f + 3.0f * checkCnt);
            EndEpisode();
            SceneManager.LoadScene(sceneName);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // int Discrete = actions.DiscreteActions[0];
        // Decide direction
        float Continuous = 360.0f * Mathf.Clamp(actions.ContinuousActions[0], -0.5f, 0.5f);
        //Debug.Log("Continuous Action : " + Continuous);

        // Turn and move
        transform.rotation = Quaternion.Euler(0f, Continuous, 0f);
        controller.Move((transform.forward * speed + ySpeed * new Vector3(0, 1f, 0)) * Time.deltaTime);

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(gameObject.transform.position);
        sensor.AddObservation(hp);
        sensor.AddObservation(bDetected);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        if (Input.GetKey(KeyCode.D))
        {
            continuousActionsOut[0] = 0.25f;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            continuousActionsOut[0] = 0f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            continuousActionsOut[0] = -0.25f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            continuousActionsOut[0] = 0.5f;
        }
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
            AddReward(3.0f);
            checkCnt = checkCnt + 1;
            //Debug.Log("Touch " + checkCnt + " CheckPoint");
        }
    }

    public void SetDetection(bool detect) { bDetected = detect; }

    public void getShot()
    {
        hp -= 1;
        AddReward(-2.0f);
        if (hp <= 0)
        {
            Debug.Log("Runner is Killed.");
            AddReward(-10.0f);
            EndEpisode();
            SceneManager.LoadScene(sceneName);
        }
    }
}
