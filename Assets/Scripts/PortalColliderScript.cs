using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalColliderScript : MonoBehaviour
{
    [SerializeField] private GameObject _portal;
    [SerializeField] private Collider _other;
    private GameObject _player;
    private float _cooldown = 10.0f;
    private bool _hasPorted = false;

    private void OnEnable()
    {
        _player = GameObject.Find("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_cooldown >= 10.0f)
        {
            if (other.gameObject.tag == "Player")
            {
                _player.transform.position = _portal.transform.position;
                _hasPorted = true;
            }
        }
        if (_cooldown <= 0.0f)
        {
            _cooldown = 10.0f;
            _hasPorted = false;
        }
        if (_hasPorted is true)
        {
            _cooldown -= Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        OnTriggerEnter(_other);
    }

}
