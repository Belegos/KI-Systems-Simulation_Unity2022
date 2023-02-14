using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class PlacementLogic
{
    public void Main(LayerMaskField _layerMask, ObjectField _prefabInput, Toggle _active, Toggle _alignToNormal,
        Toggle _randomScale, Toggle _randomXRotation, Toggle _randomYRotation, Toggle _randomZRotation,
        FloatField _minScale, FloatField _maxScale, Vector3Field _minRotation, Vector3Field _maxRotation)
    {
        var layerMask = _layerMask.value;
        var _prefab = _prefabInput.value as GameObject;
        var active = _active.value;
        var minRotation = _minRotation.value;
        var maxRotation = _maxRotation.value;
        var _offset = new Vector3(0.01f, 0.01f, 0.01f); //offset to prevent spawning inside colliders

        if (!active) return;
        var evt = Event.current;
        if (evt.IsLeftMouseButtonDown())
        {
            var ray = HandleUtility.GUIPointToWorldRay(evt.mousePosition);
            Physics.Raycast(ray, out var raycastHit, Mathf.Infinity, layerMask);
            if (raycastHit.collider == null) return;
            if (raycastHit.collider)
            {
                var obj = CreatePrefab(raycastHit.point, _prefab, layerMask, _offset, _randomScale, _minScale, _maxScale);
                if (obj == null) return;
                if (_randomXRotation.value) //TODO: Fix this
                {
                    //Random X-Rotation
                    ApplyRandomXRotation();
                }
                if (_randomYRotation.value) //TODO: Fix this
                {
                    //Random Y-Rotation
                    ApplyRandomYRotation();
                }
                if (_randomZRotation.value) //TODO: Fix this
                {
                    //Random Z-Rotation
                    ApplyRandomZRotation();
                }
                if (_alignToNormal.value) //TODO: Fix this
                {
                    obj.transform.rotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
                }
                Undo.RegisterCreatedObjectUndo(obj, "Prefab spawned");
            }
        }
    }

    private GameObject CreatePrefab(Vector3 pos, GameObject _prefab, LayerMask _layerMask, Vector3 _offset, Toggle _randomScale, FloatField _minScale, FloatField _maxScale)
    {
        var obj = PrefabUtility.InstantiatePrefab(_prefab) as GameObject; //short for intanciate
        if (_randomScale.value)//Handle Scale-Settings
        {
            ApplyRandomScale(obj, _minScale, _maxScale);
        }
        var collider = obj.GetComponent<Collider>();
        var bounds = obj.GetComponent<Collider>().bounds;
        var colliders = Physics.OverlapBox(bounds.center, bounds.extents, obj.transform.rotation, _layerMask);
        obj.transform.position = pos + new Vector3(0, bounds.size.y / 2 + _offset.y, 0);

        //TODO: Fix this
        //if (colliders.Length > 0)
        //{
        //    GameObject.DestroyImmediate(obj);
        //    Debug.LogWarning("Warning: Object collided with other SceneObject and was not spawned. Please try again.");
        //    return null;
        //} 

        return obj;
    }
    private void ApplyRandomScale(GameObject obj, FloatField _minScale, FloatField _maxScale)
    {
        var minScale = _minScale.value;
        var maxScale = _maxScale.value;
        obj.transform.localScale = Vector3.one * UnityEngine.Random.Range(minScale, maxScale);
    }

    private void ApplyRandomXRotation()
    {

    }
    private void ApplyRandomYRotation()
    {

    }
    private void ApplyRandomZRotation()
    {

    }


}

