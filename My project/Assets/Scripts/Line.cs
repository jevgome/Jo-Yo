using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{

    LineRenderer line;
    public GameObject player;
    public GameObject yoyo;

    Color lineColor = new Color(255, 255, 255, 1);
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.SetColors(lineColor,lineColor);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(1))
        {
            line.enabled = true;
            line.SetPosition(0, player.transform.position);
            line.SetPosition(1,yoyo.transform.position);
        } else
        {
            line.enabled = false; 
        }

        
        
    }
}
