using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestController : MonoBehaviour
{

    [SerializeField] float Speed;
    [SerializeField] CharacterController _charController;

    private void Awake()
    {
       // _charController = GetComponent<CharacterController>();
    }

    void Update()
    {
        float mz = Input.GetAxis("Vertical");
        float mx = Input.GetAxis("Horizontal");
        //GetAxis�� x,z �� �ο��ϱ�
        Vector3 dv = new Vector3(mx, 0, mz);
        dv = (dv.magnitude > 1) ? dv.normalized : dv; // normalizedȭ

        if (dv != Vector3.zero)
            transform.Translate(dv * Speed * Time.deltaTime);
        //_charController.Move(dv * Speed * Time.deltaTime);
    }
}
