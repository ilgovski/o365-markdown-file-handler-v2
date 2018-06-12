/*
 * Markdown File Handler - Sample Code
 * Copyright (c) Microsoft Corporation
 * All rights reserved. 
 * 
 * MIT License
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the ""Software""), to deal in 
 * the Software without restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
 * Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace MarkdownFileHandler.Models
{
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Web.Mvc;

    public class MarkdownFileModel
    {
        private MarkdownFileModel(FileHandlerActivationParameters activationParameters)
        {
            this.ActivationParameters = activationParameters;
        }

        public FileHandlerActivationParameters ActivationParameters { get; set; }
        public MvcHtmlString HtmlFileContent { get; set; }
        public string MarkdownFileContent { get; set; }

        public string ErrorMessage { get; set; }

        public bool ReadOnly { get; set; }

        public string Filename { get; set; }


        public static MarkdownFileModel GetErrorModel(FileHandlerActivationParameters parameters, string errorMessage)
        {
            return new MarkdownFileModel(parameters) { ErrorMessage = errorMessage, ReadOnly = true };
        }

        public static MarkdownFileModel GetErrorModel(FileHandlerActivationParameters parameters, Exception ex)
        {
            return new MarkdownFileModel(parameters) { ErrorMessage = ex.Message, ReadOnly = true };
        }

        public static MarkdownFileModel GetReadOnlyModel(FileHandlerActivationParameters parameters, string filename, string markdownContent, string filepath)
        {
            return new MarkdownFileModel(parameters)
            {
                MarkdownFileContent = markdownContent,
                HtmlFileContent = ConvertMarkdowntoHtml(markdownContent, filepath),
                Filename = filename,
                ReadOnly = true
            };
        }

        public static MarkdownFileModel GetWriteableModel(FileHandlerActivationParameters parameters, string filename, string markdownContent, string filepath)
        {
            return new MarkdownFileModel(parameters)
            {
                MarkdownFileContent = markdownContent,
                HtmlFileContent = ConvertMarkdowntoHtml(markdownContent, filepath),
                Filename = filename,
                ReadOnly = false
            };
        }

        private static MvcHtmlString ConvertMarkdowntoHtml(string markdownContent, string filepath)
        {
            // convert docx to html
            string origdirname = Path.GetDirectoryName(filepath);
            string origfilename = Path.GetFileName(filepath);

            ExecuteCommand(String.Format("/c cd \"{0}\" && soffice -headless -convert-to xhtml \"{1}\"", origdirname, origfilename));

            // read html file
            string htmlfilepath = filepath.Replace(".docx", ".xhtml");
            string html = "An error occured";

            if (File.Exists(htmlfilepath))
            {
                html = File.ReadAllText(htmlfilepath);

                // temporary solution: replace <a>
                html = html.Replace("<a ", "<span ").Replace("</a>", "</span>");
            }

            return new MvcHtmlString(html);
        }

        private static void ExecuteCommand(string cmd)
        {
            ProcessStartInfo cmdsi = new ProcessStartInfo("cmd.exe");
            cmdsi.Arguments = cmd;

            Process proc = Process.Start(cmdsi);
            proc.WaitForExit();
            proc.Close();
        }
    }
}