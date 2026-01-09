using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using IHatJHat.UI;

public enum Tile
{
    Empty,
    Number,
    Mine
}
public enum Cover
{
    Off,
    Solid,
    Flag
}

//

public class Minesweeper : MonoBehaviour
{
    private static int[] EASYSTATS = {9, 9, 10};
    private static int[] MEDISTATS = {16, 16, 40};
    private static int[] HARDSTATS = {16, 30, 99};
    private static int SCREENSIZE = 1200;

    public GameObject coverAnchor;
    public GameObject landAnchor;
    public GameObject sampleCoverRow;
    public GameObject sampleLandRow;

    [Header("Difficulty")]
    public bool easy;
    public bool medium;
    public bool hard;

    [Header("Customize")]
    public bool customizeOn;
    public int heightCount;
    public int widthCount;
    public int mineCount; //mineCount < widthCount*heightCount
    
    private int mines;
    private int width;
    private int height;
    private int minesLeft;
    private int minesMax;

    private List<List<Tile>> map;
    private List<List<int>> numberMap;
    private List<List<Cover>> coverVisible;
    private List<Tuple<int, int>> randCoor;

    private float tileSizeH;
    private float tileSizeW;

    private Coroutine _adjacentCovers;

    void Start()
    {
        if (customizeOn){
            height = heightCount;
            width = widthCount;
            mines = mineCount;
        }
        else if (easy){
            height = EASYSTATS[0];
            width = EASYSTATS[1];
            mines = EASYSTATS[2];
        }
        else if (medium){
            height = MEDISTATS[0];
            width = MEDISTATS[1];
            mines = MEDISTATS[2];
        }
        else{
            //hard is default, lol
            height = HARDSTATS[0];
            width = HARDSTATS[1];
            mines = HARDSTATS[2];
        }

        if (height < 4){
            height = 4;
        }
        else if (height > 50){
            height = 50;
        }
        if (width < 4){
            width = 4;
        }
        else if (width > 50){
            width = 50;
        }
        tileSizeH = SCREENSIZE / height; //i think it scales?
        tileSizeW = SCREENSIZE / width;
        
        
        if (mines < 1){
            mines = 1;
            minesLeft = mines;
            minesMax = mines;
        }
        else if (mines > height * width){
            mines = height * width;
            minesLeft = mines-1;
            minesMax = mines-1;
        }
        else {
            minesLeft = mines;
            minesMax = mines;
        }

        spawnCoverTiles();

        map = new List<List<Tile>>{};
        numberMap = new List<List<int>>{};
        coverVisible = new List<List<Cover>>{};
        randCoor = new List<Tuple<int, int>>{};
    }

    private void spawnCoverTiles(){        
        for (int i=0; i<height; i++){
            GameObject row = GameObjectUtility.DuplicateGameObject(sampleCoverRow);
            row.name = "row" + i;
            row.transform.SetSiblingIndex(i+1);

            for (int j=0; j<width; j++){
                GameObject tile = new GameObject();
                tile.name = "tile" + j;
                // Button tib = tile.AddComponent<Button>();
                LRClickButton tib = (LRClickButton)tile.AddComponent(typeof(IHatJHat.UI.LRClickButton));
                Image tii = tile.AddComponent<Image>();
                tii.sprite = Resources.Load<Sprite>("tile");
                (int, int) ack = (i, j);
                tib.OnLeftClick.AddListener(() => click(ack.Item1, ack.Item2, false));
                tib.OnRightClick.AddListener(() => click(ack.Item1, ack.Item2, true));
                tile.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSizeW, tileSizeH);
                tile.transform.SetParent(row.transform);
                tile.SetActive(true);
            }
            row.SetActive(true);
        }
        Destroy(sampleCoverRow);
    }
    private void spawnLandTiles(){
        for (int i=0; i<height; i++){
            GameObject row = GameObjectUtility.DuplicateGameObject(sampleLandRow);
            row.name = "row" + i;
            row.transform.SetSiblingIndex(i+1);

            for (int j=0; j<width; j++){
                GameObject tile = new GameObject();
                tile.name = "tile" + j;
                Image tii = tile.AddComponent<Image>();

                //handle image
                switch (numberMap[i][j]){
                    case 0:
                        tii.sprite = Resources.Load<Sprite>("0");
                        break;
                    case 1:
                        tii.sprite = Resources.Load<Sprite>("1");
                        break;
                    case 2:
                        tii.sprite = Resources.Load<Sprite>("2");
                        break;
                    case 3:
                        tii.sprite = Resources.Load<Sprite>("3");
                        break;
                    case 4:
                        tii.sprite = Resources.Load<Sprite>("4");
                        break;
                    case 5:
                        tii.sprite = Resources.Load<Sprite>("5");
                        break;
                    case 6:
                        tii.sprite = Resources.Load<Sprite>("6");
                        break;
                    case 7:
                        tii.sprite = Resources.Load<Sprite>("7");
                        break;
                    case 8:
                        tii.sprite = Resources.Load<Sprite>("8");
                        break;
                    case 9:
                        tii.sprite = Resources.Load<Sprite>("9");
                        break;
                    default:
                        tii.sprite = Resources.Load<Sprite>("mine");
                        break;
                }

                tile.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSizeW, tileSizeH);
                tile.transform.SetParent(row.transform);
                tile.SetActive(true);
            }
            row.SetActive(true);
        }
        Destroy(sampleLandRow);
    }

    //it's loops all the way down
    private void generateMap(int x, int y, bool right){
        for (int i=0; i<height; i++){
            List<Tile> row = new List<Tile>{};
            List<int> numberRow = new List<int>{};
            List<Cover> visibleRow = new List<Cover>{};
            for (int j=0; j<width; j++){
                row.Add(Tile.Empty);
                numberRow.Add(0);
                visibleRow.Add(Cover.Solid);
                randCoor.Add(new Tuple<int, int>(i, j));
            }
            map.Add(row);
            numberMap.Add(numberRow);
            coverVisible.Add(visibleRow);
        }
        
        _reshuffleMines();
        fillMap(x, y, right);
    }
    private void fillMap(int x, int y, bool right){
        int t = 0;
        for (; mines>0; mines--){
            Tuple<int, int> curr = randCoor[t];
            if (minesMax == height * width-1){
                if (curr.Item1 == x && curr.Item2 == y){
                    t++;
                    continue;
                }
            }
            else if ((curr.Item1 == x || curr.Item1 == x+1 || curr.Item1 == x-1) && (curr.Item2 == y || curr.Item2 == y+1 || curr.Item2 == y-1)){
                //no bombs at current click - start with empty square
                t++;
                continue;
            }
            map[curr.Item1][curr.Item2] = Tile.Mine;
            numberMap[curr.Item1][curr.Item2] = -1;
            randCoor.RemoveAt(t);
        }

        calculateNumbers(x, y, right);
    }
    private void calculateNumbers(int x, int y, bool right){
        for (int i=0; i<randCoor.Count; i++){
            Tuple<int, int> curr = randCoor[i];
            int count = 0;
            
            Check check = new Check();
            if (curr.Item1 == 0){
                check.nw = false;
                check.w = false;
                check.sw = false;
            }
            else if (curr.Item1 == width-1){
                check.ne = false;
                check.e = false;
                check.se = false;
            }
            if (curr.Item2 == 0){
                check.nw = false;
                check.n = false;
                check.ne = false;
            }
            else if (curr.Item2 == height-1){
                check.sw = false;
                check.s = false;
                check.se = false;
            }

            if (check.nw){
                if (numberMap[curr.Item1-1][curr.Item2-1] < 0){
                    count++;
                }
            }
            if (check.n){
                if (numberMap[curr.Item1][curr.Item2-1] < 0){
                    count++;
                }
            }
            if (check.ne){
                if (numberMap[curr.Item1+1][curr.Item2-1] < 0){
                    count++;
                }
            }
            if (check.e){
                if (numberMap[curr.Item1+1][curr.Item2] < 0){
                    count++;
                }
            }
            if (check.se){
                if (numberMap[curr.Item1+1][curr.Item2+1] < 0){
                    count++;
                }
            }
            if (check.s){
                if (numberMap[curr.Item1][curr.Item2+1] < 0){
                    count++;
                }
            }
            if (check.sw){
                if (numberMap[curr.Item1-1][curr.Item2+1] < 0){
                    count++;
                }
            }
            if (check.w){
                if (numberMap[curr.Item1-1][curr.Item2] < 0){
                    count++;
                }
            }

            numberMap[curr.Item1][curr.Item2] = count;
            if (count > 0){
                map[curr.Item1][curr.Item2] = Tile.Number;
            }
        }

        spawnLandTiles();
        click(x, y, right); //rerun after generating map fully
    }

    public void click(int x, int y, bool right){
        if (map.Count == 0){
            if (right){
                return;
            }
            generateMap(x, y, right);
            return;
        }

        if (coverVisible[x][y] == Cover.Off){
            return;
        }
        Image img = coverAnchor.transform.GetChild(x).GetChild(y).GetComponent<Image>();
        if (right){
            if (coverVisible[x][y] == Cover.Solid){
                img.sprite = Resources.Load<Sprite>("flag");
                coverVisible[x][y] = Cover.Flag;
                minesLeft--;
                checkWin();
            }
            else if (coverVisible[x][y] == Cover.Flag){
                img.sprite = Resources.Load<Sprite>("tile");
                coverVisible[x][y] = Cover.Solid;
                minesLeft++;
            }
            return;
        }
        if (coverVisible[x][y] == Cover.Flag){
            return;
        }

        coverVisible[x][y] = Cover.Off;
        img.sprite = Resources.Load<Sprite>("empty");

        if (map[x][y] == Tile.Mine){
            Debug.Log("Game Over");
            removeCover();
        }
        else if (map[x][y] == Tile.Empty){
            if (_adjacentCovers != null){
                StopCoroutine(adjacentEmpty(x, y));
            }
            _adjacentCovers = StartCoroutine(adjacentEmpty(x, y));
        }
        // else{
        //     Debug.Log("Number " + numberMap[x][y]);
        // }
    }

    private void removeCover(){
        for (int i=0; i<height; i++){
            for (int j=0; j<width; j++){
                Image img = coverAnchor.transform.GetChild(i).GetChild(j).GetComponent<Image>();
                img.sprite = Resources.Load<Sprite>("empty");
                coverVisible[i][j] = Cover.Off;
            }
        }
    }
    private IEnumerator adjacentEmpty(int x, int y){
        Check check = new Check();
        if (x == 0){
            check.w = false;
            check.nw = false;
            check.sw = false;
        }
        else if (x == width-1){
            check.e = false;
            check.ne = false;
            check.se = false;
        }
        if (y == 0){
            check.n = false;
            check.ne = false;
            check.nw = false;
        }
        else if (y == height-1){
            check.s = false;
            check.se = false;
            check.sw = false;
        }


        if (check.n){
            if (coverVisible[x][y-1] == Cover.Solid){
                _replaceImageEmpty(x, y-1);
                if (numberMap[x][y-1] == 0){
                    yield return adjacentEmpty(x, y-1);
                }
            }
        }
        if (check.e){
            if (coverVisible[x+1][y] == Cover.Solid){
                _replaceImageEmpty(x+1, y);
                if (numberMap[x+1][y] == 0){
                    yield return adjacentEmpty(x+1, y);
                }
            }
        }
        if (check.s){
            if (coverVisible[x][y+1] == Cover.Solid){
                _replaceImageEmpty(x, y+1);
                if (numberMap[x][y+1] == 0){
                    yield return adjacentEmpty(x, y+1);
                }
            }
        }
        if (check.w){
            if (coverVisible[x-1][y] == Cover.Solid){
                _replaceImageEmpty(x-1, y);
                if (numberMap[x-1][y] == 0){
                    yield return adjacentEmpty(x-1, y);
                }
            }
        }
        if (numberMap[x][y] == 0){
            if (check.nw){
                if (coverVisible[x-1][y-1] == Cover.Solid){
                    _replaceImageEmpty(x-1, y-1);
                    if (numberMap[x-1][y-1] == 0){
                        yield return adjacentEmpty(x-1, y-1);
                    }
                }
            }
            if (check.sw){
                if (coverVisible[x-1][y+1] == Cover.Solid){
                    _replaceImageEmpty(x-1, y+1);
                    if (numberMap[x-1][y+1] == 0){
                        yield return adjacentEmpty(x-1, y+1);
                    }
                }
            }
            if (check.se){
                if (coverVisible[x+1][y+1] == Cover.Solid){
                    _replaceImageEmpty(x+1, y+1);
                    if (numberMap[x+1][y+1] == 0){
                        yield return adjacentEmpty(x+1, y+1);
                    }
                }
            }
            if (check.ne){
                if (coverVisible[x+1][y-1] == Cover.Solid){
                    _replaceImageEmpty(x+1, y-1);
                    if (numberMap[x+1][y-1] == 0){
                        yield return adjacentEmpty(x+1, y-1);
                    }
                }
            }
        }
        yield return null;
    }

    public void reset(){
        map = new List<List<Tile>>{};
        numberMap = new List<List<int>>{};
        randCoor = new List<Tuple<int, int>>{};
        
        // if size change available, check here.
        // i don't think i'm going to bother implementing this here
        // i'm probably going to edit it from the vanilla version anyway, so there's no real point.
    }

    private void checkWin(){
        for (int i=0; i<height; i++){
            for (int j=0; j<width; j++){
                if (coverVisible[i][j] == Cover.Solid){
                    return;
                }
            }
        }
        Debug.Log("Game Won!");
        removeCover();
    }

    private void _reshuffleMines(){
        //want pseudorandom but unfixed
        for (int t=0; t<randCoor.Count; t++){
            Tuple<int, int> tmp = randCoor[t];
            int r = UnityEngine.Random.Range(t, randCoor.Count);
            randCoor[t] = randCoor[r];
            randCoor[r] = tmp;
        }
    }

    private void _replaceImageEmpty(int x, int y){
        Image img = coverAnchor.transform.GetChild(x).GetChild(y).GetComponent<Image>();
        img.sprite = Resources.Load<Sprite>("empty");
        coverVisible[x][y] = Cover.Off;
    }
}


public class Check
{
    public bool n = true;
    public bool ne = true;
    public bool e = true;
    public bool se = true;
    public bool s = true;
    public bool sw = true;
    public bool w = true;
    public bool nw = true;
}