using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Proyecto2.herramientas
{
    class Entorno
    {
        Entorno anterior;
        Hashtable tableSyml;

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
            if (!tableSyml.ContainsKey(key)) {
                tableSyml.Add(key, data);
                return true;
            }
            return false;
        }

        public bool changeValue(String key, Object data) {
            if (tableSyml.Contains(key)) {
                tableSyml[key] = data;
            }
            return false;
        }

        public Object getValue(String key) {
            return tableSyml[key];
        }
    }
}
