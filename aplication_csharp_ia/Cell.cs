namespace aplication_csharp_ia
{
    public class Cell
    {   
        public int Linha { get; set; }

        public int Coluna { get; set; }

        public object Item { get; set; }

        public override string ToString()
        {
            return Item.ToString();
        }
    }
}
