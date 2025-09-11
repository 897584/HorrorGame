using System;
using System.Collections;
using UnityEngine;

public class LoboIA : MonoBehaviour
{
    [Header("Configuração do Ataque")]
    public Transform JanelaAlvo; // Definido pelo Spawner
    public Vector3 posNascimento;
    public float velocidadeMovimento = 1.8f;
    public float distanciaMinima = 0.2f;

    public bool espreitando = false;
    private Animator anim;

    [Header("Jump Scare")]
    public Transform cameraLoboJumpScarePosicao;

    public event Action LoboEspreitouUmaJanela;


    ConfiguracaoNoite configuracaoNoite;

    void Start()
    {
        posNascimento = transform.position;
        if (JanelaAlvo != null)
        {
            anim = GetComponent<Animator>();
            StartCoroutine(EspreitarCoroutine());
            GetComponentInChildren<GatilhoJumpScareJanela>().janelaQueEstou = JanelaAlvo.GetComponent<Janela>();
        }
        else
        {
            Debug.LogError("⚠️ Lobo sem JanelaAlvo definida!");
        }

        configuracaoNoite = NightManager.Instance.GetNoiteAtual();


    }

    void Update()
    {
    }

    public void VoltarParaNascimento()
    {
        StopAllCoroutines();
        StartCoroutine(VoltarCoroutine());
    }

    private IEnumerator VoltarCoroutine()
    {
        anim.SetBool("Correndo", true);
        anim.SetBool("Espreitando", false);
        espreitando = false;

        while (Vector3.Distance(transform.position, posNascimento) > distanciaMinima)
        {
            Vector3 direcao = (posNascimento - transform.position).normalized;
            transform.position += direcao * velocidadeMovimento*2 * Time.deltaTime;

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direcao),
                Time.deltaTime * 5f
            );

            yield return null;
        }
        Destroy(gameObject);
    }


    private IEnumerator EspreitarCoroutine()
    {
        anim.SetBool("Andando", true);

        while (Vector3.Distance(transform.position, JanelaAlvo.position) > distanciaMinima)
        {
            Vector3 direcao = (JanelaAlvo.position - transform.position).normalized;
            transform.position += direcao * velocidadeMovimento * Time.deltaTime;

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direcao),
                Time.deltaTime * 5f
            );

            yield return null;
        }

        anim.SetBool("Andando", false);
        espreitando = true;
        anim.SetBool("Espreitando", true);
        anim.SetTrigger("Espreitar");

        transform.position = JanelaAlvo.position;
        transform.rotation = JanelaAlvo.rotation;

        LoboEspreitouUmaJanela?.Invoke();
    }

    public void JumpScare()
    {
        GetComponentInParent<Animator>().SetTrigger("JumpScare");

        Transform cameraJogador = GameMaster.Instance.CameraJogador;

        // trava o mouse por 5s
        MouseLookStable mouseLook = cameraJogador.GetComponent<MouseLookStable>();
        if (mouseLook != null)
        {
            mouseLook.TravarMovimento(5f);
        }

        // move até posição de jumpscare
        StartCoroutine(SequenciaJumpScare(cameraJogador));
    }

    public void JumpScareComMadeira()
    {
        GetComponentInParent<Animator>().SetTrigger("JumpScareJanelaMadeira");

        Transform cameraJogador = GameMaster.Instance.CameraJogador;

        // trava o mouse
        MouseLookStable mouseLook = cameraJogador.GetComponent<MouseLookStable>();
        if (mouseLook != null)
        {
            mouseLook.TravarMovimento(0.56f);
        }

        StartCoroutine(SequenciaJumpScareMadeira(cameraJogador));
    }

    private IEnumerator SequenciaJumpScare(Transform cameraJogador)
    {
        // 1️⃣ Move até o lobo
        yield return StartCoroutine(MoverCameraCoroutine(cameraJogador, cameraLoboJumpScarePosicao, 0.4f));

        // quando chegar, vira filho do lobo
        cameraJogador.SetParent(cameraLoboJumpScarePosicao);

        // 2️⃣ Espera o tempo do jumpscare (4 segundos)
        yield return new WaitForSeconds(3.7f);

        NightManager.Instance.GameOver();
    }

    private IEnumerator SequenciaJumpScareMadeira(Transform cameraJogador)
    {
        // 1️⃣ Move até o lobo
        yield return StartCoroutine(MoverCameraCoroutine(cameraJogador, cameraLoboJumpScarePosicao, 0.1f));
        var paiAtualCamera = cameraJogador.parent;
        cameraJogador.SetParent(cameraLoboJumpScarePosicao);

        GameMaster.Instance.levouDano();

        // 2️⃣ Espera o tempo do jumpscare 
        yield return new WaitForSeconds(0.7f);

        var player = GameMaster.Instance.Player;
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = new Vector3(cameraLoboJumpScarePosicao.transform.position.x, player.transform.position.y, cameraLoboJumpScarePosicao.transform.position.z);

        yield return StartCoroutine(MoverCameraCoroutine(cameraJogador, paiAtualCamera, 0.1f));

        cameraJogador.SetParent(paiAtualCamera);

        player.GetComponent<CharacterController>().enabled = true;
    }


    private IEnumerator MoverCameraCoroutine(Transform cameraJogador, Transform alvo, float duracao)
    {
        Vector3 posInicial = cameraJogador.position;
        Quaternion rotInicial = cameraJogador.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracao;

            cameraJogador.position = Vector3.Lerp(posInicial, alvo.position, t);
            cameraJogador.rotation = Quaternion.Slerp(rotInicial, alvo.rotation, t);

            yield return null;
        }

        cameraJogador.position = alvo.position;
        cameraJogador.rotation = alvo.rotation;
    }
}
