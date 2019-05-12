using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2.herramientas
{
    class Error
    {
        public int fila, columna;
        public String error;

        public Error(int fila, int columna, String error)
        {
            this.fila = fila;
            this.columna = columna;
            this.error = error;
        }
    }
}
