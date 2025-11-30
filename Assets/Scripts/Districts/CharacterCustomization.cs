using System;
using UnityEngine;

/// <summary>
/// System for character customization options.
/// </summary>
public class CharacterCustomization : MonoBehaviour
{
    public static CharacterCustomization Instance { get; private set; }

    [Header("Character Data")]
    [SerializeField] private string characterName = "Player";
    [SerializeField] private int skinToneIndex = 0;
    [SerializeField] private int hairStyleIndex = 0;
    [SerializeField] private int hairColorIndex = 0;
    [SerializeField] private int outfitIndex = 0;
    [SerializeField] private int accessoryIndex = 0;

    [Header("Available Options")]
    [SerializeField] private Color[] skinTones;
    [SerializeField] private string[] hairStyles;
    [SerializeField] private Color[] hairColors;
    [SerializeField] private string[] outfits;
    [SerializeField] private string[] accessories;

    [Header("Visual References")]
    [SerializeField] private Renderer characterRenderer;
    [SerializeField] private Transform hairTransform;

    public event Action OnCustomizationChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Gets the character name.
    /// </summary>
    public string GetCharacterName()
    {
        return characterName;
    }

    /// <summary>
    /// Sets the character name.
    /// </summary>
    public void SetCharacterName(string name)
    {
        characterName = name;
        OnCustomizationChanged?.Invoke();
    }

    /// <summary>
    /// Sets the skin tone.
    /// </summary>
    public void SetSkinTone(int index)
    {
        if (skinTones == null || index < 0 || index >= skinTones.Length) return;
        skinToneIndex = index;
        ApplySkinTone();
        OnCustomizationChanged?.Invoke();
    }

    /// <summary>
    /// Sets the hair style.
    /// </summary>
    public void SetHairStyle(int index)
    {
        if (hairStyles == null || index < 0 || index >= hairStyles.Length) return;
        hairStyleIndex = index;
        ApplyHairStyle();
        OnCustomizationChanged?.Invoke();
    }

    /// <summary>
    /// Sets the hair color.
    /// </summary>
    public void SetHairColor(int index)
    {
        if (hairColors == null || index < 0 || index >= hairColors.Length) return;
        hairColorIndex = index;
        ApplyHairColor();
        OnCustomizationChanged?.Invoke();
    }

    /// <summary>
    /// Sets the outfit.
    /// </summary>
    public void SetOutfit(int index)
    {
        if (outfits == null || index < 0 || index >= outfits.Length) return;
        outfitIndex = index;
        ApplyOutfit();
        OnCustomizationChanged?.Invoke();
    }

    /// <summary>
    /// Sets the accessory.
    /// </summary>
    public void SetAccessory(int index)
    {
        if (accessories == null || index < 0 || index >= accessories.Length) return;
        accessoryIndex = index;
        ApplyAccessory();
        OnCustomizationChanged?.Invoke();
    }

    private void ApplySkinTone()
    {
        if (characterRenderer != null && skinTones != null && skinToneIndex < skinTones.Length)
        {
            var material = characterRenderer.material;
            material.color = skinTones[skinToneIndex];
        }
    }

    private void ApplyHairStyle()
    {
        // Implementation depends on how hair is set up in the game
        // Could swap meshes, enable/disable objects, etc.
    }

    private void ApplyHairColor()
    {
        // Implementation depends on hair setup
    }

    private void ApplyOutfit()
    {
        // Implementation depends on outfit system
    }

    private void ApplyAccessory()
    {
        // Implementation depends on accessory system
    }

    /// <summary>
    /// Gets the current customization data.
    /// </summary>
    public CharacterCustomizationData GetCustomizationData()
    {
        return new CharacterCustomizationData
        {
            characterName = characterName,
            skinToneIndex = skinToneIndex,
            hairStyleIndex = hairStyleIndex,
            hairColorIndex = hairColorIndex,
            outfitIndex = outfitIndex,
            accessoryIndex = accessoryIndex
        };
    }

    /// <summary>
    /// Loads customization data.
    /// </summary>
    public void LoadCustomizationData(CharacterCustomizationData data)
    {
        characterName = data.characterName;
        skinToneIndex = data.skinToneIndex;
        hairStyleIndex = data.hairStyleIndex;
        hairColorIndex = data.hairColorIndex;
        outfitIndex = data.outfitIndex;
        accessoryIndex = data.accessoryIndex;

        ApplySkinTone();
        ApplyHairStyle();
        ApplyHairColor();
        ApplyOutfit();
        ApplyAccessory();

        OnCustomizationChanged?.Invoke();
    }

    /// <summary>
    /// Gets available skin tone count.
    /// </summary>
    public int GetSkinToneCount()
    {
        return skinTones != null ? skinTones.Length : 0;
    }

    /// <summary>
    /// Gets available hair style count.
    /// </summary>
    public int GetHairStyleCount()
    {
        return hairStyles != null ? hairStyles.Length : 0;
    }

    /// <summary>
    /// Gets available hair color count.
    /// </summary>
    public int GetHairColorCount()
    {
        return hairColors != null ? hairColors.Length : 0;
    }

    /// <summary>
    /// Gets available outfit count.
    /// </summary>
    public int GetOutfitCount()
    {
        return outfits != null ? outfits.Length : 0;
    }

    /// <summary>
    /// Gets available accessory count.
    /// </summary>
    public int GetAccessoryCount()
    {
        return accessories != null ? accessories.Length : 0;
    }
}

/// <summary>
/// Data structure for character customization.
/// </summary>
[Serializable]
public struct CharacterCustomizationData
{
    public string characterName;
    public int skinToneIndex;
    public int hairStyleIndex;
    public int hairColorIndex;
    public int outfitIndex;
    public int accessoryIndex;
}
