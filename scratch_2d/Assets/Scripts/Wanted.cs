using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Wanted : MonoBehaviour
{
    // absolutely do not call this wanted for actual production
    // it's some variation of like... spot the difference.

    //rewrite some of this to match actual function calls in-game:
    // ad fairy object spawns are handled in separate file
    // poss + randomization unneeded
    // gonna put a grid in the main thing because i don't feel like handling
    //   spawn points, but probably don't want that in actual
    public static Wanted wanted;
    
    public GameObject spawnAnchorNoGrid;
    public int spawnMax = 50;

    // movement doesn't work like this yet
    private string[] movementStyle = {"none", "diagonalLeft", "right", "waves", "random"};
    private float[] movementSpeed = {0.15f, 0.3f, 0.45f, 0.6f, 0.75f, 0.9f};
    private string path = "wanted/";
    private string[] poss = {"red", "orange", "yellow", "green", "blue", "purple"};

    public int difficulty = 0; //make private in actual
    private List<string> itlPoss = new List<string>{};
    private int buttonIndex = -1;
    private System.Random rand;

    void Start(){
        wanted = this;
        rand = new System.Random();
        randSpawns();
        loadImages();
    }

    protected internal void setDifficulty(int diff){
        difficulty = diff;
    }

    public void target(){
        Debug.Log("clicked target");
    }

    protected internal void loadImages(){
        float currX = 0f;
        float currY = 0f;
        // we'll call it a baby grid
        for (int i=0; i<itlPoss.Count; i++){
            GameObject tar = new GameObject();
            tar.name = itlPoss[i] + i;

            Image eimg = tar.AddComponent<Image>();
            eimg.sprite = Resources.Load<Sprite>(path + itlPoss[i]);

            
            RectTransform rt = tar.GetComponent<RectTransform>();
            if (currX == 0f){
                currX = rt.rect.width + 5f;
            }
            if (currY == 0f){
                currY = rt.rect.height + 5f;
            }

            tar.transform.position = new Vector3(currX, currY, 0);
            currX += rt.rect.width + 5f;
            if ((currX + 5f) > spawnAnchorNoGrid.GetComponent<RectTransform>().rect.width){
                currX = rt.rect.width + 5f;
                currY += rt.rect.height + 5f;
            }

            if (i == buttonIndex){
                Button b = tar.AddComponent<Button>();
                b.onClick.AddListener(() => target());
            }

            WantedMovement wm = (WantedMovement)tar.AddComponent(typeof(WantedMovement));
            if (i%5 == 0){
                wm.setGrow(true, 0, 0.2f, 2f, 0.5f);
            }
            if (i%4 == 0){
                wm.setSpin(true, false, 0.2f, -1);
            }
            else if (i%4 == 2){
                wm.setSpin(true, true, 0.2f, -1);
            }
            if (i%3 == 0){
                wm.setMove(true, true, 1.047198f, 2f); //5f is kinda fast
            }

            tar.transform.SetParent(spawnAnchorNoGrid.transform);
            tar.SetActive(true);
        }
    }

    private void randSpawns(){
        int tag = UnityEngine.Random.Range(0, poss.Length);
        int ct = rand.Next(poss.Length*2, spawnMax);
        bool tagless = true;
        int[] possCt = new int[poss.Length];

        for (int t=0; t<ct; t++){
            int choice = rand.Next(poss.Length);
            if (t >= ct-poss.Length){
                for (int i=0; i<possCt.Length; i++){
                    if (possCt[i] < 2 && i != tag){
                        choice = i;
                        break;
                    }
                }
            }
            if (tagless && ct-t == 1){
                choice = tag;
            }
            if (choice == tag){
                if (tagless){
                    tagless = false;
                    buttonIndex = t;
                }
                else{
                    continue;
                }
            }
            itlPoss.Add(poss[choice]);
            possCt[choice] += 1;
        }
    }
}
