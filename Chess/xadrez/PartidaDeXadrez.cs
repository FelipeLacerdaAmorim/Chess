using System;
using System.Collections.Generic;
using System.Text;
using tabuleiro;

namespace xadrez
{
    class PartidaDeXadrez
    {
        public Tabuleiro Tab { get; private set; }
        public int Turno { get; private set; }

        public Cor jogadorAtual { get; private set; }
        public bool Terminada { get; private set; }

        private HashSet<Peca> Pecas;

        private HashSet<Peca> Capturadas;
        public bool Xeque { get; private set; }

        public Peca vuneravelEnPassant { get; private set; }

        public PartidaDeXadrez()
        {
            Tab = new Tabuleiro(8, 8);
            Turno = 1;
            jogadorAtual = Cor.branco;
            Terminada = false;
            Xeque = false;
            vuneravelEnPassant = null;
            Pecas = new HashSet<Peca>();
            Capturadas = new HashSet<Peca>();
            colocarPecas();
        }

        public Peca executaMovimento(Posicao origem, Posicao destino)
        {
            Peca p = Tab.retirarPeca(origem);
            p.incrementarQtdMovimentos();
            Peca pecaCapturada = Tab.retirarPeca(destino);
            Tab.colocarPeca(p, destino);
            if(pecaCapturada != null)
            {
                Capturadas.Add(pecaCapturada);
            }

            //#Jogada especial roque pequeno
            if(p is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna + 1);
                Peca T = Tab.retirarPeca(origemT);
                T.incrementarQtdMovimentos();
                Tab.colocarPeca(T, destinoT);
            }

            //#Jogada especial roque grande
            if (p is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna -4);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna - 1);
                Peca T = Tab.retirarPeca(origemT);
                T.incrementarQtdMovimentos();
                Tab.colocarPeca(T, destinoT);
            }

            //#Jogada especial en passant
            if(p is Peao)
            {
                if(origem.Coluna != destino.Coluna && pecaCapturada == null)
                {
                    Posicao posP;
                    if(p.Cor == Cor.branco)
                    {
                        posP = new Posicao(destino.Linha + 1, destino.Coluna);
                    }
                    else
                    {
                        posP = new Posicao(destino.Linha - 1, destino.Coluna);
                    }
                    pecaCapturada = Tab.retirarPeca(posP);
                    Capturadas.Add(pecaCapturada);
                }
            }

            return pecaCapturada;
        }

        public void desfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = Tab.retirarPeca(destino);
            p.decrementarQtdMovimentos();
            if(pecaCapturada != null)
            {
                Tab.colocarPeca(pecaCapturada, destino);
                Capturadas.Remove(pecaCapturada);
            }

            Tab.colocarPeca(p, origem);

            //#Jogada especial roque pequeno
            if (p is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna + 1);
                Peca T = Tab.retirarPeca(destinoT);
                T.decrementarQtdMovimentos();
                Tab.colocarPeca(T, origemT);
            }

            //#Jogada especial roque grande
            if (p is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna - 1);
                Peca T = Tab.retirarPeca(destinoT);
                T.decrementarQtdMovimentos();
                Tab.colocarPeca(T, origemT);
            }

            //#Jogada especial passant
            if(p is Peao)
            {
                if (origem.Coluna != destino.Coluna && pecaCapturada == vuneravelEnPassant)
                {
                    Peca peao = Tab.retirarPeca(destino);
                    Posicao posP;
                    if(p.Cor == Cor.branco)
                    {
                        posP = new Posicao(3, destino.Coluna);
                    }
                    else
                    {
                        posP = new Posicao(4, destino.Coluna);
                    }
                    Tab.colocarPeca(peao, posP);
                }
            }
            //Tab.colocarPeca(p, origem);
        }

        public void realizaJogada(Posicao origem, Posicao destino) 
        {
            Peca pecaCapturada = executaMovimento(origem, destino);

            if (estaEmXeque(jogadorAtual))
            {
                desfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque!");
            }
            if (estaEmXeque(adversaria(jogadorAtual)))
            {
                Xeque = true;
            }
            else
            {
                Xeque = false;
            }

            if (testeXequemate(adversaria(jogadorAtual)))
            {
                Terminada = true;
            }
            else
            {
                Turno++;
                mudaJogador();
            }

            Peca p = Tab.peca(destino);

            //#Jogada especial en passant

            if(p is Peao && (destino.Linha == origem.Linha -2 || destino.Linha == origem.Linha + 2))
            {
                vuneravelEnPassant = p;
            }
            else
            {
                vuneravelEnPassant = null;
            }
        }

        public void validarPosicaoDeOrigem(Posicao pos)
        {
            if(Tab.peca(pos) == null)
            {
                throw new TabuleiroException("Não existe peça nessa posição!");
            }
            if(jogadorAtual != Tab.peca(pos).Cor)
            {
                throw new TabuleiroException("A peça escolhida não é sua!");
            }
            if (!Tab.peca(pos).existeMovimentoPossivel())
            {
                throw new TabuleiroException("Não há movimentos possíveis para a peça escolhida!");
            }
        }

        public void validarPosicaoDeDestino(Posicao origem, Posicao destino)
        {
            if (!Tab.peca(origem).movimentoPossivel(destino))
            {
                throw new TabuleiroException("Posição de destino inválida!");
            }
        }

        private void mudaJogador()
        {
            if(jogadorAtual == Cor.branco)
            {
                jogadorAtual = Cor.preto;
            }
            else
            {
                jogadorAtual = Cor.branco;
            }
        }

        public HashSet<Peca> pecasCapturadas(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach(Peca x in Capturadas)
            {
                if(x.Cor == cor)
                {
                    aux.Add(x);
                }
            }
            return aux;
        }

        public HashSet<Peca> pecasEmJogo(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in Pecas)
            {
                if (x.Cor == cor)
                {
                    aux.Add(x);
                }
            }
            aux.ExceptWith(pecasCapturadas(cor));
            return aux;
        }

        private Cor adversaria(Cor cor)
        {
            if(cor == Cor.branco)
            {
                return Cor.preto;
            }
            else
            {
                return Cor.branco;
            }
        }

        private Peca rei(Cor cor)
        {
            foreach (Peca x in pecasEmJogo(cor))
            {
                if(x is Rei)
                {
                    return x;
                }
            }
            return null;
        }

        public bool estaEmXeque(Cor cor)
        {
            Peca R = rei(cor);
            if(R == null)
            {
                throw new TabuleiroException("Não tem rei da cor " + cor + " no tabuleiro!");
            }
            foreach (Peca x in pecasEmJogo(adversaria(cor))){
                bool[,] mat = x.movimentosPossiveis();
                if(mat[R.Posicao.Linha, R.Posicao.Coluna])
                {
                    return true;
                }
            }
            return false;
        }
        public bool testeXequemate(Cor cor)
        {
            if (!estaEmXeque(cor))
            {
                return false;
            }
            foreach (Peca x in pecasEmJogo(cor))
            {
                bool[,] mat = x.movimentosPossiveis();
                for (int i = 0; i < Tab.Linhas; i++)
                {
                    for(int j = 0; j < Tab.Colunas; j++)
                    {
                        if (mat[i, j])
                        {
                            Posicao origem = x.Posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = executaMovimento(origem, destino);
                            bool testeXeque = estaEmXeque(cor);
                            desfazMovimento(origem, destino, pecaCapturada);
                            if (!testeXeque)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void colocarNovaPeca(char coluna, int linha, Peca peca)
        {
            Tab.colocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            Pecas.Add(peca);
        }

        public void colocarPecas()
        {
            colocarNovaPeca('a', 1, new Torre(Tab, Cor.branco));
            colocarNovaPeca('h', 1, new Torre(Tab, Cor.branco));
            colocarNovaPeca('e', 1, new Rei(Tab, Cor.branco, this));
            colocarNovaPeca('d', 1, new Dama(Tab, Cor.branco));
            colocarNovaPeca('c', 1, new Bispo(Tab, Cor.branco));
            colocarNovaPeca('f', 1, new Bispo(Tab, Cor.branco));
            colocarNovaPeca('b', 1, new Cavalo(Tab, Cor.branco));
            colocarNovaPeca('g', 1, new Cavalo(Tab, Cor.branco));
            colocarNovaPeca('a', 2, new Peao(Tab, Cor.branco, this));
            colocarNovaPeca('b', 2, new Peao(Tab, Cor.branco, this));
            colocarNovaPeca('c', 2, new Peao(Tab, Cor.branco, this));
            colocarNovaPeca('d', 2, new Peao(Tab, Cor.branco, this));
            colocarNovaPeca('e', 2, new Peao(Tab, Cor.branco, this));
            colocarNovaPeca('f', 2, new Peao(Tab, Cor.branco, this));
            colocarNovaPeca('g', 2, new Peao(Tab, Cor.branco, this));
            colocarNovaPeca('h', 2, new Peao(Tab, Cor.branco, this));

            colocarNovaPeca('a', 8, new Torre(Tab, Cor.preto));
            colocarNovaPeca('h', 8, new Torre(Tab, Cor.preto));
            colocarNovaPeca('e', 8, new Rei(Tab, Cor.preto, this));
            colocarNovaPeca('d', 8, new Dama(Tab, Cor.preto));
            colocarNovaPeca('c', 8, new Bispo(Tab, Cor.preto));
            colocarNovaPeca('f', 8, new Bispo(Tab, Cor.preto));
            colocarNovaPeca('b', 8, new Cavalo(Tab, Cor.preto));
            colocarNovaPeca('g', 8, new Cavalo(Tab, Cor.preto));
            colocarNovaPeca('a', 7, new Peao(Tab, Cor.preto, this));
            colocarNovaPeca('b', 7, new Peao(Tab, Cor.preto, this));
            colocarNovaPeca('c', 7, new Peao(Tab, Cor.preto, this));
            colocarNovaPeca('d', 7, new Peao(Tab, Cor.preto, this));
            colocarNovaPeca('e', 7, new Peao(Tab, Cor.preto, this));
            colocarNovaPeca('f', 7, new Peao(Tab, Cor.preto, this));
            colocarNovaPeca('g', 7, new Peao(Tab, Cor.preto, this));
            colocarNovaPeca('h', 7, new Peao(Tab, Cor.preto, this));
        }
    }
}
