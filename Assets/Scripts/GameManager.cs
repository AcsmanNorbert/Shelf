using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager i;

    private void Start()
    {
        if (i == null)
            i = Instantiate(this);
    }

    public GameObject playerRef;
}
