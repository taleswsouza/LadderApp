using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public enum CodigosInterpretaveis
    {
        /// <summary>
        /// Indica que nenhum simbolo esta desenhado
        /// </summary>
        NENHUM = 0,
        /// <summary>
        /// Usado para indicar o inicio da linha.
        /// </summary>
        INICIO_DA_LINHA = 1,
        /// <summary>
        /// Usado para indicar o fim da linha. Codigos interpretaveis antes deste pertencem a linha atual
        /// </summary>
        FIM_DA_LINHA = 2,
        /// <summary>
        /// Usado para desenhar a linha de fundo
        /// </summary>
        LINHA_DE_FUNDO = 3,
        /// <summary>
        /// Indica o estado digital direto de um endereco
        /// </summary>
        CONTATO_NA = 10,
        /// <summary>
        /// Indica o estado digital INVERTIDO de um endereco
        /// </summary>
        CONTATO_NF = 11,
        /// <summary>
        /// Usado para atribuir um estado digital simbLadder um endereco de saida ou de memoria
        /// </summary>
        BOBINA_SAIDA = 12,
        /// <summary>
        /// Usado para temporizar uma condicao ou evento
        /// </summary>
        TEMPORIZADOR = 13,
        /// <summary>
        /// Usado para contar condicoes ou eventos
        /// </summary>
        CONTADOR = 14,
        /// <summary>
        /// Paralelo Inicial - marca o inicio do paralelo. S� pode haver um para cada paralelo
        ///     � o primeiro c�digo dos tr�s 
        /// </summary>
        PARALELO_INICIAL = 15,
        /// <summary>
        /// Paralelo FINAL - marca o fim do paralelo. S� pode haver um para cada paralelo 
        ///     � o primeiro c�digo dos tr�s 
        /// </summary>
        PARALELO_FINAL = 16,
        /// <summary>
        /// Paralelo Pr�ximo - fica entre o paralelo inicial e final. 
        ///     Pode haver mais de um para cada paralelo (se necessario)
        /// </summary>
        PARALELO_PROXIMO = 17,
        /// <summary>
        /// RESET/CLEAR - Para zerar acumulados de contadores e temporizadores
        /// </summary>
        RESET = 18,
        /// <summary>
        /// CABE�ALHO/TAMANHO - Para indicar no c�digos interpretavel o tamanho do cabecalho
        /// </summary>
        CABECALHO_TAMANHO = 100,
        /// <summary>
        /// CABE�ALHO/ID DISPOSITIVO - Para indicar o n�mero do dispositivo usado no programa ladder
        /// </summary>
        CABECALHO_IDDISPOSITIVO = 105,
        /// <summary>
        /// CABE�ALHO/PROTEGIDO POR SENHA - Para indicar que a leitura do programa ladder deve ser feita
        /// ap�s uma autentifica��o por senha
        /// </summary>
        CABECALHO_SENHA_0 = 110,
        /// <summary>
        /// CABE�ALHO/REGISTRO DE ENDERE�AMENTO - Para identificar quantos endere�os de cada tipo foi utilizado
        /// no programa ladder
        /// </summary>
        CABECALHO_REGISTRO_ENDERECO = 120,
    }
}
