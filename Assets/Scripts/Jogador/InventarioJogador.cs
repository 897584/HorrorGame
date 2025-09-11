using UnityEngine;

public class InventarioJogador : MonoBehaviour
{
    public Transform[] posicoesItens;
    private ItemEquipavel[] itensEquipados;
    private ItemEquipavel itemAtivo;
    private int slotAtivo = -1; // controla qual slot está ativo

    void Start()
    {
        itensEquipados = new ItemEquipavel[posicoesItens.Length];
    }

    void Update()
    {
        // Soltar item ativo
        if (Input.GetKeyDown(KeyCode.G))
        {
            SoltarItemAtivo();
        }
    }

    public void TentarEquipar(ItemEquipavel item)
    {

        if (itensEquipados[0] == null)
        {
            itensEquipados[0] = item;
            item.Equipar(posicoesItens[0]);

            AtivarItem(0);
        }
    }

    private void AtivarItem(int slot)
    {
        if (slot < 0 || slot >= itensEquipados.Length) return;

        // Desativa o item ativo anterior
        if (itemAtivo != null)
        {
            itemAtivo.gameObject.SetActive(false);
        }

        itemAtivo = itensEquipados[slot];
        slotAtivo = slot;

        if (itemAtivo != null)
        {
            itemAtivo.gameObject.SetActive(true);
        }
    }

    private void DesativarItens()
    {
        if (itemAtivo != null)
        {
            itemAtivo.gameObject.SetActive(false);
            itemAtivo = null;
            slotAtivo = -1;
        }
    }

    private void SoltarItemAtivo()
{
    if (itemAtivo == null) return;

    Transform cam = GameMaster.Instance.CameraJogador;


    // Remove do array
    itensEquipados[slotAtivo] = null;

    // Desequipa
    itemAtivo.Desequipar(); 

    // --- NOVO: Raycast para verificar posição fixa ---
    Ray ray = new Ray(cam.position, cam.forward);
    if (Physics.Raycast(ray, out RaycastHit hit, 5f)) // raio de 5 metros (ajusta se quiser)
    {
        if (hit.collider.CompareTag("PosicoesItens"))
        {
            // Tenta achar um Transform "posicoesItens[0]" nesse objeto
            InventarioJogador inv = this;
            if (inv != null && inv.posicoesItens.Length > 0)
            {
                itemAtivo.transform.position = hit.transform.position;
                itemAtivo.transform.rotation = hit.transform.rotation;
                itemAtivo.gameObject.SetActive(true);

                if (hit.collider.GetComponent<PosicoesIdentify>().nomePosicao == "pia")
                    itemAtivo.estaNaPia = true;

                if (hit.collider.GetComponent<PosicoesIdentify>().nomePosicao == "fogo")
                    itemAtivo.estaNoFogo = true;


                // Reseta física
                Rigidbody rb = itemAtivo.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                itemAtivo = null;
                slotAtivo = -1;
                return; 
            }
        }
    }

    // --- Caso não tenha encontrado posição fixa, solta normalmente ---
    Rigidbody rbNormal = itemAtivo.GetComponent<Rigidbody>();

    // Define posição inicial (um pouco à frente da câmera)
    itemAtivo.transform.position = cam.position + cam.forward * 0.2f;
    itemAtivo.gameObject.SetActive(true);

    // Reseta física
    rbNormal.velocity = Vector3.zero;
    rbNormal.angularVelocity = Vector3.zero;

    // Direção baseada na câmera
    Vector3 direcaoArremesso = cam.forward;

    // Aplica força (impulso na direção da câmera, com leve arco pra cima)
    rbNormal.AddForce(direcaoArremesso * 10f + Vector3.up * 1.5f, ForceMode.Impulse);

    // Limpa referência
    itemAtivo = null;
    slotAtivo = -1;
}


}
