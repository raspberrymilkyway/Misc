using UnityEngine;
using UnityEngine.UI;

// Limit button to everything that is not transparent.
// (Image takes up whole screen for easy placement, but the whole screen should not be clickable.)
// image settings: Sprite (2D and UI); advanced > enable read/write

//usage: object.AddComponent(typeof(ClickOnlyVisible.HideInvisible));

namespace ClickOnlyVisible{
    public class HideInvisible : MonoBehaviour
    {
        void Start(){
            //https://stackoverflow.com/questions/63117045/unity-buttons-non-rectangular-shape
            GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        }
    }
}
