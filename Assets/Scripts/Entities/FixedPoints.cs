using UnityEngine;

public class FixedPoints : MonoBehaviour
{
    [SerializeField] Transform head;
    [SerializeField] Transform middle;
    [SerializeField] Transform ground;
    [Space(3)]
    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;

    public Transform Head       { get => head != null ? head : transform; }
    public Transform Middle     { get => middle != null ? middle : transform; }
    public Transform Ground     { get => ground != null ? ground : transform; }
    public Transform LeftHand   { get => leftHand != null ? leftHand : transform; }
    public Transform RightHand  { get => rightHand != null ? rightHand : transform; }

    public enum FixedPoint { Head, Middle, Ground, LeftHand, RightHand }

    public Vector3 GetPosition(FixedPoint target)
    {
        switch (target) 
        {
            case FixedPoint.Head: return head.position;
            case FixedPoint.Middle: return middle.position;
            case FixedPoint.Ground: return ground.position;
            case FixedPoint.LeftHand: return leftHand.position;
            case FixedPoint.RightHand: return rightHand.position;
            default: return Vector3.zero;
        }
    }
}
