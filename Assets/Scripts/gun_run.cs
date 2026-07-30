using UnityEngine;

public class TurretRotate : MonoBehaviour
{
    [Header("旋转速度")]
    public float rotateSpeed = 30f;
    [Header("左右摆动最大角度")]
    public float maxAngle = 45f;

    private float currentAngle;
    private int direction = 1; // 1向右 -1向左

    void Update()
    {
        // 持续累加角度
        currentAngle += direction * rotateSpeed * Time.deltaTime;

        // 到达最大角度后反向
        if (Mathf.Abs(currentAngle) >= maxAngle)
        {
            direction *= -1;
        }

        // 绕Y轴水平旋转（左右转头）
        transform.localEulerAngles = new Vector3(0, currentAngle, 0);
    }
}
