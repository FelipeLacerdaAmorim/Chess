using System;
using tabuleiro;
using xadrez;

namespace Chess
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Tabuleiro tab = new Tabuleiro(8, 8);

                tab.colocarPeca(new Torre(tab, Cor.preto), new Posicao(0, 0));
                tab.colocarPeca(new Torre(tab, Cor.preto), new Posicao(1, 3));
                tab.colocarPeca(new Torre(tab, Cor.preto), new Posicao(2, 3));

                PosicaoXadrez pose = new PosicaoXadrez('c', 7);
                Tela.imprimirTabuleiro(tab);

                Console.WriteLine(pose);
                Console.WriteLine(pose.toPosicao());
            }
            catch(TabuleiroException e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
    }
}
