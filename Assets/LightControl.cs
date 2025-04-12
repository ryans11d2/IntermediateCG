using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    [SerializeField] Vector3 turnSpeed = new Vector3(15, 10, 0);
    private bool auto = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt)) auto = !auto;
        if (auto) transform.Rotate(turnSpeed * Time.deltaTime);
    }
}
