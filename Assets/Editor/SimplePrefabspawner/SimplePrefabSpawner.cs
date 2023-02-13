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
    private GameObject _prefab;

    private Toggle _active;
    private Toggle _alignToNormal;
    
    private DropdownField _rndSettingsDropdown;
    private Toggle _randomXRotation;
    private Toggle _randomYRotation;
    private Toggle _randomZRotation;

    private DropdownField _rndRotationDropdown;
    private Vector3Field _minRotation;
    private Vector3Field _maxRotation;

    private DropdownField _scaleSettingsforRandom;
    private FloatField _minScale;
    private FloatField _maxScale;

    private Toggle _randomScale;
    private Toggle _randomRotation;

    private DropdownField _loadLayoutDropdown;


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
        SetValueFromDataSheet();
    }
    private void InitFields()
    {
        _rndSettingsDropdown = rootVisualElement.Q<DropdownField>("_rndSettingsDropdown");
        _rndRotationDropdown = rootVisualElement.Q<DropdownField>("_rndRotationDropdown");
        _scaleSettingsforRandom = rootVisualElement.Q<DropdownField>("_scaleSettingsforRandom");
        _loadLayoutDropdown = rootVisualElement.Q<DropdownField>("_loadLayoutDropdown");
        _randomXRotation = rootVisualElement.Q<Toggle>("_randomXRotation");
        _randomYRotation = rootVisualElement.Q<Toggle>("_randomYRotation");
        _randomZRotation = rootVisualElement.Q<Toggle>("_randomZRotation");

        _layerMask = rootVisualElement.Q<LayerMaskField>("Layer");
        _minRotation = rootVisualElement.Q<Vector3Field>("MinRotation");
        _maxRotation = rootVisualElement.Q<Vector3Field>("MaxRotation");
        _minScale = rootVisualElement.Q<FloatField>("MinScale");
        _maxScale = rootVisualElement.Q<FloatField>("MaxScale");
        _active = rootVisualElement.Q<Toggle>("Active");
        _alignToNormal = rootVisualElement.Q<Toggle>("AlignToNormal");
        _randomScale = rootVisualElement.Q<Toggle>("RndScale");
        _randomRotation = rootVisualElement.Q<Toggle>("RndRotation");
        _randomYRotation = rootVisualElement.Q<Toggle>("RndYRotation");

        _prefabInput = rootVisualElement.Q<ObjectField>("Prefab");
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
        _randomScale.value = _dataSheet._randomScale;
        _prefabInput.value = _dataSheet._prefab;
        //_randomRotation.value = false;
    }

    private void OnSceneGui(SceneView sceneView)
    {
        //Start();
        _placementLogic.Main(_layerMask, _prefab, _active, _minRotation, _maxRotation, _alignToNormal, _minScale, _maxScale,_randomScale, _randomRotation);
    }
}



