using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager i;

    private void Start() 
    {
        if (i == null)
            i = this;

        loadoutManager = playerRef.GetComponent<LoadoutManager>();
    }

    public GameObject playerRef;
    public LoadoutManager loadoutManager;
}
