using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyPiano
{
    public class AudioSample
    {
        public WaveFormat WaveFormat { get; private set; }
        public AudioBuffer AudioBuffer { get; set; }

        public uint[] PacketsInfo { get; private set; }

        public bool IsLoaded = false;
        
        public AudioSample ()
        {
        }

        public async Task<bool> Load(string fileName)
        {
            var uri = new Uri("ms-appx:///" + fileName, UriKind.Absolute);

            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);

            var randomAccessStream = await file.OpenReadAsync();
            Stream stream = randomAccessStream.AsStreamForRead();

            var soundStream = new SoundStream(stream);

            PacketsInfo = soundStream.DecodedPacketsInfo;
            WaveFormat = soundStream.Format;

            AudioBuffer = new AudioBuffer()
            {
                Stream = soundStream.ToDataStream(),
                AudioBytes = (int)soundStream.Length,
                Flags = BufferFlags.EndOfStream
            };

            return IsLoaded = true;
        }
    }
}