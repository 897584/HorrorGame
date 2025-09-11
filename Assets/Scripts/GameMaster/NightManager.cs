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

    // Evento disparado quando a noite termina
    public event Action OnNoiteVencida;

    public event Action OnGameOver;


    private float PontuacaoDaNoite;
    private float PontuacaoDaRun;

    public float SanidadeAtual;

    [Header("Fog Config")]
    public float fogMin = 0.002f; // fog normal (sanidade cheia)
    public float fogMax = 0.05f;  // fog forte (sanidade zerada)


    [Header("Áudio de Loucura")]
    public AudioSource audioLoucura; // Arraste o AudioSource no Inspector
    public float volumeMaximo = 1f;   // Volume máximo quando a sanidade chega a 0
    public float volumeMinimo = 0f;   // Volume inicial
    private bool musicaAtiva = false;


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
    private int balaChuva = 1;

    void Update()
    {

        if (balaChuva == 1 && JaPassouHorario(noiteAtual.ChuvaComecaAs))
        {
            GameMaster.Instance.chuvaController.ComecarTemporal();
            balaChuva = 0;
        }

        if (noiteRolando && !PausarNoite)
        {
            SanidadeAtual -= noiteAtual.vecidadeSanidade * Time.deltaTime;
            tempoRestante -= Time.deltaTime;

            if (tempoRestante <= 0)
            {
                VencerNoite();
            }
            if (SanidadeAtual <= 0) print("MORTE POR BIXIN");

            GameMaster.Instance.sanidadeBarra.fillAmount = SanidadeAtual / noiteAtual.SanidadeInicial;

            AtualizarFog();
            AtualizarChromaticAberration();
            AtualizarAudioLoucura();
            AtualizarRelogio();
        }
        else
        {
            audioLoucura.Stop();
        }
    }
    public void IniciarNoite()
    {
        var noiteEscolhidaIndex = PlayerPrefs.GetInt("NoiteEscolhida");

        noiteAtual = configuracoes[noiteEscolhidaIndex];
        SanidadeAtual = noiteAtual.SanidadeInicial;
        tempoRestante = noiteAtual.duracao;
        txtRelogio = GameMaster.Instance.RelogioText;
        txtRelogio.text = "22:00";
        balaChuva = 1;
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

     private void AtualizarFog()
    {
        float porcentagem = Mathf.Clamp01(SanidadeAtual / noiteAtual.SanidadeInicial);

        float t = Mathf.InverseLerp(0.5f, 0f, porcentagem); 

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = Mathf.Lerp(fogMin, fogMax, t);
    }

    private void AtualizarAudioLoucura()
    {
        if (audioLoucura == null) return;

        float porcentagem = Mathf.Clamp01(SanidadeAtual / noiteAtual.SanidadeInicial);

        // Começa a tocar quando chega a 50% ou menos
        if (porcentagem <= 0.5f && !musicaAtiva)
        {
            audioLoucura.Play();
            musicaAtiva = true;
        }

        if (musicaAtiva)
        {
            // Interpolação do volume: quanto menor a sanidade, maior o volume
            float t = Mathf.InverseLerp(0.5f, 0f, porcentagem);
            audioLoucura.volume = Mathf.Lerp(volumeMinimo, volumeMaximo, t);
        }
    }

    public void RecuperarSanidade(float quantidade)
    {
        if (!noiteRolando) return;

        SanidadeAtual = Mathf.Min(SanidadeAtual + quantidade, noiteAtual.SanidadeInicial);
    }


    private void AtualizarChromaticAberration()
    {
        if (chromaticAberration == null)
            chromaticAberration = GameMaster.Instance.chromaticAberration;

        if (chromaticAberration == null) return;

        float porcentagem = Mathf.Clamp01(SanidadeAtual / noiteAtual.SanidadeInicial);

        // Mesmo esquema: só depois de 50% começa a subir
        float t = Mathf.InverseLerp(0.5f, 0f, porcentagem);

        chromaticAberration.intensity.value = Mathf.Lerp(0f, 1f, t);
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
