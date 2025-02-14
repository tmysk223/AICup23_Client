using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    Camera cam;

    [Header("MasterSetting")]
    [SerializeField] CameraSettings settings;


    [Space(20)]
    [Header("Movement")]
    [SerializeField] float strafeSpeed = 1f;
    [SerializeField] float forwardSpeed = 1f;
    [SerializeField] float backwardSpeed = 1f;
    [SerializeField] float shiftMultiplier = 2f;

    [Space(20)]
    [Header("Free View")]
    [SerializeField] float sensitivity = 1f;

    [Space(20)]
    [Header("Focus")]
    [SerializeField] float focusSpeed = 1f;
    [SerializeField] float focusDistance = 10f;
    [SerializeField] float zoomSpeed = 1f;
    [SerializeField] Vector2 fovRange = new Vector2(30, 100);

    [Space(20)]
    [Header("Boundries")]
    [SerializeField] float worldRadius = 50;
    [SerializeField] float minWorldRadius = 3;
    [SerializeField] float viewCorrectionThreshold = 50;

    float lockTime;
    bool locked = false;
    bool autoLock = false;
    Vector3 velocity;
    float angleVelocity;


    struct PosRot
    {
        public Vector3 position;
        public Quaternion rotation;

        public PosRot(Vector3 position, Quaternion rotation) { 
            this.position = position; 
            this.rotation = rotation;
        }
    }

    PosRot target;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        LoadFromSettings(this.settings);
    }


    private void OnEnable()
    {
        settings.OnSettingChange += LoadFromSettings;
    }

    private void OnDisable()
    {
        settings.OnSettingChange -= LoadFromSettings;
    }


    public void LoadFromSettings(CameraSettings settings)
    {
        forwardSpeed = settings.MovementSpeed;
        strafeSpeed = settings.MovementSpeed;
        backwardSpeed = settings.MovementSpeed;
        autoLock = settings.AutoFocus;
        sensitivity = settings.Sensitiviy;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey && Time.time - lockTime > focusSpeed * 2)
        {
            locked = false;
        }
    }

    private void LateUpdate()
    {
        if (!locked)
        {
            HandlePosition();
            HandleRotation();
            HandleZoom();
            //HandleOffView();
        } else
        {
            focus();
        }

        HandleBoundry();
    }

    void HandleRotation()
    {
        float x = Input.GetAxis("Mouse X");
        float y = -Input.GetAxis("Mouse Y");

        float slowDown = Input.GetKey(KeyCode.LeftControl) ? 0.1f : 1;

        transform.Rotate(Vector3.right, y * Time.unscaledDeltaTime * sensitivity * 100 * slowDown);
        transform.Rotate(Vector3.up, x * Time.unscaledDeltaTime * sensitivity * 100 * slowDown);
    }

    void HandlePosition()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        bool shift = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && !Input.GetKey(KeyCode.Space);

        float slowDown = Input.GetKey(KeyCode.LeftControl) ? 0.1f : 1;

        transform.Translate((Vector3.forward * y * (y>0 ? forwardSpeed: backwardSpeed)
            + Vector3.right * x * strafeSpeed) * Time.unscaledDeltaTime * (shift?shiftMultiplier:1) * slowDown);

    }

    void focus()
    {
        
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, focusSpeed);
        float delta = Quaternion.Angle(transform.rotation, target.rotation);
        float nDelta = Mathf.SmoothDampAngle(delta, 0, ref angleVelocity, focusSpeed);
        if(delta > 0.01f) transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, (delta - nDelta) / delta);
    }


    void HandleZoom()
    {
        float s = Input.GetAxis("Mouse ScrollWheel");
        cam.fieldOfView -= s * Time.unscaledDeltaTime * zoomSpeed;
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, fovRange.x, fovRange.y);
    }


    void HandleBoundry()
    {
        transform.position = Vector3.ClampMagnitude(transform.position, worldRadius);
        if(transform.position.magnitude < minWorldRadius) transform.position = Vector3.Normalize(transform.position) * minWorldRadius;
    }


    void HandleOffView()
    {
        if(Quaternion.Angle(transform.rotation,Quaternion.LookRotation(-transform.position)) > viewCorrectionThreshold)
        {
            float t = 1 - viewCorrectionThreshold / Quaternion.Angle(transform.rotation, Quaternion.LookRotation(-transform.position));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-transform.position), t);
        }
    }



    public void SetTarget(Vector3 target1)
    {
        Vector3 dir1 = Random.onUnitSphere;
        Vector3 dir2 = Vector3.Cross(dir1, transform.up);
        Quaternion rot = Quaternion.LookRotation(-dir2);
        target = new PosRot(target1 + dir2 * focusDistance, rot);
    }

    public void SetTarget(Vector3 target1, Vector3 target2)
    {
        if (!autoLock) return;

        locked = true;
        lockTime = Time.time;
        Vector3 midpoint = (target1 + target2) / 2;
        Vector3 dir1 = target2 - midpoint;
        Vector3 dir2 = Vector3.Cross(dir1, transform.up);
        Quaternion rot = Quaternion.LookRotation(-dir2, Vector3.Cross(dir2,dir1));
        target = new PosRot(midpoint + dir2.normalized * focusDistance * dir1.magnitude, rot);  
    }
}
