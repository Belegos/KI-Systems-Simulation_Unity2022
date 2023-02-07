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

    private void OnSceneGui(SceneView obj)
    {
        if (!_active.value) return;
        var evt = Event.current;
        if(evt.IsLeftMouseButtonDown())
        {
            var ray = HandleUtility.GUIPointToWorldRay(evt.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _layerMask.value))
            {
                var go = Instantiate(_prefab, hit.point, Quaternion.identity);
                if (_randomPosition.value)
                {
                    go.transform.position = hit.point + UnityEngine.Random.insideUnitSphere;
                }
                if (_randomYRotation.value)
                {
                    go.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
                }
                if (_randomScale.value)
                {
                    go.transform.localScale = Vector3.one * UnityEngine.Random.Range(_minScale.value, _maxScale.value);
                }
                if (_alignToNormal.value)
                {
                    go.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                }
            }
        }
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
        _randomYRotation = rootVisualElement.Q<Toggle>("RndYRotation");
        _randomScale = rootVisualElement.Q<Toggle>("RndScale");
        _randomPosition = rootVisualElement.Q<Toggle>("RndPosition");

        var _prefabInput = rootVisualElement.Q<ObjectField>("Prefab");
        _prefabInput.RegisterValueChangedCallback(evt => { _prefab = evt.newValue as GameObject; });
    }
}


