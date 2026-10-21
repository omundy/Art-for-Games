using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{

    Camera cam;
    private Rigidbody2D rb2d;

    private void Awake()
    {
        cam = Camera.main;
        rb2d = GetComponent<Rigidbody2D>();
    }


    // two methods

    // 1. first randomly picks a new angle on click so junky looking, but rotation
    // feels intuitive
    // https://stackoverflow.com/a/46093089/441878
    //public bool rotating;
    //public Vector2 directionToMouse;
    //public float mouseAngle;
    //public float dragAngle;
    //public float startAngle;
    //public float targetAngle;

    //void OnMouseOver()
    //{
    //    if (rotating)
    //    {

    //    }

    //    // on begin 
    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        Debug.Log("Input.GetMouseButtonDown(1) - on begin drag");
    //        rotating = true;
    //        mouseAngle = GetMouseAngle();
    //        startAngle = Mathf.Atan2(transform.up.x, transform.up.y) * Mathf.Rad2Deg;
    //        dragAngle = mouseAngle - startAngle;

    //    }
    //    // on drag
    //    if (Input.GetMouseButton(1))
    //    {
    //        Debug.Log("Input.GetMouseButton(1) - on drag");
    //        mouseAngle = GetMouseAngle();
    //        targetAngle = mouseAngle - dragAngle;
    //        transform.rotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
    //    }
    //    // on end
    //    if (Input.GetMouseButtonUp(1))
    //    {
    //        Debug.Log("Input.GetMouseButtonUp(1) - on end");
    //        rotating = false;
    //    }
    //}


    //private float GetMouseAngle()
    //{
    //    directionToMouse = GetDirectionToMouse().normalized;
    //    return Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
    //}
    //private Vector2 GetDirectionToMouse()
    //{
    //    var mousePosition = cam.WorldToScreenPoint(transform.position);
    //    return Input.mousePosition - mousePosition;
    //}



    ////public void OnBeginDrag(PointerEventData data)
    ////{
    ////    var directionToMouse = GetDirectionToMouse().normalized;
    ////    var mouseAngle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
    ////    var transAngle = Mathf.Atan2(transform.up.z, transform.up.y) * Mathf.Rad2Deg;
    ////    dragAngle = mouseAngle - transAngle;
    ////}

    ////public void OnDrag(PointerEventData data)
    ////{
    ////    var directionToMouse = GetDirectionToMouse().normalized;
    ////    var mouseAngle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
    ////    var targetAngle = mouseAngle - dragAngle;

    ////    transform.rotation = Quaternion.AngleAxis(targetAngle, Vector3.right);
    ////}


    ////float rotationSpeed = 0.2f;

    ////void OnMouseDrag()
    ////{
    ////    Debug.Log("OnMouseDrag()");
    ////    float XaxisRotation = Input.GetAxis("Mouse X") * rotationSpeed;
    ////    float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSpeed;
    ////    // select the axis by which you want to rotate the GameObject
    ////    transform.Rotate(Vector3.down, XaxisRotation);
    ////    transform.Rotate(Vector3.right, YaxisRotation);
    ////}









    // 2. much simpler method, but rotation is not intuitive because you click
    // and drag at an angle - up/right = CW, down/left = CCW
    // http://gyanendushekhar.com/2018/01/11/rotate-gameobject-using-mouse-drag-or-touch-unity-tutorial/
    //public float rotationSpeed = 55f;
    //public float XaxisRotation;
    //public float YaxisRotation;

    //void OnMouseOver()
    //{
    //    // on begin 
    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        Debug.Log("Input.GetMouseButtonDown(1) - on begin drag");
    //        rotating = true;
    //    }
    //    // on drag
    //    if (Input.GetMouseButton(1))
    //    {
    //        Debug.Log("Input.GetMouseButton(1) - on drag");

    //        XaxisRotation = Input.GetAxis("Mouse X") * rotationSpeed;
    //        YaxisRotation = Input.GetAxis("Mouse Y") * rotationSpeed;
    //        // select the axis by which you want to rotate the GameObject
    //        transform.Rotate(Vector3.forward, XaxisRotation + YaxisRotation);
    //        //transform.Rotate(Vector3.right, YaxisRotation);
    //    }
    //    // on end
    //    if (Input.GetMouseButtonUp(1))
    //    {
    //        Debug.Log("Input.GetMouseButtonUp(1) - on end");
    //        rotating = false;
    //    }
    //}






    // 3. same prblem as #2
    // http://answers.unity.com/answers/1656931/view.html
    //public float mouseSpeedMultiplier = 8;
    //public float smoothSpeed = 0.04f;
    //public float mouseX;

    //void OnMouseOver()
    //{
    //    if (Input.GetMouseButton(1))
    //    {
    //        mouseX += Input.GetAxis("Mouse X") * mouseSpeedMultiplier;
    //    }
    //}

    //void LateUpdate() //Cause we are using Lerp function
    //{
    //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, -mouseX), smoothSpeed);
    //}





    //// 4. same problem as #2/#3 no angles
    //// http://answers.unity.com/answers/177525/view.html

    //public float _sensitivity;
    //public Vector3 _mouseReference;
    //public Vector3 _mouseOffset;
    //public Vector3 _rotation;
    //public bool _isRotating;

    //void Start()
    //{
    //    _sensitivity = 0.4f;
    //    _rotation = Vector3.zero;
    //}

    //void Update()
    //{
    //    if (_isRotating)
    //    {
    //        // offset
    //        _mouseOffset = (Input.mousePosition - _mouseReference);

    //        // apply rotation
    //        _rotation.z = -(_mouseOffset.x + _mouseOffset.y) * _sensitivity;

    //        //_rotation.y = -(_mouseOffset.x) * _sensitivity;
    //        //_rotation.x = -(_mouseOffset.y) * _sensitivity;
    //        // rotate
    //        //transform.Rotate(_rotation);
    //        transform.eulerAngles += _rotation;
    //        // store mouse
    //        _mouseReference = Input.mousePosition;
    //    }

    //}

    //void OnMouseOver()
    //{
    //    //on begin
    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        Debug.Log("Input.GetMouseButtonDown(1) - on begin drag");
    //        _isRotating = true;
    //    }
    //    if (Input.GetMouseButton(1))
    //    {

    //        // rotating flag
    //        //            _isRotating = true;

    //        // store mouse
    //        _mouseReference = Input.mousePosition;
    //    }
    //    else
    //    {
    //        // rotating flag
    //        _isRotating = false;
    //    }
    //    //on end
    //    if (Input.GetMouseButtonUp(1))
    //    {
    //        Debug.Log("Input.GetMouseButtonUp(1) - on end");
    //        _isRotating = false;
    //    }
    //}





    // 5. I now see my issue was that as I rotated the object it would suddenly reverse angles. 
    // Vector2.SignedAngle() keeps the degrees from reversing each rotation
    // https://stackoverflow.com/a/61856080/441878

    public float startRotation;
    public float angleBetween;
    public Vector3 mouseStartPos;
    public Vector3 mouseCurrentPos;
    public Vector3 mouseStartRelativePos;
    public Vector3 mouseCurrentRelativePos;
    public bool rotating;

    private void Update()
    {
        // safety, it seems the reference is lost in scene switching
        if (cam == null) cam = Camera.main;

        // if rotating then update rotate
        if (rotating)
        {
            UpdateRotate();
        }
    }


    // THIS WAS CAUSING ROTATION ISSUES - PROBABLY BECAUSE MY MOUSE IS ACTING FUNKY

    //// stop rotation if user releases (any) mouse
    //private void OnMouseUp()
    //{
    //    Debug.Log("RotateObject.OnMouseUp()");
    //    OnStopRotate();
    //}

    /**
     *  Called from DragObjectManager
     */
    public void OnStartRotate()
    {
        if (rotating) return;
        rotating = true;
        // store start position of mouse
        mouseStartPos = cam.ScreenToWorldPoint(InputManager.Instance.mousePosition);
        // store start rotation of rigidbody
        startRotation = rb2d.rotation;
    }
    public void OnStopRotate()
    {
        rotating = false;
    }
    private void UpdateRotate()
    {
        // update current mouse position
        mouseCurrentPos = cam.ScreenToWorldPoint(InputManager.Instance.mousePosition);

        // update relative positions 
        mouseStartRelativePos = mouseStartPos - transform.position;
        mouseCurrentRelativePos = mouseCurrentPos - transform.position;
        // get angle between two relative positions
        angleBetween = Vector2.SignedAngle(mouseCurrentRelativePos, mouseStartRelativePos);

        Debug.DrawLine(mouseStartPos, mouseCurrentPos, Color.red);
        //Debug.DrawLine(mouseCurrentPos, mouseCurrentRelativePos, Color.blue);

        // rotate object using its rigidbody
        rb2d.MoveRotation(startRotation - angleBetween);
    }



}
