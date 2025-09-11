using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class NightManager : MonoBehaviour
{
    public static NightManager Instance { get; private set; }

    [Header("Configurações das noites")]
    public List<ConfiguracaoNoite> configuracoes = new List<ConfiguracaoNoite>();

    private ConfiguracaoNoite noiteAtual;

    private float tempoRestante;
    public bool PausarNoite = false;
    private bool noiteRolando = false;
    private Text txtRelogio;

    public event Action OnNoiteVencida;

    public event Action OnGameOver;

    private float PontuacaoDaNoite;
    private float PontuacaoDaRun;

    public Dificuldade dificuldade = Dificuldade.normal;
    public enum Dificuldade
    {
        normal,
        facil,
        dificil,
        inferno
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetarDificuldade(int dif) 
    {
        dificuldade = (Dificuldade)dif;
    }

    private ChromaticAberration chromaticAberration;

    void Update()
    {
        if (noiteRolando && !PausarNoite)
        {
            tempoRestante -= Time.deltaTime;

            if (tempoRestante <= 0)
            {
                VencerNoite();
            }

            AtualizarRelogio();
        }
    }
    public void IniciarNoite()
    {
        tempoRestante = noiteAtual.duracao;
        txtRelogio = GameMaster.Instance.RelogioText;
        txtRelogio.text = "22:00";
        if (noiteAtual.ChuvaAleatorio) noiteAtual.ChuvaComecaAs = CriarHorarioAleatorio();

        noiteRolando = true;
    }

    private string CriarHorarioAleatorio()
    {
        int hora;
        if (UnityEngine.Random.value < (2f / 9f))
        {
            hora = 22 + UnityEngine.Random.Range(0, 2); 
        }
        else 
        {
            hora = UnityEngine.Random.Range(0, 8);
        }

        // Gera minutos aleatórios
        int minutos = UnityEngine.Random.Range(0, 60);

        return string.Format("{0:00}:{1:00}", hora, minutos);
    }

    public bool JaPassouHorario(string horarioAlvo)
    {
        if (txtRelogio == null) return false;

        // Tenta converter o horário alvo
        if (!TimeSpan.TryParse(horarioAlvo, out TimeSpan horaAlvo))
        {
            Debug.LogError("Formato de horário inválido! Use HH:MM");
            return false;
        }

        // Tenta pegar o horário atual do txtRelogio
        if (!TimeSpan.TryParse(txtRelogio.text, out TimeSpan horaAtual))
        {
            Debug.LogError("txtRelogio está com formato inválido!");
            return false;
        }

        // Retorna true se já passou ou é igual
        return horaAtual >= horaAlvo;
    }

    
    private void AtualizarRelogio()
    {
        if (txtRelogio == null || noiteAtual == null) return;

        int totalHoras = 9;

        // Quanto da noite já passou (0 = início, 1 = fim)
        float progresso = 1f - Mathf.Clamp01(tempoRestante / noiteAtual.duracao);

        float horasPassadas = progresso * totalHoras;
        int hora = 22 + Mathf.FloorToInt(horasPassadas);
        int minutos = Mathf.FloorToInt((horasPassadas - Mathf.Floor(horasPassadas)) * 60f);

        if (hora >= 24)
            hora -= 24;

        // Formata em HH:MM
        txtRelogio.text = string.Format("{0:00}:{1:00}", hora, minutos);
    }


    private void VencerNoite()
    {
        noiteRolando = false;
        PontuacaoDaNoite = noiteAtual.duracao; // ou tempo sobrevivido
        PontuacaoDaRun += PontuacaoDaNoite;

        Debug.Log($"✅ Noite {noiteAtual.id} vencida! Pontos desta noite: {PontuacaoDaNoite}, Total: {PontuacaoDaRun}");

        OnNoiteVencida?.Invoke();

        CarregarScena("TelaNoiteCompleta");
    }

    public void GameOver()
    {
        noiteRolando = false;
        Debug.Log("💀 Game Over!");
        OnGameOver?.Invoke();
        CarregarScena("GameOver");
    }

    private void CarregarScena(string NomeScena)
    {
        PlayerPrefs.SetString("ProximaCena", NomeScena);
        SceneManager.LoadScene("LoadingScene");
    }

    public ConfiguracaoNoite GetNoiteAtual()
    {
        return noiteAtual;
    }

    public float GetTempoRestante()
    {
        return tempoRestante;
    }
}
