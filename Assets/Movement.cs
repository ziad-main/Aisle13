using UnityEngine;

public class Movement : MonoBehaviour
{
    
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    
    [HideInInspector] public Vector3 moveDirection;
    
    private float hInput,vInput;
    private CharacterController controller;
    private Vector3 velocity;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        GetDirectionAndMove();
    }
    
    void GetDirectionAndMove()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
        moveDirection = transform.forward * vInput + transform.right * hInput;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        
    }
}
