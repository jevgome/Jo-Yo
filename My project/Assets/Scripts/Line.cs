using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{

    public LineRenderer line;
    public GameObject player;

    Color lineColor = new Color(0, 0, 0, 1);
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.SetColors(lineColor,lineColor);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseposition = Input.mousePosition;
        mouseposition.z = Camera.main.nearClipPlane;
        mouseposition = Camera.main.ScreenToWorldPoint(mouseposition);

        if (Input.GetMouseButton(0))
        {
            line.enabled = true;
            line.SetPosition(0, player.transform.position);
            line.SetPosition(1,mouseposition);
        } else
        {
            line.enabled = false; 
        }

        
        
    }
}
