using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;

public class SimplePrefabSpawner : EditorWindow
{
    [SerializeField] private VisualTreeAsset _tree;

    private ObjectField _prefabInput;
    private LayerMaskField _layerMask;
    private Vector3Field _minRotation;
    private Vector3Field _maxRotation;
    private FloatField _minScale;
    private FloatField _maxScale;
    private Toggle _active;
    private Toggle _alignToNormal;
    private Toggle _randomYRotation;
    private Toggle _randomScale;
    private Toggle _randomPosition;

    private GameObject _prefab;

    [MenuItem("Tools/SimplePrefabSpawner")]
    public static void ShowEditor()
    {
        var window = GetWindow<SimplePrefabSpawner>();
        window.titleContent = new GUIContent("Simple Prefab Spawner");
    }

    private void CreateGUI()
    {
        _tree.CloneTree(rootVisualElement);
        InitFields();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGui;
    }

    private void OnSceneGui(SceneView sceneView)
    {
        if (!_active.value) return;
        var evt = Event.current;
        if(evt.IsLeftMouseButtonDown())
        {
            var ray = HandleUtility.GUIPointToWorldRay(evt.mousePosition);
            Physics.Raycast(ray, out var raycastHit, Mathf.Infinity, _layerMask.value);
            if (raycastHit.collider == null) return;
            if (raycastHit.collider)
            {
                var obj = CreatePrefab(raycastHit.point);
                ApplyRandomRotation(obj, raycastHit.normal);
                ApplyRandomScale(obj);
                Undo.RegisterCreatedObjectUndo(obj,"Prefab spawned");
            }

        }
    }

    private GameObject CreatePrefab(Vector3 pos)
    {
        var obj = PrefabUtility.InstantiatePrefab(_prefab)as GameObject; //short for intanciate
        obj.transform.position = pos;
        return obj;
    }
    private void ApplyRandomRotation(GameObject obj, Vector3 normal)
    {
        var minRotation = _minRotation.value;
        var maxRotation = _maxRotation.value;
        var alignToNormal = _alignToNormal.value;

        if (alignToNormal)
        {
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
        }
        var rotationInEuler = obj.transform.rotation.eulerAngles;
        obj.transform.rotation = Quaternion.Euler(
            rotationInEuler.x + UnityEngine.Random.Range(minRotation.x, maxRotation.x),
            rotationInEuler.y + UnityEngine.Random.Range(minRotation.y, maxRotation.z),
            rotationInEuler.z + UnityEngine.Random.Range(minRotation.z, maxRotation.z));
    }

    private void ApplyRandomScale(GameObject obj)
    {
        var minScale = _minScale.value;
        var maxScale = _maxScale.value;
        obj.transform.localScale = Vector3.one * UnityEngine.Random.Range(minScale, maxScale);
    }
    
    private void InitFields()
    {
        _layerMask = rootVisualElement.Q<LayerMaskField>("Layer");
        _minRotation = rootVisualElement.Q<Vector3Field>("MinRotation");
        _maxRotation = rootVisualElement.Q<Vector3Field>("MaxRotation");
        _minScale = rootVisualElement.Q<FloatField>("MinScale");
        _maxScale = rootVisualElement.Q<FloatField>("MaxScale");
        _active = rootVisualElement.Q<Toggle>("Active");
        _alignToNormal = rootVisualElement.Q<Toggle>("AlignToNormal");
        _randomScale = rootVisualElement.Q<Toggle>("RndScale");
        //TODO: implement this two
        _randomYRotation = rootVisualElement.Q<Toggle>("RndYRotation");
        _randomPosition = rootVisualElement.Q<Toggle>("RndPosition");

        var _prefabInput = rootVisualElement.Q<ObjectField>("Prefab");
        _prefabInput.RegisterValueChangedCallback(evt => { _prefab = evt.newValue as GameObject; });
    }
}


