using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundEffectController : MonoBehaviour
{
   [System.Serializable]
    public class SoundGroup
    {
        public SoundType type; // Tipo do som
        public GameObject parentObject; // Objeto pai que contém os sons
        public float cooldown; // Tempo de recarga para cada som
        public bool soundLoop;
    }

    public List<SoundGroup> soundGroups; // Lista de grupos de sons

    private Dictionary<SoundType, List<Transform>> soundPool = new Dictionary<SoundType, List<Transform>>();
    private Dictionary<Transform, bool> soundUsage = new Dictionary<Transform, bool>();

    // Faixa de variação de pitch
    public float pitchMin = 0.9f;
    public float pitchMax = 1.1f;

    void Start()
    {
        // Inicializa o dicionário de sons
        foreach (var group in soundGroups)
        {
            if (group.parentObject != null)
            {
                List<Transform> children = new List<Transform>();

                foreach (Transform child in group.parentObject.transform)
                {
                    children.Add(child);
                    child.gameObject.SetActive(false); // Desativa o som inicialmente
                    soundUsage[child] = false; // Marca como disponível
                }

                soundPool[group.type] = children;
            }
        }
    }

    public void PlaySound(SoundType type, Vector3 position)
    {
        if (!soundPool.ContainsKey(type)) return;

        var sounds = soundPool[type];
        var livres = sounds.Where(s => !soundUsage[s]).ToList();
        if (livres.Count == 0) return;

        // escolhe um livre aleatório
        var sound = livres[Random.Range(0, livres.Count)];
        soundUsage[sound] = true;
        StartCoroutine(ResetSoundUsage(sound, type));

        sound.position = position;
        sound.gameObject.SetActive(true);

        var audioSource = sound.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.pitch = Random.Range(pitchMin, pitchMax);
            audioSource.Play();
        }
    }

    public void StopSound(SoundType type)
    {
        if (!soundPool.ContainsKey(type)) return;

        foreach (var sound in soundPool[type])
        {
            var audioSource = sound.GetComponent<AudioSource>();
            if (sound.gameObject.activeSelf && audioSource != null)
            {
                audioSource.Stop(); // Para o áudio
                sound.gameObject.SetActive(false); // Desativa o GameObject
                soundUsage[sound] = false; // Marca como disponível
            }
        }
    }


    private IEnumerator ResetSoundUsage(Transform sound, SoundType type)
    {
        // Aguarda o cooldown especificado para o som
        var soundSelected = soundGroups.Find(group => group.type == type);
        if (soundSelected.soundLoop) yield break;
        var cooldown = soundSelected.cooldown;

        yield return new WaitForSeconds(cooldown);

        sound.gameObject.SetActive(false); // Desativa o som
        soundUsage[sound] = false; // Marca como disponível
    }
}



public enum SoundType
{
    Ambience_windLoop,
    Passo_Madeira,
    Passo_Concreto,
    PassoGrass,
    PortaAbrir,
    PortaFechar,
    ArmarioAbrir,
    ArmarioFechar,
    Passo_Carpet,
    Pick_Item,
    SoundClick,
    TelefoneSemSinal,
    TocandoCelular,

}
