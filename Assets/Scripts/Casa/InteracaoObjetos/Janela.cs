using System;
using UnityEngine;

public class Janela : MonoBehaviour
{
    public int quantidadeMadeiraAtiva = 1;

    public GameObject Madeira;
    void Start()
    {
        if (quantidadeMadeiraAtiva >= 1) Madeira.SetActive(true);
        else Madeira.SetActive(false);
    }

    public void consumirMadeira()
    {
        quantidadeMadeiraAtiva = 0;
        DestruirEfeitoMadeira();
    }

    private void DestruirEfeitoMadeira()
    {
        throw new NotImplementedException();
    }
}
