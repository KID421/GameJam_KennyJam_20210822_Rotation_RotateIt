using UnityEngine;
using System.Collections;

/// <summary>
/// 場景物件：整體場景要旋轉的根物件
/// </summary>
public class SceneObject : RotationBase
{
    /// <summary>
    /// 委派：無傳回與無參數
    /// </summary>
    public delegate void delegateSceneRotation(float sceneRotationTime);

    /// <summary>
    /// 事件：當場景旋轉
    /// </summary>
    public event delegateSceneRotation onSceneRotation;

    public override void RotationSelf()
    {
        if (Player.rotationObjectCount == 0) return;

        base.RotationSelf();

        onSceneRotation(rotationTime);
    }

    protected override IEnumerator AfterSceneRotation(float sceneRotationTime)
    {
        yield return null;
    }
}
