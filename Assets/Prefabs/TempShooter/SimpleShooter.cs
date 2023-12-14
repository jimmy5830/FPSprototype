using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleShooter : MonoBehaviour
{
    [SerializeField]
    private GameObject runner;

    private Vector3 rayDir;
    private RaycastHit rayHit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rayDir = (runner.transform.position - transform.position).normalized;

        if(Physics.Raycast(transform.position, rayDir, out rayHit))
        {
            if (rayHit.collider.gameObject.CompareTag("Runner"))
            {
                transform.rotation = Quaternion.LookRotation(new Vector3(rayDir.x, 0f, rayDir.z));
            }
        }

    }
}
