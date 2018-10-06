using UnityEngine;
using System.Collections;

/// <summary>
/// 战斗中对话项
/// </summary>
public class DialogItem {
    /// <summary>
    /// 说话的人的头像
    /// </summary>
    public string portrait;
    /// <summary>
    /// 说的内容
    /// </summary>
    public string content;

    public DialogItem()
    {

    }

    public DialogItem(string _portrait, string _content)
    {
        portrait = _portrait;
        content = _content;
    }
}
