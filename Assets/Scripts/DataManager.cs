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

    void Start()
    {
        string foodAssetTextPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Food.json");
        string foodAssetText = System.IO.File.ReadAllText(foodAssetTextPath);
        if (foodAsset != null)
        {
            // foodInfos = JsonConvert.DeserializeObject<List<FoodInfo>>(foodAsset.text);
            foodInfos = JsonConvert.DeserializeObject<List<FoodInfo>>(foodAssetText);
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
                if (foodInfo.nfc == null || string.IsNullOrEmpty(foodInfo.nfc))
                {
                    Debug.LogWarning("foodInfo nfc is empty");
                    continue;
                }
                foodInfo.nfcs = new List<string>();
                foodInfo.nfcs.AddRange(foodInfo.nfc.Split(','));
            }
        }


        string cookBookAssetTextPath = System.IO.Path.Combine(Application.streamingAssetsPath, "CookBook.json");
        string cookBookAssetText = System.IO.File.ReadAllText(cookBookAssetTextPath);
        if (cookBookAsset != null)
        {
            // cookBookInfos = JsonConvert.DeserializeObject<List<CookBookInfo>>(cookBookAsset.text);
            cookBookInfos = JsonConvert.DeserializeObject<List<CookBookInfo>>(cookBookAssetText);

            foreach (var cookBookInfo in cookBookInfos)
            {
                if (string.IsNullOrEmpty(cookBookInfo.step))
                {
                    Debug.LogWarning("CookBookInfo step is empty");
                    continue;
                }
                cookBookInfo.steps = new List<string>();
                cookBookInfo.steps.AddRange(cookBookInfo.step.Split('\n'));
                cookBookInfo.triggerSteps = new List<string>();
                if (string.IsNullOrEmpty(cookBookInfo.triggerStep) == false)
                {
                    cookBookInfo.triggerSteps.AddRange(cookBookInfo.triggerStep.Split('\n'));
                    for (int i = 0; i < cookBookInfo.triggerSteps.Count; i++)
                    {
                        cookBookInfo.triggerSteps[i] = cookBookInfo.triggerSteps[i].Replace("\n", "").Replace("\r", ""); //清除多餘的換行符號
                    }
                }
                if (cookBookInfo.nfc == null || string.IsNullOrEmpty(cookBookInfo.nfc))
                {
                    Debug.LogWarning("CookBookInfo nfc is empty");
                    continue;
                }
                cookBookInfo.nfcs = new List<string>();
                cookBookInfo.nfcs.AddRange(cookBookInfo.nfc.Split(','));
            }
        }

        string configAssetTextPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Config.json");
        string configAssetText = System.IO.File.ReadAllText(configAssetTextPath);
        //設定檔
        if (configAsset != null)
        {
            // configDatas = JsonConvert.DeserializeObject<Dictionary<string, object>>(configAsset.text);
            configDatas = JsonConvert.DeserializeObject<Dictionary<string, object>>(configAssetText);
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
        return item;
    }

    public FoodInfo GetFoodInfoByNFC(string nfc)
    {
        if (string.IsNullOrEmpty(nfc))
        {
            Debug.LogError("NFC is null or empty");
            return null;
        }

        var item = foodInfos.Find(x => x.nfcs.Contains(nfc));
        if (item == null)
        {
            Debug.LogError("Not Find Food Info " + nfc);
            return null;
        }
        // var item = foodInfos.Find(x => x.nfc == nfc);
        // if (item == null)
        // {
        //     Debug.LogError("Not Find Food Info " + name);
        //     return null;
        // }
        return item;
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

    public CookBookInfo GetCookBookInfoByNFC(string nfc)
    {
        if (cookBookInfos == null)
        {
            return null;
        }
        var item = cookBookInfos.Find(x => x.nfcs.Contains(nfc));
        // var item = cookBookInfos.Find(x => x.nfc == nfc);
        if (item == null)
        {
            return null;
        }
        return item;
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
    public string contract;//是否契作
    public string nfc;
    public List<string> nfcs;
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
    public string triggerStep;
    public string otherfood1;
    public string otherfood2;
    public List<string> steps;
    public List<string> triggerSteps;
    public string nfc;
    public List<string> nfcs;
}

[System.Serializable]
public class ConfigInfo
{
    public string ArduinoPortName { get; set; }
    public int ArduinoBaudRate { get; set; }
    public bool MouseMode { get; set; } //將 sale 改為 MouseMode
}