using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpotTheDifference : MonoBehaviour
{
    public GameObject leftAnchor;
    public GameObject rightAnchor;
    public string csvName;

    private List<DiffCSV> images;

    // make sure your anchoring system is the same across files and scenes
    
    void Start()
    {
        images = new List<DiffCSV>{};
        readCSV();
        //spawnImages();
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
            //resize
            //set parent
            //move (is movement absolute or within parent?)
        }
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
        name = imagePath.Substring(imagePath.IndexOf("/")+1);
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