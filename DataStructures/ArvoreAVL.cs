namespace EpPentaminos.DataStructures;

public sealed class ArvoreAVL<T> where T : IComparable<T>
{
    private sealed class Node
    {
        public T Chave = default!;
        public Node? Esquerda, Direita;
        public int Altura = 1;
    }

    private Node? _raiz;
    public int Quantidade { get; private set; }

    private static int RetornarAltura(Node? node) => node?.Altura ?? 0;
    private static int FatorBalanceamento(Node? node) => node is null ? 0 : RetornarAltura(node.Esquerda) - RetornarAltura(node.Direita);

    private static Node RotacionarDireita(Node y)
    {
        Node x = y.Esquerda!;
        Node? t2 = x.Direita;
        x.Direita = y;
        y.Esquerda = t2;
        y.Altura = 1 + Math.Max(RetornarAltura(y.Esquerda), RetornarAltura(y.Direita));
        x.Altura = 1 + Math.Max(RetornarAltura(x.Esquerda), RetornarAltura(x.Direita));
        return x;
    }

    private static Node RotacionarEsquerda(Node x)
    {
        Node y = x.Direita!;
        Node? t2 = y.Esquerda;
        y.Esquerda = x;
        x.Direita = t2;
        x.Altura = 1 + Math.Max(RetornarAltura(x.Esquerda), RetornarAltura(x.Direita));
        y.Altura = 1 + Math.Max(RetornarAltura(y.Esquerda), RetornarAltura(y.Direita));
        return y;
    }

    public bool Inserir(T chave)
    {
        bool inserted = false;
        _raiz = InserirRecursivo(_raiz, chave, ref inserted);
        if (inserted)
            Quantidade++;
        return inserted;
    }

    private static Node InserirRecursivo(Node? node, T chave, ref bool inserted)
    {
        if (node is null)
        {
            inserted = true;
            return new Node { Chave = chave };
        }

        var comparacao = chave.CompareTo(node.Chave);
        if (comparacao < 0)
            node.Esquerda = InserirRecursivo(node.Esquerda, chave, ref inserted);
        else if (comparacao > 0)
            node.Direita = InserirRecursivo(node.Direita, chave, ref inserted);
        else 
        { 
            inserted = false; 
            return node;
        }

        node.Altura = 1 + Math.Max(RetornarAltura(node.Esquerda), RetornarAltura(node.Direita));

        var balance = FatorBalanceamento(node);
        if (balance > 1 && chave.CompareTo(node.Esquerda!.Chave) < 0)
            return RotacionarDireita(node);

        if (balance > 1 && chave.CompareTo(node.Esquerda!.Chave) > 0)
        {
            node.Esquerda = RotacionarEsquerda(node.Esquerda);
            return RotacionarDireita(node);
        }

        if (balance < -1 && chave.CompareTo(node.Direita!.Chave) > 0)
            return RotacionarEsquerda(node);

        if (balance < -1 && chave.CompareTo(node.Direita!.Chave) < 0)
        {
            node.Direita = RotacionarDireita(node.Direita);
            return RotacionarEsquerda(node);
        }

        return node;
    }

    public bool ContemChave(T chave)
    {
        var chaveAtual = _raiz;
        while (chaveAtual is not null)
        {
            var comparacao = chave.CompareTo(chaveAtual.Chave);
            if (comparacao == 0)
                return true;
            chaveAtual = comparacao < 0 ? chaveAtual.Esquerda : chaveAtual.Direita;
        }
        return false;
    }
}
