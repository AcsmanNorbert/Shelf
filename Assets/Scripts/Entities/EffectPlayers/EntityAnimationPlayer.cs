using UnityEngine;
using UnityEngine.AI;

public class EntityAnimationPlayer : MonoBehaviour, IEntityEffect
{
    [SerializeField] Animator animator;
    [SerializeField] EntityColliderHandler colliderHandler;
    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        EnemyNavStateSet[] stateMachines = animator.GetBehaviours<EnemyNavStateSet>();
        foreach (EnemyNavStateSet stateMachine in stateMachines) stateMachine.navMeshAI = GetComponent<EnemyNavMesh>();
    }

    public void DoEffect(EntityEvent entityEvent)
    {
        switch (entityEvent)
        {
            case EntityEvent.Dead:
                if (colliderHandler != null)
                {
                    animator.enabled = false;
                    colliderHandler.MakeRagdoll(true);
                    return;
                }
                break;
            default:
                break;
        }
        animator?.SetTrigger(entityEvent.ToString());
    }

    private void Update()
    {
        if (agent == null) return;
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
    }
}
