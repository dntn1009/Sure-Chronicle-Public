using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTestController : MonoBehaviour
{
    [SerializeField] Transform _object;
    // Update is called once per frame
    [SerializeField] Vector3 _moveVec;
    [SerializeField] float _Speed = 5f;
    void Update()
    {
        if(null != _object)
        transform.position = _object.position + _moveVec;
        else
        {
            float mz = Input.GetAxis("Vertical");
            float mx = Input.GetAxis("Horizontal");
            //GetAxis�� x,z �� �ο��ϱ�
            Vector3 dv = new Vector3(mx, 0, mz);
            dv = (dv.magnitude > 1) ? dv.normalized : dv; // normalizedȭ

            if (dv != Vector3.zero)
                transform.Translate(dv * _Speed * Time.deltaTime);
        }
    }

    public void FirstSlotFollow(GameObject obj)
    {
        _object = obj.transform;
    }
}
