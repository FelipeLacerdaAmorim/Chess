using System;
using System.Collections.Generic;
using System.Text;
using tabuleiro;

namespace xadrez
{
    class PartidaDeXadrez
    {
        public Tabuleiro Tab { get; private set; }
        private int Turno;
        private Cor jogadorAtual;
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
