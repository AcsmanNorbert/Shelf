using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CopyArmature : MonoBehaviour
{
    [SerializeField] GameObject from;
    [SerializeField] GameObject to;
    [SerializeField] bool copy;

    private void OnValidate()
    {
        if (copy) copy = Copy();
    }

    public class Bodies
    {
        public Rigidbody rb;
        public CharacterJoint joint;
    }

    private bool Copy()
    {
        Dictionary<string, Bodies> bodies = new();
        Rigidbody[] rigidbodies = from.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            Bodies body = new();
            body.rb = rb;
            if (rb.TryGetComponent(out CharacterJoint joint)) body.joint = joint;
            bodies.Add(rb.name, body);
        }

        Dictionary<string, List<GameObject>> collidersDic = new();
        Collider[] colliders = from.GetComponentsInChildren<Collider>();
        foreach (var item in colliders)
        {
            List<GameObject> obj = new() { item.gameObject };
            string name = item.transform.parent.name;
            if (!collidersDic.TryAdd(name, obj))
            {
                List<GameObject> newList = collidersDic[name];
                newList.Add(item.gameObject);
                collidersDic[name] = newList;
            }
        }

        Transform[] transforms = to.GetComponentsInChildren<Transform>();
        foreach (var item in transforms)
        {
            if (bodies.TryGetValue(item.name, out Bodies body))
            {
                if (!item.TryGetComponent(out Rigidbody rb)) item.AddComponent<Rigidbody>();
                if (body.joint != null && (!item.TryGetComponent(out CharacterJoint j))) item.AddComponent<CharacterJoint>();
            }

            if (collidersDic.TryGetValue(item.name, out List<GameObject> gos))
            {
                foreach(var go in gos)
                {
                    GameObject hitCollider = Instantiate(go, item);
                    hitCollider.name = go.name;
                }
            }
        }

        return false;
    }
}
