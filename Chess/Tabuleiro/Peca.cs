using System;
using System.Collections.Generic;
using System.Text;

namespace tabuleiro
{
    class Peca
    {
        Posicao Posicao { get; set; }
        public Cor Cor { get; set; }
        public int QtdMovimentos { get; set; }
        public Tabuleiro Tab { get; set; }

        public Peca(Posicao posicao, Tabuleiro tab, Cor cor)
        {
            Posicao = posicao;
            Tab = tab;
            Cor = cor;
            QtdMovimentos = 0;
        }
    }
}
