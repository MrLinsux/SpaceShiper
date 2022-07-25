using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinRender : MonoBehaviour
{
    public string Name = "white";
    Animator animator;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        SetSkin(Name);
    }

    public void SetSkin(string _name)
    {
        string path = "Skins\\" + _name + "\\Anim";
        if (this.TryGetComponent(out animator) && (Resources.Load(path) != null))
        {
            animator.runtimeAnimatorController = (AnimatorOverrideController)Resources.Load(path);
        }
        else if(Resources.Load(path) == null)
        {
            // если скин не найден, то ставим секретный
            path = "Skins\\darkAvatar\\Anim";
            animator.runtimeAnimatorController = (AnimatorOverrideController)Resources.Load(path);
        }
        Name = _name;
    }
}
