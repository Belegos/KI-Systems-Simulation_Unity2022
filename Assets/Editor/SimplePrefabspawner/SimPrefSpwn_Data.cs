using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
[CreateAssetMenu(fileName = "SimPrefSpwn_Data", menuName = "SimPrefSpwn_Data")]
public class SimPrefSpwn_Data : ScriptableObject
{
    public LayerMask _layerMask;
    public GameObject _prefab;
    
    public bool _active;
    public bool _alignToNormal;
    public bool _randomScale;

    public bool _randomXRotation;
    public bool _randomYRotation;
    public bool _randomZRotation;

    public Vector3 _minRotation;
    public Vector3 _maxRotation;
    
    public float _minScale;
    public float _maxScale;
    
}

