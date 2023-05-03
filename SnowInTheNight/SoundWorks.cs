using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Vorbis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace SnowInTheNight
{
    static class SoundWorks
    {
        static CachedSound snow1;
        static CachedSound snow1end;
        static CachedSound snow2;
        static CachedSound footsteps1;
        static CachedSound footsteps2;
        static CachedSound footsteps3;
        static CachedSound footsteps4;
        static CachedSound lantern1;
        static CachedSound lantern2;
        static CachedSound lantern3;
        static CachedSound fire;
        static CachedSound knock;
        static CachedSound coal;

        static CachedSound[,] bells;
        static SoundWorks()
        {
            snow1 = new CachedSound(SnowInTheNight.Properties.Resources.hurricane);
            snow1end = new CachedSound(SnowInTheNight.Properties.Resources.hurricaneend);
            snow2 = new CachedSound(SnowInTheNight.Properties.Resources.windhowl, 0.2f);
            footsteps1 = new CachedSound(SnowInTheNight.Properties.Resources.footsteps1,0.2f);
            footsteps2 = new CachedSound(SnowInTheNight.Properties.Resources.footsteps2, 0.2f);
            footsteps3 = new CachedSound(SnowInTheNight.Properties.Resources.footsteps3, 0.2f);
            footsteps4 = new CachedSound(SnowInTheNight.Properties.Resources.footsteps4, 0.2f);

            lantern1 = new CachedSound(SnowInTheNight.Properties.Resources.lantern_on, 0.5f);
            lantern2 = new CachedSound(SnowInTheNight.Properties.Resources.lantern_off, 0.2f);
            lantern3 = new CachedSound(SnowInTheNight.Properties.Resources.lantern_off2, 5f);
            fire = new CachedSound(SnowInTheNight.Properties.Resources.fireplace, 0.05f);
            knock = new CachedSound(SnowInTheNight.Properties.Resources.knock, 1f);
            coal = new CachedSound(SnowInTheNight.Properties.Resources.coal, 0.8f);

            bells = new CachedSound[4,4];
            var resourceBell = new byte[][]{
                SnowInTheNight.Properties.Resources.bell1,
                SnowInTheNight.Properties.Resources.bell2,
                SnowInTheNight.Properties.Resources.bell5,
                SnowInTheNight.Properties.Resources.bell6
            };

            var volume = new float[] {0.05f, 0.1f, 0.2f, 0.5f};
            for (int i = 0; i < bells.GetLength(0); i++)
            {
                for (int j = 0; j < bells.GetLength(1); j++)
                {
                    bells[i, j] = new CachedSound(resourceBell[i], volume[j]);
                }
            }
        }
        static Random R = new Random();

        static double timer1 = 0;
        static double timer2 = 0;
        static double timer3 = 0;
        public static bool NearFire = false;
        public static bool HighWindOn = false;
        private static bool HighWindIsFading = true;
        private static ISampleProvider provider1 = null;
        private static ISampleProvider provider2 = null;

        public static void PlaySomeSnowSound (double deltaTime)
        {
            timer1 -= deltaTime;
            timer2 -= deltaTime;
            timer3 -= deltaTime;
            stepTimer -= deltaTime;
            if (timer1 <= 0)
            {
                if (!NearFire)
                {
                    timer1 = 3 * (0.5 + R.NextDouble());
                    AudioPlaybackEngine.Instance.PlaySound(snow2);
                }
            }

            if (timer3 <= 0)
            {
                if (NearFire)
                {
                    timer3 = 2.7;
                    AudioPlaybackEngine.Instance.PlaySound(fire);
                }
            }
            NearFire = false;

            if(HighWindOn)
            {
                HighWindIsFading = false;
                if (timer2 <= 0)
                {
                    timer2 = 3 * (0.5 + R.NextDouble());
                    provider2 = provider1;
                    provider1 = AudioPlaybackEngine.Instance.PlaySound(snow1);
                }
            }
            if(!HighWindOn)
            {
                if (!HighWindIsFading)
                {
                    HighWindIsFading = true;
                    AudioPlaybackEngine.Instance.RemoveMixerInput(provider1);
                    AudioPlaybackEngine.Instance.RemoveMixerInput(provider2);
                    AudioPlaybackEngine.Instance.PlaySound(snow1end);
                }
            }
        }
        static double stepTimer = 0;
        public static void PlaySomeFootstep()
        {
            if (stepTimer > 0)
                return;
            stepTimer = 0.3;
            var r = R.NextDouble();
            if (r > 0.9)
            {
                AudioPlaybackEngine.Instance.PlaySound(footsteps1);
                //Console.WriteLine("1");
            }
            else if (r > 0.5)
            {
                AudioPlaybackEngine.Instance.PlaySound(footsteps2);
                //Console.WriteLine("2");
            }
            else if (r > 0.4)
            {
                AudioPlaybackEngine.Instance.PlaySound(footsteps3);
                //Console.WriteLine("3");
            }
            else
            {
                AudioPlaybackEngine.Instance.PlaySound(footsteps4);
                //Console.WriteLine("4");
            }
        }

        public static void PlayLanternOn()
        {
            AudioPlaybackEngine.Instance.PlaySound(lantern1);
        }
        public static void PlayLanternOff()
        {
            AudioPlaybackEngine.Instance.PlaySound(lantern2);
        }
        public static void PlayLanternOffByWind()
        {
            AudioPlaybackEngine.Instance.PlaySound(lantern3);
        }
        public static void PlayBellSound(int intencity)
        {
            if (intencity <= 0)
                return;
            intencity--;
            var b = R.Next(0, bells.GetLength(0));

            var maxIntencity = bells.GetLength(1) - 1;
            if (intencity > maxIntencity)
                intencity = maxIntencity;

            AudioPlaybackEngine.Instance.PlaySound(bells[b,intencity]);
        }
        public static void PlayLoadSound()
        {
            var intencity = 2;
            var b = 3;
            AudioPlaybackEngine.Instance.PlaySound(bells[b, intencity]);
        }

        internal static void Close()
        {
            AudioPlaybackEngine.Instance.Dispose();
        }

        internal static void PlaySomeKnocking()
        {
            AudioPlaybackEngine.Instance.PlaySound(knock);
        }
        internal static void PlayCoalThrowing()
        {
            AudioPlaybackEngine.Instance.PlaySound(coal);
        }
    }
    interface IPlaybackEngine : IDisposable
    {
        void RemoveMixerInput(ISampleProvider provider);

        ISampleProvider PlaySound(CachedSound sound);
    }

    class NoAudioPlaybackEngine : IPlaybackEngine
    {
        public ISampleProvider PlaySound(CachedSound sound)
        {
            return null;
        }
        public void RemoveMixerInput(ISampleProvider provider)
        {
        }
        public void Dispose()
        {
        }
    }
    class AudioPlaybackEngine : IPlaybackEngine
    {
        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;
        public static readonly IPlaybackEngine Instance;
        static AudioPlaybackEngine ()
        {
            try
            {
                Instance = new AudioPlaybackEngine(44100, 2);
            }
            catch(NAudio.MmException e)
            {
                Program.ErrorLog(e);
                Instance = new NoAudioPlaybackEngine();
            }
        }

        public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
        {
            outputDevice = new WaveOutEvent();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
            mixer.ReadFully = true;
            outputDevice.Init(mixer);
            outputDevice.Play();
        }

        public void RemoveMixerInput(ISampleProvider provider)
        {
            mixer.RemoveMixerInput(provider);
        }

        public ISampleProvider PlaySound(CachedSound sound)
        {
            return AddMixerInput(new CachedSoundSampleProvider(sound));
        }

        
#if DEBUG
        private ISampleProvider AddMixerInput(ISampleProvider input)
        {
            var result = ConvertToRightChannelCount(input);
            mixer.AddMixerInput(result);
            return input;
        }
#else
        private ISampleProvider AddMixerInput(ISampleProvider input)
        {
            mixer.AddMixerInput(input);
            return input;
        }
#endif
         
#if DEBUG
        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
        {
            if (input.WaveFormat.Channels == mixer.WaveFormat.Channels)
            {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2)
            {
                Console.WriteLine("audiofile has incorrect number of channels");
                return new MonoToStereoSampleProvider(input);
            }
            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }
#endif
        public void Dispose()
        {
            outputDevice.Dispose();
        }
    }

    class CachedSound
    {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }
        public CachedSound (byte[] source, float initialVolumeMultiplier = 1)
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(source, 0, source.Length);
                ms.Seek(0, SeekOrigin.Begin);
                using (var waveFileReader = new VorbisWaveReader(ms))
                {
                    WaveFormat = waveFileReader.WaveFormat;
                    var sampleProvider = waveFileReader.ToSampleProvider();
                    List<float> wholeFile = new List<float>((int)(waveFileReader.Length / 4));
                    var readBuffer = new float[waveFileReader.WaveFormat.SampleRate * waveFileReader.WaveFormat.Channels];
                    int samplesRead;
                    while ((samplesRead = sampleProvider.Read(readBuffer, 0, readBuffer.Length)) > 0)
                    {
                        wholeFile.AddRange(readBuffer.Take(samplesRead));
                    }
                    if(initialVolumeMultiplier != 1)
                        AudioData = wholeFile.Select(f => f * initialVolumeMultiplier).ToArray();
                    else
                        AudioData = wholeFile.ToArray();
                }
            }
        }
        public CachedSound(string audioFileName)
        {
            using (var audioFileReader = new AudioFileReader(audioFileName))
            {
                // TODO: could add resampling in here if required
                WaveFormat = audioFileReader.WaveFormat;
                var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
                var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
                int samplesRead;
                while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
                {
                    wholeFile.AddRange(readBuffer.Take(samplesRead));
                }
                AudioData = wholeFile.ToArray();
            }
        }
    }

    class CachedSoundSampleProvider : ISampleProvider
    {
        private readonly CachedSound cachedSound;
        private long position;

        public CachedSoundSampleProvider(CachedSound cachedSound)
        {
            this.cachedSound = cachedSound;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            var availableSamples = cachedSound.AudioData.Length - position;
            var samplesToCopy = Math.Min(availableSamples, count);
            Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
            position += samplesToCopy;
            return (int)samplesToCopy;
        }

        public WaveFormat WaveFormat { get { return cachedSound.WaveFormat; } }
    }

    class AutoDisposeFileReader : ISampleProvider
    {
        private readonly AudioFileReader reader;
        private bool isDisposed;
        public AutoDisposeFileReader(AudioFileReader reader)
        {
            this.reader = reader;
            this.WaveFormat = reader.WaveFormat;
        }
        public int Read(float[] buffer, int offset, int count)
        {
            if (isDisposed)
                return 0;
            int read = reader.Read(buffer, offset, count);
            if (read == 0)
            {
                reader.Dispose();
                isDisposed = true;
            }
            return read;
        }

        public WaveFormat WaveFormat { get; private set; }
    }
}
