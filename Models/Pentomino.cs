namespace EpPentaminos.Models;

public sealed class Pentomino
{
    public int Id { get; }

    public char Symbol { get; }

    public IReadOnlyList<IReadOnlyList<(int dr, int dc)>> Orientations { get; }

    private Pentomino(int id, char symbol, IReadOnlyList<IReadOnlyList<(int, int)>> orientations)
    {
        Id = id;
        Symbol = symbol;
        Orientations = orientations;
    }

    private static readonly (char symbol, (int linha, int coluna)[] cells)[] Formatos =
    {
        ('F', new[] { (0,1),(0,2),(1,0),(1,1),(2,1) }),
        ('I', new[] { (0,0),(0,1),(0,2),(0,3),(0,4) }),
        ('L', new[] { (0,0),(1,0),(2,0),(3,0),(3,1) }),
        ('N', new[] { (0,1),(1,1),(2,0),(2,1),(3,0) }),
        ('P', new[] { (0,0),(0,1),(1,0),(1,1),(2,0) }),
        ('T', new[] { (0,0),(0,1),(0,2),(1,1),(2,1) }),
        ('U', new[] { (0,0),(0,2),(1,0),(1,1),(1,2) }),
        ('V', new[] { (0,0),(1,0),(2,0),(2,1),(2,2) }),
        ('W', new[] { (0,0),(1,0),(1,1),(2,1),(2,2) }),
        ('X', new[] { (0,1),(1,0),(1,1),(1,2),(2,1) }),
        ('Y', new[] { (0,1),(1,0),(1,1),(2,1),(3,1) }),
        ('Z', new[] { (0,0),(0,1),(1,1),(2,1),(2,2) }),
    };

    /// <summary>Conjunto imutável das 12 peças, já com orientações pré-computadas.</summary>
    public static IReadOnlyList<Pentomino> Todos { get; } = BuildTodos();

    private static List<Pentomino> BuildTodos()
    {
        var list = new List<Pentomino>(12);
        for (var i = 0; i < Formatos.Length; i++)
        {
            var (sym, cells) = Formatos[i];
            list.Add(new Pentomino(i + 1, sym, GerarOrientacoes(cells)));
        }
        return list;
    }

    private static List<IReadOnlyList<(int, int)>> GerarOrientacoes((int linha, int coluna)[] baseCells)
    {
        var hashset = new HashSet<string>();
        var resultado = new List<IReadOnlyList<(int, int)>>();

        // Aplica até 8 transformações (rotacionar * refletir)
        for (var refletir = 0; refletir < 2; refletir++)
        {
            var current = baseCells.Select(p => (linha: p.linha, coluna: refletir == 0 ? p.coluna : -p.coluna)).ToArray();
            for (var rotacionar = 0; rotacionar < 4; rotacionar++)
            {
                var normalized = Normalizar(current);
                var chave = string.Join(";", normalized.Select(p => $"{p.Item1},{p.Item2}"));
                if (hashset.Add(chave))
                    resultado.Add(normalized);

                // Rotação 90°: (r,c) -> (c,-r)
                current = [.. current.Select(p => (p.coluna, -p.linha))];
            }
        }
        return resultado;
    }

    /// <summary>
    /// Normaliza um conjunto de células: translada para que a menor linha seja 0 e a menor coluna 0,
    /// e ordena lexicograficamente para gerar uma chave estável.
    /// </summary>
    private static IReadOnlyList<(int, int)> Normalizar(IEnumerable<(int linha, int coluna)> cells)
    {
        var arr = cells.ToArray();
        var minR = arr.Min(p => p.linha);
        var minC = arr.Min(p => p.coluna);
        return [.. arr
            .Select(p => (p.linha - minR, p.coluna - minC))
            .OrderBy(p => p.Item1).ThenBy(p => p.Item2)];
    }
}
