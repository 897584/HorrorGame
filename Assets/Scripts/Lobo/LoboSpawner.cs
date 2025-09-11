
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoboSpawner : MonoBehaviour
{
    public static LoboSpawner Instance { get; private set; }

    public GameObject prefabLobo;

    private ConfiguracaoNoite configuracaoNoite;

    public List<Transform> JanelaAtaques = new List<Transform>();

    private int lobosAtivos = 0;

    public event System.Action<LoboIA> OnLuzesPiscarem;

    void Start()
    {
        if (Instance != null && Instance != this) Debug.LogError("o instance do lobo Spawner esta preenchido");
        Instance = this;

        // pega a configuração da noite atual
        configuracaoNoite = NightManager.Instance.GetNoiteAtual();
        if (configuracaoNoite == null) Debug.LogError("configuração da noite no loboSpawn está null");


    }

    void Update()
    {
        if (NightManager.Instance.PausarNoite) return;

        if (configuracaoNoite.ataquesConfiguracoes.Any(ataque => !ataque.AtaqueDisparado && NightManager.Instance.JaPassouHorario(ataque.HorarioInicioAtaque)))
        {
            RealizarAtaque();
            var ataque = configuracaoNoite.ataquesConfiguracoes.First(ataque => !ataque.AtaqueDisparado && NightManager.Instance.JaPassouHorario(ataque.HorarioInicioAtaque));
            ataque.AtaqueDisparado = true;
        }
    }

    void RealizarAtaque()
    {
        if (lobosAtivos < configuracaoNoite.quantidadeMaxLobos) SpawnLobo();
        else Debug.LogWarning("Ele ia realizar ataque mas o numero de lobos ativos ja estava no maximo");
    }

    void SpawnLobo()
    {
        var janelaParaAtacar = JanelaAtaques[Random.Range(0, JanelaAtaques.Count)];

        Vector3 posicaoSpawn = janelaParaAtacar.GetChild(0).transform.position;
        var loboInstanciado = Instantiate(prefabLobo, posicaoSpawn, Quaternion.identity);

        var loboIA = loboInstanciado.GetComponent<LoboIA>();
        loboIA.JanelaAlvo = janelaParaAtacar;

        loboIA.LoboEspreitouUmaJanela += () =>
        {
            OnLuzesPiscarem?.Invoke(loboIA);
        };

        lobosAtivos ++;
    }

    public void RemoverLobo()
    {
        lobosAtivos = Mathf.Max(0, lobosAtivos - 1);
    }

}
