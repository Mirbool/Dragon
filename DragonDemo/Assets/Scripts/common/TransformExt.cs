using UnityEngine;

public static class TransformExt
{
    public static void RemoveAllChildren(this Transform parent)
    {
        int count = parent.childCount;
        for (int i = count -1; i >= 0; i--)
        {
            Transform transform = parent.GetChild(i);
            GameObject.Destroy(transform.gameObject);
        }
    }

    /// <summary>
    ///查找transform下的子级，不需要提供路径
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform FindChildByName(this Transform parent, string name)
    {
        Transform tmp = null;

        tmp = parent.Find(name);
        if (tmp != null)
        {
            return tmp;
        }
        else
        {
            foreach (Transform item in parent)
            {
                Transform t = FindChildByName(item, name);
                if (t != null) { return t; }
            }
        }

        return tmp;
    }
}

