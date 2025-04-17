using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class Stage2Panel : MonoSingleton<Stage2Panel>
{

    public GameObject descObj;
    public List<GameObject> descItem;
    public Button nextDescBtn;
    private List<FoodItem> foodItems; //從子物件下取得

    public GameObject finishPanel;
    public Button finishBtn;
    private List<FoodItem> resultFoodItems;
    public CountdownTimer countdownTimer;
    public TextMeshProUGUI cookbookNameText; //食譜名稱

    public List<PickItemToBucket> dragFoodsOnMap;

    public Image btnImage;
    public Sprite normalSprite;
    public Sprite goSprite;

    public RectTransform buckTransform;

    public int pickCountForScore = 0; //計算成績用，依照目前要選的是滿分，有成功選擇+1，如果沒選到就依照這個做扣分依據

    public bool isInit = false;

    public float rotateAmount = 15f; // 旋轉角度
    public float duration = 0.2f; // 旋轉時間
    void Start()
    {
        Init();
    }

    public void Init()
    {
        if (isInit)
        {
            return;
        }
        nextDescBtn.onClick.AddListener(ShowNextDesc);
        foodItems = transform.Find("PickupFoods").GetComponentsInChildren<FoodItem>().ToList();
        resultFoodItems = finishPanel.transform.Find("PickupFoods").GetComponentsInChildren<FoodItem>().ToList();
        dragFoodsOnMap = transform.Find("FoodGroup").GetComponentsInChildren<PickItemToBucket>().ToList();

        countdownTimer.onEnd += () =>
        {
            PopupPanel.Instance.PlayTimeUp(() =>
            {
                finishPanel.gameObject.SetActive(true);
                // UIManager.Instance.SetState(UIManager.UIState.Stage_3);
            });

            //Todo : 計算成績
            foreach (var item in foodItems)
            {
                if (item.gameObject.activeSelf == false)
                {
                    continue;
                }
                if (item.toggle.isOn == false)
                {
                    pickCountForScore++;
                }
            }

            GameManager.Instance.Score -= pickCountForScore;
        };

        finishBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.SetState(UIManager.UIState.Stage_3);
        });

        ResetStatus();
        isInit = true;
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

    public void ResetStatus()
    {
        pickCountForScore = 0;
        finishPanel.SetActive(false);
        descObj.SetActive(true);
        foreach (var item in descItem)
        {
            item.SetActive(false);
        }
        descItem.First().SetActive(true);
        btnImage.sprite = normalSprite;
        PickItemToBucket();
        SetCookBookInfo();

        for (int i = 0; i < buckTransform.transform.childCount; i++)
        {
            Transform childTran = buckTransform.transform.GetChild(i);
            childTran.gameObject.SetActive(false);
        }
    }


    void PickItemToBucket()
    {
        if (dragFoodsOnMap == null || dragFoodsOnMap.Count == 0)
        {
            return;
        }

        foreach (PickItemToBucket item in dragFoodsOnMap)
        {
            item.onDragItem += () =>
            {
                Debug.LogWarning(item.name + " dropped on bucket");
                string fruitName = item.name.Split('-')[1];
                OnTriggerFoodItem(fruitName);
            };

        }
    }

    public void SetCookBookInfo()
    {
        if (GameManager.Instance.CurrentCookBookIndex < 0)
        {
            Debug.LogWarning("CurrentCookBookIndex < 0");
            cookbookNameText.text = "未知";
            return;
        }
        SetFoodItems(GameManager.Instance.CurrentCookBookIndex);
        cookbookNameText.text = GameManager.Instance.CurrentCookBookInfo.name;
    }

    public void SetFoodItems(int cookbookIndex)
    {
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
                Debug.LogError(sprite.name + foods[i]);
                foodItems[i].SetFoodItem(sprite, foods[i], "", true);
                foodItems[i].SetGary();
                foodItems[i].Show();
            }
            else
            {
                foodItems[i].Hide();
            }
        }

        for (int i = 0; i < resultFoodItems.Count; i++)
        {
            if (i < foods.Count)
            {
                Sprite sprite = UIManager.Instance.GetFoodSprite(foods[i]);
                Debug.LogError(sprite.name + foods[i]);
                resultFoodItems[i].SetFoodItem(sprite, foods[i]);
                resultFoodItems[i].SetGary();
                resultFoodItems[i].Show();
            }
            else
            {
                resultFoodItems[i].Hide();
            }
        }
    }

    private void ShowNextDesc()
    {
        AudioManager.Instance.PlayAudioOnce(1);
        var currentActive = descItem.FirstOrDefault(x => x.activeSelf);
        var currentIndex = descItem.IndexOf(currentActive);
        currentActive.SetActive(false);
        if (currentIndex + 1 < descItem.Count)
        {
            descItem[currentIndex + 1].SetActive(true);
            if (currentIndex + 1 == descItem.Count - 1)
            {
                btnImage.sprite = goSprite;
            }
            else
            {
                btnImage.sprite = normalSprite;
            }
        }
        else
        {
            countdownTimer.gameObject.SetActive(false);
            descObj.SetActive(false);
            PopupPanel.Instance.PlayReadyPanel(() =>
            {
                countdownTimer.gameObject.SetActive(true);
                countdownTimer.StartTimer(30.0f);
                AudioManager.Instance.PlayBGM(1);
            });
        }
    }

    public void OnTriggerFoodItem(string foodName)
    {
        int foodIndex = -1;
        if (DataManager.Instance.haveFoodbyCookbook(GameManager.Instance.CurrentCookBookIndex, foodName, out foodIndex))
        {
            //成功
            foodItems[foodIndex - 1].Checked(foodName);
            foodItems[foodIndex - 1].SetGary(false);
            resultFoodItems[foodIndex - 1].Checked(foodName);
            resultFoodItems[foodIndex - 1].SetGary(false);
            AudioManager.Instance.PlayAudioOnce(2);
            GameManager.Instance.SetPickedFoods(foodName);
            OnSuccessPlayBucketEffect();
            if (GameManager.Instance.CheckArriver() == true)
            {
                countdownTimer.StopTimer();
                PopupPanel.Instance.PlayGoodJob(
                    () =>
                    {
                        finishPanel.gameObject.SetActive(true);
                        AudioManager.Instance.PlayAudioOnce(10);
                    }
                );
            }
        }
        else
        {
            //失敗
            AudioManager.Instance.PlayAudioOnce(3);
            buckTransform.DORotate(new Vector3(0, 0, rotateAmount), duration).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                buckTransform.DORotate(new Vector3(0, 0, -rotateAmount), duration).SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        buckTransform.DORotate(Vector3.zero, duration).SetEase(Ease.InOutSine);
                    });
            });
        }
    }
    // public DOTweenAnimation dOTweenAnimation1;
    public async void OnSuccessPlayBucketEffect()
    {
        if (buckTransform != null)
        {
            // dOTweenAnimation1.DOPlay();
            buckTransform.DOScale(new Vector3(1.2f, 0.9f, 1.0f), 0.3f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo);
            for (int i = 0; i < buckTransform.transform.childCount; i++)
            {
                Transform childTran = buckTransform.transform.GetChild(i);
                childTran.gameObject.SetActive(false);
                childTran.gameObject.SetActive(true);
                await UniTask.Delay(UnityEngine.Random.Range(0, 500));
            }
        }
    }
}
