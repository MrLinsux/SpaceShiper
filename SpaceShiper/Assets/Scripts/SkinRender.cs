using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinRender : MonoBehaviour
{
    public string Name = "white";
    Animator animator;

    void Start()
    {
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

        Trail trail;
        path = "Skins\\" + _name + "\\Trail";
        if (this.TryGetComponent(out trail) && (Resources.Load(path) != null))
        {
            trail.trailSegment = (GameObject)Resources.Load(path);
        }
        else if (Resources.Load(path) == null)
        {
            // если скин не найден, то ставим секретный
            path = "Skins\\darkAvatar\\Trail";
            trail.trailSegment = (GameObject)Resources.Load(path);
        }
        Name = _name;
    }
}
