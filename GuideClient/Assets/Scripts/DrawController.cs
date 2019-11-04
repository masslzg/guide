using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawController : MonoBehaviour
{
    public Transform DrawArea;
    public Material material;
    public Image[] ColorButtonImages;
    public Sprite[] ColorDefaultSprites;
    public Sprite[] ColorSelectedSprites;

    Color[] colors = { Color.white,Color.yellow,Color.green,Color.cyan,Color.blue,Color.magenta,Color.red,Color.black};

    List<Vector3> linePoints = new List<Vector3>();
    LineRenderer lineRenderer;
    public float startWidth = 5.0f;
    public float endWidth = 5.0f;
    public float threshold = 0.001f;
    Camera thisCamera;
    int lineCount = 0;
    int currLines = 0;

    Vector3 lastPos = Vector3.one * float.MaxValue;
    Color currentColor;

    void Awake()
    {
        thisCamera = Camera.main;        
        Reset();
    }

    public void Clear()
    {
        foreach (Transform child in DrawArea)
        {
            Destroy(child.gameObject);
        }
    }

    public void Reset()
    {
        Clear();
        ChangeColor(0);
    }

    public void ChangeColor(int index)
    {
        currentColor = colors[index];
        for (int i = 0; i < ColorButtonImages.Length; i++)
        {
            if (i == index)
                ColorButtonImages[i].sprite = ColorSelectedSprites[i];
            else
                ColorButtonImages[i].sprite = ColorDefaultSprites[i];
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)&& lineRenderer == null)
        {
            createLine();            
        }
        else if (Input.GetMouseButton(0) && lineRenderer!=null)
        {
            Vector3 mousePos = Input.mousePosition;            
            Vector3 mouseWorld = thisCamera.ScreenToWorldPoint(mousePos);
            mouseWorld.z = -10;

            float dist = Vector3.Distance(lastPos, mouseWorld);
            if (dist <= threshold)
                return;

            Debug.Log(mouseWorld);
            lastPos = mouseWorld;
            linePoints.Add(mouseWorld);

            UpdateLine();
        }
        else if (Input.GetMouseButtonUp(0) && lineRenderer!=null)
        {
            linePoints = null;
            lineRenderer = null;
            lineCount = 0;
            lastPos = Vector3.one * float.MaxValue;
        }
    }


    void UpdateLine()
    {
        lineRenderer.startWidth=startWidth;
        lineRenderer.endWidth=endWidth;
        lineRenderer.positionCount = linePoints.Count;

        for (int i = lineCount; i < linePoints.Count; i++)
        {
            lineRenderer.SetPosition(i, linePoints[i]);
        }
        lineCount = linePoints.Count;
    }

    void createLine()
    {
        lineRenderer = new GameObject("Line" + currLines).AddComponent<LineRenderer>();
        lineRenderer.material = material;
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.numCapVertices = 50;
        lineRenderer.transform.SetParent(DrawArea);
        lineRenderer.startColor = currentColor;
        lineRenderer.endColor = currentColor;
        currLines++;

        if (linePoints == null)
            linePoints = new List<Vector3>();
    }

    


}
