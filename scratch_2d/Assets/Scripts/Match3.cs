using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

public class Match3 : MonoBehaviour
{
    //shape count, grid height, grid width
    private static int[] EASYSTATS = {4, 10, 10};
    private static int[] MEDISTATS = {5, 14, 14};
    private static int[] HARDSTATS = {6, 18, 18};
    private static int SCREENSIZE = 1200;

    public GameObject anchor;
    public GameObject sampleColumn;

    [Header("Difficulty")]
    public bool easy;
    public bool medium;
    public bool hard;

    [Header("Customize")]
    public bool customizeOn;
    public string[] imageNames;
    public int gridHeight = 5;
    public int gridWidth = 5;

    private float tileSizeH;
    private float tileSizeW;
    private int[] imageCount;

    // this would need an "any more moves" checker

    // i made this with h and w as different values, potentially
    // but it looks terrible and should not be done like that
    // this needs to be square or the sizes should be calculated differently

    //min. 3 images
    private string[] images = {"circle", "diamond", "heart", "square", "star", "triangle"};

    void Start(){
        _reshuffleImages();

        if (customizeOn){
            if (imageNames.Length < 2){
                imageNames = new string[3];
                imageNames[0] = images[0];
                imageNames[1] = images[1];
                imageNames[2] = images[2];
            }
            if (gridWidth < 5){
                gridWidth = 5;
            }
            else if (gridWidth > 50){
                gridWidth = 50;
            }
            if (gridHeight < 5){
                gridHeight = 5;
            }
            else if (gridHeight > 50){
                gridHeight = 50;
            }
        }
        else if (easy){
            imageNames = new string[EASYSTATS[0]];
            for (int i=0; i<EASYSTATS[0]; i++){
                imageNames[i] = images[i];
            }
            gridHeight = EASYSTATS[1];
            gridWidth = EASYSTATS[2];
        }
        else if (medium){
            imageNames = new string[MEDISTATS[0]];
            for (int i=0; i<MEDISTATS[0]; i++){
                imageNames[i] = images[i];
            }
            gridHeight = MEDISTATS[1];
            gridWidth = MEDISTATS[2];
        }
        else{
            imageNames = new string[HARDSTATS[0]];
            for (int i=0; i<HARDSTATS[0]; i++){
                imageNames[i] = images[i];
            }
            gridHeight = HARDSTATS[1];
            gridWidth = HARDSTATS[2];
        }

        tileSizeH = SCREENSIZE / gridHeight;
        tileSizeW = SCREENSIZE / gridWidth;
        imageCount = new int[imageNames.Length];

        initGrid();
    }

    private void initGrid(){
        for (int j=0; j<gridWidth; j++){
            GameObject col = GameObjectUtility.DuplicateGameObject(sampleColumn);
            col.name = "col" + j;
            col.transform.SetSiblingIndex(j+1);

            for (int i=0; i<gridHeight; i++){
                GameObject img = new GameObject();
                img.name = "tile" + i;
                Image imi = img.AddComponent<Image>();
                imi.sprite = Resources.Load<Sprite>("match3/" + _pickImage());
                img.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSizeW, tileSizeH);

                // add drag n drop - modify plantsmovement

                img.transform.SetParent(col.transform);
                img.SetActive(true);
            }
            col.SetActive(true);
        }
        Destroy(sampleColumn);
    }

    private void _reshuffleImages(){
        //want pseudorandom but unfixed
        for (int t=0; t<images.Length; t++){
            string tmp = images[t];
            int r = UnityEngine.Random.Range(t, images.Length);
            images[t] = images[r];
            images[r] = tmp;
        }
    }

    private string _pickImage(){
        //randomizer to generate next image
        // this could be simple. or it could be complex.

        int high = 0;
        int low = 0;
        for (int i=1; i<imageCount.Length; i++){
            if (imageCount[i] > imageCount[high]){
                high = i;
            }
            else if (imageCount[i] < imageCount[low]){
                low = i;
            }
        }

        //close enough
        if (imageCount[high]-imageCount[low] < 3){
            //number here (3) should probably depend on either grid size or number of image possibilities..?
            // not really sure how to scale it
            return imageNames[UnityEngine.Random.Range(0, imageCount.Length)];
        }

        //weighted list
        List<int> poss = new List<int>{};
        for (int i=0; i<imageCount.Length; i++){
            for (int j=imageCount[i]; j<imageCount[high]; j++){
                poss.Add(imageCount[i]);
            }
        }
        return imageNames[UnityEngine.Random.Range(0, poss.Count)];
    }
}
