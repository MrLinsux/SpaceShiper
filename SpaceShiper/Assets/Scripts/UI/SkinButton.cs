using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinButton : MonoBehaviour
{
    public enum SkinType { normal, rare, unique };
    public Color select = new Color(0.5568628f, 0.8980392f, 0.5411765f);
    public Color unlock = new Color(0.882353f, 0.8980393f, 0.8980393f);
    public Color rareUnlock = new Color(1f, 0.9411765f, 0);
    public Color locked = new Color(0.5921569f, 0.8588236f, 1f);
    public Color rareLocked = new Color(0.5568628f, 0.8980392f, 0.5411765f);

    public GameObject isSelectPoint;
    public Outline outline;
    public MotherController.Skin skin;
    public MotherController motherController;
    public GameObject trail;

    public string Name = "white";
    Animator animator;


    void Start()
    {
        isSelectPoint.GetComponent<SpriteRenderer>().color = select;
        animator = this.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if(isSelectPoint.activeInHierarchy)
            animator.SetBool("isSelect", true);
    }

    public void SetButton(int cost, bool isUnlock, SkinType type, string name)
    {
        skin = new MotherController.Skin(name, cost, isUnlock, type);
        this.Name = name;

        SetSkin(Name);
        isSelectPoint.SetActive(false);

        if (isUnlock)
        {
            this.GetComponent<Outline>().spriteColor = new Color(0, 0, 0, 1);
            switch (type)
            {
                case SkinType.normal:
                    this.GetComponent<Outline>().newOutlineColor = unlock;
                    break;
                case SkinType.rare:
                    this.GetComponent<Outline>().newOutlineColor = rareUnlock;
                    break;
            }
            this.GetComponent<Button>().onClick.AddListener(() => this.SetActive(true));
        }
        else
        {
            this.GetComponent<Outline>().spriteColor = new Color(0.1647059f, 0.1647059f, 0.1647059f, 0);
            switch (type)
            {
                case SkinType.normal:
                    this.GetComponent<Outline>().newOutlineColor = locked;
                    break;
                case SkinType.rare:
                    this.GetComponent<Outline>().newOutlineColor = rareLocked;
                    break;
            }
        }
    }
    public void SetActive(bool isSelect)
    {
        if (isSelect)
        {
            this.GetComponent<Outline>().newOutlineColor = select;
            if (motherController.currentSkin != this)
                motherController.currentSkin?.SetActive(false);
            motherController.currentSkin = this;
            motherController.player.GetComponent<SkinRender>().SetSkin(Name);
            motherController.playerProgress.currentSkin = Name;
            motherController.SavePlayerProgress();
        }
        else
            switch (skin.type)
            {
                case SkinType.normal:
                    this.GetComponent<Outline>().newOutlineColor = unlock;
                    break;
                case SkinType.rare:
                    this.GetComponent<Outline>().newOutlineColor = rareUnlock;
                    break;
            }

        animator.SetBool("isSelect", isSelect);
        isSelectPoint.SetActive(isSelect);
    }

    public void SetSkin(string _name)
    {
        string path = "Skins\\" + _name + "\\Skin";
        if (this.TryGetComponent(out animator) && (Resources.Load(path) != null))
        {
            animator.runtimeAnimatorController = (AnimatorOverrideController)Resources.Load(path);
        }
        else if (Resources.Load(path) == null)
        {
            // если скин не найден, то ставим секретный
            path = "Skins\\darkAvatar\\Skin";
            animator.runtimeAnimatorController = (AnimatorOverrideController)Resources.Load(path);
        }
    }
}
