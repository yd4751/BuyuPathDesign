using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Bezier : MonoBehaviour {

    // Use this for initialization
    public GameObject line;
    public GameObject objectScreen;
    public GameObject txtStatus;

    private List<Vector3> input;
    private List<Vector3> result;
    bool bStartDraw = false;
    Vector3 vLastMousePosition = Vector3.zero;
    List<Vector3> lMousePosRecord; 

    bool bShow = false;
    private LineRenderer lineRender;
    private Text showStatus;
    void Start () {
        input = new List<Vector3>();
        result = new List<Vector3>();
        lMousePosRecord = new List<Vector3>();
        lineRender = line.GetComponent<LineRenderer>();
        showStatus = txtStatus.GetComponent<Text>();
        //测试背景图片层级
        //objectScreen.transform.SetAsFirstSibling();
        //line.transform.SetAsFirstSibling();
    }
    int nInputStatus = 0; //0-get  1-draw 2-none
	// Update is called once per frame
    void ChangeStatus()
    {
        nInputStatus = (nInputStatus + 1) % 3;
        showStatus.text = "当前状态:" + nInputStatus;

        switch (nInputStatus)
        {
            case 0:
                input.Clear();
                bShow = false;
                result.Clear();
                lineRender.enabled = false;
                break;
            case 1:
                OnDrawLine();
                break;
            case 2:
                break;
        }
    }
    void OnGetInput()
    {
        if (nInputStatus != 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z += 1;
            input.Add(pos);

            Debug.Log("CurInputSize:" + input.Count + " : " + Input.mousePosition.ToString());
        }
    }
    void OnDrawLine()
    {
        if (nInputStatus != 1) return;
        if (input.Count == 0) return;

        for (int i = 0; i < 200; i++)
        {
            result.Add(BezierLine(i / 200f, input));
        }

        lineRender.positionCount = result.Count;
        Vector3[] pos = new Vector3[lineRender.positionCount];
        for (int i = 0; i < lineRender.positionCount; i++)
            pos[i] = result[i];
        lineRender.endColor = Color.green;
        lineRender.SetPositions(pos);
        lineRender.enabled = true;

        input.Clear();
        result.Clear();
    }
    void OnDoNothing()
    {
        if (nInputStatus != 3) return;
    }
	void Update () {

        if(Input.GetMouseButtonUp(0))
        {
            //
            nInputStatus = 1;
            for(int i=0; i< lMousePosRecord.Count; i++)
                input.Add(lMousePosRecord[i]);

            Debug.Log("Total:" + lMousePosRecord.Count);
            vLastMousePosition = Vector3.zero;
            bStartDraw = false;
            lMousePosRecord.Clear();
            lineRender.enabled = false;

            //画曲线
            OnDrawLine();

        }
        if (Input.GetMouseButtonDown(0))
        {
            bStartDraw = true;
            //ChangeStatus();
        }

        //OnGetInput();
        DrawLine();


    }
    void DrawLine()
    {
        if (!bStartDraw) return ;

        Vector3 curMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        curMouse.z += 1;
        if (vLastMousePosition == Vector3.zero)
        {
            vLastMousePosition = curMouse;
        }
        else
        {
            if( (curMouse - vLastMousePosition).normalized != Vector3.zero)
            {
                lMousePosRecord.Add(curMouse);
            }
        }
        if(lMousePosRecord.Count > 1)
        {
            Vector3[] pos = new Vector3[lMousePosRecord.Count];
            lMousePosRecord.CopyTo(pos);
            lineRender.positionCount = lMousePosRecord.Count;
            lineRender.SetPositions(pos);
            lineRender.enabled = true;
        }
    }
    //每传入一个插值，获得一个目标点， 多个插值获得一组点，就是最后的曲线
    public Vector3 BezierLine(float t, List<Vector3> p)
    {
        if (p.Count < 2)
            return p[0];
        List<Vector3> newp = new List<Vector3>();
        for (int i = 0; i < p.Count - 1; i++)
        {
            Debug.DrawLine(p[i], p[i + 1]);
            Vector3 p0p1 = (1 - t) * p[i] + t * p[i + 1];
            newp.Add(p0p1);
        }
        return BezierLine(t, newp);
    }

}
