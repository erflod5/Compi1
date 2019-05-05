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
                    clases.Add(name, node);
                    generarImagen(name, node);
                    if (main_node == null) {
                        if (node.ChildNodes.Count == 3){
                            SearchMain(node.ChildNodes[2]);
                        }
                        else{
                            SearchMain(node.ChildNodes[4]);
                        }
                    }
                }
            }
            if (main_node != null) {
                generarImagen("main", main_node);
                if (main_node.ChildNodes.Count == 2) {
                    Recorrido(main_node.ChildNodes[1], new Entorno());
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
                if (node.ChildNodes.Count ==1 && node.ChildNodes[0].Term.Name.ToLower()=="main") {
                    if (main_node == null)
                        main_node = node.ChildNodes[0];
                    else {
                        /*Error doble main*/
                    }
                }
            }
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
                            Recorrido(node, h);
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
                                                foreach (ParseTreeNode node in nodeD.ChildNodes[1].ChildNodes) {
                                                    h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column,node.Token.Text,null,tipo,false));
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
                                                            Arreglo arr = new Arreglo(data, data2);
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
                                                            Arreglo arr = new Arreglo(data, data2, data3);
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
                                                                Arreglo arr = new Arreglo(data, data2);
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
                                                            Arreglo arr = new Arreglo(data, data2, data3);
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
                                                break;
                                            }
                                    }
                                    break;
                                case "asignacion":

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
                                            if (body.t != TYPE.ERROR)
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
                                case "bloqueif":
                                    {
                                        Recorrido(nodeD, h);
                                        break;
                                    }
                                case "bloquewhile":
                                    {
                                        Recorrido(nodeD, h);
                                        break;
                                    }
                                case "bloquerepetir":
                                    {
                                        Recorrido(nodeD, h);
                                        break;
                                    }
                                case "bloquehacer":
                                    { Recorrido(nodeD, h);
                                        break;
                                    }
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
                                                foreach (ParseTreeNode node in nodeD.ChildNodes[1].ChildNodes)
                                                {
                                                    h.addVariable(node.Token.Text, new Variable(node.Span.Location.Line, node.Span.Location.Column, node.Token.Text, null, tipo, @private));
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
                            }
                        }
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
                                        Recorrido(bloqueif.ChildNodes[2], nuevo);
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
                                        Recorrido(bloqueif.ChildNodes[2], nuevo);
                                    }
                                }
                                else {
                                    ParseTreeNode nuevo = raiz.ChildNodes[1];
                                    if (nuevo.Term.Name == "ELSE")
                                    {
                                        if (nuevo.ChildNodes.Count == 2) {
                                            Entorno h1 = new Entorno(h);
                                            Recorrido(nuevo.ChildNodes[1],h1);
                                        }
                                    }
                                    else {
                                        Recorrido(nuevo, h);
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
                                        Recorrido(bloqueif.ChildNodes[2], nuevo);
                                    }
                                }
                                else {
                                    Variable condicion2 = Recorrido(raiz.ChildNodes[1], h);
                                    if (!(bool)condicion2.dato) {
                                        ParseTreeNode nuevo = raiz.ChildNodes[2];
                                        if(nuevo.ChildNodes.Count == 2){
                                            Entorno h1 = new Entorno(h);
                                            Recorrido(nuevo.ChildNodes[1], h1);
                                        }
                                    }
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
                        b.t = TYPE.BOOL;
                        b.dato = false;
                        foreach (ParseTreeNode node in raiz.ChildNodes) {
                            Variable condicion = Recorrido(node, h);
                            if ((bool)condicion.dato) {
                                b.dato = true;
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
                                    Recorrido(raiz.ChildNodes[2], h1);
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
                                    Recorrido(raiz.ChildNodes[2], h1);
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
                                    Recorrido(raiz.ChildNodes[1], h1);
                                    sentencia = Recorrido(raiz.ChildNodes[3], h);
                                } while ((bool)sentencia.dato);
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
                                    Recorrido(raiz.ChildNodes[3], h1);
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
                                if (expr.ChildNodes[0].Term.Name == "Identifier")
                                {
                                    Variable o = (Variable)h.getValue(expr.ChildNodes[0].Token.Text);
                                    if (o != null) {
                                        if (o.dato != null)
                                        {
                                            b.dato = o.dato;
                                            b.t = o.t;
                                        }
                                    }
                                }
                                else if (expr.ChildNodes[0].Term.Name == "Funcion") {

                                }
                            }
                            else if (child == 2) {

                            }
                        }
                        break;
                    }
            }
            return b;
        }
    }
}
