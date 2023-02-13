using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class PlacementLogic
{
    public void Main(LayerMaskField _layerMask, GameObject _prefab, Toggle _active, Vector3Field _minRotation, Vector3Field _maxRotation, Toggle _alignToNormal, FloatField _minScale, FloatField _maxScale, Toggle _randomScale, Toggle _randomRotation)
    {
        var active = _active.value;
        var minRotation = _minRotation.value;
        var maxRotation = _maxRotation.value;
        var layerMask = _layerMask.value;
        var _offset = new Vector3(0.01f, 0.01f, 0.01f);
        
        if (!active) return;
        var evt = Event.current;
        if (evt.IsLeftMouseButtonDown())
        {
            var ray = HandleUtility.GUIPointToWorldRay(evt.mousePosition);
            Physics.Raycast(ray, out var raycastHit, Mathf.Infinity, layerMask);
            if (raycastHit.collider == null) return;
            if (raycastHit.collider)
            {
                var obj = CreatePrefab(raycastHit.point, _prefab, layerMask, _offset);
                if (obj == null) return;
                if (_randomScale.value)
                {
                    ApplyRandomScale(obj, _minScale, _maxScale);
                }
                if (_randomRotation.value)
                {
                    ApplyRandomRotation(obj, raycastHit.normal, minRotation, maxRotation, _alignToNormal);
                }
                if (_alignToNormal.value)
                {
                    obj.transform.rotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
                }
                Undo.RegisterCreatedObjectUndo(obj, "Prefab spawned");
            }
        }
    }

    private GameObject CreatePrefab(Vector3 pos, GameObject _prefab, LayerMask _layerMask, Vector3 _offset)
    {
        var obj = PrefabUtility.InstantiatePrefab(_prefab) as GameObject; //short for intanciate
        var collider = obj.GetComponent<Collider>();
        var bounds = obj.GetComponent<Collider>().bounds;
        var colliders = Physics.OverlapBox(bounds.center, bounds.extents, obj.transform.rotation, _layerMask);
        obj.transform.position = pos + new Vector3(0, bounds.size.y / 2 + _offset.y, 0);

        //if (colliders.Length > 0)
        //{
        //    GameObject.DestroyImmediate(obj);
        //    Debug.LogWarning("Warning: Object collided with other SceneObject and was not spawned. Please try again.");
        //    return null;
        //}
        return obj;
    }
    private void ApplyRandomRotation(GameObject obj, Vector3 normal, Vector3 _minRotation, Vector3 _maxRotation, Toggle _alignToNormal)
    {
        var alignToNormal = _alignToNormal.value;
        var rotationInEuler = obj.transform.rotation.eulerAngles;
        obj.transform.rotation = Quaternion.Euler(
            rotationInEuler.x + UnityEngine.Random.Range(_minRotation.x, _maxRotation.x),
            rotationInEuler.y + UnityEngine.Random.Range(_minRotation.y, _maxRotation.z),
            rotationInEuler.z + UnityEngine.Random.Range(_minRotation.z, _maxRotation.z));
    }

    private void ApplyRandomScale(GameObject obj, FloatField _minScale, FloatField _maxScale)
    {
        var minScale = _minScale.value;
        var maxScale = _maxScale.value;
        obj.transform.localScale = Vector3.one * UnityEngine.Random.Range(minScale, maxScale);
    }

}

