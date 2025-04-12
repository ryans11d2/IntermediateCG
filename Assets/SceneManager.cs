using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SceneManager : MonoBehaviour
{
    
    private Vector3 pos;

    [Header("Motion")]
    [SerializeField] Vector2 sensitivity = new Vector2(0.5f, 0.5f);
    [SerializeField] Transform points;
    [SerializeField] float zoomSpeed = 1;
    [SerializeField] Transform pointer;
    private int movePoint = 1;
    private int lastPoint = 1;
    private Vector3 mousePos;

    [Header("Interaction")]
    [SerializeField] float pushForce = 5;
    [SerializeField] float pullForce = 1;
    private GameObject pullObject = null;
    private ObjectManager lookObject = null;

    [Header("Materials")]
    [SerializeField] public Material outline;
    [SerializeField] Material[] Materials;

    [Header("Post")]
    [SerializeField] Material[] PostMaterials = new Material[9];
    [SerializeField] FullScreenPassRendererFeature Feature;

    void Start()
    {
        pos = transform.position;
        //Cursor.lockState = CursorLockMode.Locked;

    }

    void FixedUpdate()
    {
        /*
        float newZ = 0;
        if (Input.GetAxis("Mouse ScrollWheel") > 0) newZ = Mathf.Clamp(transform.position.z - zoomSpeed, minZoom, maxZoom);
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) newZ = Mathf.Clamp(transform.position.z + zoomSpeed, minZoom, maxZoom);
        else newZ = transform.position.z;
        pos = Vector3.forward * newZ;
        transform.position = pos;
        */

        //transform.position = transform.position + transform.forward * Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        if (Input.GetKey(KeyCode.Space)) {

            if (Vector3.Distance(transform.position, points.GetChild(movePoint).transform.position) < 0.1f) {
                lastPoint = movePoint;
                Debug.Log("Point " + movePoint);
            }
            movePoint = lastPoint + 1;
            if (movePoint >= points.childCount) movePoint = 0;

            transform.position = Vector3.MoveTowards(transform.position, points.GetChild(movePoint).transform.position, zoomSpeed * Time.deltaTime);
            float spread = Vector3.Distance(points.GetChild(lastPoint).transform.position, points.GetChild(movePoint).transform.position);
            float dist = Vector3.Distance(transform.position, points.GetChild(movePoint).transform.position);
            
            Debug.Log(dist / spread);
            pointer.LookAt(points.GetChild(movePoint).transform.position);
            Vector3 newRot = new Vector3(transform.eulerAngles.x, pointer.transform.eulerAngles.y, transform.eulerAngles.z);
            newRot.z = Mathf.Lerp(points.GetChild(movePoint).transform.eulerAngles.z, points.GetChild(lastPoint).transform.eulerAngles.z, dist / spread);
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, newRot, zoomSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftShift)) {

            if (Vector3.Distance(transform.position, points.GetChild(movePoint).transform.position) < 0.1f) {
                lastPoint = movePoint;
            }
            movePoint = lastPoint - 1;
            if (movePoint < 0) movePoint = points.childCount - 1;

            transform.position = Vector3.MoveTowards(transform.position, points.GetChild(movePoint).transform.position, zoomSpeed * Time.deltaTime);
            float spread = Vector3.Distance(points.GetChild(lastPoint).transform.position, points.GetChild(movePoint).transform.position);
            float dist = Vector3.Distance(transform.position, points.GetChild(movePoint).transform.position);
            
            pointer.LookAt(points.GetChild(movePoint).transform.position);
            Vector3 newRot = new Vector3(transform.eulerAngles.x, pointer.transform.eulerAngles.y, transform.eulerAngles.z);
            newRot.z = Mathf.Lerp(points.GetChild(movePoint).transform.eulerAngles.z, points.GetChild(lastPoint).transform.eulerAngles.z, dist / spread);
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, newRot, zoomSpeed * Time.deltaTime);
        }

        /*
        Vector3 lookDelta = new Vector3(-(Input.mousePosition.y - mousePos.y) * sensitivity.y, (Input.mousePosition.x - mousePos.x) * sensitivity.x, 0);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x + lookDelta.x, transform.eulerAngles.y + lookDelta.y, transform.eulerAngles.z);
        mousePos = Input.mousePosition;
        */
    }

    void Update()
    {   

        
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100)) {
            if (hit.collider.gameObject.TryGetComponent<ObjectManager>(out ObjectManager ob)) {
                if (lookObject == null) lookObject = ob;
                if (lookObject != ob) lookObject.ToggleOutline(false);
                lookObject = ob;
                lookObject.ToggleOutline(true);
            }
        }
        
        if (Input.GetMouseButtonDown(2)) SwitchObject();

        if (Input.GetMouseButton(1)) {
            if (pullObject == null) PullObject();
            else {
                pullObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                Vector3 dir = (transform.position - pullObject.transform.position).normalized;
                if (Vector3.Distance(transform.position, pullObject.transform.position) > 10) {
                    pullObject.transform.position = Vector3.Lerp(pullObject.transform.position, transform.position, 0.3f * pullForce * Time.deltaTime);
                }
                else pullObject.transform.position = Vector3.MoveTowards(pullObject.transform.position, transform.position, pullForce * Time.deltaTime);
            }
        }
        else if (Input.GetMouseButtonDown(0)) PushObject();
        else pullObject = null;

        if (Input.GetKeyDown(KeyCode.Alpha0)) SetPost(-1);
        else if (Input.GetKeyDown(KeyCode.Alpha1)) SetPost(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SetPost(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SetPost(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) SetPost(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) SetPost(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6)) SetPost(5);
        else if (Input.GetKeyDown(KeyCode.Alpha7)) SetPost(6);
        else if (Input.GetKeyDown(KeyCode.Alpha8)) SetPost(7);
        else if (Input.GetKeyDown(KeyCode.Alpha9)) SetPost(8);

    }

    private ObjectManager GetObject() {

        if (Physics.Raycast(Camera.main.ScreenPointToRay(transform.position), out RaycastHit hit, 100)) {
            if (hit.collider.gameObject.TryGetComponent<ObjectManager>(out ObjectManager o)) return o;
        }
        return null;
    }

    private void PushObject() {
        Debug.Log("Push");
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100)) {
            if (hit.collider.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb)) {
                Vector3 dir = (transform.position - hit.point).normalized;
                rb.AddForceAtPosition(-dir * pushForce, hit.point, ForceMode.Impulse);
            }
        }
    }

    private void PullObject() {
        Debug.Log("Pull");
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1024)) {
            if (hit.collider.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb)) {
                if (!rb.isKinematic) pullObject = hit.collider.gameObject;
            }
        }
    }

    private void SwitchObject() {
        Debug.Log("Switch");
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1024)) {
            if (hit.collider.gameObject.TryGetComponent<ObjectManager>(out ObjectManager obj)) {
                int idx = obj.mat;
                idx++;
                if (idx >= Materials.Length) idx = 0;
                obj.SetMaterial(Materials[idx]);
                obj.mat = idx;
            }
        }

    }

    private void SetPost(int idx) {
        if (idx == -1) Feature.passMaterial = null;
        Feature.passMaterial = PostMaterials[idx];
    }

}
