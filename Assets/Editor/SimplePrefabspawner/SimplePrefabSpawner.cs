using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System;

public class SimplePrefabSpawner : EditorWindow
{
    [SerializeField] private VisualTreeAsset _tree;
    [SerializeField] private SimPrefSpwn_Data _dataSheet;
    private PlacementLogic _placementLogic;

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
        
        var _dataSheetInput = rootVisualElement.Q<ObjectField>("DataField");
        _dataSheetInput.RegisterValueChangedCallback(evtTwo => 
        { 
            _dataSheet = evtTwo.newValue as SimPrefSpwn_Data;
            SetValueFromDataSheet();
        });
    }

    private void OnEnable()
    {
        _placementLogic = (PlacementLogic)Activator.CreateInstance(typeof(PlacementLogic));
        SceneView.duringSceneGui += OnSceneGui;
    }

    private void Start()
    {
        if (_layerMask.value > 0) return;
        else
        {
            SetValueFromDataSheet();
        }
    }

    private void SetValueFromDataSheet()
    {
        _layerMask.value = _dataSheet._layerMask.value;
        _minRotation.value = _dataSheet._minRotation;
        _maxRotation.value = _dataSheet._maxRotation;
        _minScale.value = _dataSheet._minScale;
        _maxScale.value = _dataSheet._maxScale;
        _active.value = _dataSheet._active;
        _alignToNormal.value = _dataSheet._alignToNormal;
        _prefab = _dataSheet._prefab;
    }

    private void OnSceneGui(SceneView sceneView)
    {
        Start();
        _placementLogic.Main(_layerMask, _prefab, _active, _minRotation, _maxRotation, _alignToNormal, _minScale, _maxScale);
    }
}



