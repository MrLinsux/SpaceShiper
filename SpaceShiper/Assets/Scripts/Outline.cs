using UnityEngine;

[ExecuteInEditMode]
public class Outline : MonoBehaviour
{
    public Color outlineColor = Color.white;
    public Color spriteColor = Color.clear;

    private SpriteRenderer spriteRenderer;

    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateOutline(true);
    }

    void OnDisable()
    {
        UpdateOutline(false);
    }

    void Update()
    {
        UpdateOutline(true);
    }

    void UpdateOutline(bool outline)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", outlineColor);
        mpb.SetColor("_SpriteColor", spriteColor);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}