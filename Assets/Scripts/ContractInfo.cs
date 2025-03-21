using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;


public class ContractInfo : MonoBehaviour
{
    public Button ContinueButton;
    public CanvasGroup background;
    public CanvasGroup infoGroup;
    public CanvasGroup soysourceGroup; // 這個程式如果是醬油 位置要到-90 ,0
    public CanvasGroup riceGroup; // 這個程式如果是米 位置要到0,-214

    public Image contractInfoImage;

    public List<Sprite> contractInfoImages;

    private void Start()
    {
        // soysourceGroup.gameObject.SetActive(false);
        // riceGroup.gameObject.SetActive(false);

        ContinueButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    public void Show(int type = 0)
    {
        gameObject.SetActive(true);
        contractInfoImage.sprite = contractInfoImages[type - 1];
        if (type == 1)
        {
            soysourceGroup.gameObject.SetActive(true);
            riceGroup.gameObject.SetActive(false);
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-90, 0);
            // soysourceGroup.DOFade(1, 0.5f);
        }
        else if (type == 2)
        {
            soysourceGroup.gameObject.SetActive(false);
            riceGroup.gameObject.SetActive(true);
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -214);
            riceGroup.DOFade(1, 0.5f);
        }
        FadeIn();
    }

    public void Hide()
    {
        FadeOut();
    }

    public void FadeIn(int type = 0)
    {
        background.DOFade(0, 0);
        infoGroup.DOFade(0, 0);
        soysourceGroup.DOFade(0, 0);
        riceGroup.DOFade(0, 0);

        background.DOFade(1, 0.5f);
        infoGroup.DOFade(1, 0.5f);
    }

    public void FadeOut()
    {
        background.DOFade(0, 0.5f).OnComplete(() =>
        {
            gameObject.SetActive(false);
            Stage3Panel.Instance.targetCookBookStep.animator.speed = 1;
        });
        infoGroup.DOFade(0, 0.5f);
        soysourceGroup.DOFade(0, 0.5f);
        riceGroup.DOFade(0, 0.5f);
    }
}
