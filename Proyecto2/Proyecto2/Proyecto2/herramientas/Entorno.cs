using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Proyecto2.analizador;

namespace Proyecto2.herramientas
{
    class Entorno
    {
        Entorno anterior;
        public Hashtable tableSyml;

        public Entorno(Entorno anterior, Hashtable tableSyml)
        {
            this.anterior = anterior;
            this.tableSyml = tableSyml;
        }

        public Entorno() {
            this.anterior = null;
            tableSyml = new Hashtable();
        }

        public Entorno(Entorno anterior) {
            this.anterior = anterior;
            tableSyml = new Hashtable();
        }

        public bool addVariable(String key, Object data) {
            if (!tableSyml.ContainsKey(key.ToLower())) {
                tableSyml.Add(key.ToLower(), data);
                return true;
            }
            Variable b = (Variable)data;
            Syntax.listaerrores.Add(new Error(b.fila, b.columna, "Ya existe la variable " + key));
            return false;
        }

        public bool changeValue(String key, Object data) {
            if (tableSyml.Contains(key.ToLower())) {
                tableSyml[key.ToLower()] = data;
            }
            Variable b = (Variable)data;
            Syntax.listaerrores.Add(new Error(b.fila, b.columna, "No existe la variable " + key));
            return false;
        }

        public bool addVariable(String key, Variable b, String tipo) {
            switch (tipo.ToLower()) {
                case "int":
                    if (b.dato is Int32) {
                        return addVariable(key, b);
                    }
                    return false;
                case "double":
                    if (b.dato is Int32 || b.dato is Double) {
                        return addVariable(key, b);
                    }
                    return false;
                case "bool":
                    if (b.dato is Boolean) {
                        return addVariable(key, b);
                    }
                    return false;
                case "char":
                    if (b.dato is Char) {
                        return addVariable(key, b);
                    }
                    return false;
                case "string":
                    if (b.dato is String) {
                        return addVariable(key, b);
                    }
                    return false;
                default:
                    return false;
            }
        }

        public Object getValue(String key) {
            if (tableSyml.Contains(key)) {
                return tableSyml[key.ToLower()];
            }
            return getValue(key, anterior);
        }

        private Object getValue(String key, Entorno h) {
            if (h != null) {
                if (h.tableSyml.Contains(key)) {
                    Object o = h.tableSyml[key];
                    if (o is Variable) {
                        return o;
                    }
                }
                return getValue(key, h.anterior);
            }
            return null;
        }
    }
}
