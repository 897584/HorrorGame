using UnityEngine;
using System.Collections;

public class Porta : MonoBehaviour, IInterativo
{
    [Header("Configurações da Porta")]
    public Vector3 rotacaoFechada = Vector3.zero; 
    public Vector3 rotacaoAberta = new Vector3(0, 120, 0);

    [Header("Estado Inicial")]
    public bool iniciaAberta = false;

    [Header("Referências")]
    private Collider col; // só usado em portas (não em armarinho)

    private bool aberta;
    private Coroutine animacaoAtual;

    private float tempoAnimacaoAbrindo;
    private float tempoAnimacaoFechando;

    [Tooltip("Se for marcado, funciona só por animação (sem física)")]
    public bool armarinho = false;

    void Start()
    {
        if (!armarinho)
            col = GetComponent<Collider>();

        tempoAnimacaoAbrindo = armarinho ? 1f : 1.8f;
        tempoAnimacaoFechando = armarinho ? 0.3f : 1.3f;

        aberta = iniciaAberta;

        // rotação inicial
        transform.localRotation = Quaternion.Euler(aberta ? rotacaoAberta : rotacaoFechada);
    }

    public void Interagir()
    {
        aberta = !aberta;

        if (animacaoAtual != null)
            StopCoroutine(animacaoAtual);

        animacaoAtual = StartCoroutine(MoverPorta(aberta));
    }

    private IEnumerator MoverPorta(bool abrir)
    {
        GameMaster.Instance.soundEffectController.PlaySound(
            abrir ?
            (armarinho ? SoundType.ArmarioAbrir : SoundType.PortaAbrir) :
            (armarinho ? SoundType.ArmarioFechar : SoundType.PortaFechar),
            transform.position);

        Quaternion rotacaoInicial = transform.localRotation;
        Quaternion rotacaoFinal = Quaternion.Euler(abrir ? rotacaoAberta : rotacaoFechada);

        float tempo = 0f;
        float tempoAnimacao = abrir ? tempoAnimacaoAbrindo : tempoAnimacaoFechando;

        // Se for porta, garante que comece com collider ativo
        if (!armarinho && col != null)
            col.enabled = true;

        while (tempo < tempoAnimacao)
        {
            tempo += Time.deltaTime;
            float t = tempo / tempoAnimacao;

            transform.localRotation = Quaternion.Slerp(rotacaoInicial, rotacaoFinal, t);

            // 👉 Só desativa o collider depois de 30% da animação
            if (!armarinho && col != null && t >= 0.3f && col.enabled)
                col.enabled = false;

            yield return null;
        }

        transform.localRotation = rotacaoFinal;

        // 👉 Reativa no final
        if (!armarinho && col != null)
            col.enabled = true;
    }
}
