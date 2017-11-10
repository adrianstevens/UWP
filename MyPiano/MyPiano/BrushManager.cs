using System.Collections.Generic;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.Foundation;

namespace MyPiano
{
    public class GradItem
    {
        public Color cr { get; set; }
        public double dbOffset { get; set; }
    }
    public class BrushManager
    {
        Dictionary<string, SolidColorBrush> arrBrushes;
        Dictionary<string, LinearGradientBrush> arrGradBrushes;

        public BrushManager()
        {
            arrBrushes = new Dictionary<string, SolidColorBrush>();
            arrGradBrushes = new Dictionary<string, LinearGradientBrush>();
        }

        public void DeleteBrushes()
        {
            arrBrushes.Clear();
            arrGradBrushes.Clear();
        }

        public SolidColorBrush GetBrush(Color cr)
        {
            SolidColorBrush br = null;

            if (arrBrushes.TryGetValue(cr.ToString(), out br))
                return br;

            br = new SolidColorBrush();
            br.Color = cr;

            arrBrushes.Add(cr.ToString(), br);

            return br;
        }

        public SolidColorBrush GetBrush(Color cr, byte alpha)
        {
            Color temp = cr;
            temp.A = alpha;

            return GetBrush(temp);
        }

        public SolidColorBrush GetBrush(uint uiColor)
        {
            return GetBrush( UintToColor( uiColor ));
        }

        public LinearGradientBrush GetGradientBrush(Color cr1, Color cr2, bool bHorz = false)
        {
            LinearGradientBrush br = null;

            if (arrGradBrushes.TryGetValue(cr1.ToString() + cr2.ToString(), out br))
                return br;

            br = new LinearGradientBrush();

            
            br.StartPoint = new Point(0, 0);
            if (bHorz)
                br.EndPoint = new Point(0, 1);
            else
                br.EndPoint = new Point(1, 0);

            GradientStop stop = new GradientStop();
            stop.Offset = 0;
            stop.Color = cr1;
            br.GradientStops.Add(stop);

            stop = new GradientStop();
            stop.Offset = 1;
            stop.Color = cr2;
            br.GradientStops.Add(stop);

            arrGradBrushes.Add(cr1.ToString() + cr2.ToString(), br);

            return br;
        }
        
        public LinearGradientBrush GetGradientBrush(uint uiColor1, uint uiColor2)
        {
            return GetGradientBrush(UintToColor(uiColor1), UintToColor(uiColor2));
        }

        public LinearGradientBrush GetGradientBrush(List<GradItem> items)
        {
            if (items.Count == 0)
                return null;

            string key = "";

            foreach (GradItem i in items)
            {
                key += i.cr.ToString();
            }

            if (arrGradBrushes.TryGetValue(key, out LinearGradientBrush br))
                return br;

            br = new LinearGradientBrush();

            br.StartPoint = new Point(0.5, 0);
            br.EndPoint = new Point(0.5, 1);

            GradientStop stop;
            foreach (GradItem i in items)
            {
                stop = new GradientStop();
                stop.Offset = i.dbOffset;
                stop.Color = i.cr;
                br.GradientStops.Add(stop);
            }

            arrGradBrushes.Add(key, br);

            return br;
        }

        public static Color UintToColor(uint argb)
        {
            return Color.FromArgb((byte)((argb & -16777216) >> 0x18), 
                                (byte)((argb & 0xff0000) >> 0x10), 
                                (byte)((argb & 0xff00) >> 8), 
                                (byte)(argb & 0xff));
        }
    }
}