using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Stage2Panel : MonoBehaviour
{
    public GameObject descObj;
    public List<GameObject> descItem;

    public Button nextDescBtn;

    void Start()
    {
        Init();
    }
    public void Init()
    {
        descObj.SetActive(true);
        foreach (var item in descItem)
        {
            item.SetActive(false);
        }
        descItem.First().SetActive(true);
        nextDescBtn.onClick.AddListener(ShowNextDesc);
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
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
