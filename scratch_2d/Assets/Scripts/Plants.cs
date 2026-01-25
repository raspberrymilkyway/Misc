using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Plants : MonoBehaviour
{
    public static Plants p;

    public GameObject componentsAnchor;
    public Image plant;
    public GameObject magic; //for transitions - poof! plant.

    private string[] compNames = {"air", "earth", "fire", "water"};
    private string path = "plants/";

    private string prevDrop = "";
    private ComponentData cd;

    void Start(){
        p = this;
        cd = new ComponentData();
        spawnComponents();
    }

    protected internal void spawnComponents(){
        for (int i=0; i<compNames.Length; i++){
            GameObject element = new GameObject();
            element.name = compNames[i];

            Image eimg = element.AddComponent<Image>();
            eimg.sprite = Resources.Load<Sprite>(path + compNames[i]);

            element.AddComponent(typeof(EventTrigger));
            PlantsMovement pm = (PlantsMovement)element.AddComponent(typeof(PlantsMovement));
            pm.setComponent(compNames[i]);

            element.transform.SetParent(componentsAnchor.transform);
            element.SetActive(true);
        }
    }

    protected internal void handleDrop(string info, float x, float y){
        RectTransform plT = plant.GetComponent<RectTransform>();
        // this might need to change based on anchor values (where is positive, where is negative)
        // ...would this ever run into boundary issues if i use math.abs?
        if ((x > plT.rect.x && x < plT.rect.x + plT.rect.width) && (y < Math.Abs(plT.rect.y) && y > Math.Abs(plT.rect.y + plT.rect.height))){
            int i = Array.IndexOf(compNames, info);
            if (prevDrop.Length == 0){
                prevDrop = info;
            }
            parseAddition(i);
        }
    }

    private void parseAddition(int i){
        //note: there is no end point for this. it just keeps going.
        //for these purposes, idc, but for actual use it may be a problem
        string grabbed = ((string[])cd.GetType().GetField(prevDrop).GetValue(cd))[i];
        swapPlants(grabbed);
        if (grabbed.Equals("emptyPot")){
            prevDrop = "";
        }
        else{
            prevDrop = compNames[i];
        }
    }

    private void swapPlants(string image){
        plant.sprite = Resources.Load<Sprite>(path + image);
    }

    public void resetAll(){
        prevDrop = "";
        swapPlants("emptyPot");
    }
}

public class ComponentData
{
    public string[] air   = {"leaf", "emptyPot", "flowerRed", "flowerPurple"};
    public string[] earth = {"emptyPot", "leaf", "flowerYellow", "flowerBlue"};
    public string[] fire  = {"flowerRed", "flowerYellow", "leaf", "emptyPot"};
    public string[] water = {"flowerPurple", "flowerBlue", "emptyPot", "leaf"};
}