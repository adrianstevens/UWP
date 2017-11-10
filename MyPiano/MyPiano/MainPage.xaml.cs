using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyPiano
{
    public sealed partial class MainPage : Page
    {
        PianoKeys pianoKeys;

        public MainPage()
        {
            InitializeComponent();

            pianoKeys = new PianoKeys();
            Grid.SetRow(pianoKeys, 1);
            gridMain.Children.Add(pianoKeys);

            btnGrandPiano.Click += BtnGrandPiano_Click;
            btnRhodes.Click += BtnRhodes_Click;
        }

        void BtnRhodes_Click(object sender, RoutedEventArgs e)
        {
            pianoKeys.Instrument = Instruments.Rhodes;
        }

        void BtnGrandPiano_Click(object sender, RoutedEventArgs e)
        {
            pianoKeys.Instrument = Instruments.GrandPiano;
        }
    }
}
