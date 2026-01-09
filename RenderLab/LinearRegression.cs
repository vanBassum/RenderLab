using System.Numerics;

namespace RenderLab
{
    public sealed class LinearRegression
    {
        private readonly List<Anchor> _points = new();

        // map = b + A * game
        // A = [a11 a12; a21 a22], b = (b1,b2)
        private bool _hasModel;
        private Vector2 _b;
        private float a11, a12, a21, a22;

        public IEnumerable<Anchor> GetPoints() => _points;
        public bool HasModel => _hasModel;

        public void AddPoint(Anchor point)
        {
            _points.Add(point);
            Recalculate();
        }

        public void ClearPoints()
        {
            _points.Clear();
            _hasModel = false;
        }

        public Vector2 GameToMap(Vector2 g)
        {
            if (!_hasModel) return Vector2.Zero;
            return _b + new Vector2(a11 * g.X + a12 * g.Y, a21 * g.X + a22 * g.Y);
        }

        public Vector2 MapToGame(Vector2 m)
        {
            if (!_hasModel) return Vector2.Zero;

            // Invert A
            float det = a11 * a22 - a12 * a21;
            if (MathF.Abs(det) < 1e-8f) return Vector2.Zero;

            var v = m - _b;
            float inv11 = a22 / det;
            float inv12 = -a12 / det;
            float inv21 = -a21 / det;
            float inv22 = a11 / det;

            return new Vector2(inv11 * v.X + inv12 * v.Y, inv21 * v.X + inv22 * v.Y);
        }

        private void Recalculate()
        {
            // Unknowns for mapX: [b1, a11, a12]
            // Unknowns for mapY: [b2, a21, a22]
            // Feature vector per point: [1, gx, gy]
            if (_points.Count < 3)
            {
                _hasModel = false;
                return;
            }

            double s00 = _points.Count;
            double s01 = 0, s02 = 0;
            double s11 = 0, s12 = 0, s22 = 0;

            double tx0 = 0, tx1 = 0, tx2 = 0; // X' * mapX
            double ty0 = 0, ty1 = 0, ty2 = 0; // X' * mapY

            foreach (var p in _points)
            {
                double x0 = 1.0;
                double x1 = p.Game.X;
                double x2 = p.Game.Y;

                s01 += x0 * x1;
                s02 += x0 * x2;
                s11 += x1 * x1;
                s12 += x1 * x2;
                s22 += x2 * x2;

                tx0 += x0 * p.Map.X;
                tx1 += x1 * p.Map.X;
                tx2 += x2 * p.Map.X;

                ty0 += x0 * p.Map.Y;
                ty1 += x1 * p.Map.Y;
                ty2 += x2 * p.Map.Y;
            }

            if (!TryInvertSymmetric3x3(s00, s01, s02, s11, s12, s22,
                                       out double i00, out double i01, out double i02,
                                       out double i11, out double i12, out double i22))
            {
                _hasModel = false;
                return;
            }

            // betaX = inv(M) * tX
            double bx0 = i00 * tx0 + i01 * tx1 + i02 * tx2;
            double bx1 = i01 * tx0 + i11 * tx1 + i12 * tx2;
            double bx2 = i02 * tx0 + i12 * tx1 + i22 * tx2;

            // betaY = inv(M) * tY
            double by0 = i00 * ty0 + i01 * ty1 + i02 * ty2;
            double by1 = i01 * ty0 + i11 * ty1 + i12 * ty2;
            double by2 = i02 * ty0 + i12 * ty1 + i22 * ty2;

            _b = new Vector2((float)bx0, (float)by0);
            a11 = (float)bx1; a12 = (float)bx2;
            a21 = (float)by1; a22 = (float)by2;

            _hasModel = true;
        }

        private static bool TryInvertSymmetric3x3(
            double s00, double s01, double s02,
            double s11, double s12,
            double s22,
            out double i00, out double i01, out double i02,
            out double i11, out double i12, out double i22)
        {
            double a = s00, b = s01, c = s02, d = s11, e = s12, f = s22;
            double det = a * (d * f - e * e) - b * (b * f - c * e) + c * (b * e - c * d);

            if (Math.Abs(det) < 1e-9)
            {
                i00 = i01 = i02 = i11 = i12 = i22 = 0;
                return false;
            }

            double invDet = 1.0 / det;

            i00 = (d * f - e * e) * invDet;
            i01 = -(b * f - c * e) * invDet;
            i02 = (b * e - c * d) * invDet;

            i11 = (a * f - c * c) * invDet;
            i12 = -(a * e - b * c) * invDet;

            i22 = (a * d - b * b) * invDet;

            return true;
        }
    }
}







