using System.Collections.Generic;
using DG.Tweening;
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
    public bool readyScanCookbook = false;

    [Header("掃描頁面")]
    public GameObject scanPanel;
    public TextMeshProUGUI scanTipText;
    private string waitScanText = "等待食譜掃描中";
    private int dotCount = 3;
    private Tween loopTween;

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

    public bool isInit = false;

    void Start()
    {
        Init();
    }

    void Init()
    {
        if (isInit)
        {
            return;
        }
        ResetStatus();
        nextDescTex.onClick.AddListener(ShowNextDesc);
        startBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayAudioOnce(0);
            ShowDesc(true, true);
        });
        descBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayAudioOnce(0);
            ShowDesc();
        });
        repickBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayAudioOnce(0);
            resultPanel.SetActive(false);
            ShowDesc(true, true);
        });
        startGameBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayAudioOnce(0);
            UIManager.Instance.SetState(UIManager.UIState.Stage_2);
        });

        isInit = true;
    }

    public void ResetStatus()
    {
        readyStartGame = false;
        resultPanel.SetActive(false);
        descImage.sprite = descTex01;
        descPanel.SetActive(false);
        AudioManager.Instance.PlayBGM(0);
    }

    private void Update()
    {
        if (readyScanCookbook == true)
        {
            if (Input.GetKeyUp(KeyCode.F1))
            {
                readyScanCookbook = false;
                readyStartGame = true;
                scanPanel.SetActive(false);
                resultPanel.SetActive(true);
                SenserStatus();
            }
        }
    }

    private void OnEnable()
    {
        if (isInit == false)
        {
            Init();
        }
        else
        {
            ResetStatus();
        }
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
        AudioManager.Instance.PlayAudioOnce(1);
        if (descImage.sprite == descTex01)
        {
            descImage.sprite = descTex02;
        }
        else if (descImage.sprite == descTex02)
        {
            descPanel.SetActive(false);
            // resultPanel.SetActive(readyStartGame);

            // loopTween = DOTween.To(() => dotCount, x => dotCount = x, 3, 0.5f) // 在 0.5 秒內變化 dotCount (0~3)
            // .SetLoops(-1, LoopType.Restart) // 無限循環
            // .OnUpdate(() => scanTipText.text = waitScanText + new string('.', dotCount)) // 更新文字
            // .SetEase(Ease.Linear);
            readyScanCookbook = true;
            scanPanel.SetActive(true);
            // SenserStatus();
        }
    }


    //TODO : 感應器狀態要從這邊設定
    void SenserStatus()
    {
        if (loopTween != null)
        {
            loopTween.Kill(); // 避免物件刪除時仍執行動畫
        }
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
