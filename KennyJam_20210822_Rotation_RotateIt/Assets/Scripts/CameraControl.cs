using UnityEngine;

/// <summary>
/// ��v������G�l�ܪ��a
/// </summary>
public class CameraControl : MonoBehaviour
{
    [Header("�򥻰Ѽ�")]
    public float speed = 3.5f;
    public Vector2 limitY = new Vector2(0, 1);
    public Transform target;

    private void LateUpdate()
    {
        Track();
    }

    /// <summary>
    /// �l�ܪ���
    /// </summary>
    private void Track()
    {
        Vector3 posCamera = transform.position;
        Vector3 posTarget = target.position;

        posCamera = Vector3.Lerp(posCamera, posTarget, Time.deltaTime * speed);
        posCamera.z = -10;
        posCamera.y = Mathf.Clamp(posCamera.y, limitY.x, limitY.y);

        transform.position = posCamera;
    }
}
