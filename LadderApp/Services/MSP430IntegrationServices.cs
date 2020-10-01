using LadderApp.Exceções;
using LadderApp.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LadderApp
{
    public class MSP430IntegrationServices
    {
        private Process p = new Process();
        private ProcessStartInfo startInfo = new ProcessStartInfo();
        private List<string> compiledFilenames = new List<string>();
        private List<string> createdFilenames = new List<string>();
        private bool EnabledDeletingIntermediateFiles = false;
        private String strMMCU = "";

        private String strStandardOutput = "";
        private String strStandardError = "";

        private string GetCompiledFilenames()
        {
            String filenamesText = "";
            foreach (String filename in compiledFilenames)
                filenamesText += filename + " ";

            return filenamesText.Trim();
        }

        public MSP430IntegrationServices()
        {
            p.StartInfo = startInfo;
            SetDefaults();
            compiledFilenames.Clear();
            createdFilenames.Clear();
            strMMCU = "-mmcu=msp430x2013 ";
        }

        public MSP430IntegrationServices(bool deleteIntermediateFiles)
        {
            p.StartInfo = startInfo;
            SetDefaults();
            compiledFilenames.Clear();
            createdFilenames.Clear();
            EnabledDeletingIntermediateFiles = deleteIntermediateFiles;
            strMMCU = "-mmcu=msp430x2013 ";
        }

        public bool CreateFile(string filename, string dataContent)
        {
            try
            {
                FileStream fs = new FileStream(Application.StartupPath + "\\" + filename, FileMode.Create);
                fs.Write(Encoding.Default.GetBytes(dataContent), 0, Encoding.Default.GetByteCount(dataContent));
                fs.Flush();
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "LadderApp" + filename, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            createdFilenames.Add(filename);
            return true;
        }

        private bool DeleteAllIntermediateFiles()
        {
            try
            {
                foreach (String filename in createdFilenames)
                    File.Delete(Application.StartupPath + @"\" + filename);
                createdFilenames.Clear();

                foreach (String filename in compiledFilenames)
                    File.Delete(Application.StartupPath + @"\" + filename);
                compiledFilenames.Clear();
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
        /// <param name="fileNameWithoutExtension"></param>
        /// <returns></returns>
        /// <remarks>Eclipse: Menu Project/Properties/"C/C++ Settings"/Tool Settings/mspgcc GCC C Compiler/Command:
        /// msp430-gcc
        /// ./All Options: -I"C:\mspgcc\msp430\include" -Os -Wall -c -fmessage-length=0 -mmcu=msp430x2013</remarks>
        public bool CompilesMsp430ViaGcc(String fileNameWithoutExtension)
        {
            startInfo.FileName = "msp430-gcc.exe";
            startInfo.Arguments = @"-IC:\mspgcc\msp430\include -Os -Wall -c -fmessage-length=0 " + strMMCU + "-o" + fileNameWithoutExtension + @".o .\" + fileNameWithoutExtension + @".c";
            p.Start();
            ReadStandardStrings();
            p.WaitForExit();

            if (!ShowStandardStrings(fileNameWithoutExtension))
                return false;

            compiledFilenames.Add(fileNameWithoutExtension + ".o");

            try
            {
                if (EnabledDeletingIntermediateFiles)
                    File.Delete(Application.StartupPath + @"\" + fileNameWithoutExtension + ".c");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "LadderApp" + fileNameWithoutExtension, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            createdFilenames.Remove(fileNameWithoutExtension + ".c");
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <remarks>Eclipse: Menu Project/Properties/"C/C++ Settings"/Build Steps/Post-build steps/Command:
        /// msp430-objcopy -O ihex  ${ProjName}.elf ${ProjName}.a43;msp430-size  ${ProjName}.elf</remarks>
        public bool CompileELF(String fileName)
        {
            try
            {
                startInfo.FileName = "msp430-gcc.exe";
                startInfo.Arguments = @"-Os " + strMMCU + "-o" + fileName.Replace(' ', '_') + ".elf " + this.GetCompiledFilenames();
                p.Start();
                ReadStandardStrings();
                p.WaitForExit();
            }
            catch
            {
            }

            if (EnabledDeletingIntermediateFiles)
                this.DeleteAllIntermediateFiles();

            return ShowStandardStrings(startInfo.FileName + fileName);
        }

        public bool CompileA43(String fileName)
        {
            try
            {
                /// Cria o processo para compilar - unindo todos os arquivo .o e gerando o arquivo .elf
                startInfo.FileName = "msp430-objcopy";
                startInfo.Arguments = @"-O ihex " + fileName.Replace(' ', '_') + ".elf " + fileName.Replace(' ', '_') + ".a43";
                p.Start();
                ReadStandardStrings();
                p.WaitForExit();
            }
            catch
            {
            }

            if (ShowStandardStrings(startInfo.FileName + fileName))
            {
                try
                {
                    if (EnabledDeletingIntermediateFiles)
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
                    ReadStandardStrings();
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

        public bool DownloadViaUSB(String fileName)
        {
            try
            {
                startInfo.FileName = "msp430-jtag";
                startInfo.Arguments = @"--spy-bi-wire --backend=ti --lpt=TIUSB -m -p -v " + fileName + ".a43";
                p.Start();
                ReadStandardStrings();
                p.WaitForExit();
            }
            catch
            {
            }

            return ShowStandardStrings(startInfo.FileName + fileName);
        }


        public string ReadsViaUSB()
        {
            try
            {
                startInfo.FileName = "msp430-jtag";
                startInfo.Arguments = @"--spy-bi-wire --backend=ti --lpt=TIUSB -u 0xf800-0xffff -i";
                p.Start();
                ReadStandardStrings();
                p.WaitForExit();
            }
            catch
            {
                strStandardOutput = "";
            }

            if (strStandardOutput != "")
                this.CreateFile("dump.a43", strStandardOutput);
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
                ReadStandardStrings();
                p.WaitForExit();
            }

            return startInfo.WorkingDirectory + '\\' + filePath + ".txt";
        }


        private string ConvertTITxt2String(String filePath)
        {
            const int usefulStartingPosition = 9;
            if (File.Exists(filePath))
            {
                string fileContent = File.ReadAllText(filePath);
                File.Delete(filePath);
                string charConvertedData = "";
                for (int i = usefulStartingPosition; i < fileContent.Length; i = i + 3)
                {
                    charConvertedData += (char)int.Parse(fileContent.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                }
                return charConvertedData;
            }
            return null;
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

        private bool ReadStandardStrings()
        {
            strStandardOutput = p.StandardOutput.ReadToEnd();
            strStandardError = p.StandardError.ReadToEnd();
            return true;
        }

        private bool ShowStandardStrings(String filename)
        {
            if (strStandardError != "")
            {
                if (strStandardError.StartsWith(VisualResources.ReturnedTextFromMspJtagWriteBegin) && strStandardError.EndsWith(VisualResources.ReturnedTextFromMspJtagWriteEnd))
                {
                    MessageBox.Show(strStandardError.Substring(VisualResources.ReturnedTextFromMspJtagWriteBegin.Length, strStandardError.IndexOf(VisualResources.ReturnedTextFromMspJtagWriteEnd) - VisualResources.ReturnedTextFromMspJtagWriteBegin.Length) + " recorded bytes.", "Error message:" + filename);
                    return true;
                }
                else
                {
                    CreateFile("Erro.txt", strStandardError);
                    MessageBox.Show(strStandardError, "Error message:" + filename);
                    strStandardError = "";
                    return false;
                }
            }

            if (strStandardOutput != "")
            {
                MessageBox.Show(strStandardOutput, "Message: " + filename);
                strStandardOutput = "";
                return false;
            }

            return true;
        }

    }
}
