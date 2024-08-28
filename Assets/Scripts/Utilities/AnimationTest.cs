using UnityEditor;
using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string triggerName;
    [SerializeField] string boolName;
    [SerializeField] bool boolValue;

    [Space(3), Header("Walk")]
    [SerializeField] bool walk;
    [SerializeField] float walkSpeed;
    Vector3 origin;
    [SerializeField] bool reset;

    private void Start()
    {
        reset = false;
        origin = transform.position;
    }

    private void OnValidate()
    {
        if (boolName != string.Empty) animator.SetBool(boolName, boolValue);
        if (reset && Application.isPlaying)
        {
            reset = false;
            transform.position = origin;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            animator.SetTrigger(triggerName);
        }

        if (walk) 
        {
            animator.SetFloat("Speed", walkSpeed);
            transform.position += transform.forward * walkSpeed * Time.deltaTime; 
        }
    }
}
