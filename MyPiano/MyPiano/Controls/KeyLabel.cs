using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyPiano
{
    //http://en.wikipedia.org/wiki/Note
    public class KeyLabel : UserControl
    {
        Border border;
        TextBlock textBlock;
        int octave;
        string title;

        public KeyLabel(string title, int octave)
        {
            this.octave = octave;
            this.title = title;

            border = new Border()
            {
                Height = 40,
                Width = 40,
                CornerRadius = new Windows.UI.Xaml.CornerRadius(5),
                Background = new SolidColorBrush(GetColorForOctave(octave)),
                Margin = new Windows.UI.Xaml.Thickness(4),
             };

            textBlock = new TextBlock()
            {
                Foreground = new SolidColorBrush(Colors.Black),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                Text = title,
            };

            border.Child = textBlock;

            this.Content = border;
        }

        Color GetColorForOctave (int octave)
        {
            if (octave == 0) return Color.FromArgb(255, 239, 167, 167);
            if (octave == 1) return Color.FromArgb(255, 239, 207, 167);
            if (octave == 2) return Color.FromArgb(255, 239, 239, 167);
            if (octave == 3) return Color.FromArgb(255, 167, 239, 167);
            if (octave == 4) return Color.FromArgb(255, 167, 239, 239);
            if (octave == 5) return Color.FromArgb(255, 167, 167, 239);
            if (octave == 6) return Color.FromArgb(255, 239, 167, 239);
            if (octave == 7) return Color.FromArgb(255, 239, 167, 239);

            return Color.FromArgb(255, 239, 167, 167);//wrap instead of grey
        }
    }
}