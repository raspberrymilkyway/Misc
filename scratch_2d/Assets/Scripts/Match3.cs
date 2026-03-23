using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using System;
using System.Collections.Generic;
using IHatJHat.UI;

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
    private GameObject selected;
    private GameObject sup;
    private GameObject sdown;
    private GameObject sleft;
    private GameObject sright;
    private ColorBlock cb;

    private Dictionary<GameObject, bool> matches;

    // this would need an "any more moves" checker

    // i made this with h and w as different values, potentially
    // but it looks terrible and should not be done like that
    // this needs to be square or the sizes should be calculated differently

    // this would be easier with a series of 3d objects instead of a canvas

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
        matches = new Dictionary<GameObject, bool>();
        _resetSelected();

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
                LRClickButton bu = (LRClickButton)img.AddComponent(typeof(IHatJHat.UI.LRClickButton));
                bu.OnLeftClick.AddListener(() => selectTile(img));
                bu.OnRightClick.AddListener(() => deselectTile(img));
                if (j == 0 && i == 0){
                    cb = bu.colors;
                    cb.selectedColor = new Color32(149, 149, 149, 255);
                }
                bu.colors = cb;
                
                // add drag n drop - modify plantsmovement

                img.transform.SetParent(col.transform);
                img.SetActive(true);
            }
            col.SetActive(true);
        }
        Destroy(sampleColumn);
    }

    public void selectTile(GameObject go){
        if (selected == null){
            _setSelected(go);
            //uhh maybe add selected color or anim or something? idk
        }
        else if (go == selected){
            deselectTile(go);
        }
        else{
            if (go == sup || go == sdown || go == sleft || go == sright){
                Image goi = go.GetComponent<Image>();
                Image sei = selected.GetComponent<Image>();
                if (goi.sprite == sei.sprite){
                    //invalid
                    Debug.Log("invalid (same image)");
                    _resetSelected();
                    return;
                }
                Sprite tmp = goi.sprite;
                goi.sprite = sei.sprite;
                sei.sprite = tmp;

                _checkForMatch(go);
                _resetSelected();
            }
            else{
                _setSelected(go);
            }
        }
    }
    public void deselectTile(GameObject go){
        if (selected == go){
            _resetSelected();
            // end any color or animation
        }
    }

    private void _checkForMatch(GameObject go){
        // check both
        _checkAdjacents(go);
        _checkAdjacents(selected);

        foreach (KeyValuePair<GameObject, bool> kvp in matches){
            kvp.Key.GetComponent<Image>().sprite = Resources.Load<Sprite>("match3/empty_forTesting");
        }
    }
    private void _checkAdjacents(GameObject go){
        // check if it's on the edge
        Selectable ul = go.GetComponent<Selectable>().FindSelectableOnUp();
        _checkToList(go, ul, "up");
        Selectable dr = go.GetComponent<Selectable>().FindSelectableOnDown();
        _checkToList(go, dr, "down");
        // check if it's in the middle
        Sprite im = null;
        if (ul != null){
            im = ul.gameObject.GetComponent<Image>().sprite;
            if (dr != null && im == dr.gameObject.GetComponent<Image>().sprite && im == go.GetComponent<Image>().sprite){
                try{
                    matches.Add(go, false);
                } catch (ArgumentException){
                    //
                }
                try{
                    matches.Add(ul.gameObject, false);
                } catch (ArgumentException){
                    //
                }
                try{
                    matches.Add(dr.gameObject, false);
                } catch (ArgumentException){
                    //
                }
            }
        }

        // edges
        ul = go.GetComponent<Selectable>().FindSelectableOnLeft();
        _checkToList(go, ul, "left");
        dr = go.GetComponent<Selectable>().FindSelectableOnRight();
        _checkToList(go, dr, "right");
        // middle
        if (ul != null){
            im = ul.gameObject.GetComponent<Image>().sprite;
            if (dr != null && im == dr.gameObject.GetComponent<Image>().sprite && im == go.GetComponent<Image>().sprite){
                try{
                    matches.Add(go, false);
                } catch (ArgumentException){
                    //
                }
                try{
                    matches.Add(ul.gameObject, false);
                } catch (ArgumentException){
                    //
                }
                try{
                    matches.Add(dr.gameObject, false);
                } catch (ArgumentException){
                    //
                }
            }
        }
    }
    private bool _checkNext(GameObject go, string direction){
        Selectable s = go.GetComponent<Selectable>();
        if (direction == "up"){
            s = s.FindSelectableOnUp();
        }
        else if (direction == "down"){
            s = s.FindSelectableOnDown();
        }
        else if (direction == "left"){
            s = s.FindSelectableOnLeft();
        }
        else if (direction == "right"){
            s = s.FindSelectableOnRight();
        }
        else{
            s = null;
        }

        if (s != null && s.gameObject.GetComponent<Image>().sprite == go.GetComponent<Image>().sprite){
            return true;
        }

        return false;
    }
    private void _checkToList(GameObject go, Selectable s, string direction){
        if (s != null && _checkNext(go, direction) && _checkNext(s.gameObject, direction)){
            try{
                matches.Add(go, false);
            } catch (ArgumentException){
                //
            }
            try{
                matches.Add(s.gameObject, false);
            } catch (ArgumentException){
                //
            }
            try{
                switch (direction){
                    case "up":
                        matches.Add(s.gameObject.GetComponent<Selectable>().FindSelectableOnUp().gameObject, false);
                        break;
                    case "down":
                        matches.Add(s.gameObject.GetComponent<Selectable>().FindSelectableOnDown().gameObject, false);
                        break;
                    case "left":
                        matches.Add(s.gameObject.GetComponent<Selectable>().FindSelectableOnLeft().gameObject, false);
                        break;
                    case "right":
                        matches.Add(s.gameObject.GetComponent<Selectable>().FindSelectableOnRight().gameObject, false);
                        break;
                }
            } catch (ArgumentException){
                //
            }
        }
    }

    private void _setSelected(GameObject go){
        selected = go;
        Selectable s = selected.GetComponent<Selectable>().FindSelectableOnUp();
        sup = s == null ? null : s.gameObject;
        s = selected.GetComponent<Selectable>().FindSelectableOnDown();
        sdown = s == null ? null : s.gameObject;
        s = selected.GetComponent<Selectable>().FindSelectableOnLeft();
        sleft = s == null ? null : s.gameObject;
        s = selected.GetComponent<Selectable>().FindSelectableOnRight();
        sright = s == null ? null : s.gameObject;
    }
    private void _resetSelected(){
        selected = null;
        sup = null;
        sdown = null;
        sleft = null;
        sright = null;
        EventSystem.current.SetSelectedGameObject(null);
        matches = new Dictionary<GameObject, bool>();
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
