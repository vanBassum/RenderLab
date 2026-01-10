namespace Engine2D.Rendering.Graphics
{
    public static class ColorGenerator
    {
        // Golden ratio conjugate ˜ 0.618... used to spread hues evenly.
        private const double PhiConjugate = 0.6180339887498948482;

        public static ColorRgba FromIndex(long index, byte a = 255)
        {
            // Hue in [0,1). Using fractional part of index * phi spreads colors nicely.
            double h = Frac(index * PhiConjugate);

            // To avoid "too many similar brightness" colors, vary S and V in a small pattern.
            // You can tweak these to your taste.
            int band = (int)(index % 6);
            double s = band switch
            {
                0 => 0.80,
                1 => 0.95,
                2 => 0.70,
                3 => 0.90,
                4 => 0.75,
                _ => 0.85
            };

            double v = band switch
            {
                0 => 0.95,
                1 => 0.85,
                2 => 0.90,
                3 => 0.75,
                4 => 0.80,
                _ => 0.92
            };

            return HsvToRgba(h, s, v, a);
        }

        private static double Frac(double x)
            => x - Math.Floor(x);

        // h, s, v in [0,1]
        private static ColorRgba HsvToRgba(double h, double s, double v, byte a)
        {
            double r, g, b;

            if (s <= 0.0)
            {
                r = g = b = v; // grayscale
            }
            else
            {
                double hh = (h % 1.0) * 6.0;
                int i = (int)Math.Floor(hh);
                double f = hh - i;

                double p = v * (1.0 - s);
                double q = v * (1.0 - s * f);
                double t = v * (1.0 - s * (1.0 - f));

                switch (i)
                {
                    case 0: r = v; g = t; b = p; break;
                    case 1: r = q; g = v; b = p; break;
                    case 2: r = p; g = v; b = t; break;
                    case 3: r = p; g = q; b = v; break;
                    case 4: r = t; g = p; b = v; break;
                    default: r = v; g = p; b = q; break; // case 5
                }
            }

            return new ColorRgba(
                (byte)Math.Clamp((int)Math.Round(r * 255.0), 0, 255),
                (byte)Math.Clamp((int)Math.Round(g * 255.0), 0, 255),
                (byte)Math.Clamp((int)Math.Round(b * 255.0), 0, 255),
                a
            );
        }
    }

}
