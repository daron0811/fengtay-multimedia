using System;
using UnityEngine;
using UnityCommunity.UnitySingleton;

public class PopupPanel : MonoSingleton<PopupPanel>
{
    public GameObject popupPanel = null;
    public GameObject timeUpsPanel = null;

    public event Action onPopupPanelEnd;
    public event Action onTimeUpsPanel;

    void Awake()
    {
        ResetStatus();
    }

    void ResetStatus()
    {
        popupPanel.SetActive(false);
        timeUpsPanel.SetActive(false);
        ClearAction();
    }

    public void PlayReadyPanel(Action action)
    {
        AudioManager.Instance.PlayAudioOnce(8);
        popupPanel.SetActive(true);
        Invoke("StopReadyPanelAction", 4.0f); //時間到的長度
        onPopupPanelEnd += action;
    }

    public void StopReadyPanelAction()
    {
        popupPanel.SetActive(false);
        onPopupPanelEnd?.Invoke();
        onPopupPanelEnd = null;
    }

    public void PlayTimeUp(Action action)
    {
        AudioManager.Instance.PlayAudioOnce(9);
        timeUpsPanel.SetActive(true);
        Invoke("StopTimeUpAction", 4.0f); //時間到的長度
        onTimeUpsPanel += action;

    }

    public void StopTimeUpAction()
    {
        timeUpsPanel.SetActive(false);
        onTimeUpsPanel?.Invoke();
        onTimeUpsPanel = null;
    }

    public void ClearAction()
    {
        onPopupPanelEnd = null;
        onTimeUpsPanel = null;
    }
}
