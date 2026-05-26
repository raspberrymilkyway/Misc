using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class WantedMovement : MonoBehaviour
{
    //thinking this should separate out like...
    // image translations (grow, spin, move) and image tagging.
    // i could use these for both wanted and like. hidden object

    //give this established images

    // sawtooth needs to be slower. the movement looks super fast gmao
    
    private (bool spin, bool dir, float speed, int turnCount) spinDir = (false, false, 0f, 0); //false for anti, true for clockwise
    private (bool grow, int dir, float interval) growDir = (false, 0, 0f); //-1 shrink, 0 grow and shrink, 1 grow
    private (bool move, bool keepMoving, float angle, float dist) moveDist = (false, false, 0f, 0f); //angle is in radians, apparently
    private (bool move, float dist, string style) moveStyle = (false,  0f, ""); //styles: see Wanted styles
    private int spinCount = 0;
    private float growMax = 2f;
    private float growMin = 0.5f;
    private bool growing = true;
    private bool cont = true;
    private bool setIni = false;
    private int mult = 1;
    private int stylei = -1;
    private int movei = 0;
    private int rep = 0;
    private Vector3 ini;
    private System.Random rand;

    private (float x, float y) parentSize = (0f,0f);
    private (float x, float y) parentCoor = (0f,0f);

    private List<List<float>> styles = new List<List<float>>{
        new List<float>{2.356194f}, //diagonal left
        new List<float>{}, //sine - offset, not angle. generate at start
        new List<float>{0f, 1.570796f, 0f, -1.570796f}, //clock
        new List<float>{1.047198f, -1.047198f, -1.047198f, 1.047198f}, //sawtooth
        new List<float>{}  //random - generate at start
    };
    private string[] stylesNames = new string[]{"diagonalLeft", "sine", "clock", "sawtooth", "random"};

    void Start(){
        rand = new System.Random();
        styles[1].Add((float)(rand.NextDouble()-0.5)*2); //offset between -1 and 1
        
        int ct = rand.Next(3, 20);
        for (int i=0; i<ct; i++){
            double v = (rand.NextDouble() - 0.5)* 2;
            styles[styles.Count-1].Add((float)v * 6.283185f);
        }

        RectTransform parent = (RectTransform)this.transform.parent;
        parentCoor = (parent.position.x, parent.position.y);
        parentSize = (parent.sizeDelta.x, parent.sizeDelta.y);
    }
    
    void Update(){
        if (spinDir.spin && (spinDir.turnCount < 0 || spinCount < spinDir.turnCount)){
            //does this acutally match with speed?
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + spinDir.speed * mult);
            spinCount++;
        }
        if (growDir.grow){
            Vector3 curr = this.GetComponent<RectTransform>().sizeDelta;
            if (!setIni){
                ini = curr;
                setIni = true;
            }
            float calc = curr.x / ini.x;
            if (calc >= growMax){
                growing = false;
                cont = false;
            }
            else if (calc <= growMin){
                growing = true;
                cont = false;
            }

            float x = curr.x;
            float y = curr.y;
            if ((growDir.dir < 0 && cont) || (growDir.dir == 0 && !growing)){
                x = curr.x - growDir.interval;
                y = curr.y - growDir.interval;
            }
            else if ((growDir.dir > 0 && cont) || (growDir.dir == 0 && growing)){
                x = curr.x + growDir.interval;
                y = curr.y + growDir.interval;
            }
            this.GetComponent<RectTransform>().sizeDelta = new Vector3 (x, y, 1f);
        }
        if (moveDist.keepMoving){
            move((double)moveDist.angle, moveDist.dist);
        }
        else if (moveStyle.move){
            if (stylei == 0){
                move((double)styles[stylei][0], moveStyle.dist);
            }
            else if (stylei == 1){
                sine((double)Time.time * 2f + styles[stylei][0], moveStyle.dist, 4f);
            }
            else if (stylei == 2){
                clock((double)styles[stylei][movei%styles[stylei].Count], moveStyle.dist);
            }
            else if (stylei == 3){
                sawtooth((double)styles[stylei][movei%styles[stylei].Count], moveStyle.dist);
            }
            else{ // this usually looks pretty ugly. not really sure what i was going for
                float d = (float)(rand.NextDouble() * (moveStyle.dist*1.25) + 0.5);
                move((double)styles[stylei][movei%styles[stylei].Count], d);
            }
        }
    }

    protected internal void setSpin(bool spin, bool direction, float speed, int turnCount){
        spinDir = (spin, direction, speed, turnCount);
        spinCount = 0;
        mult = direction ? -1 : 1;
    }
    protected internal void setGrow(bool grow, int direction, float interval, float maxGrow=0f, float minGrow=0f){
        // please pass grow cap if growing and shrink cap if shrinking
        growDir = (grow, direction, interval);
        if (maxGrow != 0f){
            growMax = maxGrow;
        }
        if (minGrow != 0f){
            growMin = minGrow;
        }
        setIni = false;
        growing = true;
        cont = true;
    }

    // move dist and style cannot both be run at once! pick one.
    protected internal void setMoveDist(bool move, bool keepMoving, float angle, float distance){
        moveDist = (move, keepMoving, angle, distance);
        if (move && !keepMoving){
            //go ahead and scoot
            double sin = Math.Sin((double)moveDist.angle)*moveDist.dist;
            double cos = Math.Cos((double)moveDist.angle)*moveDist.dist;
            transform.position = transform.position + new Vector3((float)cos, (float)sin, 0);
        }
    }
    protected internal void setMoveStyle(bool move, float distance, string style){
        moveStyle = (move, distance, style);
        movei = 0;
        int o = Array.IndexOf(stylesNames, style);
        stylei =  o >= 0 ? o : stylesNames.Length-1;
    }

    protected internal void stopSpin(){
        spinDir.spin = false;
    }
    protected internal void stopGrow(){
        growDir.grow = false;
    }
    protected internal void stopMove(){
        moveDist.move = false;
        moveDist.keepMoving = false;
        moveStyle.move = false;
        movei = 0;
    }

    private void move(double angle, float distance){
        checkBounds();
        double sin = Math.Sin(angle)*distance;
        double cos = Math.Cos(angle)*distance;
        transform.position = transform.position + new Vector3((float)cos, (float)sin, 0);
    }
    private void sine(double angle, float distance, float dividend=1f){
        //dividend bc the sine wave can be a little big, but the x distance shouldn't change
        checkBounds();
        transform.position = transform.position + new Vector3(distance, (float)Math.Sin(angle) * distance/dividend, 0);
    }
    private void clock(double angle, float distance){
        checkBounds();
        if (rep == 100){
            rep = 0;
            movei++;
        }
        float f = (float)(Math.Sin(angle) * distance);
        transform.position = transform.position + new Vector3(distance, f, 0);
        rep++;
    }
    private void sawtooth(double angle, float distance){
        if (rep == 100){
            rep = 0;
            movei++;
        }
        move(angle, distance);
        rep++;
    }
    private void checkBounds(){
        // too far? keep bounds
        // allows about half the image off-screen
        // https://stackoverflow.com/a/70970228

        // bottom left corner is (0,0). swap ifs based on coordinates
        if (transform.position.x < parentCoor.x){
            transform.position = new Vector3(parentCoor.x + parentSize.x, transform.position.y, 0);
        }
        else if (transform.position.x >= parentCoor.x + parentSize.x){
            transform.position = new Vector3(parentCoor.x, transform.position.y, 0);
        }
        if (transform.position.y <= parentCoor.y - parentSize.y){
                transform.position = new Vector3(transform.position.x, parentCoor.y, 0);
        }
        else if (transform.position.y >= parentCoor.y){
            transform.position = new Vector3(transform.position.x, parentCoor.y - parentSize.y, 0);
        }
    }
}