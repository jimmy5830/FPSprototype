using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ShooterState { None = -1, Idle = 0, Wander, Pursuit, Attack, }

public class ShooterFSM : MonoBehaviour
{
    [Header("Pursuit")]
    [SerializeField]
    private float targetRecognitionRange = 8;
    [SerializeField]
    private float pursuitLimitRange = 10;

    [Header("Attack")]
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform projectileSpawnPoint;
    [SerializeField]
    private float attackRange = 5;
    [SerializeField]
    private float attackRate = 1;

    private ShooterState shooterState = ShooterState.None; // ���� �� �ൿ
    private float lastAttackTime = 0;

    private Status status;      // �̵��ӵ� �� ����
    private NavMeshAgent navMeshAgent; // �̵� ��� ���� nAVmESHaGENT
    private Transform target;

    

    public void Setup(Transform target, EnemyMemoryPool enemyMemoryPool)
    {
        status = GetComponent<Status>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        this.target = target;
        
        

        // NavMeshAGent ������Ʈ���� ȸ���� ������Ʈ���� �ʵ��� ����
        navMeshAgent.updateRotation = false;
    }



    private void Awake()
    {
         // status�� navMeshAgent�� �̹� OnEnable �޼��忡�� �ʱ�ȭ�ǹǷ� ���⼭�� ����
        if (navMeshAgent == null)
         {
             navMeshAgent = GetComponent<NavMeshAgent>();
             if (navMeshAgent == null)
             {
                 Debug.LogError("NavMeshAgent not found on ShooterFSM.");
             }
             else
             {
                 // NavMeshAgent ������Ʈ���� ȸ���� ������Ʈ���� �ʵ��� ����
                 navMeshAgent.updateRotation = false;
             }
         }

        
    }

    private void OnEnable()
    {
        // ���� Ȱ��ȭ�Ǹ� ���� ���¸� "���"�� ����
        ChangeState(ShooterState.Idle);
    }

    private void OnDisable()
    {
        // ���� ��Ȱ��ȭ�� �� ���� ��� ���� ���¸� �����ϰ�, ���¸� "None"���� ����
        StopAllCoroutines();
        shooterState = ShooterState.None;
    }

    public void ChangeState(ShooterState newState)
    {
        // ���� ������� ���¿� �ٲٷ��� �ϴ� ���°� ������ �ٲ� �ʿ䰡 ���� ������ return
        if (shooterState == newState ) return;

        // ������ ������̴� ���� ����
        StopCoroutine(shooterState.ToString());
        // ���� ���� ���¸� newState�� ����
        shooterState = newState;
        /// ���ο� ���� ���
        StartCoroutine(shooterState.ToString());
    }

    private IEnumerator Idle()
    {
        // n�� �Ŀ� "��ȸ" ���·� �����ϴ� �ڷ�ƾ ����
        StartCoroutine("AutoChangeFromIdleToWander");

        while (true)
        {
            // ��� ������ �� �ϴ� �ൿ
            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        // 1~4�� �ð� ���
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        // ���¸� "��ȸ"�� ����
        ChangeState(ShooterState.Wander);
    }

    private IEnumerator Wander()
    {
        float currentTime    = 0;
        float maxTime       = 10;

        // �̵� �ӵ� ����
        navMeshAgent.speed = status.WalkSpeed;

        // ��ǥ ��ġ ����
        navMeshAgent.SetDestination(CalculateWanderPosition());

        // ��ǥ ��ġ�� ȸ��
        Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);

        while ( true )
        {
            currentTime += Time.deltaTime;

            // ��ǥ ��ġ�� �����ϰ� �����ϰų� �ʹ� �����ð����� ��ȸ�ϱ� ���¿� �ӹ��� ������
            to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);
            if ((to - from).sqrMagnitude < 0.01f || currentTime >= maxTime)
            {
                // ���¸� "���"�� ����
                ChangeState(ShooterState.Idle);
            }

            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ���� (��ȸ, ����, ���Ÿ� ����)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }


    }

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10; //  ���� ��ġ�� �������� �ϴ� ���� ������
        int wanderJitter = 0;   // ���õ� ���� (wanderJitterMin ~ wanderJitterMax)
        int wanderJitterMin = 0; // �ּ� ����
        int wanderJitterMax = 360; // �ִ� ����

        // ���� �� ĳ���Ͱ� �ִ� ������ �߽� ��ġ�� ũ�� (������ ��� �ൿ�� ���� �ʵ���)
        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        // �ڽ��� ��ġ�� �߽����� ������ �Ÿ�, ���õ� ������ ��ġ�� ��ǥ�� ��ǥ�������� ����
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        // ������ ��ǥ��ġ�� �ڽ��� �̵������� ����� �ʰ� ����
        targetPosition.x = Mathf.Clamp(targetPosition.x, rangePosition.x - rangeScale.x*0.5f, rangePosition.x + rangeScale.x * 0.5f);
        targetPosition.y = 0.0f;
        targetPosition.z = Mathf.Clamp(targetPosition.z, rangePosition.z - rangeScale.z*0.5f, rangePosition.z + rangeScale.z * 0.5f);

        return targetPosition;
    }

    private Vector3 SetAngle(float radius, int angle)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;

        return position;
    }

    private IEnumerator Pursuit()
    {
        while ( true )
        {
            // �̵� �ӵ� ���� (��ȸ�Ҷ��� �ȴ� �ӵ��� �̵�, ������ ���� �ٴ� �ӵ��� �̵�)
            navMeshAgent.speed = status.RunSpeed;

            // ��ǥ��ġ�� ���� �÷��̾��� ��ġ�� ����
            navMeshAgent.SetDestination(target.position);

            // Ÿ�� ������ ��� �ֽ��ϵ��� ��
            LookRotationToTarget();

            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator Attack()
    {
        navMeshAgent.ResetPath();

        while ( true )
        {
            // Ÿ�� ���� �ֽ�
            LookRotationToTarget();

            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ���� ( ��ȸ, �߰�, ���Ÿ� ����)
            CalculateDistanceToTargetAndSelectState();

            if ( Time.time - lastAttackTime > attackRate )
            {
                // �����ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ����
                lastAttackTime = Time.time;

                // �߻�ü ����
                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                clone.GetComponent<EnemyProjectile>().Setup(target.position);
            }

            yield return null;
        }
    }

    private void LookRotationToTarget()
    {
        if (target == null) return;

        // ��ǥ ��ġ
        Vector3 to = new Vector3(target.position.x, 0, target.position.z);
        // �� ��ġ
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        // �ٷ� ����
        transform.rotation = Quaternion.LookRotation(to - from);
        // ������ ����
    }

    private void CalculateDistanceToTargetAndSelectState()
    {
        if (target == null) return;

        // �÷��̾��� ���� �Ÿ� ��� �� �Ÿ��� ���� �ൿ ����
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= attackRange)
        {
            ChangeState(ShooterState.Attack);
        }
        else if (distance <= targetRecognitionRange)
        {
            ChangeState(ShooterState.Pursuit);
        }
        else if (distance >= pursuitLimitRange)
        {
            ChangeState(ShooterState.Wander);
        }
    }

    private void OnDrawGizmos()
    {

        // "��ȸ" ������ �� �̵��� ��� ǥ��
        if (navMeshAgent != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);
        }
        // ��ǥ �ν� ����
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        // ���� ����
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitLimitRange);

        // ���� ����
        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void TakeDamage(int damage)
    {
        bool isDie = status.DecreaseHP(damage);

       
    }

   
}
