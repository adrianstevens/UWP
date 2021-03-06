﻿using System;
using Windows.UI;

public struct HslColor
{
    // value from 0 to 1 
    public double A;
    // value from 0 to 360 
    public double H;
    // value from 0 to 1 
    public double S;
    // value from 0 to 1 
    public double L;

    private static double ByteToPct(byte v)
    {
        double d = v;
        d /= 255;
        return d;
    }

    private static byte PctToByte(double pct)
    {
        pct *= 255;
        pct += .5;
        if (pct > 255) pct = 255;
        if (pct < 0) pct = 0;
        return (byte)pct;
    }

    public static HslColor FromColor(Color c)
    {
        return HslColor.FromArgb(c.A, c.R, c.G, c.B);
    }

    public static HslColor FromArgb(byte A, byte R, byte G, byte B)
    {
        HslColor c = FromRgb(R, G, B);
        c.A = ByteToPct(A);
        return c;
    }

    public static HslColor FromRgb(byte R, byte G, byte B)
    {
        HslColor c = new HslColor();
        c.A = 1;

        double r = ByteToPct(R);
        double g = ByteToPct(G);
        double b = ByteToPct(B);
        double max = Math.Max(b, Math.Max(r, g));
        double min = Math.Min(b, Math.Min(r, g));

        if (max == min)
            c.H = 0;
        else if (max == r && g >= b)
            c.H = 60 * ((g - b) / (max - min));
        else if (max == r && g < b)
            c.H = 60 * ((g - b) / (max - min)) + 360;
        else if (max == g)
            c.H = 60 * ((b - r) / (max - min)) + 120;
        else if (max == b)
            c.H = 60 * ((r - g) / (max - min)) + 240;

        c.L = .5 * (max + min);

        if (max == min)
            c.S = 0;
        else if (c.L <= .5)
            c.S = (max - min) / (2 * c.L);
        else if (c.L > .5)
            c.S = (max - min) / (2 - 2 * c.L);

        return c;
    }

    public static HslColor FromHsl(double H, double S, double L)
    {
        HslColor c = new HslColor();
        c.A = 1;
        c.H = H;
        c.S = S;
        c.L = L;

        return c;
    }

    public static HslColor FromHsv (double H, double S, double V)
    {
        var rgb = ColorFromHSV(H, S, V);
        return FromColor(rgb);
    }

    public HslColor Lighten(double pct)
    {
        HslColor c = new HslColor();
        c.A = this.A;
        c.H = this.H;
        c.S = this.S;
        c.L = Math.Min(Math.Max(this.L + pct, 0), 1);
        return c;
    }

    public HslColor Darken(double pct)
    {
        return Lighten(-pct);
    }

    private double norm(double d)
    {
        if (d < 0) d += 1;
        if (d > 1) d -= 1;
        return d;
    }

    private double getComponent(double tc, double p, double q)
    {
        if (tc < (1.0 / 6.0))
        {
            return p + ((q - p) * 6 * tc);
        }
        if (tc < .5)
        {
            return q;
        }
        if (tc < (2.0 / 3.0))
        {
            return p + ((q - p) * 6 * ((2.0 / 3.0) - tc));
        }
        return p;
    }

    public Color ToColor()
    {
        double q = 0;
        if (L < .5)
        {
            q = L * (1 + S);
        }
        else
        {
            q = L + S - (L * S);
        }
        double p = (2 * L) - q;
        double hk = H / 360;
        double r = getComponent(norm(hk + (1.0 / 3.0)), p, q);
        double g = getComponent(norm(hk), p, q);
        double b = getComponent(norm(hk - (1.0 / 3.0)), p, q);
        return Color.FromArgb(PctToByte(A), PctToByte(r), PctToByte(g), PctToByte(b));
    }

        
    public static Color ColorFromHSV(double hue, double saturation, double value)
    {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);

        value = value * 255;
        int v = Convert.ToInt32(value);
        int p = Convert.ToInt32(value * (1 - saturation));
        int q = Convert.ToInt32(value * (1 - f * saturation));
        int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

        if (hi == 0)
            return Color.FromArgb(255, (byte)v, (byte)t, (byte)p);
        else if (hi == 1)
            return Color.FromArgb(255, (byte)q, (byte)v, (byte)p);
        else if (hi == 2)
            return Color.FromArgb(255, (byte)p, (byte)v, (byte)t);
        else if (hi == 3)
            return Color.FromArgb(255, (byte)p, (byte)q, (byte)v);
        else if (hi == 4)
            return Color.FromArgb(255, (byte)t, (byte)p, (byte)v);
        else
            return Color.FromArgb(255, (byte)v, (byte)p, (byte)q);
    }
}