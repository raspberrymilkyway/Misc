using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;
using TMPro;

public class Skyscrapers : MonoBehaviour
{
    private static int SCREENSIZE = 1200;

    public GameObject anchor;
    public GameObject sampleRow;
    public GameObject sampleInputField;

    //i'm gonna try to randomize this eventually but.
    // i do think it'll need to be like. hand-done. for the game
    
    public int currentLevel = 1; //make private/protected internal in actual
    public Color32 bgColor = new Color32(224, 224, 224, 255);
    public Color32 arrowColor = new Color32(0, 0, 0, 255);

    [Header("Custom (unimplemented)")]
    public bool custom = false;
    public int buildingsPerRow = 4; // n x n grid
    public int nilPerRow = 0;

    private float tileSize;
    private int[] arrowsN;
    private int[] arrowsE;
    private int[] arrowsS;
    private int[] arrowsW;
    private int[][] skyscrapers;
    private int[][] userSkyscrapers;
    //oh this should maybe have a pencil option

    void Start(){
        tileSize = SCREENSIZE / (buildingsPerRow+4);

        // if (!custom){
            loadFile();
        // }
        userSkyscrapers = new int[skyscrapers.Length][];
        for (int i=0; i<skyscrapers.Length; i++){
            userSkyscrapers[i] = new int[skyscrapers[0].Length];
        }
        spawnTiles();
    }

    private void loadFile(){
        TextAsset ac = Resources.Load<TextAsset>("skyscrapers/levels/level" + currentLevel);
        string[] lines = ac.text.Split("\n");
        skyscrapers = new int[lines.Length-2][];

        for (int i=0; i<lines.Length; i++){
            lines[i] = lines[i].Trim('\r');
            string l = lines[i].Substring(2, lines[i].Length-4); //trim asterisk, space
            int[] arr = Array.ConvertAll(l.Split(" "), (item => Int32.Parse(item)));
            buildingsPerRow = arr.Length;

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
        //pascal case parameters, please!
        spawnNumberRow("N");
        spawnArrowRow("Down");
        spawnSkyscrapers();
        spawnArrowRow("Up");
        spawnNumberRow("S");
        Destroy(sampleRow);
    }

    private void spawnNumberRow(string cardinal){
        GameObject row = GameObjectUtility.DuplicateGameObject(sampleRow);
        row.name = "numbers" + cardinal;
        row.transform.SetSiblingIndex(anchor.transform.childCount);
        for (int i=0; i<buildingsPerRow+4; i++){
            GameObject number = new GameObject();
            number.name = "number" + i;
            Image numberi = number.AddComponent<Image>();
            if (i==0 || i==1 || i==buildingsPerRow+2 || i==buildingsPerRow+3){
                numberi.sprite = Resources.Load<Sprite>("skyscrapers/arrowNil");
                numberi.color = bgColor;
            }
            else{
                if (cardinal.Equals("N") || cardinal.Equals("North")){
                    numberi.sprite = Resources.Load<Sprite>("skyscrapers/" + arrowsN[i-2].ToString());
                }
                else{
                    numberi.sprite = Resources.Load<Sprite>("skyscrapers/" + arrowsS[i-2].ToString());
                }
            }
            number.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize, tileSize);
            number.transform.SetParent(row.transform);
            number.SetActive(true);
        }
        row.SetActive(true);
    }

    private void spawnArrowRow(string direction){
        GameObject row = GameObjectUtility.DuplicateGameObject(sampleRow);
        row.name = "arrows" + direction;
        row.transform.SetSiblingIndex(anchor.transform.childCount);
        for (int i=0; i<buildingsPerRow+4; i++){
            GameObject arrow = new GameObject();
            arrow.name = "arrow" + i;
            Image arrowi = arrow.AddComponent<Image>();
            if (i==0 || i==1 || i==buildingsPerRow+2 || i==buildingsPerRow+3){
                arrowi.sprite = Resources.Load<Sprite>("skyscrapers/arrowNil");
                arrowi.color = bgColor;
            }
            else{
                arrowi.sprite = Resources.Load<Sprite>("skyscrapers/arrow" + direction);
                arrowi.color = arrowColor;
            }
            arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize, tileSize);
            arrow.transform.SetParent(row.transform);
            arrow.SetActive(true);
        }
        row.SetActive(true);
    }

    private void spawnSkyscrapers(){
        for (int i=0; i<skyscrapers.Length; i++){
            GameObject row = GameObjectUtility.DuplicateGameObject(sampleRow);
            row.name = "scrapers" + i;
            row.transform.SetSiblingIndex(anchor.transform.childCount);
            for (int j=0; j<skyscrapers[i].Length+2; j++){
                GameObject scrap = new GameObject();
                scrap.name = "scrap" + j;
                Image scrapi = scrap.AddComponent<Image>();
                if (j==0){
                    scrap.name = "numberW";
                    scrap.transform.SetParent(row.transform);
                    scrapi.sprite = Resources.Load<Sprite>("skyscrapers/" + arrowsW[i].ToString());
                    GameObject arrow = new GameObject();
                    arrow.name = "arrowRight";
                    Image arrowi = arrow.AddComponent<Image>();
                    arrowi.sprite = Resources.Load<Sprite>("skyscrapers/arrowRight");
                    arrowi.color = arrowColor;
                    arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize, tileSize);
                    arrow.transform.SetParent(row.transform);
                    arrow.SetActive(true);
                }
                else if (j==skyscrapers[i].Length+1){
                    scrapi.sprite = Resources.Load<Sprite>("skyscrapers/" + arrowsE[i].ToString());
                    GameObject arrow = new GameObject();
                    arrow.name = "arrowLeft";
                    Image arrowi = arrow.AddComponent<Image>();
                    arrowi.sprite = Resources.Load<Sprite>("skyscrapers/arrowLeft");
                    arrowi.color = arrowColor;
                    arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize, tileSize);
                    arrow.transform.SetParent(row.transform);
                    arrow.SetActive(true);
                    scrap.name = "numberE";
                    scrap.transform.SetParent(row.transform);
                }
                else{
                    scrap.transform.SetParent(row.transform);
                    scrapText(scrap, i, j-1);
                    scrapi.sprite = Resources.Load<Sprite>("skyscrapers/arrowNil");
                    //...size is automatic(ally 0) until given a reason to exist.
                    scrapi.color = new Color32(0, 0, 0, 0);
                }
                scrap.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize, tileSize);
                scrap.SetActive(true);
            }
            row.SetActive(true);
        }
        Destroy(sampleInputField);
    }

    private void scrapText(GameObject scrap, int i, int j){
        GameObject inp = GameObjectUtility.DuplicateGameObject(sampleInputField);
        RectTransform rt = inp.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(tileSize, tileSize);
        inp.transform.SetParent(scrap.transform);
        rt.transform.localPosition = new Vector3(0,0,0);
        inp.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate{updateFilled(inp, i, j);});
        inp.SetActive(true);
    }

    private bool checkAllFilled(){
        Debug.Log("\n");
        for (int i=0; i<userSkyscrapers.Length; i++){
            string s = "";
            for (int j=0; j<userSkyscrapers[i].Length; j++){
                s += userSkyscrapers[i][j].ToString() + " ";
                if (userSkyscrapers[i][j] < 1){
                    return false;
                }
            }
            Debug.Log(s);
        }
        return true;
    }

    private bool checkGrid(){
        for (int i=0; i<userSkyscrapers.Length; i++){
            for (int j=0; j<userSkyscrapers[i].Length; j++){
                if (userSkyscrapers[i][j] != skyscrapers[i][j]){
                    Debug.Log("not correct");
                    return false;
                }
            }
        }
        return true;
    }

    private void win(){
        Debug.Log("Game over - you win!");
    }

    public void updateFilled(GameObject go, int i, int j){
        if (go == null){
            return;
        }
        string num = go.GetComponent<TMP_InputField>().text;
        if (num.Length < 1 || num.Equals("-")){
            userSkyscrapers[i][j] = 0;
        }
        else{
            userSkyscrapers[i][j] = Int32.Parse(num);
        }
        if (checkAllFilled() && checkGrid()){
            win();
        }
    }
}
