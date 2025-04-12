using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public bool interact = true;
    public int mat = 0;
    Rigidbody rb;
    GameObject outline;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        outline = Instantiate(new GameObject());
        outline.transform.position = transform.position;
        outline.transform.rotation = transform.rotation;
        outline.transform.localScale = transform.localScale;
        outline.transform.parent = gameObject.transform;
        outline.AddComponent<MeshFilter>();
        outline.GetComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
        outline.AddComponent<MeshRenderer>();
        outline.GetComponent<MeshRenderer>().material = FindObjectOfType<SceneManager>().outline;
        ToggleOutline(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMaterial(Material mat) {
        GetComponent<Renderer>().material = mat;
    }

    public void Push(Vector3 pos, Vector3 dir, float force) {
        //rb.AddForceAtPosition(dir * force, Vector3.zero, ForceMode.Impulse);
        Debug.Log(dir * force);
        rb.AddForce(dir * force, ForceMode.Impulse);
    }

    public void ToggleOutline(bool on) {
        outline.SetActive(on);
    }

}
