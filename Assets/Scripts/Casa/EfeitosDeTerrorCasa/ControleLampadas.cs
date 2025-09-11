using System.Collections;
using UnityEngine;

public class ControleLampadas : MonoBehaviour
{
    private Light lampada;
    private float intensidadeOriginal;

    void Start()
    {
        lampada = GetComponent<Light>();
        if (lampada != null)
            intensidadeOriginal = lampada.intensity;

        if (LoboSpawner.Instance != null)
        {
            LoboSpawner.Instance.OnLuzesPiscarem += PiscarLuzes;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            PiscarLampada();
    }

    private void PiscarLuzes(LoboIA lobo)
    {
        PiscarLampada();
    }

    public void PiscarLampada()
    {
        StopAllCoroutines(); // garante que não acumule várias corrotinas
        StartCoroutine(PiscarCoroutine());
    }

    private IEnumerator PiscarCoroutine()
    {
        for (int i = 0; i < 2; i++)
        {
            yield return StartCoroutine(AlterarIntensidade(intensidadeOriginal * 0.3f, 0.1f));
            // voltar ao normal
            yield return StartCoroutine(AlterarIntensidade(intensidadeOriginal, 0.15f));
        }
    }

    private IEnumerator AlterarIntensidade(float alvo, float duracao)
    {
        float inicio = lampada.intensity;
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            lampada.intensity = Mathf.Lerp(inicio, alvo, tempo / duracao);
            yield return null;
        }

        lampada.intensity = alvo; // garante valor final
    }
}
