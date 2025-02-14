using UnityEngine;
using System.Collections;

public class Navigator : MonoBehaviour
{
    // ========== EXISTING FIELDS ==========
    public float mouseMovingSpeed = 2f;
    public float mousePanningSpeed = 3f;
    public float mouseTurningSpeed = 5f;
    public float mousePitchSpeed = 10f;
    public float mouseScrollSpeed = 1;

    public float XBOXMovingSpeed = 1f;
    public float XBOXTurningSpeed = 30f;
    public float XBOXPitchSpeed = 15f;
    public float XBOXPanningSpeed = 1.5f;
    public float XBOXExtraSpeedFactor = 2;

    public bool XBOXInvertY = false;

    private float currentSpeed;
    private Vector3 currentPan;
    private float currentTurn;
    private float currentPitch;

    private Vector3 initPos;
    private Quaternion initRotation;
    private bool initFly;
    private float initEyeHeight;

    public bool terrainFollow = true;
    private RaycastHit raycastHitInfo;
    public Transform headTransform;
    public LayerMask terrainMask = -1; // Must include layers for walls/floor/doors
    private float rayDistance = 1000;
    public float rotResetSpeed = 5;
    public bool flying;
    public float gravity = 9.81f;
    private float droppingSpeed = 0;
    public float climbSpeed = 7;

    public float characterRadius = 0.15f; // Used for both vertical & horizontal spherecasts

    bool one_click_right = false;
    float timer_for_double_click_right;
    Vector3 mouseFirstClickPos_right;

    bool one_click_left = false;
    float timer_for_double_click_left;
    Vector3 mouseFirstClickPos_left;

    public float doubleClickDelay = 0.4f;
    public float clickDistanceTreshold = 3;

    private float guiTime = 3;
    private float guiTimer = 0;
    private string sMessage;

    public bool balancing = false;
    public bool moving;

    // ========== MONO BEHAVIOUR METHODS ==========
    void Start()
    {
        initPos = this.transform.position;
        initRotation = this.transform.rotation;
        initFly = flying;
        if (headTransform)
        {
            initEyeHeight = headTransform.localPosition.y;
        }
    }

    void OnGUI()
    {
        if (guiTimer > 0)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.blue;

            GUI.Label(new Rect(Screen.width / 3, Screen.height / 2, 200, 30), sMessage, style);
        }
    }

    private void Update()
    {
        if (currentPitch != 0 || currentSpeed != 0 || currentTurn != 0 ||
            currentPan.x != 0 || currentPan.y != 0 || balancing)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }

        guiTimer -= Time.deltaTime;
        guiTimer = Mathf.Clamp(guiTimer, 0, guiTime);
    }

    void LateUpdate()
    {
        // 1) Handle Double-Clicks & Balancing (existing logic)
        HandleDoubleClicksAndBalancing();

        // 2) Handle SHIFT + Mouse Scroll => Eye height changes
        HandleEyeHeightAdjust();

        // 3) Reset if needed
        if (Input.GetKey(KeyCode.R) || Input.GetKeyDown("joystick button 6"))
        {
            Reset();
        }

        // 4) Gather input for movement & rotation
        GatherMovementInput();

        // 5) Actually apply rotation around head or self
        ApplyRotation();

        // 6) Combine horizontal movement
        Vector3 forward = Vector3.Cross(transform.right, Vector3.up) * currentSpeed * Time.deltaTime;

        // Convert local pan.x to world direction
        Vector3 horizontalLocalPan = new Vector3(currentPan.x * Time.deltaTime, 0, 0);
        Vector3 worldPanX = transform.TransformDirection(horizontalLocalPan);

        // Vertical pan in world space
        Vector3 verticalPan = new Vector3(0, currentPan.y * Time.deltaTime, 0);

        // Sum them up
        Vector3 totalHorizontalDisplacement = worldPanX + verticalPan + forward;

        // If not flying, do a spherecast for walls/doors => clamp displacement
        if (!flying && totalHorizontalDisplacement.sqrMagnitude > 0.0001f)
        {
            totalHorizontalDisplacement = CheckHorizontalCollision(totalHorizontalDisplacement);
        }

        // 7) Translate horizontally
        transform.Translate(totalHorizontalDisplacement, Space.World);

        // 8) If not flying, handle "gravity" => spherecast downward 
        if (!flying && headTransform && terrainFollow)
        {
            HandleVerticalTerrainFollow();
        }
    }

    // ========== NEW / REFACTORED METHODS ==========

    private Vector3 CheckHorizontalCollision(Vector3 desiredDisplacement)
    {
        float distance = desiredDisplacement.magnitude;
        if (distance < 0.0001f) return desiredDisplacement;

        Vector3 startPos = transform.position;
        Vector3 direction = desiredDisplacement.normalized;

        if (Physics.SphereCast(startPos, characterRadius, direction,
                               out RaycastHit hit, distance, terrainMask))
        {
            float hitDist = hit.distance;
            if (hitDist < distance)
            {
                float bottomY = transform.position.y;
                float collisionY = hit.point.y;
                float stepHeight = collisionY - bottomY;
                float maxStepHeight = 0.2f;
                float floorThreshold = -0.1f;

                if (stepHeight >= floorThreshold && stepHeight <= maxStepHeight)
                {
                    float stepCheckOffset = 0.1f;
                    Vector3 stepCheckStart = new Vector3(
                        startPos.x,
                        collisionY + stepCheckOffset,
                        startPos.z
                    );

                    if (Physics.SphereCast(stepCheckStart, characterRadius, direction,
                                           out RaycastHit stepHit, distance, terrainMask))
                    {
                        float newHitDist = stepHit.distance;
                        if (newHitDist < distance)
                        {
                            return direction * (newHitDist * 0.95f);
                        }
                    }

                    float newY = collisionY + 0.001f;
                    Vector3 pos = transform.position;
                    pos.y = newY;
                    transform.position = pos;

                    return desiredDisplacement;
                }
                else
                {
                    return direction * (hitDist * 0.95f);
                }
            }
        }

        return desiredDisplacement;
    }

    private void HandleVerticalTerrainFollow()
    {
        Vector3 headPos = headTransform.position;
        Debug.DrawRay(headPos, -Vector3.up * rayDistance, Color.green);

        if (Physics.SphereCast(headPos, characterRadius, -Vector3.up,
                               out raycastHitInfo, rayDistance, terrainMask))
        {
            float dist = raycastHitInfo.distance;

            if (dist - characterRadius > headTransform.localPosition.y && dist < rayDistance)
            {
                droppingSpeed += gravity * Time.deltaTime;
                transform.Translate(0, -droppingSpeed * Time.deltaTime, 0, Space.World);
            }
            else if (dist - characterRadius < headTransform.localPosition.y && dist > 0)
            {
                droppingSpeed = 0;
                float offsetY = (headTransform.localPosition.y - dist + characterRadius);
                transform.position = new Vector3(
                    transform.position.x,
                    Mathf.Lerp(transform.position.y, transform.position.y + offsetY,
                               Time.deltaTime * climbSpeed),
                    transform.position.z
                );
            }
        }
        else
        {
            droppingSpeed += gravity * Time.deltaTime;
            transform.Translate(0, -droppingSpeed * Time.deltaTime, 0, Space.World);
        }
    }

    private void HandleEyeHeightAdjust()
    {
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
             Input.GetAxis("Mouse ScrollWheel") != 0.0f)
        {
            ShowMessage("Eye height: " + headTransform.localPosition.y.ToString("F2") + "m", 3);
            Vector3 headLocalPos = headTransform.localPosition;
            headLocalPos.y += (Input.GetAxis("Mouse ScrollWheel")) * mouseScrollSpeed * 0.1f;
            headTransform.localPosition = headLocalPos;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            HideMessage();
        }
    }

    private void HandleDoubleClicksAndBalancing()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!one_click_right)
            {
                one_click_right = true;
                mouseFirstClickPos_right = Input.mousePosition;
                timer_for_double_click_right = Time.time;
            }
            else
            {
                one_click_right = false;
                if (Vector3.Distance(mouseFirstClickPos_right, Input.mousePosition) < clickDistanceTreshold)
                {
                    balancing = true;
                }
            }
        }
        if (one_click_right)
        {
            if ((Time.time - timer_for_double_click_right) > doubleClickDelay)
            {
                one_click_right = false;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!one_click_left)
            {
                one_click_left = true;
                mouseFirstClickPos_left = Input.mousePosition;
                timer_for_double_click_left = Time.time;
            }
            else
            {
                one_click_left = false;
                if (Vector3.Distance(mouseFirstClickPos_left, Input.mousePosition) < clickDistanceTreshold)
                {
                    balancing = true;
                    flying = false;
                }
            }
        }
        if (one_click_left)
        {
            if ((Time.time - timer_for_double_click_left) > doubleClickDelay)
            {
                one_click_left = false;
            }
        }

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.H))
        {
            balancing = true;
            flying = false;
        }

        if (balancing)
        {
            currentPitch = 0;
            float angle = Vector3.Angle(transform.up, Vector3.up);
            if (Vector3.Dot(Vector3.up, transform.forward) > 0)
            {
                transform.RotateAround(headTransform.position, transform.right, angle * Time.deltaTime * rotResetSpeed);
            }
            else if (Vector3.Dot(Vector3.up, transform.forward) < 0)
            {
                transform.RotateAround(headTransform.position, transform.right, -angle * Time.deltaTime * rotResetSpeed);
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
                balancing = false;
            }

            if (transform.localEulerAngles.x > 359f || transform.localEulerAngles.x < 1f)
            {
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
                balancing = false;
            }
        }
    }

    private void GatherMovementInput()
    {
        if (!Input.GetKey(KeyCode.L) && (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2)))
        {
            if (Input.GetMouseButton(2))
            {
                if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                {
                    currentSpeed = 0;
                    currentPan = Vector3.zero;
                    currentPitch += Input.GetAxis("Mouse Y") * mousePitchSpeed * 0.1f;
                    currentTurn += Input.GetAxis("Mouse X") * mouseTurningSpeed * 0.1f;
                }
                else
                {
                    currentPan.y += Input.GetAxis("Mouse Y") * mousePanningSpeed * 0.01f;
                    currentPan.x += Input.GetAxis("Mouse X") * mousePanningSpeed * 0.01f;
                }
            }
            else if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
            {
                currentPan.y += Input.GetAxis("Mouse Y") * mousePanningSpeed * 0.01f;
                currentPan.x += Input.GetAxis("Mouse X") * mousePanningSpeed * 0.01f;
            }
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                currentPitch += Input.GetAxis("Mouse Y") * mousePitchSpeed * 0.1f;
                currentTurn += Input.GetAxis("Mouse X") * mouseTurningSpeed * 0.1f;
            }
            else
            {
                currentTurn += Input.GetAxis("Mouse X") * mouseTurningSpeed * 0.1f;
                currentSpeed += Input.GetAxis("Mouse Y") * mouseMovingSpeed * 0.01f;
            }
        }
        else
        {
            currentSpeed = 0;
            currentPitch = 0;
            currentPan = Vector3.zero;
            currentTurn = 0;

            float extraSpeed = 1 + Mathf.Clamp01(Input.GetAxis("XboxOne_LT") + Input.GetAxis("XboxOne_RT")) * XBOXExtraSpeedFactor;

            if (Input.GetButton("XboxOne_RB"))
            {
                if (XBOXInvertY)
                {
                    currentPitch = -Input.GetAxis("XboxOne_RS_v") * XBOXPitchSpeed * extraSpeed;
                }
                else
                {
                    currentPitch = Input.GetAxis("XboxOne_RS_v") * XBOXPitchSpeed * extraSpeed;
                }
            }
            if (Input.GetButtonUp("XboxOne_RB"))
            {
                currentPitch = 0;
            }

            if (Input.GetButton("XboxOne_LB") && !Input.GetKey("joystick button 0"))
            {
                currentPan.y = Input.GetAxis("XboxOne_LS_v") * XBOXPanningSpeed * extraSpeed;
                currentSpeed = 0;
            }
            else if (!Input.GetKey("joystick button 0"))
            {
                currentSpeed = Input.GetAxis("XboxOne_LS_v") * XBOXMovingSpeed * extraSpeed;
            }

            if (Input.GetButtonUp("XboxOne_LB"))
            {
                currentPan.y = 0;
            }

            if (Input.GetButtonDown("XboxOne_RS_B"))
            {
                balancing = true;
            }
            if (Input.GetButtonDown("XboxOne_LS_B"))
            {
                flying = false;
            }

            currentTurn = Input.GetAxis("XboxOne_RS_h") * XBOXTurningSpeed * extraSpeed;
            if (!Input.GetKey("joystick button 0"))
            {
                currentPan.x = Input.GetAxis("XboxOne_LS_h") * XBOXPanningSpeed * extraSpeed;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            currentTurn = 0;
            currentSpeed = 0;
            currentPitch = 0;
        }
        if (Input.GetMouseButtonUp(1))
        {
            currentTurn = 0;
            currentSpeed = 0;
            currentPitch = 0;
        }
        if (Input.GetMouseButtonUp(2))
        {
            currentPan = Vector3.zero;
            currentPitch = 0;
        }

        if (currentPan.y != 0)
        {
            flying = true;
        }
    }

    private void ApplyRotation()
    {
        if (headTransform)
        {
            transform.RotateAround(headTransform.position, Vector3.up, currentTurn * Time.deltaTime);
            transform.RotateAround(headTransform.position, transform.right, -currentPitch * Time.deltaTime);
        }
        else
        {
            transform.RotateAround(transform.position, Vector3.up, currentTurn * Time.deltaTime);
            transform.RotateAround(transform.position, transform.right, -currentPitch * Time.deltaTime);
        }
    }

    private void Reset()
    {
        flying = initFly;
        droppingSpeed = 0; // Reset vertical drop speed as well
    }

    public void ShowMessage(string message, float time)
    {
        sMessage = message;
        guiTimer = time;
        guiTime = time;
    }

    public void HideMessage()
    {
        guiTime = 0;
    }
}
