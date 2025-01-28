using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject introOBJ;
    public GameObject stage1_OBJ;
    public GameObject stage2_OBJ;
    public GameObject stage3_OBJ;
    public GameObject stage4_OBJ;

    public enum UIState
    {
        Intro,
        MainMenu,
        Game,
        Pause,
        GameOver
    }

    private UIState currentState;

    void Start()
    {
        SetState(UIState.Intro);
    }

    public void SetState(UIState newState)
    {
        currentState = newState;
        UpdateUI();
    }

    private void UpdateUI()
    {
        introOBJ.SetActive(currentState == UIState.Intro);
        stage1_OBJ.SetActive(currentState == UIState.MainMenu);
        stage2_OBJ.SetActive(currentState == UIState.Game);
        stage3_OBJ.SetActive(currentState == UIState.Pause);
        stage4_OBJ.SetActive(currentState == UIState.GameOver);
    }

    public void OnIntroComplete()
    {
        SetState(UIState.MainMenu);
    }

    public void OnStartGame()
    {
        SetState(UIState.Game);
    }

    public void OnPauseGame()
    {
        SetState(UIState.Pause);
    }

    public void OnResumeGame()
    {
        SetState(UIState.Game);
    }

    public void OnGameOver()
    {
        SetState(UIState.GameOver);
    }
}
