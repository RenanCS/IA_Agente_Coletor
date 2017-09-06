namespace aplication_csharp_ia
{
    public class Nodo
    {
        public Nodo(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.xy = string.Concat(x, y);
            this.fscore = int.MaxValue;
            this.gscore = 0;
            this.prev = null;
        }

        public int x { get; set; }
        public int y { get; set; }
        public string xy { get; set; }

        public int fscore { get; set; }
        public int gscore { get; set; }
        public Nodo prev { get; set; }

    }
}
