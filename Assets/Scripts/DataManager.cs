using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityCommunity.UnitySingleton;
using System;
using UnityEngine.InputSystem;
using System.Drawing;

public class DataManager : MonoSingleton<DataManager>
{
    public TextAsset foodAsset;
    public List<FoodInfo> foodInfos;
    public Dictionary<int, List<FoodInfo>> foodBySeason;
    public TextAsset cookBookAsset;
    public List<CookBookInfo> cookBookInfos;
    public TextAsset configAsset;
    public Dictionary<string, object> configDatas;

    // void Awake(){
    //     string path = Application.streamingAssetsPath + "/food.json";
    //     if (System.IO.File.Exists(path))
    //     {
    //         string jsonContent = System.IO.File.ReadAllText(path);
    //         foodInfos = JsonConvert.DeserializeObject<List<FoodInfo>>(foodAsset.text);
    //     }
    //     else
    //     {
    //         Debug.LogError("Food JSON file not found at: " + path);
    //     }
    // }

    #region 載入資料
    // public GameObject 
    void Start()
    {
        if (foodAsset != null)
        {
            foodInfos = JsonConvert.DeserializeObject<List<FoodInfo>>(foodAsset.text);
            foodBySeason = new Dictionary<int, List<FoodInfo>>();

            foreach (var foodInfo in foodInfos)
            {
                foreach (char seasonChar in foodInfo.season)
                {
                    int season = int.Parse(seasonChar.ToString());
                    if (!foodBySeason.ContainsKey(season))
                    {
                        foodBySeason[season] = new List<FoodInfo>();
                    }
                    foodBySeason[season].Add(foodInfo);
                }
            }
        }

        if (cookBookAsset != null)
        {
            cookBookInfos = JsonConvert.DeserializeObject<List<CookBookInfo>>(cookBookAsset.text);

            foreach (var cookBookInfo in cookBookInfos)
            {
                if (string.IsNullOrEmpty(cookBookInfo.step))
                {
                    Debug.LogWarning("CookBookInfo step is empty");
                    continue;
                }
                cookBookInfo.steps = new List<string>();
                cookBookInfo.steps.AddRange(cookBookInfo.step.Split('\n'));
            }
        }

        //設定檔
        if (configAsset != null)
        {
            configDatas = JsonConvert.DeserializeObject<Dictionary<string, object>>(configAsset.text);
            Debug.LogWarning(configDatas.Count);
        }
    }

    #endregion

    #region 設定檔
    public void GetConfigData<T>(string key, out T value)
    {
        value = default(T);
        if (configDatas == null)
        {
            return;
        }
        if (configDatas.ContainsKey(key))
        {
            try
            {
                value = (T)Convert.ChangeType(configDatas[key], typeof(T));
            }
            catch (InvalidCastException)
            {
                Debug.LogError($"Failed to cast config data for key: {key} to type: {typeof(T)}");
            }
        }
    }

    // Example usage:
    // int someIntValue;
    // GetConfigData<int>("someIntKey", out someIntValue);
    // Debug.Log("Config value for 'someIntKey': " + someIntValue);

    // string someStringValue;
    // GetConfigData<string>("someStringKey", out someStringValue);
    // Debug.Log("Config value for 'someStringKey': " + someStringValue);

    #endregion

    #region 食材資料
    public List<FoodInfo> GetFoodBySeason(int season)
    {
        if (foodBySeason == null)
        {
            return null;
        }
        if (foodBySeason.ContainsKey(season))
        {
            return foodBySeason[season];
        }
        return null;
    }

    public FoodInfo GetFoodInfo(string name)
    {
        var item = foodInfos.Find(x => x.name == name);
        if (item == null)
        {
            Debug.LogError("Not Find Food Info " + name);
            return null;
        }
        return foodInfos.Find(x => x.name == name);
    }
    #endregion

    #region 食譜資料
    public CookBookInfo GetCookBookInfo(int index)
    {
        if (cookBookInfos == null)
        {
            return null;
        }
        if (cookBookInfos[index] == null)
        {
            return null;
        }
        // if (cookBookInfos[index].steps == null)
        // {
        //     cookBookInfos[index].steps = new List<string>();
        //     cookBookInfos[index].steps.AddRange(cookBookInfos[index].step.Split('\n'));
        // }
        return cookBookInfos[index];
    }

    /// <summary>
    /// 取得這個食譜的食材
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public List<string> GetFoodbyCookbook(int index)
    {
        List<string> foods = new List<string>();
        CookBookInfo cookBookInfo = GetCookBookInfo(index);
        if (cookBookInfo == null)
        {
            return foods;
        }
        if (!string.IsNullOrEmpty(cookBookInfo.food1))
        {
            foods.Add(cookBookInfo.food1);
        }
        if (!string.IsNullOrEmpty(cookBookInfo.food2))
        {
            foods.Add(cookBookInfo.food2);
        }
        if (!string.IsNullOrEmpty(cookBookInfo.food3))
        {
            foods.Add(cookBookInfo.food3);
        }
        if (!string.IsNullOrEmpty(cookBookInfo.food4))
        {
            foods.Add(cookBookInfo.food4);
        }
        return foods;
    }

    public List<string> GetOtherFoodbyCookbook(int index)
    {
        List<string> foods = new List<string>();
        CookBookInfo cookBookInfo = GetCookBookInfo(index);
        if (cookBookInfo == null)
        {
            return foods;
        }
        if (!string.IsNullOrEmpty(cookBookInfo.otherfood1))
        {
            foods.Add(cookBookInfo.otherfood1);
        }
        if (!string.IsNullOrEmpty(cookBookInfo.otherfood2))
        {
            foods.Add(cookBookInfo.otherfood2);
        }
        return foods;
    }

    //驗證是不是有這個食材
    public bool haveFoodbyCookbook(int index, string food, out int foodIndex)
    {
        foodIndex = -1;
        CookBookInfo cookBookInfo = GetCookBookInfo(index);
        if (cookBookInfo == null)
        {
            return false;
        }
        if (cookBookInfo.food1 == food)
        {
            foodIndex = 1;
            return true;
        }
        if (cookBookInfo.food2 == food)
        {
            foodIndex = 2;
            return true;
        }
        if (cookBookInfo.food3 == food)
        {
            foodIndex = 3;
            return true;
        }
        if (cookBookInfo.food4 == food)
        {
            foodIndex = 4;
            return true;
        }
        return false;
    }
    #endregion
}

[System.Serializable]
public class FoodInfo
{
    public int id;// { get; set; }
    public string name;// { get; set; }
    public string locate; //{ get; set; }
    public string season; //{ get; set; }
    public string season_text; //{ get; set; }
    public string nutritionTips; //{ get; set; }
}

[System.Serializable]
public class CookBookInfo
{
    public int id;// { get; set; }
    public string name;// { get; set; }
    public int season; //{ get; set; }
    public string icon;
    public string food1; //{ get; set; }
    public string food2; //{ get; set; }
    public string food3; //{ get; set; }
    public string food4; //{ get; set; }
    public string step;
    public string otherfood1;
    public string otherfood2;
    public List<string> steps;
}
