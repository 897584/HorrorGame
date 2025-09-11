using System;
using UnityEngine;

public class ItemEquipavel : MonoBehaviour, IInterativo
{
    public string guid;

    private bool equipado = false;
    private InventarioJogador inventario;

    [Header("Configuração de rotação")]
    public Vector3 RotacaoCustomizada;

    [Header("Armazenamento de Água")]
    public bool ArmazenaAgua = false;
    public float capacidadeAgua = 2f;
    public float quantidadeAtualAgua = 0f;
    public float taxaEnchimento = 0.5f;  

    [Header("Indicador de Nível de Água")]
    public GameObject indicadorAgua;  
    public float alturaMinima = 0f;   
    public float alturaMaxima = 0.2f; 

    [Header("Aquecimento da Água")]
    public bool aguaAquecida = false;    // se a água está aquecida
    public float tempoParaAquecer = 5f;  // segundos necessários no fogo
    public float tempoParaEsfriar = 3f;  // segundos para esfriar
    private float aquecimentoAtual = 0f; // contador interno
    public GameObject particleAquecido;

    public bool estaNaPia = false;
    public bool estaNoFogo = false;

    void Awake()
    {
        guid = Guid.NewGuid().ToString();
    }

    void Start()
    {
        Collider playerCollider = GameMaster.Instance.Player.GetComponent<Collider>();
        Collider myCollider = GetComponent<Collider>();

        if (playerCollider != null && myCollider != null)
        {
            Physics.IgnoreCollision(playerCollider, myCollider, true);
        }

        inventario = GameMaster.Instance.inventarioJogador;
    }

    void Update()
    {
        // Enchimento de água
        if (ArmazenaAgua && estaNaPia && Pia.AguaLigada)
        {
            quantidadeAtualAgua += taxaEnchimento * Time.deltaTime;
            quantidadeAtualAgua = Mathf.Clamp(quantidadeAtualAgua, 0, capacidadeAgua);
        }

        // Aquecimento de água
        AtualizarAquecimento();

        // Atualiza indicador de nível
        AtualizarIndicadorAgua();
    }

    private void AtualizarAquecimento()
    {
        if (!ArmazenaAgua) return;

        if (quantidadeAtualAgua > 0 && estaNoFogo)
        {
            // aquecendo
            aquecimentoAtual += Time.deltaTime;
            if (aquecimentoAtual >= tempoParaAquecer)
            {
                aguaAquecida = true;
                particleAquecido.SetActive(true);
            }
        }
        else
        {
            // esfriando (se tiver tempo definido)
            if (aquecimentoAtual > 0)
            {
                aquecimentoAtual -= Time.deltaTime / tempoParaEsfriar * tempoParaAquecer;
                aquecimentoAtual = Mathf.Clamp(aquecimentoAtual, 0, tempoParaAquecer);

                if (aquecimentoAtual <= 0)
                {
                    aguaAquecida = false;
                    particleAquecido.SetActive(false);
                }
            }
            else
            {
                aguaAquecida = false;
            }
        }
    }

    private void AtualizarIndicadorAgua()
    {
        if (indicadorAgua == null) return;

        MeshRenderer renderer = indicadorAgua.GetComponent<MeshRenderer>();

        if (quantidadeAtualAgua <= 0)
        {
            if (renderer.enabled) renderer.enabled = false;
            return;
        }
        else
        {
            if (!renderer.enabled) renderer.enabled = true;
        }

        float nivel = quantidadeAtualAgua / capacidadeAgua;
        Vector3 pos = indicadorAgua.transform.localPosition;
        pos.y = Mathf.Lerp(alturaMinima, alturaMaxima, nivel);
        indicadorAgua.transform.localPosition = pos;
    }

    public void Interagir()
    {
        if (inventario != null)
        {
            inventario.TentarEquipar(this);
        }
    }

    public void Equipar(Transform ponto)
    {
        if (ponto == null) return;

        estaNaPia = false;
        estaNoFogo = false;

        transform.position = ponto.position;
        transform.localEulerAngles = RotacaoCustomizada;

        Rigidbody rb = GetComponent<Rigidbody>();
        Collider col = GetComponent<Collider>();

        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;

        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        transform.SetParent(ponto);
        equipado = true;

        GameMaster.Instance.soundEffectController.PlaySound(SoundType.Pick_Item, transform.position);
    }

    public void Desequipar()
    {
        if (!equipado) return;

        Rigidbody rb = GetComponent<Rigidbody>();
        Collider col = GetComponent<Collider>();

        if (rb != null) rb.isKinematic = false;
        if (col != null) col.enabled = true;

        gameObject.layer = LayerMask.NameToLayer("Default");

        transform.SetParent(null);
        equipado = false;
    }
}
