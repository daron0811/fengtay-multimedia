using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityCommunity.UnitySingleton;

public class DataManager : MonoSingleton<DataManager>
{
    public TextAsset foodAsset;
    public List<FoodInfo> foodInfos;
    public Dictionary<int, List<FoodInfo>> foodBySeason;
    public TextAsset cookBookAsset;
    public List<CookBookInfo> cookBookInfos;

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

        if(cookBookAsset!=null)
        {
            cookBookInfos = JsonConvert.DeserializeObject<List<CookBookInfo>>(cookBookAsset.text);
        }
    }

    public List<FoodInfo> GetFoodBySeason(int season)
    {
        if(foodBySeason==null)
        {
            return null;
        }
        if (foodBySeason.ContainsKey(season))
        {
            return foodBySeason[season];
        }
        return null;
    }

    public CookBookInfo GetCookBookInfo(int index)
    {
        if (cookBookInfos == null)
        {
            return null;
        }
        return cookBookInfos[index];
    }

    public bool haveFoodbyCookbook(int index, string food,out int foodIndex)
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
}

[System.Serializable]
public class FoodInfo
{
    public int id;// { get; set; }
    public string name;// { get; set; }
    public string locate; //{ get; set; }
    public string food; //{ get; set; }
    public string season; //{ get; set; }
}

[System.Serializable]
public class CookBookInfo
{
    public int id;// { get; set; }
    public string name;// { get; set; }
    public string Season; //{ get; set; }
    public string food1; //{ get; set; }
    public string food2; //{ get; set; }
    public string food3; //{ get; set; }
    public string food4; //{ get; set; }
}
