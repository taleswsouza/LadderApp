using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public enum OperationCode
    {
        /// <summary>
        /// Indica que nenhum simbolo esta desenhado
        /// </summary>
        None = 0,
        /// <summary>
        /// Usado para indicar o inicio da linha.
        /// </summary>
        LineBegin = 1,
        /// <summary>
        /// Usado para indicar o fim da linha. Codigos interpretaveis antes deste pertencem a linha atual
        /// </summary>
        LineEnd = 2,
        /// <summary>
        /// Usado para desenhar a linha de fundo
        /// </summary>
        BackgroundLine = 3,
        /// <summary>
        /// Indica o estado digital direto de um endereco
        /// </summary>
        NormallyOpenContact = 10,
        /// <summary>
        /// Indica o estado digital INVERTIDO de um endereco
        /// </summary>
        NormallyClosedContact = 11,
        /// <summary>
        /// Usado para atribuir um estado digital simbLadder um endereco de saida ou de memoria
        /// </summary>
        OutputCoil = 12,
        /// <summary>
        /// Usado para temporizar uma condicao ou evento
        /// </summary>
        Timer = 13,
        /// <summary>
        /// Usado para contar condicoes ou eventos
        /// </summary>
        Counter = 14,
        /// <summary>
        /// Paralelo Inicial - marca o inicio do paralelo. Só pode haver um para cada paralelo
        ///     É o primeiro código dos três 
        /// </summary>
        ParallelBranchBegin = 15,
        /// <summary>
        /// Paralelo FINAL - marca o fim do paralelo. Só pode haver um para cada paralelo 
        ///     É o primeiro código dos três 
        /// </summary>
        ParallelBranchEnd = 16,
        /// <summary>
        /// Paralelo Próximo - fica entre o paralelo inicial e final. 
        ///     Pode haver mais de um para cada paralelo (se necessario)
        /// </summary>
        ParallelBranchNext = 17,
        /// <summary>
        /// RESET/CLEAR - Para zerar acumulados de contadores e temporizadores
        /// </summary>
        Reset = 18,
        /// <summary>
        /// CABEÇALHO/TAMANHO - Para indicar no códigos interpretavel o tamanho do cabecalho
        /// </summary>
        HeadLenght = 100,
        /// <summary>
        /// CABEÇALHO/ID DISPOSITIVO - Para indicar o número do dispositivo usado no programa ladder
        /// </summary>
        HeadDeviceId = 105,
        /// <summary>
        /// CABEÇALHO/PROTEGIDO POR SENHA - Para indicar que a leitura do programa ladder deve ser feita
        /// após uma autentificação por senha
        /// </summary>
        HeadPassword0 = 110,
        /// <summary>
        /// CABEÇALHO/REGISTRO DE ENDEREÇAMENTO - Para identificar quantos endereços de cada tipo foi utilizado
        /// no programa ladder
        /// </summary>
        HeadAddressingRecords = 120,
    }
}
