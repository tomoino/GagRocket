using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class launch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void pushbutton(){
        Debug.Log("ボタンを押した");
        GameObject obj = GameObject.Find("Sphere");  
        obj.transform.position += Vector3.up;
    }
}
