using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyPiano
{
    public sealed partial class PianoKeys : UserControl
    {
        public Instruments Instrument
        {
            get { return sampleManager.Instrument; }
            set { sampleManager.Instrument = value; }
        }

        List<PianoKey> pianoKeys = new List<PianoKey>();

        static double BLACK_KEY_HEIGHT = 119;
        static double BLACK_KEY_WIDTH = 32;
        static double WHITE_KEY_HEIGHT = 260;
        static double WHITE_KEY_WIDTH = 54;

        public Action KeyPressed;

        Color colorScheme = Colors.SkyBlue;

        SampleManager sampleManager = new SampleManager();

        public PianoKeys()
        {
            Height = WHITE_KEY_HEIGHT + 16;

            this.InitializeComponent();

            AlignControls();
            AddKeys();
        }

        public void SetColor (Color color)
        {
            colorScheme = color;
            HslColor hsl = HslColor.FromColor(color);
            hsl.L += 0.1;

            Color keyColor = hsl.ToColor();
            
            mainGrid.Background = new SolidColorBrush(color);

            for(int i = 0; i < pianoKeys.Count; i++)
            {
                pianoKeys[i].SetColor(keyColor);
            }
        }

         void MyScroll_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
         {
            var hslColor = HslColor.FromArgb(255, 239, 167, 167);

            double halfOctave = 7*WHITE_KEY_WIDTH / 2;
            double sixOct = 14 * halfOctave;

            var hue = 360 * (myScroll.HorizontalOffset + halfOctave) / sixOct;
            hslColor.H = hue;

            mainGrid.Background = new SolidColorBrush(hslColor.ToColor());
        }

        void AlignControls ()
        {
           myScroll.Height = myStack.Height = this.Height = WHITE_KEY_HEIGHT + 16;
        }

        private void AddKeys ()
        {
            pianoKeys.Clear();

            double width = 0;
            //lower 3 keys A, A#, & B
            Canvas keyCanvas = new Canvas();
            keyCanvas.Width = WHITE_KEY_WIDTH * 2;
            keyCanvas.Height = WHITE_KEY_HEIGHT;
           
            //A 0
            var key = CreateWhiteKey(0, 0);
            pianoKeys.Add(key);
            keyCanvas.Children.Add(key);

            //B 0
            key = CreateWhiteKey(2, 0);
            key.Margin = new Thickness(WHITE_KEY_WIDTH, 0, 0, 0);
            pianoKeys.Add(key);
            keyCanvas.Children.Add(key);

            //A# 0
            key = CreateBlackKey(1, 0);
            key.Margin = new Thickness(WHITE_KEY_WIDTH * 2 - BLACK_KEY_WIDTH * 2, 0, 0, 0);
            pianoKeys.Add(key);
            keyCanvas.Children.Add(key);

            myStack.Children.Add(keyCanvas);
            width += keyCanvas.Width;

            //6 full octaves
            for (int i = 0; i < 7; i++)
            {
                var oc = GetOctave((i+1));
                
                myStack.Children.Add(oc);
                width += oc.Width;
            }

            //final C
            key = CreateWhiteKey(3, 8);


            keyCanvas = new Canvas
            {
                Width = WHITE_KEY_WIDTH,
                Height = WHITE_KEY_HEIGHT
            };

            keyCanvas.Children.Add(key);
            pianoKeys.Add(key);
            width += key.Width;
            myStack.Children.Add(keyCanvas);

            myStack.Width = width;
        }

        PianoKey CreateWhiteKey (int note, int octave)
        {
            var whiteKey = new PianoKey(PianoKeyColor.White, note, octave)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,

                Width = WHITE_KEY_WIDTH,
                Height = WHITE_KEY_HEIGHT
            };

            whiteKey.OnPressed += PianoKeyPressed;
            whiteKey.OnReleased += PianoKeyReleased;

            return whiteKey;
        }

        PianoKey CreateBlackKey (int note, int octave)
        {
            var blackKey = new PianoKey(PianoKeyColor.Black, note, octave)
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = BLACK_KEY_WIDTH,
                Height = BLACK_KEY_HEIGHT
            };

            blackKey.OnPressed += PianoKeyPressed;
            blackKey.OnReleased += PianoKeyReleased;

            return blackKey;
        }

        Canvas GetOctave (int octave)
        {
            var octaveCanvas = new Canvas();

            //width of 7 white keys starting (C to B)
            octaveCanvas.Height = WHITE_KEY_HEIGHT;
            octaveCanvas.Width = 7 * WHITE_KEY_WIDTH;

            //marking the black notes
            var notes = new string[] { "", "Cs", "", "Ds", "", "", "Fs", "", "Gs", "", "As", "" };
            int count = 0;

            for (int i = 0; i < 12; i++)//12 so we can correlate note value 
            {
                if (notes[i].Length > 0)
                    continue;

                int note = (i + 3) % 12;//since the octave starts on C
                var whiteKey = CreateWhiteKey(note, octave);
                whiteKey.Margin = new Thickness(count * WHITE_KEY_WIDTH, 0, 0, 0);
            
                octaveCanvas.Children.Add(whiteKey);
                pianoKeys.Add(whiteKey);

                count++;
            }

            //align the black keys 
            for (int i = 0; i < 12; i++)
            {
                if (notes[i].Length > 0)
                {
                    int note = (i + 3) % 12;

                    var blackKey = CreateBlackKey(note, octave);
                    blackKey.Margin = new Thickness(i * BLACK_KEY_WIDTH, 0, 0, 0);
                
                    octaveCanvas.Children.Add(blackKey);
                    pianoKeys.Add(blackKey);
                }
            } 
             
            return octaveCanvas;
        }

        async void PianoKeyPressed(object MySender, KeyEventArgs eArg)
        {
            KeyPressed?.Invoke();

            await sampleManager.PlayNote(eArg.Note, eArg.Octave);
        }

        void PianoKeyReleased(object MySender, KeyEventArgs eArg)
        {
            sampleManager.StopNote(eArg.Note, eArg.Octave);
        }
    }
}