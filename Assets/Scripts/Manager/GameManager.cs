using System.Collections.Generic;
using System.Linq;
using UnityCommunity.UnitySingleton;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public int currentCookBookIndex = -1;
    public int CurrentCookBookIndex
    {
        get
        {
            return currentCookBookIndex;
        }
        set
        {
            currentCookBookIndex = value;
            if (value < 0)
            {
                return;
            }
            if (currentCookBookIndex >= 0)
            {
                CurrentCookBookInfo = DataManager.Instance.cookBookInfos[currentCookBookIndex];
            }

            pickedFoods = new Dictionary<string, bool>();
            List<string> foods = DataManager.Instance.GetFoodbyCookbook(currentCookBookIndex);
            for (int i = 0; i < foods.Count; i++)
            {
                pickedFoods[foods[i]] = false;
            }
        }

    }
    public CookBookInfo CurrentCookBookInfo = null;

    private int score = 5;

    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            if (score < 0)
            {
                score = 0;
            }
            if (score > 5)
            {
                score = 5;
            }
        }
    }

    private Dictionary<string, bool> pickedFoods;
    // private List<bool> pickedFoods = new List<bool>();
    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        CurrentCookBookIndex = -1;
    }

    public void ResetStatus()
    {
        CurrentCookBookIndex = -1;
        Score = 5;
    }

    //用來判斷這個食材是否有被選擇
    public void SetPickedFoods(string foodName)
    {
        if (pickedFoods == null)
        {
            pickedFoods = new Dictionary<string, bool>();
        }
        if (pickedFoods.ContainsKey(foodName))
        {
            if (pickedFoods[foodName] == true)
            {
                Debug.LogError(foodName + " 已經完成");
                return;
            }

            Debug.LogError(foodName + " : 完成");
            pickedFoods[foodName] = true;
        }
        else
        {
            Debug.LogError("Not This Food : " + foodName);
        }
    }

    public int MaxFoodsCount()
    {
        return DataManager.Instance.GetFoodbyCookbook(currentCookBookIndex).Count;
    }


    public int GetPickedFoodsCount()
    {
        // 計算 pickedFoods 中值為 true 的項目數量
        return pickedFoods.Count(pair => pair.Value);
    }

    public bool CheckArriver()
    {
        if (GetPickedFoodsCount() == GameManager.Instance.MaxFoodsCount())
        {
            return true;
        }
        return false;
    }

    // private void SetPickedFoods()
    // {
    //     pickedFoods.Clear();
    //     for (int i = 0; i < CurrentCookBookInfo.foods.Count; i++)
    //     {
    //         pickedFoods.Add(false);
    //     }
    // }
}
