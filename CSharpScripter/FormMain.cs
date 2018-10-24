﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace CSharpScripter {
   public partial class FormMain : Form {
      TextEditorControl tbxCode;
      public FormMain() {
         InitializeComponent();
         InitializeEditor();
         Console.SetOut(new TextBoxWriter(this.tbxRun));
      }

      private void InitializeEditor() {
         this.tbxCode = new TextEditorControl();
         this.tbxCode.Dock = DockStyle.Fill;
         this.tbxCode.BorderStyle = BorderStyle.FixedSingle;
         this.tbxCode.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy("C#");
         this.pnlEditor.Controls.Add(this.tbxCode);
      }

      private void Run() {
         this.SaveCode();

         // 코드
         string code = this.tbxCode.Text;

         // 컴파일러 생성
         CodeDomProvider codeDom = CodeDomProvider.CreateProvider("CSharp");
         // 컴파일 파라미터
         CompilerParameters cparams = new CompilerParameters();
         cparams.GenerateInMemory = true;
         // 컴파일
         CompilerResults results = codeDom.CompileAssemblyFromSource(cparams, code);

         // 출력 메시지
         foreach (var result in results.Output) {
            this.tbxRun.AppendText(result+Environment.NewLine);
         }

         // 에러가 있으면
         if (results.Errors.Count != 0)
            return;

         // 에러 없으면
         // 어셈블리 로딩
         Type myType = results.CompiledAssembly.GetType("Test.MyClass");
         // 메인함수 실행
         var mi = myType.GetMethod("Main");
         mi.Invoke(null, new object[0]);
         Console.WriteLine("Finished");
      }

      private void btnRun_Click(object sender, EventArgs e) {
         this.Run();
      }

      private void SaveCode() {
         Properties.Settings.Default.code = this.tbxCode.Text;
         Properties.Settings.Default.Save();
      }

      private void LoadCode() {
         Properties.Settings.Default.Reload();
         this.tbxCode.Text = Properties.Settings.Default.code;
      }

      private void btnClear_Click(object sender, EventArgs e) {
         this.tbxRun.Clear();
      }

      private void btnSave_Click(object sender, EventArgs e) {
         this.SaveCode();
      }

      private void btnLoad_Click(object sender, EventArgs e) {
         this.LoadCode();
      }

      private void FormMain_Load(object sender, EventArgs e) {
         this.LoadCode();
      }

      private void FormMain_FormClosing(object sender, FormClosingEventArgs e) {
         this.SaveCode();
      }
   }
}
