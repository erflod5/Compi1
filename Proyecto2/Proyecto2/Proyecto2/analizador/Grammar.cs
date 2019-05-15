using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;

namespace Proyecto2.analizador
{
    class Grammar : Irony.Parsing.Grammar
    {
        public Grammar() : base(caseSensitive: false) {
            #region ER
            RegexBasedTerminal num = new RegexBasedTerminal("num", "[0-9]+");
            RegexBasedTerminal dec = new RegexBasedTerminal("dec", "[0-9]+[.][0-9]+");
            RegexBasedTerminal rtrue = new RegexBasedTerminal("rtrue", "true|verdadero");
            RegexBasedTerminal rfalse = new RegexBasedTerminal("rfalse", "false|falso");
            CommentTerminal com_linea = new CommentTerminal("comentariolinea", ">>", "\n", "\r\n");
            CommentTerminal comentarioBloque = new CommentTerminal("comentarioBloque", "<-", "->");
            CommentTerminal com_multi = new CommentTerminal("comentariomulti", "<-", "->");

            StringLiteral StringLiteral = TerminalFactory.CreateCSharpString("String");
            StringLiteral CharLiteral = TerminalFactory.CreateCSharpChar("Char");
            IdentifierTerminal identifier = new IdentifierTerminal("Identifier");

            NonGrammarTerminals.Add(com_linea);
            NonGrammarTerminals.Add(comentarioBloque);
            NonGrammarTerminals.Add(com_multi);
            #endregion

            #region Terminales

            /*simbolos*/
            var igual = ToTerm("=");
            var aumento = ToTerm("++");
            var decremento = ToTerm("--");
            var punto = ToTerm(".");


            /*aritmeticos*/
            var mas = ToTerm("+");
            var menos = ToTerm("-");
            var por = ToTerm("*");
            var entre = ToTerm("/");
            var elevado = ToTerm("^");

            /*relacionales*/
            var igualLogico = ToTerm("==");
            var desigual = ToTerm("!=");
            var mayor = ToTerm(">");
            var mayorigual = ToTerm(">=");
            var menor = ToTerm("<");
            var menorigual = ToTerm("<=");

            /*logicos*/
            var or = ToTerm("||");
            var and = ToTerm("&&");
            var not = ToTerm("!");


            /*tipos de dato*/
            var rint = ToTerm("int");
            var rdouble = ToTerm("double");
            var rbool = ToTerm("bool");
            var rchar = ToTerm("char");
            var rstring = ToTerm("string");
            var rarray = ToTerm("array");

            /*Reservadas*/
            var rclase = ToTerm("clase");
            var rimportar = ToTerm("importar");
            var rvoid = ToTerm("void");
            var rpublico = ToTerm("publico");
            var rprivado = ToTerm("privado");
            var rreturn = ToTerm("return");
            var rnew = ToTerm("new");
            var roverride = ToTerm("override");
            var rmain = ToTerm("main");
            var rprint = ToTerm("print");
            var rshow = ToTerm("show");

            /*sentencias*/
            var rif = ToTerm("if");
            var relse = ToTerm("else");
            var rfor = ToTerm("for");
            var rrepeat = ToTerm("repeat");
            var rwhile = ToTerm("while");
            var rcomprobar = ToTerm("comprobar");
            var rcaso = ToTerm("caso");
            var rsalir = ToTerm("salir");
            var rdefecto = ToTerm("defecto");
            var rhacer = ToTerm("hacer");
            var rmientras = ToTerm("mientras");
            var rcontinuar = ToTerm("continuar");

            /*figure*/
            var raddfigure = ToTerm("addfigure");
            var rcircle = ToTerm("circle");
            var rtriangle = ToTerm("triangle");
            var rsquare = ToTerm("square");
            var rline = ToTerm("line");
            var rfigure = ToTerm("figure");

            #endregion

            #region No Terminales
            NonTerminal S = new NonTerminal("S"),
            E = new NonTerminal("E"),
            FUNCION = new NonTerminal("Funcion"), PARAMFUNCION = new NonTerminal("ParamFuncion"),
            BLOQUE = new NonTerminal("BLOQUE"), LISTA_BLOQUE = new NonTerminal("LISTA_BLOQUE"),
            ASIGNACION = new NonTerminal("Asignacion"), DECLARACION = new NonTerminal("Declaracion"),
            METODOS = new NonTerminal("Metodos"), SENTENCIAS = new NonTerminal("Sentencias"),
            LID = new NonTerminal("ListaId"), T_DATO = new NonTerminal("TipoDato"), DIM = new NonTerminal("Dimension"),
            LDIM = new NonTerminal("LDim"), DIM1 = new NonTerminal("Dim1"), DIM2 = new NonTerminal("Dim2"), DIM3 = new NonTerminal("Dim3"),
            LISTACLASE = new NonTerminal("ListaClase"), BLOCKCLASE = new NonTerminal("Bloqueclase"),
            LPARAMETROS = new NonTerminal("ListaParametros"), VISIBILIDAD = new NonTerminal("Visibilidad"),
            OVERRIDE = new NonTerminal("Override"), LISTAMSENTENCIA = new NonTerminal("ListaMSentencia"), MSENTENCIA = new NonTerminal("MSentencia"),
            INMETODO = new NonTerminal("InMetodo"), BLOQUESHOW = new NonTerminal("BloqueShow"), BLOQUEPRINT = new NonTerminal("BloquePrint"),
            BLOQUEIF = new NonTerminal("BloqueIf"), BLOQUEREPETIR = new NonTerminal("BloqueRepetir"), BLOQUEFOR = new NonTerminal("BloqueFor"),
            BLOQUEWHILE = new NonTerminal("BloqueWhile"), BLOQUEELSEIF = new NonTerminal("BloqueElseIf"), BLOQUEHACER = new NonTerminal("BloqueHacer"),
            BLOQUECOMPROBAR = new NonTerminal("BloqueComprobar"), BLOQUECASO = new NonTerminal("BloqueCaso"), ADDFIGURE = new NonTerminal("AddFigure"),
            FIGURE = new NonTerminal("Figure"), TFIGURE = new NonTerminal("TFigure"), B3 = new NonTerminal("B3"), B2 = new NonTerminal("B2"),
            MAIN = new NonTerminal("Main"), L1 = new NonTerminal("L1"), C1 = new NonTerminal("C1"), D1 = new NonTerminal("D1"),
            IF = new NonTerminal("IF"), ElseIF = new NonTerminal("ELSEIF"), ELSE = new NonTerminal("ELSE"),
            BARREGLO = new NonTerminal("BArreglo"), T_DATO1 = new NonTerminal("TipoDato");

            #endregion

            #region Gramaticas
            S.Rule = LISTACLASE;

            LISTACLASE.Rule = MakePlusRule(LISTACLASE, BLOCKCLASE);

            BLOCKCLASE.Rule = rclase + identifier + ToTerm("{") + LISTA_BLOQUE + ToTerm("}")  //3 ya
                    | rclase + identifier + rimportar + LID + ToTerm("{") + LISTA_BLOQUE + ToTerm("}") //5 ya
                    | rclase + identifier + ToTerm("{") + ToTerm("}") //2 ya
                    | rclase + identifier + rimportar + LID + ToTerm("{") + ToTerm("}"); //4 ya

            LISTA_BLOQUE.Rule = MakePlusRule(LISTA_BLOQUE, BLOQUE);


            BLOQUE.Rule = VISIBILIDAD + DECLARACION
                    | DECLARACION
                    | ASIGNACION
                    | VISIBILIDAD + METODOS
                    | METODOS
                    | MAIN
                    ;

            MAIN.Rule = rmain + ToTerm("(") + ToTerm(")") + ToTerm("{") + INMETODO + ToTerm("}");

            METODOS.Rule = identifier + rvoid + OVERRIDE + ToTerm("(") + LPARAMETROS + ToTerm(")") + ToTerm("{") + INMETODO + ToTerm("}") //4 hijos
                    | identifier + rvoid + OVERRIDE + ToTerm("(") + ToTerm(")") + ToTerm("{") + INMETODO + ToTerm("}") //3 hijos

                    | identifier + T_DATO + OVERRIDE + ToTerm("(") + ToTerm(")") + ToTerm("{") + INMETODO + ToTerm("}") //3 hijos
                    | identifier + T_DATO + OVERRIDE + ToTerm("(") + LPARAMETROS + ToTerm(")") + ToTerm("{") + INMETODO + ToTerm("}") //4 hijos

                    | identifier + rarray + T_DATO + DIM + OVERRIDE + ToTerm("(") + ToTerm(")") + ToTerm("{") + INMETODO + ToTerm("}") //5 hijos
                    | identifier + rarray + T_DATO + DIM + OVERRIDE + ToTerm("(") + LPARAMETROS + ToTerm(")") + ToTerm("{") + INMETODO + ToTerm("}"); //6 hijos

            OVERRIDE.Rule = roverride
                    | Empty;


            INMETODO.Rule = LISTAMSENTENCIA
                    | Empty;

            LISTAMSENTENCIA.Rule = MakePlusRule(LISTAMSENTENCIA, MSENTENCIA);

            MSENTENCIA.Rule = VISIBILIDAD + DECLARACION
                    | DECLARACION
                    | ASIGNACION
                    | BLOQUESHOW //YA
                    | BLOQUEPRINT //YA
                    | BLOQUEREPETIR //YA 
                    | BLOQUEWHILE //YA 
                    | BLOQUEIF //YA
                    | BLOQUEHACER //YA 
                    | BLOQUEFOR //YA 
                    | BLOQUECOMPROBAR //YA 
                    | ADDFIGURE//YA 
                    | FIGURE//YA
                    | FUNCION + ToTerm(";")
                    | D1 + ToTerm(";")
                    | rcontinuar + ToTerm(";") //YA
                    | rsalir + ToTerm(";") //YA
                    | rreturn + E + ToTerm(";"); //YA


            BLOQUEIF.Rule = IF
                    | IF + ELSE
                    | IF + BLOQUEELSEIF
                    | IF + BLOQUEELSEIF + ELSE;

            IF.Rule = rif + ToTerm("(") + E + ToTerm(")") + ToTerm("{") + INMETODO + ToTerm("}");

            ELSE.Rule = relse + ToTerm("{") + INMETODO + ToTerm("}");

            ElseIF.Rule = relse + rif + ToTerm("(") + E + ToTerm(")") + ToTerm("{") + INMETODO + ToTerm("}");

            BLOQUEELSEIF.Rule = MakePlusRule(BLOQUEELSEIF, ElseIF);

            BLOQUEREPETIR.Rule = rrepeat + ToTerm("(") + E + ToTerm(")") + ToTerm("{") + INMETODO + ToTerm("}");

            BLOQUEWHILE.Rule = rwhile + ToTerm("(") + E + ToTerm(")") + ToTerm("{") + INMETODO + ToTerm("}");

            BLOQUEPRINT.Rule = rprint + ToTerm("(") + E + ToTerm(")") + ToTerm(";");

            BLOQUESHOW.Rule = rshow + ToTerm("(") + E + ToTerm(",") + E + ToTerm(")") + ToTerm(";")
                    | rshow + ToTerm("(") + E + ToTerm(")") + ToTerm(";");

            BLOQUEHACER.Rule = rhacer + ToTerm("{") + INMETODO + ToTerm("}") + rmientras + ToTerm("(") + E + ToTerm(")") + ToTerm(";");

            BLOQUEFOR.Rule = rfor + ToTerm("(") + T_DATO + identifier + igual + E + ToTerm(";") + E + ToTerm(";") + ASIGNACION + ToTerm(")") + ToTerm("{") + INMETODO + ToTerm("}")
                    | rfor + ToTerm("(") + identifier + igual + E + ToTerm(";") + E + ToTerm(";") + ASIGNACION + ToTerm(")") + ToTerm("{") + INMETODO + ToTerm("}");

            BLOQUECOMPROBAR.Rule = rcomprobar + ToTerm("(") + E + ToTerm(")") + ToTerm("{") + BLOQUECASO + ToTerm("}");

            BLOQUECASO.Rule = MakePlusRule(BLOQUECASO, C1);

            C1.Rule = rcaso + E + ToTerm(":") + INMETODO
                    | rdefecto + ToTerm(":") + INMETODO;
                
            FIGURE.Rule = rfigure + ToTerm("(") + E + ToTerm(")") + ToTerm(";");

            ADDFIGURE.Rule = raddfigure + ToTerm("(") + TFIGURE + ToTerm(")") + ToTerm(";");

            TFIGURE.Rule = rcircle + ToTerm("(") + E + ToTerm(",") + E + ToTerm(",") + E + ToTerm(",") + E + ToTerm(",") + E + ToTerm(")")
                    | rtriangle + ToTerm("(") + E + ToTerm(",") + E + ToTerm(",") + E + ToTerm(",") + E + ToTerm(",") + E + ToTerm(",") + E + ToTerm(",") + E + ToTerm(",") + E + ToTerm(")")
                    | rsquare + ToTerm("(") + E + ToTerm(",") + E + ToTerm(",") + E + ToTerm(",") + E + ToTerm(",") + E + ToTerm(",") + E + ToTerm(")")
                    | rline + ToTerm("(") + E + ToTerm(",") + E + ToTerm(",") + E + ToTerm(",") + E + ToTerm(",") + E + ToTerm(",") + E + ToTerm(")");

            DECLARACION.Rule = T_DATO + LID + ToTerm(";")
                    | identifier + identifier  + ToTerm(";")
                    | T_DATO + LID + igual + E + ToTerm(";")
                    | T_DATO + rarray + LID + DIM + ToTerm(";")
                    | identifier + identifier + igual + rnew + identifier + ToTerm("(") + ToTerm(")") + ToTerm(";")
                    | identifier + identifier + igual + E + ToTerm(";")
                    | T_DATO + rarray + LID + DIM + igual + LDIM + ToTerm(";");



            ASIGNACION.Rule = identifier + igual + E + ToTerm(";") //2 hijos ya
                    | identifier + igual + rnew + identifier + ToTerm("(") + ToTerm(")") + ToTerm(";") //3 hijos ya
                    | identifier + DIM + igual + E + ToTerm(";") //3 hijos ya 
                    | D1 + DIM + igual + E + ToTerm(";")
                    | D1 + DIM + aumento + ToTerm(";")
                    | D1 + DIM + decremento + ToTerm(";")

                    | identifier + DIM + aumento + ToTerm(";")
                    | identifier + DIM + decremento + ToTerm(";")

                    | identifier + aumento + ToTerm(";") //2 hijos ya
                    | identifier + decremento + ToTerm(";") //2 hijos ya
                    | identifier + aumento
                    | identifier + decremento
                    | D1 + igual + E + ToTerm(";") //2 hijos pendiente
                    | D1 + aumento + ToTerm(";")
                    | D1 + decremento + ToTerm(";")
                ;

            LDIM.Rule = ToTerm("{") + DIM1 + ToTerm("}")
                    | ToTerm("{") + DIM2 + ToTerm("}")
                    | ToTerm("{") + DIM3 + ToTerm("}");


            DIM3.Rule = MakePlusRule(DIM3, ToTerm(","), B3);

            B3.Rule = ToTerm("{") + DIM2 + ToTerm("}");

            DIM2.Rule = MakePlusRule(DIM2, ToTerm(","), B2 );

            B2.Rule = ToTerm("{") + DIM1 + ToTerm("}");

            DIM1.Rule = MakePlusRule(DIM1,ToTerm(","),E);

            LID.Rule = MakePlusRule(LID,ToTerm(","),identifier);

            LPARAMETROS.Rule = MakePlusRule(LPARAMETROS, ToTerm(","), L1);

            L1.Rule = T_DATO + identifier
                    | T_DATO + rarray + identifier + DIM;

            T_DATO.Rule = rint
                    | rchar
                    | rdouble
                    | rbool
                    | rstring
                    | identifier;

            T_DATO1.Rule = rint
                    | rchar
                    | rdouble
                    | rbool
                    | rstring;

            VISIBILIDAD.Rule = rprivado
                    | rpublico;

            DIM.Rule = ToTerm("[") + E + ToTerm("]")
                    | ToTerm("[") + E + ToTerm("]") + ToTerm("[") + E + ToTerm("]")
                    | ToTerm("[") + E + ToTerm("]") + ToTerm("[") + E + ToTerm("]") + ToTerm("[") + E + ToTerm("]");

            E.Rule = E + or + E
                    | E + and + E
                    | not + E

                    | E + mayor + E
                    | E + mayorigual + E
                    | E + menor + E
                    | E + menorigual + E
                    | E + igualLogico + E
                    | E + desigual + E

                    | E + mas + E
                    | E + menos + E
                    | E + por + E
                    | E + entre + E
                    | E + elevado + E

                    | menos + E
                    | D1 + aumento
                    | D1 + decremento

                    | dec
                    | rtrue
                    | rfalse
                    | StringLiteral
                    | CharLiteral
                    | num
                    | ToTerm("(") + E + ToTerm(")")
                    | D1
                    | BARREGLO
                    | BARREGLO + aumento
                    | BARREGLO + decremento;

            BARREGLO.Rule = D1 + ToTerm("[") + E + ToTerm("]")
                    | D1 + ToTerm("[") + E + ToTerm("]") + ToTerm("[") + E + ToTerm("]")
                    | D1 + ToTerm("[") + E + ToTerm("]") + ToTerm("[") + E + ToTerm("]") + ToTerm("[") + E + ToTerm("]");



            D1.Rule = MakePlusRule(D1, punto, identifier)
                    | MakePlusRule(D1,punto,FUNCION);

            FUNCION.Rule = identifier + ToTerm("(") + PARAMFUNCION + ToTerm(")")
                    | identifier + ToTerm("(") + ToTerm(")");

            PARAMFUNCION.Rule = MakePlusRule(PARAMFUNCION, ToTerm(","), E);
                    

            #endregion

            #region Preferencias
            this.Root = S;
            this.MarkTransient(T_DATO,OVERRIDE,INMETODO,VISIBILIDAD,B2,B3);   
            this.RegisterOperators(1, Associativity.Left, or);
            this.RegisterOperators(2, Associativity.Left, and);
            this.RegisterOperators(3, Associativity.Right, not);
            this.RegisterOperators(4, Associativity.Left, mayor, mayorigual, menor, menorigual, igualLogico, desigual);
            this.RegisterOperators(5, Associativity.Left, mas, menos);
            this.RegisterOperators(6, Associativity.Left, por, entre);
            this.RegisterOperators(7, Associativity.Left, elevado);
            this.RegisterOperators(8, Associativity.Left, aumento, decremento);
            this.MarkPunctuation("(", ")", ",", ";", ":", "[", "]", "{", "}","=");

            #endregion
        }
    }
}
