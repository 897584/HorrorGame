using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConfiguracaoNoite
{
    [Tooltip("Número da noite (só pra referência visual)")]
    public int id;

    [Tooltip("Quantidades de lobos que podem atacar ao mesmo tempo")]
    public int quantidadeMaxLobos = 1;

    [Tooltip("Duração da noite em segundos")]
    public float duracao = 180f;

    [Tooltip("Duração em segundos do tempo que o lobo demora para entrar pela janela")]
    public float duracaoEspreitarJanela = 30;

    [Tooltip("Velocidade que a sanidade cai")]
    [Range(0f, 100f)] public float vecidadeSanidade = 2f;

    [Tooltip("Sanidade max da noite")]
    [Range(0f, 10000f)] public float SanidadeInicial = 1000f;

    [Tooltip("Configuração de ataques")]
    public List<Ataques> ataquesConfiguracoes = new List<Ataques>();

    [Tooltip("Configuração de tempo")]
    public bool ChuvaAleatorio = false;
    public string ChuvaComecaAs = "09:00";


}

[System.Serializable]
public class Ataques
{
    public string HorarioInicioAtaque;
    public bool AtaqueDisparado = false;
}
