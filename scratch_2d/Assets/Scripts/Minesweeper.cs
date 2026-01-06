using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor;

public enum Tile
{
    Empty,
    Number,
    Mine
}

//hmm there's definitely math to spawning this
// but i don't know it
// come back and rework this at some point!

// not gonna check for playability right now
// simply randomizing locations.
// ...bomb clusters are bad though, fix that.

public class Minesweeper : MonoBehaviour
{
    private static int[] EASYSTATS = {9, 9, 10};
    private static int[] MEDISTATS = {16, 16, 40};
    private static int[] HARDSTATS = {16, 30, 99};
    private static int SCREENSIZE = 1200;

    public GameObject coverAnchor;
    public GameObject landAnchor;
    public GameObject sampleRow;

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

    private List<List<Tile>> map;
    private List<List<int>> numberMap;
    private List<Tuple<int, int>> randCoor;
    
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
        
        if (mines > height * width){
            mines = height * width - 1;
        }
        minesLeft = mines;

        spawnTiles();

        map = new List<List<Tile>>{};
        numberMap = new List<List<int>>{};
        randCoor = new List<Tuple<int, int>>{};
    }

    private void spawnTiles(){
        Sprite sprite = Resources.Load<Sprite>("tile");
        float tileSizeH = SCREENSIZE / height; //i think it scales? idk
        float tileSizeW = SCREENSIZE / width;
        
        for (int i=0; i<height; i++){
            GameObject row = GameObjectUtility.DuplicateGameObject(sampleRow);
            row.name = "row" + i;
            // row.transform.SetParent(coverAnchor.transform);

            for (int j=0; j<width; j++){
                GameObject tile = new GameObject();
                tile.name = "tile" + j;
                Button tib = tile.AddComponent<Button>();
                Image tii = tile.AddComponent<Image>();
                tii.sprite = sprite;
                (int, int) ack = (i, j);
                tib.onClick.AddListener(() => click(ack.Item1, ack.Item2));
                tile.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSizeW, tileSizeH);
                tile.transform.SetParent(row.transform);
                tile.SetActive(true);
            }
            row.SetActive(true);
        }
        Destroy(sampleRow);
    }

    //it's loops all the way down
    private void generateMap(int x, int y){
        for (int i=0; i<height; i++){
            List<Tile> row = new List<Tile>{};
            List<int> numberRow = new List<int>{};
            for (int j=0; j<width; j++){
                row.Add(Tile.Empty);
                numberRow.Add(0);
                randCoor.Add(new Tuple<int, int>(i, j));
            }
            map.Add(row);
            numberMap.Add(numberRow);
        }
        
        _reshuffleMines();
        fillMap(x, y);
    }
    private void fillMap(int x, int y){
        int t = 0;
        for (; mines>0; mines--){
            Tuple<int, int> curr = randCoor[t];
            if (curr.Item1 == x && curr.Item2 == y){
                //no bombs at current click
                t++;
                continue;
            }
            map[curr.Item1][curr.Item2] = Tile.Mine;
            numberMap[curr.Item1][curr.Item2] = -1;
            randCoor.RemoveAt(t);
        }

        calculateNumbers(x, y);
    }
    private void calculateNumbers(int x, int y){
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

        click(x, y); //rerun after generating map fully
    }

    public void click(int x, int y){
        Debug.Log(x + " " + y);
        if (map.Count == 0){
            generateMap(x, y);
            return;
        }
        if (map[x][y] == Tile.Mine){
            Debug.Log("Game Over");
        }
        else if (map[x][y] == Tile.Number){
            Debug.Log("Number " + numberMap[x][y]);
        }
        else{
            Debug.Log("Empty");
        }

        _print();
    }

    public void reset(){
        map = new List<List<Tile>>{};
        numberMap = new List<List<int>>{};
        randCoor = new List<Tuple<int, int>>{};
        
        // if size change available, check here.
        // reset screen, but i'm not really handling any of that yet
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

    private void _print(){
        for (int i=0; i<numberMap.Count; i++){
            string s = "";
            for (int j=0; j<numberMap[i].Count; j++){
                s += " " + numberMap[i][j];
            }
            Debug.Log(s);
        }
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