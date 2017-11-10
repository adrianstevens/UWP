using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


// http://sharpdx.org/forum/5-api-usage/3324-how-to-correctly-clean-up-an-xaudio2-sourcevoice 
// http://sharpdx.org/forum/7-documentation/39-audio-samples
public class SoundEffect : IDisposable
{
    private XAudio2 audioDevice;
    private AudioBuffer audioBuffer;
    private SoundStream soundStream;
    private WaveFormat waveFormat;
 
    private Pool<SoundEffectInstance> instances;
 
    public SoundEffect(XAudio2 audioDevice)
    {
        this.audioDevice = audioDevice;
        this.instances = new Pool<SoundEffectInstance>(CreatePoolInstance);
    }
 
    SoundEffectInstance CreatePoolInstance()
    {
        SoundEffectInstance instance = new SoundEffectInstance(this);
        instance.StreamEnd += InstanceStreamEnd;
        return instance;
    }
 
    void InstanceStreamEnd(object sender, EventArgs e)
    {
        // Return the instance to the pool
        instances.FlagFreeItem((SoundEffectInstance)sender);
    }
 
    public void Dispose()
    {
        //instances.Dispose();
        //Edited by Adrian
        instances.Clear();

        this.audioBuffer.Stream.Dispose();
    }
 
    public async Task<bool> Load(string filename)
    {
        var uri = new Uri("ms-appx:///" + filename, UriKind.Absolute);

        var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);

        var randomAccessStream = await file.OpenReadAsync();
        Stream stream = randomAccessStream.AsStreamForRead();

        soundStream = new SoundStream(stream);

        waveFormat = soundStream.Format;

        audioBuffer = new AudioBuffer()
        {
            Stream = soundStream.ToDataStream(),
            AudioBytes = (int)soundStream.Length,
            Flags = BufferFlags.EndOfStream
        };

        return true;
    }
 
    internal AudioBuffer AudioBuffer
    {
        get { return this.audioBuffer; }
    }
 
    internal SoundStream SoundStream
    {
        get { return this.soundStream; }
    }
 
    internal XAudio2 Device
    {
        get { return this.audioDevice; }
    }
 
    internal WaveFormat WaveFormat
    {
        get { return this.waveFormat; }
    }

    SoundEffectInstance instance;
    public void Play(float volume)
    {
        Stop();
        instance = instances.GetFreeItem();
        instance.Volume = volume;
        instance.Play();
    }

    public void Stop ()
    {
        if (instance != null)
        {
            instance.Stop();
        }
    }

    public float Volume
    {
        get { if(instance == null) return 0f; return instance.Volume; }
        set { instance.Volume = value; }
    }
}

public class SoundEffectInstance
 {
    public event EventHandler StreamEnd;

    SoundEffect soundEffect;
    SourceVoice sourceVoice;

    public float Volume
    {
        get { return sourceVoice.Volume; }
        set { sourceVoice.SetVolume(value); }
    }

    internal SoundEffectInstance(SoundEffect soundEffect)
    {
        this.soundEffect = soundEffect;
        sourceVoice = new SourceVoice(this.soundEffect.Device, this.soundEffect.WaveFormat, true);
        sourceVoice.StreamEnd += SourceVoice_StreamEnd;
    }
 
     void SourceVoice_StreamEnd()
     {
        StreamEnd?.Invoke(this, EventArgs.Empty);
     }
 
     public void Play()
     {
         sourceVoice.SubmitSourceBuffer(this.soundEffect.AudioBuffer, this.soundEffect.SoundStream.DecodedPacketsInfo);
         sourceVoice.Start();
     }

     public void Stop ()
     {
         sourceVoice.Stop();
     }
 }

public class Pool<T>
{
    readonly List<T> items = new List<T>();
    readonly Queue<T> freeItems = new Queue<T>();

    readonly Func<T> createItemAction;

    public Pool(Func<T> createItemAction)
    {
        this.createItemAction = createItemAction;
    }

    public void FlagFreeItem(T item)
    {
        freeItems.Enqueue(item);
    }

    public T GetFreeItem()
    {
        if (freeItems.Count == 0)
        {
            T item = createItemAction();
            items.Add(item);

            return item;
        }

        return freeItems.Dequeue();
    }

    public List<T> Items
    {
        get { return items; }
    }

    public void Clear()
    {
        items.Clear();
        freeItems.Clear();
    }
}