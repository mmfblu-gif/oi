# Buddies para Minecraft Java

Conteúdo independente para **Astro, Mochi, Bolt, Drako, Kitsu, Nimbo e Marina**. Não substitui o aplicativo ou as animações de vitória/derrota existentes.

[Baixar o pacote ZIP](https://github.com/mmfblu-gif/oi/raw/refs/heads/main/buddies-minecraft.zip).

- `PREVIA-MINECRAFT.html`: prévia animada em arquivo único; abra no Chrome, Edge ou Firefox atualizado, mesmo offline.
- `COLECAO-MINECRAFT.png`: painel dos sete personagens construindo e impressionados.
- `FALAS.txt` / `falas.json`: 105 falas de evento, mais 21 específicas para diamante, esmeralda e detritos ancestrais.
- `sprites/` / `animacoes.json`: novos quadros transparentes normais/suaves e instruções de reprodução.
- `src/`: arte, falas e controle de eventos em C# 5, com namespace próprio `BuddyMinecraft`.

## O que acontece

| Evento confirmado do jogador local | Reação | Falas |
| --- | --- | --- |
| Obter minério comum | `Goal` existente | Três por personagem |
| Obter minério raro | Nova `Impressed` | Três opções, com uma específica para o minério |
| Desbloquear conquista/avanço | `Victory` existente | Três por personagem |
| Morrer | `Defeat` existente | Três por personagem |
| Abrir a bancada 3 × 3 | Nova `CraftingIntro` → `Crafting` | Três convites, um a cada abertura |
| Fechar a bancada | Interrompe a construção | Sem fala extra |

**Construção:** o buddy traz uma planta azul, segura a folha e move um lápis sobre o desenho de uma casa. Cada um tem seu detalhe: estrela, pegada, régua, garras, folha, nuvem ou nota musical. A expressão acompanha o lápis.

**Impressionado:** olhos arregalados, boca surpresa, mão no rosto, elevação suave e brilhos. O achado muda entre diamante, esmeralda e detritos ancestrais. A animação toca uma vez e termina em repouso. O modo suave reduz a amplitude, sem flashes.

## Viabilidade e captura no Java

A arte e o controle estão implementados. **Este pacote não contém um mod Fabric/Forge/NeoForge, plugin de servidor ou conector que capture eventos dentro do Minecraft.** A versão e o loader do jogo ainda não foram informados. Seu desenvolvedor conecta os sinais ao módulo; a prévia é uma simulação, não evidência de funcionamento no jogo.

Uma integração Java pode fornecer obtenção de itens, avanços novos e abertura/fechamento da tela de bancada. Também pode fornecer fabricação de itens, morte, dano, mudança de bioma/dimensão e chefes, conforme os eventos disponíveis no conector. Esses eventos adicionais não estão implementados neste pacote.

Cuidados para a captura:

- Dispare `ObtainMineral` após obtenção real do item pelo jogador local. Quebrar o bloco sem coletar o drop não significa obter o minério.
- Não trate o inventário inicial, organização de slots, transferência do mesmo item entre slots ou eventos de outros jogadores como coleta nova.
- Com Fortune ou vários itens no mesmo pickup, envie um evento com `count > 0`; não um evento por unidade.
- Envie apenas avanços visíveis recém-concluídos. Ignore receitas/avanços internos sem exibição e conquistas antigas recebidas ao entrar no mundo.
- Abra a rotina quando a tela da bancada 3 × 3 realmente aparecer. Clicar no bloco não garante abertura; a grade 2 × 2 do inventário não é uma bancada.
- Use `SetCrafting(false, now)` ao fechar/trocar essa tela, e `Reset()` ao sair do mundo ou perder a conexão.
- Envie eventos por APIs do conector/mod autorizado. Não leia memória do jogo nem injete processos. Se houver transporte local, mantenha autenticação e validação de origem.

## Minérios

Raros por padrão: `minecraft:diamond`, `minecraft:emerald`, `minecraft:ancient_debris`, e os blocos de minério de diamante/esmeralda, inclusive deepslate (Silk Touch).

Comuns: carvão, ferro, cobre, ouro, lápis-lazúli, redstone, quartzo, ametista e os blocos correspondentes listados em `MineralRules`. A classificação é uma escolha de conteúdo editável. Esmeraldas recebidas por comércio só devem disparar se o conector deliberadamente tratar isso como obtenção; não diga que foram mineradas.

Não existe `minecraft:netherite_ore` no catálogo. O material raro natural do Nether é `minecraft:ancient_debris` (detritos ancestrais).

## Usar o módulo C#

O namespace e os enums são independentes. Mapeie os nomes de personagens para o catálogo do aplicativo, sem substituir os enums existentes.

O projeto `.csproj` usa .NET 8 apenas para a validação portátil. Para o aplicativo WPF em .NET Framework 4.8, incorpore os arquivos C# 5 de `src/` ao projeto existente; não referencie uma DLL compilada para .NET 8.

```csharp
var session = new BuddyMinecraft.MinecraftSession();
session.SetCrafting(true, 10.0);
session.ObtainMineral("mundo1:pickup:42", "minecraft:diamond", 2, 12.0);
var feedback = session.Snapshot(BuddyMinecraft.PetKind.Kitsu, 12.5);
// feedback.Animation == "Impressed"
// feedback.Speech == "DIAMANTE ENCANTADO!"
```

`now` é um relógio monotônico em segundos do aplicativo, não um timestamp arbitrário do evento remoto. IDs devem ser únicos por ocorrência e por sessão de mundo, não apenas o ID do item/avanço. O módulo guarda os últimos 256 IDs para evitar duplicações; a captura deve também manter uma base limpa ao conectar.

`Snapshot` fornece animação, fala, item, tempo, variante e revelação. `Goal`, `Victory` e `Defeat` continuam indo para os renderers existentes. Para `Crafting` e `Impressed`, use os PNGs ou `MinecraftBuddyArt.Draw`.

```csharp
var canvas = new BuddyMinecraft.WpfPremiumCanvas(drawingContext);
BuddyMinecraft.MinecraftBuddyArt.Draw(canvas,
    BuddyMinecraft.PetKind.Kitsu,
    BuddyMinecraft.MinecraftPose.Impressed,
    feedback.Seconds, 0.3,
    BuddyMinecraft.MineralVisual.Diamond);
```

O adaptador WPF requer as assemblies WPF no Windows. O canvas lógico tem 260 × 260; escala e posicionamento ficam com o renderer hospedeiro. Na construção, passe `feedback.Reveal` como último argumento para trazer a planta ao abrir. Escolha `MineralVisual` pela família de `feedback.ItemId`. O núcleo vetorial pode ser desenhado na taxa de quadros do app, sem ficar limitado aos PNGs.

### Prioridade e duração

Morte > conquista > raro > comum. Morte cancela construção e esvazia a fila. Conquistas e raros podem interromper reações mais fracas; a reação interrompida não reinicia. Eventos distintos de mesma prioridade ou menor podem aguardar em fila (até oito), expirando após 12 segundos. Durante a reação de morte, novas coletas/conquistas são descartadas. A construção volta após a reação apenas se a bancada continuou aberta.

Minério comum: intervalo mínimo de cinco segundos e sem interrupção de reação ativa. Raro: intervalo mínimo de 4,2 segundos. Isso evita falas em cascata; nem cada unidade nem cada pickup rápido receberá uma fala própria. A fala de construção aparece nos primeiros cinco segundos de cada abertura, sem repetir a cada volta do lápis.

`MinecraftSession` deve ser acessado na thread de UI ou serializado pelo host. Ele não abre conexões e não usa temporizadores próprios. Cabe ao aplicativo ocultar o balão conforme a duração existente e desenhar novamente a cada quadro.

## PNGs transparentes

São 70 folhas: sete buddies × cinco clipes × normal/suave. Cada quadro mede 160 × 160, em oito colunas, da esquerda para a direita e depois de cima para baixo.

- `CraftingIntro`: 16 quadros em 0,6 s, uma vez. Depois passe a `Crafting`.
- `Crafting`: 64 quadros em 6 s, em loop enquanto a bancada estiver aberta. Começa no mesmo gesto final da introdução.
- `ImpressedDiamond`, `ImpressedEmerald`, `ImpressedDebris`: 64 quadros em 4,2 s, uma vez. São a mesma coreografia com achados distintos.

Clipes únicos: `floor(min(1, segundos/duração) * (quadros - 1))`.
Loop: `floor((segundos % duração)/duração * quadros)`.

No loop de construção, conte o tempo a partir do fim da introdução (`max(0, feedback.Seconds - 0.6)`). Trocar personagem/modo suave não deve reiniciar o relógio. Ao fechar a bancada, pare o loop imediatamente. Use o manifesto para os caminhos e tamanhos; estas folhas não têm o layout dos atlases antigos do aplicativo.

## Testes e limites

O núcleo foi compilado com C# 5 no .NET SDK 8, sem pacotes NuGet. Para repetir os testes portáteis:

```sh
dotnet run --project review/Review.csproj -- --test
```

Para regenerar os quadros vetoriais da revisão:

```sh
dotnet run --project review/Review.csproj -- review
```

Passaram 83 verificações de falas, eventos e arte, com 6.048 amostras de desenho. Os PNGs, a prévia offline e o ZIP também foram conferidos. **WPF nativo, o aplicativo completo e a captura de eventos em Minecraft Java real ainda precisam de validação no PC de vocês.** Não houve alteração nas animações ou nas falas de CS e Rocket League.
