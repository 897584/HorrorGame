using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScene : MonoBehaviour
{
    private string cenaDestino;
    private void Start()
    {
        cenaDestino = PlayerPrefs.GetString("ProximaCena");
        StartCoroutine(CarregarCena());
    }

    private IEnumerator CarregarCena()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(cenaDestino);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            print("Progresso: " + (operation.progress * 100) + "%");
            yield return null;
        }

        // Aqui ele já carregou 90% e só falta ativar
        print("Cena carregada, esperando ativação...");

        yield return new WaitForSeconds(2f);

        operation.allowSceneActivation = true;
    }

}
