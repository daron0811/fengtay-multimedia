using System;
using UnityEngine;
using UnityCommunity.UnitySingleton;
using DG.Tweening;
using UnityEngine.UI;

public class PopupPanel : MonoSingleton<PopupPanel>
{
    public GameObject popupPanel = null;
    public GameObject timeUpsPanel = null;
    public GameObject goPanel = null;
    public GameObject goodJobPanel = null;

    public GameObject cutScenePanel = null;

    public event Action onPopupPanelEnd;
    public event Action onTimeUpsPanel;
    public event Action onGoodJobPanel;
    public event Action onGoPanel;

    public event Action onGoCutScene;

    void Awake()
    {
        ResetStatus();
    }

    void ResetStatus()
    {
        popupPanel.SetActive(false);
        timeUpsPanel.SetActive(false);
        goodJobPanel.SetActive(false);
        goPanel.SetActive(false);
        cutScenePanel.SetActive(false);
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

    public void PlayGoodJob(Action action)
    {
        AudioManager.Instance.PlayAudioOnce(9);
        goodJobPanel.SetActive(true);
        Invoke("StopGoodJobAction", 4.0f); //時間到的長度
        onGoodJobPanel += action;

    }

    public void StopGoodJobAction()
    {
        goodJobPanel.SetActive(false);
        onGoodJobPanel?.Invoke();
        onGoodJobPanel = null;
    }

    public void PlayGo(Action action)
    {
        AudioManager.Instance.PlayAudioOnce(9);
        goPanel.SetActive(true);
        Invoke("StopGoAction", 1.0f); //時間到的長度
        onGoPanel += action;
    }

    public void StopGoAction()
    {
        goPanel.SetActive(false);
        onGoPanel?.Invoke();
        onGoPanel = null;
    }

    public void PlayCutScene(Action action)
    {
        AudioManager.Instance.PlayAudioOnce(9);
        cutScenePanel.SetActive(true);
        float duration = 1.0f;
        Material targetMaterial = cutScenePanel.GetComponent<RawImage>().material;
        DOTween.To(() => targetMaterial.GetFloat("_Fade"),
                   x => targetMaterial.SetFloat("_Fade", x),
                   1, duration) // 0 → 1
               .OnComplete(() =>
               {
                   Invoke("StopCutSceneAction", 0.3f); //時間到的長度
                   DOTween.To(() => targetMaterial.GetFloat("_Fade"),
                              x => targetMaterial.SetFloat("_Fade", x),
                              0, duration).OnComplete(() =>
                              {
                                  cutScenePanel.SetActive(false);
                              }).SetDelay(0.3f);
               });
        // ); // 1 → 0
        onGoCutScene += action;
    }

    public void StopCutSceneAction()
    {

        onGoCutScene?.Invoke();
        onGoCutScene = null;
    }

    public void ClearAction()
    {
        onPopupPanelEnd = null;
        onTimeUpsPanel = null;
        onGoodJobPanel = null;
        onGoPanel = null;
        onGoCutScene = null;
    }
}
