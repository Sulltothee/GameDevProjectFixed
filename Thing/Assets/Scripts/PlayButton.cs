using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [SerializeField]
   public void PlayGame()
    {
      SceneManager.LoadScene("Level 1");
       
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}



