using System;
using EpPentaminos.Game;
using EpPentaminos.Models;
using EpPentaminos.Search;

namespace EpPentaminos;

/// <summary>
/// Ponto de entrada do sistema. Apresenta um menu CLI com os modos:
///   1) Resolver (DFS / BFS)
///   2) Jogar (interativo)
/// </summary>
public static class Program
{
    public static void Main()
    {
        Console.WriteLine("=========================================");
        Console.WriteLine("  Pentaminós — Resolução e Jogo Interativo");
        Console.WriteLine("=========================================");

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("[1] Resolver");
            Console.WriteLine("[2] Jogar");
            Console.WriteLine("[0] Sair");
            Console.Write("> ");
            var op = Console.ReadLine();
            switch (op)
            {
                case "1": 
                    Resolver(); 
                    break;
                case "2": 
                    Jogar(); 
                    break;
                case "0": 
                    return;
                default: 
                    Console.WriteLine("Opção inválida."); 
                    break;
            }
        }
    }

    private static (int linhas, int colunas)? LerDimensoes()
    {
        Console.Write("Linhas (m): ");
        if (!int.TryParse(Console.ReadLine(), out int m))
            return null;
        
        Console.Write("Colunas (n): ");
        if (!int.TryParse(Console.ReadLine(), out int n))
            return null;

        var (ok, msg) = Tabuleiro.ValidarDimensoes(m, n);
        Console.WriteLine($"Validação: {msg}");
        return ok ? (m, n) : null;
    }

    private static void Resolver()
    {
        var dimensoes = LerDimensoes();
        
        if (dimensoes is null) 
        {
            Console.WriteLine("Dimensões inválidas."); 
            return; 
        }

        var (linhas, colunas) = dimensoes.Value;

        Console.WriteLine("Estratégia: [d]fs ou [b]fs ?");
        Console.Write("> ");
        var strat = (Console.ReadLine() ?? "d").Trim().ToLowerInvariant();

        var todos = false;
        if (strat != "b")
        {
            Console.Write("Encontrar todas as soluções? (s/n): ");
            todos = (Console.ReadLine() ?? "n").Trim().StartsWith("s", StringComparison.OrdinalIgnoreCase);
        }

        var inicial = Estado.Inicial(linhas, colunas);
        var pecas = Pentomino.Todos;

        ResultadoPesquisa result = strat == "b"
            ? GrafoDeBusca.BreadthFirstSearch(inicial, pecas)
            : GrafoDeBusca.DepthFirstSearch(inicial, pecas, todos);

        Console.WriteLine();
        Console.WriteLine($"--- Resultado ({(strat == "b" ? "BFS" : "DFS")}) ---");
        Console.WriteLine($"Soluções encontradas : {result.Solucoes.Count}");
        Console.WriteLine($"Estados explorados   : {result.EstadosExplorados}");
        Console.WriteLine($"Tempo decorrido      : {result.TempoCorrido.TotalMilliseconds:F2} ms");
        Console.WriteLine();

        var solucoes = Math.Min(result.Solucoes.Count, 5);
        for (int i = 0; i < solucoes; i++)
        {
            Console.WriteLine($"Solução #{i + 1}:");
            Console.Write(result.Solucoes[i].Renderizar(pecas));
            Console.WriteLine();
        }
        if (result.Solucoes.Count > solucoes)
            Console.WriteLine($"(... mais {result.Solucoes.Count - solucoes} soluções omitidas)");
    }

    private static void Jogar()
    {
        var dimensoes = LerDimensoes();
        if (dimensoes is null) 
        {
            Console.WriteLine("Dimensões inválidas.");
            return;
        }
        
        var (linhas, colunas) = dimensoes.Value;
        new GameEngine(linhas, colunas).Run();
    }
}
