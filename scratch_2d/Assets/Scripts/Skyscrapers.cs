using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;

public class Skyscrapers : MonoBehaviour
{
    private static int SCREENSIZE = 1200;

    public GameObject anchor;
    public GameObject sampleRow;

    //i'm gonna try to randomize this eventually but.
    // i do think it'll need to be like. hand-done. for the game
    
    public int currentLevel = 1; //make private/protected internal in actual

    [Header("Custom (unimplemented)")]
    public bool custom = false;
    public int buildingsPerRow = 4; // n x n grid
    public int nilPerRow = 0;

    //otherwise, int arrays for each col/row
    // ...can i make unity display int[,] in a menu?

    private float tileSize;
    private int[] arrowsN;
    private int[] arrowsE;
    private int[] arrowsS;
    private int[] arrowsW;
    private int[][] skyscrapers;
    //oh this should maybe have a pencil option

    void Start(){
        tileSize = SCREENSIZE / (buildingsPerRow+2);

        if (!custom){
            loadFile();
        }
    }

    private void loadFile(){
        TextAsset ac = Resources.Load<TextAsset>("skyscrapers/levels/level" + currentLevel);
        string[] lines = ac.text.Split("\n");
        skyscrapers = new int[lines.Length-2][];

        for (int i=0; i<lines.Length; i++){
            lines[i] = lines[i].Trim('\r');
            string l = lines[i].Substring(2, lines[i].Length-4); //trim asterisk, space
            int[] arr = Array.ConvertAll(l.Split(" "), (item => Int32.Parse(item)));

            if (lines[i][0].Equals('*')){
                if (i == 0){
                    arrowsN = arr;
                    arrowsE = new int[arrowsN.Length];
                    arrowsW = new int[arrowsN.Length];
                }
                else{
                    arrowsS = arr;
                }
            }
            else{ //normal numbers
                arrowsW[i-1] = Int32.Parse(lines[i].Substring(0, 1));
                arrowsE[i-1] = Int32.Parse(lines[i].Substring(lines[i].Length-1));
                skyscrapers[i-1] = arr;
            }
        }
    }

    private void spawnTiles(){
        spawnArrowRow();
        for (int i=0; i<0; i++){
            GameObject row = GameObjectUtility.DuplicateGameObject(sampleRow);
            row.name = "row" + i;
            row.transform.SetSiblingIndex(i+3);
        }
        Destroy(sampleRow);
    }

    private void spawnNumberRow(){
        GameObject row = GameObjectUtility.DuplicateGameObject(sampleRow);
        row.name = "skyscraperCount";
        row.transform.SetSiblingIndex(1);
        for (int i=0; i<buildingsPerRow+1; i++){
            GameObject arrow = new GameObject();
            arrow.name = "arrow" + i;
            Image arrowi = arrow.AddComponent<Image>();
            arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize, tileSize);
            arrow.transform.SetParent(row.transform);
            arrow.SetActive(true);
        }
    }

    private void spawnArrowRow(){
        GameObject row = GameObjectUtility.DuplicateGameObject(sampleRow);
        row.name = "arrows";
        row.transform.SetSiblingIndex(2);
        for (int i=0; i<buildingsPerRow+1; i++){
            GameObject arrow = new GameObject();
            arrow.name = "arrow" + i;
            Image arrowi = arrow.AddComponent<Image>();
            arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize, tileSize);
            arrow.transform.SetParent(row.transform);
            arrow.SetActive(true);
        }
    }

    private void checkGrid(){
        //
    }
}
