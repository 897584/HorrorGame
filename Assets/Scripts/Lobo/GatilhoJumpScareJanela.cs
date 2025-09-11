using UnityEngine;

public class GatilhoJumpScareJanela : MonoBehaviour
{
    float temp = 0;
    bool entrou = false;
    bool JumpScareGatilho = false;

    public Janela janelaQueEstou;
    void Update()
    {
        if (entrou) temp += Time.deltaTime;

        if (temp > 1 && !JumpScareGatilho && janelaQueEstou.quantidadeMadeiraAtiva <= 0)
        {
            JumpScareGatilho = true;
            GetComponentInParent<LoboIA>().JumpScare();
        }

        if (temp > 1 && janelaQueEstou.quantidadeMadeiraAtiva > 0)
        {
            JumpScareGatilho = true;
            GetComponentInParent<LoboIA>().JumpScareComMadeira();
            temp = -3;
            entrou = false;
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && GetComponentInParent<LoboIA>().espreitando) entrou = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && GetComponentInParent<LoboIA>().espreitando) entrou = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")  entrou = false;
    }
}
