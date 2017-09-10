using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aplication_csharp_ia
{
    public class Agente
    {
        public int tentativas_limpeza { get; set; }
        public TIPO_ACAO Acao { get; set; }  
        public List<Nodo> caminhoLimpo { get; set; }  
        public string Simbolo { get; set; } = " A "; 

        //Nivelamento do lixo
        public int capacidade_maxima_lixo { get; set; }
        public int quantLixo { get; set; }

        public int max_column { get; set; }

        //Nivelamento da bateria
        public int capacidade_maxima_bateria { get; set; }
        private int capacidade_minima_bateria { get; set; } = 10;
        public int quantBateria { get; set; }

        //Posição atual do agente
        public Nodo posAtual { get; set; }

        //Última posição no caso de ir recarregar e/ou esvaziar lixo
        public Nodo ultimaPosicao { get; set; }

        public Agente(int capacidade_maxima_lixo, int capacidade_maxima_bateria)
        {
            this.capacidade_maxima_lixo = capacidade_maxima_lixo;
            this.capacidade_maxima_bateria = capacidade_maxima_bateria;
            this.caminhoLimpo = new List<Nodo>();
        }

        public bool LixoCheio()
        {
            return capacidade_maxima_lixo == quantLixo;
        }

        public bool BateriaCheia()
        {
            return capacidade_maxima_bateria == quantBateria;
        }

        public bool PoucaBateria()
        {
            return (quantBateria <= capacidade_minima_bateria);
        }

        public void EncherBateria()
        {
            quantBateria = capacidade_maxima_bateria;
        }

        public void LimparLixeira()
        {
            quantLixo = 0;
        }

        //Heurística 
        internal int Heuristica(Nodo p1, Nodo p2)
        {

            var x = p1.x - p2.x;
            var y = p1.y - p2.y;
            return Math.Abs(x) + Math.Abs(y);
        }

        internal LinkedList<Nodo> buscaCaminho(Nodo inicio, Cell objetivo, Ambiente amb)
        {
            Cell[,] map = (Cell[,])amb.map.Clone();

            Nodo atual = inicio;                                                                            

            Nodo nodo_objetivo = new Nodo(objetivo.linha, objetivo.coluna);

            List<Nodo> sucessores_objetivo = amb.BuscaSucessores(nodo_objetivo);

            LinkedList<Nodo> conjuntoAberto = new LinkedList<Nodo>();

            LinkedList<Nodo> conjuntoFechado = new LinkedList<Nodo>();

            conjuntoAberto.AddLast(inicio);


            while (true)
            {
                var listoder = conjuntoAberto.OrderBy(o => o.fscore).ToList();

                conjuntoAberto.Clear();

                conjuntoAberto = new LinkedList<Nodo>(listoder);

                atual = conjuntoAberto.First();                                                             

                if (sucessores_objetivo.Any(o => o.xy == atual.xy))
                    break; // Encontrou o caminho

                if (conjuntoAberto.Count == 0)
                    break; //Não encontrou o caminho

                conjuntoAberto.RemoveFirst();

                conjuntoFechado.AddFirst(atual);


                foreach (var vizinho in amb.BuscaSucessores(atual))
                {
                    string tipo_vizinho = amb.map[vizinho.x, vizinho.y].item.ToString();
                    
                    //Posição vizinho é recarga e/ou lixeira não computar
                    if (tipo_vizinho == " L " || tipo_vizinho == " R " || tipo_vizinho == " P ")
                        continue;
                    
                    //Vizinho já está na lista fechada
                    if (conjuntoFechado.Any(o => o.xy == vizinho.xy))
                        continue;

                    //Vizinho não está na lista aberta
                    if (!conjuntoAberto.Any(o => o.xy == vizinho.xy))
                    {
                        vizinho.prev = atual;

                        vizinho.gscore = atual.gscore + 1;

                        vizinho.fscore = vizinho.gscore +
                                         Heuristica(vizinho, nodo_objetivo);

                        conjuntoAberto.AddFirst(vizinho);

                    }
                    else
                    {

                        var vizinho_antigo = conjuntoAberto.FirstOrDefault(o => o.xy == vizinho.xy);

                        if (vizinho_antigo.gscore < vizinho.gscore)
                        {
                            conjuntoAberto.Remove(vizinho_antigo);

                            vizinho_antigo.gscore = atual.gscore + 1;

                            vizinho_antigo.fscore = vizinho_antigo.gscore + Heuristica(vizinho_antigo, nodo_objetivo);

                            vizinho_antigo.prev = atual;

                            conjuntoAberto.AddFirst(vizinho_antigo);
                        }

                    }

                }

            }

            LinkedList<Nodo> caminho = new LinkedList<Nodo>();

            caminho.AddFirst(atual);

            while (atual.prev != null)
            {
                caminho.AddFirst(atual.prev);

                atual = atual.prev;
            }

            return caminho;

        }

        public LinkedList<Nodo> BuscaMelhorCaminho(Nodo posAtual, List<Cell> listObj, Ambiente amb)
        {

            LinkedList<Nodo> lPontos = new LinkedList<Nodo>();

            foreach (var l in listObj)
            {

                LinkedList<Nodo> lAux = buscaCaminho(posAtual, l, amb);

                if (lAux == null)
                    continue;

                //Armazena menor percurso
                if (lPontos.Count == 0 || lAux.Count < lPontos.Count)
                    lPontos = lAux;
            }

            return lPontos;
        }


        public override string ToString()
        {
            return Simbolo;
        }


    }

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


static class Extensions
{
    public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
    {
        return listToClone.Select(item => (T)item.Clone()).ToList();
    }
}
