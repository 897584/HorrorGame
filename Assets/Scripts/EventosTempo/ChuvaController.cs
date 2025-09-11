using System.Collections;
using UnityEngine;

public class ChuvaController : MonoBehaviour
{
    [Header("Partículas")]
    public ParticleSystem[] particleChuva;

    [Header("Áudio")]
    public GameObject[] temporalAudioObjs; // vários GameObjects com AudioSource

    private AudioSource[] temporalAudios;

    private void Awake()
    {
        // Pega todos os AudioSources
        temporalAudios = new AudioSource[temporalAudioObjs.Length];
        for (int i = 0; i < temporalAudioObjs.Length; i++)
        {
            if (temporalAudioObjs[i] != null)
                temporalAudios[i] = temporalAudioObjs[i].GetComponent<AudioSource>();
        }
    }

    // Começa temporal até 500
    public void ComecarTemporal()
    {
        foreach (var ps in particleChuva)
        {
            StartCoroutine(AjustarRate(ps, 700, 10f));
        }

        // Controla áudio de todos os pontos
        foreach (var audioSource in temporalAudios)
        {
            if (audioSource != null)
            {
                audioSource.gameObject.SetActive(true);
                StartCoroutine(AjustarVolume(audioSource, 0f, 1f, 10f));
            }
        }
    }

    private IEnumerator AjustarRate(ParticleSystem ps, float alvo, float duracao)
    {
        var rate = ps.emission.rateOverTime;
        float inicio = rate.constant;
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            float novoRate = Mathf.Lerp(inicio, alvo, tempo / duracao);
            var emission = ps.emission;
            emission.rateOverTime = novoRate;
            yield return null;
        }

        var finalEmission = ps.emission;
        finalEmission.rateOverTime = alvo;
    }

    private IEnumerator AjustarVolume(AudioSource audioSource, float inicio, float alvo, float duracao, bool desativarNoFinal = false)
    {
        float tempo = 0f;
        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(inicio, alvo, tempo / duracao);
            yield return null;
        }

        audioSource.volume = alvo;

        if (desativarNoFinal)
            audioSource.gameObject.SetActive(false);
    }
}
