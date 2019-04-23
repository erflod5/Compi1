using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2.herramientas
{
    class Variable
    {
        public int fila, columna;
        public String nombre;
        public Object dato;
        public TYPE t;

        public Variable(int fila, int columna, string nombre, object dato, TYPE t)
        {
            this.fila = fila;
            this.columna = columna;
            this.nombre = nombre;
            this.dato = dato;
            this.t = t;
        }

        public Variable() {
            this.fila = 0;
            this.columna = 0;
            this.nombre = "";
            this.dato = null;
        }
        
    }
    public enum TYPE {INT, STRING, DOUBLE, CHAR, BOOL, VOID, FUNCION, CLASS };
}
