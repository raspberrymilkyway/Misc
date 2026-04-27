using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;

public class SpotTheDifference : MonoBehaviour
{
    public GameObject leftAnchor;
    public GameObject rightAnchor;
    public string csvName;

    private List<DiffCSV> images;
    private ColorBlock cbTrp;
    private ColorBlock cbNor;

    // make sure your anchoring system is the same across files and scenes
    
    void Start()
    {
        images = new List<DiffCSV>{};
        cbTrp = new ColorBlock();
        UnityEngine.Color opa = new UnityEngine.Color(1f, 1f, 1f, 1f);
        UnityEngine.Color found = new UnityEngine.Color(0.5f, 0.5f, 0.5f, 0.5f);
        cbTrp.disabledColor = found;
        cbTrp.colorMultiplier = 1f;
        cbNor.normalColor = opa;
        cbNor.highlightedColor = opa;
        cbNor.selectedColor = opa;
        cbNor.pressedColor = opa;
        cbNor.disabledColor = found;
        cbNor.colorMultiplier = 1f;

        readCSV();
        spawnImages();
    }

    //string imagePath, bool presentLeft, bool presentRight, 
    // float leftX, float leftY, float sizeX, float sizeY, 
    // bool needsInvisibleButton
    private void readCSV(){
        TextAsset ta = Resources.Load<TextAsset>("spot/" + csvName);
        string[] lines = ta.text.Split("\n");

        for (int i=1; i<lines.Length; i++){
            string[] ele = lines[i].Split(", ");

            //convert
            bool left = bool.Parse(ele[1]);
            bool right = bool.Parse(ele[2]);
            bool button = bool.Parse(ele[7]);
            float xcoor = float.Parse(ele[3]);
            float ycoor = float.Parse(ele[4]);
            float xsize = float.Parse(ele[5]);
            float ysize = float.Parse(ele[6]);

            DiffCSV d = new DiffCSV(ele[0], left, right, (xcoor, ycoor), (xsize, ysize), button);
            images.Add(d);
        }
    }

    private void spawnImages(){
        for (int i=0; i<images.Count; i++){
            GameObject go = new GameObject();
            go.name = images[i].name + i;
            Image img = go.AddComponent<Image>();
            img.sprite = Resources.Load<Sprite>(images[i].imagePath);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(images[i].sizes.x, images[i].sizes.y);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0, 0);

            if (images[i].presentOnLeft && images[i].presentOnRight){
                GameObject rgo = GameObjectUtility.DuplicateGameObject(go);
                rgo.transform.SetParent(rightAnchor.transform);
                go.transform.SetParent(leftAnchor.transform);
                
                rt.localPosition = new Vector2(images[i].coordinates.x, images[i].coordinates.y);
                rgo.GetComponent<RectTransform>().localPosition = new Vector2(images[i].coordinates.x, images[i].coordinates.y);
            }
            else if (images[i].presentOnLeft){
                go.transform.SetParent(leftAnchor.transform);
                rt.localPosition = new Vector2(images[i].coordinates.x, images[i].coordinates.y);
                Button b = go.AddComponent<Button>();
                b.onClick.AddListener(() => clickDifference(go));
                b.colors = cbNor;
                if (images[i].needsInvisibleButton){
                    GameObject rgo = new GameObject();
                    rgo.name = images[i].name + "Inv" + i;
                    RectTransform rrt = rgo.AddComponent<RectTransform>();
                    rrt.transform.SetParent(rightAnchor.transform);
                    rrt.sizeDelta = rt.sizeDelta;
                    rrt.anchorMin = new Vector2(0, 0);
                    rrt.anchorMax = new Vector2(0, 0);
                    rrt.pivot = new Vector2(0, 0);
                    rrt.localPosition = rt.localPosition;
                    Image ri = rgo.AddComponent<Image>();
                    // ri.color = new UnityEngine.Color(0,0,0,0);
                    b = rgo.AddComponent<Button>();
                    b.onClick.AddListener(() => clickDifference(rgo));
                    b.colors = cbTrp;
                }
            }
            else if (images[i].presentOnRight){
                go.transform.SetParent(rightAnchor.transform);
                rt.localPosition = new Vector2(images[i].coordinates.x, images[i].coordinates.y);
                Button b = go.AddComponent<Button>();
                b.onClick.AddListener(() => clickDifference(go));
                b.colors = cbNor;
                if (images[i].needsInvisibleButton){
                    GameObject lgo = new GameObject();
                    lgo.name = images[i].name + "Inv" + i;
                    RectTransform lrt = lgo.AddComponent<RectTransform>();
                    lrt.transform.SetParent(leftAnchor.transform);
                    lrt.sizeDelta = rt.sizeDelta;
                    lrt.anchorMin = new Vector2(0, 0);
                    lrt.anchorMax = new Vector2(0, 0);
                    lrt.pivot = new Vector2(0, 0);
                    lrt.localPosition = rt.localPosition;
                    Image li = lgo.AddComponent<Image>();
                    // li.color = new UnityEngine.Color(0,0,0,0);
                    b = lgo.AddComponent<Button>();
                    b.onClick.AddListener(() => clickDifference(lgo));
                    b.colors = cbTrp;
                }
            }
        }
    }

    public void clickDifference(GameObject go){
        Debug.Log(go.name + " clicked");
        go.GetComponent<Button>().interactable = false;
    }
}

public class DiffCSV
{
    string path;
    string eman;
    bool left;
    bool right;
    (float x, float y) coor;
    (float x, float y) size;
    bool addInvisibleButton;

    public DiffCSV(string p, bool presentLeft, bool presentRight, (float, float) c, (float, float) s, bool button){
        imagePath = p;
        presentOnLeft = presentLeft;
        presentOnRight = presentRight;
        coordinates = c;
        sizes = s;
        needsInvisibleButton = button;
        name = imagePath.Substring(imagePath.IndexOf("/")+1); //edit if different - perhaps look for last "/"?
    }

    protected internal string imagePath{
        get => path;
        set => path = value;
    }
    protected internal string name{
        get => eman;
        set => eman = value;
    }
    protected internal bool presentOnLeft{
        get => left;
        set => left = value;
    }
    protected internal bool presentOnRight{
        get => right;
        set => right = value;
    }
    protected internal (float x, float y) coordinates{
        get => coor;
        set => coor = value;
    }
    protected internal (float x, float y) sizes{
        get => size;
        set => size = value;
    }
    protected internal bool needsInvisibleButton{
        get => addInvisibleButton;
        set => addInvisibleButton = value;
    }
}