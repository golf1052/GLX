using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GLX
{
    public class Sound
    {
        public SoundEffect soundEffect;
        public List<SoundEffectInstance> sounds;
        public SoundEffectInstance instance;

        public Sound(SoundEffect loadedSound)
        {
            soundEffect = loadedSound;
            sounds = new List<SoundEffectInstance>();
            instance = soundEffect.CreateInstance();
        }

        public void Play()
        {
            SoundEffectInstance instance = soundEffect.CreateInstance();
            sounds.Add(instance);
            instance.Play();
            RemoveDeadSounds();
        }

        public void PlayIn3D(AudioListener listener, AudioEmitter emitter)
        {
            SoundEffectInstance instance = soundEffect.CreateInstance();
            instance.Apply3D(listener, emitter);
            sounds.Add(instance);
            instance.Play();
            RemoveDeadSounds();
        }

        private void RemoveDeadSounds()
        {
            for (int i = 0; i < sounds.Count; i++)
            {
                if (sounds[i].State == SoundState.Stopped)
                {
                    sounds.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
