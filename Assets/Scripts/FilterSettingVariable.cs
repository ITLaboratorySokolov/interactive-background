using UnityEngine;

/// <summary>
/// Variable with values for filter settings
/// </summary>
[CreateAssetMenu(fileName = "Variable", menuName = "TechnologyLab.Common/Variables/FilterSetting")]
public class FilterSettingVariable : ScriptableObject
{
    /// <summary> Minimum depth </summary>
    [SerializeField]
    private float minDepth;
    
    /// <summary> Maximum depth </summary>
    [SerializeField]
    private float maxDepth;
    
    /// <summary> Horizontal pan </summary>
    [SerializeField]
    private int panHor;
    
    /// <summary> Vertical pan </summary>
    [SerializeField]
    private int panVert;

    /// <summary> Zoom </summary>
    [SerializeField]
    private float zoom;
    
    public float MaxDepth { get => maxDepth; set => maxDepth = value; }
    public float MinDepth { get => minDepth; set => minDepth = value; }
    public int PanHor { get => panHor; set => panHor = value; }
    public int PanVert { get => panVert; set => panVert = value; }
    public float Zoom { get => zoom; set => zoom = value; }
}
