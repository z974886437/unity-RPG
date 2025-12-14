using System;
using UnityEngine;

public class UI : MonoBehaviour
{
    public UI_SkillToolTip skillToolTip;
    public UI_SkillTree skillTree;
    private bool skillTreeEnabled;

    private void Awake()
    {
        skillToolTip = GetComponentInChildren<UI_SkillToolTip>();
        skillTree = GetComponentInChildren<UI_SkillTree>(true);
    }

    public void ToggleSkillTreeUI()
    {
        skillTreeEnabled = !skillTreeEnabled; // 切换技能树的启用状态（如果当前启用，则禁用，反之亦然）
        skillTree.gameObject.SetActive(skillTreeEnabled);// 根据 skillTreeEnabled 的值来设置技能树的显示状态
        skillToolTip.ShowToolTip(false, null);// 隐藏技能提示工具，如果需要的话
    }
}
