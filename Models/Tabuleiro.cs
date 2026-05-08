using System.Text;

namespace EpPentaminos.Models;

public sealed class Tabuleiro
{
    public int Linhas { get; }
    public int Colunas { get; }
    private readonly int[,] _grid;

    public Tabuleiro(int linhas, int colunas)
    {
        if (linhas <= 0 || colunas <= 0)
            throw new ArgumentException("Dimensões devem ser positivas.");
        Linhas = linhas;
        Colunas = colunas;
        _grid = new int[linhas, colunas];
    }

    private Tabuleiro(int[,] grid)
    {
        Linhas = grid.GetLength(0);
        Colunas = grid.GetLength(1);
        _grid = grid;
    }

    public int this[int linha, int coluna] => _grid[linha, coluna];

    public int Area => Linhas * Colunas;

    public static (bool ok, string message) ValidarDimensoes(int linhas, int colunas)
    {
        var area = linhas * colunas;
        if (linhas <= 0 || colunas <= 0)
            return (false, "Linhas e colunas devem ser positivas.");
        if (area < 5)
            return (false, "Área total deve ser >= 5.");
        if (area % 5 != 0)
            return (true, "Aviso: área não é múltipla de 5; cobertura completa é impossível.");
        return (true, "OK");
    }

    public Tabuleiro Clone()
    {
        var copy = (int[,])_grid.Clone();
        return new Tabuleiro(copy);
    }

    public bool PodeInserir(IReadOnlyList<(int dr, int dc)> orientacao, int linha, int coluna)
    {
        for (var i = 0; i < orientacao.Count; i++)
        {
            var rr = linha + orientacao[i].dr;
            var cc = coluna + orientacao[i].dc;
            
            if (rr < 0 || rr >= Linhas || cc < 0 || cc >= Colunas)
                return false;

            if (_grid[rr, cc] != 0)
                return false;
        }
        return true;
    }

    public void Inserir(IReadOnlyList<(int dr, int dc)> orientacao, int linha, int coluna, int pieceId)
    {
        for (var i = 0; i < orientacao.Count; i++)
            _grid[linha + orientacao[i].dr, coluna + orientacao[i].dc] = pieceId;
    }

    public void Remover(IReadOnlyList<(int dr, int dc)> orientacao, int linha, int coluna)
    {
        for (var i = 0; i < orientacao.Count; i++)
            _grid[linha + orientacao[i].dr, coluna + orientacao[i].dc] = 0;
    }

    public bool TentarEncontrarPrimeiroVazio(out int r, out int c)
    {
        for (int i = 0; i < Linhas; i++)
            for (int j = 0; j < Colunas; j++)
                if (_grid[i, j] == 0)
                {
                    r = i; c = j; return true;
                }
        r = c = -1;
        return false;
    }

    public string Renderizar(IReadOnlyList<Pentomino> pecas)
    {
        var sb = new StringBuilder();
        
        sb.Append("  ");
        for (var j = 0; j < Colunas; j++)
            sb.Append(j % 10).Append(' ');
            
        sb.AppendLine();
        for (var i = 0; i < Linhas; i++)
        {
            sb.Append(i % 10).Append(' ');
            for (var j = 0; j < Colunas; j++)
            {
                var v = _grid[i, j];
                var ch = v == 0 ? '.' : pecas[v - 1].Symbol;
                sb.Append(ch).Append(' ');
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public string RetornarEstadoChave()
    {
        var sb = new StringBuilder(Linhas * Colunas);
        for (var i = 0; i < Linhas; i++)
            for (var j = 0; j < Colunas; j++)
                sb.Append((char)('a' + _grid[i, j]));
        return sb.ToString();
    }
}
