using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MakePixelArt : MonoBehaviour
{
    public GameObject pixelAnchor;
    public int RESOLUTION = 1200;
    public int PIXSIZE = 200;
    public Color32 COLOR = new Color32(45, 224, 203, 255);
    public int ROWSIZE = 12;

    protected internal List<Pixel> pixels = new List<Pixel>();

    public void Start(){
        // Pixel pixel = readPixel("colors");
        // spawnPixelArtFromFile(pixel);
        readPixels(new string[]{"colors"});
        loadPixel(0);
    }

    public void loadPixel(int index){
        spawnPixelArtFromFile(pixels[index]);
    }

    private void readPixels(string[] filenames){
        foreach (string filename in filenames){
            pixels.Add(readPixel(filename));
        }
    }

    private Pixel readPixel(string filename){
        TextAsset read = Resources.Load<TextAsset>(filename);
        Pixel pixel = JsonUtility.FromJson<Pixel>(read.text);
        return pixel;
    }

    private void spawnPixelArt(){
        for (int rowCount=0; rowCount<RESOLUTION/PIXSIZE; rowCount++){
            GameObject row = new GameObject("Row" + rowCount, typeof(RectTransform));
            row.transform.SetParent(pixelAnchor.transform);
            RectTransform anc = row.GetComponent<RectTransform>();
            anc.anchorMin = new Vector2(0f, 1f);
            anc.anchorMax = new Vector2(0f, 1f);
            anc.pivot = new Vector2(0f, 1f);
            anc.sizeDelta = new Vector2(RESOLUTION, 100);
            anc.anchoredPosition = new Vector3(0, -rowCount*PIXSIZE, 0);

            for (int i=0; i<RESOLUTION/PIXSIZE; i++){
                GameObject pix = new GameObject("Pixel" + i);
                pix.transform.SetParent(row.transform);
                Image img = pix.AddComponent<Image>();
                img.color = COLOR;
                RectTransform pixanc = pix.GetComponent<RectTransform>();
                pixanc.anchorMin = new Vector2(0f, 1f);
                pixanc.anchorMax = new Vector2(0f, 1f);
                pixanc.pivot = new Vector2(0f, 1f);
                pixanc.sizeDelta = new Vector2(PIXSIZE, PIXSIZE);
                pixanc.anchoredPosition = new Vector3(i*PIXSIZE, 0, 0);

                pix.SetActive(true);
            }
            row.SetActive(true);
        }
    }

    private void spawnPixelArtFromFile(Pixel pixel){
        //does not contain error checking for file being a different size than the rowsize
        for (int rowCount=0; rowCount<ROWSIZE; rowCount++){
            GameObject row = new GameObject("Row" + rowCount, typeof(RectTransform));
            row.transform.SetParent(pixelAnchor.transform);
            RectTransform anc = row.GetComponent<RectTransform>();
            anc.anchorMin = new Vector2(0f, 1f);
            anc.anchorMax = new Vector2(0f, 1f);
            anc.pivot = new Vector2(0f, 1f);
            anc.sizeDelta = new Vector2(RESOLUTION, 100);
            anc.anchoredPosition = new Vector3(0, -rowCount*PIXSIZE, 0);

            Row currRow = grabRow(pixel, rowCount);

            for (int i=0; i<ROWSIZE; i++){
                Color currColor = grabColor(currRow, i);
                int r = currColor.r;
                int g = currColor.g;
                int b = currColor.b;

                GameObject pix = new GameObject("Pixel" + i);
                pix.transform.SetParent(row.transform);
                Image img = pix.AddComponent<Image>();
                img.color = new Color32((byte)r, (byte)g, (byte)b, 255);
                RectTransform pixanc = pix.GetComponent<RectTransform>();
                pixanc.anchorMin = new Vector2(0f, 1f);
                pixanc.anchorMax = new Vector2(0f, 1f);
                pixanc.pivot = new Vector2(0f, 1f);
                pixanc.sizeDelta = new Vector2(PIXSIZE, PIXSIZE);
                pixanc.anchoredPosition = new Vector3(i*PIXSIZE, 0, 0);

                pix.SetActive(true);
            }
            row.SetActive(true);
        }
    }

    private Row grabRow(Pixel pixel, int count){
        Row r = (Row)pixel.GetType().GetField("Row" + count).GetValue(pixel);
        return r;
    }
    private Color grabColor(Row row, int count){
        Color c = (Color)row.GetType().GetField("Color" + count).GetValue(row);
        return c;
    }
}

[Serializable]
public class Pixel{
    public Row Row0;
    public Row Row1;
    public Row Row2;
    public Row Row3;
    public Row Row4;
    public Row Row5;
    public Row Row6;
    public Row Row7;
    public Row Row8;
    public Row Row9;
    public Row Row10;
    public Row Row11;
}

[Serializable]
public class Row{
    public Color Color0;
    public Color Color1;
    public Color Color2;
    public Color Color3;
    public Color Color4;
    public Color Color5;
    public Color Color6;
    public Color Color7;
    public Color Color8;
    public Color Color9;
    public Color Color10;
    public Color Color11;
}

[Serializable]
public class Color{
    public int r;
    public int g;
    public int b;
}