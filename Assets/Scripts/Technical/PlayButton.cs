using UnityEngine;
using UnityEngine.UIElements;

public class PlayButton : MonoBehaviour
{
    Image buttonImage;
    Sprite playImage, pauseImage;

    void Awake()
    {
       // buttonImage.sprite = playImage;
    }
    public void PlayPause()
    {
        if (GameManager.Instance.gameState == GameState.Play)
        {
            //Pause
            GameManager.Instance.Pause();
            buttonImage.sprite = pauseImage;
        }
        else if (GameManager.Instance.gameState == GameState.Pause)
        {
            //Play
            GameManager.Instance.Resume();
            buttonImage.sprite = playImage;
        }
        else if (GameManager.Instance.gameState == GameState.BetweenStages)
        {
            //Start Stage - Play
            GameManager.Instance.gameState = GameState.Play;
            buttonImage.sprite = pauseImage;
        }
    }
    public void AutoStartToggle()
    {
        WaveManager.Instance.autoStart = !WaveManager.Instance.autoStart;
    }
}

