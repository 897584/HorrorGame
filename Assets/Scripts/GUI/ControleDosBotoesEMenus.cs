using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControleDosBotoesEMenus : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void PlayGame()
    {
        CarregarScena("Mapa1");
    }

    public void VoltarParaMenu()
    {
        CarregarScena("MenuPrincipal");
    }

    private void CarregarScena(string NomeScena)
    {
        PlayerPrefs.SetString("ProximaCena", NomeScena);
        SceneManager.LoadScene("LoadingScene");
    }


    public void SoltarSomClick()
    {
        GetComponent<SoundEffectController>().PlaySound(SoundType.SoundClick, transform.position);
    }

}
