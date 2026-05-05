using UnityEngine;

//script to add a reference to game object to another game object
// just to use in spot the difference

public class Paralleled : MonoBehaviour
{
    GameObject p;

    protected internal GameObject parallel{
        get => p;
        set => p = value;
    }
}