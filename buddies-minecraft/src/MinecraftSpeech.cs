using System;
namespace BuddyMinecraft {
public static class MinecraftSpeech {
static readonly string[][][] Lines = {
new string[][] {
new string[] { "AMOSTRA ESPACIAL COLETADA!", "MINÉRIO NA CARGA!", "EXPLORAÇÃO RENDENDO!" },
new string[] { "ACHADO DE OUTRO PLANETA!", "OLHA O BRILHO DISSO!", "RARIDADE EM ÓRBITA!" },
new string[] { "CONQUISTA EM ÓRBITA!", "UM BLOCO PRA HISTÓRIA!", "EXPLORAÇÃO DE SUCESSO!" },
new string[] { "SINAL PERDIDO NO MUNDO.", "FICOU UM VAZIO AQUI...", "ESSA JORNADA DOEU." },
new string[] { "ABRINDO O PROJETO LUNAR.", "VAMOS PLANEJAR ESSA BASE!", "UM BLOCO PRA DECOLAR." },
},
new string[][] {
new string[] { "MINÉRIO NA PATINHA!", "ESSE BRILHA, MIAU!", "MAIS UM PRO NOSSO BAÚ!" },
new string[] { "ATÉ ARREGALEI OS OLHOS!", "BRILHOU ATÉ NO BIGODE!", "MINHA PATINHA DEU SORTE!" },
new string[] { "CONQUISTA, MIAU!", "PATINHA NESSA MEDALHA!", "ESSE MUNDO É UM NOVELO!" },
new string[] { "CADÊ MINHAS NOVE VIDAS?", "MEU BIGODE ATÉ CAIU...", "QUERIA TE DAR UM COLO." },
new string[] { "PATINHA FIRME NO LÁPIS.", "UMA CASINHA PRA COCHILAR?", "VOU RISCAR ESSE PROJETO!" },
},
new string[][] {
new string[] { "MINÉRIO CATALOGADO!", "RECURSO ADICIONADO!", "COLETA EFICIENTE!" },
new string[] { "RARIDADE DETECTADA!", "MEU LED ATÉ BRILHOU!", "VALOR ACIMA DO ESPERADO!" },
new string[] { "CONQUISTA REGISTRADA!", "PROGRESSO EM BLOCOS!", "OBJETIVO COMPILADO!" },
new string[] { "EXPLORADOR SEM SINAL.", "MEUS CIRCUITOS SENTIRAM.", "ERRO: SAUDADE DO PARCEIRO." },
new string[] { "CARREGANDO A PLANTA...", "RÉGUA VIRTUAL ATIVADA.", "PROJETO EM PROCESSAMENTO." },
},
new string[][] {
new string[] { "MAIS UM PRO TESOURO!", "GUARDA NA NOSSA PILHA!", "ESSA COLETA ESQUENTOU!" },
new string[] { "MINHAS ESCAMAS BRILHARAM!", "ISSO MERECE UM COFRE!", "QUE TESOURO É ESSE?!" },
new string[] { "CONQUISTA PRO TESOURO!", "ATÉ O ENDER IA APLAUDIR!", "UMA LENDA EM BLOCOS!" },
new string[] { "MINHA BRASA FICOU FRIA.", "NEM MINHA ASA TE SALVOU.", "DOEU MAIS QUE ESPADA." },
new string[] { "PLANEJANDO NOSSO CASTELO.", "LÁPIS LONGE DO MEU FOGO.", "CABEM ASAS NESSE PROJETO?" },
},
new string[][] {
new string[] { "COLETA COM UM TRUQUE!", "MEU AMULETO TÁ EM DIA.", "MINÉRIO NA NOSSA BOLSA!" },
new string[] { "AS TRÊS CAUDAS PARARAM!", "PARECE ATÉ FEITIÇO!", "OLHA O QUE A SORTE TROUXE!" },
new string[] { "CONQUISTA ENCANTADA!", "UM NOVO TRUQUE EM BLOCOS!", "MAIS UMA LENDA NO LIVRO!" },
new string[] { "MEU AMULETO RACHOU.", "AS CAUDAS SENTIRAM...", "NEM A MAGIA SEGUROU." },
new string[] { "DESENHANDO NOSSO REFÚGIO.", "UM TRAÇO DE MAGIA AQUI.", "TRÊS CAUDAS, UM PROJETO." },
},
new string[][] {
new string[] { "CHOVEU MINÉRIO!", "COLETA DE CÉU ABERTO!", "MAIS RECURSOS NO RADAR!" },
new string[] { "ABRIU UM SOL AQUI!", "QUE RAIO DE SORTE!", "ESSE ACHADO É UM CLARÃO!" },
new string[] { "CONQUISTA DE CÉU ABERTO!", "UM ARCO-ÍRIS EM BLOCOS!", "CHUVA DE CONQUISTAS!" },
new string[] { "CHOVEU NO NOSSO MUNDO.", "O CÉU FICOU QUIETINHO.", "ATÉ MEU RAIO SE APAGOU." },
new string[] { "VOU DESENHAR UM TELHADO.", "PROJETO À PROVA DE CHUVA!", "UM TRAÇO, UMA NUVEM." },
},
new string[][] {
new string[] { "COLETA NO COMPASSO!", "MINÉRIO NO NOSSO SET!", "MAIS UM NO RITMO!" },
new string[] { "PAREI ATÉ O BEAT!", "ESSE ACHADO VIROU HIT!", "OITO BRAÇOS EM CHOQUE!" },
new string[] { "CONQUISTA NO PLAY!", "ESSE BLOCO VIROU HIT!", "MAIS UMA FAIXA NA LENDA!" },
new string[] { "O MUNDO FICOU SEM SOM.", "PAUSA TRISTE NO SET...", "MEUS OITO BRAÇOS CAÍRAM." },
new string[] { "DESENHANDO NOSSO PALCO.", "O LÁPIS TÁ NO RITMO!", "OITO BRAÇOS NA OBRA!" },
},
};
static readonly string[][] Rare = {
new string[] { "DIAMANTE ESTELAR!", "ESMERALDA EM ÓRBITA!", "DETRITOS DE OUTRO MUNDO!" },
new string[] { "DIAMANTE, MIAU!", "ESMERALDA COR DE MOCHI!", "DETRITOS? QUE ACHADO!" },
new string[] { "DIAMANTE CONFIRMADO!", "ESMERALDA IDENTIFICADA!", "DETRITOS CATALOGADOS!" },
new string[] { "DIAMANTE PRO TESOURO!", "ESMERALDA DE DRAGÃO!", "TESOURO DO NETHER!" },
new string[] { "DIAMANTE ENCANTADO!", "ESMERALDA DA SORTE!", "UM ACHADO ANCESTRAL!" },
new string[] { "CHUVA DE DIAMANTES!", "ESMERALDA NO RADAR!", "DETRITOS NA PREVISÃO!" },
new string[] { "DIAMANTE NO DROP!", "ESMERALDA NA PLAYLIST!", "DETRITOS VIRARAM HIT!" },
};
public static string Phrase(PetKind kind, string eventName, int variant, string itemId="") {
if(!Enum.IsDefined(typeof(PetKind),kind)) return "";
int index=Array.IndexOf(new[] {"minerio_obtido","minerio_raro","conquista","morte","bancada_aberta"},eventName);
if(index<0) return "";
int v=((variant%3)+3)%3;
if(index==1 && v==0) {
 int ore=MineralRules.Family(itemId);
 if(ore>=0) return Rare[(int)kind][ore];
}
return Lines[(int)kind][index][v];
}
} }
