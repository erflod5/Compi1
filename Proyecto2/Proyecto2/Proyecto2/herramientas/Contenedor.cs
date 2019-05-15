using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto2.herramientas
{
    class Contenedor
    {
        private RichTextBox rtb;
        private TabPage tp;
        public Label lb;
        private String nombre;

        public Contenedor(RichTextBox rtb, TabPage tp)
        {
            this.rtb = rtb;
            this.tp = tp;
        }

        public Contenedor(RichTextBox rtb, TabPage tp, Label lb)
        {
            this.rtb = rtb;
            this.tp = tp;
            this.lb = lb;
        }

        public void SetRichTextBox(RichTextBox rtb) {
            this.rtb = rtb;
        }

        public void SetTabPage(TabPage tp) {
            this.tp = tp;
        }

        public void SetNombre(String nombre) {
            this.nombre = nombre;
        }

        public RichTextBox GetRichTextBox() {
            return rtb;
        }

        public TabPage GetTabPage() {
            return tp;
        }

        public String GetNombre() {
            return nombre;
        }
    }
}
