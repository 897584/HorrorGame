using UnityEngine;
using System; // necessário para Action

public class Pia : MonoBehaviour, IInterativo
{
    [Header("Partículas da água ligada")]
    public GameObject aguaAtiva; // objeto que representa a água (partículas)

    private bool estaLigada = false;
    public static bool AguaLigada { get; private set; }

    public void Interagir()
    {
        estaLigada = !estaLigada; // alterna o estado

        if (aguaAtiva != null)
        {
            aguaAtiva.SetActive(estaLigada);
        }
        AguaLigada = estaLigada;
    }
}
