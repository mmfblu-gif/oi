# Falas para CS e Minecraft

Somente textos para **Astro, Mochi, Bolt, Drako, Kitsu, Nimbo e Marina**. Cada personagem tem três falas por evento, com vocabulário próprio: exploração espacial, gato, robô, dragão, magia, clima e música. Total: **126 falas**.

- `FALAS.txt`: todas as falas para ler, copiar e colar.
- `falas.json`: conteúdo em UTF-8, organizado por buddy, jogo e evento.

## Reutilização das animações

| Jogo | Evento | Animação existente |
| --- | --- | --- |
| CS/CS2 | Abate de outro jogador | `Goal` — comemoração |
| CS/CS2 | Morte do jogador local | `Conceded` — tristeza |
| CS/CS2 | Vitória | `Victory` |
| CS/CS2 | Derrota | `Defeat` |
| Minecraft | Conquista / avanço | `Victory` |
| Minecraft | Morte do jogador local | `Defeat` |

Os nomes Goal e Conceded são apenas os IDs de animação já existentes. Nenhuma fala nova trata o evento como futebol. No Minecraft, conquista aciona apenas Victory e morte apenas Defeat: não dispare duas animações para o mesmo evento.

## Para integrar

Leia `buddies[nome][jogo][evento]` no JSON e escolha uma das três falas. Use o mapa `mapeamento_animacoes` para manter as animações atuais. Por exemplo: `buddies.Kitsu.cs.abate[0]` retorna “ABATE ENCANTADO!”.

Alterne as três opções sem repetir a última. Mantenha o tempo de exibição atual; troque somente o texto. As falas são curtas (até 27 caracteres), mas a largura final depende da fonte e deve ser conferida no balão do aplicativo.

Vitória/derrota no CS acompanham o evento que a integração já fornece (round ou partida); os textos não presumem qual dos dois. As mortes não presumem arma, agressor, causa, perda de itens nem possibilidade de respawn. Os textos também servem ao Minecraft Hardcore sem prometer retorno.

Este pacote não detecta eventos, não altera código do aplicativo e não modifica sprites, animações, falas de outros jogos ou rotinas de desktop. O desenvolvedor faz a ligação com os eventos existentes.
