using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;
using WINGRAPHVIZLib;
using Proyecto2.herramientas;
using System.Windows.Forms;
using System.Collections;

namespace Proyecto2.analizador
{
    class Syntax : Irony.Parsing.Grammar
    {
        public static Hashtable clases = new Hashtable();
        public static ParseTreeNode main_node;

        public static bool analizar(String cadena) {
            clases.Clear();
            main_node = null;
            Grammar g = new Grammar();
            LanguageData lg = new LanguageData(g);
            Parser pr = new Parser(lg);
            ParseTree tree = pr.Parse(cadena);
            ParseTreeNode raiz = tree.Root;
            if (raiz == null) return false;
            addClass(raiz.ChildNodes[0]);
            return true;
        }

        private static void addClass(ParseTreeNode raiz) {
            foreach (ParseTreeNode node in raiz.ChildNodes) {
                String name = node.ChildNodes[1].Token.Value.ToString();
                if (!clases.ContainsKey(name)) {
                    Clase c = new Clase(name, node);
                    clases.Add(name, c);
                    generarImagen(name, node);
                    if (main_node == null) {
                        if (node.ChildNodes.Count == 3){
                            SearchMain(node.ChildNodes[3]);
                        }
                        else{
                            SearchMain(node.ChildNodes[5]);
                        }
                    }
                }
            }
        }

        private static void generarImagen(String name, ParseTreeNode raiz) {
            String stringdot = Graficador.getDot(raiz);
            DOT dot = new DOT();
            BinaryImage img = dot.ToPNG(stringdot);
            img.Save("Image/" + name + ".png");
        }

        private static void SearchMain(ParseTreeNode raiz) {
            foreach (ParseTreeNode node in raiz.ChildNodes)
            {
                if (node.ChildNodes[0].Token.Value.Equals("Main")) {
                    if (main_node == null)
                        main_node = node.ChildNodes[0];
                    else {
                        /*Error doble main*/
                    }
                }
            }
        }

        public static Variable primerRecorrido(ParseTreeNode raiz, Entorno h) {
            Variable b = new Variable();
            return b;
        }
    }
}
