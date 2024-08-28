using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavMesh : MonoBehaviour
{
    public enum State
    {
        Seeking,
        Follow,
        Attack,
        Dead,
        Stop
    }

    [SerializeField] GameObject mesh;
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] Health health;
    public Transform player;

    [Space(3), Header("Debug stuff")]
    public bool showGizmos;
    [SerializeField] bool isSleeping = false;
    bool alertGizmoDraw;
    [SerializeField] bool debugState;

    [Space(3), Header("Attack state")]
    [Tooltip("The time between two \"OnAttackTrigger\" events are called")]
    [SerializeField] float attackSpeed = 1f;
    [Tooltip("The time it takes for the character to continue moving after it attacked")]
    [SerializeField] float attackRecovery = 0.5f;
    [Tooltip("If false, wait for \"Attack recovery\" amount of time before it's first attack event")]
    [SerializeField] bool attackImmediately = false;
    [Tooltip("Call \"AttackUpdate\" during FollowUpdate")]
    [SerializeField] bool alwaysAttack = false;
    [Tooltip("The distance which from the agent starts attacking it's target")]
    [SerializeField] float attackRange = 30f;
    [Tooltip("If true the agent only attacks the target if they are in LoS with it, if false it attacks if it's in \"Attack range\"")]
    [SerializeField] bool attackRaycast = true;
    [SerializeField] bool stepBeforeAttack = true;
    [SerializeField] bool faceTargetDuringAttack = true;
    [SerializeField, Range(0, 360)] float fov = 40f;

    [Space(3), Header("Seeking state")]
    [Tooltip("If true, finds player immdeiately")]
    [SerializeField] bool findTargetImmediately = false; // was public for some reason
    [SerializeField] float seekingMovementSpeed = 1f;
    [SerializeField] float seekingAngularSpeed = 300f;
    [SerializeField] float seekingDistance = 20f;
    [Tooltip("If true the agent only sees the target if they are in LoS with it, if false it sees it if it's in \"Seeking range\"")]
    [SerializeField] bool seekingRaycast = true;

    [Space(3), Header("Follow state")]
    [SerializeField] float followMovementSpeed = 3.5f;
    [SerializeField] float followAngularSpeed = 500f;
    [Tooltip("Sets up NavMeshAgent's StoppingDistance")]
    [SerializeField] float followDistance = 40f;
    [Tooltip("If true the agent only follows the target if it is in LoS with it, if false it follows it if it's in \"Follow range\"")]
    [SerializeField] bool followRaycast = true;

    [Space(10)]
    public UnityEvent OnAttackTrigger;

    private void Start()
    {
        if (mesh == null)
        {
            GameObject meshSearch = transform.GetChild(0).gameObject;
            if (meshSearch == null)
                meshSearch = gameObject;
            mesh = meshSearch;
            Debug.Log("There was no mesh set to " + gameObject.name + ". I set " + mesh.name + ". Your welcome");
        }
        player = GameManager.i.playerRef.transform;

        if (health == null) health = GetComponent<Health>();
        SetState(State.Seeking);
    }

    private void OnValidate()
    {
        if (navMeshAgent == null) navMeshAgent = GetComponent<NavMeshAgent>();

        if (attackImmediately) attackTimer = attackSpeed;
    }

    // Update inside STATE_MACHINE -> STATE
    #region STATE_MACHINE

    #region STATE
    // State machine Setup

    public State CurrentState { private set; get; } = State.Seeking;

    public void SetState(State newState)
    {
        switch (newState)
        {
            case State.Seeking:
                navMeshAgent.speed = seekingMovementSpeed;
                //navMeshAgent.acceleration = seekingMovementSpeed;
                navMeshAgent.angularSpeed = seekingAngularSpeed;
                break;
            case State.Follow:
                navMeshAgent.speed = followMovementSpeed;
                //navMeshAgent.acceleration = followMovementSpeed;
                navMeshAgent.angularSpeed = followAngularSpeed;
                navMeshAgent.isStopped = false;
                break;
            case State.Dead:
                navMeshAgent.enabled = false;
                break;
            case State.Attack:
                StartCoroutine(StepForward());
                if (attackImmediately) attackTimer = attackSpeed;
                else attackTimer = 0;
                break;
            default:
                break;
        }
        if (debugState) Debug.Log($"{gameObject.name} - {newState}");
        CurrentState = newState;
    }

    private void Update()
    {
        if (!navMeshAgent.enabled) return;
        if (isSleeping) return;

        switch (CurrentState)
        {
            case State.Seeking:
                SeekingUpdate();
                break;
            case State.Follow:
                FollowUpdate();
                break;
            case State.Attack:
                AttackUpdate();
                break;
        }
    }
    #endregion STATE

    #region SEEKING
    // Seeking

    Transform target;
    public Health Target
    {
        get
        {
            if (target != null) return target.GetComponent<Health>();
            else return null;
        }
    }

    private void SeekingUpdate()
    {
        if (!findTargetImmediately)
        {
            if (seekingRaycast && RaycastTarget(player, seekingDistance)) TargetFoundAlert();
            else if (!seekingRaycast && Vector3.Distance(transform.position, player.position) <= seekingDistance) TargetFoundAlert();
        }
        else
            TargetFoundAlert();
    }
    #endregion SEEKING

    #region FOLLOW
    // Follow

    private void FollowUpdate()
    {
        navMeshAgent.destination = target.position;
        if (!alwaysAttack)
        {
            if ((followRaycast && RaycastTarget(target, followDistance)) || (!followRaycast && InAttackRange())) SetState(State.Attack);
        }
        else AttackUpdate();
        if (attackImmediately) TickAttackTimer();
    }

    IEnumerator StepForward()
    {
        if (stepBeforeAttack) yield return new WaitForSeconds(0.5f);
        else yield return null;
        navMeshAgent.isStopped = true;
    }
    #endregion FOLLOW

    #region ATTACK
    // Attack

    bool doingAttack;
    float attackTimer;

    private void AttackUpdate()
    {
        transform.LookAt(target);
        if (doingAttack) return;
        if (attackTimer < attackSpeed)
        {
            TickAttackTimer();
            return;
        }

        bool canAttack = false;
        if (!alwaysAttack)
        {
            if (FovCheck(target) && FindTarget(target, followDistance)) canAttack = true;
        }
        else if (InAttackRange()) canAttack = true;

        if (canAttack)
        {
            Debug.Log("yes");
            StartCoroutine(StartAttack());
        }
        else
        {
            Debug.Log("no");
            FaceTarget();
            SetState(State.Follow);
        }
    }

    IEnumerator StartAttack()
    {
        attackTimer = 0;
        doingAttack = true;
        OnAttackTrigger?.Invoke();

        if (faceTargetDuringAttack) FaceTarget();
        PlayEffect(EntityEvent.Attack);

        yield return new WaitForSeconds(attackRecovery);

        if (CurrentState == State.Dead) yield break;
        doingAttack = false;
    }

    private void TickAttackTimer() => attackTimer += Time.deltaTime;
    #endregion ATTACK

    #region FIND_TARGET
    public static float alertRadius = 16f;
    Vector3 alertPosition;
    float alertGizmoDuration = 1f;

    void FaceTarget()
    {
        if (target != null)
        {
            //transform.rotation = Quaternion.LookRotation(transform.forward);
        }
    }

    bool FindTarget(Transform target, float distance) => attackRaycast == true ? RaycastTarget(target, distance) : InAttackRange();

    public void TargetFound()
    {
        if (CurrentState != State.Seeking) return;

        target = player;
        SetState(State.Follow);
        PlayEffect(EntityEvent.Alert);
    }

    public void TargetFoundAlert()
    {
        alertPosition = transform.position;
        StartCoroutine(AlertGizmoShow());
        Collider[] alertColliders = Physics.OverlapSphere(transform.position, alertRadius);

        if (alertColliders == null) return;
        foreach (Collider collider in alertColliders)
            if (collider.TryGetComponent(out EnemyNavMesh enemyNavMesh)) enemyNavMesh.TargetFound();
    }

    bool FovCheck(Transform target)
    {
        //Transform head = health.FixedPoints.Head;
        Transform transform = base.transform;
        Vector3 direction = (target.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, direction) >= fov / 2) return false;
        return true;
    }

    bool RaycastTarget(Transform target, float distance)
    {
        Vector3 aimPosition = target.position;
        Vector3 head = health.FixedPoints.Head.position;
        //if (TryGetComponent(out Weapon weapon)) head = weapon.ShootingPointPosition;

        if (target.TryGetComponent(out FixedPoints targetFixedPoints)) aimPosition = targetFixedPoints.Middle.position;
        Vector3 direction = (aimPosition - head).normalized;
        Debug.DrawLine(head, direction * distance + head);
        Physics.Raycast(head, direction, out RaycastHit hit, distance, ~health.ReverseTargetMask);

        if (hit.collider == target.GetComponent<Collider>()) return true;
        else return false;
    }

    public bool InAttackRange() => target != null ? Vector3.Distance(transform.position, target.position) <= attackRange : false;
    public bool InAttackRange(out float distance)
    {
        distance = Vector3.Distance(transform.position, target.position);
        return target != null ? Vector3.Distance(transform.position, target.position) <= attackRange : false;
    }
    #endregion FIND_TARGET

    #endregion STATE_MACHINE

    public void PlayEffect(EntityEvent entityEvent)
    {
        IEntityEffect[] players = GetComponents<IEntityEffect>();
        foreach (IEntityEffect player in players)
            player.DoEffect(entityEvent);
    }

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            if (CurrentState == State.Seeking)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, seekingDistance);
            }
            else if (CurrentState != State.Dead)
            {
                Gizmos.DrawWireSphere(transform.position, navMeshAgent.stoppingDistance);
            }
            if (alertGizmoDraw)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(alertPosition, alertRadius);
            }
            Transform head = health.FixedPoints.Head;
            if (head != null)
            {
                Vector3 headPosition = head.position;
                Gizmos.color = Color.red;
                Gizmos.DrawLine(headPosition, headPosition + head.forward * seekingDistance);
                //fov
                Vector3 angle1 = DirectionFromAngle(head.eulerAngles.y, -fov / 2);
                Vector3 angle2 = DirectionFromAngle(head.eulerAngles.y, fov / 2);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(headPosition, headPosition + angle1 * seekingDistance);
                Gizmos.DrawLine(headPosition, headPosition + angle2 * seekingDistance);

                if (target != null)
                {
                    if (CurrentState == State.Follow) Gizmos.color = Color.cyan;
                    else Gizmos.color = Color.red;
                    Gizmos.DrawLine(head.position, target.position);
                }
            }
        }
    }
    IEnumerator AlertGizmoShow()
    {
        alertGizmoDraw = true;
        yield return new WaitForSeconds(alertGizmoDuration);
        alertGizmoDraw = false;
    }
    Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}