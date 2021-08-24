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

        public PartidaDeXadrez()
        {
            Tab = new Tabuleiro(8, 8);
            Turno = 1;
            jogadorAtual = Cor.branco;
            Terminada = false;
            colocarPecas();
        }

        public void executaMovimento(Posicao origem, Posicao destino)
        {
            Peca p = Tab.retirarPeca(origem);
            p.incrementarQtdMovimentos();
            Peca pecaCapturada = Tab.retirarPeca(destino);
            Tab.colocarPeca(p, destino);
        }

        public void realizaJogada(Posicao origem, Posicao destino) 
        {
            executaMovimento(origem, destino);
            Turno++;
            mudaJogador();
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
            if (!Tab.peca(origem).podeMoverPara(destino))
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

        public void colocarPecas()
        {
            Tab.colocarPeca(new Torre(Tab, Cor.branco), new PosicaoXadrez('a', 1).toPosicao());
            Tab.colocarPeca(new Torre(Tab, Cor.branco), new PosicaoXadrez('h', 1).toPosicao());
            Tab.colocarPeca(new Rei(Tab, Cor.branco), new PosicaoXadrez('e', 1).toPosicao());

            Tab.colocarPeca(new Torre(Tab, Cor.preto), new PosicaoXadrez('a', 8).toPosicao());
            Tab.colocarPeca(new Torre(Tab, Cor.preto), new PosicaoXadrez('h', 8).toPosicao());
            Tab.colocarPeca(new Rei(Tab, Cor.preto), new PosicaoXadrez('e', 8).toPosicao());
        }
    }
}
