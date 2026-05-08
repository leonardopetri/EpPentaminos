# EpPentaminos

Projeto em C# / .NET que implementa o quebra-cabeça dos **12 pentaminós**, com dois modos de uso:

1. **Resolver** — busca automática (DFS ou BFS) por soluções que cobrem o tabuleiro.
2. **Jogar** — modo interativo via terminal, no qual o jogador posiciona manualmente as peças.

---

## O que é um pentaminó?

Um **pentaminó** é uma figura plana formada por **5 quadrados unitários** ligados pelas arestas. Existem exatamente **12 pentaminós distintos** (a menos de rotação e reflexão), tradicionalmente nomeados pelas letras:

```
F   I   L   N   P   T   U   V   W   X   Y   Z
```

O objetivo do jogo é **cobrir um tabuleiro retangular** `m × n` usando essas peças sem sobrepor nem deixar buracos. Como cada peça ocupa 5 células, a área `m * n` precisa ser múltipla de 5 (e tipicamente ≤ 60, já que há 12 peças × 5 = 60 células).

---

## Como executar

Pré-requisitos: **.NET SDK** instalado.

```bash
dotnet run --project EpPentaminos.csproj
```

Você verá o menu principal:

```
[1] Resolver
[2] Jogar
[0] Sair
```

### Definindo o tabuleiro

Em ambos os modos é solicitado:

- **Linhas (m)** — número de linhas do tabuleiro.
- **Colunas (n)** — número de colunas do tabuleiro.

Restrições verificadas automaticamente:

- `m > 0` e `n > 0`
- `m * n >= 5`
- Se `m * n` não for múltiplo de 5, o programa avisa que **cobertura completa é impossível**.

Tamanhos clássicos: `6×10`, `5×12`, `4×15`, `3×20`.

---

## Modo Jogar (interativo)

A cada rodada o tabuleiro é exibido com coordenadas:

```
  0 1 2 3 4 5 6 7 8 9
0 . . . . . . . . . .
1 . . . . . . . . . .
...
```

- `.` representa célula vazia.
- A letra do pentaminó (ex.: `F`, `I`, `L`...) representa células já ocupadas.

### Comandos

| Comando | Sintaxe | Descrição |
|---------|---------|-----------|
| Posicionar peça | `p <letra> <orientacao#> <linha> <coluna>` | Coloca a peça na célula âncora indicada usando a orientação (índice) escolhida. |
| Mostrar tabuleiro | `s` | Apenas re-renderiza o tabuleiro. |
| Listar orientações | `o <letra>` | Mostra todas as orientações da peça (rotações + reflexões), com índice e visualização. |
| Sair | `q` | Encerra o modo de jogo. |

> A célula âncora `(linha, coluna)` corresponde ao deslocamento `(0,0)` da orientação. As demais células da peça são posicionadas relativamente a essa âncora.

### Exemplo de partida

```
o I
> Peça I possui 2 orientações:
>   [0]: (0,0) (0,1) (0,2) (0,3) (0,4)
>     I I I I I
>   [1]: (0,0) (1,0) (2,0) (3,0) (4,0)
>     I
>     I
>     I
>     I
>     I

p I 0 0 0
> OK — peça I posicionada.
```

### Exemplo de jogo completo (tabuleiro 5×5... ou similar)

Sequência de comandos que cobre o tabuleiro usando 5 peças (`Y`, `I`, `U`, `V`, `L`):

```
p y 0 0 3
p i 0 4 0
p u 1 0 2
p v 0 1 1
p l 6 0 0
```

Cole linha a linha no prompt do modo **Jogar**. Cada comando segue o formato `p <letra> <orientacao#> <linha> <coluna>`, onde:

- `<letra>` — símbolo da peça.
- `<orientacao#>` — índice da orientação (use `o <letra>` para listá-las).
- `<linha> <coluna>` — posição da célula âncora `(0,0)` da orientação escolhida.

Ao final da sequência, o tabuleiro estará totalmente coberto e o jogo exibirá:

```
Tabuleiro completo — você venceu!
```

### Vitória e fim de jogo

- O jogo termina automaticamente em **vitória** quando todas as 12 peças são usadas **ou** quando não há mais células vazias.
- Cada peça pode ser usada **no máximo uma vez**.
- Tentativas inválidas (fora dos limites, sobreposição, peça já usada, índice de orientação inválido) são rejeitadas com mensagem de erro, sem alterar o tabuleiro.

---

## Modo Resolver

Roda automaticamente um algoritmo de busca:

- **DFS** (busca em profundidade) — pode opcionalmente **enumerar todas as soluções**.
- **BFS** (busca em largura) — encontra uma solução.

Ao final são exibidos:

- Número de soluções encontradas
- Estados explorados
- Tempo total
- Até 5 soluções renderizadas

---

## As 12 peças (formato base)

Abaixo o formato canônico de cada pentaminó, conforme definido em [Models/Pentomino.cs](Models/Pentomino.cs#L20-L33). Cada peça possui várias orientações geradas por rotações de 90° e reflexão; o número de orientações distintas varia por peça.

### F — 8 orientações

```
. F F
F F .
. F .
```

### I — 2 orientações

```
I I I I I
```

### L — 8 orientações

```
L .
L .
L .
L L
```

### N — 8 orientações

```
. N
. N
N N
N .
```

### P — 8 orientações

```
P P
P P
P .
```

### T — 4 orientações

```
T T T
. T .
. T .
```

### U — 4 orientações

```
U . U
U U U
```

### V — 4 orientações

```
V . .
V . .
V V V
```

### W — 4 orientações

```
W . .
W W .
. W W
```

### X — 1 orientação

```
. X .
X X X
. X .
```

### Y — 8 orientações

```
. Y
Y Y
. Y
. Y
```

### Z — 4 orientações

```
Z Z .
. Z .
. Z Z
```

> **Total de orientações distintas:** 8+2+8+8+8+4+4+4+4+1+8+4 = **63**.
>
> Para listar dinamicamente as orientações de qualquer peça durante uma partida, use o comando `o <letra>` no modo Jogar.

---

## Estrutura do projeto

| Caminho | Descrição |
|---------|-----------|
| [Program.cs](Program.cs) | Menu principal (Resolver / Jogar). |
| [Game/GameEngine.cs](Game/GameEngine.cs) | Loop interativo do modo Jogar. |
| [Models/Pentomino.cs](Models/Pentomino.cs) | Definição das 12 peças e geração das orientações. |
| [Models/Tabuleiro.cs](Models/Tabuleiro.cs) | Matriz do tabuleiro, validações e renderização. |
| [Models/Estado.cs](Models/Estado.cs) | Estado de busca (tabuleiro + peças usadas). |
| [Search/GrafoDeBusca.cs](Search/GrafoDeBusca.cs) | Algoritmos DFS e BFS. |
| [DataStructures/ArvoreAVL.cs](DataStructures/ArvoreAVL.cs) | Estrutura auxiliar (AVL) para controle de estados. |

---

## Dicas para jogar

- Comece pelas peças menos flexíveis (ex.: `X`, `I`) ou pelos cantos do tabuleiro.
- Use `o <letra>` antes de posicionar para escolher a orientação correta.
- Lembre-se que `(linha, coluna)` é a posição da **célula âncora** `(0,0)` da orientação; demais células são deslocamentos relativos.
- Se ficar sem jogadas válidas antes de completar o tabuleiro, use `q` para sair e tentar novamente.

Bom jogo!
