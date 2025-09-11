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
        CarregarScena("Gameplay");
    }

    public void VoltarParaMenu()
    {
        CarregarScena("Menu");
    }

    private void CarregarScena(string NomeScena)
    {
        PlayerPrefs.SetString("ProximaCena", NomeScena);
        SceneManager.LoadScene("Loading");
    }


    public void SoltarSomClick()
    {
        GetComponent<SoundEffectController>().PlaySound(SoundType.SoundClick, transform.position);
    }

}
