using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proyecto2.analizador;

namespace Proyecto2.herramientas
{
    class Arreglo
    {
        private int i, j, k;
        private Object[] array;
        public TYPE T;
       
        public Arreglo(int i) {
            this.i = i;
            this.j = this.k = -1;
            array = new Object[i];
        }

        public Arreglo(int i, int j) {
            this.i = i;
            this.j = j;
            this.k = -1;
            array = new object[i * j];
        }

        public Arreglo(int i, int j, int k) {
            this.i = i;
            this.j = j;
            this.k = k;
            array = new object[i * j * k];
        }

        public void setData(int i, Object data, TYPE tipo) {
            if (tipo == T || (tipo == TYPE.DOUBLE && T == TYPE.INT) || (tipo == TYPE.INT || T == TYPE.DOUBLE)) {
                if (this.j == -1 && this.k == -1)
                {
                    if (i < this.i)
                    {
                        array[i] = data;
                    }
                    else
                    {
                        /*ERROR DE POSICION*/
                        Syntax.listaerrores.Add(new Error(0, 0, "Posicion fuera del intervalo"));
                    }
                }
                else
                {
                    //ERROR
                    Syntax.listaerrores.Add(new Error(0, 0, "Posicion fuera del intervalo"));
                }
            }
        }

        public void setData1(int i, int dato)
        {
            if (this.j == -1 && this.k == -1)
            {
                if (i < this.i)
                {
                    if (this.T == TYPE.INT)
                        array[i] = Int32.Parse(array[i].ToString()) + dato;
                    else if (this.T == TYPE.DOUBLE)
                        array[i] = Double.Parse(array[i].ToString()) + dato;
                    else if (this.T == TYPE.INT)
                        array[i] = (Char)array[i] + dato;
                    else
                        Syntax.listaerrores.Add(new Error(0, 0, "Tipo de dato no soportado para aumento o decremento"));
                }
                else
                {
                    /*ERROR DE POSICION*/
                    Syntax.listaerrores.Add(new Error(0, 0, "Posicion fuera del intervalo"));
                }
            }
            else
            {
                //ERROR
                Syntax.listaerrores.Add(new Error(0, 0, "Posicion fuera del intervalo"));
            }
        }

        public void setData(int i, int j, Object data, TYPE tipo) {
            if (tipo == T || (tipo == TYPE.DOUBLE && T == TYPE.INT) || (tipo == TYPE.INT || T == TYPE.DOUBLE))
            {
                if (this.k == -1)
                {
                    if (this.j != -1)
                    {
                        if (i < this.i && j < this.j)
                        {
                            array[j * this.i + i] = data;
                        }
                        else
                        {
                            /*ERROR DE POSICION*/
                            Syntax.listaerrores.Add(new Error(0, 0, "Posicion fuera del intervalo"));
                        }
                    }
                    else
                    {
                        /*ERROR*/
                        Syntax.listaerrores.Add(new Error(0, 0, "Posicion fuera del intervalo"));
                    }
                }
                else
                {
                    /*error again*/
                    Syntax.listaerrores.Add(new Error(0, 0, "Posicion fuera del intervalo"));
                }
            }
        }

        public void setData1(int i, int j, int dato)
        {
            if (this.k == -1)
            {
                if (this.j != -1)
                {
                    if (i < this.i && j < this.j)
                    {
                        if (this.T == TYPE.INT)
                            array[j * this.i + i] = Int32.Parse(array[i].ToString()) + dato;
                        else if (this.T == TYPE.DOUBLE)
                            array[j * this.i + i] = Double.Parse(array[i].ToString()) + dato;
                        else if (this.T == TYPE.INT)
                            array[j * this.i + i] = (Char)array[i] + dato;
                        else
                            Syntax.listaerrores.Add(new Error(0, 0, "Tipo de dato no soportado para aumento o decremento"));
                    }
                    else
                    {
                        /*ERROR DE POSICION*/
                        Syntax.listaerrores.Add(new Error(0, 0, "Posicion fuera del intervalo"));
                    }
                }
                else
                {
                    /*ERROR*/
                    Syntax.listaerrores.Add(new Error(0, 0, "Posicion fuera del intervalo"));
                }
            }
            else
            {
                /*error again*/
                Syntax.listaerrores.Add(new Error(0, 0, "Posicion fuera del intervalo"));
            }
        }

        public void setData(int i, int j, int k, Object data, TYPE tipo) {
            if (tipo == T || (tipo == TYPE.DOUBLE && T == TYPE.INT) || (tipo == TYPE.INT || T == TYPE.DOUBLE))
            {
                if (this.k != -1 && this.j != -1)
                {
                    if (i < this.i && j < this.j && k < this.k)
                    {
                        array[k * this.j * this.i + j * this.i + i] = data;
                    }
                }
                else
                {
                    /*ERROR DE DIMENSION EN ARRAY*/
                    Syntax.listaerrores.Add(new Error(0, 0, "Posicion fuera del intervalo"));
                }
            }
        }

        public void setData1(int i, int j, int k, int dato)
        {
            if (this.k != -1 && this.j != -1)
            {
                if (i < this.i && j < this.j && k < this.k)
                {
                    if (this.T == TYPE.INT)
                        array[k * this.j * this.i + j * this.i + i] = Int32.Parse(array[i].ToString()) + dato;
                    else if (this.T == TYPE.DOUBLE)
                        array[k * this.j * this.i + j * this.i + i] = Double.Parse(array[i].ToString()) + dato;
                    else if (this.T == TYPE.INT)
                        array[k * this.j * this.i + j * this.i + i] = (Char)array[i] + dato;
                    else
                        Syntax.listaerrores.Add(new Error(0, 0, "Tipo de dato no soportado para aumento o decremento"));
                }
            }
            else
            {
                /*ERROR DE DIMENSION EN ARRAY*/
                Syntax.listaerrores.Add(new Error(0, 0, "Posicion fuera del intervalo"));
            }
        }

        public Object getData(int i) {
            if (this.j == -1 && this.k == -1) {
                if (i < this.i) {
                    return array[i];
                }
            }
            Syntax.listaerrores.Add(new Error(0, 0, "Posicion: " + i + " fuera del intervalo"));
            return null;
        }

        public Object getData(int i, int j) {
            if (this.k == -1) {
                if (i < this.i && j < this.j) {
                    return array[j * this.i + i];
                }
            }
            Syntax.listaerrores.Add(new Error(0, 0, "Posicion: " + i + "," + j + " fuera del intervalo"));
            return null;
        }

        public Object getData(int i, int j, int k) {
            if (i < this.i && j < this.j && k < this.k) {
                return array[k * this.j * this.i + j * this.i + i];
            }
            Syntax.listaerrores.Add(new Error(0, 0, "Posicion: " + i + "," + j + "," + k + " fuera del intervalo"));

            return null;
        }

        public void setTipo(String tipo)
        {
            switch (tipo.ToLower())
            {
                case "int":
                    this.T = TYPE.INT;
                    break;
                case "string":
                    this.T = TYPE.STRING;
                    break;
                case "double":
                    this.T = TYPE.DOUBLE;
                    break;
                case "char":
                    this.T = TYPE.CHAR;
                    break;
                case "bool":
                    this.T = TYPE.BOOL;
                    break;
            }
        }
    }
}
