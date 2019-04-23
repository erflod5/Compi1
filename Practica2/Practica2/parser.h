/* A Bison parser, made by GNU Bison 3.0.4.  */

/* Bison interface for Yacc-like parsers in C

   Copyright (C) 1984, 1989-1990, 2000-2015 Free Software Foundation, Inc.

   This program is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>.  */

/* As a special exception, you may create a larger work that contains
   part or all of the Bison parser skeleton and distribute that work
   under terms of your choice, so long as that work isn't itself a
   parser generator using the skeleton or a modified version thereof
   as a parser skeleton.  Alternatively, if you modify or redistribute
   the parser skeleton itself, you may (at your option) remove this
   special exception, which will cause the skeleton and the resulting
   Bison output files to be licensed under the GNU General Public
   License without this special exception.

   This special exception was added by the Free Software Foundation in
   version 2.2 of Bison.  */

#ifndef YY_YY_PARSER_H_INCLUDED
# define YY_YY_PARSER_H_INCLUDED
/* Debug traces.  */
#ifndef YYDEBUG
# define YYDEBUG 0
#endif
#if YYDEBUG
extern int yydebug;
#endif

/* Token type.  */
#ifndef YYTOKENTYPE
# define YYTOKENTYPE
  enum yytokentype
  {
    r_int = 258,
    r_string = 259,
    r_double = 260,
    r_char = 261,
    r_bool = 262,
    r_arreglo = 263,
    r_imprimir = 264,
    r_show = 265,
    r_si = 266,
    r_sino = 267,
    r_para = 268,
    r_repetir = 269,
    entero = 270,
    decimal = 271,
    caracter = 272,
    booleano = 273,
    cadena = 274,
    igual_logico = 275,
    desigual = 276,
    mayor = 277,
    mayor_igual = 278,
    menor = 279,
    menor_igual = 280,
    oor = 281,
    aand = 282,
    noot = 283,
    aumento = 284,
    decremento = 285,
    mas = 286,
    menos = 287,
    por = 288,
    entre = 289,
    potencia = 290,
    par_i = 291,
    par_d = 292,
    puntocoma = 293,
    coma = 294,
    igual = 295,
    cor_i = 296,
    cor_d = 297,
    llave_i = 298,
    llave_d = 299,
    id = 300
  };
#endif

/* Value type.  */
#if ! defined YYSTYPE && ! defined YYSTYPE_IS_DECLARED

union YYSTYPE
{
#line 25 "parser.y" /* yacc.c:1909  */

//se especifican los tipo de valores para los no terminales y lo terminales
char TEXT [256];
class NodoAst *nodo;

#line 106 "parser.h" /* yacc.c:1909  */
};

typedef union YYSTYPE YYSTYPE;
# define YYSTYPE_IS_TRIVIAL 1
# define YYSTYPE_IS_DECLARED 1
#endif

/* Location type.  */
#if ! defined YYLTYPE && ! defined YYLTYPE_IS_DECLARED
typedef struct YYLTYPE YYLTYPE;
struct YYLTYPE
{
  int first_line;
  int first_column;
  int last_line;
  int last_column;
};
# define YYLTYPE_IS_DECLARED 1
# define YYLTYPE_IS_TRIVIAL 1
#endif


extern YYSTYPE yylval;
extern YYLTYPE yylloc;
int yyparse (void);

#endif /* !YY_YY_PARSER_H_INCLUDED  */
