using UnityEngine;

public class SmallGameSlot : MonoBehaviour
{
    public new SpriteRenderer renderer;
    [Tooltip("期望匹配的图")]
    public Sprite expected;
    [Tooltip("成功匹配期望图片后显示的图")]
    public Sprite success;
    public SmallGameSlot[] neighbors;

    private Sprite origin;

    public bool IsSuccess()
    {
        return renderer.sprite == success || renderer.sprite == expected;
    }

    //判断自己现在是否是空心的
    public bool IsEmptySlot()
    {
        return renderer.sprite == null;
    }

    public void ResetSlot()
    {
        renderer.sprite = origin;
    }

    public void SetSprite(Sprite sprite)
    {
        renderer.sprite = sprite == expected ? success : sprite;
    }

    public bool Hasneighbors()
    {
        return neighbors != null && neighbors.Length > 0;
    }

    //向空心邻居移动
    public bool TryMoveToEmpty()
    {
        if (IsEmptySlot()) { return false; }

        foreach (var v in neighbors)
        {
            if (v.IsEmptySlot())
            {
                v.SetSprite(renderer.sprite == success ? expected : renderer.sprite);
                renderer.sprite = null;
                return true;
            }
        }
        return false;
    }

    private void OnMouseDown()
    {
        //移动到空心邻居
        TryMoveToEmpty();
    }

    private void Start()
    {
        origin = renderer.sprite;
    }
}