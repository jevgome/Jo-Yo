using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{

    public LineRenderer line;
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject yoyo;

    public Color lineColor = new Color(255, 255, 255, 1);
    // Start is called before the first frame update
    public void Start()
    {
        line = GetComponent<LineRenderer>();
        line.startColor = lineColor;
        line.endColor = lineColor;
    }

    // Update is called once per frame
    public void Update()
    {

        if (yoyo.GetComponent<SpriteRenderer>().enabled)
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
