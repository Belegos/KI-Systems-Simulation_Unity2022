using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class PlacementLogic
{/// <summary>
 /// This method instantiates a new prefab GameObject at the position of a mouse click in the scene view of Unity editor. 
 /// The method also includes several options for randomizing the scale and rotation of the instantiated object, 
 /// as well as aligning it to the surface normal at the spawn location.
 ///
 /// If the newly spawned object intersects with other colliders in the scene, the method searches in all directions to find a valid position 
 /// with no intersections.If the search fails after a specified number of iterations, the method will return an error message.
 ///
 /// Lastly, the method registers the spawned object for undo functionality with the Ctrl+Z keyboard shortcut.
 /// </summary>
 /// <param name="_layerMask"></param>
 /// <param name="_prefabInput"></param>
 /// <param name="_active"></param>
 /// <param name="_alignToNormal"></param>
 /// <param name="_randomScale"></param>
 /// <param name="_randomXRotation"></param>
 /// <param name="_randomYRotation"></param>
 /// <param name="_randomZRotation"></param>
 /// <param name="_minScale"></param>
 /// <param name="_maxScale"></param>
 /// <param name="_minRotation"></param>
 /// <param name="_maxRotation"></param>
    public void Main(LayerMaskField _layerMask, ObjectField _prefabInput, Toggle _active, Toggle _alignToNormal,
        Toggle _randomScale, Toggle _randomXRotation, Toggle _randomYRotation, Toggle _randomZRotation,
        FloatField _minScale, FloatField _maxScale, Vector3Field _minRotation, Vector3Field _maxRotation)
    {
        LayerMask layerMask = _layerMask.value;
        GameObject prefab = _prefabInput.value as GameObject;
        bool active = _active.value;
        bool validPlace = false;
        Vector3 minRotation = _minRotation.value;
        Vector3 maxRotation = _maxRotation.value;
        float offset = 0.01f;
        int iterationCount = 0;
        Vector3 _offsetX = new Vector3(offset, 0, 0);//offset to prevent spawning inside colliders
        Vector3 _offsetY = new Vector3(0f, offset, 0f);
        Vector3 _offsetZ = new Vector3(0, 0, offset);
        Vector3 _offsetXMinus = new Vector3(-offset, 0, 0);
        Vector3 _offsetYMinus = new Vector3(0f, -offset, 0f);
        Vector3 _offsetZMinus = new Vector3(0, 0, -offset);
        var evt = Event.current;
        Collider[] hits;


        if (!active) return;
        if (evt.IsLeftMouseButtonDown())
        {
            var ray = HandleUtility.GUIPointToWorldRay(evt.mousePosition);
            Physics.Raycast(ray, out var raycastHit, Mathf.Infinity, layerMask);
            if (raycastHit.collider == null) return;
            if (raycastHit.collider)
            {
                iterationCount = 0;//reset count for error prevention
                var newObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                if (_randomScale.value)
                {
                    ApplyRandomScale(newObject, _minScale, _maxScale);
                }
                if (_alignToNormal.value)
                {
                    var normal = raycastHit.normal;
                    var rotation = Quaternion.FromToRotation(Vector3.up, normal);
                    newObject.transform.rotation = rotation;
                }
                if (_randomXRotation.value || _randomYRotation.value || _randomZRotation.value)
                {
                    Vector3 rotation = new Vector3(
                        _randomXRotation.value ? Random.Range(minRotation.x, maxRotation.x) : newObject.transform.eulerAngles.x,
                        _randomYRotation.value ? Random.Range(minRotation.y, maxRotation.y) : newObject.transform.eulerAngles.y,
                        _randomZRotation.value ? Random.Range(minRotation.z, maxRotation.z) : newObject.transform.eulerAngles.z
                    );
                    newObject.transform.eulerAngles = rotation;
                }
                newObject.transform.position = raycastHit.point;

                // Check for collisions with other objects in the layer
                Bounds bounds = newObject.GetComponent<Renderer>().bounds;

                hits = Physics.OverlapBox(bounds.center, bounds.extents, newObject.transform.rotation, layerMask);

                // Move the object in every direction until all colliders are clear
                if (hits.Length > 0)
                {
                    #region iterationalObjects
                    //These objects are localy used to find the shortest side, on wich the object will be moved
                    GameObject objectXPlus = newObject;
                    Bounds boundsXPlus = bounds;

                    GameObject objectYPlus = newObject;
                    Bounds boundsYPlus = bounds;

                    GameObject objectZPlus = newObject;
                    Bounds boundsZPlus = bounds;

                    GameObject objectXMinus = newObject;
                    Bounds boundsXMinus = bounds;

                    GameObject objectYMinus = newObject;
                    Bounds boundsYMinus = bounds;

                    GameObject objectZMinus = newObject;
                    Bounds boundsZMinus = bounds;
                    #endregion
                    while (validPlace is false)
                    {
                        if (validPlace is false)//check in X+ direction
                        {
                            hits = SearchForNoColliderHits(layerMask, _offsetX, ref iterationCount, objectXPlus, boundsXPlus);
                            if (hits.Length <= 0)
                            {
                                newObject.transform.position = objectXPlus.transform.position;
                                validPlace = true;
                                break;
                            }
                        }
                        if (validPlace is false)//check in Y+ direction
                        {
                            hits = SearchForNoColliderHits(layerMask, _offsetY, ref iterationCount, objectYPlus, boundsYPlus);
                            if (hits.Length <= 0)
                            {
                                newObject.transform.position = objectYPlus.transform.position;
                                validPlace = true;
                                break;
                            }
                        }
                        if (validPlace is false)//check in Z+ direction
                        {
                            hits = SearchForNoColliderHits(layerMask, _offsetZ, ref iterationCount, objectZPlus, boundsZPlus);
                            if (hits.Length <= 0)
                            {
                                newObject.transform.position = objectZPlus.transform.position;
                                validPlace = true;
                                break;
                            }
                        }
                        if (validPlace is false)//check in X- direction
                        {
                            hits = SearchForNoColliderHits(layerMask, _offsetXMinus, ref iterationCount, objectXMinus, boundsXMinus);
                            if (hits.Length <= 0)
                            {
                                newObject.transform.position = objectXMinus.transform.position;
                                validPlace = true;
                                break;
                            }
                        }
                        if (validPlace is false)//check in Y- direction
                        {
                            hits = SearchForNoColliderHits(layerMask, _offsetYMinus, ref iterationCount, objectYMinus, boundsYMinus);
                            if (hits.Length <= 0)
                            {
                                newObject.transform.position = objectYMinus.transform.position;
                                validPlace = true;
                                break;
                            }
                        }
                        if (validPlace is false)//check in Z- direction
                        {
                            hits = SearchForNoColliderHits(layerMask, _offsetZMinus, ref iterationCount, objectZMinus, boundsZMinus);
                            if (hits.Length <= 0)
                            {
                                newObject.transform.position = objectZMinus.transform.position;
                                validPlace = true;
                                break;
                            }
                        }
                        if (hits.Length <= 0) { validPlace = true; break; }
                        if (iterationCount >= 10000 && hits.Length > 0)
                        {
                            Debug.LogError("Can't find a valid position, please try somewhere else. " + iterationCount + " times iterated until fail.");
                            validPlace = true;
                            break;
                        }
                        iterationCount++;
                    }
                }
                Undo.RegisterCreatedObjectUndo(newObject, "Prefab spawned");//register for undo with ctrl+z
            }
        }
    }

    private static Collider[] SearchForNoColliderHits(int layerMask, Vector3 _offset, ref int _iterationCound, GameObject newObject, Bounds bounds)
    {
        Collider[] hits;
        Vector3 localOffset = _offset;
        for (int i = 0; i < _iterationCound; i++)
        {
            localOffset = localOffset + _offset;
        }
        newObject.transform.position = newObject.transform.position + localOffset;
        bounds.center = bounds.center + localOffset;
        hits = Physics.OverlapBox(bounds.center, bounds.extents, newObject.transform.rotation, layerMask);
        return hits;
    }

    private void ApplyRandomScale(GameObject obj, FloatField _minScale, FloatField _maxScale)
    {
        var minScale = _minScale.value;
        var maxScale = _maxScale.value;
        obj.transform.localScale = Vector3.one * UnityEngine.Random.Range(minScale, maxScale);
    }
}

