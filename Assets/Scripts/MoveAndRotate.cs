using UnityEngine;
using UnityEngine.InputSystem;

using NewTouchPhase = UnityEngine.InputSystem.TouchPhase;

public class MoveAndRotate : MonoBehaviour
{
    [Header("移动")]
    public float moveSpeed = 0.5f;

    [Header("旋转")]
    public float rotationSpeed = 90f;

    [Header("AR相机")]
    public Camera referenceCamera;

    [Header("手机触控")]
    public float touchControlRadius = 150f;

    private int moveFingerId = -1;
    private int rotateFingerId = -1;

    private Vector2 moveTouchStart;
    private Vector2 rotateTouchStart;

    private Vector3 touchMoveInput;
    private float touchRotateInput;

    private Vector2 keyboardMoveInput;
    private float keyboardRotateInput;

    private void Start()
    {
        if (referenceCamera == null)
        {
            referenceCamera = Camera.main;
        }
    }

    private void Update()
    {
        ReadKeyboardInput();
        ReadTouchInput();

        Vector3 keyboardMove = new Vector3(
            keyboardMoveInput.x,
            0f,
            keyboardMoveInput.y
        );

        Vector3 finalMove =
            keyboardMove + touchMoveInput;

        finalMove = Vector3.ClampMagnitude(
            finalMove,
            1f
        );

        float finalRotate = Mathf.Clamp(
            keyboardRotateInput + touchRotateInput,
            -1f,
            1f
        );

        MoveObject(finalMove);
        RotateObject(finalRotate);
    }

    private void ReadKeyboardInput()
    {
        keyboardMoveInput = Vector2.zero;
        keyboardRotateInput = 0f;

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (keyboard.wKey.isPressed)
        {
            keyboardMoveInput.y += 1f;
        }

        if (keyboard.sKey.isPressed)
        {
            keyboardMoveInput.y -= 1f;
        }

        if (keyboard.aKey.isPressed)
        {
            keyboardMoveInput.x -= 1f;
        }

        if (keyboard.dKey.isPressed)
        {
            keyboardMoveInput.x += 1f;
        }

        keyboardMoveInput = Vector2.ClampMagnitude(
            keyboardMoveInput,
            1f
        );

        if (keyboard.qKey.isPressed)
        {
            keyboardRotateInput -= 1f;
        }

        if (keyboard.eKey.isPressed)
        {
            keyboardRotateInput += 1f;
        }
    }

    private void ReadTouchInput()
    {
        touchMoveInput = Vector3.zero;
        touchRotateInput = 0f;

        Touchscreen touchscreen = Touchscreen.current;

        if (touchscreen == null)
        {
            moveFingerId = -1;
            rotateFingerId = -1;
            return;
        }

        foreach (var touch in touchscreen.touches)
        {
            NewTouchPhase phase =
                touch.phase.ReadValue();

            if (phase == NewTouchPhase.None)
            {
                continue;
            }

            int fingerId = touch.touchId.ReadValue();
            Vector2 position =
                touch.position.ReadValue();

            if (phase == NewTouchPhase.Began)
            {
                // 左半屏控制移动
                if (position.x < Screen.width * 0.5f &&
                    moveFingerId == -1)
                {
                    moveFingerId = fingerId;
                    moveTouchStart = position;
                }
                // 右半屏控制旋转
                else if (rotateFingerId == -1)
                {
                    rotateFingerId = fingerId;
                    rotateTouchStart = position;
                }
            }

            if (fingerId == moveFingerId)
            {
                UpdateMoveTouch(position);
            }

            if (fingerId == rotateFingerId)
            {
                UpdateRotateTouch(position);
            }

            if (phase == NewTouchPhase.Ended ||
                phase == NewTouchPhase.Canceled)
            {
                ReleaseFinger(fingerId);
            }
        }
    }

    private void UpdateMoveTouch(Vector2 position)
    {
        Vector2 offset =
            position - moveTouchStart;

        float radius =
            Mathf.Max(touchControlRadius, 1f);

        Vector2 normalized =
            Vector2.ClampMagnitude(
                offset / radius,
                1f
            );

        touchMoveInput = new Vector3(
            normalized.x,
            0f,
            normalized.y
        );
    }

    private void UpdateRotateTouch(Vector2 position)
    {
        float offsetX =
            position.x - rotateTouchStart.x;

        float radius =
            Mathf.Max(touchControlRadius, 1f);

        touchRotateInput = Mathf.Clamp(
            offsetX / radius,
            -1f,
            1f
        );
    }

    private void ReleaseFinger(int fingerId)
    {
        if (fingerId == moveFingerId)
        {
            moveFingerId = -1;
            touchMoveInput = Vector3.zero;
        }

        if (fingerId == rotateFingerId)
        {
            rotateFingerId = -1;
            touchRotateInput = 0f;
        }
    }

    private void MoveObject(Vector3 input)
    {
        if (input.sqrMagnitude < 0.001f)
        {
            return;
        }

        Vector3 worldDirection;

        if (referenceCamera != null)
        {
            Vector3 cameraForward =
                referenceCamera.transform.forward;

            Vector3 cameraRight =
                referenceCamera.transform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            worldDirection =
                cameraForward * input.z +
                cameraRight * input.x;
        }
        else
        {
            worldDirection =
                transform.TransformDirection(input);
        }

        worldDirection =
            Vector3.ClampMagnitude(worldDirection, 1f);

        transform.position +=
            worldDirection *
            moveSpeed *
            Time.deltaTime;
    }

    private void RotateObject(float input)
    {
        if (Mathf.Abs(input) < 0.001f)
        {
            return;
        }

        float angle =
            input *
            rotationSpeed *
            Time.deltaTime;

        Quaternion rotationChange =
            Quaternion.Euler(0f, angle, 0f);

        transform.rotation =
            rotationChange * transform.rotation;
    }
}