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

    public void SetCutScenBG(int season)
    {
        Material targetMaterial = cutScenePanel.GetComponent<RawImage>().material;

        Color seasonColor = Color.white;

        switch (season)
        {
            case 1:
                seasonColor = new Color32(233, 183, 173, 255);
                break;
            case 2:
                seasonColor = new Color32(147, 192, 130, 255);
                break;
            case 3:
                seasonColor = new Color32(238, 208, 130, 255);
                break;
            case 4:
                seasonColor = new Color32(174, 158, 147, 255);
                break;
        }

        targetMaterial.SetColor("_Color", seasonColor);
    }

    public void PlayCutScene(Action action)
    {
        AudioManager.Instance.PlayAudioOnce(11);
        cutScenePanel.SetActive(true);
        float screenWidth = Screen.width;
        RectTransform imageRect = cutScenePanel.GetComponent<RectTransform>();
        // 設定初始位置在畫面外左側
        imageRect.anchoredPosition = new Vector2(-screenWidth / 2 - imageRect.rect.width, 0);

        // 目標位置（移動到畫面右側並超出）
        float targetX = screenWidth / 2 + imageRect.rect.width;
        imageRect.DOAnchorPos(new Vector2(targetX, 0), 2.0f)
            .SetEase(Ease.Linear) // 設定線性移動
            .OnComplete(() => Debug.Log("動畫完成")); // 可加入回呼事件
        onGoCutScene += action;
        
        Invoke("StopCutSceneAction", 1.0f);
        return;
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
