﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LadderApp.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class MicrocontrollersBaseCodeFilesResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MicrocontrollersBaseCodeFilesResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LadderApp.Resources.MicrocontrollersBaseCodeFilesResource", typeof(MicrocontrollersBaseCodeFilesResource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //====================================================================================
        ///// Arquivo de definição geral
        ///// Autor: Silvano Fonseca Paganoto
        ///// Data: 15/09/2010
        /////====================================================================================
        ///
        ///
        ///#ifndef DEFINICAO_H_
        ///#define DEFINICAO_H_
        ///#define RECARGA_CCR0 10000
        /////====================================================================================
        ///// Includes
        /////===================================================================== [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string definicaoH {
            get {
                return ResourceManager.GetString("definicaoH", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //====================================================================================
        ///// Arquivo da Lista endereçso do usados no microcontrolador
        ///// Autor: Silvano Fonseca Paganoto
        ///// Data: 15/09/2010
        /////====================================================================================
        ///#ifndef ENDERECOS_H_
        ///#define ENDERECOS_H_
        ///
        /////====================================================================================
        ///// Includes
        /////===================================================================== [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string enderecosH {
            get {
                return ResourceManager.GetString("enderecosH", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //====================================================================================
        ///// Rotina de execução do contador
        /////====================================================================================
        ///void ExecContador(TContador *Contador)
        ///{
        ///	switch (Contador-&gt;Tipo)
        ///	{
        ///#EXEC_COUNTER_TYPE_0_FUNCTION_C#
        ///#EXEC_COUNTER_TYPE_1_FUNCTION_C#
        ///	default:
        ///		break;
        ///	}
        ///	if (Contador-&gt;EN == 0)
        ///		Contador-&gt;Pulso = 1;
        ///}
        ///.
        /// </summary>
        internal static string ExecContador_funcoesC {
            get {
                return ResourceManager.GetString("ExecContador_funcoesC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to void ExecContador(TContador *Contador);.
        /// </summary>
        internal static string ExecContador_funcoesH {
            get {
                return ResourceManager.GetString("ExecContador_funcoesH", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 	case 0: // Contador Crescente
        ///		if (Contador-&gt;Reset == 1)
        ///		{
        ///			Contador-&gt;DN = 0;
        ///			Contador-&gt;Acumulado = 0;
        ///			Contador-&gt;Reset = 0;
        ///		}
        ///		if (Contador-&gt;EN == 1 &amp;&amp; Contador-&gt;Pulso == 1)
        ///		{
        ///			Contador-&gt;Pulso = 0;
        ///			if (Contador-&gt;Acumulado &lt;= 255)
        ///			{
        ///				Contador-&gt;Acumulado++;
        ///				if (Contador-&gt;Acumulado &gt;= Contador-&gt;Preset)
        ///					Contador-&gt;DN = 1;
        ///				else
        ///					Contador-&gt;DN = 0;
        ///			}
        ///		}
        ///		break;
        ///.
        /// </summary>
        internal static string ExecContador_Tipo0_funcoesC {
            get {
                return ResourceManager.GetString("ExecContador_Tipo0_funcoesC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 	case 1: // Contador Decrescente
        ///		if (Contador-&gt;Reset == 1)
        ///		{
        ///			Contador-&gt;Acumulado = Contador-&gt;Preset;
        ///		}
        ///		if (Contador-&gt;EN == 1 &amp;&amp; Contador-&gt;Pulso == 1)
        ///		{
        ///			Contador-&gt;Pulso = 0;
        ///			if (Contador-&gt;Acumulado &gt; 0)
        ///			{
        ///				Contador-&gt;Acumulado--;
        ///				if (Contador-&gt;Acumulado == 0)
        ///					Contador-&gt;DN = 1;
        ///
        ///				else
        ///					Contador-&gt;DN = 0;
        ///			}
        ///		}
        ///		break;
        ///.
        /// </summary>
        internal static string ExecContador_Tipo1_funcoesC {
            get {
                return ResourceManager.GetString("ExecContador_Tipo1_funcoesC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //====================================================================================
        ///// Rotina de execução do temporizador
        /////====================================================================================
        ///void ExecTemporizador(TTemporizador *Temporizador)
        ///{
        ///	char EventoPresente = 0;
        ///	switch (Temporizador-&gt;Tipo)
        ///	{
        /////-----------------------------------------------------------------------
        ///#EXEC_TIMER_TYPE_0_FUNCTION_C#
        /////-----------------------------------------------------------------------        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ExecTemporizador_funcoesC {
            get {
                return ResourceManager.GetString("ExecTemporizador_funcoesC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to void ExecTemporizador(TTemporizador *Temporizador);
        ///.
        /// </summary>
        internal static string ExecTemporizador_funcoesH {
            get {
                return ResourceManager.GetString("ExecTemporizador_funcoesH", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 	case 0:// TON ====================================================================
        ///
        ///		if ((Temporizador-&gt;EN) &amp;&amp; (!Temporizador-&gt;Reset))
        ///		{
        ///			//-------------------------------------------------------
        ///			if (Sistema.Evento10ms == 1)
        ///			{
        ///				(Temporizador-&gt;Parcial)++;
        ///				switch (Temporizador-&gt;Base)
        ///				{
        ///				case 0:
        ///					EventoPresente = 1;
        ///					break;
        ///				case 1:
        ///					if ((Temporizador-&gt;Parcial) &gt;= 10)
        ///						EventoPresente = 1;
        ///					break;
        ///				case 2:
        ///					if ((Temporizador-&gt;Parcial) &gt;= [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ExecTemporizador_Tipo0_funcoes {
            get {
                return ResourceManager.GetString("ExecTemporizador_Tipo0_funcoes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 	case 1: // TOFF
        ///		if (Temporizador-&gt;EN)
        ///		{
        ///			Temporizador-&gt;Acumulado = Temporizador-&gt;Preset;
        ///			Temporizador-&gt;DN = 1;
        ///		}
        ///		else
        ///		{
        ///			if (Sistema.Evento10ms)
        ///			{
        ///				(Temporizador-&gt;Parcial)++;
        ///				switch (Temporizador-&gt;Base)
        ///				{
        ///				case 0:
        ///					EventoPresente = 1;
        ///					break;
        ///				case 1:
        ///					if ((Temporizador-&gt;Parcial) &gt;= 10)
        ///						EventoPresente = 1;
        ///					break;
        ///				case 2:
        ///					if ((Temporizador-&gt;Parcial) &gt;= 100)
        ///						EventoPresente = 1;
        ///					break;
        ///				case 3:
        ///					if  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ExecTemporizador_Tipo1_funcoes {
            get {
                return ResourceManager.GetString("ExecTemporizador_Tipo1_funcoes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //====================================================================================
        ///// Rotina de execução do temporizador
        /////====================================================================================
        ///void ExecTemporizadores(void)
        ///{
        ///#TIMERS_USER_C#
        ///}.
        /// </summary>
        internal static string ExecTemporizadores_usuarioC {
            get {
                return ResourceManager.GetString("ExecTemporizadores_usuarioC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to void ExecTemporizadores(void);.
        /// </summary>
        internal static string ExecTemporizadores_usuarioH {
            get {
                return ResourceManager.GetString("ExecTemporizadores_usuarioH", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //====================================================================================
        ///// Arquivo de funções do sistema
        ///// Autor: Silvano Fonseca Paganoto
        ///// Data: 15/09/2010
        /////====================================================================================/*
        ///
        /////====================================================================================
        ///// Includes
        /////====================================================================================
        ///#include &quot;funcoes.h&quot;
        ///
        ///#EXEC_TIMER_FUNCTION_C#
        ///
        ///# [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string funcoesC {
            get {
                return ResourceManager.GetString("funcoesC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //====================================================================================
        ///// Arquivo de prototipo das funções do sistema
        ///// Autor: Silvano Fonseca Paganoto
        ///// Data: 15/09/2010
        /////====================================================================================
        ///#ifndef FUNCOES_H_
        ///#define FUNCOES_H_
        /////====================================================================================
        ///// Includes
        /////====================================================================================
        ///#i [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string funcoesH {
            get {
                return ResourceManager.GetString("funcoesH", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //====================================================================================
        ///// Arquivo de interrupções do sistema
        ///// Autor: Silvano Fonseca Paganoto
        ///// Data: 15/09/2010
        /////====================================================================================/*
        ///
        /////====================================================================================
        ///// Includes
        /////====================================================================================
        ///#include &quot;ladderprogram.h&quot;
        ///
        ///unsigned char Acc [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string interrupcaoC {
            get {
                return ResourceManager.GetString("interrupcaoC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //====================================================================================
        ///// Arquivo da rotina do usuário
        ///// Autor: Silvano Fonseca Paganoto
        ///// Data: 15/09/2010
        /////====================================================================================
        ///
        /////====================================================================================
        ///// Includes
        /////====================================================================================
        ///#include &quot;ladderprogram.h&quot;
        ///
        /////======================= [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ladderprogramC {
            get {
                return ResourceManager.GetString("ladderprogramC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //====================================================================================
        ///// Arquivo de protótipo das funções do programa do usuario
        ///// Autor: Silvano Fonseca Paganoto
        ///// Data: 15/09/2010
        /////====================================================================================
        ///
        ///#ifndef USUARIO_H_
        ///#define USUARIO_H_
        ///
        /////====================================================================================
        ///// Includes
        /////======================================================================== [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ladderprogramH {
            get {
                return ResourceManager.GetString("ladderprogramH", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //====================================================================================
        ///// Arquivo principal
        ///// Autor: Silvano Fonseca Paganoto
        ///// Data: 15/09/2010
        /////====================================================================================
        ///
        /////====================================================================================
        ///// Includes
        /////====================================================================================
        ///#include &quot;definicao.h&quot;
        ///#include &quot;setuphardware.h&quot;
        ///#include &quot;ladd [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string mainC {
            get {
                return ResourceManager.GetString("mainC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //====================================================================================
        ///// Arquivo de configuração do hardware
        ///// Autor: Silvano Fonseca Paganoto
        ///// Data: 15/09/2010
        /////====================================================================================
        ///
        /////====================================================================================
        ///// Includes
        /////====================================================================================
        ///#include &quot;setuphardware.h&quot;
        ///
        /////================ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string setupHardwareC {
            get {
                return ResourceManager.GetString("setupHardwareC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //====================================================================================
        ///// Arquivo de protótipo das funções para configuração de hardware
        ///// Autor: Silvano Fonseca Paganoto
        ///// Data: 15/09/2010
        /////====================================================================================
        ///
        ///#ifndef SETUPHARDWARE_H_
        ///#define SETUPHARDWARE_H_
        ///
        /////====================================================================================
        ///// Includes
        /////===================================================== [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string setupHardwareH {
            get {
                return ResourceManager.GetString("setupHardwareH", resourceCulture);
            }
        }
    }
}
