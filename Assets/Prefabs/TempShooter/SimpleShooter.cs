using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleShooter : MonoBehaviour
{
    [SerializeField]
    private GameObject runner;

    [SerializeField]
    private GameObject bullet;

    private Controller runnerController;
    private Vector3 rayDir;
    private RaycastHit rayHit;

    private bool bDetect;

    // Start is called before the first frame update
    void Start()
    {
        runnerController = runner.GetComponent<Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        bDetect = false;
        rayDir = (runner.transform.position - transform.position).normalized;

        if(Physics.Raycast(transform.position, rayDir, out rayHit))
        {
            if (rayHit.collider.gameObject.CompareTag("Runner"))
            {
                bDetect = true;
                transform.rotation = Quaternion.LookRotation(new Vector3(rayDir.x, 0f, rayDir.z));
            }
        }

        runnerController.SetDetection(bDetect);

        if (bDetect)
        {
            bool bFire = Random.Range(0f, 1f) > 0.925;
            if (bFire)
            {
                Vector3 aimCenter = runner.transform.Find("AimPoint").transform.position;
                Vector3 aimPos = aimCenter + Random.insideUnitSphere * 0.5f;
                Vector3 firePos = transform.Find("FirePoint").transform.position;
                Vector3 fireDir = (aimPos - firePos).normalized;

                Instantiate(bullet, firePos, Quaternion.LookRotation(fireDir));
            }
        }
    }
}
