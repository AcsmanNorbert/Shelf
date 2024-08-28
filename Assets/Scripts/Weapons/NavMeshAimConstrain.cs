using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class NavMeshAimConstrain : MonoBehaviour
{
    [SerializeField] EnemyNavMesh navMesh;
    [SerializeField] MultiAimConstraint[] aimConstraints;
    [SerializeField] float wakeupSpeed = 5f;
    [SerializeField] ConstraintTargets[] targets;

    [Serializable]
    public class ConstraintTargets
    {
        public Transform transform;
        public FixedPoints.FixedPoint targetPosition;
        public float followSpeed = 1;
    }

    private void Start()
    {
        if (aimConstraints != null)
            foreach (var constraint in aimConstraints) constraint.weight = 0;
    }

    private void Update()
    {
        if (navMesh.Target == null) return;

        int lerpTarget = navMesh.CurrentState == EnemyNavMesh.State.Attack ? 1 : 0;
        foreach (var cons in aimConstraints)
        {
            if (wakeupSpeed > 0f) cons.weight = Mathf.Lerp(cons.weight, lerpTarget, Time.deltaTime * wakeupSpeed);
            else cons.weight = lerpTarget;
        }
        foreach (var target in targets)
        {
            Vector3 targetPos = navMesh.Target.FixedPoints.GetPosition(target.targetPosition);
            if (target.followSpeed > 0f) target.transform.position = Vector3.Slerp(target.transform.position, targetPos, Time.deltaTime * target.followSpeed);
            else target.transform.position = targetPos;
        }

    }
}
