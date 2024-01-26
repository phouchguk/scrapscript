namespace ScrapScript.Parse
{
    public class Binop : Object
    {
        public Binop(BinopKind op, Object left, Object right)
        {
            Op = op;
            Left = left;
            Right = right;
        }

        public BinopKind Op { get; }
        public Object Left { get; }
        public Object Right { get; }
    }
}
