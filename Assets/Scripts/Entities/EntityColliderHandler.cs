using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityColliderHandler : MonoBehaviour
{
    [SerializeField] bool getColliders;
    [SerializeField] bool renameColliders;
    [SerializeField] bool giveCollidersScript;
    [SerializeField] bool getRigidbodies;
    [SerializeField] bool setVelocityAmount;

    [SerializeField] float velocityAmount;
    [SerializeField] Health health;

    [SerializeField] List<Collider> colliders = new();
    [SerializeField] List<HitCollision> hitColliders = new();
    [SerializeField] List<Rigidbody> rigidbodies = new();


    // Start is called before the first frame update
    void OnValidate()
    {
        if (getColliders)
        {
            colliders = new();
            Collider[] childrenColliders = GetComponentsInChildren<Collider>();
            if (childrenColliders != null)
                colliders.AddRange(childrenColliders);
            foreach (Collider collider in childrenColliders)
            {
                if (!collider.TryGetComponent(out DrawCollider drawer)) drawer = collider.AddComponent<DrawCollider>();
                drawer.drawCollider = true;
                drawer.color = new Color(1, 0.5f, 0); //orange
            }

            //Collider myCollider = GetComponent<Collider>();
            //if (myCollider != null) colliders.Add(myCollider);

            getColliders = false;
        }

        if (renameColliders)
        {
            string currentName = "";
            int amount = 0;
            if (colliders == null) return;
            foreach (Collider collider in colliders)
            {
                string parentName = collider.transform.parent.name;
                if (currentName != parentName)
                {
                    amount = 0;
                    currentName = parentName;
                }
                amount++;
                collider.name = $"{parentName}-{amount}_{collider.GetType().Name}";
            }
            renameColliders = false;
        }

        if (giveCollidersScript)
        {
            hitColliders = new();
            foreach (Collider collider in colliders)
            {
                if (!collider.TryGetComponent(out HitCollision hc)) hc = collider.AddComponent<HitCollision>();
                hc.health = health;
                hitColliders.Add(hc);
                collider.isTrigger = false;
            }
            giveCollidersScript = false;
        }

        if (getRigidbodies)
        {
            rigidbodies = new();
            rigidbodies.AddRange(GetComponentsInChildren<Rigidbody>());
            if (rigidbodies == null) Debug.Log($"No rigidbodies found on {gameObject.name}.");
            else Debug.Log($"Dont forget to setup bone joints on {gameObject.name}. ;)");
            foreach (Rigidbody rb in rigidbodies)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
            getRigidbodies = false;
        }

        if (setVelocityAmount)
        {
            foreach (HitCollision hitCollision in hitColliders)
                hitCollision.velocityAmount = velocityAmount;
            setVelocityAmount = false;
        }
    }

    public void MakeRagdoll(bool ragdoll)
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.useGravity = ragdoll;
            rb.isKinematic = !ragdoll;
        }
    }
}
