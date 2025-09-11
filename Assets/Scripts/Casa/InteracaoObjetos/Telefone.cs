using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class Dublagem
{
    public GameObject objeto;    // GameObject que tem a dublagem
    public float delay;          // Tempo em segundos até tocar
}

public class Telefone : MonoBehaviour
{
    public Dublagem[] dublagens; // Configure no Inspector

    private bool tocouSequencia = false; // Garante que toque só uma vez

    void Start()
    {
        StartCoroutine(TocarTelefone());
    }

    private IEnumerator TocarTelefone()
    {
        yield return new WaitForSeconds(3);
        GameMaster.Instance.soundEffectController.PlaySound(SoundType.TocandoCelular, transform.position);
    }

    public void AtendeuOTelefone()
    {
        if (!tocouSequencia)
        {
            StopCoroutine(TocarTelefone());
            GameMaster.Instance.soundEffectController.StopSound(SoundType.TocandoCelular);
            StartCoroutine(SequenciaDublagens());
            tocouSequencia = true;
        }
    }

    private IEnumerator SequenciaDublagens()
    {
        foreach (var dub in dublagens)
        {
            if (dub.objeto != null)
                dub.objeto.SetActive(true);

            yield return new WaitForSeconds(dub.delay);
        }
        NightManager.Instance.PausarNoite = false;
    }
}
