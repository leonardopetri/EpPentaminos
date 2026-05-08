using EpPentaminos.Models;

namespace EpPentaminos.Game;

public sealed class GameEngine(int linhas, int colunas)
{
    private readonly Tabuleiro _board = new(linhas, colunas);
    private readonly IReadOnlyList<Pentomino> _pieces = Pentomino.Todos;
    private readonly HashSet<int> _used = [];

    public void Run()
    {
        Console.WriteLine("=== Modo Jogar — Pentaminós ===");
        Console.WriteLine("Comandos:");
        Console.WriteLine("  p <letra> <orientacao#> <linha> <coluna>  — coloca peça");
        Console.WriteLine("  s                                      — mostra tabuleiro");
        Console.WriteLine("  o <letra>                              — lista orientações da peça");
        Console.WriteLine("  q                                      — sair");
        Console.WriteLine();

        while (true)
        {
            Console.Write(_board.Renderizar(_pieces));
            Console.Write($"Peças usadas: {_used.Count}/12 > ");
            string? line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            try
            {
                switch (parts[0].ToLowerInvariant())
                {
                    case "q": return;
                    case "s": continue;
                    case "o": MostrarOrientacoes(parts[1][0]); break;
                    case "p": TratarPosicao(parts); break;
                    default: Console.WriteLine("Comando desconhecido."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }

            if (_used.Count == 12 || !_board.TentarEncontrarPrimeiroVazio(out _, out _))
            {
                Console.WriteLine("Tabuleiro completo — você venceu!");
                Console.Write(_board.Renderizar(_pieces));
                return;
            }
        }
    }

    private void MostrarOrientacoes(char letter)
    {
        var piece = AcharPeca(letter);
        Console.WriteLine($"Peça {piece.Symbol} possui {piece.Orientations.Count} orientações:");
        for (int i = 0; i < piece.Orientations.Count; i++)
        {
            var orientacao = piece.Orientations[i];
            // bounding box já normalizada (minR=minC=0); calcula apenas os máximos.
            int maxR = orientacao.Max(p => p.dr);
            int maxC = orientacao.Max(p => p.dc);

            // Tabuleiro temporário com o tamanho exato da peça.
            var preview = new Tabuleiro(maxR + 1, maxC + 1);
            preview.Inserir(orientacao, 0, 0, piece.Id);

            Console.WriteLine($"  [{i}]: {string.Join(" ", orientacao.Select(p => $"({p.dr},{p.dc})"))}");
            // Indenta cada linha do render para destacar visualmente.
            foreach (var line in preview.Renderizar(_pieces).Split('\n'))
            {
                if (!string.IsNullOrWhiteSpace(line))
                    Console.WriteLine("    " + line.TrimEnd());
            }
        }
    }

    private void TratarPosicao(string[] parts)
    {
        if (parts.Length < 5)
        {
            Console.WriteLine("Uso: p <letra> <orientacao#> <linha> <coluna>");
            return;
        }
        var piece = AcharPeca(parts[1][0]);
        if (_used.Contains(piece.Id))
            throw new InvalidOperationException($"Peça {piece.Symbol} já foi usada.");

        int orientIdx = int.Parse(parts[2]);
        if (orientIdx < 0 || orientIdx >= piece.Orientations.Count)
            throw new ArgumentException("Índice de orientação inválido.");

        int linha = int.Parse(parts[3]);
        int coluna = int.Parse(parts[4]);
        var orientacao = piece.Orientations[orientIdx];

        if (!_board.PodeInserir(orientacao, linha, coluna))
            throw new InvalidOperationException("Jogada inválida: fora dos limites ou sobreposição.");

        _board.Inserir(orientacao, linha, coluna, piece.Id);
        _used.Add(piece.Id);
        Console.WriteLine($"OK — peça {piece.Symbol} posicionada.");
    }

    private Pentomino AcharPeca(char letter)
    {
        char up = char.ToUpperInvariant(letter);
        var p = _pieces.FirstOrDefault(x => x.Symbol == up)
                ?? throw new ArgumentException($"Peça '{letter}' não existe.");
        return p;
    }
}
