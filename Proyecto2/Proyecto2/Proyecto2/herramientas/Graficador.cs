using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace Proyecto2.herramientas
{
    class Graficador
    {
        private static int contador;
        private static String grafo;

        public static String getDot(ParseTreeNode raiz) {
            grafo = "digraph G{";
            grafo += "nodo0[label=\"" + escapar(raiz.ToString()) + "\"];\n";
            contador = 1;
            recorrerAst("nodo0", raiz);
            grafo += "}";
            return grafo;
        }

        private static void recorrerAst(String padre, ParseTreeNode raiz) {
            foreach (ParseTreeNode hijo in raiz.ChildNodes) {
                String nameHijo = "nodo" + contador.ToString();
                grafo += nameHijo + "[label=\"" + escapar(hijo.ToString()) + "\"];\n";
                grafo += padre + "->" + nameHijo + ";\n";
                contador++;
                recorrerAst(nameHijo, hijo);
            }
        }

        private static String escapar(String cadena) {
            cadena = cadena.Replace("\\", "\\\\");
            cadena = cadena.Replace("\"", "\\\"");
            return cadena;
        }


    }
}
