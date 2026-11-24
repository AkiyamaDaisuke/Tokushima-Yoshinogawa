# Documentação do jogo

## Visão geral

**Tokushima & Yoshinogawa** é um jogo local de estratégia por turnos para duas pessoas. Cada jogador administra uma cidade, cuida da economia e tenta terminar a partida com a maior satisfação da população.

O jogo foi desenvolvido em C# com WPF para Windows. Os recursos e o progresso existem durante a partida aberta; não há salvamento de partidas.

## Objetivo

Ao configurar a partida, escolham a dificuldade e a duração: 5, 7 ou 10 rodadas. Cada jogador realiza um turno em cada rodada. Depois do último turno da partida, vence quem tiver a maior satisfação.

Se a satisfação ficar igual, os critérios de desempate são, nesta ordem:

1. Maior número de soldados.
2. Maior saldo de Yenes.
3. Empate, se os três valores forem iguais.

## Começar uma partida

Na tela **Jogar**, preencham o nome e a cidade de cada jogador. A dificuldade e o número de rodadas são compartilhados pela partida, e as duas cidades começam com a mesma configuração de acordo com a dificuldade.

| Dificuldade | Satisfação inicial | Yenes iniciais | População inicial |
| --- | ---: | ---: | ---: |
| Fácil | 80% | 1.200 | 20 |
| Normal | 65% | 1.000 | 20 |
| Difícil | 50% | 800 | 15 |

Cada cidade começa também com 20 terras, 30 grãos, alíquota de imposto de 10%, nenhuma fazenda, nenhum quartel e nenhum soldado.

## Como funciona um turno

O indicador no alto da tela mostra de quem é a vez e qual rodada está em andamento. Durante o próprio turno, o jogador pode negociar, plantar, construir, recrutar e escolher a alíquota de imposto. Ao terminar, use **Encerrar turno**.

O jogo alterna entre o primeiro e o segundo jogador. Depois que ambos encerram seus turnos, começa a rodada seguinte. Ao encerrar o turno do segundo jogador na última rodada, a tela de resultado mostra o vencedor e os dados finais das cidades.

Os recursos são separados: cada jogador administra o saldo, as terras, os grãos, a população, as construções e os soldados da própria cidade.

## Mercado e economia

O mercado permite comprar e vender grãos e terras. Digite uma quantidade inteira positiva antes de escolher uma operação.

| Recurso | Comprar | Vender |
| --- | ---: | ---: |
| 1 grão | 10 Yenes | 7 Yenes |
| 1 terra | 100 Yenes | 70 Yenes |

Compras exigem saldo suficiente. Vendas exigem estoque suficiente. Terras que estejam plantadas não podem ser vendidas. Uma transação válida atualiza o saldo e o estoque na hora.

## Agricultura e população

Para plantar, informe quantas terras deseja usar e escolha **Plantar**. Cada terra plantada consome 1 grão como semente. A terra fica reservada até o encerramento do turno; nesse momento, a colheita é adicionada ao estoque e a terra volta a ficar livre.

- Uma terra plantada rende 3 grãos.
- Cada fazenda acrescenta 1 grão à produção de cada terra plantada.
- Fórmula da colheita: `terras plantadas × (3 + número de fazendas)`.

No encerramento do turno, a população consome `arredondar para cima(população ÷ 10)` grãos, com consumo mínimo de 1. Se, após a colheita, houver pelo menos 5 grãos além do necessário para o consumo, a população cresce em 1. Se não houver grãos suficientes para alimentar a população, ela diminui em 1 e a satisfação cai 5 pontos. A população nunca fica abaixo de 1.

## Governo e impostos

Escolha uma alíquota de 5%, 10% ou 20%. A arrecadação é aplicada ao encerrar o turno e é calculada pela regra do jogo:

`arrecadação = população × alíquota ÷ 10`

O cálculo usa números inteiros. A alíquota também afeta a satisfação ao final do turno:

- **5%:** aumenta 1 ponto de satisfação.
- **10%:** não altera diretamente a satisfação.
- **20%:** reduz 3 pontos de satisfação.

A satisfação fica entre 0% e 100%. A taxa selecionada permanece como escolha daquele jogador e pode ser alterada em seus próximos turnos.

## Construções e soldados

As construções são compradas com Yenes durante o turno:

| Construção ou unidade | Custo | Efeito |
| --- | ---: | --- |
| Fazenda | 250 Yenes | Cada fazenda adiciona 1 grão por terra plantada em cada colheita. É possível construir várias. |
| Quartel | 400 Yenes | Libera o recrutamento de até 20 soldados. Só é permitido um quartel por cidade. |
| Soldado | 60 Yenes | Adiciona um soldado à cidade; o quartel comporta até 20. |

Ao encerrar o turno, cada soldado custa 2 Yenes de manutenção. O saldo não fica negativo: se a manutenção ultrapassar o saldo disponível após os impostos, ele fica em zero. Soldados também aumentam a satisfação: a partir de 5 soldados, a cidade ganha 1 ponto por turno; com 10 ou mais, ganha 2 pontos. O bônus não ultrapassa 100% de satisfação.

No jogo atual, soldados representam segurança e entram como critério de desempate. Não há combate entre cidades.

## Ordem de processamento ao encerrar o turno

1. Colheita das terras plantadas.
2. Cálculo e aplicação da arrecadação de impostos e da manutenção dos soldados.
3. Ajuste de satisfação por impostos e soldados.
4. Consumo de grãos e possível crescimento ou redução da população.
5. Salvamento dos dados do jogador e passagem do turno ao oponente ou à próxima rodada.

## Executar o projeto

1. Abra `Tokushima-Yoshinogawa_WPF.slnx` no Visual Studio.
2. Restaure os componentes do projeto, se solicitado.
3. Execute com **F5**.

### Requisitos

- Windows 10 ou Windows 11.
- .NET 10 SDK.
- Visual Studio com suporte a desenvolvimento desktop com .NET, ou outro ambiente capaz de compilar WPF.

## Organização do código

- `Tokushima-Yoshinogawa_WPF/MainWindow.xaml`: menu inicial.
- `Tokushima-Yoshinogawa_WPF/Jogando.xaml`: configuração da partida e interface de administração.
- `Tokushima-Yoshinogawa_WPF/Jogando.xaml.cs`: regras do mercado, turnos, agricultura, impostos, construções e vitória.
- `Tokushima-Yoshinogawa_WPF/Models/JogadorPartida.cs`: estado independente de cada jogador.
- `Tokushima-Yoshinogawa_WPF/Models/UserData.cs`: estado atualmente carregado na interface durante o turno.

