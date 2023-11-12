using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float HorizontalInput;
    public float VerticalInput;
    public bool IsLeftMouseDown;
    public bool IsSpaceKeyDown;
    // Update is called once per frame
    void Update()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
        if (!IsLeftMouseDown && Time.timeScale != 0)
        {
            IsLeftMouseDown = Input.GetMouseButtonDown(0);
        }
        if (!IsSpaceKeyDown && Time.timeScale != 0)
        {
            IsSpaceKeyDown = Input.GetKeyDown(KeyCode.Space);
        }
    }
    private void OnDisable() 
    {
        ClearCache();
    }

    public void ClearCache()
    {
        IsLeftMouseDown = false;
        IsSpaceKeyDown = false;
        
        HorizontalInput = 0f;
        VerticalInput = 0f;
    }
}
