using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class UISelectMap : MonoBehaviour
{
    public ToggleGroup toggleGroup;
    public List<Texture> images;
    public RawImage bg;

    public DataManager dataManager;

    public GameObject foodGroup;

    public Dictionary<string, GameObject> foodImage;

    public int currentSeason = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foodImage = new Dictionary<string, GameObject>();
        foreach (Transform child in foodGroup.transform)
        {
            string locateName = child.name.Split('-')[0]; // 儲存地點
            foodImage[locateName] = child.gameObject;
        }
        SetImage(0);
    }

    // Update is called once per frame
    void Update()
    {
        Toggle activeToggle = toggleGroup.ActiveToggles().FirstOrDefault();
        if (activeToggle != null)
        {
            int index = 0;
            if (int.TryParse(activeToggle.name, out index))
            {
                if (currentSeason != index)
                {
                    currentSeason = index;
                    SetImage(index);
                }
                if (index >= 0 && index < images.Count)
                {
                    bg.texture = images[index];
                }
            }
        }
    }

    void SetImage(int index)
    {
        List<FoodInfo> foodInfos = dataManager.GetFoodBySeason(currentSeason + 1);

        foreach (KeyValuePair<string, GameObject> entry in foodImage)
        {
            // Debug.LogError(entry.Key);
            entry.Value.gameObject.SetActive(false);
            if (foodInfos.Find(x => x.locate == entry.Key) != null) // 用地點去查詢食物
            {
                entry.Value.gameObject.SetActive(true);
            }
        }


        // foreach (var foodInfo in foodInfos)
        // {
        //     GameObject food = foodImage[foodInfo.locate];
        //     food.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + foodInfo.food);
        // }

        if (index >= 0 && index < images.Count)
        {
            bg.texture = images[index];
        }
    }
}
