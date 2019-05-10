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
using System.Drawing;

namespace Proyecto2.analizador
{
    class Syntax : Irony.Parsing.Grammar
    {
        public static Hashtable clases = new Hashtable();
        public static ParseTreeNode main_node;
        public static ParseTreeNode claseActual;
        public static Entorno h = new Entorno();
        public static ArrayList forms = new ArrayList();
        public static ArrayList images = new ArrayList();

        public static bool analizar(String cadena) {
            clases.Clear();
            forms.Clear();
            images.Clear();
            main_node = claseActual = null;
            h = new Entorno();
            Form1.console.Clear();
            Grammar g = new Grammar();
            LanguageData lg = new LanguageData(g);
            Parser pr = new Parser(lg);
            ParseTree tree = pr.Parse(cadena);
            ParseTreeNode raiz = tree.Root;
            forms.Add(new Form2());
            if (raiz == null) return false;
            addClass(raiz.ChildNodes[0]);
            return true;
        }

        private static void importar(ParseTreeNode raiz, Entorno h) {
            if (raiz.ChildNodes.Count == 3)
            {
                addGlobal(raiz.ChildNodes[2], h);
            }
            else if (raiz.ChildNodes.Count == 4)
            {
                foreach (ParseTreeNode node in raiz.ChildNodes[3].ChildNodes)
                {
                    String nombre = node.Token.Text;
                    if (clases.Contains(nombre))
                    {
                        ParseTreeNode m = (ParseTreeNode)clases[nombre];
                        importar(m, h);
                    }
                }
            }
            else if (raiz.ChildNodes.Count == 5)
            {
                addGlobal(raiz.ChildNodes[4], h);
                foreach (ParseTreeNode node in raiz.ChildNodes[3].ChildNodes)
                {
                    String nombre = node.Token.Text;
                    if (clases.Contains(nombre))
                    {
                        ParseTreeNode m = (ParseTreeNode)clases[nombre];
                        importar(m, h);
                    }
                }
            }
        }

        private static void addClass(ParseTreeNode raiz) {
            foreach (ParseTreeNode node in raiz.ChildNodes) {
                String name = node.ChildNodes[1].Token.Value.ToString();
                if (!clases.ContainsKey(name)) {
                    clases.Add(name, node);
                    generarImagen(name, node);
                    if (main_node == null) {
                        if (node.ChildNodes.Count == 3)
                        {
                            if (SearchMain(node.ChildNodes[2]))
                                claseActual = node;
                        }
                        else if (node.ChildNodes.Count == 2 || node.ChildNodes.Count == 4) { }
                        else
                        {
                            if (SearchMain(node.ChildNodes[4]))
                                claseActual = node;
                        }
                    }
                }
            }
            if (main_node != null) {
                generarImagen("main", main_node);
                if (main_node.ChildNodes.Count == 2) {
                    importar(claseActual, h);
                    Recorrido(main_node.ChildNodes[1], new Entorno(h));
                }
            }
        }

        private static void generarImagen(String name, ParseTreeNode raiz) {
            String stringdot = Graficador.getDot(raiz);
            DOT dot = new DOT();
            BinaryImage img = dot.ToPNG(stringdot);
            img.Save("Image/" + name + ".png");
        }

        private static bool SearchMain(ParseTreeNode raiz) {
            foreach (ParseTreeNode node in raiz.ChildNodes)
            {
                if (node.ChildNodes.Count == 1 && node.ChildNodes[0].Term.Name.ToLower() == "main") {
                    if (main_node == null) {
                        main_node = node.ChildNodes[0];
                        return true;
                    }
                    else {
                        /*Error doble main*/
                    }
                }
            }
            return false;
        }

        private static Variable addGlobal(ParseTreeNode raiz, Entorno h)
        {
            Variable b = new Variable();
            b.fila = raiz.Span.Location.Line;
            b.columna = raiz.Span.Location.Column;
            switch (raiz.Term.Name.ToLower())
            {
                case "lista_bloque":
                    {
                        foreach (ParseTreeNode node in raiz.ChildNodes)
                        {
                            addGlobal(node, h);
                        }
                        break;
                    }
                case "bloque":
                    {
                        if (raiz.ChildNodes.Count == 2)
                        {
                            bool @private = raiz.ChildNodes[0].Token.Text == "privado" ? true : false;
                            ParseTreeNode nodeD = raiz.ChildNodes[1];
                            switch (nodeD.Term.Name.ToLower())
                            {
                                case "declaracion":
                                    switch (nodeD.ChildNodes.Count)
                                    {
                                        case 2:
                                            {
                                                String tipo = nodeD.ChildNodes[0].Token.Text;
                                                if (nodeD.ChildNodes[1].ChildNodes.Count == 0)
                                                {
                                                    String nombre = nodeD.ChildNodes[1].Token.Text;
                                                    h.addVariable(nombre, new Variable(nodeD.Span.Location.Line, nodeD.Span.Location.Column, nombre, null, tipo, @private));
                                                }
                                                else
                                                {
                                                    foreach (ParseTreeNode node in nodeD.ChildNodes[1].ChildNodes)
                                                    {
                                                        h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, null, tipo, @private));
                                                    }
                                                }
                                                break;
                                            }
                                        case 3:
                                            {
                                                Variable b1 = Recorrido(nodeD.ChildNodes[2], h);
                                                String tipo = nodeD.ChildNodes[0].Token.Text;
                                                foreach (ParseTreeNode node in nodeD.ChildNodes[1].ChildNodes)
                                                {
                                                    h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, b1.dato, tipo, @private), tipo);
                                                }
                                                break;
                                            }
                                        case 4:
                                            {
                                                String tipo = nodeD.ChildNodes[0].Token.Text;
                                                int dim = nodeD.ChildNodes[3].ChildNodes.Count;
                                                if (dim == 1)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    if (dim1.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        if (data > 0)
                                                        {
                                                            Arreglo arr = new Arreglo(data);
                                                            foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                            {
                                                                h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, @private));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }
                                                }
                                                else if (dim == 2)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    Variable dim2 = Recorrido(nodeD.ChildNodes[3].ChildNodes[1], h);
                                                    if (dim1.t == TYPE.INT && dim2.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        Int32 data2 = (Int32)dim2.dato;
                                                        if (data > 0 && data2 >= 0)
                                                        {
                                                            Arreglo arr = new Arreglo(data, data2);
                                                            foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                            {
                                                                h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, @private));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }
                                                }
                                                else if (dim == 3)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    Variable dim2 = Recorrido(nodeD.ChildNodes[3].ChildNodes[1], h);
                                                    Variable dim3 = Recorrido(nodeD.ChildNodes[3].ChildNodes[2], h);
                                                    if (dim1.t == TYPE.INT && dim2.t == TYPE.INT && dim3.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        Int32 data2 = (Int32)dim2.dato;
                                                        Int32 data3 = (Int32)dim3.dato;
                                                        if (data > 0 && data2 >= 0 && data3 >= 0)
                                                        {
                                                            Arreglo arr = new Arreglo(data, data2, data3);
                                                            foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                            {
                                                                h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, @private));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }

                                                }
                                                else
                                                {
                                                    tipo = tipo.ToLower();
                                                    if (clases.Contains(tipo))
                                                    {
                                                        String name = nodeD.ChildNodes[1].Token.Text;
                                                        Clase clase = new Clase();
                                                        ParseTreeNode nodo = (ParseTreeNode)clases[tipo];
                                                        importar(nodo, clase.principal);
                                                        h.addVariable(name, new Variable(nodeD.Span.Location.Line, nodeD.Span.Location.Column, name, clase, "clase", @private));
                                                    }
                                                    else
                                                    {
                                                        /*NO EXISTE LA CLASE*/
                                                    }
                                                }
                                                break;
                                            }
                                        case 5:
                                            {
                                                String tipo = nodeD.ChildNodes[0].Token.Text;
                                                int dim = nodeD.ChildNodes[3].ChildNodes.Count;
                                                ParseTreeNode dataA = nodeD.ChildNodes[4].ChildNodes[0];
                                                if (dim == 1)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    if (dim1.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        if (data > 0)
                                                        {
                                                            if (dataA.Term.Name == "Dim1")
                                                            {
                                                                Arreglo arr = new Arreglo(data);
                                                                int i = 0;
                                                                foreach (ParseTreeNode node in dataA.ChildNodes)
                                                                {
                                                                    Variable v = Recorrido(node, h);
                                                                    arr.setData(i, v.dato);
                                                                    i++;
                                                                }
                                                                foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                                {
                                                                    h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, @private));
                                                                }
                                                            }
                                                            else
                                                            {
                                                                /*1 DIM */
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }
                                                }
                                                else if (dim == 2)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    Variable dim2 = Recorrido(nodeD.ChildNodes[3].ChildNodes[1], h);
                                                    if (dim1.t == TYPE.INT && dim2.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        Int32 data2 = (Int32)dim2.dato;
                                                        if (data > 0 && data2 >= 0)
                                                        {
                                                            if (dataA.Term.Name == "Dim2")
                                                            {
                                                                Arreglo arr = new Arreglo(data, data2);
                                                                int i = 0;
                                                                foreach (ParseTreeNode node in dataA.ChildNodes)
                                                                {
                                                                    int j = 0;
                                                                    foreach (ParseTreeNode node1 in node.ChildNodes)
                                                                    {
                                                                        Variable v = Recorrido(node1, h);
                                                                        arr.setData(j, i, v.dato);
                                                                        j++;
                                                                    }
                                                                    i++;
                                                                }
                                                                foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                                {
                                                                    h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, @private));
                                                                }
                                                            }
                                                            else
                                                            {
                                                                /*DIM 2*/
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }
                                                }
                                                else if (dim == 3)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    Variable dim2 = Recorrido(nodeD.ChildNodes[3].ChildNodes[1], h);
                                                    Variable dim3 = Recorrido(nodeD.ChildNodes[3].ChildNodes[2], h);
                                                    if (dim1.t == TYPE.INT && dim2.t == TYPE.INT && dim3.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        Int32 data2 = (Int32)dim2.dato;
                                                        Int32 data3 = (Int32)dim3.dato;
                                                        if (data > 0 && data2 >= 0 && data3 >= 0)
                                                        {
                                                            Arreglo arr = new Arreglo(data, data2, data3);
                                                            foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                            {
                                                                h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, @private));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }

                                                }
                                                break;
                                            }
                                        case 6:
                                            { break; }
                                    }
                                    break;
                                case "metodos": {
                                        String nombre = "$" + nodeD.ChildNodes[0].Token.Text;
                                        String tipo = nodeD.ChildNodes[1].Token.Text;
                                        if (tipo.ToLower().Equals("void"))
                                        {
                                            h.addVariable(nombre, new Variable(nodeD.Span.Location.Line,nodeD.Span.Location.Column,nombre,nodeD,"void",@private));
                                        }
                                        else {
                                            h.addVariable(nombre, new Variable(nodeD.Span.Location.Line, nodeD.Span.Location.Column, nombre, nodeD, "funcion", @private));
                                        }
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            ParseTreeNode nodeD = raiz.ChildNodes[0];
                            switch (nodeD.Term.Name.ToLower())
                            {
                                case "declaracion":
                                    switch (nodeD.ChildNodes.Count)
                                    {
                                        case 2:
                                            {
                                                String tipo = nodeD.ChildNodes[0].Token.Text;
                                                if (nodeD.ChildNodes[1].ChildNodes.Count == 0)
                                                {
                                                    String nombre = nodeD.ChildNodes[1].Token.Text;
                                                    h.addVariable(nombre, new Variable(nodeD.Span.Location.Line, nodeD.Span.Location.Column, nombre, null, tipo, false));
                                                }
                                                else
                                                {
                                                    foreach (ParseTreeNode node in nodeD.ChildNodes[1].ChildNodes)
                                                    {
                                                        h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, null, tipo, false));
                                                    }
                                                }
                                                break;
                                            }
                                        case 3:
                                            {
                                                Variable b1 = Recorrido(nodeD.ChildNodes[2], h);
                                                String tipo = nodeD.ChildNodes[0].Token.Text;
                                                foreach (ParseTreeNode node in nodeD.ChildNodes[1].ChildNodes)
                                                {
                                                    h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, b1.dato, tipo, false), tipo);
                                                }
                                                break;
                                            }
                                        case 4:
                                            {
                                                String tipo = nodeD.ChildNodes[0].Token.Text;
                                                int dim = nodeD.ChildNodes[3].ChildNodes.Count;
                                                if (dim == 1)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    if (dim1.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        if (data > 0)
                                                        {
                                                            Arreglo arr = new Arreglo(data);
                                                            foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                            {
                                                                h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, false));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }
                                                }
                                                else if (dim == 2)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    Variable dim2 = Recorrido(nodeD.ChildNodes[3].ChildNodes[1], h);
                                                    if (dim1.t == TYPE.INT && dim2.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        Int32 data2 = (Int32)dim2.dato;
                                                        if (data > 0 && data2 >= 0)
                                                        {
                                                            Arreglo arr = new Arreglo(data2, data);
                                                            foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                            {
                                                                h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, false));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }
                                                }
                                                else if (dim == 3)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    Variable dim2 = Recorrido(nodeD.ChildNodes[3].ChildNodes[1], h);
                                                    Variable dim3 = Recorrido(nodeD.ChildNodes[3].ChildNodes[2], h);
                                                    if (dim1.t == TYPE.INT && dim2.t == TYPE.INT && dim3.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        Int32 data2 = (Int32)dim2.dato;
                                                        Int32 data3 = (Int32)dim3.dato;
                                                        if (data > 0 && data2 >= 0 && data3 >= 0)
                                                        {
                                                            Arreglo arr = new Arreglo(data3, data2, data);
                                                            foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                            {
                                                                h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, false));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }

                                                }
                                                else
                                                {
                                                    tipo = tipo.ToLower();
                                                    if (clases.Contains(tipo))
                                                    {
                                                        String name = nodeD.ChildNodes[1].Token.Text;
                                                        Clase clase = new Clase();
                                                        ParseTreeNode nodo = (ParseTreeNode)clases[tipo];
                                                        importar(nodo, clase.principal);
                                                        h.addVariable(name, new Variable(nodeD.Span.Location.Line, nodeD.Span.Location.Column, name, clase, "clase", false));
                                                    }
                                                    else
                                                    {
                                                        /*NO EXISTE LA CLASE*/
                                                    }
                                                }
                                                break;
                                            }
                                        case 5:
                                            {
                                                String tipo = nodeD.ChildNodes[0].Token.Text;
                                                int dim = nodeD.ChildNodes[3].ChildNodes.Count;
                                                ParseTreeNode dataA = nodeD.ChildNodes[4].ChildNodes[0];
                                                if (dim == 1)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    if (dim1.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        if (data > 0)
                                                        {
                                                            if (dataA.Term.Name == "Dim1")
                                                            {
                                                                Arreglo arr = new Arreglo(data);
                                                                int i = 0;
                                                                foreach (ParseTreeNode node in dataA.ChildNodes)
                                                                {
                                                                    Variable v = Recorrido(node, h);
                                                                    arr.setData(i, v.dato);
                                                                    i++;
                                                                }
                                                                foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                                {
                                                                    h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, false));
                                                                }
                                                            }
                                                            else
                                                            {
                                                                /*1 DIM */
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }
                                                }
                                                else if (dim == 2)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    Variable dim2 = Recorrido(nodeD.ChildNodes[3].ChildNodes[1], h);
                                                    if (dim1.t == TYPE.INT && dim2.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        Int32 data2 = (Int32)dim2.dato;
                                                        if (data > 0 && data2 >= 0)
                                                        {
                                                            if (dataA.Term.Name == "Dim2")
                                                            {
                                                                Arreglo arr = new Arreglo(data2, data);
                                                                int i = 0;
                                                                foreach (ParseTreeNode node in dataA.ChildNodes)
                                                                {
                                                                    int j = 0;
                                                                    foreach (ParseTreeNode node1 in node.ChildNodes)
                                                                    {
                                                                        Variable v = Recorrido(node1, h);
                                                                        arr.setData(j, i, v.dato);
                                                                        j++;
                                                                    }
                                                                    i++;
                                                                }
                                                                foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                                {
                                                                    h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, false));
                                                                }
                                                            }
                                                            else
                                                            {
                                                                /*DIM 2*/
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }
                                                }
                                                else if (dim == 3)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    Variable dim2 = Recorrido(nodeD.ChildNodes[3].ChildNodes[1], h);
                                                    Variable dim3 = Recorrido(nodeD.ChildNodes[3].ChildNodes[2], h);
                                                    if (dim1.t == TYPE.INT && dim2.t == TYPE.INT && dim3.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        Int32 data2 = (Int32)dim2.dato;
                                                        Int32 data3 = (Int32)dim3.dato;
                                                        if (data > 0 && data2 >= 0 && data3 >= 0)
                                                        {
                                                            if (dataA.Term.Name == "Dim3")
                                                            {
                                                                Arreglo arr = new Arreglo(data3, data2, data);
                                                                int i = 0;
                                                                foreach (ParseTreeNode node in dataA.ChildNodes)
                                                                {
                                                                    int j = 0;
                                                                    foreach (ParseTreeNode nod1 in node.ChildNodes)
                                                                    {
                                                                        int k = 0;
                                                                        foreach (ParseTreeNode node2 in nod1.ChildNodes)
                                                                        {
                                                                            Variable v = Recorrido(node2, h);
                                                                            arr.setData(k, j, i, v.dato);
                                                                            k++;
                                                                        }
                                                                        j++;
                                                                    }
                                                                    i++;
                                                                }
                                                                foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                                {
                                                                    h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, false));
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }

                                                }
                                                break;
                                            }
                                    }
                                    break;
                                case "metodos":
                                    {
                                        String nombre = "$" + nodeD.ChildNodes[0].Token.Text;
                                        String tipo = nodeD.ChildNodes[1].Token.Text;
                                        if (tipo.ToLower().Equals("void"))
                                        {
                                            h.addVariable(nombre, new Variable(nodeD.Span.Location.Line, nodeD.Span.Location.Column, nombre, nodeD, "void", false));
                                        }
                                        else
                                        {
                                            h.addVariable(nombre, new Variable(nodeD.Span.Location.Line, nodeD.Span.Location.Column, nombre, nodeD, "funcion", false));
                                        }
                                        break;
                                    }
                                case "asignacion":

                                    break;
                            }
                        }
                        break;
                    }
            }
            return b;
        }

        public static Variable Recorrido(ParseTreeNode raiz, Entorno h) {
            Variable b = new Variable();
            b.fila = raiz.Span.Location.Line;
            b.columna = raiz.Span.Location.Column;
            switch (raiz.Term.Name.ToLower()) {
                case "listamsentencia":
                    {
                        foreach (ParseTreeNode node in raiz.ChildNodes)
                        {
                            Variable nueva = Recorrido(node, h);
                            if (nueva.t == TYPE.CONTINUAR || nueva.t == TYPE.SALIR)
                            {
                                b.t = nueva.t;
                                break;
                            }
                            else if (nueva.t == TYPE.RETURN) {
                                return nueva;
                            }
                        }
                        break;
                    }
                case "msentencia":
                    {
                        if (raiz.ChildNodes.Count == 1)
                        {
                            ParseTreeNode nodeD = raiz.ChildNodes[0];
                            switch (nodeD.Term.Name.ToLower())
                            {
                                case "declaracion":
                                    switch (nodeD.ChildNodes.Count) {
                                        case 2:
                                            {
                                                String tipo = nodeD.ChildNodes[0].Token.Text;
                                                if (nodeD.ChildNodes[1].ChildNodes.Count == 0)
                                                {
                                                    String nombre = nodeD.ChildNodes[1].Token.Text;
                                                    h.addVariable(nombre, new Variable(nodeD.Span.Location.Line, nodeD.Span.Location.Column, nombre, null, tipo, false));
                                                }
                                                else {
                                                    foreach (ParseTreeNode node in nodeD.ChildNodes[1].ChildNodes)
                                                    {
                                                        h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, null, tipo, false));
                                                    }
                                                }
                                                break;
                                            }
                                        case 3:
                                            {
                                                Variable b1 = Recorrido(nodeD.ChildNodes[2], h);
                                                String tipo = nodeD.ChildNodes[0].Token.Text;
                                                foreach (ParseTreeNode node in nodeD.ChildNodes[1].ChildNodes)
                                                {
                                                    h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, b1.dato, tipo, false), tipo);
                                                }
                                                break;
                                            }
                                        case 4:
                                            {
                                                String tipo = nodeD.ChildNodes[0].Token.Text;
                                                int dim = nodeD.ChildNodes[3].ChildNodes.Count;
                                                if (dim == 1)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    if (dim1.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        if (data > 0)
                                                        {
                                                            Arreglo arr = new Arreglo(data);
                                                            foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                            {
                                                                h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, false));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }
                                                }
                                                else if (dim == 2)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    Variable dim2 = Recorrido(nodeD.ChildNodes[3].ChildNodes[1], h);
                                                    if (dim1.t == TYPE.INT && dim2.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        Int32 data2 = (Int32)dim2.dato;
                                                        if (data > 0 && data2 >= 0)
                                                        {
                                                            Arreglo arr = new Arreglo(data2, data);
                                                            foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                            {
                                                                h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, false));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }
                                                }
                                                else if (dim == 3)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    Variable dim2 = Recorrido(nodeD.ChildNodes[3].ChildNodes[1], h);
                                                    Variable dim3 = Recorrido(nodeD.ChildNodes[3].ChildNodes[2], h);
                                                    if (dim1.t == TYPE.INT && dim2.t == TYPE.INT && dim3.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        Int32 data2 = (Int32)dim2.dato;
                                                        Int32 data3 = (Int32)dim3.dato;
                                                        if (data > 0 && data2 >= 0 && data3 >= 0)
                                                        {
                                                            Arreglo arr = new Arreglo(data3, data2, data);
                                                            foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                            {
                                                                h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, false));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }

                                                }
                                                else {
                                                    tipo = tipo.ToLower();
                                                    if (clases.Contains(tipo))
                                                    {
                                                        String name = nodeD.ChildNodes[1].Token.Text;
                                                        Clase clase = new Clase();
                                                        ParseTreeNode nodo = (ParseTreeNode)clases[tipo];
                                                        importar(nodo, clase.principal);
                                                        h.addVariable(name, new Variable(nodeD.Span.Location.Line,nodeD.Span.Location.Column,name,clase,"clase",false));
                                                    }
                                                    else {
                                                        /*NO EXISTE LA CLASE*/
                                                    }
                                                }
                                                break;
                                            }
                                        case 5:
                                            {
                                                String tipo = nodeD.ChildNodes[0].Token.Text;
                                                int dim = nodeD.ChildNodes[3].ChildNodes.Count;
                                                ParseTreeNode dataA = nodeD.ChildNodes[4].ChildNodes[0];
                                                if (dim == 1)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    if (dim1.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        if (data > 0)
                                                        {
                                                            if (dataA.Term.Name == "Dim1")
                                                            {
                                                                Arreglo arr = new Arreglo(data);
                                                                int i = 0;
                                                                foreach (ParseTreeNode node in dataA.ChildNodes) {
                                                                    Variable v = Recorrido(node, h);
                                                                    arr.setData(i, v.dato);
                                                                    i++;
                                                                }
                                                                foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                                {
                                                                    h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, false));
                                                                }
                                                            }
                                                            else {
                                                                /*1 DIM */
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }
                                                }
                                                else if (dim == 2)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    Variable dim2 = Recorrido(nodeD.ChildNodes[3].ChildNodes[1], h);
                                                    if (dim1.t == TYPE.INT && dim2.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        Int32 data2 = (Int32)dim2.dato;
                                                        if (data > 0 && data2 >= 0)
                                                        {
                                                            if (dataA.Term.Name == "Dim2")
                                                            {
                                                                Arreglo arr = new Arreglo(data2, data);
                                                                int i = 0;
                                                                foreach (ParseTreeNode node in dataA.ChildNodes) {
                                                                    int j = 0;
                                                                    foreach (ParseTreeNode node1 in node.ChildNodes)
                                                                    {
                                                                        Variable v = Recorrido(node1, h);
                                                                        arr.setData(j, i, v.dato);
                                                                        j++;
                                                                    }
                                                                    i++;
                                                                }
                                                                foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                                {
                                                                    h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, false));
                                                                }
                                                            }
                                                            else {
                                                                /*DIM 2*/
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }
                                                }
                                                else if (dim == 3)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    Variable dim2 = Recorrido(nodeD.ChildNodes[3].ChildNodes[1], h);
                                                    Variable dim3 = Recorrido(nodeD.ChildNodes[3].ChildNodes[2], h);
                                                    if (dim1.t == TYPE.INT && dim2.t == TYPE.INT && dim3.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        Int32 data2 = (Int32)dim2.dato;
                                                        Int32 data3 = (Int32)dim3.dato;
                                                        if (data > 0 && data2 >= 0 && data3 >= 0)
                                                        {
                                                            if (dataA.Term.Name == "Dim3") {
                                                                Arreglo arr = new Arreglo(data3, data2, data);
                                                                int i = 0;
                                                                foreach (ParseTreeNode node in dataA.ChildNodes) {
                                                                    int j = 0;
                                                                    foreach (ParseTreeNode nod1 in node.ChildNodes) {
                                                                        int k = 0;
                                                                        foreach (ParseTreeNode node2 in nod1.ChildNodes) {
                                                                            Variable v = Recorrido(node2, h);
                                                                            arr.setData(k, j, i, v.dato);
                                                                            k++;
                                                                        }
                                                                        j++;
                                                                    }
                                                                    i++;
                                                                }
                                                                foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                                {
                                                                    h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, false));
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }

                                                }
                                                break;
                                            }
                                    }
                                    break;
                                case "asignacion":
                                    if (nodeD.ChildNodes.Count == 2)
                                    {
                                        if (nodeD.ChildNodes[0].Term.Name == "D1")
                                        {
                                            ParseTreeNode nodo = nodeD.ChildNodes[0];
                                            Variable clase = (Variable) h.getValue(nodo.ChildNodes[0].Token.Text);
                                            if (clase != null && clase.dato is Clase) {
                                                Clase c = (Clase)clase.dato;
                                                String nombrevar = nodo.ChildNodes[1].Token.Text;
                                                Variable var = (Variable)c.principal.getValue(nombrevar);
                                                if (var != null)
                                                {
                                                    if (nodeD.ChildNodes[1].Term.Name == "E")
                                                    {
                                                        Variable dato = Recorrido(nodeD.ChildNodes[1], h);
                                                        if (dato.esnum() && var.esnum() || dato.t == var.t)
                                                        {
                                                            var.dato = dato.dato;
                                                        }
                                                    }
                                                    else if (nodeD.ChildNodes[1].Term.Name == "++")
                                                    {
                                                        if (var.t == TYPE.INT)
                                                        {
                                                            var.dato = (Int32)var.dato + 1;
                                                        }
                                                        else if (var.t == TYPE.DOUBLE)
                                                        {
                                                            var.dato = (Double)var.dato + 1;
                                                        }
                                                        else if (var.t == TYPE.CHAR) {
                                                            var.dato = (Char)var.dato + 1;
                                                        }
                                                    }
                                                    else {
                                                        if (var.t == TYPE.INT)
                                                        {
                                                            var.dato = (Int32)var.dato - 1;
                                                        }
                                                        else if (var.t == TYPE.DOUBLE)
                                                        {
                                                            var.dato = (Double)var.dato - 1;
                                                        }
                                                        else if (var.t == TYPE.CHAR)
                                                        {
                                                            var.dato = (Char)var.dato - 1;
                                                        }
                                                    }
                                                }
                                                else {

                                                }
                                            }
                                        }
                                        else
                                        {
                                            Variable buscar = (Variable)h.getValue(nodeD.ChildNodes[0].Token.Text);
                                            if (buscar != null)
                                            {
                                                if (nodeD.ChildNodes[1].Term.Name == "E")
                                                {
                                                    Variable valor = Recorrido(nodeD.ChildNodes[1], h);
                                                    if (valor.t == TYPE.INT || valor.t == TYPE.DOUBLE && buscar.t == TYPE.DOUBLE)
                                                    {
                                                        buscar.dato = valor.dato;
                                                    }
                                                    else if (valor.t == buscar.t)
                                                    {
                                                        buscar.dato = valor.dato;
                                                    }
                                                }
                                                else if (nodeD.ChildNodes[1].Term.Name == "++")
                                                {
                                                    if (buscar.t == TYPE.INT)
                                                    {
                                                        buscar.dato = (Int32)buscar.dato + 1;
                                                    }
                                                    else if (buscar.t == TYPE.DOUBLE)
                                                    {
                                                        buscar.dato = (Double)buscar.dato + 1;
                                                    }
                                                    else if (buscar.t == TYPE.CHAR)
                                                    {
                                                        buscar.dato = (Char)buscar.dato + 1;
                                                    }
                                                    else
                                                    {

                                                    }
                                                }
                                                else if (nodeD.ChildNodes[1].Term.Name == "--")
                                                {
                                                    if (buscar.t == TYPE.INT)
                                                    {
                                                        buscar.dato = (Int32)buscar.dato - 1;
                                                    }
                                                    else if (buscar.t == TYPE.DOUBLE)
                                                    {
                                                        buscar.dato = (Double)buscar.dato - 1;
                                                    }
                                                    else if (buscar.t == TYPE.CHAR)
                                                    {
                                                        buscar.dato = (Char)buscar.dato - 1;
                                                    }
                                                    else
                                                    {

                                                    }
                                                }
                                                else
                                                {

                                                }
                                            }
                                        }
                                    }
                                    else if (nodeD.ChildNodes.Count == 3) {
                                        if (nodeD.ChildNodes[0].Term.Name == "D1")
                                        {
                                            ParseTreeNode nodo = nodeD.ChildNodes[0];
                                            Variable clase = (Variable)h.getValue(nodo.ChildNodes[0].Token.Text);
                                            if (clase != null && clase.dato is Clase) {
                                                Clase c = (Clase)clase.dato;
                                                String nombrevar = nodo.ChildNodes[1].Token.Text;
                                                Variable var = (Variable)c.principal.getValue(nombrevar);
                                                if(var!=null && var.dato is Arreglo)
                                                {
                                                    Arreglo arreglo = (Arreglo)var.dato;
                                                    ParseTreeNode dimension = nodeD.ChildNodes[1];
                                                    Variable dato = Recorrido(nodeD.ChildNodes[2], h);
                                                    if (dimension.ChildNodes.Count == 1)
                                                    {
                                                        Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                        if (pos1.t == TYPE.INT)
                                                        {
                                                            arreglo.setData((int)pos1.dato, dato);
                                                        }
                                                    }
                                                    else if (dimension.ChildNodes.Count == 2)
                                                    {
                                                        Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                        Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                                        if (pos1.t == TYPE.INT && pos2.t == TYPE.INT)
                                                        {
                                                            arreglo.setData((int)pos1.dato, (int)pos2.dato, dato);
                                                        }
                                                    }
                                                    else {
                                                        Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                        Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                                        Variable pos3 = Recorrido(dimension.ChildNodes[2], h);
                                                        if (pos1.t == TYPE.INT && pos2.t == TYPE.INT && pos3.t == TYPE.INT)
                                                        {
                                                            arreglo.setData((int)pos1.dato, (int)pos2.dato, (int)pos3.dato, dato);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Object buscar = h.getValue(nodeD.ChildNodes[0].Token.Text);
                                            if (buscar != null)
                                            {
                                                if (nodeD.ChildNodes[1].Term.Name == "Dimension")
                                                {
                                                    Arreglo arreglo = (Arreglo)((Variable)buscar).dato;
                                                    ParseTreeNode dimension = nodeD.ChildNodes[1];
                                                    Variable dato = Recorrido(nodeD.ChildNodes[2], h);
                                                    if (dimension.ChildNodes.Count == 1)
                                                    {
                                                        Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                        if (pos1.t == TYPE.INT)
                                                        {
                                                            arreglo.setData((int)pos1.dato, dato);
                                                        }
                                                    }
                                                    else if (dimension.ChildNodes.Count == 2)
                                                    {
                                                        Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                        Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                                        if (pos1.t == TYPE.INT && pos2.t == TYPE.INT)
                                                        {
                                                            arreglo.setData((int)pos1.dato, (int)pos2.dato, dato);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                        Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                                        Variable pos3 = Recorrido(dimension.ChildNodes[2], h);
                                                        if (pos1.t == TYPE.INT && pos2.t == TYPE.INT && pos3.t == TYPE.INT)
                                                        {
                                                            arreglo.setData((int)pos1.dato, (int)pos2.dato, (int)pos3.dato, dato);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Variable var = (Variable)buscar;
                                                    ParseTreeNode nodo = (ParseTreeNode)clases[nodeD.ChildNodes[2].Token.Text];
                                                    Clase clase = new Clase();
                                                    importar(nodo, clase.principal);
                                                    h.changeValue(var.nombre, clase);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case "bloqueprint":
                                    {
                                        Variable imp = Recorrido(nodeD.ChildNodes[1], h);
                                        if (imp.t != TYPE.ERROR)
                                        {
                                            Form1.console.AppendText(imp.dato.ToString() + "\n");
                                        }
                                        break;
                                    }
                                case "bloqueshow":
                                    {
                                        if (nodeD.ChildNodes.Count == 2)
                                        {
                                            Variable body = Recorrido(nodeD.ChildNodes[1], h);
                                            if (body.t != TYPE.ERROR && body.dato!=null)
                                            {
                                                MessageBox.Show(body.dato.ToString());
                                            }
                                        }
                                        else
                                        {
                                            Variable title = Recorrido(nodeD.ChildNodes[1], h);
                                            Variable body = Recorrido(nodeD.ChildNodes[2], h);
                                            if (title.t != TYPE.ERROR && body.t != TYPE.ERROR)
                                            {
                                                MessageBox.Show(body.dato.ToString(), title.dato.ToString());
                                            }
                                        }
                                        break;
                                    }
                                default:
                                    b = Recorrido(nodeD, h);
                                    break;
                            }
                        }
                        else
                        {
                            bool @private = raiz.ChildNodes[0].Token.Text == "privado" ? true : false;
                            ParseTreeNode nodeD = raiz.ChildNodes[1];
                            switch (nodeD.Term.Name.ToLower())
                            {
                                case "declaracion":
                                    switch (nodeD.ChildNodes.Count)
                                    {
                                        case 2:
                                            {
                                                String tipo = nodeD.ChildNodes[0].Token.Text;
                                                if (nodeD.ChildNodes[1].ChildNodes.Count == 0)
                                                {
                                                    String nombre = nodeD.ChildNodes[1].Token.Text;
                                                    h.addVariable(nombre, new Variable(nodeD.Span.Location.Line, nodeD.Span.Location.Column, nombre, null, tipo, @private));
                                                }
                                                else
                                                {
                                                    foreach (ParseTreeNode node in nodeD.ChildNodes[1].ChildNodes)
                                                    {
                                                        h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, null, tipo, @private));
                                                    }
                                                }
                                                break;
                                            }
                                        case 3:
                                            {
                                                Variable b1 = Recorrido(nodeD.ChildNodes[2], h);
                                                String tipo = nodeD.ChildNodes[0].Token.Text;
                                                foreach (ParseTreeNode node in nodeD.ChildNodes[1].ChildNodes)
                                                {
                                                    h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, b1.dato, tipo, @private), tipo);
                                                }
                                                break;
                                            }
                                        case 4:
                                            {
                                                String tipo = nodeD.ChildNodes[0].Token.Text;
                                                int dim = nodeD.ChildNodes[3].ChildNodes.Count;
                                                if (dim == 1)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    if (dim1.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        if (data > 0)
                                                        {
                                                            Arreglo arr = new Arreglo(data);
                                                            foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                            {
                                                                h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, @private));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }
                                                }
                                                else if (dim == 2)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    Variable dim2 = Recorrido(nodeD.ChildNodes[3].ChildNodes[1], h);
                                                    if (dim1.t == TYPE.INT && dim2.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        Int32 data2 = (Int32)dim2.dato;
                                                        if (data > 0 && data2 >= 0)
                                                        {
                                                            Arreglo arr = new Arreglo(data, data2);
                                                            foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                            {
                                                                h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, @private));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }
                                                }
                                                else if (dim == 3)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    Variable dim2 = Recorrido(nodeD.ChildNodes[3].ChildNodes[1], h);
                                                    Variable dim3 = Recorrido(nodeD.ChildNodes[3].ChildNodes[2], h);
                                                    if (dim1.t == TYPE.INT && dim2.t == TYPE.INT && dim3.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        Int32 data2 = (Int32)dim2.dato;
                                                        Int32 data3 = (Int32)dim3.dato;
                                                        if (data > 0 && data2 >= 0 && data3 >= 0)
                                                        {
                                                            Arreglo arr = new Arreglo(data, data2, data3);
                                                            foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                            {
                                                                h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, @private));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }

                                                }
                                                else
                                                {
                                                    tipo = tipo.ToLower();
                                                    if (clases.Contains(tipo))
                                                    {
                                                        String name = nodeD.ChildNodes[1].Token.Text;
                                                        Clase clase = new Clase();
                                                        ParseTreeNode nodo = (ParseTreeNode)clases[tipo];
                                                        importar(nodo, clase.principal);
                                                        h.addVariable(name, new Variable(nodeD.Span.Location.Line, nodeD.Span.Location.Column, name, clase, "clase", @private));
                                                    }
                                                    else
                                                    {
                                                        /*NO EXISTE LA CLASE*/
                                                    }
                                                }
                                                break;
                                            }
                                        case 5:
                                            {
                                                String tipo = nodeD.ChildNodes[0].Token.Text;
                                                int dim = nodeD.ChildNodes[3].ChildNodes.Count;
                                                ParseTreeNode dataA = nodeD.ChildNodes[4].ChildNodes[0];
                                                if (dim == 1)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    if (dim1.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        if (data > 0)
                                                        {
                                                            if (dataA.Term.Name == "Dim1")
                                                            {
                                                                Arreglo arr = new Arreglo(data);
                                                                int i = 0;
                                                                foreach (ParseTreeNode node in dataA.ChildNodes)
                                                                {
                                                                    Variable v = Recorrido(node, h);
                                                                    arr.setData(i, v.dato);
                                                                    i++;
                                                                }
                                                                foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                                {
                                                                    h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, @private));
                                                                }
                                                            }
                                                            else
                                                            {
                                                                /*1 DIM */
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }
                                                }
                                                else if (dim == 2)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    Variable dim2 = Recorrido(nodeD.ChildNodes[3].ChildNodes[1], h);
                                                    if (dim1.t == TYPE.INT && dim2.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        Int32 data2 = (Int32)dim2.dato;
                                                        if (data > 0 && data2 >= 0)
                                                        {
                                                            if (dataA.Term.Name == "Dim2")
                                                            {
                                                                Arreglo arr = new Arreglo(data, data2);
                                                                int i = 0;
                                                                foreach (ParseTreeNode node in dataA.ChildNodes)
                                                                {
                                                                    int j = 0;
                                                                    foreach (ParseTreeNode node1 in node.ChildNodes)
                                                                    {
                                                                        Variable v = Recorrido(node1, h);
                                                                        arr.setData(j, i, v.dato);
                                                                        j++;
                                                                    }
                                                                    i++;
                                                                }
                                                                foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                                {
                                                                    h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, @private));
                                                                }
                                                            }
                                                            else
                                                            {
                                                                /*DIM 2*/
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }
                                                }
                                                else if (dim == 3)
                                                {
                                                    Variable dim1 = Recorrido(nodeD.ChildNodes[3].ChildNodes[0], h);
                                                    Variable dim2 = Recorrido(nodeD.ChildNodes[3].ChildNodes[1], h);
                                                    Variable dim3 = Recorrido(nodeD.ChildNodes[3].ChildNodes[2], h);
                                                    if (dim1.t == TYPE.INT && dim2.t == TYPE.INT && dim3.t == TYPE.INT)
                                                    {
                                                        Int32 data = (Int32)dim1.dato;
                                                        Int32 data2 = (Int32)dim2.dato;
                                                        Int32 data3 = (Int32)dim3.dato;
                                                        if (data > 0 && data2 >= 0 && data3 >= 0)
                                                        {
                                                            Arreglo arr = new Arreglo(data, data2, data3);
                                                            foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                            {
                                                                h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, @private));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                    }

                                                }
                                                break;
                                            }
                                        case 6:
                                            { break; }
                                    }
                                    break;
                                default:
                                    {
                                        Variable nueva = Recorrido(raiz.ChildNodes[1], h);
                                        b.t = TYPE.RETURN;
                                        b.taux = nueva.t;
                                        b.dato = nueva.dato;
                                        return b;
                                    }
                            }
                        }
                        break;
                    }
                case "continuar":
                    {
                        b.dato = null;
                        b.t = TYPE.CONTINUAR;
                        break;
                    }
                case "salir":
                    {
                        b.dato = null;
                        b.t = TYPE.SALIR;
                        break;
                    }
                case "bloqueif": {
                        ParseTreeNode bloqueif = raiz.ChildNodes[0];
                        Variable condicion = Recorrido(bloqueif.ChildNodes[1], h);
                        if (raiz.ChildNodes.Count == 1)
                        {
                            if (condicion.t == TYPE.BOOL)
                            {
                                if ((bool)condicion.dato)
                                {
                                    b.t = TYPE.BOOL;
                                    b.dato = true;
                                    if (bloqueif.ChildNodes.Count == 3) {
                                        Entorno nuevo = new Entorno(h);
                                        Variable nueva = Recorrido(bloqueif.ChildNodes[2], nuevo);
                                        if (nueva.t == TYPE.CONTINUAR)
                                            b.t = TYPE.CONTINUAR;
                                        else if (nueva.t == TYPE.SALIR)
                                            break;
                                        else if (nueva.t == TYPE.RETURN) {
                                            return nueva;
                                        }
                                    }
                                }
                            }
                            else {
                                /*ERROR DE TIPO*/
                            }
                        }
                        else if (raiz.ChildNodes.Count == 2)
                        {
                            if (condicion.t == TYPE.BOOL)
                            {
                                if ((bool)condicion.dato)
                                {
                                    b.t = TYPE.BOOL;
                                    b.dato = true;
                                    if (bloqueif.ChildNodes.Count == 3)
                                    {
                                        Entorno nuevo = new Entorno(h);
                                        Variable nueva = Recorrido(bloqueif.ChildNodes[2], nuevo);
                                        if (nueva.t == TYPE.SALIR) { break; }
                                        else if (nueva.t == TYPE.CONTINUAR) { b.t = TYPE.CONTINUAR; }
                                        else if(nueva.t == TYPE.RETURN) { return nueva; }
                                    }
                                }
                                else {
                                    ParseTreeNode nuevo = raiz.ChildNodes[1];
                                    if (nuevo.Term.Name == "ELSE")
                                    {
                                        if (nuevo.ChildNodes.Count == 2) {
                                            Entorno h1 = new Entorno(h);
                                            Variable nueva = Recorrido(nuevo.ChildNodes[1],h1);
                                            if (nueva.t == TYPE.CONTINUAR)
                                                b.t = TYPE.CONTINUAR;
                                            else if(nueva.t == TYPE.RETURN) { return nueva; }
                                        }
                                    }
                                    //PENDIENTE DE RETURN
                                    else {
                                        Variable nueva = Recorrido(nuevo, h);
                                        if (nueva.t == TYPE.CONTINUAR)
                                            b.t = TYPE.CONTINUAR;
                                        else if(nueva.t == TYPE.RETURN) { b.t = TYPE.RETURN; b.taux = nueva.taux; b.dato = nueva.dato; return b; }
                                    }
                                }
                            }
                            else
                            {
                                /*ERROR DE TIPO*/
                            }
                        }
                        else if (raiz.ChildNodes.Count == 3) {
                            if (condicion.t == TYPE.BOOL)
                            {
                                if ((bool)condicion.dato)
                                {
                                    b.t = TYPE.BOOL;
                                    b.dato = true;
                                    if (bloqueif.ChildNodes.Count == 3)
                                    {
                                        Entorno nuevo = new Entorno(h);
                                        Variable nueva = Recorrido(bloqueif.ChildNodes[2], nuevo);
                                        if (nueva.t == TYPE.CONTINUAR)
                                            b.t = TYPE.CONTINUAR;
                                        else if (nueva.t == TYPE.RETURN) {
                                            b.t = TYPE.RETURN;
                                            b.taux = nueva.taux;
                                            b.dato = nueva.dato;
                                            return b;
                                        }
                                    }
                                }
                                else {
                                    Variable condicion2 = Recorrido(raiz.ChildNodes[1], h);
                                    if (!(bool)condicion2.dato)
                                    {
                                        ParseTreeNode nuevo = raiz.ChildNodes[2];
                                        if (nuevo.ChildNodes.Count == 2)
                                        {
                                            Entorno h1 = new Entorno(h);
                                            Variable nueva = Recorrido(nuevo.ChildNodes[1], h1);
                                            if (nueva.t == TYPE.CONTINUAR)
                                                b.t = TYPE.CONTINUAR;
                                        }
                                    }
                                    else if (condicion2.t == TYPE.CONTINUAR)
                                        b.t = TYPE.CONTINUAR;
                                    else if(condicion2.t == TYPE.RETURN) { /*pendiente b.t = TYPE.RETURN; b.taux = condicion2.taux;*/  }
                                }
                            }
                            else
                            {
                                /*ERROR DE TIPO*/
                            }
                        }
                        break;
                    }

                case "bloqueelseif": {
                        b.dato = false;
                        foreach (ParseTreeNode node in raiz.ChildNodes) {
                            Variable condicion = Recorrido(node, h);
                            if ((bool)condicion.dato) {
                                b.dato = true;
                                if (condicion.t == TYPE.CONTINUAR)
                                    b.t = TYPE.CONTINUAR;
                                break;
                            }
                        }
                        break;
                    }

                case "bloquewhile": {
                        Variable condicion = Recorrido(raiz.ChildNodes[1], h);
                        if (condicion.t == TYPE.BOOL)
                        {
                            while ((bool)condicion.dato) {
                                if (raiz.ChildNodes.Count == 3) {
                                    Entorno h1 = new Entorno(h);
                                    Variable nueva = Recorrido(raiz.ChildNodes[2], h1);
                                    if (nueva.t == TYPE.SALIR) {
                                        break;
                                    }
                                    else if(nueva.t == TYPE.RETURN)
                                    {
                                        return nueva;
                                    }
                                }
                                condicion = Recorrido(raiz.ChildNodes[1], h);
                            }
                        }
                        else {
                            /*ERROR DE TIPO DE DATO*/
                        }
                        break;
                    }

                case "bloquerepetir": {
                        Variable cantidad = Recorrido(raiz.ChildNodes[1], h);
                        if (cantidad.t == TYPE.INT)
                        {
                            if (raiz.ChildNodes.Count == 3) {
                                int num = (int)cantidad.dato;
                                for (int i = 0; i < num; i++)
                                {
                                    Entorno h1 = new Entorno(h);
                                    Variable nueva = Recorrido(raiz.ChildNodes[2], h1);
                                    if (nueva.t == TYPE.SALIR)
                                    {
                                        break;
                                    }
                                    else if(nueva.t == TYPE.RETURN)
                                    {
                                        return nueva;
                                    }
                                }
                            }
                        }
                        else {

                        /*error tipo dato*/}
                        break;
                    }

                case "bloquehacer":
                    {
                        if (raiz.ChildNodes.Count == 4) {
                            Variable sentencia = Recorrido(raiz.ChildNodes[3], h);
                            if (sentencia.t == TYPE.BOOL) {
                                do
                                {
                                    Entorno h1 = new Entorno(h);
                                    Variable nueva = Recorrido(raiz.ChildNodes[1], h1);
                                    if (nueva.t == TYPE.SALIR)
                                    {
                                        break;
                                    }
                                    else if(nueva.t == TYPE.RETURN)
                                    {
                                        return nueva;
                                    }
                                    sentencia = Recorrido(raiz.ChildNodes[3], h);
                                } while ((bool)sentencia.dato);
                            }
                        }
                        break;
                    }
                case "bloquefor":
                    {
                        if (raiz.ChildNodes.Count == 6)
                        {
                            Variable buscar = (Variable)h.getValue(raiz.ChildNodes[1].Token.Text);
                            if (buscar != null) {
                                Variable dato = Recorrido(raiz.ChildNodes[2],h);
                                buscar.dato = dato.dato;
                                buscar.t = dato.t;
                                Variable condicion = Recorrido(raiz.ChildNodes[3], h);
                                while ((bool)condicion.dato) {
                                    Entorno h1 = new Entorno(h);
                                    Variable nueva = Recorrido(raiz.ChildNodes[5], h1);
                                    if (nueva.t == TYPE.SALIR)
                                    {
                                        break;
                                    }
                                    else if(nueva.t == TYPE.RETURN)
                                    {
                                        return nueva; 
                                    }
                                    Recorrido(raiz.ChildNodes[4], h);
                                    condicion = Recorrido(raiz.ChildNodes[3], h1);
                                }
                            }
                        }
                        else {
                            Entorno h1 = new Entorno(h);
                            Variable dato = Recorrido(raiz.ChildNodes[3], h);
                            String tipo = raiz.ChildNodes[1].Token.Text;
                            ParseTreeNode name = raiz.ChildNodes[2];
                            h1.addVariable(name.Token.Text, new Variable(name.Span.Location.Line, name.Span.Location.Column, name.Token.Text, dato.dato, tipo, false), tipo);
                            Variable condicion = Recorrido(raiz.ChildNodes[4], h1);
                            while ((bool)condicion.dato) {
                                Variable nueva = Recorrido(raiz.ChildNodes[6], h1);
                                if (nueva.t == TYPE.SALIR)
                                {
                                    break;
                                }
                                else if (nueva.t == TYPE.RETURN) {
                                    return nueva;
                                }
                                Recorrido(raiz.ChildNodes[5], h1);
                                condicion = Recorrido(raiz.ChildNodes[4], h1);
                            }
                        }
                        break;
                    }
                case "elseif":
                    {
                        b.dato = false;
                        b.t = TYPE.BOOL;
                        Variable condicion = Recorrido(raiz.ChildNodes[2], h);
                        if (condicion.t == TYPE.BOOL)
                        {
                            if ((bool)condicion.dato)
                            {
                                b.dato = true;
                                if (raiz.ChildNodes.Count == 4) {
                                    Entorno h1 = new Entorno(h);
                                    Variable nueva = Recorrido(raiz.ChildNodes[3], h1);
                                    if (nueva.t == TYPE.CONTINUAR)
                                        b.t = TYPE.CONTINUAR;
                                }
                            }
                        }
                        else {
                            /*ERROR DE CONDICION*/
                        }
                        break;
                    }
                case "e":
                    {
                        int n = raiz.ChildNodes.Count;
                        if (n == 3)
                        {
                            Variable s1 = Recorrido(raiz.ChildNodes[0], h);
                            Variable s2 = Recorrido(raiz.ChildNodes[2], h);
                            switch (raiz.ChildNodes[1].Term.Name)
                            {
                                case "+":
                                    switch (s1.t)
                                    {
                                        case TYPE.INT:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Int32)s1.dato + (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.STRING;
                                                    b.dato = s1.dato.ToString() + s2.dato.ToString();
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = Double.Parse(s1.dato.ToString()) + Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Int32)s1.dato + (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Int32)s1.dato + ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.STRING:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.STRING;
                                                    b.dato = s1.dato.ToString() + s2.dato.ToString();
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.STRING;
                                                    b.dato = s1.dato.ToString() + s2.dato.ToString();
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.STRING;
                                                    b.dato = s1.dato.ToString() + s2.dato.ToString();
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.STRING;
                                                    b.dato = s1.dato.ToString() + s2.dato.ToString();
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.ERROR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                            }
                                            break;
                                        case TYPE.DOUBLE:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = Double.Parse(s1.dato.ToString()) + Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.STRING;
                                                    b.dato = s1.dato.ToString() + s2.dato.ToString();
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = Double.Parse(s1.dato.ToString()) + Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = (Double)s1.dato + (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = (Double)s1.dato + ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.CHAR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Char)s1.dato + (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.STRING;
                                                    b.dato = s1.dato.ToString() + s2.dato.ToString();
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = (Char)s1.dato + Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Char)s1.dato + (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Char)s1.dato + ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.BOOL:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.INT;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) + (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) + Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.INT;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) + (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Boolean)s1.dato || (Boolean)s2.dato;
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.ERROR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    break;
                                                case TYPE.STRING:
                                                    break;
                                                case TYPE.DOUBLE:
                                                    break;
                                                case TYPE.CHAR:
                                                    break;
                                                case TYPE.BOOL:
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                case "-":
                                    switch (s1.t)
                                    {
                                        case TYPE.INT:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Int32)s1.dato - (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*ERROR*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = Double.Parse(s1.dato.ToString()) - Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Int32)s1.dato - (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Int32)s1.dato - ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.STRING:
                                            b.t = TYPE.ERROR;
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    break;
                                                case TYPE.STRING:
                                                    break;
                                                case TYPE.DOUBLE:
                                                    break;
                                                case TYPE.CHAR:
                                                    break;
                                                case TYPE.BOOL:
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.DOUBLE:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = Double.Parse(s1.dato.ToString()) - Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = Double.Parse(s1.dato.ToString()) - Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = (Double)s1.dato - (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = (Double)s1.dato - ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.CHAR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Char)s1.dato - (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*ERROR*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = (Char)s1.dato - Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Char)s1.dato - (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.BOOL:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.INT;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) - (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) - Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.ERROR;
                                                    /*ERROR*/
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    /*ERROR*/
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.ERROR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    break;
                                                case TYPE.STRING:
                                                    break;
                                                case TYPE.DOUBLE:
                                                    break;
                                                case TYPE.CHAR:
                                                    break;
                                                case TYPE.BOOL:
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                case "*":
                                    switch (s1.t)
                                    {
                                        case TYPE.INT:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Int32)s1.dato * (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*ERROR*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = Double.Parse(s1.dato.ToString()) * Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Int32)s1.dato * (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Int32)s1.dato * ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.STRING:
                                            b.t = TYPE.ERROR;
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    break;
                                                case TYPE.STRING:
                                                    break;
                                                case TYPE.DOUBLE:
                                                    break;
                                                case TYPE.CHAR:
                                                    break;
                                                case TYPE.BOOL:
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.DOUBLE:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = Double.Parse(s1.dato.ToString()) * Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = Double.Parse(s1.dato.ToString()) * Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = (Double)s1.dato * (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = (Double)s1.dato * ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.CHAR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Char)s1.dato * (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*ERROR*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = (Char)s1.dato * Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Char)s1.dato * (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Char)s1.dato * ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.BOOL:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.INT;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) * (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) * Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.INT;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) * (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Boolean)s1.dato && (Boolean)s2.dato;
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.ERROR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    break;
                                                case TYPE.STRING:
                                                    break;
                                                case TYPE.DOUBLE:
                                                    break;
                                                case TYPE.CHAR:
                                                    break;
                                                case TYPE.BOOL:
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                case "/":
                                    switch (s1.t)
                                    {
                                        case TYPE.INT:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.DOUBLE;
                                                    double data = Double.Parse(s2.dato.ToString());
                                                    if (data != 0)
                                                    {
                                                        b.dato = Double.Parse(s1.dato.ToString()) / data;
                                                    }
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*ERROR*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    double data1 = Double.Parse(s2.dato.ToString());
                                                    if (data1 != 0)
                                                    {
                                                        b.dato = Double.Parse(s1.dato.ToString()) / data1;
                                                    }
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.DOUBLE;
                                                    if ((Char)s2.dato != 0)
                                                    {
                                                        b.dato = Double.Parse(s1.dato.ToString()) / (Char)s2.dato;
                                                    }
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.INT;
                                                    int boo = ((Boolean)s2.dato ? 1 : 0);
                                                    if (boo != 0)
                                                    {
                                                        b.dato = (Int32)s1.dato;
                                                    }
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.STRING:
                                            b.t = TYPE.ERROR;
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    break;
                                                case TYPE.STRING:
                                                    break;
                                                case TYPE.DOUBLE:
                                                    break;
                                                case TYPE.CHAR:
                                                    break;
                                                case TYPE.BOOL:
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.DOUBLE:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.DOUBLE;
                                                    double data = Double.Parse(s2.dato.ToString());
                                                    if (data != 0)
                                                    {
                                                        b.dato = Double.Parse(s1.dato.ToString()) / data;
                                                    }
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*ERROR*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    double data1 = Double.Parse(s2.dato.ToString());
                                                    if (data1 != 0)
                                                    {
                                                        b.dato = Double.Parse(s1.dato.ToString()) / data1;
                                                    }
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.DOUBLE;
                                                    if ((Char)s2.dato != 0)
                                                    {
                                                        b.dato = Double.Parse(s1.dato.ToString()) / (Char)s2.dato;
                                                    }
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.DOUBLE;
                                                    int boo = ((Boolean)s2.dato ? 1 : 0);
                                                    if (boo != 0)
                                                    {
                                                        b.dato = Double.Parse(s1.dato.ToString());
                                                    }
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.CHAR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.DOUBLE;
                                                    double data = Double.Parse(s2.dato.ToString());
                                                    if (data != 0)
                                                    {
                                                        b.dato = (Char)s1.dato / data;
                                                    }
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*ERROR*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    double data1 = Double.Parse(s2.dato.ToString());
                                                    if (data1 != 0)
                                                    {
                                                        b.dato = (Char)s1.dato / data1;
                                                    }
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.DOUBLE;
                                                    if ((Char)s2.dato != 0)
                                                    {
                                                        b.dato = (Char)s1.dato / (Char)s2.dato;
                                                    }
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.INT;
                                                    int boo = ((Boolean)s2.dato ? 1 : 0);
                                                    if (boo != 0)
                                                    {
                                                        b.dato = (Char)s1.dato;
                                                    }
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.BOOL:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.DOUBLE;
                                                    double data = Double.Parse(s2.dato.ToString());
                                                    if (data != 0)
                                                    {
                                                        b.dato = ((Boolean)s1.dato ? 1 : 0) / data;
                                                    }
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*ERROR*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    double data1 = Double.Parse(s2.dato.ToString());
                                                    if (data1 != 0)
                                                    {
                                                        b.dato = ((Boolean)s1.dato ? 1 : 0) / data1;
                                                    }
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.DOUBLE;
                                                    if ((Char)s2.dato != 0)
                                                    {
                                                        b.dato = ((Boolean)s2.dato ? 1 : 0) / (Char)s2.dato;
                                                    }
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    /*Error*/
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.ERROR:
                                            break;
                                    }
                                    break;
                                case "^":
                                    switch (s1.t)
                                    {
                                        case TYPE.INT:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Int32)Math.Pow((Int32)s1.dato, (Int32)s2.dato);
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*ERROR*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = Math.Pow(Double.Parse(s1.dato.ToString()), Double.Parse(s2.dato.ToString()));
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Int32)Math.Pow((Int32)s1.dato, (Char)s2.dato);
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Int32)Math.Pow((Int32)s1.dato, ((Boolean)s2.dato ? 1 : 0));
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.STRING:
                                            b.t = TYPE.ERROR;
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    break;
                                                case TYPE.STRING:
                                                    break;
                                                case TYPE.DOUBLE:
                                                    break;
                                                case TYPE.CHAR:
                                                    break;
                                                case TYPE.BOOL:
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.DOUBLE:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = Math.Pow(Double.Parse(s1.dato.ToString()), Double.Parse(s2.dato.ToString()));
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = Math.Pow(Double.Parse(s1.dato.ToString()), Double.Parse(s2.dato.ToString()));
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = Math.Pow((Double)s1.dato, (Char)s2.dato);
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = Math.Pow((Double)s1.dato, ((Boolean)s2.dato ? 1 : 0));
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.CHAR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Int32)Math.Pow((Char)s1.dato, (Int32)s2.dato);
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*ERROR*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = Math.Pow((Char)s1.dato, Double.Parse(s2.dato.ToString()));
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Int32)Math.Pow((Char)s1.dato, (Char)s2.dato);
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Int32)Math.Pow((Char)s1.dato, ((Boolean)s2.dato ? 1 : 0));
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.BOOL:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Int32)Math.Pow(((Boolean)s1.dato ? 1 : 0), (Int32)s2.dato);
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = Math.Pow(((Boolean)s1.dato ? 1 : 0), Double.Parse(s2.dato.ToString()));
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.INT;
                                                    b.dato = (Int32)Math.Pow(((Boolean)s1.dato ? 1 : 0), (Char)s2.dato);
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    /*ERROR*/
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.ERROR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    break;
                                                case TYPE.STRING:
                                                    break;
                                                case TYPE.DOUBLE:
                                                    break;
                                                case TYPE.CHAR:
                                                    break;
                                                case TYPE.BOOL:
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                case "==":
                                    switch (s1.t)
                                    {
                                        case TYPE.INT:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato == (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) == Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato == (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato == ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.STRING:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = s1.dato.ToString() == s2.dato.ToString();
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.ERROR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                            }
                                            break;
                                        case TYPE.DOUBLE:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) == Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) == Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Double)s1.dato == (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Double)s1.dato == ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.CHAR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato == (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato == Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato == (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.BOOL:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) == (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) == Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Boolean)s1.dato == (Boolean)s2.dato;
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.ERROR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    break;
                                                case TYPE.STRING:
                                                    break;
                                                case TYPE.DOUBLE:
                                                    break;
                                                case TYPE.CHAR:
                                                    break;
                                                case TYPE.BOOL:
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                case "!=":
                                    switch (s1.t)
                                    {
                                        case TYPE.INT:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato != (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) != Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato != (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato != ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.STRING:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = s1.dato.ToString() != s2.dato.ToString();
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.ERROR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                            }
                                            break;
                                        case TYPE.DOUBLE:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) != Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) != Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Double)s1.dato != (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Double)s1.dato != ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.CHAR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato != (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato != Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato != (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.BOOL:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) != (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) != Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Boolean)s1.dato != (Boolean)s2.dato;
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.ERROR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    break;
                                                case TYPE.STRING:
                                                    break;
                                                case TYPE.DOUBLE:
                                                    break;
                                                case TYPE.CHAR:
                                                    break;
                                                case TYPE.BOOL:
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                case ">":
                                    switch (s1.t)
                                    {
                                        case TYPE.INT:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato > (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) > Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato > (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato > ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.STRING:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = String.Compare(s1.dato.ToString(),s2.dato.ToString()) > 0 ? true : false;
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.ERROR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                            }
                                            break;
                                        case TYPE.DOUBLE:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) > Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) > Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Double)s1.dato > (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Double)s1.dato > ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.CHAR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato > (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato > Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato > (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.BOOL:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) > (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) > Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) > ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.ERROR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    break;
                                                case TYPE.STRING:
                                                    break;
                                                case TYPE.DOUBLE:
                                                    break;
                                                case TYPE.CHAR:
                                                    break;
                                                case TYPE.BOOL:
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                case ">=":
                                    switch (s1.t)
                                    {
                                        case TYPE.INT:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato >= (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) >= Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato >= (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato >= ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.STRING:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = String.Compare(s1.dato.ToString(), s2.dato.ToString()) >= 0 ? true : false;
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.ERROR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                            }
                                            break;
                                        case TYPE.DOUBLE:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) >= Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) >= Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Double)s1.dato >= (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Double)s1.dato >= ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.CHAR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato >= (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato >= Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato >= (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.BOOL:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) >= (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) >= Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) >= ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.ERROR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    break;
                                                case TYPE.STRING:
                                                    break;
                                                case TYPE.DOUBLE:
                                                    break;
                                                case TYPE.CHAR:
                                                    break;
                                                case TYPE.BOOL:
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                case "<":
                                    switch (s1.t)
                                    {
                                        case TYPE.INT:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato < (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) < Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato < (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato < ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.STRING:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = String.Compare(s1.dato.ToString(), s2.dato.ToString()) < 0 ? true : false;
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.ERROR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                            }
                                            break;
                                        case TYPE.DOUBLE:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) < Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) < Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Double)s1.dato < (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Double)s1.dato < ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.CHAR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato < (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato < Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato < (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.BOOL:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) < (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) < Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) < ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.ERROR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    break;
                                                case TYPE.STRING:
                                                    break;
                                                case TYPE.DOUBLE:
                                                    break;
                                                case TYPE.CHAR:
                                                    break;
                                                case TYPE.BOOL:
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                case "<=":
                                    switch (s1.t)
                                    {
                                        case TYPE.INT:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato <= (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) <= Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato <= (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Int32)s1.dato <= ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.STRING:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = String.Compare(s1.dato.ToString(), s2.dato.ToString()) <= 0 ? true : false;
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.ERROR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                            }
                                            break;
                                        case TYPE.DOUBLE:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) <= Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = Double.Parse(s1.dato.ToString()) <= Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Double)s1.dato <= (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Double)s1.dato <= ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.CHAR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato <= (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato <= Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Char)s1.dato <= (Char)s2.dato;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.BOOL:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) <= (Int32)s2.dato;
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.ERROR;
                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) <= Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.ERROR;
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) <= ((Boolean)s2.dato ? 1 : 0);
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                        case TYPE.ERROR:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    break;
                                                case TYPE.STRING:
                                                    break;
                                                case TYPE.DOUBLE:
                                                    break;
                                                case TYPE.CHAR:
                                                    break;
                                                case TYPE.BOOL:
                                                    break;
                                                case TYPE.ERROR:
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                case "&&":
                                    if (s1.t == TYPE.BOOL && s2.t == TYPE.BOOL)
                                    {
                                        b.t = TYPE.BOOL;
                                        b.dato = (Boolean)s1.dato && (Boolean)s2.dato;
                                    }
                                    else
                                    {
                                        b.t = TYPE.ERROR;
                                    }
                                    break;
                                case "||":
                                    if (s1.t == TYPE.BOOL && s2.t == TYPE.BOOL)
                                    {
                                        b.t = TYPE.BOOL;
                                        b.dato = (Boolean)s1.dato || (Boolean)s2.dato;
                                    }
                                    break;
                            }
                        }
                        else if (n == 2)
                        {
                            Variable s1 = Recorrido(raiz.ChildNodes[1], h);
                            switch (raiz.ChildNodes[0].Term.Name)
                            {
                                case "!":
                                    if (s1.t == TYPE.BOOL)
                                    {
                                        b.t = TYPE.BOOL;
                                        b.dato = !(Boolean)s1.dato;
                                    }
                                    break;
                                case "-":
                                    if (s1.t == TYPE.INT)
                                    {
                                        b.t = TYPE.INT;
                                        b.dato = -(int)s1.dato;
                                    }
                                    else if (s1.t == TYPE.DOUBLE) {
                                        b.t = TYPE.DOUBLE;
                                        b.dato = -(double)s1.dato;
                                    }
                                    break;
                                case "--":
                                    {

                                        break;
                                    }
                            }
                        }
                        else if (n == 1)
                        {
                            ParseTreeNode expr = raiz.ChildNodes[0];
                            int child = expr.ChildNodes.Count;
                            if (child == 0)
                            {
                                b.addVar(expr.Token.Text, expr.Token.Terminal.Name);
                            }
                            else if (child == 1)
                            {
                                ParseTreeNode node = expr.ChildNodes[0];
                                if (node.Term.Name == "Identifier")
                                {
                                    Variable o = (Variable)h.getValue(expr.ChildNodes[0].Token.Text);
                                    if (o != null)
                                    {
                                        if (o.dato != null)
                                        {
                                            b.dato = o.dato;
                                            b.t = o.t;
                                        }
                                    }
                                }
                                else if (node.Term.Name == "Funcion")
                                {
                                    Variable dato = Recorrido(node,h);
                                    if (dato.t == TYPE.RETURN)
                                    {
                                        b.dato = dato.dato;
                                        b.t = dato.taux;
                                    }
                                    else {
                                        b.dato = 0;
                                        b.t = TYPE.ERROR;
                                    }
                                }
                            }
                            else if(expr.Term.Name=="BArreglo")
                            {
                                if (expr.ChildNodes[0].ChildNodes.Count == 1)
                                {
                                    Variable buscar = (Variable)h.getValue(expr.ChildNodes[0].ChildNodes[0].Token.Text);
                                    if (buscar != null)
                                    {
                                        Arreglo arreglo = (Arreglo)buscar.dato;
                                        if (expr.ChildNodes.Count == 2)
                                        {
                                            Variable pos0 = Recorrido(expr.ChildNodes[1], h);
                                            b.t = buscar.t;
                                            b.dato = arreglo.getData((int)pos0.dato);
                                        }
                                        else if (expr.ChildNodes.Count == 3)
                                        {
                                            Variable pos0 = Recorrido(expr.ChildNodes[1], h);
                                            Variable pos1 = Recorrido(expr.ChildNodes[2], h);
                                            b.t = buscar.t;
                                            b.dato = arreglo.getData((int)pos1.dato, (int)pos0.dato);
                                        }
                                        else
                                        {
                                            Variable pos0 = Recorrido(expr.ChildNodes[1], h);
                                            Variable pos1 = Recorrido(expr.ChildNodes[2], h);
                                            Variable pos2 = Recorrido(expr.ChildNodes[3], h);
                                            b.t = buscar.t;
                                            b.dato = arreglo.getData((int)pos2.dato, (int)pos1.dato, (int)pos0.dato);
                                        }
                                    }
                                }
                                else
                                {

                                }
                            }
                            else if (child == 2) {
                                if (expr.ChildNodes[1].ChildNodes.Count == 0)
                                {
                                    //pos 0 es la clase y pos1 es la variable
                                    String nombreclas = expr.ChildNodes[0].Token.Text;
                                    String nombrevar = expr.ChildNodes[1].Token.Text;
                                    Variable v = (Variable)h.getValue(nombreclas);
                                    if (v != null && v.dato is Clase)
                                    {
                                        Clase c = (Clase)v.dato;
                                        Variable dato = (Variable)c.principal.getValue(nombrevar);
                                        if (dato != null && dato.dato != null) {
                                            b.dato = dato.dato;
                                            b.t = dato.t;
                                        }
                                        else
                                        {
                                            b.t = TYPE.ERROR;
                                            b.dato = 0;
                                        }
                                    }
                                    else {
                                        /*NO SE ENCONTRO LA CLASE*/
                                    }
                                }
                                else {
                                    /*FUNCIONES GET*/
                                }
                            }
                        }
                        break;
                    }
                case "asignacion":
                    if (raiz.ChildNodes.Count == 2)
                    {
                        Variable buscar = (Variable)h.getValue(raiz.ChildNodes[0].Token.Text);
                        if (buscar != null)
                        {
                            if (raiz.ChildNodes[1].Term.Name == "E")
                            {
                                Variable valor = Recorrido(raiz.ChildNodes[1], h);
                                if (valor.t == TYPE.INT || valor.t == TYPE.DOUBLE && buscar.t == TYPE.DOUBLE)
                                {
                                    buscar.dato = valor.dato;
                                }
                                else if (valor.t == buscar.t)
                                {
                                    buscar.dato = valor.dato;
                                }
                            }
                            else if (   raiz.ChildNodes[1].Term.Name == "++")
                            {
                                if (buscar.t == TYPE.INT)
                                {
                                    buscar.dato = (Int32)buscar.dato + 1;
                                }
                                else if (buscar.t == TYPE.DOUBLE)
                                {
                                    buscar.dato = (Double)buscar.dato + 1;
                                }
                                else if (buscar.t == TYPE.CHAR)
                                {
                                    buscar.dato = (Char)buscar.dato + 1;
                                }
                                else
                                {

                                }
                            }
                            else if (raiz.ChildNodes[1].Term.Name == "--")
                            {
                                if (buscar.t == TYPE.INT)
                                {
                                    buscar.dato = (Int32)buscar.dato - 1;
                                }
                                else if (buscar.t == TYPE.DOUBLE)
                                {
                                    buscar.dato = (Double)buscar.dato - 1;
                                }
                                else if (buscar.t == TYPE.CHAR)
                                {
                                    buscar.dato = (Char)buscar.dato - 1;
                                }
                                else
                                {

                                }
                            }
                            else
                            {

                            }
                        }
                    }
                    else if (raiz.ChildNodes.Count == 3)
                    {
                        Object buscar = h.getValue(raiz.ChildNodes[0].Token.Text);
                        if (buscar != null)
                        {
                            if (raiz.ChildNodes[1].Term.Name == "Dimension")
                            {
                                Arreglo arreglo = (Arreglo)((Variable)buscar).dato;
                                ParseTreeNode dimension = raiz.ChildNodes[1];
                                Variable dato = Recorrido(raiz.ChildNodes[2], h);
                                if (dimension.ChildNodes.Count == 1)
                                {
                                    Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                    if (pos1.t == TYPE.INT)
                                    {
                                        arreglo.setData((int)pos1.dato, dato);
                                    }
                                }
                                else if (dimension.ChildNodes.Count == 2)
                                {
                                    Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                    Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                    if (pos1.t == TYPE.INT && pos2.t == TYPE.INT)
                                    {
                                        arreglo.setData((int)pos1.dato, (int)pos2.dato, dato);
                                    }
                                }
                                else
                                {
                                    Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                    Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                    Variable pos3 = Recorrido(dimension.ChildNodes[2], h);
                                    if (pos1.t == TYPE.INT && pos2.t == TYPE.INT && pos3.t == TYPE.INT)
                                    {
                                        arreglo.setData((int)pos1.dato, (int)pos2.dato, (int)pos3.dato, dato);
                                    }
                                }
                            }
                            else
                            {

                            }
                        }
                    }
                    break;
                case "funcion":
                    {
                        String nombre = "$" + raiz.ChildNodes[0].Token.Text;
                        Variable metodo = (Variable)h.getValue(nombre);
                        if (metodo != null)
                        {
                            ParseTreeNode nodo = (ParseTreeNode)metodo.dato;
                            //no hay parametros
                            Entorno nuevo = new Entorno(h);
                            if (raiz.ChildNodes.Count == 1)
                            {
                                if (nodo.ChildNodes.Count == 3)
                                {
                                    Variable retorno = Recorrido(nodo.ChildNodes[2],nuevo);
                                    TYPE tipo = getType(nodo.ChildNodes[1].Token.Text);
                                    if (retorno.t == TYPE.CONTINUAR)
                                    {
                                        /*ERROR EN IF CONTINUAR*/
                                    }
                                    else if (retorno.t == TYPE.RETURN)
                                    {
                                        if (tipo == TYPE.VOID)
                                        {
                                            /*ERROR EL TIPO VOID NO RETORNA NADA*/
                                        }
                                        else if (retorno.taux == tipo)
                                        {
                                            /*RETORRNO CORRECTO*/
                                            b.dato = retorno.dato;
                                            b.taux = retorno.taux;
                                            b.t = retorno.t;
                                        }
                                        else {
                                            /*RETORNO INCORRECTO*/
                                        }
                                    }
                                    else {
                                        if (tipo != TYPE.VOID) {
                                            /*ERROR NO RETORNA NADA*/
                                        }
                                    }
                                }
                                else if (nodo.ChildNodes.Count == 5)
                                {
                                    Variable retorno = Recorrido(nodo.ChildNodes[4], nuevo);
                                }
                                else {
                                    /*ERROR DE PARAMETROS*/
                                }
                            }
                            //si hay parametros
                            else
                            {
                                if (nodo.ChildNodes.Count == 4)
                                {
                                    ParseTreeNode param1 = nodo.ChildNodes[2];
                                    ParseTreeNode param2 = raiz.ChildNodes[1];
                                    if (param1.ChildNodes.Count == param2.ChildNodes.Count)
                                    {
                                        Entorno h1 = new Entorno(h);
                                        for (int i = 0; i < param1.ChildNodes.Count; i++) {
                                            ParseTreeNode l1 = param1.ChildNodes[i];
                                            Variable dato = Recorrido(param2.ChildNodes[i], h);
                                            if (comprobar(l1.ChildNodes[0].Token.Text.ToLower(), dato.t))
                                            {
                                                h1.addVariable(l1.ChildNodes[1].Token.Text, new Variable(dato.columna,dato.fila,l1.ChildNodes[1].Token.Text,dato.dato,dato.t));
                                            }
                                            else {
                                                break;
                                            }
                                        }
                                        Variable retorno = Recorrido(nodo.ChildNodes[3], h1);
                                        if (retorno.t == TYPE.CONTINUAR)
                                        {
                                            /*ERROR EN IF CONTINUAR*/
                                        }
                                        else if (retorno.t == TYPE.RETURN)
                                        {
                                            TYPE tipo = getType(nodo.ChildNodes[0].Token.Text);
                                            if (tipo == TYPE.VOID)
                                            {
                                                /*ERROR EL TIPO VOID NO RETORNA NADA*/
                                            }
                                            else if (retorno.taux == tipo)
                                            {
                                                /*RETORRNO CORRECTO*/
                                            }
                                            else
                                            {
                                                /*RETORNO INCORRECTO*/
                                            }
                                        }
                                        else
                                        {

                                        }
                                    }
                                    else {
                                        /*ERROR PARAMETROS*/
                                    }
                                }
                                else if (nodo.ChildNodes.Count == 6)
                                {
                                    ParseTreeNode param1 = nodo.ChildNodes[4];
                                    ParseTreeNode param2 = raiz.ChildNodes[1];
                                    if (param1.ChildNodes.Count == param2.ChildNodes.Count)
                                    {
                                        Entorno h1 = new Entorno(h);
                                        for (int i = 0; i < param1.ChildNodes.Count; i++)
                                        {
                                            ParseTreeNode l1 = param1.ChildNodes[i];
                                            Variable dato = Recorrido(param2.ChildNodes[i], h);
                                            if (comprobar(l1.ChildNodes[0].Token.Text.ToLower(), dato.t))
                                            {
                                                h1.addVariable(l1.ChildNodes[1].Token.Text, new Variable(dato.columna, dato.fila, l1.ChildNodes[1].Token.Text, dato, dato.t));
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        Recorrido(nodo.ChildNodes[5], h1);
                                    }
                                    else {
                                        /*ERROR DE PARAMETROS*/
                                    }
                                }
                                else {
                                    /*ERROR DE PARAMETROS*/
                                }
                            }
                        }
                        else {
                            /*meotodo no encontrado*/
                        }
                        break;
                    }
                case "tfigure":
                    {
                        String nombre = raiz.ChildNodes[0].Term.Name;
                        Form2 fr = (Form2)forms[forms.Count - 1];
                        Graphics g = fr.picture.CreateGraphics();
                        switch (nombre) {
                            case "square":
                                {
                                    Variable n1 = Recorrido(raiz.ChildNodes[1], h);
                                    Variable n2 = Recorrido(raiz.ChildNodes[2], h);
                                    Variable n3 = Recorrido(raiz.ChildNodes[3], h);
                                    Variable n4 = Recorrido(raiz.ChildNodes[4], h);
                                    Variable n5 = Recorrido(raiz.ChildNodes[5], h);
                                    Variable n6 = Recorrido(raiz.ChildNodes[6], h);
                                    if (n1.t == TYPE.STRING && n2.t == TYPE.BOOL && n3.esnum() && n4.esnum() && n5.esnum() && n6.esnum()) {
                                        if ((bool)n2.dato)
                                        {
                                            Brush brus = new SolidBrush(getcolor(n1.dato.ToString()));
                                            Rectangle rec = new Rectangle(Int32.Parse(n3.dato.ToString()), Int32.Parse(n3.dato.ToString()), Int32.Parse(n3.dato.ToString()), Int32.Parse(n3.dato.ToString()));
                                            images.Add(new Figure(brus,rec,1));
                                        }
                                        else {
                                            Pen pen = new Pen(getcolor(n1.dato.ToString()));
                                            Rectangle rec = new Rectangle(Int32.Parse(n3.dato.ToString()), Int32.Parse(n3.dato.ToString()), Int32.Parse(n3.dato.ToString()), Int32.Parse(n3.dato.ToString()));
                                            images.Add(new Figure(pen,rec,1));
                                        }
                                    }
                                }
                                break;
                            case "line":
                                {
                                    Variable n1 = Recorrido(raiz.ChildNodes[1], h);
                                    Variable n2 = Recorrido(raiz.ChildNodes[2], h);
                                    Variable n3 = Recorrido(raiz.ChildNodes[3], h);
                                    Variable n4 = Recorrido(raiz.ChildNodes[4], h);
                                    Variable n5 = Recorrido(raiz.ChildNodes[5], h);
                                    Variable n6 = Recorrido(raiz.ChildNodes[6], h);
                                    if (n1.t == TYPE.STRING && n2.esnum() && n3.esnum() && n4.esnum() && n5.esnum() && n6.esnum())
                                    {
                                        Pen pen = new Pen(getcolor(n1.dato.ToString()),float.Parse(n6.dato.ToString()));
                                        Point[] p = { new Point(Int32.Parse(n2.dato.ToString()), Int32.Parse(n3.dato.ToString())),
                                        new Point(Int32.Parse(n4.dato.ToString()), Int32.Parse(n5.dato.ToString()))};
                                        images.Add(new Figure(pen, p,2));
                                    }
                                }
                                break;
                            case "circle":
                                {
                                    Variable n1 = Recorrido(raiz.ChildNodes[1], h);
                                    Variable n2 = Recorrido(raiz.ChildNodes[2], h);
                                    Variable n3 = Recorrido(raiz.ChildNodes[3], h);
                                    Variable n4 = Recorrido(raiz.ChildNodes[4], h);
                                    Variable n5 = Recorrido(raiz.ChildNodes[5], h);
                                    if (n1.t == TYPE.STRING && n2.esnum() && n3.t == TYPE.BOOL && n4.esnum() && n5.esnum())
                                    {
                                        int radio = Int32.Parse(n2.dato.ToString());
                                        int x = Int32.Parse(n4.dato.ToString());
                                        int y = Int32.Parse(n5.dato.ToString());
                                        if ((bool)n3.dato)
                                        {
                                            Brush brus = new SolidBrush(getcolor(n1.dato.ToString()));
                                            Rectangle rec = new Rectangle(x-radio,y-radio,radio*2,radio*2);
                                            images.Add(new Figure(brus, rec,3));

                                        }
                                        else {
                                            Pen pen = new Pen(getcolor(n1.dato.ToString()));
                                            Rectangle rec = new Rectangle(x - radio, y - radio, radio * 2, radio * 2);
                                            images.Add(new Figure(pen, rec, 3));
                                        }
                                    }
                                }
                                break;
                            case "triangle":
                                {
                                    Variable n1 = Recorrido(raiz.ChildNodes[1], h);
                                    Variable n2 = Recorrido(raiz.ChildNodes[2], h);
                                    Variable n3 = Recorrido(raiz.ChildNodes[3], h);
                                    Variable n4 = Recorrido(raiz.ChildNodes[4], h);
                                    Variable n5 = Recorrido(raiz.ChildNodes[5], h);
                                    Variable n6 = Recorrido(raiz.ChildNodes[6], h);
                                    Variable n7 = Recorrido(raiz.ChildNodes[7], h);
                                    Variable n8 = Recorrido(raiz.ChildNodes[8], h);
                                    if (n1.t == TYPE.STRING && n2.t == TYPE.BOOL && n3.esnum() && n4.esnum() && n5.esnum() && n6.esnum() && n7.esnum() && n8.esnum()) {
                                        Point[] a = { new Point(Int32.Parse(n3.dato.ToString()), Int32.Parse(n4.dato.ToString())),
                                        new Point(Int32.Parse(n5.dato.ToString()), Int32.Parse(n6.dato.ToString())),
                                        new Point(Int32.Parse(n7.dato.ToString()), Int32.Parse(n8.dato.ToString()))};
                                        if ((bool)n2.dato)
                                        {
                                            Brush brus = new SolidBrush(getcolor(n1.dato.ToString()));
                                            images.Add(new Figure(brus, a, 4));
                                            // g.FillPolygon(brus, a);

                                        }
                                        else {
                                            Pen pen = new Pen(getcolor(n1.dato.ToString()));
                                            images.Add(new Figure(pen, a, 4));
                                            //g.DrawPolygon(pen, a);
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                    }
                case "addfigure":
                    {
                        Recorrido(raiz.ChildNodes[1], h);
                        break;
                    }
                case "figure": {
                        Form2 fr = (Form2)forms[forms.Count - 1];
                        String titulo = Recorrido(raiz.ChildNodes[1], h).dato.ToString();
                        fr.Visible = true;
                        fr.Text = titulo;
                        Graphics o = fr.picture.CreateGraphics();

                        foreach (Figure f in images) {
                            if (f.tipo == 1)
                            {
                                Rectangle r = (Rectangle)f.figure;
                                if (f.pen is Pen)
                                {
                                    Pen p = (Pen)f.pen;
                                    o.DrawRectangle(p, r);
                                }
                                else {
                                    Brush p = (Brush)f.pen;
                                    o.FillRectangle(p, r);
                                }
                            }
                            else if (f.tipo == 2)
                            {
                                Pen p = (Pen)f.pen;
                                Point[] p1 = (Point[])f.figure;
                                o.DrawLine(p, p1[0], p1[1]);
                            }
                            else if (f.tipo == 3)
                            {
                                Rectangle rect = (Rectangle)f.figure;
                                if (f.pen is Pen)
                                {
                                    Pen p = (Pen)f.pen;
                                    o.DrawEllipse(p, rect);
                                }
                                else
                                {
                                    Brush p = (Brush)f.pen;
                                    o.FillEllipse(p, rect);
                                }
                            }
                            else {
                                Point[] a = (Point[])f.figure;
                                if (f.pen is Pen)
                                {
                                    Pen p = (Pen)f.pen;
                                    o.DrawPolygon(p, a);
                                }
                                else
                                {
                                    Brush p = (Brush)f.pen;
                                    o.FillPolygon(p, a);
                                }
                            }
                        }
                        images.Clear();
                        forms.Add(new Form2());
                        break;
                    }
                case "d1": {
                        Variable clase = (Variable)h.getValue(raiz.ChildNodes[0].Token.Text);
                        if (clase != null && clase.dato is Clase) {
                            Clase c = (Clase)clase.dato;
                            ParseTreeNode funcion = raiz.ChildNodes[1];
                            String nombre = "$" + funcion.ChildNodes[0].Token.Text;
                            Variable metodo = (Variable)c.principal.getValue(nombre);
                            if (metodo != null) {
                                ParseTreeNode nodo = (ParseTreeNode)metodo.dato;
                                Entorno nuevo = new Entorno(c.principal);
                                if (funcion.ChildNodes.Count == 1)
                                {
                                    if (nodo.ChildNodes.Count == 3)
                                    {
                                        Recorrido(nodo.ChildNodes[2], nuevo);
                                    }
                                    else if (nodo.ChildNodes.Count == 5)
                                    {
                                        Recorrido(nodo.ChildNodes[4], nuevo);
                                    }
                                    else
                                    {
                                        /*ERROR DE PARAMETROS*/
                                    }
                                }
                                else {
                                    if (nodo.ChildNodes.Count == 4)
                                    {
                                        ParseTreeNode param1 = nodo.ChildNodes[2];
                                        ParseTreeNode param2 = funcion.ChildNodes[1];
                                        if (param1.ChildNodes.Count == param2.ChildNodes.Count)
                                        {
                                            for (int i = 0; i < param1.ChildNodes.Count; i++)
                                            {
                                                ParseTreeNode l1 = param1.ChildNodes[i];
                                                Variable dato = Recorrido(param2.ChildNodes[i], h);
                                                if (comprobar(l1.ChildNodes[0].Token.Text.ToLower(), dato.t))
                                                {
                                                    nuevo.addVariable(l1.ChildNodes[1].Token.Text, new Variable(dato.columna, dato.fila, l1.ChildNodes[1].Token.Text, dato.dato, dato.t));
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                            Recorrido(nodo.ChildNodes[3], nuevo);
                                        }
                                        else
                                        {
                                            /*ERROR PARAMETROS*/
                                        }
                                    }
                                    else if (nodo.ChildNodes.Count == 6)
                                    {
                                        ParseTreeNode param1 = nodo.ChildNodes[4];
                                        ParseTreeNode param2 = funcion.ChildNodes[1];
                                        if (param1.ChildNodes.Count == param2.ChildNodes.Count)
                                        {
                                            for (int i = 0; i < param1.ChildNodes.Count; i++)
                                            {
                                                ParseTreeNode l1 = param1.ChildNodes[i];
                                                Variable dato = Recorrido(param2.ChildNodes[i], h);
                                                if (comprobar(l1.ChildNodes[0].Token.Text.ToLower(), dato.t))
                                                {
                                                    nuevo.addVariable(l1.ChildNodes[1].Token.Text, new Variable(dato.columna, dato.fila, l1.ChildNodes[1].Token.Text, dato, dato.t));
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                            Recorrido(nodo.ChildNodes[5], nuevo);
                                        }
                                        else
                                        {
                                            /*ERROR DE PARAMETROS*/
                                        }
                                    }
                                    else
                                    {
                                        /*ERROR DE PARAMETROS*/
                                    }
                                }
                            }
                        }
                        else
                        {
                            /*NO SE ENCONTRO LA CLASE*/
                        }
                        break;
                    }
            }
            return b;
        }
        private static bool comprobar(String t1, TYPE t2) {
            if (t1 == "int" && t2 == TYPE.INT)
            {
                return true;
            }
            else if (t1 == "double" && (t2 == TYPE.INT || t2 == TYPE.DOUBLE))
            {
                return true;
            }
            else if (t1 == "string" && t2 == TYPE.STRING)
            {
                return true;
            }
            else if (t1 == "char" && t2 == TYPE.CHAR)
            {
                return true;
            }
            else if (t1 == "bool" && t2 == TYPE.BOOL)
            {
                return true;
            }
            else if (t2 == TYPE.CLASS) {
                return true;
            }
            return false;
        }

        private static Color getcolor(String color) {
            if (color.Substring(0, 1) == "#")
            {
                return ColorTranslator.FromHtml(color);
            }
            else {
                return Color.FromName(color);
            }
        }

        private static TYPE getType(String tipo) {
            switch (tipo)
            {
                case "int":
                    return TYPE.INT;
                case "bool":
                    return TYPE.BOOL;
                case "string":
                    return TYPE.STRING;
                case "double":
                    return TYPE.DOUBLE;
                case "char":
                    return TYPE.CHAR;
                case "void":
                    return TYPE.VOID;
                case "array":
                    return TYPE.ARRAY;
                default:
                    return TYPE.CLASS;
            }

        }
    }
}
