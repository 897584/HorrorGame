using UnityEngine;

public class ControladorPassos : MonoBehaviour
{
    public PlayerMovementCC movimentoJogador;
    public LayerMask camadaDeteccaoChao;

    public enum TipoChao { Grama, Pedra, Madeira, Tapete }
    private TipoChao chaoAtual = TipoChao.Grama;

    private float tempoPasso = 0f;
    public float intervaloPassoAndando = 0.5f;
    public float intervaloPassoCorrendo = 0.3f;
    public float multiplicadorPassoAgachado = 1.8f;

    void Update()
    {
        tempoPasso -= Time.deltaTime;

        if (movimentoJogador.moveDir.magnitude > 0.3f && tempoPasso <= 0f && movimentoJogador.GetComponent<CharacterController>().isGrounded && movimentoJogador.podeMover)
        {
            VerificarChao();
            TocarPasso(chaoAtual);

            float intervalo = movimentoJogador.isRunning ? intervaloPassoCorrendo : intervaloPassoAndando;
            if (movimentoJogador.isCrouching) intervalo *= multiplicadorPassoAgachado;

            tempoPasso = intervalo;
        }
    }

    void VerificarChao()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3, camadaDeteccaoChao))
        {
            if (hit.collider.CompareTag("pedra"))
                chaoAtual = TipoChao.Pedra;
            else if (hit.collider.CompareTag("madeira"))
                chaoAtual = TipoChao.Madeira;
            else if (hit.collider.CompareTag("carpet"))
                chaoAtual = TipoChao.Tapete;
            else
                chaoAtual = TipoChao.Grama;
        }
        else
        {
            chaoAtual = TipoChao.Grama;
        }
    }

    void TocarPasso(TipoChao chao)
    {
        switch (chao)
        {
            case TipoChao.Pedra:
                GameMaster.Instance.soundEffectController.PlaySound(SoundType.Passo_Concreto, transform.position);
                break;
            case TipoChao.Madeira:
                GameMaster.Instance.soundEffectController.PlaySound(SoundType.Passo_Madeira, transform.position);
                break;
            case TipoChao.Grama:
                GameMaster.Instance.soundEffectController.PlaySound(SoundType.PassoGrass, transform.position);
                break;
            case TipoChao.Tapete:
                GameMaster.Instance.soundEffectController.PlaySound(SoundType.Passo_Carpet, transform.position);
                break;
            default:
                GameMaster.Instance.soundEffectController.PlaySound(SoundType.PassoGrass, transform.position);
                break;
        }
    }
}
