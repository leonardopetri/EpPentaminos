using System.Diagnostics;
using EpPentaminos.DataStructures;
using EpPentaminos.Models;

namespace EpPentaminos.Search;

public sealed record ResultadoPesquisa(
    List<Tabuleiro> Solucoes,
    long EstadosExplorados,
    TimeSpan TempoCorrido);

public static class GrafoDeBusca
{
    public static ResultadoPesquisa DepthFirstSearch(
        Estado inicial,
        IReadOnlyList<Pentomino> pecas,
        bool encontrarTodos)
    {
        var solutions = new List<Tabuleiro>();
        var visitados = new ArvoreAVL<string>();
        long explorados = 0;

        var sw = Stopwatch.StartNew();
        var stack = new Stack<Estado>();
        stack.Push(inicial);
        visitados.Inserir(inicial.Tabuleiro.RetornarEstadoChave());

        while (stack.Count > 0)
        {
            var estado = stack.Pop();
            explorados++;

            if (estado.EstaPreenchido())
            {
                solutions.Add(estado.Tabuleiro);
                if (!encontrarTodos)
                    break;
                continue;
            }

            foreach (var child in estado.Expandir(pecas))
            {
                var chave = child.Tabuleiro.RetornarEstadoChave();
                if (visitados.Inserir(chave))
                    stack.Push(child);
            }
        }

        sw.Stop();
        return new ResultadoPesquisa(solutions, explorados, sw.Elapsed);
    }
    
    public static ResultadoPesquisa BreadthFirstSearch(
        Estado inicial,
        IReadOnlyList<Pentomino> pecas)
    {
        var solutions = new List<Tabuleiro>();
        var visitados = new ArvoreAVL<string>();
        long explorados = 0;

        var sw = Stopwatch.StartNew();
        var queue = new Queue<Estado>();
        queue.Enqueue(inicial);
        visitados.Inserir(inicial.Tabuleiro.RetornarEstadoChave());

        while (queue.Count > 0)
        {
            var estado = queue.Dequeue();
            explorados++;

            if (estado.EstaPreenchido())
            {
                solutions.Add(estado.Tabuleiro);
                break;
            }

            foreach (var child in estado.Expandir(pecas))
            {
                var chave = child.Tabuleiro.RetornarEstadoChave();
                if (visitados.Inserir(chave))
                    queue.Enqueue(child);
            }
        }

        sw.Stop();
        return new ResultadoPesquisa(solutions, explorados, sw.Elapsed);
    }
}
