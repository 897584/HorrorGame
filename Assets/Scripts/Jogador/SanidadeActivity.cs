using System.Collections;
using UnityEngine;

public class SanidadeActivity : MonoBehaviour
{
    [Header("Configuração da Recuperação")]
    [Tooltip("Percentual da taxa de perda de sanidade que será recuperado (1 = 100%)")]
    [Range(1f, 10f)]public float multiplicadorRecuperacao = 1.5f; // 1.5 = 150%

    private Coroutine recuperandoSanidade;

    public void ComecarRecuperar()
    {
        if (recuperandoSanidade == null)
            recuperandoSanidade = StartCoroutine(Recuperar());
    }

    public void PararRecuperar()
    {
        if (recuperandoSanidade != null)
        {
            StopCoroutine(recuperandoSanidade);
            recuperandoSanidade = null;
        }
    }

    private IEnumerator Recuperar()
    {
        while (true)
        {
            if (NightManager.Instance != null)
            {
                var noite = NightManager.Instance.GetNoiteAtual();
                if (noite != null)
                {
                    // Calcula a taxa de recuperação baseada na velocidade de perda
                    float taxa = noite.vecidadeSanidade * multiplicadorRecuperacao;

                    NightManager.Instance.RecuperarSanidade(taxa * Time.deltaTime);
                }
            }
            yield return null;
        }
    }
}