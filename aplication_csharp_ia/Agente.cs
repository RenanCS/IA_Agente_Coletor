using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aplication_csharp_ia
{
    public class Agente
    {
        public List<Nodo> caminhoLimpo { get; set; }

        //Nivelamento do lixo
        private int capacidade_maxima_lixo { get; set; }
        public int quantLixo { get; set; }

        //Nivelamento da bateria
        private int capacidade_maxima_bateria { get; set; }
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
            return capacidade_maxima_bateria == quantLixo;
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

        //Heurística 
        internal int Heuristica(Nodo p1, Nodo p2){

            var x = p1.x - p2.x;
            var y = p1.y - p2.y;
            return Math.Abs(x) + Math.Abs(y);
        }

        internal List<Nodo> buscaCaminho(Nodo inicio, Cell objetivo, Ambiente amb){

            Console.WriteLine(inicio.x + "-" + inicio.y + " => " + objetivo.linha + "-" + objetivo.coluna);
            Console.ReadKey();

            // The set of nodes already evaluated
            // closedSet:= { }
            Boolean[] conjuntoFechado = new Boolean[amb.map.Length];

            // The set of currently discovered nodes that are not evaluated yet.
            // Initially, only the start node is known.
            // openSet:= { start}
            LinkedList<Nodo> conjuntoAberto = new LinkedList<Nodo>();
            conjuntoAberto.AddLast(inicio);

            Boolean[] indiceConjuntoAberto = new Boolean[amb.map.Length];

            // For each node, which node it can most efficiently be reached from.
            // If a node can be reached from many nodes, cameFrom will eventually contain the
            // most efficient previous step.
            // cameFrom:= the empty map
            List<Nodo> vemDe = new List<Nodo>(amb.map.Length);

            // For each node, the cost of getting from the start node to that node.
            // gScore:= map with default value of Infinity
            List<int> gScore = new List<int>(amb.map.Length);

            // For each node, the total cost of getting from the start node to the goal
            // by passing by that node. That value is partly known, partly heuristic.
            // fScore:= map with default value of Infinity
            List<int> fScore = new List<int>(amb.map.Length);

            Console.WriteLine(amb.map.Length);


            for(int i = 0; i < amb.map.Length; i++){
                indiceConjuntoAberto[i] = false;
            }

            // The cost of going from start to start is zero.
            // gScore[start] := 0
            inicio.gscore = 0;

            // For the first node, that value is completely heuristic.
            // fScore[start] := heuristic_cost_estimate(start, goal)
            inicio.fscore = Heuristica(
                inicio, new Nodo(objetivo.linha, objetivo.coluna));

            // Esse 10 deveria ser a largura do mapa.
            indiceConjuntoAberto[inicio.y * 10 + inicio.x] = true;

            // while openSet is not empty
            while (conjuntoAberto.Count > 0){

                conjuntoAberto.OrderBy(o => o.fscore).ToList();
                
                // current:= the node in openSet having the lowest fScore[] value
                Nodo atual = conjuntoAberto.First();

                amb.map[atual.x, atual.y] = new Cell() { item = " Z " };
                Console.WriteLine(amb.map.ToString());


                Console.WriteLine("atual -> " + atual.x + "-" + atual.y + "\n");

                // if current = goal
                //      return reconstruct_path(cameFrom, current)
                if (atual.x == objetivo.linha && atual.y == objetivo.coluna){
                    // Remonta caminho
                    Console.WriteLine("Encontrou o caminho\n");

                    Console.WriteLine(atual.prev.x + " - " + atual.prev.y + "\n");

                    Console.ReadKey();
                    
                    
                    
                    //return constroiCaminho();
                }

                // openSet.Remove(current)
                conjuntoAberto.RemoveFirst();
                indiceConjuntoAberto[inicio.y * 10 + inicio.x] = false;
                // closedSet.Add(current)
                conjuntoFechado[atual.y* 10 + atual.x] = true;

                // for each neighbor of current
                foreach (var suc in amb.BuscaSucessores(atual)){


                    Console.WriteLine("suc -> " + suc.x + "-" + suc.y + "\n");
                    Console.ReadKey();

                    // Console.WriteLine(suc.xy);

                    // if neighbor in closedSet
                    //    continue		// Ignore the neighbor which is already evaluated.
                    if (conjuntoFechado[suc.y * 10 + suc.x] == true)
                        continue;

                    //if neighbor not in openSet	// Discover a new node
                    //    openSet.Add(neighbor)
                    if (indiceConjuntoAberto[suc.y * 10 + suc.x] == false){
                        conjuntoAberto.AddLast(suc);
                        indiceConjuntoAberto[suc.y * 10 + suc.x] = true;
                    }

                    // The distance from start to a neighbor
                    // tentative_gScore:= gScore[current] + dist_between(current, neighbor)
                    int gScore_temp = atual.gscore + Heuristica(atual, suc);

                    // if tentative_gScore >= gScore[neighbor]
                    //    continue		// This is not a better path.
                    if (gScore_temp >= suc.gscore)
                        continue;

                    // This path is the best until now. Record it!
                    // cameFrom[neighbor] := current
                    //vemDe[suc.y * amb.map.Length + suc.x] = atual;
                    suc.prev = atual;
                    
                    // gScore[neighbor] := tentative_gScore
                    // fScore[neighbor] := gScore[neighbor] + heuristic_cost_estimate(neighbor, goal)
                    suc.gscore = gScore_temp;
                    suc.fscore = gScore_temp + Heuristica(suc, new Nodo(objetivo.linha, objetivo.coluna));

                }
            }


            Console.WriteLine("falhou");

            // return failure
            return null;

        }

        public List<Nodo> BuscaMelhorCaminho(Nodo posAtual, List<Cell> listObj, Ambiente amb)
        {

            List<Nodo> lPontos = new List<Nodo>();

            foreach (var l in listObj)
            {

                List<Nodo>  lAux = buscaCaminho(posAtual, l, amb);

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
            return " A ";
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
