using UnityEngine;
using UnityEngine.SceneManagment;

public class PlayButton
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Level 1");

    }
}
