using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aplication_csharp_ia
{
    public class Ambiente
    {
        public Cell[,] map { get; set; }

        private int Tamanho;


        public Ambiente(Agente oAgente,int iTam, int iQuantRecargas, int iQuantLixeiras)
        {
            Tamanho = iTam;
            map = new Cell[iTam, iTam];
            InicializaMap(oAgente,iTam, iQuantRecargas, iQuantLixeiras);
        }
        
        private void InicializaMap(Agente oAgente,int iTam, int iQuantRecargas, int iQuantLixeiras)
        {
            Random randLixo = new Random();

            //O ambiente deve ter entre 40% e 85% de lixo
            int quantLixo = (Tamanho * randLixo.Next(40, 85)) / 100;

            //Desenho map
            DesenhaBordas(map, iTam);
            DesenhaParedes(map, iTam);
            DesenhaLixeira(map, iTam, iQuantLixeiras);
            DesenhaRecargas(map, iTam, iQuantRecargas);

            //Inicia agente
            map[0, 0].item = oAgente;





        }

        private void DesenhaRecargas(Cell[,] map, int iTam, int iQuantRecargas)
        {
        }

        private void DesenhaLixeira(Cell[,] map, int iTam, int iQuantLixeiras)
        {
        }

        private void DesenhaParedes(Cell[,] map, int iTam)
        {
            //Desenha Parede
            int iEspaco = 3;
            int iDisDireita = iTam - iEspaco - 1;

            for (int i = 2; i < iTam - 2; i++)
            {
                if (i == 2 || i == iTam - 3)
                {
                    map[i, iEspaco - 1] = new Cell() { item = " P " };
                    map[i, iDisDireita + 1] = new Cell() { item = " P " };
                }

                map[i, iEspaco] = new Cell() { item = " P " };
                map[i, iDisDireita] = new Cell() { item = " P " };

            }
        }

        private void DesenhaBordas(Cell[,] map, int iTam)
        {
            //Desenha bordas                    
            for (int i = 0; i < iTam; i++)
            {
                for (int j = 0; j < iTam; j++)
                {
                    map[i, j] = new Cell() { item = " . " };
                }
            }
        }

        public override string ToString()
        {
            var linha = "\t";

            for (int i = 0; i < Tamanho; i++)
            {
                for (int j = 0; j < Tamanho; j++)
                {
                    linha += map[i, j].item;
                }

                linha += "\n\t";

            }
            return linha;
        }

    }
}
