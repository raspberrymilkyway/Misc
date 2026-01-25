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
    
    public GameObject spawnAnchor;
    public int spawnMax = 50;

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
        for (int i=0; i<itlPoss.Count; i++){
            GameObject tar = new GameObject();
            tar.name = itlPoss[i] + i;

            Image eimg = tar.AddComponent<Image>();
            eimg.sprite = Resources.Load<Sprite>(path + itlPoss[i]);

            if (i == buttonIndex){
                Button b = tar.AddComponent<Button>();
                b.onClick.AddListener(() => target());
            }

            tar.transform.SetParent(spawnAnchor.transform);
            tar.SetActive(true);
        }
    }

    private void randSpawns(){
        int tag = UnityEngine.Random.Range(0, poss.Length);
        int ct = rand.Next(5, spawnMax);
        bool tagless = true;

        for (int t=0; t<ct; t++){
            int choice = rand.Next(6);
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
        }
    }
}
