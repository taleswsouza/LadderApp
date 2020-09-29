using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using LadderApp.Exceções;
using LadderApp.Resources;

namespace LadderApp
{
    public class MSP430IntegrationServices
    {
        private Process p = new Process();
        private ProcessStartInfo startInfo = new ProcessStartInfo();
        private List<string> lstNomesArquivosCompilados = new List<string>();
        private List<string> lstNomesArquivosCriados = new List<string>();
        private bool bHabilitaDeletarArqIntermediarios = false;
        private String strMMCU = "";

        private String strStandardOutput = "";
        private String strStandardError = "";

        private string RetornaArquivosCompilados()
        {
            String strResult = "";

            foreach (String _strFile in lstNomesArquivosCompilados)
                strResult += _strFile + " ";

            return strResult.Trim();
        }

        public MSP430IntegrationServices()
        {
            p.StartInfo = startInfo;
            SetDefaults();
            lstNomesArquivosCompilados.Clear();
            lstNomesArquivosCriados.Clear();
            strMMCU = "-mmcu=msp430x2013 ";
        }

        public MSP430IntegrationServices(bool bDeletarArquivos)
        {
            p.StartInfo = startInfo;
            SetDefaults();
            lstNomesArquivosCompilados.Clear();
            lstNomesArquivosCriados.Clear();
            bHabilitaDeletarArqIntermediarios = bDeletarArquivos;
            strMMCU = "-mmcu=msp430x2013 ";
        }

        public bool CriaArquivo(String strNomeArquivo, String strDadosArquivo)
        {
            FileStream fileCGravado;

            try
            {
                fileCGravado = new FileStream(Application.StartupPath + "\\" + strNomeArquivo, FileMode.Create);
                fileCGravado.Write(Encoding.Default.GetBytes(strDadosArquivo), 0, Encoding.Default.GetByteCount(strDadosArquivo));
                fileCGravado.Flush();
                fileCGravado.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "LadderApp" + strNomeArquivo, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            lstNomesArquivosCriados.Add(strNomeArquivo);
            return true;
        }

        private bool DeletaTodosArquivosIntermediarios()
        {
            try
            {
                foreach (String _strFile in lstNomesArquivosCriados)
                    File.Delete(Application.StartupPath + @"\" + _strFile);
                lstNomesArquivosCriados.Clear();

                foreach (String _strFile in lstNomesArquivosCompilados)
                    File.Delete(Application.StartupPath + @"\" + _strFile);
                lstNomesArquivosCompilados.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "LadderApp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strNomeArquivo"></param>
        /// <returns></returns>
        /// <remarks>Eclipse: Menu Project/Properties/"C/C++ Settings"/Tool Settings/mspgcc GCC C Compiler/Command:
        /// msp430-gcc
        /// ./All Options: -I"C:\mspgcc\msp430\include" -Os -Wall -c -fmessage-length=0 -mmcu=msp430x2013</remarks>
        public bool CompilaMSP430gcc(String strNomeArquivo)
        {
            startInfo.FileName = "msp430-gcc.exe";
            startInfo.Arguments = @"-IC:\mspgcc\msp430\include -Os -Wall -c -fmessage-length=0 " + strMMCU + "-o" + strNomeArquivo + @".o .\" + strNomeArquivo + @".c";
            p.Start();
            RecuperaStandardStrings();
            p.WaitForExit();

            if (!ShowStandardStrings(strNomeArquivo))
                return false;

            lstNomesArquivosCompilados.Add(strNomeArquivo + ".o");

            try
            {
                if (bHabilitaDeletarArqIntermediarios)
                    File.Delete(Application.StartupPath + @"\" + strNomeArquivo + ".c");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "LadderApp" + strNomeArquivo, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            lstNomesArquivosCriados.Remove(strNomeArquivo + ".c");

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <remarks>Eclipse: Menu Project/Properties/"C/C++ Settings"/Build Steps/Post-build steps/Command:
        /// msp430-objcopy -O ihex  ${ProjName}.elf ${ProjName}.a43;msp430-size  ${ProjName}.elf</remarks>
        public bool CompilaELF(String fileName)
        {
            try
            {
                startInfo.FileName = "msp430-gcc.exe";
                startInfo.Arguments = @"-Os " + strMMCU + "-o" + fileName.Replace(' ', '_') + ".elf " + this.RetornaArquivosCompilados();
                p.Start();
                RecuperaStandardStrings();
                p.WaitForExit();
            }
            catch
            {
            }

            if (bHabilitaDeletarArqIntermediarios)
                this.DeletaTodosArquivosIntermediarios();

            return ShowStandardStrings(startInfo.FileName + fileName);
        }

        public bool CompilaA43(String fileName)
        {
            try
            {
                /// Cria o processo para compilar - unindo todos os arquivo .o e gerando o arquivo .elf
                startInfo.FileName = "msp430-objcopy";
                startInfo.Arguments = @"-O ihex " + fileName.Replace(' ', '_') + ".elf " + fileName.Replace(' ', '_') + ".a43";
                p.Start();
                RecuperaStandardStrings();
                p.WaitForExit();
            }
            catch
            {
            }

                if (ShowStandardStrings(startInfo.FileName + fileName))
                {
                    try
                    {
                        if (bHabilitaDeletarArqIntermediarios)
                            File.Delete(Application.StartupPath + @"\" + fileName.Replace(' ', '_') + ".elf");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "LadderApp" + fileName.Replace(' ', '_') + ".elf", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    try
                    {
                        startInfo.FileName = "msp430-strip";
                        startInfo.Arguments = @"-s " + fileName.Replace(' ', '_') + ".a43";
                        p.Start();
                        RecuperaStandardStrings();
                        p.WaitForExit();
                    }
                    catch
                    {
                    }
                }
                else
                    return false;

            return ShowStandardStrings(startInfo.FileName + fileName);
        }

        public bool GravaViaUSB(String fileName)
        {
            try
            {
                startInfo.FileName = "msp430-jtag";
                startInfo.Arguments = @"--spy-bi-wire --backend=ti --lpt=TIUSB -m -p -v " + fileName + ".a43";
                p.Start();
                RecuperaStandardStrings();
                p.WaitForExit();
            }
            catch
            {
            }

            return ShowStandardStrings(startInfo.FileName + fileName);
        }


        public String LeViaUSB()
        {
            try
            {
                startInfo.FileName = "msp430-jtag";
                startInfo.Arguments = @"--spy-bi-wire --backend=ti --lpt=TIUSB -u 0xf800-0xffff -i";
                p.Start();
                RecuperaStandardStrings();
                p.WaitForExit();
            }
            catch
            {
                strStandardOutput = "";
            }

            if (strStandardOutput != "")
                this.CriaArquivo("dump.a43", strStandardOutput);
            else
            {
                if (strStandardError.Contains("Could not initialize the library (port: TIUSB)"))
                    throw new CouldNotInitializeTIUSBException();
                else
                    throw new NotSupportedException();
            }
            

            return ConvertHex2String(Application.StartupPath + @"\dump.a43");
        }

        private string ConvertHex2TITxt(String filePath)
        {
            if (File.Exists(filePath))
            {
                startInfo.WorkingDirectory = Directory.GetParent(filePath).FullName;
                filePath = filePath.Split('\\')[filePath.Split('\\').Length - 1];
                startInfo.FileName = "ihex2titext";
                startInfo.Arguments = filePath + @" -o " + filePath + ".txt";
                p.Start();
                RecuperaStandardStrings();
                p.WaitForExit();
            }

            return startInfo.WorkingDirectory + '\\' + filePath + ".txt";
        }


        private string ConvertTITxt2String(String filePath)
        {
            string DadosArquivo = "", DadosConvertidosChar = "";
            /// número definido em avaliação do arquivo TITxt
            Int32 _posInicialUtil = 9;

            if (File.Exists(filePath))
            {
                DadosArquivo = File.ReadAllText(filePath);
                File.Delete(filePath);
                for (int i = _posInicialUtil; i < DadosArquivo.Length; i = i + 3)
                {
                    try
                    {
                        DadosConvertidosChar += (char)Int32.Parse(DadosArquivo.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                    }
                    catch
                    {
                    }
                }
            }

            return DadosConvertidosChar;
        }

        public string ConvertHex2String(String filePath)
        {
            return ConvertTITxt2String(ConvertHex2TITxt(filePath));
        }


        private void SetDefaults()
        {
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = System.Windows.Forms.Application.StartupPath;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
        }

        private bool RecuperaStandardStrings()
        {
            strStandardOutput = p.StandardOutput.ReadToEnd();
            strStandardError = p.StandardError.ReadToEnd();

            return true;
        }

        private bool ShowStandardStrings(String strNomeArquivo)
        {
            if (strStandardError != "")
            {
                if (strStandardError.StartsWith(VisualResources.STR_RETORNO_MSPJTAG_ESCREVER_INI) && strStandardError.EndsWith(VisualResources.STR_RETORNO_MSPJTAG_ESCREVER_FIM))
                {
                    MessageBox.Show(strStandardError.Substring(VisualResources.STR_RETORNO_MSPJTAG_ESCREVER_INI.Length, strStandardError.IndexOf(VisualResources.STR_RETORNO_MSPJTAG_ESCREVER_FIM) - VisualResources.STR_RETORNO_MSPJTAG_ESCREVER_INI.Length) + " recorded bytes.", "Error message:" + strNomeArquivo);
                    return true;
                }
                else
                {
                    CriaArquivo("Erro.txt", strStandardError);
                    MessageBox.Show(strStandardError, "Error message:" + strNomeArquivo);
                    strStandardError = "";
                    return false;
                }
            }

            if (strStandardOutput != "")
            {
                MessageBox.Show(strStandardOutput, "Message: " + strNomeArquivo);
                strStandardOutput = "";
                return false;
            }

            return true;
        }

    }
}
