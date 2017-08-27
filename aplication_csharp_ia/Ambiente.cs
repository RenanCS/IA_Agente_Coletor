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

        private int tam_map;
        private List<Cell> lixeiras;
        private List<Cell> recargas;
        private Agente oAgente;

        public Ambiente(Agente oAgent, int tamanho, int iQuantRecargas, int iQuantLixeiras)
        {
            this.oAgente = oAgent;
            tam_map = tamanho;
            map = new Cell[tamanho, tamanho];
            lixeiras = new List<Cell>();
            recargas = new List<Cell>();

            InicializaMap(iQuantRecargas, iQuantLixeiras);
        }

        private void InicializaMap(int iQuantRecargas, int iQuantLixeiras)
        {
            Random randLixo = new Random();

            //O ambiente deve ter entre 40% e 85% de lixo
            int quantLixo = ((tam_map * 2) * randLixo.Next(40, 85)) / 100;

            //Desenho map
            DesenhaBase(map, tam_map);
            DesenhaParedes(map, tam_map);
            DesenhaRecargasLixeiras(map, tam_map, iQuantLixeiras, true);
            DesenhaRecargasLixeiras(map, tam_map, iQuantRecargas, false);
            DesenhaLixo(map, tam_map, quantLixo);

            //Inicia agente
            oAgente.posAtual = new Ponto(0, 0);
            oAgente.quantLixo = 0;
            oAgente.EncherBateria();
            map[0, 0].item = oAgente;


        }

        private void DesenhaBase(Cell[,] map, int iNxN)
        {
            for (int i = 0; i < iNxN; i++)
            {
                for (int j = 0; j < iNxN; j++)
                {
                    map[i, j] = new Cell() { item = " . " };
                }
            }
        }

        private void DesenhaParedes(Cell[,] map, int iNxN)
        {
            //Desenha Parede
            int iEspaco = 3;
            int iDisDireita = iNxN - iEspaco - 1;

            for (int i = 2; i < iNxN - 2; i++)
            {
                if (i == 2 || i == iNxN - 3)
                {
                    map[i, iEspaco - 1] = new Cell() { item = " P " };
                    map[i, iDisDireita + 1] = new Cell() { item = " P " };
                }

                map[i, iEspaco] = new Cell() { item = " P " };
                map[i, iDisDireita] = new Cell() { item = " P " };
            }
        }

        private void DesenhaRecargasLixeiras(Cell[,] map, int iNxN, int iQuantObjetos, bool DesenhaLixeira)
        {
            Random rand = new Random();

            //Espaço entre a parede e a borda 
            int iEspaco = 2;
            int iDisDireita = iNxN - iEspaco;

            //Insere todos os objetos solicitados por parâmetro
            while (iQuantObjetos > 0)
            {
                for (int i = 3; i < iNxN - 3; i++)
                {
                    for (int cE = 0, cD = iNxN - 1; cE < iEspaco && cD > iDisDireita; cE++, cD--)
                    {
                        //Alocação de objeto de forma aleatória
                        bool inserirEsquerda = (rand.Next(0, 10) % 5) == 0;
                        bool inserirDireita = (rand.Next(10, 20) % 15) == 0;

                        int idxcoluna = 0;

                        //Verifica qual lado colocar o objeto
                        if (inserirEsquerda)
                            idxcoluna = cE;
                        else if (inserirDireita)
                            idxcoluna = cD;
                        else
                            continue;

                        if (map[i, idxcoluna].item.ToString() == " . ")
                        {
                            if (DesenhaLixeira)
                            {
                                map[i, idxcoluna].item = " L ";
                                lixeiras.Add(new Cell() { item = " L ", linha = i, coluna = idxcoluna });
                            }
                            else
                            {
                                map[i, idxcoluna].item = " R ";
                                recargas.Add(new Cell() { item = " L ", linha = i, coluna = idxcoluna });
                            }

                            if ((iQuantObjetos -= 1) == 0)
                                return;
                        }
                    }
                }
            }
        }

        private void DesenhaLixo(Cell[,] map, int iNxN, int quantLixo)
        {
            while (quantLixo > 0)
            {
                Random rand = new Random();

                for (int i = 0; i < iNxN; i++)
                {
                    for (int j = 0; j < iNxN; j++)
                    {
                        //Alocação de objeto de forma aleatória
                        bool inserirLixo = (rand.Next(0, 10) % 5) == 0;

                        if (inserirLixo)
                        {
                            if (map[i, j].item.ToString() == " . ")
                            {
                                map[i, j] = new Cell() { item = " X " };

                                if ((quantLixo -= 1) == 0)
                                    return;
                            }
                        }
                    }
                }
            }
        }

        internal bool Atualiza()
        {
            bool limpou_tudo = false;

            List<Ponto> sucessores = BuscaSucessores(oAgente.posAtual);

            //Verifica Bateria
            if (oAgente.PoucaBateria())
            {
                //Procurar um ponto de recarga
            }
            else if (oAgente.LixoCheio())
            {
                //Procurar um ponto de lixeira   
            }
            else
            {
                bool avancou = false;

                //Verificar ambiente somente sucessores que não foram limpos
                foreach (var suc in sucessores)
                {
                    if(oAgente.caminhoLimpo.Any(p => p.xy == suc.xy))
                        continue;
                    
                     var obj = map[suc.x, suc.y].item.ToString();

                    if (obj == " X " || obj == " . ")
                    {
                        avancou = true;

                        map[oAgente.posAtual.x, oAgente.posAtual.y].item = " . ";

                        oAgente.caminhoLimpo.Add(oAgente.posAtual);

                        oAgente.posAtual = new Ponto(suc.x, suc.y);

                        map[suc.x, suc.y].item = oAgente;

                        break;

                    }
                    else
                    {
                        continue;
                    }

                }

                //Caso o agente se encurrale, ele limpa cache
                if(!avancou)
                    oAgente.caminhoLimpo.Clear();

            }



            return limpou_tudo;
        }


        private List<Ponto> BuscaSucessores(Ponto posAtual)
        {
            //Obs: Está nesta ordem, por causa da movimentação do agente
            // ->, Down, <-, Up,Diagonal Esq Sup, D.D.Sup, D.E.Inf, D.D.Inf 
            var x = posAtual.x;
            var y = posAtual.y;

            List<Ponto> l = new List<Ponto>();

            //Posição Posterior
            if (y + 1 < tam_map && map[x, y + 1].item != " P ")
                l.Add(new Ponto(x, y + 1));

            //Posição Anterior
            if (y - 1 >= 0 && map[x, y - 1].item != " P ")
                l.Add(new Ponto(x, y - 1));

            //Posicao Inferior
            if (x + 1 < tam_map && map[x + 1, y].item != " P ")
                l.Add(new Ponto( x + 1,  y ));



            //Posição Superior
            if (x - 1 >= 0 && map[x - 1, y].item != " P ")
                l.Add(new Ponto(x - 1, y ));



            //Diagonal Esquerda Superior
            if (x - 1 >= 0 && y - 1 >= 0 && map[x - 1, y - 1].item != " P ")
                l.Add(new Ponto(  x - 1,  y - 1 ));

            //Diagonal Direita Superior
            if (x - 1 >= 0 && y + 1 < tam_map && map[x - 1, y + 1].item != " P ")
                l.Add(new Ponto( x - 1,  y + 1 ));


            //Diagonal Esquerda Inferior
            if (x + 1 < tam_map && y - 1 >= 0 && map[x + 1, y - 1].item != " P ")
                l.Add(new Ponto( x + 1,  y - 1 ));


            //Diagonal Direita Inferior
            if (x + 1 < tam_map && y + 1 < tam_map && map[x + 1, y + 1].item != " P ")
                l.Add(new Ponto( x + 1,  y + 1 ));


            return l;
        }

        public override string ToString()
        {
            var linha = "\t";

            for (int i = 0; i < tam_map; i++)
            {
                for (int j = 0; j < tam_map; j++)
                {
                    linha += map[i, j].item;
                }

                linha += "\n\t";

            }
            return linha;
        }

    }
}
