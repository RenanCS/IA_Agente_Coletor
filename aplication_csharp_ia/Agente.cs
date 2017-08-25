using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aplication_csharp_ia
{
    public class Agente
    {
        //Nivelamento do lixo
        private int maxLixeira { get; set; }
        public int quantLixo { get; set; }

        //Nivelamento da bateria
        private int minBateria { get; set; }
        public int quantBateria { get; set; }

        //Última posição
        public int[,] ultimaPosicao { get; set; }

        public Agente(int maxLixeira, int minBateria)
        {
            this.maxLixeira = maxLixeira;
            this.minBateria = minBateria;
        }

        public bool getMaxLixo()
        {
            return maxLixeira == quantLixo;
        }

        public bool getMinBateria()
        {
            return minBateria == quantBateria;
        }

        public bool getMaxBateria()
        {
            return 100 == quantBateria;
        }

        public override string ToString()
        {
            return " A ";
        }
    }
}
