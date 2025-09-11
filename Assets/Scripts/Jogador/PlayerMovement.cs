using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementCC : MonoBehaviour
{
    public static PlayerMovementCC instance;

    private CharacterController controller;

    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float crouchSpeed = 2.5f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Transform cameraHolder;
    public float cameraNormalY = 0.75f;
    public float cameraCrouchY = 0.5f;
    public float smoothSpeed = 6f;

    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool isCrouching;
    [HideInInspector] public Vector3 moveDir;

    private Vector3 velocity;
    private bool isGrounded;

    public bool podeMover = true;

    private Animator anim;

    void Start()
    {
        instance = this;
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!podeMover) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        moveDir = (transform.right * x + transform.forward * z).normalized;

        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isCrouching) Levantar();
            else Agachar();
        }

        isRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching && z > 0;

        if (isCrouching && Input.GetKey(KeyCode.LeftShift) && z > 0)
        {
            Levantar();
            isRunning = true;
        }

        float currentSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;
        else if (isRunning) currentSpeed = runSpeed;
        else currentSpeed = walkSpeed;

        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        velocity.y += -9.81f * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- Controle do Animator ---
         if (anim != null)
        {
            if (moveDir.magnitude > 0.1f) // está se movendo
            {
                if (isCrouching) // andando agachado
                    anim.SetInteger("EstadoMovimento", 3);
                else if (isRunning) // correndo
                    anim.SetInteger("EstadoMovimento", 2);
                else // andando normal
                    anim.SetInteger("EstadoMovimento", 1);
            }
            else
            {
                anim.SetInteger("EstadoMovimento", 0); // parado
            }
        }


        float targetCamY = isCrouching ? cameraCrouchY : cameraNormalY;
        Vector3 camPos = cameraHolder.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetCamY, smoothSpeed * Time.deltaTime);
        cameraHolder.localPosition = camPos;
    }

    void Agachar()
    {
        isCrouching = true;
    }

    void Levantar()
    {
        isCrouching = false;
    }
}
