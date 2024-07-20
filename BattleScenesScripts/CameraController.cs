using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class CameraController : MonoBehaviour//, IPointerDownHandler
{
    //public enum CameraModes { Follow, Isometric, Free }

    private Transform cameraTransform;
    //private Transform dummyTarget;
    public Transform CameraTarget;
    public ClickController clickController;

    public float FollowDistance = 30.0f;
    public float MaxFollowDistance = 100.0f;
    public float MinFollowDistance = 2.0f;

    //public float ElevationAngle = 30.0f;
    //public float MaxElevationAngle = 85.0f;
    //public float MinElevationAngle = 0f;

    public float OrbitalAngle = 0f;

    //public CameraModes CameraMode = CameraModes.Follow;

    public bool MovementSmoothing = true;
    public bool RotationSmoothing = false;
    private bool previousSmoothing;

    public float MovementSmoothingValue = 25f;
    public float RotationSmoothingValue = 5.0f;

    public float MoveSensitivity = 2.0f;

    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 desiredPosition;
    private float mouseX;
    private float mouseY;
    private Vector3 moveVector;
    private float mouseWheel;
    public float minOrthographicSize;
    public float maxOrthographicSize;
    public float thresholdForMouseClick;
    public float thresholdForTouch;
    private LayerMask shipLayer;
    public float rayDistance;
    public bool canCreateUSP = true;


    // Controls for Touches on Mobile devices
    //private float prev_ZoomDelta;


    private const string event_SmoothingValue = "Slider - Smoothing Value";
    private const string event_FollowDistance = "Slider - Camera Zoom";


    void Awake()
    {
        //print("qualitySetting.vSyncCount = " + QualitySettings.vSyncCount + 
        //    " \nApplication.targetFrameRate = " + Application.targetFrameRate);
        //print("Input.simulateMouseWithTouches = " + Input.simulateMouseWithTouches);

        if (QualitySettings.vSyncCount > 0)
            Application.targetFrameRate = 60;
        else
            Application.targetFrameRate = -1;

        //if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        //    Input.simulateMouseWithTouches = false;

        cameraTransform = transform;
        previousSmoothing = MovementSmoothing;
    }


    void Start()
    {
        shipLayer = LayerMask.GetMask("ShipLayer");
        Debug.Log($"go: {gameObject.name}");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        GetPlayerInput();


        // Check if we still have a valid target
        if (CameraTarget != null)
        {
            //if (CameraMode == CameraModes.Isometric)
            {
                //desiredPosition = CameraTarget.position + Quaternion.Euler(ElevationAngle, OrbitalAngle, 0f) * new Vector3(0, 0, -FollowDistance);
            }
            //else if (CameraMode == CameraModes.Follow)
            {
                desiredPosition = CameraTarget.position + CameraTarget.TransformDirection(Quaternion.Euler(0f, OrbitalAngle, 0f) * (new Vector3(0, 0, -FollowDistance)));
            }
            //else
            //{
            //    // Free Camera implementation
            //}

            if (MovementSmoothing == true)
            {
                // Using Smoothing
                cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, desiredPosition, ref currentVelocity, MovementSmoothingValue * Time.fixedDeltaTime);
                //cameraTransform.position = Vector3.Lerp(cameraTransform.position, desiredPosition, Time.deltaTime * 5.0f);
            }
            else
            {
                // Not using Smoothing
                cameraTransform.position = desiredPosition;
            }

            //if (RotationSmoothing == true)
            //    cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.LookRotation(CameraTarget.position - cameraTransform.position), RotationSmoothingValue * Time.deltaTime);
            //else
            //{
            //    cameraTransform.LookAt(CameraTarget);
            //}

        }

    }



    void GetPlayerInput()
    {
        moveVector = Vector3.zero;

        // Check Mouse Wheel Input prior to Shift Key so we can apply multiplier on Shift for Scrolling
        mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        float touchCount = Input.touchCount;

        //if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || touchCount > 0)
        {
            //mouseWheel *= 10;

            //if (Input.GetKeyDown(KeyCode.I))
            //    CameraMode = CameraModes.Isometric;

            //if (Input.GetKeyDown(KeyCode.F))
            //    CameraMode = CameraModes.Follow;

            //if (Input.GetKeyDown(KeyCode.S))
            //    MovementSmoothing = !MovementSmoothing;


            

            // Get Input from Mobile Device
            if (touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                print("touch count = 1 !");

                Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;

                // Handle elevation changes
                //if (deltaPosition.y > 0.01f || deltaPosition.y < -0.01f)
                //{
                //    ElevationAngle -= deltaPosition.y * 0.1f;
                //    // Limit Elevation angle between min & max values.
                //    ElevationAngle = Mathf.Clamp(ElevationAngle, MinElevationAngle, MaxElevationAngle);
                //}


                // Handle left & right 
                if (deltaPosition.x > 0.01f || deltaPosition.x < -0.01f)
                {
                    OrbitalAngle += deltaPosition.x * 0.1f;
                    if (OrbitalAngle > 360)
                        OrbitalAngle -= 360;
                    if (OrbitalAngle < 0)
                        OrbitalAngle += 360;
                }

            }

            //if(Input.GetMouseButtonDown(0))
            //{
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //    if(canCreateUSP)
            //        clickController.CreateUSP(ray.origin);
            //}


            //Check for right mouse button to move camera
            if (Input.GetMouseButton(1))
            {
                //print("Mouse button 1 was pressed!");

                mouseY = Input.GetAxis("Mouse Y");
                mouseX = Input.GetAxis("Mouse X");
                Vector3 moveCamera = new Vector3(-mouseX, -mouseY, 0) * Camera.main.orthographicSize / 10;
                cameraTransform.position += moveCamera;

            }


        }

        // Check Pinching to Zoom in - out on Mobile device
        if (touchCount == 2)
        {
            print("touch count = 2 !");

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevTouchDelta = (touch0PrevPos - touch1PrevPos).magnitude;
            float touchDelta = (touch0.position - touch1.position).magnitude;

            float zoomDelta = prevTouchDelta - touchDelta;

            if (zoomDelta > 0.01f || zoomDelta < -0.01f)
            {
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize += zoomDelta, minOrthographicSize, maxOrthographicSize);
            }

        }

        // Check MouseWheel to Zoom in-out
        if (mouseWheel < -0.01f || mouseWheel > 0.01f)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize -= mouseWheel, minOrthographicSize, maxOrthographicSize);
        }


    }

    public void ClickIsNotInAnEmptyPlace()
    {
        StartCoroutine(ProhibitingOfCreationUSP());
    }

    private IEnumerator ProhibitingOfCreationUSP()
    {
        canCreateUSP = false;
        yield return null;
        canCreateUSP = true;
    }


    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    print(eventData);
    //    print(eventData.position);
    //    print(eventData.pointerClick);
    //    print(eventData.pointerCurrentRaycast.gameObject);
    //}
}
