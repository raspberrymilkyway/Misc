using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Tangrams : MonoBehaviour
{
    // to do: implement more shape spawn
    // actually allow user to drag shapes
    // check to see if shapes are within spot
    // check to see if shapes overlap
    // grab layout group and change so the image sizes are dynamic?
    //    or just restructure the damn thing
    //    ...maybe make a custom layout group? this doesn't work
    public GameObject outline;
    public GameObject shapeAnchor;

    protected internal string outlineShape = "rectangle";
    protected internal List<ShapeData> sd = new List<ShapeData>();

    void Start(){
        getOutlineShape();
        loadShapeData();
        spawnShapes();
    }

    private void getOutlineShape(){
        //grab outlineShape from calling file
    }

    private void loadShapeData(){
        // shape, xSize, ySize
        TextAsset ac = Resources.Load<TextAsset>("tangram/shapeData/" + outlineShape);
        string[] lines = ac.text.Split("\n");
        for (int i=0; i<lines.Length; i++){
            int j = lines[i].IndexOf(", ");
            int k = lines[i].IndexOf(", ", j+1);

            string name = lines[i].Substring(0, j);
            float x = float.Parse(lines[i].Substring(j+2, k-j-2));
            float y = float.Parse(lines[i].Substring(k+2));

            sd.Add(new ShapeData(name, x, y));
        }
    }

    private void spawnShapes(){
        Sprite square = Resources.Load<Sprite>("tangram/square-filled");
        Sprite triangle = Resources.Load<Sprite>("tangram/triangle-filled");
        
        for (int i=0; i<sd.Count; i++){
            GameObject shape = new GameObject();
            shape.name = "shape_" + i;
            shape.transform.SetParent(shapeAnchor.transform);
            shape.AddComponent(typeof(ClickOnlyVisible.HideInvisible));
            Image img = shape.AddComponent<Image>();
            
            switch (sd[i].shapeName){
                case "square":
                    img.sprite = square;
                    break;
                case "triangle":
                    img.sprite = triangle;
                    break;
            }

            shape.SetActive(true);
        }
    }
}

public class ShapeData{
    string name;
    float xSize;
    float ySize;

    public ShapeData(string n, float x, float y){
        shapeName = n;
        shapeX = x;
        shapeY = y;
    }

    protected internal string shapeName{
        get => name;
        set => name = value;
    }
    protected internal float shapeX{
        get => xSize;
        set => xSize = value;
    }
    protected internal float shapeY{
        get => ySize;
        set => ySize = value;
    }
}