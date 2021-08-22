using UnityEngine;
using System.Collections;

/// <summary>
/// 旋轉基底
/// 旋轉物件
/// 註冊場景旋轉時要處理的事情
/// </summary>
public class RotationBase : MonoBehaviour
{
    #region 欄位：公開
    [Header("旋轉時間"), Range(0, 10)]
    public float rotationTime = 0.2f;
    [Header("旋轉次數"), Range(0, 100)]
    public int rotationCount = 30;
    [Header("滑入顏色覆蓋")]
    public Color colorMouseEnter = new Color(207/255f, 87/255f, 93/255f);
    [Header("旋轉前上升與旋轉後下降的數值")]
    public float valueBeforeRotation;
    public float valueAfterRotation;
    [Header("是否能旋轉")]
    public bool canRotation = true;
    #endregion

    #region 欄位：私人
    private SceneObject sceneObject;
    #endregion

    #region 欄位：保護
    protected bool isRotating;
    protected Rigidbody2D rig;
    protected SpriteRenderer spr;
    protected int countClick;
    protected Player player;
    #endregion

    #region 方法：保護
    /// <summary>
    /// 當場景旋轉時
    /// </summary>
    protected virtual void WhenSceneRotation(float sceneRotationTime)
    {
        StartCoroutine(AfterSceneRotation(sceneRotationTime));
    }

    /// <summary>
    /// 滑鼠滑入的效果
    /// </summary>
    protected virtual void MouseEnterEffect()
    {
        spr.color = colorMouseEnter;
    }

    /// <summary>
    /// 滑鼠滑出的效果
    /// </summary>
    protected virtual void MouseExitEffect()
    {
        spr.color = Color.white;
    }

    /// <summary>
    /// 當場景旋轉時
    /// </summary>
    public virtual void RotationSelf()
    {
        if (!canRotation || Player.rotationObjectCount == 0) return;

        player.UpdateRotationObjectCount();

        StartCoroutine(Rotation());
    }
    #endregion

    #region 方法：私人
    private IEnumerator Rotation()
    {
        if (!isRotating)
        {
            isRotating = true;
            countClick++;

            RigidbodyType2D type = rig.bodyType;
            rig.bodyType = RigidbodyType2D.Static;
            transform.position += Vector3.up * valueBeforeRotation;
            Vector3 originalEuler = transform.eulerAngles;

            for (int i = 0; i < rotationCount; i++)
            {
                float interval = rotationTime / rotationCount;
                float angle = 180 / rotationCount;
                Vector3 euler = transform.eulerAngles;
                euler.z -= angle;                                       // 順時針 -
                transform.eulerAngles = euler;
                yield return new WaitForSeconds(interval);
            }

            transform.eulerAngles = originalEuler + Vector3.forward * 180;
            isRotating = false;
            rig.bodyType = type;

            transform.position -= Vector3.up * valueAfterRotation;
        }
    }

    /// <summary>
    /// 當場景旋轉之後要做的處理
    /// 關閉剛體並在場景旋轉之後啟動
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator AfterSceneRotation(float sceneRotationTime)
    {
        transform.position += Vector3.up * 2;
        RigidbodyType2D rigType = rig.bodyType;
        rig.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(sceneRotationTime + 0.5f);
        rig.bodyType = rigType;
    }
    #endregion

    #region 事件
    protected virtual void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        rig = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();

        sceneObject = GameObject.Find("場景地板根物件").GetComponent<SceneObject>();
        sceneObject.onSceneRotation += WhenSceneRotation;
    }

    private void OnMouseEnter()
    {
        MouseEnterEffect();
    }

    private void OnMouseExit()
    {
        MouseExitEffect();
    }

    private void OnMouseDown()
    {
        RotationSelf();
    }
    #endregion
}
