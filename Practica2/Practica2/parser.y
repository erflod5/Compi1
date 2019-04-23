%{
#include "scanner.h"
#include "qdebug.h"
#include "nodoast.h"
#include <iostream>
#include "gerror.h"
extern int yylineno;
extern int columna;
extern char *yytext;
extern NodoAst *raiz;
extern QList<gError> lerror;

int yyerror(const char* mens)
{
    std::cout << mens <<" "<<yytext<< std::endl;
    lerror.append(*new gError(2,columna,yylineno,yytext));
    return 0;
}
%}
//error-verbose si se especifica la opcion los errores sintacticos son especificados por BISON
%defines "parser.h"
%output "parser.cpp"
%error-verbose
%locations
%union{
//se especifican los tipo de valores para los no terminales y lo terminales
char TEXT [256];
class NodoAst *nodo;
}

%token<TEXT> r_int r_string r_double r_char r_bool r_arreglo;

%token<TEXT> r_imprimir r_show r_si r_sino r_para r_repetir;

%token<TEXT> entero decimal caracter booleano cadena;

%token<TEXT> igual_logico desigual mayor mayor_igual menor menor_igual;

%token<TEXT> oor aand noot;

%token<TEXT> aumento decremento mas menos por entre potencia;

%token<TEXT> par_i par_d puntocoma coma igual cor_i cor_d llave_i llave_d;

%token<TEXT> id;


/*No terimanesl*/
%type<nodo> INICIO;//
%type<nodo> E L BLOCK;
%type<nodo> B_VAR B_IF B_PARA B_REPETIR B_IMPRIMIR B_SHOW B_ELSE;
%type<nodo> DECLARA_VAR ASIGNA_VAR TIPO_VAR IN_PARA TIPO_UN;
%type<nodo> L_ID L_SINOSI L_ARRAY L_FILA L_COLUMNA L_PROF;
%left oor
%left aand
%left noot
%left igual_logico desigual mayor mayor_igual menor menor_igual
%left mas menos
%left por entre
%left potencia
%start INICIO
%%
INICIO : L { raiz = $$; }
;
L: L  BLOCK
        {
          $$ = $1; $$->add(*$2);
        }
        | BLOCK
        {
          Tipo t = PRINCIPAL;
          $$ = new NodoAst(yylineno,columna,t,"principal","principal");
          $$->add(*$1);
        }
;

BLOCK : B_VAR {
          $$ = $1;
        }
        |B_IF{
          $$ = $1;
        }
        |B_PARA{
          $$ = $1;
        }
        |B_REPETIR{
          $$ = $1;
        }
        |B_IMPRIMIR{
          $$ = $1;
        }
        |B_SHOW{
          $$ = $1;
        }
;

B_VAR : DECLARA_VAR {
          $$ = $1;
        }
        | ASIGNA_VAR{
          $$ = $1;
        }
;

DECLARA_VAR : TIPO_VAR L_ID puntocoma{
          Tipo t = LID;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"LID","LID");
          nuevo->add(*$1); nuevo->add(*$2);
          $$ = nuevo;
        }
        | TIPO_VAR L_ID igual E puntocoma{
          Tipo t = LID;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"LID","LID");
          nuevo->add(*$1); nuevo->add(*$2); nuevo->add(*$4);
          $$ = nuevo;
        }
        | TIPO_VAR r_arreglo L_ID L_ARRAY puntocoma{
          Tipo t = LARRAY;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"Larray","Larray");
          nuevo->add(*$1); nuevo->add(*$3); nuevo->add(*$4);
          $$ = nuevo;
        }
        | TIPO_VAR r_arreglo L_ID L_ARRAY igual llave_i L_FILA llave_d puntocoma{
          Tipo t = LARRAY;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"Larray","Larray");
          nuevo->add(*$1); nuevo->add(*$3); nuevo->add(*$4); nuevo->add(*$7);
          $$ = nuevo;
        }
        | TIPO_VAR r_arreglo L_ID L_ARRAY igual llave_i L_COLUMNA llave_d  puntocoma{
          Tipo t = LARRAY;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"Larray","Larray");
          nuevo->add(*$1); nuevo->add(*$3); nuevo->add(*$4); nuevo->add(*$7);
          $$ = nuevo;
        }
        | TIPO_VAR r_arreglo L_ID L_ARRAY igual llave_i L_PROF llave_d puntocoma{
          Tipo t = LARRAY;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"Larray","Larray");
          nuevo->add(*$1); nuevo->add(*$3); nuevo->add(*$4); nuevo->add(*$7);
          $$ = nuevo;
        }

;

TIPO_VAR : r_int{
          Tipo t = RINT;
          $$ = new NodoAst(yylineno,columna,t,"int","int");
        }
        | r_string{
          Tipo t = RSTRING;
          $$ = new NodoAst(yylineno,columna,t,"string","string");
        }
        |r_double{
          Tipo t = RDOUBLE;
          $$ = new NodoAst(yylineno,columna,t,"double","double");
        }
        |r_char{
          Tipo t = RCHAR;
          $$ = new NodoAst(yylineno,columna,t,"char","char");
        }
        |r_bool{
          Tipo t = RBOOL;
          $$ = new NodoAst(yylineno,columna,t,"bool","bool");
        }
;

ASIGNA_VAR : id igual E puntocoma{
          Tipo t = ASID;
          $$ = new NodoAst(yylineno,columna,t,"Asignacion","Asignacion");
          Tipo t1 = RID; NodoAst *nq = new NodoAst(yylineno,columna,t1,$1,"id");
          $$->add(*nq);
          $$->add(*$3);
        }
        | id TIPO_UN puntocoma{
          Tipo t = ASID;
          $$ = new NodoAst(yylineno,columna,t,"Asignacion","Asignacion");
          Tipo t1 = RID; NodoAst *nq = new NodoAst(yylineno,columna,t1,$1,"id");
          $$->add(*nq);
          $$->add(*$2);
        }
        | id cor_i E cor_d igual E puntocoma{
          Tipo t = ASARR;
          $$ = new NodoAst(yylineno,columna,t,"AsignacionArray","AsignacionArray");
          Tipo t1 = RID; NodoAst *nq = new NodoAst(yylineno,columna,t1,$1,"id");
          $$->add(*nq);
          $$->add(*$3);
          $$->add(*$6);
        }

          | id cor_i E cor_d cor_i E cor_d igual E puntocoma{
            Tipo t = ASARR;
            $$ = new NodoAst(yylineno,columna,t,"AsignacionArray","AsignacionArray");
            Tipo t1 = RID; NodoAst *nq = new NodoAst(yylineno,columna,t1,$1,"id");
            $$->add(*nq);
            $$->add(*$3);
            $$->add(*$6);
            $$->add(*$9);
          }
          | id cor_i E cor_d cor_i E cor_d cor_i E cor_d igual E puntocoma{
              Tipo t = ASARR;
              $$ = new NodoAst(yylineno,columna,t,"AsignacionArray","AsignacionArray");
              Tipo t1 = RID; NodoAst *nq = new NodoAst(yylineno,columna,t1,$1,"id");
              $$->add(*nq);
              $$->add(*$3);
              $$->add(*$6);
              $$->add(*$9);
              $$->add(*$12);

        }
;

L_ARRAY : cor_i E cor_d{
          Tipo t = DIM_ARRAY;
          $$ = new NodoAst(yylineno,columna,t,"LArray","LArray");
          $$->add(*$2);
        }
        | cor_i E cor_d cor_i E cor_d {
          Tipo t = DIM_ARRAY;
          $$ = new NodoAst(yylineno,columna,t,"LArray","LArray");
          $$->add(*$2);
          $$->add(*$5);
        }
        | cor_i E cor_d cor_i E cor_d cor_i E cor_d{
          Tipo t = DIM_ARRAY;
          $$ = new NodoAst(yylineno,columna,t,"LArray","LArray");
          $$->add(*$2);
          $$->add(*$5);
          $$->add(*$8);
        }
;

L_PROF : L_PROF coma llave_i L_FILA llave_d{
          $$ = $1;
          $$->add(*$4);
        }
        | llave_i L_FILA llave_d{
          Tipo t = LISTPROF;
          $$ = new NodoAst(yylineno,columna,t,"LProf","LProf");
          $$->add(*$2);
        }
;

L_FILA : L_FILA coma llave_i L_COLUMNA llave_d {
          $$ = $1;
          $$->add(*$4);
        }
        | llave_i L_COLUMNA llave_d{
          Tipo t = LISTROW;
          $$ = new NodoAst(yylineno,columna,t,"LRow","LRow");
          $$->add(*$2);
        }
;

L_COLUMNA : L_COLUMNA coma E {
          $$ = $1;
          $$->add(*$3);
        }
        | E {
          Tipo t = LISTCOLUMN;
          $$ = new NodoAst(yylineno,columna,t,"LCol","LCol");
          $$->add(*$1);
        }
;

B_SHOW : r_show par_i E par_d puntocoma{
          Tipo t = RSHOW;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"show","show");
          nuevo->add(*$3);
          $$ = nuevo;
        }
        | r_show par_i E coma E par_d puntocoma{
          Tipo t = RSHOW;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"show","show");
          nuevo->add(*$3);
          nuevo->add(*$5);
          $$ = nuevo;
        }
;

B_IMPRIMIR : r_imprimir par_i E par_d puntocoma{
          Tipo t = RIMPRIMIR;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"imprimir","imprimir");
          nuevo->add(*$3);
          $$ = nuevo;
        }
;

B_REPETIR : r_repetir par_i E par_d llave_i L llave_d {
          Tipo t = RREPETIR;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"repetir","repetir");
          nuevo->add(*$3);
          nuevo->add(*$6);
          $$ = nuevo;
        }
;

B_PARA : r_para par_i IN_PARA par_d llave_i L llave_d {
          Tipo t = RPARA;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"para","para");
          nuevo->add(*$3);
          nuevo->add(*$6);
          $$ = nuevo;
        }
;

IN_PARA : r_int id igual E puntocoma E puntocoma id TIPO_UN{
          Tipo t = INPARA;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"inpara","inpara");
          $$ = nuevo;

          Tipo t1 = VID; NodoAst *n1 = new NodoAst(yylineno,columna,t1,$2,"id");
          NodoAst *n2 = new NodoAst(yylineno,columna,t1,$8,"id");

          $$->add(*n1); $$->add(*$4);
          $$->add(*$6);
          $$->add(*n2); $$->add(*$9);

        }
        |id igual E puntocoma E puntocoma id TIPO_UN{
          Tipo t = INPARA2;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"inpara2","inpara2");
          $$ = nuevo;

          Tipo t1 = VID; NodoAst *n1 = new NodoAst(yylineno,columna,t1,$1,"id");
          NodoAst *n2 = new NodoAst(yylineno,columna,t1,$7,"id");

          $$->add(*n1); $$->add(*$3);
          $$->add(*$5);
          $$->add(*n2); $$->add(*$8);
        }
;


TIPO_UN : aumento{
          Tipo t = AUMENTO; $$ = new NodoAst(yylineno,columna,t,$1,"++");
        }
        | decremento{
          Tipo t = DECREMENTO; $$ = new NodoAst(yylineno,columna,t,$1,"--");
        }
;

L_ID : L_ID coma id{
          $$ = $1;
          Tipo t = RID; NodoAst *nuevo = new NodoAst(yylineno,columna,t,$3,"id");
          $$->add(*nuevo);
        }
        |id{
          Tipo t = LISTID; $$ = new NodoAst(yylineno,columna,t,"listd","listd");
          Tipo t1 = RID; NodoAst *nuevo = new NodoAst(yylineno,columna,t1,$1,"id");
          $$->add(*nuevo);
        }
;

B_IF : r_si par_i E par_d llave_i L llave_d{
          Tipo t = LISTIF;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"listaif","listaif");
          Tipo t1 = RSINO; NodoAst *nuevo1 = new NodoAst(yylineno,columna,t1,"si","si");
          nuevo1->add(*$3); nuevo1->add(*$6);
          nuevo->add(*nuevo1);
          $$ = nuevo;
        }
        |r_si par_i E par_d llave_i L llave_d B_ELSE{
          Tipo t = LISTIF;
          NodoAst *nuevo = new NodoAst(yylineno,yylineno,t,"listaif","listaif");
          Tipo t1 = RSINO; NodoAst *nuevo1 = new NodoAst(yylineno,columna,t1,"si","si");
          nuevo1->add(*$3); nuevo1->add(*$6);
          nuevo->add(*nuevo1); nuevo->add(*$8);
          $$ = nuevo;
        }
        |r_si par_i E par_d llave_i L llave_d L_SINOSI{
          Tipo t = LISTIF;
          NodoAst *nuevo = new NodoAst(yylineno,yylineno,t,"listaif","listaif");
          Tipo t1 = RSINO; NodoAst *nuevo1 = new NodoAst(yylineno,columna,t1,"si","si");
          nuevo1->add(*$3); nuevo1->add(*$6);
          nuevo->add(*nuevo1); nuevo->add(*$8);
          $$ = nuevo;
        }
        |r_si par_i E par_d llave_i L llave_d L_SINOSI B_ELSE{
          Tipo t = LISTIF;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"listaif","listaif");
          Tipo t1 = RSINO; NodoAst *nuevo1 = new NodoAst(yylineno,columna,t1,"si","si");
          nuevo1->add(*$3); nuevo1->add(*$6);
          nuevo->add(*nuevo1); nuevo->add(*$8); nuevo->add(*$9);
          $$ = nuevo;
        }
;

B_ELSE : r_sino llave_i L llave_d{
          Tipo t = RSINO;
          $$ = new NodoAst(yylineno,columna,t,"sino","sino");
          $$->add(*$3);
        }
;

L_SINOSI : L_SINOSI r_sino r_si par_i E par_d llave_i L llave_d{
          $$ = $1;
          Tipo t = RSINOSI; NodoAst *nuevo = new NodoAst(yylineno,columna,t,"sinosi","sinosi");
          nuevo->add(*$5); nuevo->add(*$8);
          $$->add(*nuevo);
        }
        | r_sino r_si par_i E par_d llave_i L llave_d{
          Tipo t = LISTEIF;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"listaelseif","listaelseif");
          Tipo t1 = RSINOSI; NodoAst *nuevo1 = new NodoAst(yylineno,columna,t1,"sinosi","sinosi");
          nuevo1->add(*$4); nuevo1->add(*$7);
          nuevo->add(*nuevo1);
          $$ = nuevo;
        }
;

E:       E aand E { Tipo t = AND; NodoAst *nodo = new NodoAst(yylineno,columna,t,$2,"and"); nodo->add(*$1); nodo->add(*$3); $$=nodo;}
        |E oor E {Tipo t = OR; NodoAst *nodo = new NodoAst(yylineno,columna,t,$2,"or"); nodo->add(*$1); nodo->add(*$3); $$=nodo; }
        |noot E {Tipo t = NOT; NodoAst *nodo = new NodoAst(yylineno,columna,t,$1,"not"); nodo->add(*$2); $$=nodo; }
        |E igual_logico E {Tipo t = IGUALLOGICO; NodoAst *nodo = new NodoAst(yylineno,columna,t,$2,"iguallogico"); nodo->add(*$1); nodo->add(*$3); $$=nodo;}
        |E desigual E {Tipo t = DESIGUAL; NodoAst *nodo = new NodoAst(yylineno,columna,t,$2,"desigual"); nodo->add(*$1); nodo->add(*$3); $$=nodo;}
        |E mayor E {Tipo t = MAYOR; NodoAst *nodo = new NodoAst(yylineno,columna,t,$2,"mayor"); nodo->add(*$1); nodo->add(*$3); $$=nodo;}
        |E mayor_igual E {Tipo t = MAYORQUE; NodoAst *nodo = new NodoAst(yylineno,columna,t,$2,"mayorigual"); nodo->add(*$1); nodo->add(*$3); $$=nodo;}
        |E menor E {Tipo t = MENOR; NodoAst *nodo = new NodoAst(yylineno,columna,t,$2,"menor"); nodo->add(*$1); nodo->add(*$3); $$=nodo;}
        |E menor_igual E {Tipo t = MENORQUE; NodoAst *nodo = new NodoAst(yylineno,columna,t,$2,"menorigual"); nodo->add(*$1); nodo->add(*$3); $$=nodo;}
        |E mas E { Tipo t = MAS; NodoAst *nodo = new NodoAst(yylineno,columna,t,$2,"suma"); nodo->add(*$1); nodo->add(*$3); $$=nodo;}
        |E menos E {Tipo t = MENOS; NodoAst *nodo = new NodoAst(yylineno,columna,t,$2,"resta"); nodo->add(*$1); nodo->add(*$3); $$=nodo; }
        |E por E {Tipo t = POR; NodoAst *nodo = new NodoAst(yylineno,columna,t,$2,"multiplicacion"); nodo->add(*$1); nodo->add(*$3); $$=nodo; }
        |E entre E{Tipo t = DIV; NodoAst *nodo = new NodoAst(yylineno,columna,t,$2,"division"); nodo->add(*$1); nodo->add(*$3); $$=nodo; }
        |E potencia E{Tipo t = POTENCIA; NodoAst *nodo = new NodoAst(yylineno,columna,t,$2,"potencia"); nodo->add(*$1); nodo->add(*$3); $$=nodo; }
        |entero TIPO_UN {Tipo t = NUMERO; NodoAst *nodo = new NodoAst(yylineno,columna,t,$1,"entero"); nodo->add(*$2); $$=nodo; }
        |caracter TIPO_UN {Tipo t = CARACTER; NodoAst *nodo = new NodoAst(yylineno,columna,t,$1,"caracter"); nodo->add(*$2); $$=nodo; }
        |decimal TIPO_UN {Tipo t = DECIMAL; NodoAst *nodo = new NodoAst(yylineno,columna,t,$1,"decimal"); nodo->add(*$2); $$=nodo; }
        |id TIPO_UN{Tipo t = RID; NodoAst *nodo = new NodoAst(yylineno,columna,t,$1,"id"); nodo->add(*$2); $$=nodo; }
        |menos E {Tipo t = MENOS; NodoAst *nodo = new NodoAst(yylineno,columna,t,$1,"menos"); nodo->add(*$2); $$=nodo; }
        |entero {Tipo t = NUMERO; $$ = new NodoAst(yylineno,columna,t,$1,"entero"); }
        |caracter {Tipo t = CARACTER; $$ = new NodoAst(yylineno,columna,t,$1,"caracter"); }
        |decimal {Tipo t = DECIMAL; $$ = new NodoAst(yylineno,columna,t,$1,"decimal"); }
        |booleano { Tipo t = BOOLEANO; $$ = new NodoAst(yylineno,columna,t,$1,"booleano");}
        |cadena { Tipo t = CADENA; $$ = new NodoAst(yylineno,columna,t,$1,"cadena");}
        |id { Tipo t = VID; $$ = new NodoAst(yylineno,columna,t,$1,"id");}

        |id cor_i E cor_d {Tipo t = DATOMATRIZ; $$ = new NodoAst(yylineno,columna,t,"datoarray","datoarray");
          Tipo t1 = VID; NodoAst *nuevo = new NodoAst(yylineno,columna,t1,$1,"id");
          Tipo t2 = POSARR; NodoAst *nuevo2 = new NodoAst(yylineno,columna,t2,"posarr","posarr"); nuevo2->add(*$3);
          $$->add(*nuevo); $$->add(*nuevo2);
        }
        |id cor_i E cor_d cor_i E cor_d {Tipo t = DATOMATRIZ; $$ = new NodoAst(yylineno,columna,t,"datoarray","datoarray");
          Tipo t1 = VID; NodoAst *nuevo = new NodoAst(yylineno,columna,t1,$1,"id");
          Tipo t2 = POSARR; NodoAst *nuevo2 = new NodoAst(yylineno,columna,t2,"posarr","posarr"); nuevo2->add(*$3);
          NodoAst *nuevo3 = new NodoAst(yylineno,columna,t2,"posarr","posarr"); nuevo3->add(*$6);
          $$->add(*nuevo); $$->add(*nuevo2); $$->add(*nuevo3);
        }
        |id cor_i E cor_d cor_i E cor_d cor_i E cor_d {
          Tipo t = DATOMATRIZ; $$ = new NodoAst(yylineno,columna,t,"datoarray","datoarray");

          Tipo t1 = VID; NodoAst *nuevo = new NodoAst(yylineno,columna,t1,$1,"id");
          Tipo t2 = POSARR; NodoAst *nuevo2 = new NodoAst(yylineno,columna,t2,"posarr","posarr"); nuevo2->add(*$3);
          NodoAst *nuevo3 = new NodoAst(yylineno,columna,t2,"posarr","posarr"); nuevo3->add(*$6);
          NodoAst *nuevo4 = new NodoAst(yylineno,columna,t2,"posarr","posarr"); nuevo4->add(*$9);
          $$->add(*nuevo); $$->add(*nuevo2); $$->add(*nuevo3); $$->add(*nuevo4);
        }
        |par_i E par_d{$$=$2;}
;
%%
