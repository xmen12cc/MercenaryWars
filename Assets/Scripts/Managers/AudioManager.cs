using UnityEngine;

namespace MercenaryWars.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("Background")]
        [SerializeField] private AudioSource bgMusicSource;

        [Header("SFX Sources (add 4 Audio Source components)")]
        [SerializeField] private AudioSource[] sfxSources;
        private int sfxIndex = 0;

        [Header("Player SFX")]
        [SerializeField] private AudioClip footstepClip;
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip shootClip;
        [SerializeField] private AudioClip playerHurtClip;
        [SerializeField] private AudioClip meleeClip;

        [Header("Enemy SFX")]
        [SerializeField] private AudioClip enemySpotClip;
        [SerializeField] private AudioClip enemyAttackClip;
        [SerializeField] private AudioClip enemyDeathClip;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Preload all clips to eliminate first-play delay
            PreloadClip(footstepClip);
            PreloadClip(jumpClip);
            PreloadClip(shootClip);
            PreloadClip(playerHurtClip);
            PreloadClip(meleeClip);
            PreloadClip(enemySpotClip);
            PreloadClip(enemyAttackClip);
            PreloadClip(enemyDeathClip);
        }

        private void PreloadClip(AudioClip clip)
        {
            if (clip != null)
                clip.LoadAudioData();
        }

        // Uses a pool of AudioSources so sounds never interrupt each other
        private void PlaySFX(AudioClip clip)
        {
            if (clip == null || sfxSources == null || sfxSources.Length == 0) return;

            // Find a source that isn't playing, otherwise cycle
            AudioSource source = null;
            for (int i = 0; i < sfxSources.Length; i++)
            {
                if (!sfxSources[i].isPlaying)
                {
                    source = sfxSources[i];
                    break;
                }
            }

            // All busy — use round robin
            if (source == null)
            {
                source = sfxSources[sfxIndex % sfxSources.Length];
                sfxIndex++;
            }

            source.clip = clip;
            source.Play();
        }

        public void PlayBGMusic() { if (bgMusicSource != null) bgMusicSource.Play(); }
        public void StopBGMusic() { if (bgMusicSource != null) bgMusicSource.Stop(); }

        public void PlayFootstep()    => PlaySFX(footstepClip);
        public void PlayJump()        => PlaySFX(jumpClip);
        public void PlayShoot()       => PlaySFX(shootClip);
        public void PlayPlayerHurt()  => PlaySFX(playerHurtClip);
        public void PlayMelee()       => PlaySFX(meleeClip);
        public void PlayEnemySpot()   => PlaySFX(enemySpotClip);
        public void PlayEnemyAttack() => PlaySFX(enemyAttackClip);
        public void PlayEnemyDeath()  => PlaySFX(enemyDeathClip);
    }
}