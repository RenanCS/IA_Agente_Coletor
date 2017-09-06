using System;
using System.Collections.Generic;
using System.Linq;

namespace aplication_csharp_ia
{
    public class Agente
    {
        public TIPO_ACAO Acao { get; set; }

        public List<Nodo> CaminhoLimpo { get; set; }

        public string Simbolo { get; set; } = " A ";

        //Nivelamento do lixo
        private int CapacidadeMaximaLixo { get; set; }
        public int QuantLixo { get; set; }

        public int max_column { get; set; }

        //Nivelamento da bateria
        private int CapacidadeMaximaBateria { get; set; }
        private int CapacidadeMinimaBateria { get; set; } = 10;
        public int quantBateria { get; set; }

        //Posição atual do agente
        public Nodo PosicaoAtual { get; set; }

        //Última posição no caso de ir recarregar e/ou esvaziar lixo
        public Nodo UltimaPosicao { get; set; }

        public Agente(int capacidadeMaximaLixo, int capacidadeMaximaBateria)
        {
            CapacidadeMaximaLixo = capacidadeMaximaLixo;
            CapacidadeMaximaBateria = capacidadeMaximaBateria;
            CaminhoLimpo = new List<Nodo>();
        }

        public bool LixoCheio() => CapacidadeMaximaLixo == QuantLixo;

        public bool BateriaCheia() => CapacidadeMaximaBateria == quantBateria;

        public bool PoucaBateria() => (quantBateria <= CapacidadeMinimaBateria);

        public void EncherBateria() => quantBateria = CapacidadeMaximaBateria;

        //Heurística 
        public int Heuristica(Nodo p1, Nodo p2)
        {
            var x = p1.x - p2.x;
            var y = p1.y - p2.y;
            return Math.Abs(x) + Math.Abs(y);
        }

        public LinkedList<Nodo> buscaCaminho(Nodo inicio, Cell objetivo, Ambiente amb)
        {
            Cell[,] mapa = (Cell[,])amb.map.Clone();

            Nodo atual = inicio;

            Console.WriteLine(inicio.x + "-" + inicio.y + " => " + objetivo.Linha + "-" + objetivo.Coluna);

            Nodo nodo_objetivo = new Nodo(objetivo.Linha, objetivo.Coluna);

            List<Nodo> sucessores_objetivo = amb.BuscaSucessores(nodo_objetivo);

            LinkedList<Nodo> conjuntoAberto = new LinkedList<Nodo>();

            LinkedList<Nodo> conjuntoFechado = new LinkedList<Nodo>();

            conjuntoAberto.AddLast(inicio);


            while (true)
            {
                conjuntoAberto.OrderBy(o => o.fscore).ToList();

                atual = conjuntoAberto.First();

                Console.WriteLine(atual.x + " - " + atual.y + "\n");

                Console.WriteLine(amb.ToString());


                if (sucessores_objetivo.Any(o => o.xy == atual.xy))
                    break; // Encontrou o caminho

                if (conjuntoAberto.Count == 0)
                    break; //Não encontrou o caminho

                conjuntoAberto.RemoveFirst();

                conjuntoFechado.AddFirst(atual);


                foreach (var vizinho in amb.BuscaSucessores(atual))
                {
                    string tipo_vizinho = amb.map[vizinho.x, vizinho.y].Item.ToString();

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
}
