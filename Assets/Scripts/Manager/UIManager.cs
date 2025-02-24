using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class UIManager : MonoSingleton<UIManager>
{
    public GameObject stage1_OBJ;
    public GameObject stage2_OBJ;
    public GameObject stage3_OBJ;
    public GameObject stage4_OBJ;
    public GameObject stage5_OBJ;

    public enum UIState
    {
        Stage_1,
        Stage_2,
        Stage_3,
        Stage_4,
        Stage_5
    }

    private UIState currentState = UIState.Stage_1;

    void Start()
    {
        SetState(UIState.Stage_1, false);
        LoadSpriteFromPath();
    }

    public void SetState(UIState newState, bool cutScene = true)
    {
        Debug.LogError("SetState: " + newState);
        currentState = newState;
        if (cutScene == true)
        {
            PopupPanel.Instance.PlayCutScene(() =>
            {
                UpdateUI();
            });
            // Invoke("UpdateUI", 0.5f); //時間到的長度
        }
        else
        {
            UpdateUI();
        }


    }

    private void UpdateUI()
    {
        stage1_OBJ.SetActive(currentState == UIState.Stage_1);
        stage2_OBJ.SetActive(currentState == UIState.Stage_2);
        stage3_OBJ.SetActive(currentState == UIState.Stage_3);
        stage4_OBJ.SetActive(currentState == UIState.Stage_4);
        stage5_OBJ.SetActive(currentState == UIState.Stage_5);
    }

    public void OnIntroComplete()
    {
        SetState(UIState.Stage_2);
    }

    public void OnStartGame()
    {
        SetState(UIState.Stage_3);
    }

    public void OnPauseGame()
    {
        SetState(UIState.Stage_4);
    }

    public void OnResumeGame()
    {
        SetState(UIState.Stage_3);
    }

    public void OnGameOver()
    {
        SetState(UIState.Stage_5);
    }

    public List<Sprite> sprites;
    public List<Sprite> cookbookSprite;

    public Sprite GetFoodSprite(string name)
    {
        // string resetName = name.Split("-")[1];
        return sprites.FirstOrDefault(x => x.name == name);
    }

    public Sprite GetCookBookSprite(string name)
    {
        return cookbookSprite.FirstOrDefault(x => x.name == name);
    }

    public void LoadSpriteFromPath()
    {
        sprites = new List<Sprite>();
        sprites = Resources.LoadAll<Sprite>("Icon/Food").ToList();
        foreach (var sprite in sprites)
        {
            string resetName = sprite.name;
            if (sprite.name.Split("-").Length > 1)
            {
                resetName = sprite.name.Split("-")[1]; // 因為有共用的材料
            }
            sprite.name = resetName;
        }


        var otherFood = new List<Sprite>();
        otherFood = Resources.LoadAll<Sprite>("Icon/OtherFood").ToList();
        sprites.AddRange(otherFood);

        cookbookSprite = new List<Sprite>();
        cookbookSprite = Resources.LoadAll<Sprite>("Icon/CookBook").ToList();

        // string path = Application.dataPath+"/Resources/Textures";
        // DirectoryInfo dir = new DirectoryInfo(path);
        // FileInfo[] info = dir.GetFiles("*.png");
        // foreach (FileInfo f in info)
        // {
        //     byte[] fileData = File.ReadAllBytes(f.FullName);
        //     Texture2D tex = new Texture2D(2, 2);
        //     tex.LoadImage(fileData);
        //     Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        //     sprites.Add(sprite);
        // }


        // // string path = Application.dataPath + "/Resources/Textures/food_01.png";
        // string path = Application.streamingAssetsPath+"/icon/food_01.png";
        // byte[] fileData = File.ReadAllBytes(path);
        // Texture2D tex = new Texture2D(2, 2);
        // tex.LoadImage(fileData);
        // sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
    }
}
