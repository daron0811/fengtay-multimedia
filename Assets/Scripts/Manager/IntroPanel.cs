using System.Collections;
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
    private string waitScanText = "等待食譜掃描中 ";
    private int dotCount = 3;
    private Tween loopTween;

    bool isDotRunning = false;

    [Header("描述說明")]
    //說明描述板子
    public GameObject descPanel;
    public Image descImage;
    public Sprite descTex01;
    public Sprite descTex02;

    public GameObject descOBJ1;
    public GameObject descOBJ2;

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

        //返回重選
        repickBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayAudioOnce(0);
            resultPanel.SetActive(false);
            scanPanel.SetActive(true);
            readyScanCookbook = true;
            readyStartGame = false;
            //todo : 這裡要做arduino掃描
            // ShowDesc(true, true);
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
        isDotRunning = false;
        resultPanel.SetActive(false);

        // descImage.sprite = descTex01;
        descOBJ1.SetActive(false);
        descOBJ2.SetActive(false);

        scanPanel.SetActive(false);
        descPanel.SetActive(false);
        AudioManager.Instance.PlayBGM(0);

    }

    private void Update()
    {
        if (readyScanCookbook == true)
        {
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                isDotRunning = false;
                AudioManager.Instance.PlayAudioOnce(5);
                readyScanCookbook = false;
                readyStartGame = true;
                scanPanel.SetActive(false);
                resultPanel.SetActive(true);

                GameManager.Instance.CurrentCookBookIndex = 0; // 目前預設是西瓜
                SenserStatus();
            }
            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                isDotRunning = false;
                AudioManager.Instance.PlayAudioOnce(5);
                readyScanCookbook = false;
                readyStartGame = true;
                scanPanel.SetActive(false);
                resultPanel.SetActive(true);

                GameManager.Instance.CurrentCookBookIndex = 1; // 目前預設是西瓜
                SenserStatus();
            }
            if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                isDotRunning = false;
                AudioManager.Instance.PlayAudioOnce(5);
                readyScanCookbook = false;
                readyStartGame = true;
                scanPanel.SetActive(false);
                resultPanel.SetActive(true);

                GameManager.Instance.CurrentCookBookIndex = 2; // 目前預設是西瓜
                SenserStatus();
            }
            if (Input.GetKeyUp(KeyCode.Alpha4))
            {
                isDotRunning = false;
                AudioManager.Instance.PlayAudioOnce(5);
                readyScanCookbook = false;
                readyStartGame = true;
                scanPanel.SetActive(false);
                resultPanel.SetActive(true);

                GameManager.Instance.CurrentCookBookIndex = 3; // 目前預設是西瓜
                SenserStatus();
            }
            if (Input.GetKeyUp(KeyCode.Alpha5))
            {
                isDotRunning = false;
                AudioManager.Instance.PlayAudioOnce(5);
                readyScanCookbook = false;
                readyStartGame = true;
                scanPanel.SetActive(false);
                resultPanel.SetActive(true);

                GameManager.Instance.CurrentCookBookIndex = 4; // 目前預設是西瓜
                SenserStatus();
            }
            if (Input.GetKeyUp(KeyCode.Alpha6))
            {
                isDotRunning = false;
                AudioManager.Instance.PlayAudioOnce(5);
                readyScanCookbook = false;
                readyStartGame = true;
                scanPanel.SetActive(false);
                resultPanel.SetActive(true);

                GameManager.Instance.CurrentCookBookIndex = 5; // 目前預設是西瓜
                SenserStatus();
            }
            if (Input.GetKeyUp(KeyCode.Alpha7))
            {
                isDotRunning = false;
                AudioManager.Instance.PlayAudioOnce(5);
                readyScanCookbook = false;
                readyStartGame = true;
                scanPanel.SetActive(false);
                resultPanel.SetActive(true);

                GameManager.Instance.CurrentCookBookIndex = 6; // 目前預設是西瓜
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
        // descImage.sprite = descTex01;

        descOBJ1.SetActive(true);
        TweenerFade(descOBJ1.transform);
        descOBJ2.SetActive(false);

        descPanel.SetActive(show);
    }

    /// <summary>
    /// 顯示下一個說明
    /// </summary>
    void ShowNextDesc()
    {
        AudioManager.Instance.PlayAudioOnce(1);

        if (descOBJ1.activeInHierarchy)
        {
            descOBJ1.SetActive(false);
            descOBJ2.SetActive(true);
            TweenerFade(descOBJ2.transform);
        }
        else
        {
            descPanel.SetActive(false);
            // resultPanel.SetActive(readyStartGame);

            // loopTween = DOTween.To(() => dotCount, x => dotCount = x, 3, 0.5f) // 在 0.5 秒內變化 dotCount (0~3)
            // .SetLoops(-1, LoopType.Restart) // 無限循環
            // .OnUpdate(() => scanTipText.text = waitScanText + new string('.', dotCount)) // 更新文字
            // .SetEase(Ease.Linear);
            if (readyStartGame)
            {
                readyScanCookbook = true;
                scanPanel.SetActive(true);
                isDotRunning = true;
                StartCoroutine(AnimateDots());
            }
        }

        // if (descImage.sprite == descTex01)
        // {
        //     descImage.sprite = descTex02;
        // }
        // else if (descImage.sprite == descTex02)
        // {
        //     descPanel.SetActive(false);
        //     // resultPanel.SetActive(readyStartGame);

        //     // loopTween = DOTween.To(() => dotCount, x => dotCount = x, 3, 0.5f) // 在 0.5 秒內變化 dotCount (0~3)
        //     // .SetLoops(-1, LoopType.Restart) // 無限循環
        //     // .OnUpdate(() => scanTipText.text = waitScanText + new string('.', dotCount)) // 更新文字
        //     // .SetEase(Ease.Linear);
        //     if (readyStartGame)
        //     {
        //         readyScanCookbook = true;
        //         scanPanel.SetActive(true);
        //         isDotRunning = true;
        //         StartCoroutine(AnimateDots());
        //     }
        //     // SenserStatus();
        // }
    }


    IEnumerator AnimateDots()
    {
        while (isDotRunning)
        {
            scanTipText.text = waitScanText + new string('.', dotCount); // 更新文字
            dotCount = (dotCount + 1) % 4; // 讓dotCount 從 0 到 3 循環
            yield return new WaitForSeconds(0.5f); // 每 0.5 秒更新一次
        }
    }


    //TODO : 感應器狀態要從這邊設定
    void SenserStatus()
    {
        if (loopTween != null)
        {
            loopTween.Kill(); // 避免物件刪除時仍執行動畫
        }

        SetCookBookInfo();
    }

    public void SetCookBookInfo()
    {
        cookBookNameText.text = GameManager.Instance.CurrentCookBookInfo.name;
        cookbookNameText_2.text = GameManager.Instance.CurrentCookBookInfo.name;
        cookbookSeasonImage.sprite = seasonBackSprite[GameManager.Instance.CurrentCookBookInfo.season - 1];
        cookbookIcon.sprite = UIManager.Instance.GetCookBookSprite(GameManager.Instance.CurrentCookBookInfo.icon);
        PopupPanel.Instance.SetCutScenBG(GameManager.Instance.CurrentCookBookInfo.season);
        // cookbookIcon.sprite = UIManager.Instance.GetFoodSprite(GameManager.Instance.CurrentCookBookInfo.food1);
    }


    public void TweenerFade(Transform fatherTrans)
    {
        if (fatherTrans == null)
        {
            return;
        }

        List<Image> childImages = new List<Image>();

        int count = fatherTrans.childCount;

        for (int i = 0; i < count; i++)
        {
            Transform childTran = fatherTrans.GetChild(i);
            Image childImage = childTran.GetComponent<Image>();
            childImage.color = new Color(1, 1, 1, 0);
            childImages.Add(childImage);

            // 記錄初始位置
            Vector3 startPos = childTran.position;
            childTran.position = new Vector3(startPos.x, startPos.y - 50f, startPos.z); // 往下移 50

        }

        // 創建 DoTween Sequence
        Sequence sequence = DOTween.Sequence();
        float delayBetweenAnimations = 0.2f; // 每個物件間隔 0.2 秒開始動畫

        foreach (Image img in childImages)
        {
            Transform imgTransform = img.transform;
            sequence.Append(
                img.DOFade(1f, 0.5f) // 透明度 0 → 1，0.5 秒
                     .OnStart(() => imgTransform.DOMoveY(imgTransform.position.y + 50f, 0.5f)) // 移動回原來的位置
            )
            .AppendInterval(delayBetweenAnimations); // 每個動畫間隔
        }
    }
}
