using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSelect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetProperties(Material mat) {
        mat.GetTexturePropertyNames();
    }

}
