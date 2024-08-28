using UnityEngine;

public class MobSpawnpoint : MonoBehaviour
{
    public bool drawGizmos;
    public Color color = Color.red;

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        Gizmos.color = color;
        Gizmos.DrawMesh(Resources.Load<Mesh>("Models/SpawnPiramid"), transform.position, transform.rotation, transform.localScale * 2);
    }
}
