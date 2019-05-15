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
        public static Entorno h1;
        public static ArrayList forms = new ArrayList();
        public static ArrayList images = new ArrayList();
        public static ArrayList listaerrores = new ArrayList();

        public static bool analizar(String cadena) {
            clases.Clear();
            forms.Clear();
            images.Clear();
            listaerrores.Clear();
            main_node = claseActual = null;
            h = new Entorno();
            Form1.console.Clear();
            Grammar g = new Grammar();
            LanguageData lg = new LanguageData(g);
            Parser pr = new Parser(lg);
            ParseTree tree = pr.Parse(cadena);
            ParseTreeNode raiz = tree.Root;
            if (raiz == null) {
                listaerrores.Add(new Error(tree.ParserMessages[0].Location.Line,tree.ParserMessages[0].Location.Column,tree.ParserMessages[0].Message));
                return false;
            }
            forms.Add(new Form2());
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
                    String nombre = node.Token.Text.ToLower();
                    if (clases.Contains(nombre))
                    {
                        ParseTreeNode m = (ParseTreeNode)clases[nombre];
                        importar(m, h);
                    }
                    else
                    {
                        listaerrores.Add(new Error(node.Span.Location.Line,node.Span.Location.Column,"No se encontro la clase: " + nombre));
                    }
                }
            }
            else if (raiz.ChildNodes.Count == 5)
            {
                addGlobal(raiz.ChildNodes[4], h);
                foreach (ParseTreeNode node in raiz.ChildNodes[3].ChildNodes)
                {
                    String nombre = node.Token.Text.ToLower();
                    if (clases.Contains(nombre))
                    {
                        ParseTreeNode m = (ParseTreeNode)clases[nombre];
                        importar(m, h);
                    }
                    else
                    {
                        listaerrores.Add(new Error(node.Span.Location.Line, node.Span.Location.Column,"No se encontro la clase: " + nombre));
                    }
                }
            }
        }

        private static void addClass(ParseTreeNode raiz) {
            foreach (ParseTreeNode node in raiz.ChildNodes) {
                String name = node.ChildNodes[1].Token.Value.ToString().ToLower();
                if (!clases.ContainsKey(name)) {
                    clases.Add(name, node);
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
                if (main_node.ChildNodes.Count == 2) {
                    importar(claseActual, h);
                    h1 = new Entorno(h);
                    Recorrido(main_node.ChildNodes[1], h1);
                }
            }
        }

        public static void generar()
        {
            foreach(DictionaryEntry dic in clases)
            {
                generarImagen(dic.Key.ToString(), (ParseTreeNode)dic.Value);
            }
            if (main_node != null)
            {
                generarImagen("main", main_node);
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
                        listaerrores.Add(new Error(node.Span.Location.Line, node.Span.Location.Column, "Main repetido"));
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
                                                        listaerrores.Add(new Error(nodeD.Span.Location.Line,nodeD.Span.Location.Column,"No se encontro la clase: " + tipo));
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
                                                                    arr.setData(i, v.dato,v.t);
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
                                                                listaerrores.Add(new Error(dataA.Span.Location.Line,dataA.Span.Location.Column,"El arreglo es de 1 dimension"));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                            listaerrores.Add(new Error(dataA.Span.Location.Line, dataA.Span.Location.Column, "La posicion no puede ser negativa"));

                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                        listaerrores.Add(new Error(dataA.Span.Location.Line, dataA.Span.Location.Column, "La posicion debe ser un entero"));

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
                                                                        arr.setData(j, i, v.dato,v.t);
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
                                                                listaerrores.Add(new Error(dataA.Span.Location.Line, dataA.Span.Location.Column, "El arreglo es de 2 dimensiones"));

                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                            listaerrores.Add(new Error(dataA.Span.Location.Line, dataA.Span.Location.Column, "La posicion no puede ser negativa"));

                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                        listaerrores.Add(new Error(dataA.Span.Location.Line, dataA.Span.Location.Column, "La posicion no es un entero"));
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
                                                            listaerrores.Add(new Error(dataA.Span.Location.Line, dataA.Span.Location.Column, "La posicion no puede ser negativa"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                        listaerrores.Add(new Error(dataA.Span.Location.Line, dataA.Span.Location.Column, "La posicion debe ser un numero"));

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
                                                            listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser positiva"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                        listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion no es un entero"));

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
                                                            listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser positiva"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                        listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser un entero"));

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
                                                            listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser positiva"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                        listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser un entero"));
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
                                                        listaerrores.Add(new Error(nodeD.Span.Location.Line,nodeD.Span.Location.Column, "La clase " + tipo + " no existe"));
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
                                                                    arr.setData(i, v.dato,v.t);
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
                                                                listaerrores.Add(new Error(dim1.fila, dim1.columna, "El arreglo es de 1 dimension"));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                            listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser positiva"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                        listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser un entero"));
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
                                                                        arr.setData(j, i, v.dato,v.t);
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
                                                                listaerrores.Add(new Error(dim1.fila, dim1.columna, "El arreglo es de 2 dimensiones"));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                            listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser positiva"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                        listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser un entero"));
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
                                                                            arr.setData(k, j, i, v.dato,v.t);
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
                            if (nueva.t == TYPE.CONTINUAR || nueva.t == TYPE.SALIR || nueva.t == TYPE.RETURN)
                            {
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
                                                            arr.setTipo(tipo);
                                                            foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                            {
                                                                h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, false));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                            listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser positiva"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                        listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser un entero"));
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
                                                            arr.setTipo(tipo);
                                                            foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                            {
                                                                h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, false));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                            listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser positiva"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                        listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser un entero"));
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
                                                            arr.setTipo(tipo);
                                                            foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                            {
                                                                h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, false));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                            listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser positiva"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                        listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser un entero"));
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
                                                        listaerrores.Add(new Error(nodeD.Span.Location.Line, nodeD.Span.Location.Column, "No existe la clase " + tipo));
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
                                                                arr.setTipo(tipo);
                                                                int i = 0;
                                                                foreach (ParseTreeNode node in dataA.ChildNodes) {
                                                                    Variable v = Recorrido(node, h);
                                                                    arr.setData(i, v.dato,v.t);
                                                                    i++;
                                                                }
                                                                foreach (ParseTreeNode node in nodeD.ChildNodes[2].ChildNodes)
                                                                {
                                                                    h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, arr, tipo, false));
                                                                }
                                                            }
                                                            else {
                                                                /*1 DIM */
                                                                listaerrores.Add(new Error(dim1.fila, dim1.columna, "El arreglo es de 1 dimension"));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                            listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser positiva"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                        listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser un entero"));
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
                                                                arr.setTipo(tipo);
                                                                int i = 0;
                                                                foreach (ParseTreeNode node in dataA.ChildNodes) {
                                                                    int j = 0;
                                                                    foreach (ParseTreeNode node1 in node.ChildNodes)
                                                                    {
                                                                        Variable v = Recorrido(node1, h);
                                                                        arr.setData(j, i, v.dato,v.t);
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
                                                                listaerrores.Add(new Error(dim1.fila, dim1.columna, "El arreglo es de 2 dimensiones"));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            /*POSICION NEGATIVA*/
                                                            listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser positiva"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                        listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser un entero"));
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
                                                                arr.setTipo(tipo);
                                                                int i = 0;
                                                                foreach (ParseTreeNode node in dataA.ChildNodes) {
                                                                    int j = 0;
                                                                    foreach (ParseTreeNode nod1 in node.ChildNodes) {
                                                                        int k = 0;
                                                                        foreach (ParseTreeNode node2 in nod1.ChildNodes) {
                                                                            Variable v = Recorrido(node2, h);
                                                                            arr.setData(k, j, i, v.dato,v.t);
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
                                                            listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser positiva"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*NO ES ENTERO*/
                                                        listaerrores.Add(new Error(dim1.fila, dim1.columna, "La posicion debe ser un entero"));
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
                                            if (clase != null && clase.dato is Clase)
                                            {
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
                                                        else {
                                                            listaerrores.Add(new Error(dato.fila,dato.columna,"Tipo de dato no aceptado"));
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
                                                        else if (var.t == TYPE.CHAR)
                                                        {
                                                            var.dato = (Char)var.dato + 1;
                                                        }
                                                        else {
                                                            listaerrores.Add(new Error(var.fila, var.columna, "Tipo de dato no aceptado para aumento"));
                                                        }
                                                    }
                                                    else
                                                    {
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
                                                        else {
                                                            listaerrores.Add(new Error(var.fila, var.columna, "Tipo de dato no aceptado para decremento"));
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    listaerrores.Add(new Error(var.fila,var.columna,"La Variable fue nula"));
                                                }
                                            }
                                            else {
                                                listaerrores.Add(new Error(b.fila,b.columna,"La clase dio nulo"));
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
                                                    else
                                                    {
                                                        listaerrores.Add(new Error(buscar.fila, buscar.columna, "Tipo de dato no aceptado"));
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
                                                        listaerrores.Add(new Error(buscar.fila, buscar.columna, "Tipo de dato no aceptado para aumento"));
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
                                                        listaerrores.Add(new Error(buscar.fila, buscar.columna, "Tipo de dato no aceptado para decremento"));
                                                    }
                                                }
                                                else
                                                {

                                                }
                                            }
                                            else {
                                                listaerrores.Add(new Error(b.fila, b.columna, "Regreso nulo"));
                                            }
                                        }
                                    }
                                    else if (nodeD.ChildNodes.Count == 3) {
                                        if (nodeD.ChildNodes[0].Term.Name == "D1")
                                        {
                                            ParseTreeNode nodo = nodeD.ChildNodes[0];
                                            Variable clase = (Variable)h.getValue(nodo.ChildNodes[0].Token.Text);
                                            if (clase != null && clase.dato is Clase)
                                            {
                                                Clase c = (Clase)clase.dato;
                                                String nombrevar = nodo.ChildNodes[1].Token.Text;
                                                Variable var = (Variable)c.principal.getValue(nombrevar);
                                                if (var != null && var.dato is Arreglo)
                                                {
                                                    Arreglo arreglo = (Arreglo)var.dato;
                                                    ParseTreeNode dimension = nodeD.ChildNodes[1];
                                                    if (nodeD.ChildNodes[2].Term.Name == "++")
                                                    {
                                                        if (dimension.ChildNodes.Count == 1)
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            if (pos1.t == TYPE.INT)
                                                            {
                                                                arreglo.setData1((int)pos1.dato, 1);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }
                                                        else if (dimension.ChildNodes.Count == 2)
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                                            if (pos1.t == TYPE.INT && pos2.t == TYPE.INT)
                                                            {
                                                                arreglo.setData1((int)pos1.dato, (int)pos2.dato, 1);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                                            Variable pos3 = Recorrido(dimension.ChildNodes[2], h);
                                                            if (pos1.t == TYPE.INT && pos2.t == TYPE.INT && pos3.t == TYPE.INT)
                                                            {
                                                                arreglo.setData1((int)pos1.dato, (int)pos2.dato, (int)pos3.dato, 1);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }
                                                    }
                                                    else if (nodeD.ChildNodes[2].Term.Name == "--")
                                                    {
                                                        if (dimension.ChildNodes.Count == 1)
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            if (pos1.t == TYPE.INT)
                                                            {
                                                                arreglo.setData1((int)pos1.dato, -1);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }
                                                        else if (dimension.ChildNodes.Count == 2)
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                                            if (pos1.t == TYPE.INT && pos2.t == TYPE.INT)
                                                            {
                                                                arreglo.setData1((int)pos1.dato, (int)pos2.dato, -1);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                                            Variable pos3 = Recorrido(dimension.ChildNodes[2], h);
                                                            if (pos1.t == TYPE.INT && pos2.t == TYPE.INT && pos3.t == TYPE.INT)
                                                            {
                                                                arreglo.setData1((int)pos1.dato, (int)pos2.dato, (int)pos3.dato, -1);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }
                                                    }
                                                    else {
                                                        Variable dato = Recorrido(nodeD.ChildNodes[2], h);
                                                        if (dimension.ChildNodes.Count == 1)
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            if (pos1.t == TYPE.INT)
                                                            {
                                                                arreglo.setData((int)pos1.dato, dato.dato, dato.t);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }
                                                        else if (dimension.ChildNodes.Count == 2)
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                                            if (pos1.t == TYPE.INT && pos2.t == TYPE.INT)
                                                            {
                                                                arreglo.setData((int)pos1.dato, (int)pos2.dato, dato.dato, dato.t);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                                            Variable pos3 = Recorrido(dimension.ChildNodes[2], h);
                                                            if (pos1.t == TYPE.INT && pos2.t == TYPE.INT && pos3.t == TYPE.INT)
                                                            {
                                                                arreglo.setData((int)pos1.dato, (int)pos2.dato, (int)pos3.dato, dato.dato, dato.t);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }
                                                    }
                                                }
                                                else {
                                                    listaerrores.Add(new Error(b.fila, b.columna, "Regreso nulo"));
                                                }
                                            }
                                            else {
                                                listaerrores.Add(new Error(b.fila, b.columna, "Clase no encontrada"));
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
                                                    if (nodeD.ChildNodes[2].Term.Name == "++")
                                                    {
                                                        if (dimension.ChildNodes.Count == 1)
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            if (pos1.t == TYPE.INT)
                                                            {
                                                                arreglo.setData1((int)pos1.dato, 1);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }
                                                        else if (dimension.ChildNodes.Count == 2)
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                                            if (pos1.t == TYPE.INT && pos2.t == TYPE.INT)
                                                            {
                                                                arreglo.setData1((int)pos1.dato, (int)pos2.dato, 1);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                                            Variable pos3 = Recorrido(dimension.ChildNodes[2], h);
                                                            if (pos1.t == TYPE.INT && pos2.t == TYPE.INT && pos3.t == TYPE.INT)
                                                            {
                                                                arreglo.setData1((int)pos1.dato, (int)pos2.dato, (int)pos3.dato, 1);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }
                                                    }
                                                    else if (nodeD.ChildNodes[2].Term.Name == "--") {
                                                        if (dimension.ChildNodes.Count == 1)
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            if (pos1.t == TYPE.INT)
                                                            {
                                                                arreglo.setData1((int)pos1.dato, -1);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }
                                                        else if (dimension.ChildNodes.Count == 2)
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                                            if (pos1.t == TYPE.INT && pos2.t == TYPE.INT)
                                                            {
                                                                arreglo.setData1((int)pos1.dato, (int)pos2.dato, -1);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                                            Variable pos3 = Recorrido(dimension.ChildNodes[2], h);
                                                            if (pos1.t == TYPE.INT && pos2.t == TYPE.INT && pos3.t == TYPE.INT)
                                                            {
                                                                arreglo.setData1((int)pos1.dato, (int)pos2.dato, (int)pos3.dato, -1);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        Variable dato = Recorrido(nodeD.ChildNodes[2], h);
                                                        if (dimension.ChildNodes.Count == 1)
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            if (pos1.t == TYPE.INT)
                                                            {
                                                                arreglo.setData((int)pos1.dato, dato.dato, dato.t);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }
                                                        else if (dimension.ChildNodes.Count == 2)
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                                            if (pos1.t == TYPE.INT && pos2.t == TYPE.INT)
                                                            {
                                                                arreglo.setData((int)pos1.dato, (int)pos2.dato, dato.dato, dato.t);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                                            Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                                            Variable pos3 = Recorrido(dimension.ChildNodes[2], h);
                                                            if (pos1.t == TYPE.INT && pos2.t == TYPE.INT && pos3.t == TYPE.INT)
                                                            {
                                                                arreglo.setData((int)pos1.dato, (int)pos2.dato, (int)pos3.dato, dato.dato, dato.t);
                                                            }
                                                            else
                                                            {
                                                                listaerrores.Add(new Error(pos1.fila, pos1.columna, "La posicion debe ser un entero"));
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Variable var = (Variable)buscar;
                                                    ParseTreeNode nodo = (ParseTreeNode)clases[nodeD.ChildNodes[2].Token.Text.ToLower()];
                                                    Clase clase = new Clase();
                                                    importar(nodo, clase.principal);
                                                    h.addVariable(var.nombre, new Variable(nodeD.Span.Location.Line, nodeD.Span.Location.Column, var.nombre, clase, "clase", var.Private));
                                                }
                                            }
                                            else {
                                                listaerrores.Add(new Error(b.fila, b.columna, "Regreso nulo"));
                                            }
                                        }
                                    }
                                    break;
                                case "bloqueprint":
                                    {
                                        Variable imp = Recorrido(nodeD.ChildNodes[1], h);
                                        if (imp.t != TYPE.ERROR && imp.dato!= null)
                                        {
                                            Form1.console.AppendText(imp.dato.ToString() + "\n");
                                        }
                                        else
                                        {
                                            listaerrores.Add(new Error(b.fila, b.columna, "No se puede imprimir un nulo"));
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
                                            else
                                            {
                                                listaerrores.Add(new Error(body.fila,body.columna,"No se puede imprimir un nulo"));
                                            }
                                        }
                                        else
                                        {
                                            Variable title = Recorrido(nodeD.ChildNodes[1], h);
                                            Variable body = Recorrido(nodeD.ChildNodes[2], h);
                                            if (title.t != TYPE.ERROR && body.t != TYPE.ERROR && title.dato!= null && body.dato!=null)
                                            {
                                                MessageBox.Show(body.dato.ToString(), title.dato.ToString());
                                            }
                                            else
                                            {
                                                listaerrores.Add(new Error(body.fila, body.columna, "No se puede imprimir un nulo"));
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
                                                                    arr.setData(i, v.dato,v.t);
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
                                                                        arr.setData(j, i, v.dato,v.t);
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
                                        if (nueva.t == TYPE.CONTINUAR || nueva.t == TYPE.SALIR || nueva.t == TYPE.RETURN)
                                            return nueva;
                                    }
                                }
                            }
                            else {
                                /*ERROR DE TIPO*/
                                listaerrores.Add(new Error(condicion.fila,condicion.columna,"La condicion debe ser booleana"));
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
                                        if (nueva.t == TYPE.SALIR || nueva.t == TYPE.CONTINUAR || nueva.t == TYPE.RETURN) {
                                            return nueva;
                                        }
                                    }
                                }
                                else {
                                    ParseTreeNode nuevo = raiz.ChildNodes[1];
                                    if (nuevo.Term.Name == "ELSE")
                                    {
                                        if (nuevo.ChildNodes.Count == 2) {
                                            Entorno h1 = new Entorno(h);
                                            Variable nueva = Recorrido(nuevo.ChildNodes[1],h1);
                                            if (nueva.t == TYPE.CONTINUAR || nueva.t == TYPE.RETURN || nueva.t == TYPE.SALIR)
                                                return nueva;
                                        }
                                    }
                                    else {
                                        Variable nueva = Recorrido(nuevo, h);
                                        if (nueva.t == TYPE.CONTINUAR || nueva.t == TYPE.SALIR)
                                            return nueva;
                                        else if(nueva.t == TYPE.RETURN) { b.t = TYPE.RETURN; b.dato = nueva.datoaux; b.taux = nueva.taux; return b; }
                                    }
                                }
                            }
                            else
                            {
                                /*ERROR DE TIPO*/
                                listaerrores.Add(new Error(condicion.fila, condicion.columna, "La condicion debe ser booleana"));
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
                                        else if (nueva.t == TYPE.RETURN || nueva.t == TYPE.SALIR) {
                                            return nueva;
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
                                            if (nueva.t == TYPE.CONTINUAR || nueva.t == TYPE.SALIR || nueva.t == TYPE.RETURN)
                                                return nueva;
                                        }
                                    }
                                    else {
                                        if (condicion2.t == TYPE.CONTINUAR || condicion2.t == TYPE.SALIR)
                                        {
                                            return condicion2;
                                        }
                                        else if (condicion2.t == TYPE.RETURN) {
                                            b.t = TYPE.RETURN;
                                            b.dato = condicion2.datoaux;
                                            b.taux = condicion2.taux;
                                            return b;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                /*ERROR DE TIPO*/
                                listaerrores.Add(new Error(condicion.fila, condicion.columna, "La condicion debe ser booleana"));
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
                                if (condicion.t == TYPE.CONTINUAR || condicion.t == TYPE.SALIR)
                                    b.t = condicion.t;
                                else if(condicion.t == TYPE.RETURN)
                                {
                                    b.t = TYPE.RETURN;
                                    b.taux = condicion.taux;
                                    b.datoaux = condicion.datoaux;
                                }
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
                            listaerrores.Add(new Error(condicion.fila, condicion.columna, "La condicion debe ser booleana"));
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
                            listaerrores.Add(new Error(cantidad.fila, cantidad.columna, "La condicion debe ser un entero"));
                            /*error tipo dato*/
                        }
                        break;
                    }

                case "bloquehacer":
                    {
                        if (raiz.ChildNodes.Count == 4) {
                            Variable sentencia = Recorrido(raiz.ChildNodes[3], h);
                            if (sentencia.t == TYPE.BOOL)
                            {
                                do
                                {
                                    Entorno h1 = new Entorno(h);
                                    Variable nueva = Recorrido(raiz.ChildNodes[1], h1);
                                    if (nueva.t == TYPE.SALIR)
                                    {
                                        break;
                                    }
                                    else if (nueva.t == TYPE.RETURN)
                                    {
                                        return nueva;
                                    }
                                    sentencia = Recorrido(raiz.ChildNodes[3], h);
                                } while ((bool)sentencia.dato);
                            }
                            else {
                                listaerrores.Add(new Error(sentencia.fila, sentencia.columna, "La condicion debe ser un booleano"));
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
                                if (condicion.t == TYPE.BOOL)
                                {
                                    while ((bool)condicion.dato)
                                    {
                                        Entorno h1 = new Entorno(h);
                                        Variable nueva = Recorrido(raiz.ChildNodes[5], h1);
                                        if (nueva.t == TYPE.SALIR)
                                        {
                                            break;
                                        }
                                        else if (nueva.t == TYPE.RETURN)
                                        {
                                            return nueva;
                                        }
                                        Recorrido(raiz.ChildNodes[4], h);
                                        condicion = Recorrido(raiz.ChildNodes[3], h1);
                                    }
                                }
                                else
                                {
                                    listaerrores.Add(new Error(condicion.fila, condicion.columna, "La condicion debe ser un booleano"));
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
                            if (condicion.t == TYPE.BOOL)
                            {
                                while ((bool)condicion.dato)
                                {
                                    Variable nueva = Recorrido(raiz.ChildNodes[6], h1);
                                    if (nueva.t == TYPE.SALIR)
                                    {
                                        break;
                                    }
                                    else if (nueva.t == TYPE.RETURN)
                                    {
                                        return nueva;
                                    }
                                    Recorrido(raiz.ChildNodes[5], h1);
                                    condicion = Recorrido(raiz.ChildNodes[4], h1);
                                }
                            }
                            else
                            {
                                listaerrores.Add(new Error(condicion.fila, condicion.columna, "La condicion debe ser un booleano"));
                            }
                        }
                        break;
                    }
                case "bloquecomprobar":
                    {
                        Variable expr = Recorrido(raiz.ChildNodes[1], h);
                        ParseTreeNode lista = raiz.ChildNodes[2];
                        bool comprobado = false;
                        ParseTreeNode defecto = null;
                        foreach(ParseTreeNode node in lista.ChildNodes)
                        {
                            if (node.ChildNodes.Count == 3)
                            {
                                Variable exprcomp = Recorrido(node.ChildNodes[1], h);
                                if(exprcomp.dato.ToString() == expr.dato.ToString())
                                {
                                    comprobado = true;
                                    Entorno h1 = new Entorno(h);
                                    Variable nueva = Recorrido(node.ChildNodes[2], h1);
                                    if (nueva.t == TYPE.CONTINUAR || nueva.t == TYPE.RETURN)
                                    {
                                        return nueva;
                                    }
                                    else if (nueva.t == TYPE.SALIR)
                                        break;
                                }
                            }
                            else {
                                if (node.ChildNodes[0].Term.Name == "defecto")
                                {
                                    defecto = node.ChildNodes[1];
                                }
                            }
                        }
                        if (!comprobado)
                        {
                            Entorno h1 = new Entorno(h);
                            Variable nueva = Recorrido(defecto, h1);
                            if (nueva.t == TYPE.CONTINUAR || nueva.t == TYPE.RETURN)
                                return nueva;
                        }
                        break;
                    }
                case "elseif":
                    {
                        b.dato = false;
                        Variable condicion = Recorrido(raiz.ChildNodes[2], h);
                        if (condicion.t == TYPE.BOOL)
                        {
                            if ((bool)condicion.dato)
                            {
                                b.dato = true;
                                if (raiz.ChildNodes.Count == 4) {
                                    Entorno h1 = new Entorno(h);
                                    Variable nueva = Recorrido(raiz.ChildNodes[3], h1);
                                    if (nueva.t == TYPE.CONTINUAR || nueva.t == TYPE.SALIR)
                                        b.t = nueva.t;
                                    else if (nueva.t == TYPE.RETURN) {
                                        b.t = nueva.t;
                                        b.datoaux = nueva.dato;
                                        b.taux = nueva.taux;
                                    }
                                }
                            }
                        }
                        else {
                            /*ERROR DE CONDICION*/
                            listaerrores.Add(new Error(condicion.fila, condicion.columna, "La condicion debe ser un booleano"));
                        }
                        break;
                    }
                    //ahorita vooy
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
                                                    b.t = TYPE.ERROR;
                                                    listaerrores.Add(new Error(s2.fila,s2.columna,"No se puede Entero + Error"));
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
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede String + Bool"));
                                                    /*error*/
                                                    break;
                                                case TYPE.ERROR:
                                                    b.t = TYPE.ERROR;
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede String + Error"));
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
                                                    b.t = TYPE.ERROR;
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Double + Error"));
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
                                                    b.t = TYPE.ERROR;
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Char + Error"));
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
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Bool + String"));
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
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Bool + Error"));
                                                    break;
                                            }
                                            break;
                                        case TYPE.ERROR:
                                            b.t = TYPE.ERROR;
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error + Int"));
                                                    break;
                                                case TYPE.STRING:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error + String"));
                                                    break;
                                                case TYPE.DOUBLE:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error + Double"));
                                                    break;
                                                case TYPE.CHAR:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error + Char"));
                                                    break;
                                                case TYPE.BOOL:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error + Bool"));
                                                    break;
                                                case TYPE.ERROR:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error + Error"));
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
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Int - String"));
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
                                                    b.t = TYPE.ERROR;
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Int - Error"));
                                                    break;
                                            }
                                            break;
                                        case TYPE.STRING:
                                            b.t = TYPE.ERROR;
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede String - Int"));
                                                    break;
                                                case TYPE.STRING:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede String - String"));
                                                    break;
                                                case TYPE.DOUBLE:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede String - Double"));
                                                    break;
                                                case TYPE.CHAR:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede String - Char"));
                                                    break;
                                                case TYPE.BOOL:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede String - Bool"));
                                                    break;
                                                case TYPE.ERROR:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede String - Error"));
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
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Double - Error"));
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
                                                    b.t = TYPE.ERROR;
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Error - Int"));
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
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Char - String"));
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
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Char - Bool"));
                                                    /*error*/
                                                    break;
                                                case TYPE.ERROR:
                                                    b.t = TYPE.ERROR;
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Char - Error"));
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
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Bool - String"));

                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.DOUBLE;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) - Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.ERROR;
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Bool - Char"));

                                                    /*ERROR*/
                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Bool - Bool"));

                                                    /*ERROR*/
                                                    break;
                                                case TYPE.ERROR:
                                                    b.t = TYPE.ERROR;
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Bool - Error"));

                                                    break;
                                            }
                                            break;
                                        case TYPE.ERROR:
                                            b.t = TYPE.ERROR;
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Error - Int"));
                                                    break;
                                                case TYPE.STRING:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Error - String"));

                                                    break;
                                                case TYPE.DOUBLE:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Error - Double"));

                                                    break;
                                                case TYPE.CHAR:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Error - Char"));

                                                    break;
                                                case TYPE.BOOL:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Error - Bool"));

                                                    break;
                                                case TYPE.ERROR:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Error - Error"));

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
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Int * String"));
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
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Int * Error "));
                                                    b.t = TYPE.ERROR;
                                                    break;
                                            }
                                            break;
                                        case TYPE.STRING:
                                            b.t = TYPE.ERROR;
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede String * Int "));

                                                    break;
                                                case TYPE.STRING:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede String * String "));

                                                    break;
                                                case TYPE.DOUBLE:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede String * Double "));

                                                    break;
                                                case TYPE.CHAR:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede String * Char "));

                                                    break;
                                                case TYPE.BOOL:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede String * Bool "));

                                                    break;
                                                case TYPE.ERROR:
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede String * Error "));

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
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Double * String "));

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
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Double * Error "));
                                                    b.t = TYPE.ERROR;
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
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Char * String "));

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
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Char * Error"));
                                                    b.t = TYPE.ERROR;
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
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Bool * String "));

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
                                                    listaerrores.Add(new Error(s2.fila, s2.columna, "No se puede Bool * Error "));
                                                    b.t = TYPE.ERROR;
                                                    break;
                                            }
                                            break;
                                        case TYPE.ERROR:
                                            b.t = TYPE.ERROR;
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error * Int"));

                                                    break;
                                                case TYPE.STRING:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error * String"));

                                                    break;
                                                case TYPE.DOUBLE:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error * Double "));

                                                    break;
                                                case TYPE.CHAR:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error * Char "));

                                                    break;
                                                case TYPE.BOOL:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error * Bool"));

                                                    break;
                                                case TYPE.ERROR:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error * Error "));

                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                    //pendiente div 0
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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Int / String "));
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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Int / Error "));
                                                    b.t = TYPE.ERROR;
                                                    break;
                                            }
                                            break;
                                        case TYPE.STRING:
                                            b.t = TYPE.ERROR;
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede String / Int"));

                                                    break;
                                                case TYPE.STRING:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede String / String "));

                                                    break;
                                                case TYPE.DOUBLE:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede String / Double "));

                                                    break;
                                                case TYPE.CHAR:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede String / Char "));

                                                    break;
                                                case TYPE.BOOL:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede String / Bool "));

                                                    break;
                                                case TYPE.ERROR:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede String / Error "));

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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Double / String "));

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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Double / Error "));
                                                    b.t = TYPE.ERROR;
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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Char / Error "));

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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Char / Error "));
                                                    b.t = TYPE.ERROR;
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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Bool / String "));

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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Bool / Bool "));

                                                    /*Error*/
                                                    break;
                                                case TYPE.ERROR:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Bool / Error "));
                                                    b.t = TYPE.ERROR;
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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede  Int ^ String "));

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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Int ^ Error  "));

                                                    break;
                                            }
                                            break;
                                        case TYPE.STRING:
                                            b.t = TYPE.ERROR;
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede  String ^ Int  "));

                                                    break;
                                                case TYPE.STRING:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede  String ^ String  "));

                                                    break;
                                                case TYPE.DOUBLE:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede String ^ Double  "));

                                                    break;
                                                case TYPE.CHAR:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede String  ^ Char  "));

                                                    break;
                                                case TYPE.BOOL:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede  String ^ Bool "));

                                                    break;
                                                case TYPE.ERROR:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede String ^ Error "));

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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Double ^ String "));

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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Double ^ Error  "));
                                                    b.t = TYPE.ERROR;
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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede  Char ^ String "));

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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede  Char ^ Error "));
                                                    b.t = TYPE.ERROR;
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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Bool ^ Error "));

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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede  Bool ^ Error "));
                                                    b.t = TYPE.ERROR;
                                                    break;
                                            }
                                            break;
                                        case TYPE.ERROR:
                                            b.t = TYPE.ERROR;
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error ^ Int "));

                                                    break;
                                                case TYPE.STRING:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error ^ String  "));

                                                    break;
                                                case TYPE.DOUBLE:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error ^ Double "));

                                                    break;
                                                case TYPE.CHAR:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error ^  Char"));

                                                    break;
                                                case TYPE.BOOL:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error ^ Bool "));

                                                    break;
                                                case TYPE.ERROR:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error ^ Error "));

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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Int == String"));
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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Int == Error"));
                                                    b.t = TYPE.ERROR;
                                                    break;
                                            }
                                            break;
                                        case TYPE.STRING:
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    b.t = TYPE.ERROR;
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede String == Int"));
                                                    break;
                                                case TYPE.STRING:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = s1.dato.ToString() == s2.dato.ToString();
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.ERROR;
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede String == Double "));

                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.ERROR;
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede String == Char "));

                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.ERROR;
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede String == Bool "));

                                                    /*error*/
                                                    break;
                                                case TYPE.ERROR:
                                                    b.t = TYPE.ERROR;
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede String  == Error"));

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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Double == String"));

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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Double == Error "));
                                                    b.t = TYPE.ERROR;
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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Char == String"));

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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Char == Bool "));

                                                    break;
                                                case TYPE.ERROR:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Char == Error"));
                                                    b.t = TYPE.ERROR;
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
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Bool == String "));

                                                    /*error*/
                                                    break;
                                                case TYPE.DOUBLE:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = ((Boolean)s1.dato ? 1 : 0) == Double.Parse(s2.dato.ToString());
                                                    break;
                                                case TYPE.CHAR:
                                                    b.t = TYPE.ERROR;
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Bool == Char"));

                                                    break;
                                                case TYPE.BOOL:
                                                    b.t = TYPE.BOOL;
                                                    b.dato = (Boolean)s1.dato == (Boolean)s2.dato;
                                                    break;
                                                case TYPE.ERROR:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Bool == Error "));
                                                    b.t = TYPE.ERROR;
                                                    break;
                                            }
                                            break;
                                        case TYPE.ERROR:
                                            b.t = TYPE.ERROR;
                                            switch (s2.t)
                                            {
                                                case TYPE.INT:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error == Int"));

                                                    break;
                                                case TYPE.STRING:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error == String"));

                                                    break;
                                                case TYPE.DOUBLE:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error == Double"));

                                                    break;
                                                case TYPE.CHAR:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error == Char"));

                                                    break;
                                                case TYPE.BOOL:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error == Bool "));

                                                    break;
                                                case TYPE.ERROR:
                                                    listaerrores.Add(new Error(s1.fila, s1.columna, "No se puede Error == Error "));

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
                                        listaerrores.Add(new Error(s1.fila, s1.columna, "Las condciones deben ser booleanas"));
                                    }
                                    break;
                                case "||":
                                    if (s1.t == TYPE.BOOL && s2.t == TYPE.BOOL)
                                    {
                                        b.t = TYPE.BOOL;
                                        b.dato = (Boolean)s1.dato || (Boolean)s2.dato;
                                    }
                                    else {
                                        b.t = TYPE.ERROR;
                                        listaerrores.Add(new Error(s1.fila, s1.columna, "Las condiciones deben ser booleanas"));
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
                                    else {
                                        b.t = TYPE.ERROR;
                                        listaerrores.Add(new Error(s1.fila, s1.columna, "La condicion debe ser booleana"));
                                    }
                                    break;
                                case "-":
                                    if (s1.t == TYPE.INT)
                                    {
                                        b.t = TYPE.INT;
                                        b.dato = -(int)s1.dato;
                                    }
                                    else if (s1.t == TYPE.DOUBLE)
                                    {
                                        b.t = TYPE.DOUBLE;
                                        b.dato = -(double)s1.dato;
                                    }
                                    else {
                                        b.t = TYPE.ERROR;
                                        listaerrores.Add(new Error(s1.fila, s1.columna, "Solo los numeros pueden ser negativos"));
                                    }
                                    break;
                                case "D1":
                                    {
                                        ParseTreeNode nodo = raiz.ChildNodes[0];
                                        Variable buscar = (Variable)h.getValue(nodo.ChildNodes[0].Token.Text);
                                        if (nodo.ChildNodes.Count == 1)
                                        {
                                            if (buscar != null)
                                            {
                                                if (raiz.ChildNodes[1].Term.Name == "++")
                                                {
                                                    if (buscar.t == TYPE.INT)
                                                    {
                                                        b.dato = buscar.dato;
                                                        b.t = TYPE.INT;
                                                        buscar.dato = (Int32)buscar.dato + 1;
                                                    }
                                                    else if (buscar.t == TYPE.DOUBLE)
                                                    {
                                                        b.dato = buscar.dato;
                                                        b.t = TYPE.DOUBLE;
                                                        buscar.dato = (Double)buscar.dato + 1;
                                                    }
                                                    else if (buscar.t == TYPE.CHAR)
                                                    {
                                                        b.dato = buscar.dato;
                                                        b.t = TYPE.CHAR;
                                                        buscar.dato = (Char)buscar.dato + 1;
                                                    }
                                                    else
                                                    {
                                                        b.t = TYPE.ERROR;
                                                        listaerrores.Add(new Error(b.fila, b.columna, "Tipo de dato no acepta aumento"));

                                                    }
                                                }
                                                else
                                                {
                                                    if (buscar.t == TYPE.INT)
                                                    {
                                                        b.dato = buscar.dato;
                                                        b.t = TYPE.INT;
                                                        buscar.dato = (Int32)buscar.dato - 1;
                                                    }
                                                    else if (buscar.t == TYPE.DOUBLE)
                                                    {
                                                        b.dato = buscar.dato;
                                                        b.t = TYPE.DOUBLE;
                                                        buscar.dato = (Double)buscar.dato - 1;
                                                    }
                                                    else if (buscar.t == TYPE.CHAR)
                                                    {
                                                        b.dato = buscar.dato;
                                                        b.t = TYPE.CHAR;
                                                        buscar.dato = (Char)buscar.dato - 1;
                                                    }
                                                    else
                                                    {
                                                        b.t = TYPE.ERROR;
                                                        listaerrores.Add(new Error(b.fila, b.columna, "Tipo de dato no acepta decremento"));
                                                    }
                                                }
                                            }
                                            else {
                                                listaerrores.Add(new Error(b.fila, b.columna, "Variable no encontrada"));
                                                b.t = TYPE.ERROR;
                                            }
                                        }
                                        else {
                                            /*ES CON CLASES*/
                                            if (buscar != null && buscar.dato is Clase)
                                            {
                                                Clase c = (Clase)buscar.dato;
                                                Variable bus = (Variable)c.principal.getValue(nodo.ChildNodes[1].Token.Text);
                                                if (bus != null)
                                                {
                                                    if (raiz.ChildNodes[1].Term.Name == "++")
                                                    {
                                                        if (bus.t == TYPE.INT)
                                                        {
                                                            b.dato = bus.dato;
                                                            b.t = TYPE.INT;
                                                            bus.dato = (Int32)bus.dato + 1;
                                                        }
                                                        else if (bus.t == TYPE.DOUBLE)
                                                        {
                                                            b.dato = buscar.dato;
                                                            b.t = TYPE.DOUBLE;
                                                            bus.dato = (Double)bus.dato + 1;
                                                        }
                                                        else if (bus.t == TYPE.CHAR)
                                                        {
                                                            b.dato = buscar.dato;
                                                            b.t = TYPE.CHAR;
                                                            bus.dato = (Char)bus.dato + 1;
                                                        }
                                                        else
                                                        {
                                                            listaerrores.Add(new Error(b.fila, b.columna, "Tipo de dato no acepta aumento"));
                                                            b.t = TYPE.ERROR;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (bus.t == TYPE.INT)
                                                        {
                                                            bus.dato = (Int32)bus.dato - 1;
                                                        }
                                                        else if (bus.t == TYPE.DOUBLE)
                                                        {
                                                            bus.dato = (Double)bus.dato - 1;
                                                        }
                                                        else if (buscar.t == TYPE.CHAR)
                                                        {
                                                            bus.dato = (Char)bus.dato - 1;
                                                        }
                                                        else
                                                        {
                                                            listaerrores.Add(new Error(b.fila, b.columna, "Tipo de dato no acepta decremento"));
                                                        }
                                                    }
                                                }
                                                else {
                                                    listaerrores.Add(new Error(b.fila, b.columna, "Variable no encontrada"));
                                                    b.t = TYPE.ERROR;
                                                }
                                            }
                                            else {
                                                listaerrores.Add(new Error(b.fila, b.columna, "Clase no encontrada"));
                                                b.t = TYPE.ERROR;
                                            }
                                        }
                                        break;
                                    }
                                case "BArreglo": {
                                        ParseTreeNode expr = raiz.ChildNodes[0];
                                        if (raiz.ChildNodes[1].Term.Name == "++")
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
                                                        b.t = arreglo.T;
                                                        b.dato = arreglo.getData((int)pos0.dato);
                                                        arreglo.setData1((int)pos0.dato, 1);
                                                    }
                                                    else if (expr.ChildNodes.Count == 3)
                                                    {
                                                        Variable pos0 = Recorrido(expr.ChildNodes[1], h);
                                                        Variable pos1 = Recorrido(expr.ChildNodes[2], h);
                                                        b.t = arreglo.T;
                                                        b.dato = arreglo.getData((int)pos1.dato, (int)pos0.dato);
                                                        arreglo.setData1((int)pos0.dato,(int)pos1.dato, 1);

                                                    }
                                                    else
                                                    {
                                                        Variable pos0 = Recorrido(expr.ChildNodes[1], h);
                                                        Variable pos1 = Recorrido(expr.ChildNodes[2], h);
                                                        Variable pos2 = Recorrido(expr.ChildNodes[3], h);
                                                        b.t = arreglo.T;
                                                        b.dato = arreglo.getData((int)pos2.dato, (int)pos1.dato, (int)pos0.dato);
                                                        arreglo.setData1((int)pos0.dato,(int)pos1.dato,(int)pos2.dato, 1);
                                                    }
                                                }
                                            }
                                            else
                                            {

                                            }
                                        }
                                        else {
                                            if (expr.ChildNodes[0].ChildNodes.Count == 1)
                                            {
                                                Variable buscar = (Variable)h.getValue(expr.ChildNodes[0].ChildNodes[0].Token.Text);
                                                if (buscar != null)
                                                {
                                                    Arreglo arreglo = (Arreglo)buscar.dato;
                                                    if (expr.ChildNodes.Count == 2)
                                                    {
                                                        Variable pos0 = Recorrido(expr.ChildNodes[1], h);
                                                        b.t = arreglo.T;
                                                        b.dato = arreglo.getData((int)pos0.dato);
                                                        arreglo.setData1((int)pos0.dato,-1);

                                                    }
                                                    else if (expr.ChildNodes.Count == 3)
                                                    {
                                                        Variable pos0 = Recorrido(expr.ChildNodes[1], h);
                                                        Variable pos1 = Recorrido(expr.ChildNodes[2], h);
                                                        b.t = arreglo.T;
                                                        b.dato = arreglo.getData((int)pos1.dato, (int)pos0.dato);
                                                        arreglo.setData1((int)pos0.dato, (int)pos1.dato, -1);
                                                    }
                                                    else
                                                    {
                                                        Variable pos0 = Recorrido(expr.ChildNodes[1], h);
                                                        Variable pos1 = Recorrido(expr.ChildNodes[2], h);
                                                        Variable pos2 = Recorrido(expr.ChildNodes[3], h);
                                                        b.t = arreglo.T;
                                                        b.dato = arreglo.getData((int)pos2.dato, (int)pos1.dato, (int)pos0.dato);
                                                        arreglo.setData1((int)pos0.dato, (int)pos1.dato, (int)pos2.dato, -1);

                                                    }
                                                }
                                            }
                                            else
                                            {

                                            }
                                        }
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
                            else if (expr.Term.Name == "E") {
                                b = Recorrido(expr, h);
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
                                    /*AQUI EMPIEZA LA MASACRE*/
                                    String nombre = "$" + node.ChildNodes[0].Token.Text;
                                    Variable metodo = (Variable)h.getValue(nombre);
                                    Variable dato = null;
                                    if (metodo != null)
                                    {
                                        ParseTreeNode nodo = (ParseTreeNode)metodo.dato;
                                        Entorno nuevo = new Entorno(h);
                                        if (node.ChildNodes.Count == 1)
                                        {
                                            if (nodo.ChildNodes.Count == 3)
                                            {
                                                Variable retorno = Recorrido(nodo.ChildNodes[2], nuevo);
                                                TYPE tipo = getType(nodo.ChildNodes[1].Token.Text);
                                                if (retorno.t == TYPE.CONTINUAR || retorno.t == TYPE.SALIR)
                                                {
                                                    /*ERROR EN IF CONTINUAR*/
                                                    listaerrores.Add(new Error(retorno.fila, retorno.columna, "Error en ambiente If"));
                                                }
                                                else if (retorno.t == TYPE.RETURN)
                                                {
                                                    if (tipo == TYPE.VOID)
                                                    {
                                                        /*ERROR EL TIPO VOID NO RETORNA NADA*/
                                                        listaerrores.Add(new Error(retorno.fila, retorno.columna, "El tipo void no debe retornar nada"));
                                                    }
                                                    else if (retorno.taux == tipo)
                                                    {
                                                        /*RETORRNO CORRECTO*/
                                                        b.dato = retorno.dato;
                                                        b.taux = retorno.taux;
                                                        b.t = retorno.t;
                                                        dato = retorno;
                                                    }
                                                    else
                                                    {
                                                        /*RETORNO INCORRECTO*/
                                                        listaerrores.Add(new Error(retorno.fila, retorno.columna, "Tipo de dato de retorno no es correcto"));
                                                    }
                                                }
                                                else
                                                {
                                                    if (tipo != TYPE.VOID)
                                                    {
                                                        /*ERROR NO RETORNA NADA*/
                                                        listaerrores.Add(new Error(retorno.fila, retorno.columna, "No se ha retornado nada"));
                                                    }
                                                }
                                            }
                                            else if (nodo.ChildNodes.Count == 5)
                                            {
                                                Variable retorno = Recorrido(nodo.ChildNodes[4], nuevo);
                                                TYPE tipo = getType(nodo.ChildNodes[1].Token.Text);
                                                TYPE tipoaux = getType(nodo.ChildNodes[2].Token.Text);
                                                if (retorno.t == TYPE.CONTINUAR || retorno.t == TYPE.SALIR)
                                                {
                                                    /*ERROR SALIR EN EL ENTORNO INCORRECTO*/
                                                    listaerrores.Add(new Error(retorno.fila, retorno.columna, "Error en ambiente If"));

                                                }
                                                else if (retorno.t == TYPE.RETURN)
                                                {
                                                    if (tipoaux == retorno.taux)
                                                    {
                                                        Arreglo arreglo = (Arreglo)retorno.dato;
                                                        if (arreglo.T == tipoaux)
                                                        {
                                                            b.dato = retorno.dato;
                                                            b.taux = retorno.taux;
                                                            b.t = retorno.t;
                                                            dato = retorno;
                                                        }
                                                        else
                                                        { /*NO SON DEL MISMO TIPO*/
                                                            listaerrores.Add(new Error(retorno.fila, retorno.columna, "Tipo de retorno diferente"));
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    /*DEBE RETORNAR ALGO*/
                                                    listaerrores.Add(new Error(retorno.fila, retorno.columna, "No se ha retornado nada"));
                                                }
                                            }
                                            else
                                            {
                                                listaerrores.Add(new Error(b.fila, b.columna, "El metodo no contiene parametros"));
                                            }
                                        }
                                        else
                                        {
                                            if (nodo.ChildNodes.Count == 4)
                                            {
                                                ParseTreeNode param1 = nodo.ChildNodes[2];
                                                ParseTreeNode param2 = node.ChildNodes[1];
                                                if (param1.ChildNodes.Count == param2.ChildNodes.Count)
                                                {
                                                    Entorno h1 = new Entorno(h);
                                                    for (int i = 0; i < param1.ChildNodes.Count; i++)
                                                    {
                                                        ParseTreeNode l1 = param1.ChildNodes[i];
                                                        Variable dato1 = Recorrido(param2.ChildNodes[i], h);
                                                        if (comprobar(l1.ChildNodes[0].Token.Text.ToLower(), dato1.t))
                                                        {
                                                            if (l1.ChildNodes.Count == 2)
                                                                h1.addVariable(l1.ChildNodes[1].Token.Text, new Variable(dato1.columna, dato1.fila, l1.ChildNodes[1].Token.Text, dato1.dato, dato1.t));
                                                            else
                                                                h1.addVariable(l1.ChildNodes[2].Token.Text, new Variable(dato1.columna, dato1.fila, l1.ChildNodes[2].Token.Text, dato1.dato, dato1.t));
                                                        }
                                                        else
                                                        {
                                                            //ERROR DE TIPO DE PARAMETROS
                                                            listaerrores.Add(new Error(dato1.fila, dato1.columna, "Tipo de parametro incorrecto"));
                                                            return b;
                                                        }
                                                    }
                                                    Variable retorno = Recorrido(nodo.ChildNodes[3], h1);
                                                    if (retorno.t == TYPE.CONTINUAR || retorno.t == TYPE.SALIR)
                                                    {
                                                        /*ERROR EN IF CONTINUAR*/
                                                        listaerrores.Add(new Error(retorno.fila, retorno.columna, "Error en ambiente If"));
                                                    }
                                                    else if (retorno.t == TYPE.RETURN)
                                                    {
                                                        TYPE tipo = getType(nodo.ChildNodes[1].Token.Text.ToLower());
                                                        if (tipo == TYPE.VOID)
                                                        {
                                                            /*ERROR EL TIPO VOID NO RETORNA NADA*/
                                                            listaerrores.Add(new Error(retorno.fila, retorno.columna, "Tipo Void no debe retornar algo"));
                                                        }
                                                        else if (retorno.taux == tipo)
                                                        {
                                                            /*RETORRNO CORRECTO*/
                                                            b.dato = retorno.dato;
                                                            b.taux = retorno.taux;
                                                            b.t = retorno.t;
                                                            dato = retorno;
                                                        }
                                                        else
                                                        {
                                                            /*RETORNO INCORRECTO*/
                                                            listaerrores.Add(new Error(retorno.fila, retorno.columna, "Dato de retorno incorrecto"));

                                                        }
                                                    }
                                                    else
                                                    {
                                                        listaerrores.Add(new Error(retorno.fila, retorno.columna, "No se ha retornado nada"));

                                                    }
                                                }
                                                else
                                                {
                                                    listaerrores.Add(new Error(b.fila, b.columna, "Cantidad de parametros incorrecta"));
                                                }
                                            }
                                            else if (nodo.ChildNodes.Count == 6)
                                            {
                                                ParseTreeNode param1 = nodo.ChildNodes[4];
                                                ParseTreeNode param2 = node.ChildNodes[1];
                                                if (param1.ChildNodes.Count == param2.ChildNodes.Count)
                                                {
                                                    Entorno h1 = new Entorno(h);
                                                    for (int i = 0; i < param1.ChildNodes.Count; i++)
                                                    {
                                                        ParseTreeNode l1 = param1.ChildNodes[i];
                                                        Variable dato1 = Recorrido(param2.ChildNodes[i], h);
                                                        if (comprobar(l1.ChildNodes[0].Token.Text.ToLower(), dato1.t))
                                                        {
                                                            if (l1.ChildNodes.Count == 2)
                                                                h1.addVariable(l1.ChildNodes[1].Token.Text, new Variable(dato1.columna, dato1.fila, l1.ChildNodes[1].Token.Text, dato1.dato, dato1.t));
                                                            else
                                                                h1.addVariable(l1.ChildNodes[2].Token.Text, new Variable(dato1.columna, dato1.fila, l1.ChildNodes[2].Token.Text, dato1.dato, dato1.t));
                                                        }
                                                        else
                                                        {
                                                            /*ERROR DE COMPARACION DE TIPOS DE PARAMETRO*/
                                                            listaerrores.Add(new Error(dato1.fila, dato1.columna, "Tipos de datos de parametros diferentes"));
                                                            return b;
                                                        }
                                                    }
                                                    Variable retorno = Recorrido(nodo.ChildNodes[5], h1);
                                                    if (retorno.t == TYPE.CONTINUAR || retorno.t == TYPE.SALIR)
                                                    {
                                                        /*error de entornos*/
                                                        listaerrores.Add(new Error(retorno.fila, retorno.columna, "Error en ambiente If"));
                                                    }
                                                    else if (retorno.t == TYPE.RETURN)
                                                    {
                                                        TYPE tipo = getType(nodo.ChildNodes[1].Token.Text);
                                                        TYPE tipoaux = getType(nodo.ChildNodes[2].Token.Text);
                                                        if (tipoaux == retorno.taux)
                                                        {
                                                            Arreglo arreglo = (Arreglo)retorno.dato;
                                                            if (arreglo.T == tipoaux)
                                                            {
                                                                b.dato = retorno.dato;
                                                                b.t = retorno.t;
                                                                b.taux = retorno.taux;
                                                                dato = retorno;
                                                            }
                                                            else
                                                            {
                                                                /*ERROR DE TIPOS*/
                                                                listaerrores.Add(new Error(retorno.fila, retorno.columna, "Tipo de retorno diferente"));
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*ERROR DEBE RETORNAR ALGO*/
                                                        listaerrores.Add(new Error(retorno.fila, retorno.columna, "No se ha retornado nada"));

                                                    }
                                                }
                                                else
                                                {
                                                    /*ERROR DE PARAMETROS*/
                                                    listaerrores.Add(new Error(b.fila, b.columna, "Cantidad de parametros incorrecta"));

                                                }
                                            }
                                            else
                                            {
                                                /*ERROR DE PARAMETROS*/
                                            }
                                        }
                                    }
                                    else
                                    {
                                        listaerrores.Add(new Error(b.fila, b.columna, "No se encontro el metodo: " + nombre));
                                    }
                                    /*AQUI TERMINA LA MASACRE*/
                                    if (dato != null && dato.t == TYPE.RETURN)
                                    {
                                        b.dato = dato.dato;
                                        b.t = dato.taux;
                                    }
                                    else
                                    {
                                        b.dato = 0;
                                        b.t = TYPE.ERROR;
                                    }
                                }
                            }
                            else if (expr.Term.Name == "BArreglo")
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
                                            b.t = arreglo.T;
                                            b.dato = arreglo.getData((int)pos0.dato);
                                        }
                                        else if (expr.ChildNodes.Count == 3)
                                        {
                                            Variable pos0 = Recorrido(expr.ChildNodes[1], h);
                                            Variable pos1 = Recorrido(expr.ChildNodes[2], h);
                                            b.t = arreglo.T;
                                            b.dato = arreglo.getData((int)pos1.dato, (int)pos0.dato);
                                        }
                                        else
                                        {
                                            Variable pos0 = Recorrido(expr.ChildNodes[1], h);
                                            Variable pos1 = Recorrido(expr.ChildNodes[2], h);
                                            Variable pos2 = Recorrido(expr.ChildNodes[3], h);
                                            b.t = arreglo.T;
                                            b.dato = arreglo.getData((int)pos2.dato, (int)pos1.dato, (int)pos0.dato);
                                        }
                                    }
                                }
                                else
                                {
                                    ParseTreeNode d1 = expr.ChildNodes[0];
                                    Variable clase = (Variable)h.getValue(d1.ChildNodes[0].Token.Text.ToLower());
                                    if (clase != null && clase.dato is Clase)
                                    {
                                        Clase c = (Clase)clase.dato;
                                        Variable buscar = (Variable)c.principal.getValue(d1.ChildNodes[1].Token.Text);
                                        if (buscar != null && buscar.dato is Arreglo)
                                        {
                                            Arreglo arreglo = (Arreglo)buscar.dato;
                                            if (expr.ChildNodes.Count == 2)
                                            {
                                                Variable pos0 = Recorrido(expr.ChildNodes[1], h);
                                                b.t = arreglo.T;
                                                b.dato = arreglo.getData((int)pos0.dato);
                                            }
                                            else if (expr.ChildNodes.Count == 3)
                                            {
                                                Variable pos0 = Recorrido(expr.ChildNodes[1], h);
                                                Variable pos1 = Recorrido(expr.ChildNodes[2], h);
                                                b.t = arreglo.T;
                                                b.dato = arreglo.getData((int)pos1.dato, (int)pos0.dato);
                                            }
                                            else
                                            {
                                                Variable pos0 = Recorrido(expr.ChildNodes[1], h);
                                                Variable pos1 = Recorrido(expr.ChildNodes[2], h);
                                                Variable pos2 = Recorrido(expr.ChildNodes[3], h);
                                                b.t = arreglo.T;
                                                b.dato = arreglo.getData((int)pos2.dato, (int)pos1.dato, (int)pos0.dato);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        listaerrores.Add(new Error(b.fila, b.columna, "No se encontro la clase "));
                                    }
                                }
                            }
                            else if (child == 2)
                            {
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
                                        if (dato != null && dato.dato != null)
                                        {
                                            b.dato = dato.dato;
                                            b.t = dato.t;
                                        }
                                        else
                                        {
                                            b.t = TYPE.ERROR;
                                            b.dato = 0;
                                        }
                                    }
                                    else
                                    {
                                        /*NO SE ENCONTRO LA CLASE*/
                                    }
                                }
                                else
                                {
                                    /*FUNCIONES GET DE OTRAS CLASES!!!*/
                                    Variable get = Recorrido(expr, h);
                                    b.dato = get.dato;
                                    b.t = get.t;
                                    return b;
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
                                    listaerrores.Add(new Error(b.fila, b.columna, "Tipo de dato no acepta aumento"));

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
                                    listaerrores.Add(new Error(b.fila, b.columna, "Tipo de dato no acepta decremento"));

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
                                        arreglo.setData((int)pos1.dato, dato.dato,dato.t);
                                    }
                                }
                                else if (dimension.ChildNodes.Count == 2)
                                {
                                    Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                    Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                    if (pos1.t == TYPE.INT && pos2.t == TYPE.INT)
                                    {
                                        arreglo.setData((int)pos1.dato, (int)pos2.dato, dato.dato,dato.t);
                                    }
                                }
                                else
                                {
                                    Variable pos1 = Recorrido(dimension.ChildNodes[0], h);
                                    Variable pos2 = Recorrido(dimension.ChildNodes[1], h);
                                    Variable pos3 = Recorrido(dimension.ChildNodes[2], h);
                                    if (pos1.t == TYPE.INT && pos2.t == TYPE.INT && pos3.t == TYPE.INT)
                                    {
                                        arreglo.setData((int)pos1.dato, (int)pos2.dato, (int)pos3.dato, dato.dato,dato.t);
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
                                    if (retorno.t == TYPE.CONTINUAR || retorno.t == TYPE.SALIR)
                                    {
                                        /*ERROR EN IF CONTINUAR*/
                                        listaerrores.Add(new Error(retorno.fila, retorno.columna, "Error en ambiente If"));
                                    }
                                    else if (retorno.t == TYPE.RETURN)
                                    {
                                        if (tipo == TYPE.VOID)
                                        {
                                            /*ERROR EL TIPO VOID NO RETORNA NADA*/
                                            listaerrores.Add(new Error(retorno.fila, retorno.columna, "El tipo void no debe retornar nada"));
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
                                            listaerrores.Add(new Error(retorno.fila, retorno.columna, "Tipo de dato de retorno no es correcto"));
                                        }
                                    }
                                    else {
                                        if (tipo != TYPE.VOID) {
                                            /*ERROR NO RETORNA NADA*/
                                            listaerrores.Add(new Error(retorno.fila, retorno.columna, "No se ha retornado nada"));
                                        }
                                    }
                                }
                                else if (nodo.ChildNodes.Count == 5)
                                {
                                    Variable retorno = Recorrido(nodo.ChildNodes[4], nuevo);
                                    TYPE tipo = getType(nodo.ChildNodes[1].Token.Text);
                                    TYPE tipoaux = getType(nodo.ChildNodes[2].Token.Text);
                                    if(retorno.t == TYPE.CONTINUAR || retorno.t == TYPE.SALIR)
                                    {
                                        /*ERROR SALIR EN EL ENTORNO INCORRECTO*/
                                        listaerrores.Add(new Error(retorno.fila, retorno.columna, "Error en ambiente If"));

                                    }
                                    else if(retorno.t == TYPE.RETURN)
                                    {
                                        if(tipoaux == retorno.taux)
                                        {
                                            Arreglo arreglo = (Arreglo)retorno.dato;
                                            if(arreglo.T == tipoaux)
                                            {
                                                b.dato = retorno.dato;
                                                b.taux = retorno.taux;
                                                b.t = retorno.t;
                                            }
                                            else { /*NO SON DEL MISMO TIPO*/
                                                listaerrores.Add(new Error(retorno.fila, retorno.columna, "Tipo de retorno diferente"));
                                            }

                                        }
                                    }
                                    else
                                    {
                                        /*DEBE RETORNAR ALGO*/
                                        if(tipo != TYPE.VOID)
                                        listaerrores.Add(new Error(retorno.fila, retorno.columna, "No se ha retornado nada"));
                                    }
                                }
                                else {
                                    /*ERROR DE PARAMETROS*/
                                    listaerrores.Add(new Error(b.fila, b.columna, "El metodo no contiene parametros"));
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
                                                if(l1.ChildNodes.Count==2)
                                                    h1.addVariable(l1.ChildNodes[1].Token.Text, new Variable(dato.columna,dato.fila,l1.ChildNodes[1].Token.Text,dato.dato,dato.t));
                                                else
                                                    h1.addVariable(l1.ChildNodes[2].Token.Text, new Variable(dato.columna, dato.fila, l1.ChildNodes[2].Token.Text, dato.dato, dato.t));
                                            }
                                            else {
                                                //ERROR DE TIPO DE PARAMETROS
                                                listaerrores.Add(new Error(dato.fila, dato.columna, "Tipo de parametro incorrecto"));
                                                return b;
                                            }
                                        }
                                        Variable retorno = Recorrido(nodo.ChildNodes[3], h1);
                                        TYPE tipo = getType(nodo.ChildNodes[1].Token.Text.ToLower());
                                        if (retorno.t == TYPE.CONTINUAR || retorno.t == TYPE.SALIR)
                                        {
                                            /*ERROR EN IF CONTINUAR*/
                                            listaerrores.Add(new Error(retorno.fila, retorno.columna, "Error en ambiente If"));
                                        }
                                        else if (retorno.t == TYPE.RETURN)
                                        {
                                            if (tipo == TYPE.VOID)
                                            {
                                                /*ERROR EL TIPO VOID NO RETORNA NADA*/
                                                listaerrores.Add(new Error(retorno.fila, retorno.columna, "Tipo Void no debe retornar algo"));
                                            }
                                            else if (retorno.taux == tipo)
                                            {
                                                /*RETORRNO CORRECTO*/
                                                b.dato = retorno.dato;
                                                b.taux = retorno.taux;
                                                b.t = retorno.t;
                                            }
                                            else
                                            {
                                                /*RETORNO INCORRECTO*/
                                                listaerrores.Add(new Error(retorno.fila, retorno.columna, "Dato de retorno incorrecto"));

                                            }
                                        }
                                        else
                                        {
                                            if(tipo != TYPE.VOID)
                                                listaerrores.Add(new Error(retorno.fila, retorno.columna, "No se ha retornado nada"));
                                        }
                                    }
                                    else {
                                        /*ERROR PARAMETROS*/
                                        listaerrores.Add(new Error(b.fila, b.columna, "Cantidad de parametros incorrecta"));

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
                                                if(l1.ChildNodes.Count == 2)
                                                    h1.addVariable(l1.ChildNodes[1].Token.Text, new Variable(dato.columna, dato.fila, l1.ChildNodes[1].Token.Text, dato, dato.t));
                                                else
                                                    h1.addVariable(l1.ChildNodes[2].Token.Text, new Variable(dato.columna, dato.fila, l1.ChildNodes[2].Token.Text, dato, dato.t));
                                            }
                                            else
                                            {
                                                /*ERROR DE COMPARACION DE TIPOS DE PARAMETRO*/
                                                listaerrores.Add(new Error(dato.fila, dato.columna, "Tipos de datos de parametros diferentes"));

                                                return b;
                                            }
                                        }
                                        Variable retorno = Recorrido(nodo.ChildNodes[5], h1);
                                        if(retorno.t == TYPE.CONTINUAR || retorno.t == TYPE.SALIR)
                                        {
                                            /*error de entornos*/
                                            listaerrores.Add(new Error(retorno.fila, retorno.columna, "Error en ambiente If"));
                                        }
                                        else if(retorno.t == TYPE.RETURN)
                                        {
                                            TYPE tipo = getType(nodo.ChildNodes[1].Token.Text);
                                            TYPE tipoaux = getType(nodo.ChildNodes[2].Token.Text);
                                            if (tipoaux == retorno.taux)
                                            {
                                                Arreglo arreglo = (Arreglo)retorno.dato;
                                                if (arreglo.T == tipoaux)
                                                {
                                                    b.dato = retorno.dato;
                                                    b.t = retorno.t;
                                                    b.taux = retorno.taux;
                                                }
                                                else {
                                                    /*ERROR DE TIPOS*/
                                                    listaerrores.Add(new Error(retorno.fila, retorno.columna, "Tipo de retorno diferente"));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            /*ERROR DEBE RETORNAR ALGO*/
                                            listaerrores.Add(new Error(retorno.fila, retorno.columna, "No se ha retornado nada"));

                                        }
                                    }
                                    else {
                                        /*ERROR DE PARAMETROS*/
                                        listaerrores.Add(new Error(b.fila, b.columna, "Cantidad de parametros incorrecta"));

                                    }
                                }
                                else {
                                    /*ERROR DE PARAMETROS*/
                                }
                            }
                        }
                        else {
                            /*meotodo no encontrado*/
                            listaerrores.Add(new Error(b.fila, b.columna, "No se encontro el metodo: " + nombre));
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
                                    if (n1.t == TYPE.STRING && n2.t == TYPE.BOOL && n3.esnum() && n4.esnum() && n5.esnum() && n6.esnum())
                                    {
                                        int num3 = Convert.ToInt32(Double.Parse(n3.dato.ToString()));
                                        int num4 = Convert.ToInt32(Double.Parse(n4.dato.ToString()));
                                        int num5 = Convert.ToInt32(Double.Parse(n5.dato.ToString()));
                                        int num6 = Convert.ToInt32(Double.Parse(n6.dato.ToString()));
                                        Rectangle rec = new Rectangle(num3 - num6 / 2, num4 - num5 / 2, num6, num5);
                                        if ((bool)n2.dato)
                                        {
                                            Brush brus = new SolidBrush(getcolor(n1.dato.ToString()));
                                            images.Add(new Figure(brus, rec, 1));
                                        }
                                        else
                                        {
                                            Pen pen = new Pen(getcolor(n1.dato.ToString()));
                                            images.Add(new Figure(pen, rec, 1));
                                        }
                                    }
                                    else {
                                        listaerrores.Add(new Error(n1.fila, n1.columna, "Tipos de parametros erroneos para Square"));
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
                                        Pen pen = new Pen(getcolor(n1.dato.ToString()), float.Parse(n6.dato.ToString()));

                                        Point[] p = { new Point(Convert.ToInt32(Double.Parse(n2.dato.ToString())), Convert.ToInt32(Double.Parse(n3.dato.ToString()))),
                                        new Point(Convert.ToInt32(Double.Parse(n4.dato.ToString())), Convert.ToInt32(Double.Parse(n5.dato.ToString())) )};
                                        images.Add(new Figure(pen, p, 2));
                                    }
                                    else {
                                        listaerrores.Add(new Error(n1.fila, n1.columna, "Tipos de Parametros erroneos para Line"));
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
                                        int radio = Convert.ToInt32(Double.Parse(n2.dato.ToString()));
                                        int x = Convert.ToInt32(Double.Parse(n4.dato.ToString()));
                                        int y = Convert.ToInt32(Double.Parse(n5.dato.ToString()));
                                        Rectangle rec = new Rectangle(x - radio, y - radio, radio * 2, radio * 2);
                                        if ((bool)n3.dato)
                                        {
                                            Brush brus = new SolidBrush(getcolor(n1.dato.ToString()));
                                            images.Add(new Figure(brus, rec,3));
                                        }
                                        else {
                                            Pen pen = new Pen(getcolor(n1.dato.ToString()));
                                            images.Add(new Figure(pen, rec, 3));
                                        }
                                    }
                                    else
                                    {
                                        listaerrores.Add(new Error(n1.fila, n1.columna, "Tipos de parametros erroneos para Circle"));
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
                                        Point[] a = { new Point(Convert.ToInt32(Double.Parse(n3.dato.ToString())), Convert.ToInt32(Double.Parse(n4.dato.ToString()))),
                                        new Point(Convert.ToInt32(Double.Parse(n5.dato.ToString())), Convert.ToInt32(Double.Parse(n6.dato.ToString()))),
                                        new Point(Convert.ToInt32(Double.Parse(n7.dato.ToString())), Convert.ToInt32(Double.Parse(n8.dato.ToString())) )};
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
                                    else
                                    {
                                        listaerrores.Add(new Error(n1.fila, n1.columna, "Tipos de parametros erroneos para Triangle"));
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
                        Variable title = Recorrido(raiz.ChildNodes[1], h);
                        if (title.t != TYPE.ERROR) {
                            Form2 fr = (Form2)forms[forms.Count - 1];
                            String titulo = title.dato.ToString();
                            fr.Visible = true;
                            fr.Text = titulo;
                            Graphics o = fr.picture.CreateGraphics();
                            foreach (Figure f in images)
                            {
                                if (f.tipo == 1)
                                {
                                    Rectangle r = (Rectangle)f.figure;
                                    if (f.pen is Pen)
                                    {
                                        Pen p = (Pen)f.pen;
                                        o.DrawRectangle(p, r);
                                    }
                                    else
                                    {
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
                                else
                                {
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
                        }
                        else
                        {
                            listaerrores.Add(new Error(title.fila,title.columna, "Devolvio error"));
                        }
                        break;
                    }

                case "d1": {
                        Variable clase = (Variable)h.getValue(raiz.ChildNodes[0].Token.Text);
                        if (clase != null && clase.dato is Clase) {
                            Clase c = (Clase)clase.dato;
                            ParseTreeNode funcion = raiz.ChildNodes[1];
                            String nombre = "$" + funcion.ChildNodes[0].Token.Text;
                            Variable metodo = (Variable)c.principal.getValue(nombre);
                            if (metodo != null && !metodo.Private)
                            {
                                ParseTreeNode nodo = (ParseTreeNode)metodo.dato;
                                Entorno nuevo = new Entorno(c.principal);
                                if (funcion.ChildNodes.Count == 1)
                                {
                                    if (nodo.ChildNodes.Count == 3)
                                    {
                                        Variable retorno = Recorrido(nodo.ChildNodes[2], nuevo);
                                        TYPE tipo = getType(nodo.ChildNodes[1].Token.Text);
                                        if (retorno.t == TYPE.CONTINUAR || retorno.t == TYPE.SALIR)
                                        {
                                            /*ERROR EN IF CONTINUAR*/
                                            listaerrores.Add(new Error(b.fila, b.columna, "Error en ambiente If"));
                                        }
                                        else if (retorno.t == TYPE.RETURN)
                                        {
                                            if (tipo == TYPE.VOID)
                                            {
                                                /*ERROR EL TIPO VOID NO RETORNA NADA*/
                                                listaerrores.Add(new Error(b.fila, b.columna, "Tipo Void no debe retornar nada"));

                                            }
                                            else if (retorno.taux == tipo)
                                            {
                                                /*RETORRNO CORRECTO*/
                                                b.dato = retorno.dato;
                                                b.taux = retorno.taux;
                                                b.t = retorno.t;
                                            }
                                            else
                                            {
                                                /*RETORNO INCORRECTO*/
                                                listaerrores.Add(new Error(b.fila, b.columna, "Retorno incorrecto"));
                                            }
                                        }
                                        else
                                        {
                                            if (tipo != TYPE.VOID)
                                            {
                                                /*ERROR NO RETORNA NADA*/
                                                listaerrores.Add(new Error(b.fila, b.columna, "No se ha retornado nada"));

                                            }
                                        }
                                    }
                                    else if (nodo.ChildNodes.Count == 5)
                                    {
                                        Variable retorno = Recorrido(nodo.ChildNodes[4], nuevo);
                                        TYPE tipo = getType(nodo.ChildNodes[1].Token.Text);
                                        if (retorno.t == TYPE.CONTINUAR || retorno.t == TYPE.SALIR)
                                        {
                                            /*ERROR SALIR EN EL ENTORNO INCORRECTO*/
                                            listaerrores.Add(new Error(b.fila, b.columna, "Error en ambiente If"));

                                        }
                                        else if (retorno.t == TYPE.RETURN)
                                        {
                                            if (tipo == retorno.taux)
                                            {
                                                b.dato = retorno.dato;
                                                b.taux = retorno.taux;
                                                b.t = retorno.t;
                                            }
                                        }
                                        else
                                        {
                                            /*DEBE RETORNAR ALGO*/
                                            listaerrores.Add(new Error(b.fila, b.columna, "No se ha retornado nada"));

                                        }
                                    }
                                    else
                                    {
                                        /*ERROR DE PARAMETROS*/
                                        listaerrores.Add(new Error(b.fila, b.columna, "El metodo no contiene parametros"));

                                    }
                                }
                                else
                                {
                                    if (nodo.ChildNodes.Count == 4)
                                    {
                                        ParseTreeNode param1 = nodo.ChildNodes[2];
                                        ParseTreeNode param2 = raiz.ChildNodes[1].ChildNodes[1];
                                        if (param1.ChildNodes.Count == param2.ChildNodes.Count)
                                        {
                                            Entorno h1 = new Entorno(c.principal);
                                            for (int i = 0; i < param1.ChildNodes.Count; i++)
                                            {
                                                ParseTreeNode l1 = param1.ChildNodes[i];
                                                Variable dato = Recorrido(param2.ChildNodes[i], h);
                                                if (comprobar(l1.ChildNodes[0].Token.Text.ToLower(), dato.t))
                                                {
                                                    if (l1.ChildNodes.Count == 2)
                                                        h1.addVariable(l1.ChildNodes[1].Token.Text, new Variable(dato.columna, dato.fila, l1.ChildNodes[1].Token.Text, dato.dato, dato.t));
                                                    else
                                                        h1.addVariable(l1.ChildNodes[2].Token.Text, new Variable(dato.columna, dato.fila, l1.ChildNodes[2].Token.Text, dato.dato, dato.t));
                                                }
                                                else
                                                {
                                                    listaerrores.Add(new Error(b.fila, b.columna, "Tipo de dato de parametro incorrecto"));
                                                    //ERROR DE TIPO DE PARAMETROS
                                                    return b;
                                                }
                                            }
                                            Variable retorno = Recorrido(nodo.ChildNodes[3], h1);
                                            if (retorno.t == TYPE.CONTINUAR || retorno.t == TYPE.SALIR)
                                            {
                                                /*ERROR EN IF CONTINUAR*/
                                                listaerrores.Add(new Error(b.fila, b.columna, "Error en ambiente If"));
                                            }
                                            else if (retorno.t == TYPE.RETURN)
                                            {
                                                TYPE tipo = getType(nodo.ChildNodes[1].Token.Text.ToLower());
                                                if (tipo == TYPE.VOID)
                                                {
                                                    /*ERROR EL TIPO VOID NO RETORNA NADA*/
                                                    listaerrores.Add(new Error(b.fila, b.columna, "Tipo de dato void no debe retornar nada"));

                                                }
                                                else if (retorno.taux == tipo)
                                                {
                                                    /*RETORRNO CORRECTO*/
                                                    b.dato = retorno.dato;
                                                    b.taux = retorno.taux;
                                                    b.t = retorno.t;
                                                }
                                                else
                                                {
                                                    /*RETORNO INCORRECTO*/
                                                    listaerrores.Add(new Error(b.fila, b.columna, "Retorno incorrecto"));

                                                }
                                            }
                                            else
                                            {
                                                listaerrores.Add(new Error(b.fila, b.columna, "No se ha retornado nada"));

                                            }
                                        }
                                        else
                                        {
                                            /*ERROR PARAMETROS*/
                                            listaerrores.Add(new Error(b.fila, b.columna, "Cantidad de parametros incorrecta"));

                                        }
                                    }
                                    else if (nodo.ChildNodes.Count == 6)
                                    {
                                        ParseTreeNode param1 = nodo.ChildNodes[4];
                                        ParseTreeNode param2 = raiz.ChildNodes[1];
                                        if (param1.ChildNodes.Count == param2.ChildNodes.Count)
                                        {
                                            Entorno h1 = new Entorno(c.principal);
                                            for (int i = 0; i < param1.ChildNodes.Count; i++)
                                            {
                                                ParseTreeNode l1 = param1.ChildNodes[i];
                                                Variable dato = Recorrido(param2.ChildNodes[i], h);
                                                if (comprobar(l1.ChildNodes[0].Token.Text.ToLower(), dato.t))
                                                {
                                                    if (l1.ChildNodes.Count == 2)
                                                        h1.addVariable(l1.ChildNodes[1].Token.Text, new Variable(dato.columna, dato.fila, l1.ChildNodes[1].Token.Text, dato, dato.t));
                                                    else
                                                        h1.addVariable(l1.ChildNodes[2].Token.Text, new Variable(dato.columna, dato.fila, l1.ChildNodes[2].Token.Text, dato, dato.t));
                                                }
                                                else
                                                {
                                                    /*ERROR DE COMPARACION DE TIPOS DE PARAMETRO*/
                                                    listaerrores.Add(new Error(b.fila, b.columna, "Parametro incorrecto"));

                                                    return b;
                                                }
                                            }
                                            Variable retorno = Recorrido(nodo.ChildNodes[5], h1);
                                            if (retorno.t == TYPE.CONTINUAR || retorno.t == TYPE.SALIR)
                                            {
                                                /*error de entornos*/
                                                listaerrores.Add(new Error(b.fila, b.columna, "Error en ambiente If"));

                                            }
                                            else if (retorno.t == TYPE.RETURN)
                                            {
                                                TYPE tipo = getType(nodo.ChildNodes[1].Token.Text.ToLower());
                                                if (tipo == retorno.taux)
                                                {
                                                    b.dato = retorno.dato;
                                                    b.t = retorno.t;
                                                    b.taux = retorno.taux;
                                                }
                                            }
                                            else
                                            {
                                                /*ERROR DEBE RETORNAR ALGO*/
                                                listaerrores.Add(new Error(b.fila, b.columna, "No se ha retornado nada"));

                                            }
                                        }
                                        else
                                        {
                                            /*ERROR DE PARAMETROS*/
                                            listaerrores.Add(new Error(b.fila, b.columna, "Cantidad de parametros incorrecta"));

                                        }
                                    }
                                    else
                                    {
                                        /*ERROR DE PARAMETROS*/
                                        listaerrores.Add(new Error(b.fila, b.columna, "Cantidad de parametros incorrecta"));

                                    }
                                }
                            }
                            else {
                                /*METODO NO ENCONTRADO*/
                                listaerrores.Add(new Error(b.fila, b.columna, "Metodo no encontrado: " + nombre));

                            }
                        }
                        else
                        {
                            /*NO SE ENCONTRO LA CLASE*/
                            listaerrores.Add(new Error(b.fila,b.columna, "Clase no encontrada"));

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
            else if (t2 == TYPE.CLASS)
            {
                return true;
            }
            else if (t1 == "array" && t2 == TYPE.ARRAY)
                return true;
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
            switch (tipo.ToLower())
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
