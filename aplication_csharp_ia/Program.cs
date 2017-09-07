using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace aplication_csharp_ia
{
    class Program
    {

        /*
         * MODO - FUNCIONANDO
         * 
         *      DIREITA
         *      BAIXO
         *      ESQUERDA
         *    
         * MODO - INDO_RECARREGAR VOLTANDO_RECARGA INDO_DESCARTAR VOLTANDO_DO DESCARTE
         * 
         *      QUALQUER DIREÇÃO 
         * 
         * 
         * 
         * 
         */


        static void Main(string[] args)
        {

            Init(args);
        }

        static void Init(string[] args)
        {
            Console.WriteLine("Inicializa com o mínimo viável... ");
            bool bLimpouAmbiente = false;

            //Variáveis para o agente
            int capacidade_lixo_coletado = 5;
            int carga_maxima = 20;

            //Variáveis para o ambiente
            int lixeiras, recargas = 1;
            int tamanho = 10;
            int quant_lixeiras = 3;
            int quant_recargas = 3;

            Console.WriteLine("Tradução...");
            //(n) =(Linha,Coluna), Capacidade Lixo (T), Carga Minima (C), QuantLixeiras (l), QuantRecargas(r)
            foreach (var value in args)
            {
                var dados = int.Parse(value.Split('=')[0]);

                switch (value)
                {
                    case "n":
                        tamanho = dados;
                        break;
                    case "t":
                        capacidade_lixo_coletado = dados;
                        break;
                    case "c":
                        carga_maxima = dados;
                        break;
                    case "l":
                        quant_lixeiras = dados;
                        break;
                    case "r":
                        quant_recargas = dados;
                        break;
                }
            }


            Console.WriteLine("Inicializa o agente...");
            var oAgente = new Agente(capacidade_lixo_coletado, carga_maxima);

            Console.WriteLine("Inicializa mapa...");
            var oMapa = new Ambiente(oAgente, tamanho, quant_recargas, quant_lixeiras);

            //Print Map
            Console.WriteLine(oMapa.ToString());

            

            //Atualização
            while (!bLimpouAmbiente)
            {
                //Atualiza as ações do agente e o mapa
                bLimpouAmbiente = oMapa.Atualiza();

                //Limpa console
                Console.Clear();

                //Print Map
                Console.WriteLine(oMapa.ToString());

                Thread.Sleep(500);

            }

            //Informações do grupo
            StringBuilder sDadosDev = new StringBuilder();
            sDadosDev.AppendLine("Inteligência Artifical T1 2017/2 ");
            sDadosDev.AppendLine("***Agente Coletor de Lixo*** ");
            sDadosDev.AppendLine("Desenvolvedores:");
            sDadosDev.AppendLine("Anderson Fraga, Jovani Brasil, Matheus Lima e  Renan Carvalho");
            Console.WriteLine(sDadosDev.ToString());


        }

    }



}
