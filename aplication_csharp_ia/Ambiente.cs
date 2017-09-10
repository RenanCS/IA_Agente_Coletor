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
        private int tamanho_percorrido = 0;
        private List<Cell> lixeiras;
        private List<Cell> recargas;
        private Agente oAgente;

        private bool Procurando_Lixeira;
        private bool Procurando_Recarga;
        private bool ativar_descida = true;

        private LinkedList<Nodo> melhorcaminho;
        private LinkedList<Nodo> caminhopercorrido;

        private string tipo_nodo_ref = " . ";

        public Ambiente(Agente oAgent, int tamanho, int iQuantRecargas, int iQuantLixeiras)
        {
            this.oAgente = oAgent;
            tam_map = tamanho;
            map = new Cell[tamanho, tamanho];
            lixeiras = new List<Cell>();
            recargas = new List<Cell>();

            caminhopercorrido = new LinkedList<Nodo>();

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
            oAgente.tentativas_limpeza = int.Parse(Math.Pow((tam_map * tam_map), 2).ToString());
            oAgente.posAtual = new Nodo(0, 0);
            oAgente.ultimaPosicao = oAgente.posAtual;
            oAgente.quantLixo = 0;
            oAgente.Acao = TIPO_ACAO.DESCIDA;
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
                                recargas.Add(new Cell() { item = " R ", linha = i, coluna = idxcoluna });
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

            if (Procurando_Lixeira)
            {
                ExecutaAcaoEspecifica(true);
            }
            else if (Procurando_Recarga)
            {
                ExecutaAcaoEspecifica(false);
            }
            else
            {

                oAgente.Simbolo = " A ";

                //Verifica Bateria
                if (oAgente.PoucaBateria())
                {
                    Procurando_Recarga = true;

                    //Chamar método A*
                    melhorcaminho = oAgente.BuscaMelhorCaminho(oAgente.posAtual, recargas, this);

                    tamanho_percorrido = melhorcaminho.Count;

                }
                //Verifica Lixeira
                else if (oAgente.LixoCheio())
                {
                    Procurando_Lixeira = true;

                    //Chamar método A*
                    melhorcaminho = oAgente.BuscaMelhorCaminho(oAgente.posAtual, lixeiras, this);

                    tamanho_percorrido = melhorcaminho.Count;
                }
                else
                {

                    AtualizaDirecaoAgente();

                    oAgente.tentativas_limpeza -= 1;

                    if (oAgente.tentativas_limpeza == 0)
                    {
                        limpou_tudo = true;
                    }


                    List<Nodo> sucessores = new List<Nodo>();

                    //Retorna a lista de sucessores
                    sucessores = BuscaSucessores(oAgente.posAtual, oAgente.Acao).Where(o => o.xy != oAgente.ultimaPosicao.xy).ToList();

                    //Verificar ambiente somente sucessores que não foram limpos
                    foreach (var suc in sucessores)
                    {

                        var obj = map[suc.x, suc.y].item.ToString();

                        if (obj == " X " || obj == " . ")
                        {
                            if (obj == " X ")
                                oAgente.quantLixo += 1;

                            oAgente.quantBateria -= 1;

                            map[oAgente.posAtual.x, oAgente.posAtual.y].item = " . ";

                            oAgente.ultimaPosicao = oAgente.posAtual;

                            oAgente.posAtual = new Nodo(suc.x, suc.y);

                            map[suc.x, suc.y].item = oAgente;

                            break;

                        }
                        else
                        {
                            continue;
                        }

                    }


                }
            }


            return limpou_tudo;
        }

        private void ExecutaAcaoEspecifica(bool lixeira)
        {
            if (melhorcaminho.Count > 0)
            {
                //Remove o primeiro ponto de coordenada
                var caminho = melhorcaminho.First();

                //Atualiza o map
                map[oAgente.posAtual.x, oAgente.posAtual.y].item = tipo_nodo_ref;

                tipo_nodo_ref = map[caminho.x, caminho.y].item.ToString();

                //Atualiza o ponto atual do agente
                oAgente.posAtual = new Nodo(caminho.x, caminho.y);

                oAgente.Simbolo = " A*";

                //Atualiza o map com a o obj do agente
                map[caminho.x, caminho.y].item = oAgente;

                //Remove o primeiro ponto, para obter os adjacentes
                melhorcaminho.RemoveFirst();

                //Adiciona no caminho percorrido 
                caminhopercorrido.AddFirst(caminho);

                //Quando zerar a lista, significa que o agente chegou no ponto de recarga
                if (melhorcaminho.Count == 0)
                {
                    if (lixeira)
                        //Esvaziar lixeira
                        oAgente.LimparLixeira();
                    else
                        //Carraga a bateria
                        oAgente.EncherBateria();

                }
            }
            else if (caminhopercorrido.Count > 0)
            {
                //Remove o primeiro ponto de coordenada
                var caminho = caminhopercorrido.First();

                //Atualiza o map
                map[oAgente.posAtual.x, oAgente.posAtual.y].item = tipo_nodo_ref;

                tipo_nodo_ref = map[caminho.x, caminho.y].item.ToString();

                //Atualiza o ponto atual do agente
                oAgente.posAtual = new Nodo(caminho.x, caminho.y);

                oAgente.Simbolo = " A'";

                //Atualiza o map com a o obj do agente
                map[caminho.x, caminho.y].item = oAgente;

                //Remove o primeiro ponto, para obter os adjacentes
                caminhopercorrido.RemoveFirst();

            }
            else
            {

                //Evita que entre de novo na condição
                if (lixeira)
                    Procurando_Lixeira = false;
                else
                    Procurando_Recarga = false;
            }

        }

        private void AtualizaDirecaoAgente()
        {

            //Agente chegou na última linha
            if (oAgente.posAtual.x == tam_map - 1 && oAgente.Acao == TIPO_ACAO.DESCIDA)
                oAgente.Acao = TIPO_ACAO.SUBIDA;

            //Agente chegou na primeira linha
            else if (oAgente.posAtual.x == 0 && oAgente.Acao == TIPO_ACAO.SUBIDA)
                oAgente.Acao = TIPO_ACAO.DESCIDA;

            //Agente está no canto dreito superior
            if (oAgente.posAtual.x == 0 && oAgente.posAtual.y == tam_map - 1)
            {
                if (oAgente.Acao == TIPO_ACAO.SUBIDA || oAgente.Acao == TIPO_ACAO.SUBIDA)
                {
                    oAgente.Acao = TIPO_ACAO.HORIZONTAL_ESQUERDA;
                    oAgente.ultimaPosicao = oAgente.posAtual;
                }
                else if (oAgente.Acao == TIPO_ACAO.HORIZONTAL_ESQUERDA || oAgente.Acao == TIPO_ACAO.HORIZONTAL_DIREITA)
                {
                    oAgente.Acao = TIPO_ACAO.HORIZONTAL_PLUS;
                }
            }

            // Agente está no canto direito inferior
            else if (oAgente.posAtual.x == tam_map - 1 && oAgente.posAtual.y == tam_map - 1)
            {
                oAgente.Acao = TIPO_ACAO.HORIZONTAL_DIREITA;
                oAgente.ultimaPosicao = oAgente.posAtual;
            }

        }

        public List<Nodo> BuscaSucessores(Nodo posAtual, TIPO_ACAO tipo_acao = TIPO_ACAO.DESCIDA)
        {
            //Obs: Está nesta ordem, por causa da movimentação do agente
            // ->, Down, <-, Up,Diagonal Esq Sup, D.D.Sup, D.E.Inf, D.D.Inf 
            var x = posAtual.x;
            var y = posAtual.y;

            List<Nodo> l = new List<Nodo>();

            switch (tipo_acao)
            {
                case TIPO_ACAO.DESCIDA:

                    //AGENTE DESCENDO

                    //ABAIXO
                    if (x + 1 < tam_map && map[x + 1, y].item != " P ")
                        l.Add(new Nodo(x + 1, y));

                    //DIREITA
                    if (y + 1 < tam_map && map[x, y + 1].item != " P ")
                        l.Add(new Nodo(x, y + 1));

                    // ACIMA
                    if (x - 1 >= 0 && map[x - 1, y].item != " P ")
                        l.Add(new Nodo(x - 1, y));

                    break;

                case TIPO_ACAO.SUBIDA:

                    //AGENTE SUBINDO

                    // ACIMA
                    if (x - 1 >= 0 && map[x - 1, y].item != " P ")
                        l.Add(new Nodo(x - 1, y));

                    //DIREITA
                    if (y + 1 < tam_map && map[x, y + 1].item != " P ")
                        l.Add(new Nodo(x, y + 1));

                    //ABAIXO
                    if (x + 1 < tam_map && map[x + 1, y].item != " P ")
                        l.Add(new Nodo(x + 1, y));

                    break;

                case TIPO_ACAO.HORIZONTAL_ESQUERDA:

                    //AGENTE ANDANDO NA HORIZONTAL INDO PARA ESQUERDA

                    //ESQUERDA
                    if (y - 1 >= 0 && map[x, y - 1].item != " P ")
                        l.Add(new Nodo(x, y - 1));

                    //DIREITA
                    if (y + 1 < tam_map && map[x, y + 1].item != " P ")
                        l.Add(new Nodo(x, y + 1));

                    //ABAIXO
                    if (x + 1 < tam_map && map[x + 1, y].item != " P ")
                        l.Add(new Nodo(x + 1, y));

                    // ACIMA
                    if (x - 1 >= 0 && map[x - 1, y].item != " P ")
                        l.Add(new Nodo(x - 1, y));

                    break;

                case TIPO_ACAO.HORIZONTAL_DIREITA:

                    //AGENTE ANDANDO NA HORIZONTAL INDO PARA DIREITA

                    //DIREITA
                    if (y + 1 < tam_map && map[x, y + 1].item != " P ")
                        l.Add(new Nodo(x, y + 1));

                    //ESQUERDA
                    if (y - 1 >= 0 && map[x, y - 1].item != " P ")
                        l.Add(new Nodo(x, y - 1));

                    // ACIMA
                    if (x - 1 >= 0 && map[x - 1, y].item != " P ")
                        l.Add(new Nodo(x - 1, y));

                    //Diagonal Esquerda Superior
                    if (x - 1 >= 0 && y - 1 >= 0 && map[x - 1, y - 1].item != " P ")
                        l.Add(new Nodo(x - 1, y - 1));

                    //Diagonal Direita Superior
                    if (x - 1 >= 0 && y + 1 < tam_map && map[x - 1, y + 1].item != " P ")
                        l.Add(new Nodo(x - 1, y + 1));

                    //ABAIXO
                    if (x + 1 < tam_map && map[x + 1, y].item != " P ")
                        l.Add(new Nodo(x + 1, y));


                    break;

                case TIPO_ACAO.HORIZONTAL_PLUS:

                    //AGENTE ANDANDO NA HORIZONTAL INDO PARA DIREITA

                    //DIREITA
                    if (y + 1 < tam_map && map[x, y + 1].item != " P ")
                        l.Add(new Nodo(x, y + 1));


                    //ESQUERDA
                    if (y - 1 >= 0 && map[x, y - 1].item != " P ")
                        l.Add(new Nodo(x, y - 1));


                    //ABAIXO
                    if (x + 1 < tam_map && map[x + 1, y].item != " P ")
                        l.Add(new Nodo(x + 1, y));


                    //Diagonal Esquerda Inferior
                    if (x + 1 < tam_map && y - 1 >= 0 && map[x + 1, y - 1].item != " P ")
                        l.Add(new Nodo(x + 1, y - 1));


                    //Diagonal Direita Inferior
                    if (x + 1 < tam_map && y + 1 < tam_map && map[x + 1, y + 1].item != " P ")
                        l.Add(new Nodo(x + 1, y + 1));


                    break;


            }



            //Diagonal Esquerda Inferior
            if (x + 1 < tam_map && y - 1 >= 0 && map[x + 1, y - 1].item != " P ")
                l.Add(new Nodo(x + 1, y - 1));


            //Diagonal Direita Inferior
            if (x + 1 < tam_map && y + 1 < tam_map && map[x + 1, y + 1].item != " P ")
                l.Add(new Nodo(x + 1, y + 1));


            //Diagonal Direita Superior
            if (x - 1 >= 0 && y + 1 < tam_map && map[x - 1, y + 1].item != " P ")
                l.Add(new Nodo(x - 1, y + 1));

            //Diagonal Esquerda Superior
            if (x - 1 >= 0 && y - 1 >= 0 && map[x - 1, y - 1].item != " P ")
                l.Add(new Nodo(x - 1, y - 1));



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


            linha += "\n\t ****DADOS DO AGENTE**** ";
            linha += "\n \t Carga Bateria: " + oAgente.quantBateria;
            linha += "\n \t Carga Lixo: " + oAgente.quantLixo;
            linha += "\n \t Tentativas de limpeza: " + oAgente.tentativas_limpeza;

            linha += "\n\n";

            linha += "\n\t ****LEGENDA AGENTE**** ";
            linha += "\n \t Executando A* | Retornando A' ";
            linha += "\n \t Tamanho do Mapa: " +  tam_map + " x " + tam_map;
            linha += "\n \t Capacidade Máxima Recarga: " + oAgente.capacidade_maxima_bateria;
            linha += "\n \t Capacidade Máxima Lixo: " + oAgente.capacidade_maxima_lixo;
            linha += "\n \t Quantidade lixeiras: " + lixeiras.Count;
            linha += "\n \t Quantidade Recargas: " + recargas.Count;




            linha += "\n\n";

            if (Procurando_Lixeira)
            {
                linha += " \n\t Buscando melhor caminho para lixeira ****";
                linha += " \n\t Melhor caminho: " + tamanho_percorrido;
            }

            if (Procurando_Recarga)
            {
                linha += " \n\t Buscando melhor caminho para recarga ****";
                linha += " \n\t Melhor caminho: " + tamanho_percorrido;
            }


            return linha;
        }

    }


    public enum TIPO_ACAO
    {
        DESCIDA = 1,
        SUBIDA,
        HORIZONTAL_ESQUERDA,
        HORIZONTAL_DIREITA,
        HORIZONTAL_PLUS
    }
}
