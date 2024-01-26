namespace ScrapScript.Scrap
{
    public class Prec
    {
        public double Pl { get; private set; }
        public double Pr { get; private set; }

        public Prec(double l, double r)
        {
            this.Pl = l;
            this.Pr = r;
        }

        public static Prec Lp(double n)
        {
            return new Prec(n, n - 0.1);
        }

        public static Prec Rp(double n)
        {
            return new Prec(n, n + 0.1);
        }

        public static Prec Np(double n)
        {
            return new Prec(n, n);
        }

        public static Prec Xp(double n)
        {
            return new Prec(n, 0);
        }
    }
}
