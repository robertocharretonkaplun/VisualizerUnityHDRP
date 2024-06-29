using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleV2 : MonoBehaviour
{
    public GameObject ObjectToEnable;
    public GameObject ObjectToDisable;
   
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnToggleButtonClick()
    {
        ObjectToEnable.SetActive(!ObjectToEnable.activeSelf);
        ObjectToDisable.SetActive(!ObjectToDisable.activeSelf);
        
    }
}
