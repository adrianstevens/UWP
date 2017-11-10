using System;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyPiano
{
    class PianoKeyCode : UserControl
    {
        public EventHandler<KeyEventArgs> OnPressed { get; set; }
        public EventHandler<KeyEventArgs> OnReleased { get; set; }
        public EventHandler<KeyEventArgs> OnClicked { get; set; }

        Grid layoutRoot;
        Border backgroundBorder;

        BrushManager brushMan = new BrushManager();

        KeyLabel label;

        public PianoKeyColor KeyColorType { get; private set; }

        int note, octave;

        bool buttonIsDown = false;

        Color primaryColor = Colors.PowderBlue;

        string title, titleAlt;

        public PianoKeyCode (PianoKeyColor eKey = PianoKeyColor.White, int note = 0, int octave = 4)
        {
            this.KeyColorType = eKey;
            this.title = GetLabel(note, octave);
            this.titleAlt = GetLabel(note, octave, true);
            this.note = note;
            this.octave = octave;

            CreateControls();

            DrawUpState();
        }

        public PianoKeyCode(PianoKeyColor eKey = PianoKeyColor.White, string title = "", string titleAlt = "")
        {
            this.KeyColorType = eKey;
            this.title = title;
            this.titleAlt = titleAlt;

            CreateControls();

            DrawUpState();
        }
        
        string GetLabel (int note, int octave, bool alternateLabel = false)
        {
            string[] notes = new string[] { "A", "A♯", "B", "C", "C♯", "D", "D♯", "E", "F", "F♯", "G", "G♯" };

            string[] notesAlt = new string[] 
		    {
			    ("La"), ("La♯"), ("Si"), ("Do"), ("Do♯"), ("Re"),
			    ("Re♯"), ("Mi"), ("Fa"), ("Fa♯"), ("Sol"), ("Sol♯")
		    };

            if (alternateLabel == true)
                return notesAlt[note] + octave;
            else
                return notes[note] + octave;
        }

        void CreateControls ()
        {
            layoutRoot = new Grid()
            {
                Background = new SolidColorBrush(Colors.Transparent)
            };
            
            layoutRoot.PointerEntered += LayoutRoot_PointerPressed;
            layoutRoot.PointerExited += LayoutRoot_PointerExited;
            layoutRoot.PointerPressed +=LayoutRoot_PointerPressed;
            layoutRoot.PointerReleased += LayoutRoot_PointerReleased;
            layoutRoot.PointerCanceled += LayoutRoot_PointerExited;
            layoutRoot.PointerCaptureLost += LayoutRoot_PointerExited;

            backgroundBorder = new Border()
            {
                BorderThickness = new Windows.UI.Xaml.Thickness(1),
                IsHitTestVisible = false,
                CornerRadius = new Windows.UI.Xaml.CornerRadius(0, 0, 4, 4),
            };

            layoutRoot.Children.Add(backgroundBorder);

            if (this.KeyColorType == PianoKeyColor.White)
            {
                label = new KeyLabel(title, octave)
                {
                    VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom
                };

                layoutRoot.Children.Add(label);
            }

            this.Content = layoutRoot;
        }

        void DrawUpState ()
        {
            backgroundBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));

            if (KeyColorType == PianoKeyColor.White)
                backgroundBorder.Background = brushMan.GetGradientBrush(Colors.LightGray, Colors.White, true);// Globals.brushMan.GetBrush(Colors.White);
            else
                backgroundBorder.Background = brushMan.GetGradientBrush(Color.FromArgb(255, 40, 40, 40), Colors.Black, true);
        }

        void DrawDownState ()
        {
            backgroundBorder.Background = brushMan.GetBrush(primaryColor);       
            backgroundBorder.BorderBrush = brushMan.GetBrush(Colors.White);
        }

        void LayoutRoot_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.Pointer.IsInContact == false)
                return;

            buttonIsDown = false;
            DrawUpState();

            OnReleased?.Invoke(sender, new KeyEventArgs() { Note = note, Octave = octave });
        }

        void LayoutRoot_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.Pointer.IsInContact == false || buttonIsDown == true)
                return;

            buttonIsDown = true;
            DrawDownState();

            OnPressed?.Invoke(sender, new KeyEventArgs() { Note = note, Octave = octave });
        }

        void LayoutRoot_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            buttonIsDown = false;
            DrawUpState();

            OnClicked?.Invoke(sender, new KeyEventArgs() { Note = note, Octave = octave });

            OnReleased?.Invoke(sender, new KeyEventArgs() { Note = note, Octave = octave });
        }

        public void SetColor(Color color)
        {
            primaryColor = color;
        }
    }
}