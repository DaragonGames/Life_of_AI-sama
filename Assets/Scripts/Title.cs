using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public void Load()
    {
        SceneManager.LoadScene("Game");
    }
}