using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2.herramientas
{
    class Arreglo
    {
        private int i, j, k;
        private Object[] array;

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

        public void setData(int i, Object data) {
            if (this.j == -1 && this.k == -1){
                if (i < this.i)
                {
                    array[i] = data;
                }
                else {
                    /*ERROR DE POSICION*/
                }
            }
            else {
                //ERROR
            }
        }

        public void setData(int i, int j, Object data) {
            if (this.k == -1)
            {
                if (this.j != -1)
                {
                    if (i < this.i && j < this.j)
                    {
                        array[j * this.i  + i] = data;
                    }
                    else
                    {
                        /*ERROR DE POSICION*/
                    }
                }
                else
                {
                    /*ERROR*/
                }
            }
            else {
                /*error again*/
            }
        }

        public void setData(int i, int j, int k, Object data) {
            if (this.k != -1 && this.j != -1)
            {
                if (i < this.i && j < this.j && k < this.k) {
                    array[k * this.j*this.i + j * this.i + i] = data;
                }
            }
            else {
                /*ERROR DE DIMENSION EN ARRAY*/
            }
        }

        public Object getData(int i) {
            if (this.j == -1 && this.k == -1) {
                if (i < this.i) {
                    return array[i];
                }
            }
            return null;
        }

        public Object getData(int i, int j) {
            if (this.k == -1) {
                if (i < this.i && j < this.j) {
                    return array[j * this.i + i];
                }
            }
            return null;
        }

        public Object getData(int i, int j, int k) {
            if (i < this.i && j < this.j && k < this.k) {
                return array[k * this.j * this.i + j * this.i + i];
            }
            return null;
        }
    }
}
