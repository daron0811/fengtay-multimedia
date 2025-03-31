using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections;
using Unity.Mathematics;

public class Stage4Panel : MonoBehaviour
{
    [Header("描述說明")]
    public GameObject descObj;
    public List<GameObject> descItem;
    public Button nextDescBtn;

    [Header("詳細表")]
    public GameObject detailPanel;
    private List<FoodItem> foodItems; //從子物件下取得
    private List<FoodItem> otherFoodItems; //從子物件下取得
    public TextMeshProUGUI cookbookNameText;
    public CountdownTimer detailPanelTimer;

    public Image cookbookDetailImage; //TODO:要用最後狀態的圖

    public TextMeshProUGUI localText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI seasonText;
    public TextMeshProUGUI nutritionTips;
    public Image detailPanelFood;

    public FoodItem scanFood;

    [Header("檢查完成")]
    public GameObject checkEndPanel;
    public Button gotoResultBtn;

    public GameObject waitScanTipOBJ;
    private bool waitForScan = false;

    private string waitScanTipTex = "等待掃描中 ";

    public TextMeshProUGUI waitScanTipText;
    public TextMeshProUGUI waitScanTipText2;

    public TextMeshProUGUI waitTipText;
    private string gotoResultText = "耐心等待評分中 ";
    private bool isDotRunning = false;
    private int dotCount = 3;

    [Header("最後結算")]
    public GameObject resultPanel;
    public CanvasGroup scorePanel;
    public CanvasGroup finalPanel;
    public Button gotoEndBtn;

    public Image rateImage;
    public List<Sprite> rateSprites;

    public Image seasonImage;
    public List<Sprite> seasonSprites;

    public Transform cookbookItem;

    public Vector3 originPos = new Vector3(-379.0f, -47.0f, 0.0f);
    public Vector3 resultPos = new Vector3(-379.0f, 55.0f, 0.0f);

    public TextMeshProUGUI finalCookbookNameText;
    public Image finalCookbookImage;
    public List<FoodItem> finalFoodItems;
    public List<Image> seasonImages;

    public TextMeshProUGUI commonText;
    public TextMeshProUGUI commonDescText;

    public Button returnTitleBtn;
    public Button playAgainBtn;

    public List<string> rateCommonText;
    public List<string> rateDescTexts;

    public GameObject contractImage;//契作圖片
    private bool isInit = false;

    public Sprite marksprite;
    public Sprite unmarksprite;

    public Image sensorImage;
    public Sprite successSprite;
    public Sprite failSprite;

    public Image qecodeImage;
    public List<Sprite> foodbookQRCode;
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

        foodItems = detailPanel.transform.Find("Content/PickupFoods").GetComponentsInChildren<FoodItem>().ToList();
        otherFoodItems = detailPanel.transform.Find("Content/OtherFoods").GetComponentsInChildren<FoodItem>().ToList();
        finalFoodItems = resultPanel.transform.Find("FinalPanel/Food/FoodGroup").GetComponentsInChildren<FoodItem>().ToList();
        seasonImages = resultPanel.transform.Find("FinalPanel/Season/SeasonIcon").GetComponentsInChildren<Image>().ToList();

        gotoResultBtn.onClick.AddListener(() =>
        {
            SetFinalPanel();
        });

        gotoEndBtn.onClick.AddListener(() =>
        {
            cookbookItem.DOLocalMove(resultPos, 1.0f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                finalPanel.gameObject.SetActive(true);
                scorePanel.DOFade(0.0f, 1.0f);
                finalPanel.DOFade(1.0f, 1.0f);
            });
        });

        returnTitleBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.SetState(UIManager.UIState.Stage_1);
        });

        playAgainBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.SetState(UIManager.UIState.Stage_1);
        });

        foreach (var item in foodItems)
        {
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                detailPanelFood.gameObject.SetActive(true);
                detailPanelFood.sprite = item.foodSprite.sprite;
                SetDetialPanelInfo(item.foodSprite.sprite.name);
            });
        }

        ResetStatus();

        isInit = true;
    }

    public void ResetStatus()
    {
        checkEndPanel.SetActive(false);
        descObj.SetActive(true);
        foreach (var item in descItem)
        {
            item.SetActive(false);
        }
        descItem.First().SetActive(true);
        nextDescBtn.onClick.AddListener(ShowNextDesc);

        detailPanel.SetActive(false);
        resultPanel.SetActive(false);
        waitScanTipOBJ.SetActive(false);
        waitForScan = false;

        SetCookBookInfo();

        contractImage.SetActive(false);
        scanFood.gameObject.SetActive(false);
        scorePanel.gameObject.SetActive(true);
        finalPanel.gameObject.SetActive(false);

        localText.text = "";
        foodText.text = "";
        seasonText.text = "";
        nutritionTips.text = "";

        detailPanelFood.gameObject.SetActive(false);


        // foreach (var item in otherFoodItems)
        // {
        //     item.Hide();
        // }
    }

    void OnEnable()
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

    private void ShowNextDesc()
    {
        var currentActive = descItem.FirstOrDefault(x => x.activeSelf);
        var currentIndex = descItem.IndexOf(currentActive);
        currentActive.SetActive(false);
        if (currentIndex + 1 < descItem.Count)
        {
            descItem[currentIndex + 1].SetActive(true);
        }
        else
        {
            descObj.SetActive(false);
            waitScanTipOBJ.SetActive(true);
            waitForScan = true;
            detailPanel.SetActive(true);
            StartCoroutine(AnimateWaitDots());
            SetDetialPanel();
        }
    }

    IEnumerator AnimateWaitDots()
    {
        while (waitForScan)
        {
            waitScanTipText.text = waitScanTipTex + new string('.', dotCount); // 更新文字
            waitScanTipText2.text = waitScanTipTex + new string('.', dotCount); // 更新文字
            dotCount = (dotCount + 1) % 4; // 讓dotCount 從 0 到 3 循環
            yield return new WaitForSeconds(0.5f); // 每 0.5 秒更新一次
        }
    }

    #region 設定詳細頁面
    public void SetCookBookInfo()
    {
        SetFoodItems(GameManager.Instance.CurrentCookBookIndex);
        SetOtherFoodItems(GameManager.Instance.CurrentCookBookIndex);
        cookbookNameText.text = GameManager.Instance.CurrentCookBookInfo.name;
        cookbookDetailImage.sprite = UIManager.Instance.GetCookBookSprite(GameManager.Instance.CurrentCookBookInfo.icon);
        finalCookbookNameText.text = GameManager.Instance.CurrentCookBookInfo.name;
        finalCookbookImage.sprite = UIManager.Instance.GetCookBookSprite(GameManager.Instance.CurrentCookBookInfo.icon);
        qecodeImage.sprite = foodbookQRCode[GameManager.Instance.currentCookBookIndex];
    }

    //設定食材類型
    public void SetFoodItems(int cookbookIndex)
    {
        if (cookbookIndex == -1)
        {
            return;
        }
        List<string> foods = DataManager.Instance.GetFoodbyCookbook(cookbookIndex);
        if (foods == null)
        {
            return;
        }

        Dictionary<string, bool> pickedFoods = GameManager.Instance.pickedFoods;
        for (int i = 0; i < foodItems.Count; i++)
        {
            if (i < foods.Count)
            {
                bool isPicked = true;
                if (pickedFoods.ContainsKey(foods[i]) == true)
                {
                    isPicked = pickedFoods[foods[i]];
                }
                Sprite sprite = UIManager.Instance.GetFoodSprite(foods[i]);
                foodItems[i].SetFoodItem(sprite, foods[i], "", isPicked);
                foodItems[i].transform.Find("Background/Checkmark").GetComponent<Image>().sprite = isPicked ? marksprite : unmarksprite;
                foodItems[i].Show();
            }
            else
            {
                finalFoodItems[i].Hide();
                foodItems[i].Hide();
            }
        }
    }

    public void SetOtherFoodItems(int cookbookIndex)
    {
        if (cookbookIndex == -1)
        {
            return;
        }
        List<string> otherFoods = DataManager.Instance.GetOtherFoodbyCookbook(cookbookIndex);
        if (otherFoods == null)
        {
            return;
        }

        for (int i = 0; i < otherFoodItems.Count; i++)
        {
            if (i < otherFoods.Count)
            {
                Sprite sprite = UIManager.Instance.GetFoodSprite(otherFoods[i]);
                otherFoodItems[i].SetFoodItem(sprite, otherFoods[i]);
                otherFoodItems[i].Show();
            }
            else
            {
                otherFoodItems[i].Hide();
            }
        }


    }

    //設定詳細頁面
    void SetDetialPanel()
    {
        PopupPanel.Instance.PlayGo(
            () =>
            {
                detailPanelTimer.StartTimer(15.0f); //先設15秒,15秒後到下一段
            }
        );

        detailPanelTimer.onEnd += () =>
        {
            checkEndPanel.SetActive(true);
            isDotRunning = true;
            StartCoroutine(AnimateDots());
            PN532Manager.Instance.StopSerialThread();
        };

        PN532Manager.Instance.StartSerialThread("R2", CheckArduinoData);
    }

    #region Arduino設定
    public void CheckArduinoData(ArduinoMessage msg)
    {
        if (msg == null)
        {
            AudioManager.Instance.PlayAudioOnce(3);
            return;
        }
        if (string.IsNullOrEmpty(msg.nfc) || msg.nfc.Length < 20)
        {
            AudioManager.Instance.PlayAudioOnce(3);
            return;
        }
        var foodInfo = DataManager.Instance.GetFoodInfoByNFC(msg.nfc);
        if (foodInfo == null)
        {
            AudioManager.Instance.PlayAudioOnce(3);
            return;
        }

        AudioManager.Instance.PlayAudioOnce(5);

        detailPanelFood.gameObject.SetActive(true);
        detailPanelFood.sprite = UIManager.Instance.GetFoodSprite(foodInfo.name);
        SetDetialPanelInfo(foodInfo.name);
    }
    #endregion

    IEnumerator AnimateDots()
    {
        float waitTimer = Time.time;
        while (isDotRunning)
        {
            float nowT = Time.time - waitTimer;

            waitTipText.text = gotoResultText + new string('.', dotCount); // 更新文字
            dotCount = (dotCount + 1) % 4; // 讓dotCount 從 0 到 3 循環
            yield return new WaitForSeconds(0.5f); // 每 0.5 秒更新一次
            if (nowT > 4.0f)
            {
                isDotRunning = false;
                checkEndPanel.SetActive(false);
                SetFinalPanel();
            }
        }
    }

    /// <summary>
    /// 掃描後顯示資訊
    /// </summary>
    /// <param name="foodName"></param>
    void SetDetialPanelInfo(string foodName)
    {
        waitScanTipOBJ.SetActive(false);
        //TODO : 這裡要判斷這個物件是不是有完成
        AudioManager.Instance.PlayAudioOnce(5);

        FoodInfo foodInfo = DataManager.Instance.GetFoodInfo(foodName);

        //判斷是不是有掃描過
        Dictionary<string, bool> pickedFoods = GameManager.Instance.pickedFoods;
        if (pickedFoods.ContainsKey(foodName) == true)
        {
            sensorImage.sprite = pickedFoods[foodName] ? successSprite : failSprite;
        }
        else
        {
            sensorImage.sprite = successSprite;
        }

        if (foodInfo == null)
        {
            return;
        }
        localText.text = foodInfo.locate;
        foodText.text = foodInfo.name;
        seasonText.text = foodInfo.season_text;
        nutritionTips.text = foodInfo.nutritionTips;

        contractImage.SetActive(!string.IsNullOrEmpty(foodInfo.contract));

        scanFood.gameObject.SetActive(true);
        scanFood.SetFoodItem(UIManager.Instance.GetFoodSprite(foodInfo.name), foodInfo.name);
    }
    #endregion

    #region 結算
    public void SetFinalPanel()
    {
        //TODO: 還要設定季節,QRCODE,使用的在地食材
        cookbookItem.localPosition = originPos;
        scorePanel.alpha = 1.0f;
        finalPanel.alpha = 0.0f;
        scorePanel.gameObject.SetActive(true);
        finalPanel.gameObject.SetActive(false);
        detailPanel.SetActive(false);
        resultPanel.SetActive(true);

        SetFinalFoodInfo(GameManager.Instance.CurrentCookBookIndex);
        SetFinalFoodSeason();

        int rate = GameManager.Instance.Score;
        CookBookInfo cookBookInfo = GameManager.Instance.CurrentCookBookInfo;
        int season = cookBookInfo.season - 1;//GameManager.Instance.GetSeason();
        rate = math.clamp(rate, 0, rateSprites.Count - 1);
        rateImage.sprite = rateSprites[rate];
        seasonImage.sprite = seasonSprites[season];
        commonText.text = rateCommonText[rate];
        commonDescText.text = rateDescTexts[rate];

        // cookbookImage.sprite = UIManager.Instance.GetCookBookSprite(GameManager.Instance.CurrentCookBookIndex);
    }

    public void SetFinalFoodSeason()
    {
        CookBookInfo cookBookInfo = GameManager.Instance.CurrentCookBookInfo;
        if (cookBookInfo == null)
        {
            return;
        }
        for (int i = 0; i < seasonImages.Count; i++)
        {
            if (i == cookBookInfo.season - 1)
            {
                seasonImages[i].gameObject.SetActive(true);
            }
            else
            {
                seasonImages[i].gameObject.SetActive(false);
            }
        }
    }
    public void SetFinalFoodInfo(int cookbookIndex)
    {
        if (cookbookIndex == -1)
        {
            return;
        }
        List<string> foods = DataManager.Instance.GetFoodbyCookbook(cookbookIndex);
        if (foods == null)
        {
            return;
        }
        for (int i = 0; i < foodItems.Count; i++)
        {
            if (i < foods.Count)
            {
                FoodInfo foodInfo = DataManager.Instance.GetFoodInfo(foods[i]);
                Sprite sprite = UIManager.Instance.GetFoodSprite(foods[i]);
                finalFoodItems[i].SetFoodItem(sprite, foods[i], foodInfo.locate);
                foodItems[i].Show();
            }
            else
            {
                finalFoodItems[i].Hide();
                foodItems[i].Hide();
            }
        }
    }

    #endregion
}
