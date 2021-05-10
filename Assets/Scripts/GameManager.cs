using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform startPoint;
    public GameObject player;
    private SceneChanger sc;

    void Start()
    {
        sc = GameObject.Find("SceneLoader").GetComponent<SceneChanger>();
        Instantiate(player, startPoint);
    }

    public void Restart()
    {
        sc.ChangeScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Update()
    {
        
    }
}
