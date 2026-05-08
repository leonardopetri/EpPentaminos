namespace EpPentaminos.Models;

public sealed class Estado(Tabuleiro tabuleiro, IReadOnlySet<int> pecasUsadas)
{
    public Tabuleiro Tabuleiro { get; } = tabuleiro;
    public IReadOnlySet<int> PecasUsadas { get; } = pecasUsadas;

    public static Estado Inicial(int linhas, int colunas) => new(new Tabuleiro(linhas, colunas), new HashSet<int>());

    public bool EstaPreenchido() => !Tabuleiro.TentarEncontrarPrimeiroVazio(out _, out _);

    public IEnumerable<Estado> Expandir(IReadOnlyList<Pentomino> pecas)
    {
        if (!Tabuleiro.TentarEncontrarPrimeiroVazio(out int targetR, out int targetC))
            yield break;

        for (var i = 0; i < pecas.Count; i++)
        {
            if (PecasUsadas.Contains(i))
                continue;

            var piece = pecas[i];
            foreach (var orientacao in piece.Orientations)
            {
                foreach (var (dr, dc) in orientacao)
                {
                    var anchorR = targetR - dr;
                    var anchorC = targetC - dc;
                    if (anchorR < 0 || anchorC < 0)
                        continue;
                    
                    if (!Tabuleiro.PodeInserir(orientacao, anchorR, anchorC))
                        continue;

                    var nextTabuleiro = Tabuleiro.Clone();
                    nextTabuleiro.Inserir(orientacao, anchorR, anchorC, piece.Id);
                    var nextPecasUsadas = new HashSet<int>(PecasUsadas) { i };
                    yield return new Estado(nextTabuleiro, nextPecasUsadas);
                }
            }
        }
    }
}
