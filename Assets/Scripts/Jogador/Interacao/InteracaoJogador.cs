using UnityEngine;

public interface IInterativo
{
    void Interagir();
}

public class InteracaoJogador : MonoBehaviour
{
    public float alcance = 3f; // distância máxima de interação
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("InteracaoJogador precisa estar em um objeto com Camera!");
        }
    }

    bool estaMirandoInterativo = false;
    bool estavaMirandoInterativo = false;
    void Update()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, alcance))
        {
            IInterativo interativo = hit.collider.GetComponent<IInterativo>();
            if (interativo != null)
            {
                if (Input.GetKeyDown(KeyCode.E)) interativo.Interagir();
                estaMirandoInterativo = true;
            }
            else estaMirandoInterativo = false;
        }
        else estaMirandoInterativo = false;

        if (estaMirandoInterativo != estavaMirandoInterativo)
        {
            GameMaster.Instance.IconMaozinha.SetActive(estaMirandoInterativo);
            estavaMirandoInterativo = estaMirandoInterativo;
        }

    }
}
