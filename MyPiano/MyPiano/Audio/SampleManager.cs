using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MyPiano
{
    public enum Instruments
    {
        BrightPiano,
        GrandPiano,
        ElectricPiano,
        Rhodes,
        count
    }
    
    public class SampleManager
    {
        public Instruments Instrument { get; set; } = Instruments.BrightPiano;

        XAudio2 audio;
        MasteringVoice masteringVoice;

        Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();
        List<SoundEffect> soundEffectsToRelease = new List<SoundEffect>();

        DispatcherTimer timer = null;

        string[] notes = new string[] { "A", "As", "B", "C", "Cs", "D", "Ds", "E", "F", "Fs", "G", "Gs" };

        public SampleManager ()
        {
            audio = new XAudio2();
            masteringVoice = new MasteringVoice(audio);
        }

        public async Task<SoundEffect> PlayNote (int note, int octave)
        {
            string filePath = GetFullPath(note, octave);
                        
            return await GetSample(filePath);
        }

        async Task<SoundEffect> GetSample (string filePath)
        {
            if (soundEffects.ContainsKey(filePath))
            {
                soundEffects.Remove(filePath);
            }

            var soundEffect = new SoundEffect(audio);
            await soundEffect.Load(filePath);

            soundEffects.Add(filePath, soundEffect);

            soundEffect.Play(0.6f);

            return soundEffect;
        }

        string GetSoundFilePath(Instruments instrument)
        {
            string res = @"Assets\Instruments\"; //Rhodes\" + notes[note] + octave.ToString() + ".wav";

            switch (instrument)
            {
                case Instruments.BrightPiano:
                    res += @"BrightPiano\";
                    break;
                case Instruments.ElectricPiano:
                    res += @"ElectricPiano\";
                    break;
                case Instruments.Rhodes:
                    res += @"Rhodes\";
                    break;
                case Instruments.GrandPiano:
                    res += @"GrandPiano\";
                    break;

            }
            return res;
        }

        string GetFullPath(int note, int octave)
        {
            string filePath = GetSoundFilePath(Instrument) + notes[note] + octave.ToString() + ".wav";
            return filePath;
        }

        public void StopNote(int note, int octave)
        {
            //string res = @"Assets\Instruments\Rhodes\" + notes[note] + octave.ToString() + ".wav";
            string res = GetFullPath(note, octave);

            if (soundEffects.ContainsKey(res) == false)
                return;

            SoundEffect sfx = null;

            if (soundEffects.TryGetValue(res, out sfx) == true)
            {
                soundEffects.Remove(res);
                soundEffectsToRelease.Add(sfx);

                if (timer == null)
                {
                    timer = new DispatcherTimer();
                    timer.Tick += timer_Tick;
                    timer.Interval = new TimeSpan(0, 0, 0, 0, 25);
                    timer.Start();
                }
            }
        }

        void timer_Tick(object sender, object e)
        {
            for (int i = soundEffectsToRelease.Count - 1; i > -1; i--)
            {
                if (soundEffectsToRelease[i] == null || soundEffectsToRelease[i].Volume == 0f)
                {
                    soundEffectsToRelease.RemoveAt(i);
                }
                else
                {
                    soundEffectsToRelease[i].Volume = (float)Math.Max(0, soundEffectsToRelease[i].Volume - 0.07f);
                }
            }

            if (soundEffectsToRelease.Count == 0)
            {
                timer.Stop();
                timer = null;
            }
        }
    }
}