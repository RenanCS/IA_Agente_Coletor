using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aplication_csharp_ia
{
    class Program
    {
        static void Main(string[] args)
        {
            Init(args);
        }

        static void Init(string [] args)
        {      
            Console.WriteLine("Inicializa com o mínimo viável... ");

            int iLixeiras, iRecargas = 1;
            int iTam = 10;
            int iCapacidadeLixo = 20;
            int iCargaMinima = 20;
            int QuantLixeiras = 10;
            int QuantRecargas = 10;

            Console.WriteLine("Tradução...");
            //(n) =(Linha,Coluna), Capacidade Lixo (T), Carga Minima (C), QuantLixeiras (l), QuantRecargas(r)
            foreach (var value in args)
            {
                var dados = int.Parse(value.Split('=')[0]);

                switch (value)
                {
                    case "n":
                        iTam = dados;
                        break;
                    case "t":
                        iCapacidadeLixo = dados;
                        break;
                    case "c":
                        iCargaMinima = dados;
                        break;
                    case "l":
                        QuantLixeiras = dados;
                        break;
                    case "r":
                        QuantRecargas = dados;
                        break;
                }
            }


            Console.WriteLine("Inicializa o agente...");
            var oAgente = new Agente(iCapacidadeLixo,iCargaMinima);

            Console.WriteLine("Inicializa mapa...");
            var map = new Ambiente(oAgente,iTam , QuantRecargas, QuantLixeiras);
                                            

            //Print Map
           Console.WriteLine(map.ToString());


            string Pause = "";

        }

    }



}
