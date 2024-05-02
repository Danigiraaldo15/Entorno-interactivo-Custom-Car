using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("CONFIG. AUDIO")]
    [SerializeField] AudioMixer mixer;
    public Slider Slider_Musica;
    public Slider Slider_SFX;
    public float SliderMusica_Vol;
    public float SliderSFX_Vol;
    const string Mix_Music = "Musica";
    const string Mix_SFX = "SFX";

    private void Start()
    {
        Slider_Musica.value = PlayerPrefs.GetFloat("MusicaVol", SliderMusica_Vol); // Aquí toma el valor definido abajo y lo establence al iniciar la escena.
        AudioListener.volume = Slider_Musica.value;

        Slider_SFX.value = PlayerPrefs.GetFloat("SFXVol", SliderSFX_Vol); // Aquí toma el valor definido abajo y lo establence al iniciar la escena.
        AudioListener.volume = Slider_SFX.value;
    }

    public void MusicaVol(float valor) // Aqui se establece el valor del volumen de musica.
    {
        mixer.SetFloat(Mix_Music, Mathf.Log10(valor) * 20);
        SliderMusica_Vol = valor;
        PlayerPrefs.SetFloat("MusicaVol", SliderMusica_Vol);
    }

    public void SFXVol(float valor) // Aqui se establece el valor del volumen de SFX.
    {
        mixer.SetFloat(Mix_SFX, Mathf.Log10(valor) * 20);
        SliderSFX_Vol = valor;
        PlayerPrefs.SetFloat("SFXVol", SliderSFX_Vol);
    }
    private void Awake()
    {
        Slider_Musica.onValueChanged.AddListener(MusicaVol);
        Slider_SFX.onValueChanged.AddListener(SFXVol);
    }
}
