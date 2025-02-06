using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroPanel : MonoBehaviour
{
    [Header("標題")]
    public GameObject titleObj; //標題
    public Button startBtn; //開始按鈕
    public Button descBtn; // 說明按鈕
    public bool readyStartGame = false; // 準備進入遊戲

    [Header("描述說明")]
    //說明描述板子
    public GameObject descPanel;
    public Image descImage;
    public Sprite descTex01;
    public Sprite descTex02;

    public Button nextDescTex;

    [Header("遊戲管理")]
    public GameObject resultPanel;
    public Button repickBtn;
    public Button startGameBtn;

    [Header("食物偵測")]
    public TextMeshProUGUI cookBookNameText;
    public TextMeshProUGUI cookbookNameText_2;
    public Image cookbookIcon;

    public Image cookbookSeasonImage;

    public List<Sprite> seasonBackSprite;

    void Start()
    {
        Init();
    }
    void Init()
    {
        readyStartGame = false;
        resultPanel.SetActive(false);
        descImage.sprite = descTex01;
        descPanel.SetActive(false);

        nextDescTex.onClick.AddListener(ShowNextDesc);
        startBtn.onClick.AddListener(() =>
        {
            ShowDesc(true, true);
        });
        descBtn.onClick.AddListener(() =>
        {
            ShowDesc();
        });
        repickBtn.onClick.AddListener(() =>
        {
            resultPanel.SetActive(false);
            ShowDesc(true, true);
        });
        startGameBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.SetState(UIManager.UIState.Stage_2);
        });
    }

    /// <summary>
    /// 顯示說明
    /// </summary>
    /// <param name="show"></param>
    /// <param name="readyToGame"></param>
    void ShowDesc(bool show = true, bool readyToGame = false)
    {
        readyStartGame = readyToGame;
        descImage.sprite = descTex01;
        descPanel.SetActive(show);
    }

    /// <summary>
    /// 顯示下一個說明
    /// </summary>
    void ShowNextDesc()
    {
        if (descImage.sprite == descTex01)
        {
            descImage.sprite = descTex02;
        }
        else if (descImage.sprite == descTex02)
        {
            descPanel.SetActive(false);
            resultPanel.SetActive(readyStartGame);
            SenserStatus();
        }
    }

    //TODO : 感應器狀態要從這邊設定
    void SenserStatus()
    {
        GameManager.Instance.CurrentCookBookIndex = 0; // 目前預設是西瓜
        SetCookBookInfo();
    }

    public void SetCookBookInfo()
    {
        cookBookNameText.text = GameManager.Instance.CurrentCookBookInfo.name;
        cookbookNameText_2.text = GameManager.Instance.CurrentCookBookInfo.name;
        cookbookSeasonImage.sprite = seasonBackSprite[GameManager.Instance.CurrentCookBookInfo.season - 1];
        // cookbookIcon.sprite = UIManager.Instance.GetFoodSprite(GameManager.Instance.CurrentCookBookInfo.food1);
    }

}
