using UnityEngine;

public class NavMeshLookAtTarget : MonoBehaviour
{
    [SerializeField] EnemyNavMesh navMesh;
    [SerializeField] Transform body;
    [SerializeField] Transform target;
    [SerializeField] float speed;

    private void Update()
    {
        if (navMesh.Target == null) return;
        if (navMesh.CurrentState != EnemyNavMesh.State.Attack) return;   

        target.position = Vector3.Slerp(target.position, navMesh.Target.FixedPoints.Middle.position, Time.deltaTime * speed);
        body.LookAt(target, Vector3.up);
        body.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
}
