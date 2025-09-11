using System.Collections;
using UnityEngine;
using UnityEngine.Events; // <- IMPORTANTE

public class InteractionSeat : MonoBehaviour, IInterativo
{
    [Header("Configuração de Interação")]
    public Transform cameraPosicao;        
    public string animTriggerEntrar = "Entrar";
    public string animTriggerSair = "Sair";
    public float tempoSuavizarCameraAtePosicaoDeEntrar = 0.5f;
    public float tempoSuavizarCameraAtePosicaoDeSair = 0.5f;
    public float cooldownExtraEntrada = 0.1f;
    public float cooldownExtraSaida = 0.1f;
    public float duracaoAnimacaoEntrada = 1.0f;
    public float duracaoAnimacaoSaida = 1.0f;
    public bool bloquearMovimento = true;
    public bool bloquearMouse = true;

    [Header("Eventos de Interação")]
    public UnityEvent onEntrarInteracao;
    public UnityEvent onSairInteracao;

    private Transform cameraResetTransform;
    private Transform cameraJogador;
    private Transform PaiCamQuandoEstaEmTerceiros;
    private bool jogadorDentro = false;
    private bool emTransicao = false; 
    private Animator anim;
    private Coroutine interacaoCoroutine;

    [Header("Controle de Câmera dentro da Interação")]
    public bool usarCameraLimitada = true;
    public float limiteX = 30f;
    public float limiteY = 15f;

    private float xRot = 0f;
    private float yRot = 0f;
    private float sensibilidadeMouse = 100f;
    private Quaternion rotacaoBase;

    void Start()
    {
        anim = GetComponent<Animator>();
        cameraResetTransform = GameMaster.Instance.PaiCamMovAnim;
        cameraJogador = GameMaster.Instance.CameraJogador;
        PaiCamQuandoEstaEmTerceiros = GameMaster.Instance.MovQuandoEstaAnimacaoTerceiros;
    }

    void Update()
    {
        if (jogadorDentro && !emTransicao && Input.GetKeyDown(KeyCode.E))
        {
            if (interacaoCoroutine != null)
                StopCoroutine(interacaoCoroutine);

            emTransicao = true;
            interacaoCoroutine = StartCoroutine(SairInteracao());
        }
        
        if (jogadorDentro && usarCameraLimitada && !emTransicao)
        {
            ControlarCameraLimitada();
        }
    }

    private void ControlarCameraLimitada()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadeMouse * Time.deltaTime;

        yRot += mouseX;
        xRot -= mouseY;

        yRot = Mathf.Clamp(yRot, -limiteX, limiteX);
        xRot = Mathf.Clamp(xRot, -limiteY, limiteY);

        Quaternion rot = Quaternion.Euler(xRot, yRot, 0);
        cameraJogador.rotation = rotacaoBase * rot;
    }

    public void Interagir()
    {
        if (!jogadorDentro && !emTransicao)
        {
            emTransicao = true;
            interacaoCoroutine = StartCoroutine(EntrarInteracao());
        }
    }

    private IEnumerator EntrarInteracao()
    {
        jogadorDentro = true;

        if (bloquearMovimento)
            PlayerMovementCC.instance.podeMover = false;

        yield return StartCoroutine(MoverCameraCoroutine(cameraJogador, cameraPosicao, tempoSuavizarCameraAtePosicaoDeEntrar));
        PaiCamQuandoEstaEmTerceiros.SetParent(cameraPosicao);

        if (anim != null && !string.IsNullOrEmpty(animTriggerEntrar))
            anim.SetTrigger(animTriggerEntrar);

        MouseLookStable mouseLook = cameraJogador.GetComponent<MouseLookStable>();

        if (bloquearMouse)
        {
            if (mouseLook != null)
                mouseLook.podeMover = false;
        }

        yield return new WaitForSeconds(duracaoAnimacaoEntrada + cooldownExtraEntrada);

        emTransicao = false; 
        rotacaoBase = cameraJogador.rotation;

        if (mouseLook != null)
            sensibilidadeMouse = mouseLook.mouseSensitivity;

        xRot = 0f;
        yRot = 0f;

        // 🔥 dispara evento configurado no inspector
        onEntrarInteracao?.Invoke();
    }

    private IEnumerator SairInteracao()
    {
        if (anim != null && !string.IsNullOrEmpty(animTriggerSair))
            anim.SetTrigger(animTriggerSair);

        yield return new WaitForSeconds(duracaoAnimacaoSaida);

        if (cameraResetTransform != null)
            yield return StartCoroutine(MoverCameraCoroutine(cameraJogador, cameraResetTransform, tempoSuavizarCameraAtePosicaoDeSair));
        
        PaiCamQuandoEstaEmTerceiros.SetParent(cameraResetTransform);

        if (bloquearMouse)
        {
            MouseLookStable mouseLook = cameraJogador.GetComponent<MouseLookStable>();
            if (mouseLook != null)
                mouseLook.podeMover = true;
        }

        if (bloquearMovimento)
            PlayerMovementCC.instance.podeMover = true;

        jogadorDentro = false;

        yield return new WaitForSeconds(cooldownExtraSaida);

        emTransicao = false; 

        // 🔥 dispara evento configurado no inspector
        onSairInteracao?.Invoke();
    }

    private IEnumerator MoverCameraCoroutine(Transform cameraJogador, Transform alvo, float duracao)
    {
        float t = 0f;
        Vector3 startPos = cameraJogador.position;
        Quaternion startRot = cameraJogador.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime / duracao;
            cameraJogador.position = Vector3.Lerp(startPos, alvo.position, t);
            cameraJogador.rotation = Quaternion.Slerp(startRot, alvo.rotation, t);
            yield return null;
        }

        cameraJogador.position = alvo.position;
        cameraJogador.rotation = alvo.rotation;
    }
}
