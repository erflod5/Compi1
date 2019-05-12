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
        public Object datoaux;
        public TYPE t;
        public TYPE taux;
        public bool Private;

        public Variable(int fila, int columna, string nombre, object dato, TYPE t)
        {
            this.fila = fila;
            this.columna = columna;
            this.nombre = nombre;
            this.dato = dato;
            this.t = t;
        }

        public Variable(int fila, int columna, string nombre, object dato, String type, bool @private)
        {
            this.fila = fila;
            this.columna = columna;
            this.nombre = nombre;
            this.dato = dato;
            addType(type);
            Private = @private;
        }

        public Variable() {
            this.fila = 0;
            this.columna = 0;
            this.nombre = "";
            this.dato = null;
        }

        public void addType(String type) {
            switch (type.ToLower()) {
                case "int":
                    this.t = TYPE.INT;
                    break;
                case "bool":
                    this.t = TYPE.BOOL;
                    break;
                case "string":
                    this.t = TYPE.STRING;
                    break;
                case "double":
                    this.t = TYPE.DOUBLE;
                    break;
                case "char":
                    this.t = TYPE.CHAR;
                    break;
                case "void":
                    this.t = TYPE.VOID;
                    break;
                case "funcion":
                    this.t = TYPE.FUNCION;  
                    break;
                default:
                    this.t = TYPE.CLASS;
                    break;
            }
        }

        public bool esnum() {
            return this.t == TYPE.INT || this.t == TYPE.DOUBLE ? true : false;
        }

        public void addVar(Object dato, String tipo) {
            switch (tipo.ToLower()) {
                case "num":
                    this.dato = Int32.Parse(dato.ToString());
                    this.t = TYPE.INT;
                    break;
                case "dec":
                    this.dato = Double.Parse(dato.ToString());
                    this.t = TYPE.DOUBLE;
                    break;
                case "rtrue":
                    this.dato = (Boolean) true;
                    this.t = TYPE.BOOL;
                    break;
                case "rfalse":
                    this.t = TYPE.BOOL;
                    this.dato =(Boolean)false;
                    break;
                case "string":
                    this.t = TYPE.STRING;
                    this.dato = (String)dato.ToString().Replace("\"","");
                    break;
                case "char":
                    this.t = TYPE.CHAR;
                    this.dato = (Char)Char.Parse(dato.ToString().Replace("'",""));
                    break;
            }
        }
    }
    public enum TYPE {INT, STRING, DOUBLE, CHAR, BOOL, VOID, FUNCION, CLASS, CONTINUAR,SALIR, RETURN, ARRAY, ERROR };
}
