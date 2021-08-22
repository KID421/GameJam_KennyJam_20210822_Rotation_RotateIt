using UnityEngine;

/// <summary>
/// 道具：場景上的靜態道具物件
/// </summary>
public class Prop : RotationBase
{
    [Header("是否能推動")]
    public bool canPush;

    public override void RotationSelf()
    {
        canPush = !canPush;
        rig.bodyType = canPush ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;

        base.RotationSelf();
    }

    protected override void Start()
    {
        base.Start();

        rig.bodyType = canPush ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
    }
}
