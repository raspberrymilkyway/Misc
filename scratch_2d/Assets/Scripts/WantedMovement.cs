using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(RectTransform))]
public class WantedMovement : MonoBehaviour
{
    //thinking this should separate out like...
    // image translations (grow, spin, move) and image tagging.
    // i could use these for both wanted and like. hidden object

    //give this established images
    
    private (bool spin, bool dir, float speed, int turnCount) spinDir = (false, false, 0f, 0); //false for anti, true for clockwise
    private (bool grow, int dir, float interval) growDir = (false, 0, 0f); //-1 shrink, 0 grow and shrink, 1 grow
    private (bool move, bool keepMoving, float angle, float dist) moveDist = (false, false, 0f, 0f); //angle is in radians, apparently
    private int spinCount = 0;
    private float growMax = 2f;
    private float growMin = 0.5f;
    private bool growing = true;
    private bool cont = true;
    private bool setIni = false;
    private Vector3 ini;
    
    void Update(){
        if (spinDir.spin && (spinDir.turnCount < 0 || spinCount < spinDir.turnCount)){
            //does this acutally match with speed?
            int mult = 1;
            if (!spinDir.dir){
                mult = -1;
            }
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
            // too far? keep bounds
            // this allows like... half the image off-screen, i think
            if (transform.position.x < Screen.safeArea.xMin){
                transform.position = new Vector3(Screen.safeArea.xMax, transform.position.y, 0);
            }
            else if (transform.position.x >= Screen.safeArea.xMax){
                transform.position = new Vector3(Screen.safeArea.xMin, transform.position.y, 0);
            }
            if (transform.position.y <= Screen.safeArea.yMin){
                 transform.position = new Vector3(transform.position.x, Screen.safeArea.yMax, 0);
            }
            else if (transform.position.y >= Screen.safeArea.yMax){
                transform.position = new Vector3(transform.position.x, Screen.safeArea.yMin, 0);
            }

            double sin = Math.Sin((double)moveDist.angle)*moveDist.dist;
            double cos = Math.Cos((double)moveDist.angle)*moveDist.dist;
            transform.position = transform.position + new Vector3((float)cos, (float)sin, 0);
        }
    }

    protected internal void setSpin(bool spin, bool direction, float speed, int turnCount){
        spinDir = (spin, direction, speed, turnCount);
        spinCount = 0;
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
    protected internal void setMove(bool move, bool keepMoving, float angle, float distance){
        moveDist = (move, keepMoving, angle, distance);
        if (move && !keepMoving){
            //go ahead and scoot
            double sin = Math.Sin((double)moveDist.angle)*moveDist.dist;
            double cos = Math.Cos((double)moveDist.angle)*moveDist.dist;
            transform.position = transform.position + new Vector3((float)cos, (float)sin, 0);
        }
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
    }
}