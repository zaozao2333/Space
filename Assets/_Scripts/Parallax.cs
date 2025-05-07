using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public GameObject poi;
    public GameObject[] panels;
    public float scrollSpeed = -30f;

    public float motionmult = 0.25f;
    private float panelHt;
    private float depth;
    // Start is called before the first frame update
    void Start()
    {
        panelHt = panels[0].transform.localScale.y;
        depth = panels[0].transform.localScale.z;
        panels[0].transform.position = new Vector3(0, 0, depth);
        panels[1].transform.position = new Vector3(0, panelHt, depth);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float tY, tX = 0;
        tY = Time.time * scrollSpeed % panelHt + (panelHt * 0.5f);
        if (poi != null)
        {
            tX = -poi.transform.position.x * motionmult;
        }
        panels[0].transform.position = new Vector3(tX, tY, depth);
        if (tY > 0)
        {
            panels[1].transform.position = new Vector3(tX, tY - panelHt, depth);
        }
        else
        {
            panels[1].transform.position = new Vector3(tX, tY + panelHt, depth);
        }
    }
}
