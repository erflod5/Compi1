using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2
{
    class Figure
    {
        public Object pen;
        public Object figure;
        public int tipo;

        public Figure() {
            pen = null;
            figure = null;
        }

        public Figure(Object o, Object ob, int tipo) {
            pen = o;
            figure = ob;
            this.tipo = tipo;
        }
    }
}
