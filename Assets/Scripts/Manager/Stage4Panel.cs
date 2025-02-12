using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    public TextMeshProUGUI localText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI seasonText;
    public TextMeshProUGUI nutritionTips;
    public Image detailPanelFood;

    public FoodItem scanFood;

    [Header("最後結算")]
    public GameObject resultPanel;
    public CanvasGroup scorePanel;
    public CanvasGroup finalPanel;
    public Button gotoEndBtn;

    public Image rateImage;
    public List<Sprite> rateSprites;

    public Image seasonImage;
    public List<Sprite> seasonSprites;

    public Image cookbookImage; //圖示

    public Transform cookbookItem;

    public Vector3 originPos = new Vector3(-379.0f, -47.0f, 0.0f);
    public Vector3 resultPos = new Vector3(-379.0f, 55.0f, 0.0f);

    public TextMeshProUGUI finalCookbookNameText;
    public Image finalCookbookImage;
    public List<FoodItem> finalFoodItems;
    public List<Image> seasonImages;

    public Button returnTitleBtn;
    public Button playAgainBtn;

    private bool isInit = false;
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
        descObj.SetActive(true);
        foreach (var item in descItem)
        {
            item.SetActive(false);
        }
        descItem.First().SetActive(true);
        nextDescBtn.onClick.AddListener(ShowNextDesc);

        detailPanel.SetActive(false);
        resultPanel.SetActive(false);

        SetCookBookInfo();

        scanFood.gameObject.SetActive(false);
        scorePanel.gameObject.SetActive(true);
        finalPanel.gameObject.SetActive(false);

        localText.text = "";
        foodText.text = "";
        seasonText.text = "";
        nutritionTips.text = "";

        detailPanelFood.gameObject.SetActive(false);


        foreach (var item in otherFoodItems)
        {
            item.Hide();
        }
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
        // ResetStatus();
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
            detailPanel.SetActive(true);
            SetDetialPanel();
        }
    }

    #region 設定詳細頁面
    public void SetCookBookInfo()
    {
        SetFoodItems(GameManager.Instance.CurrentCookBookIndex);
        cookbookNameText.text = GameManager.Instance.CurrentCookBookInfo.name;
        finalCookbookNameText.text = GameManager.Instance.CurrentCookBookInfo.name;
        finalCookbookImage.sprite = UIManager.Instance.GetCookBookSprite(GameManager.Instance.CurrentCookBookInfo.icon);
    }

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

        for (int i = 0; i < foodItems.Count; i++)
        {
            if (i < foods.Count)
            {
                Sprite sprite = UIManager.Instance.GetFoodSprite(foods[i]);
                foodItems[i].SetFoodItem(sprite, foods[i]);
                foodItems[i].Show();
            }
            else
            {
                finalFoodItems[i].Hide();
                foodItems[i].Hide();
            }
        }
    }

    //設定詳細頁面
    void SetDetialPanel()
    {
        PopupPanel.Instance.PlayReadyPanel(
            () =>
            {
                detailPanelTimer.StartTimer(10.0f); //先設10秒
            }
        );


        detailPanelTimer.onEnd += () =>
        {
            SetFinalPanel();
        };
    }

    /// <summary>
    /// 掃描後顯示資訊
    /// </summary>
    /// <param name="foodName"></param>
    void SetDetialPanelInfo(string foodName)
    {
        //TODO : 這裡要判斷這個物件是不是有完成
        AudioManager.Instance.PlayAudioOnce(5);

        FoodInfo foodInfo = DataManager.Instance.GetFoodInfo(foodName);
        if (foodInfo == null)
        {
            return;
        }
        localText.text = foodInfo.locate;
        foodText.text = foodInfo.name;
        seasonText.text = foodInfo.season_text;
        nutritionTips.text = foodInfo.nutritionTips;

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
        int score = 0;//= GameManager.Instance.GetScore();
        int rate = 0;//GameManager.Instance.GetRate();
        int season = 0;//GameManager.Instance.GetSeason();

        rateImage.sprite = rateSprites[rate];
        seasonImage.sprite = seasonSprites[season];
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
