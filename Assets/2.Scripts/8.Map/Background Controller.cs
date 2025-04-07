using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] float _height = 5;
    [SerializeField] Transform _player;

    Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (null != _player)
            this.transform.position = new Vector3(this.transform.position.x, _player.position.z - _height, this.transform.position.z);
        else
            this.transform.position = new Vector3(this.transform.position.x, _camera.transform.position.z - _height + 3, this.transform.position.z);
    }
}
