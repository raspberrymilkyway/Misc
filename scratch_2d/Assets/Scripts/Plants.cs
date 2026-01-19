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

    private string[] compNames = {"water", "fire", "earth", "air"};
    private string path = "plants/";

    void Start(){
        p = this;
        spawnComponents();
    }

    protected internal void spawnComponents(){
        for (int i=0; i<compNames.Length; i++){
            GameObject element = new GameObject();
            element.name = compNames[i];

            Image eimg = element.AddComponent<Image>();
            eimg.sprite = Resources.Load<Sprite>(path + compNames[i]);

            EventTrigger etrig = (EventTrigger)element.AddComponent(typeof(EventTrigger));
            element.AddComponent(typeof(PlantsMovement));

            element.transform.SetParent(componentsAnchor.transform);
            element.SetActive(true);
        }
    }

    protected internal void handleDrop(string info, float x, float y){
        RectTransform plT = plant.GetComponent<RectTransform>();
        // this if needs to change based on anchor values (where is positive, where is negative)
        // ...would this ever run into boundary issues if i use math.abs?
        if ((x > plT.rect.x && x < plT.rect.x + plT.rect.width) && (y < Math.Abs(plT.rect.y) && y > Math.Abs(plT.rect.y + plT.rect.height))){
            Debug.Log("inside plant image");
        }
    }
}