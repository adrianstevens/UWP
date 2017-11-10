using System;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyPiano
{
    public enum PianoKeyColor
    {
        White,
        Black,
    }

    public class KeyEventArgs : EventArgs
    {
        public int Octave { get; set; }
        public int Note { get; set; }
    }

    public sealed partial class PianoKey : UserControl
    {
        public EventHandler<KeyEventArgs> OnPressed { get; set; }
        public EventHandler<KeyEventArgs> OnReleased { get; set; }
        public EventHandler<KeyEventArgs> OnClicked { get; set; }

        public PianoKeyColor KeyColorType { get; private set; }

        BrushManager brushMan = new BrushManager();

        int note, octave;

        bool buttonIsDown = false;

        Color primaryColor = Colors.PowderBlue;

        string title;

        public PianoKey(PianoKeyColor keyColor = PianoKeyColor.White, int note = 0, int octave = 4)
        {
            this.InitializeComponent();

            this.KeyColorType = keyColor;
            this.title = GetKeyName(note, octave);
            this.note = note;
            this.octave = octave;

            gridMain.PointerPressed += GridMain_PointerPressed;
            gridMain.PointerEntered += GridMain_PointerPressed;
            gridMain.PointerExited += GridMain_PointerExited;
            gridMain.PointerCanceled += GridMain_PointerExited;
            gridMain.PointerCaptureLost += GridMain_PointerExited;
            gridMain.PointerReleased += GridMain_PointerReleased;
            

            SetUpState();
        }

        string GetKeyName(int note, int octave)
        {
            string[] notes = new string[] { "A", "A♯", "B", "C", "C♯", "D", "D♯", "E", "F", "F♯", "G", "G♯" };
            return notes[note] + octave;
        }

        void SetUpState()
        {
            backgroundBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));

            if (KeyColorType == PianoKeyColor.White)
                backgroundBorder.Background = brushMan.GetGradientBrush(Colors.LightGray, Colors.White, true);
            else
                backgroundBorder.Background = brushMan.GetGradientBrush(Color.FromArgb(255, 40, 40, 40), Colors.Black, true);
        }

        void SetDownState()
        {
            backgroundBorder.Background = brushMan.GetBrush(primaryColor);
            backgroundBorder.BorderBrush = brushMan.GetBrush(Colors.White);
        }

        void GridMain_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.Pointer.IsInContact == false)
                return;

            buttonIsDown = false;
            SetUpState();

            OnReleased?.Invoke(sender, new KeyEventArgs() { Note = note, Octave = octave });
        }

        void GridMain_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.Pointer.IsInContact == false || buttonIsDown == true)
                return;

            buttonIsDown = true;
            SetDownState();

            OnPressed?.Invoke(sender, new KeyEventArgs() { Note = note, Octave = octave });
        }

        void GridMain_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            buttonIsDown = false;
            SetUpState();

            OnClicked?.Invoke(sender, new KeyEventArgs() { Note = note, Octave = octave });

            OnReleased?.Invoke(sender, new KeyEventArgs() { Note = note, Octave = octave });
        }

        public void SetColor(Color color)
        {
            primaryColor = color;
        }
    }
}