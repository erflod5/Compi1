using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Irony.Parsing;

namespace Proyecto2.herramientas
{
    class Clase
    {
        Entorno principal;
        ArrayList lista_importar;
        ParseTreeNode raiz;
        String name;

        public Clase() {
            principal = new Entorno();
            lista_importar = new ArrayList();
            this.name = "";
            this.raiz = null;
        }

        public Clase(String name, ParseTreeNode raiz) {
            this.name = name;
            this.raiz = raiz;
            principal = new Entorno();
            lista_importar = new ArrayList();
        }
    }
}
