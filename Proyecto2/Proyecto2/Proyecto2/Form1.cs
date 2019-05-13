using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Forms;
using Proyecto2.analizador;
using Proyecto2.herramientas;
using System.IO;

namespace Proyecto2
{
    public partial class Form1 : Form
    {
        ArrayList list_fil;
        public static RichTextBox console;
        public Form1()
        {
            InitializeComponent();
            list_fil = new ArrayList();
            list_fil.Add(new Contenedor(richTextBox2, tabControl2.SelectedTab));
            console = richTextBox1;
        }

        private void eliminarPestaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Desea eliminar la pestana actual?", "Eliminar", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes) {
                foreach (Contenedor c in list_fil)
                {
                    if (c.GetTabPage() == tabControl2.SelectedTab) {
                        list_fil.Remove(c);
                        tabControl2.TabPages.Remove(tabControl2.SelectedTab);
                        return;
                    }
                }
            }
        }

        private void compilarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                String texto = "";
                String textoAux = "";
                foreach(Contenedor c in list_fil)
                {
                    if (c.GetTabPage() == tabControl2.SelectedTab)
                        textoAux = c.GetRichTextBox().Text;
                    else
                        texto += c.GetRichTextBox().Text;
                }
                texto = textoAux + texto;
                bool resultado = Syntax.analizar(texto);
                if (resultado)
                {
                    Console.WriteLine("Entrada correcta\n");
                    dataGridView1.Rows.Clear();
                    dataGridView1.Refresh();
                    if(Syntax.h !=null)
                        addGlobal(Syntax.h.tableSyml);
                    if(Syntax.h1!=null)
                        addGlobal(Syntax.h1.tableSyml);
                }
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString());
            }
        }

        private void addGlobal(Hashtable hs) {
            foreach(DictionaryEntry a in hs)
            {
                Variable b = (Variable)a.Value;
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = index;
                dataGridView1.Rows[index].Cells[1].Value = b.nombre;
                dataGridView1.Rows[index].Cells[2].Value = b.dato;
                dataGridView1.Rows[index].Cells[3].Value = b.t;
                dataGridView1.Rows[index].Cells[4].Value = b.fila;
                dataGridView1.Rows[index].Cells[5].Value = b.columna;
            }
        }

        private void crearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage tp = new TabPage("Default");
            tabControl2.TabPages.Add(tp);
            RichTextBox rtb = new RichTextBox();
            rtb.Dock = DockStyle.Fill;
            rtb.Multiline = true;
            rtb.BackColor = Color.Black;
            rtb.ForeColor = richTextBox2.ForeColor;
            rtb.Font = richTextBox2.Font;
            tp.Controls.Add(rtb);
            list_fil.Add(new Contenedor(rtb, tp));
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog browser1 = new OpenFileDialog();
            browser1.Title = "Nuevo Archivo de Entrada";
            //browser1.Filter = "Archivo Fi|*.fi";
            DialogResult dr = browser1.ShowDialog();
            if (dr == DialogResult.OK) {
                foreach (Contenedor c in list_fil){
                    if (tabControl2.SelectedTab == c.GetTabPage()) {
                        c.GetRichTextBox().Text = File.ReadAllText(browser1.FileName);
                        FileInfo fi = new FileInfo(browser1.FileName);
                        c.GetTabPage().Text = fi.Name;
                        c.SetNombre(browser1.FileName);
                        return;
                    }
                }
            }
        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Contenedor c in list_fil) {
                if (tabControl2.SelectedTab == c.GetTabPage()) {
                    if (tabControl2.SelectedTab.Text == "Default")
                    {
                        SaveFileDialog save = new SaveFileDialog();
                        save.Filter = "Archivo Fi|*.fi";
                        save.Title = "Guardar Archivo";
                        save.ShowDialog();
                        if (save.FileName != "") {
                            c.GetRichTextBox().SaveFile(save.FileName,RichTextBoxStreamType.PlainText);
                            c.SetNombre(save.FileName);
                            FileInfo f = new FileInfo(save.FileName);
                            c.GetTabPage().Text = f.Name;
                        }
                    }
                    else {
                        c.GetRichTextBox().SaveFile(c.GetNombre(),RichTextBoxStreamType.PlainText);
                    }
                    return;
                }
            }
        }

        private void guardarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Contenedor c in list_fil) {
                if (tabControl2.SelectedTab == c.GetTabPage()) {
                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "Archivo Fi|*.fi";
                    save.Title = "Guardar Archivo";
                    save.ShowDialog();
                    if (save.FileName != "")
                    {
                        c.GetRichTextBox().SaveFile(save.FileName, RichTextBoxStreamType.PlainText);
                        c.SetNombre(save.FileName);
                        FileInfo f = new FileInfo(save.FileName);
                        c.GetTabPage().Text = f.Name;
                    }
                }
            }
        }

        private void generarASTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String ruta = Path.Combine(Application.StartupPath, "Image");
            try
            {
                if(Directory.Exists(ruta))
                    System.IO.Directory.Delete(ruta, true);
                DirectoryInfo dr = Directory.CreateDirectory(ruta);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Syntax.generar();
            System.Diagnostics.Process.Start(ruta);
        }

        private void erroresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ruta = Path.Combine(Application.StartupPath, "errores.html");
            try
            {
                if (File.Exists(ruta))
                    File.Delete(ruta); 
            }
            catch
            {

            }
            StringBuilder bf = new StringBuilder();
            bf.Append("<html>\n <head>\n <meta charset=\"utf-8\" />\n <title>Reporte de Tokens</title>\n <meta name=\"viewport\" content=\"initial-scale=1.0; maximum-scale=1.0; width=device-width;\">\n");
            bf.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/main.css\">\n </head>\n <body>\n <div class=\"table-title\">\n <h3>Reporte de Errores</h3>\n </div>\n");
            bf.Append("<table class=\"table-fill\">\n <thead>\n <th class=\"text-left\">Fila</th>\n <th class=\"text-left\">Columna</th>\n <th class=\"text-left\">Error</th>	\n");
            bf.Append("</tr>\n </thead>\n <tbody class=\"table-hover\">");
            foreach (Error er in Syntax.listaerrores)
            {
                bf.Append("<tr> <td class=\"text-left\"> " + er.fila + "</td>");
                bf.Append("<td class=\"text-left\">" + er.columna + "</td>");
                bf.Append("<td class=\"text-left\">" + er.error + "</td> </tr>");
            }
            bf.Append("</tbody>\n </table>\n </body>\n </html>");

            using (StreamWriter mylogs = File.AppendText(ruta))
            {
                mylogs.WriteLine(bf.ToString());
                mylogs.Close();
            }
            System.Diagnostics.Process.Start(ruta);
        }
    }
}
