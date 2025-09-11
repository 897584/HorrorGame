using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{

    public static GameMaster Instance { get; private set; }

    public SoundEffectController soundEffectController;
    public InventarioJogador inventarioJogador;
    public GameObject IconMaozinha;

    public Transform PaiCamMovAnim;
    public Transform Player;
    public Transform CameraJogador;
    public Transform MovQuandoEstaAnimacaoTerceiros;
    public Transform posicaoDaCasa;

    public Image sanidadeBarra;
    public Text RelogioText;
    public Animator animatorBlood;

    public PostProcessVolume volume;
    [HideInInspector]
    public ChromaticAberration chromaticAberration;

    public ChuvaController chuvaController;

    private void Awake()
    {
        // Garante que só exista um GameMaster
        if (Instance != null && Instance != this) Debug.LogError("InteracaoJogador precisa estar em um objeto com Camera!");

        Instance = this;

        if (volume != null)
        {
            volume.profile.TryGetSettings(out chromaticAberration);
        }
    }

    void Start()
    {
        soundEffectController.PlaySound(SoundType.Ambience_windLoop, transform.position);

        if (NightManager.Instance != null) NightManager.Instance.IniciarNoite();
        else Debug.LogError("Inicie o jogo pelo menu");
    }

    public void levouDano()
    {
        animatorBlood.SetTrigger("Dano");
    }
}
