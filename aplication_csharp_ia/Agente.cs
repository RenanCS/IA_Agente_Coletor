using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aplication_csharp_ia
{
    public class Agente
    {
        public List<Ponto> caminhoLimpo { get; set; }

        //Nivelamento do lixo
        private int capacidade_maxima_lixo { get; set; }
        public int quantLixo { get; set; }

        //Nivelamento da bateria
        private int capacidade_maxima_bateria { get; set; }  
        private int capacidade_minima_bateria { get; set; } = 10;
        public int quantBateria { get; set; }

        //Posição atual do agente
        public Ponto posAtual { get; set; }
        
        //Última posição no caso de ir recarregar e/ou esvaziar lixo
        public Ponto ultimaPosicao { get; set; }

        public Agente(int capacidade_maxima_lixo, int capacidade_maxima_bateria)
        {
            this.capacidade_maxima_lixo = capacidade_maxima_lixo;
            this.capacidade_maxima_bateria = capacidade_maxima_bateria;
            this.caminhoLimpo = new List<Ponto>();
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

        
        public void aEstrela(Ponto inicio, Cell objetivo, Cell[,] map)
        {
            List<Boolean> visitados = new List<Boolean>();

            
            




        }
        
        public override string ToString()
        {
            return " A ";
        }

    }

    public class Ponto
    {
        public Ponto(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.xy = string.Concat(x, y);
        }

        public int x { get; set; }
        public int y { get; set; }
        public string xy { get; set; }
           
        
    }
  
}
