/* A Bison parser, made by GNU Bison 3.0.4.  */

/* Bison implementation for Yacc-like parsers in C

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

/* C LALR(1) parser skeleton written by Richard Stallman, by
   simplifying the original so-called "semantic" parser.  */

/* All symbols defined below should begin with yy or YY, to avoid
   infringing on user name space.  This should be done even for local
   variables, as they might otherwise be expanded by user macros.
   There are some unavoidable exceptions within include files to
   define necessary library symbols; they are noted "INFRINGES ON
   USER NAME SPACE" below.  */

/* Identify Bison output.  */
#define YYBISON 1

/* Bison version.  */
#define YYBISON_VERSION "3.0.4"

/* Skeleton name.  */
#define YYSKELETON_NAME "yacc.c"

/* Pure parsers.  */
#define YYPURE 0

/* Push parsers.  */
#define YYPUSH 0

/* Pull parsers.  */
#define YYPULL 1




/* Copy the first part of user declarations.  */
#line 1 "parser.y" /* yacc.c:339  */

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

#line 86 "parser.cpp" /* yacc.c:339  */

# ifndef YY_NULLPTR
#  if defined __cplusplus && 201103L <= __cplusplus
#   define YY_NULLPTR nullptr
#  else
#   define YY_NULLPTR 0
#  endif
# endif

/* Enabling verbose error messages.  */
#ifdef YYERROR_VERBOSE
# undef YYERROR_VERBOSE
# define YYERROR_VERBOSE 1
#else
# define YYERROR_VERBOSE 1
#endif

/* In a future release of Bison, this section will be replaced
   by #include "parser.h".  */
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
#line 25 "parser.y" /* yacc.c:355  */

//se especifican los tipo de valores para los no terminales y lo terminales
char TEXT [256];
class NodoAst *nodo;

#line 178 "parser.cpp" /* yacc.c:355  */
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

/* Copy the second part of user declarations.  */

#line 209 "parser.cpp" /* yacc.c:358  */

#ifdef short
# undef short
#endif

#ifdef YYTYPE_UINT8
typedef YYTYPE_UINT8 yytype_uint8;
#else
typedef unsigned char yytype_uint8;
#endif

#ifdef YYTYPE_INT8
typedef YYTYPE_INT8 yytype_int8;
#else
typedef signed char yytype_int8;
#endif

#ifdef YYTYPE_UINT16
typedef YYTYPE_UINT16 yytype_uint16;
#else
typedef unsigned short int yytype_uint16;
#endif

#ifdef YYTYPE_INT16
typedef YYTYPE_INT16 yytype_int16;
#else
typedef short int yytype_int16;
#endif

#ifndef YYSIZE_T
# ifdef __SIZE_TYPE__
#  define YYSIZE_T __SIZE_TYPE__
# elif defined size_t
#  define YYSIZE_T size_t
# elif ! defined YYSIZE_T
#  include <stddef.h> /* INFRINGES ON USER NAME SPACE */
#  define YYSIZE_T size_t
# else
#  define YYSIZE_T unsigned int
# endif
#endif

#define YYSIZE_MAXIMUM ((YYSIZE_T) -1)

#ifndef YY_
# if defined YYENABLE_NLS && YYENABLE_NLS
#  if ENABLE_NLS
#   include <libintl.h> /* INFRINGES ON USER NAME SPACE */
#   define YY_(Msgid) dgettext ("bison-runtime", Msgid)
#  endif
# endif
# ifndef YY_
#  define YY_(Msgid) Msgid
# endif
#endif

#ifndef YY_ATTRIBUTE
# if (defined __GNUC__                                               \
      && (2 < __GNUC__ || (__GNUC__ == 2 && 96 <= __GNUC_MINOR__)))  \
     || defined __SUNPRO_C && 0x5110 <= __SUNPRO_C
#  define YY_ATTRIBUTE(Spec) __attribute__(Spec)
# else
#  define YY_ATTRIBUTE(Spec) /* empty */
# endif
#endif

#ifndef YY_ATTRIBUTE_PURE
# define YY_ATTRIBUTE_PURE   YY_ATTRIBUTE ((__pure__))
#endif

#ifndef YY_ATTRIBUTE_UNUSED
# define YY_ATTRIBUTE_UNUSED YY_ATTRIBUTE ((__unused__))
#endif

#if !defined _Noreturn \
     && (!defined __STDC_VERSION__ || __STDC_VERSION__ < 201112)
# if defined _MSC_VER && 1200 <= _MSC_VER
#  define _Noreturn __declspec (noreturn)
# else
#  define _Noreturn YY_ATTRIBUTE ((__noreturn__))
# endif
#endif

/* Suppress unused-variable warnings by "using" E.  */
#if ! defined lint || defined __GNUC__
# define YYUSE(E) ((void) (E))
#else
# define YYUSE(E) /* empty */
#endif

#if defined __GNUC__ && 407 <= __GNUC__ * 100 + __GNUC_MINOR__
/* Suppress an incorrect diagnostic about yylval being uninitialized.  */
# define YY_IGNORE_MAYBE_UNINITIALIZED_BEGIN \
    _Pragma ("GCC diagnostic push") \
    _Pragma ("GCC diagnostic ignored \"-Wuninitialized\"")\
    _Pragma ("GCC diagnostic ignored \"-Wmaybe-uninitialized\"")
# define YY_IGNORE_MAYBE_UNINITIALIZED_END \
    _Pragma ("GCC diagnostic pop")
#else
# define YY_INITIAL_VALUE(Value) Value
#endif
#ifndef YY_IGNORE_MAYBE_UNINITIALIZED_BEGIN
# define YY_IGNORE_MAYBE_UNINITIALIZED_BEGIN
# define YY_IGNORE_MAYBE_UNINITIALIZED_END
#endif
#ifndef YY_INITIAL_VALUE
# define YY_INITIAL_VALUE(Value) /* Nothing. */
#endif


#if ! defined yyoverflow || YYERROR_VERBOSE

/* The parser invokes alloca or malloc; define the necessary symbols.  */

# ifdef YYSTACK_USE_ALLOCA
#  if YYSTACK_USE_ALLOCA
#   ifdef __GNUC__
#    define YYSTACK_ALLOC __builtin_alloca
#   elif defined __BUILTIN_VA_ARG_INCR
#    include <alloca.h> /* INFRINGES ON USER NAME SPACE */
#   elif defined _AIX
#    define YYSTACK_ALLOC __alloca
#   elif defined _MSC_VER
#    include <malloc.h> /* INFRINGES ON USER NAME SPACE */
#    define alloca _alloca
#   else
#    define YYSTACK_ALLOC alloca
#    if ! defined _ALLOCA_H && ! defined EXIT_SUCCESS
#     include <stdlib.h> /* INFRINGES ON USER NAME SPACE */
      /* Use EXIT_SUCCESS as a witness for stdlib.h.  */
#     ifndef EXIT_SUCCESS
#      define EXIT_SUCCESS 0
#     endif
#    endif
#   endif
#  endif
# endif

# ifdef YYSTACK_ALLOC
   /* Pacify GCC's 'empty if-body' warning.  */
#  define YYSTACK_FREE(Ptr) do { /* empty */; } while (0)
#  ifndef YYSTACK_ALLOC_MAXIMUM
    /* The OS might guarantee only one guard page at the bottom of the stack,
       and a page size can be as small as 4096 bytes.  So we cannot safely
       invoke alloca (N) if N exceeds 4096.  Use a slightly smaller number
       to allow for a few compiler-allocated temporary stack slots.  */
#   define YYSTACK_ALLOC_MAXIMUM 4032 /* reasonable circa 2006 */
#  endif
# else
#  define YYSTACK_ALLOC YYMALLOC
#  define YYSTACK_FREE YYFREE
#  ifndef YYSTACK_ALLOC_MAXIMUM
#   define YYSTACK_ALLOC_MAXIMUM YYSIZE_MAXIMUM
#  endif
#  if (defined __cplusplus && ! defined EXIT_SUCCESS \
       && ! ((defined YYMALLOC || defined malloc) \
             && (defined YYFREE || defined free)))
#   include <stdlib.h> /* INFRINGES ON USER NAME SPACE */
#   ifndef EXIT_SUCCESS
#    define EXIT_SUCCESS 0
#   endif
#  endif
#  ifndef YYMALLOC
#   define YYMALLOC malloc
#   if ! defined malloc && ! defined EXIT_SUCCESS
void *malloc (YYSIZE_T); /* INFRINGES ON USER NAME SPACE */
#   endif
#  endif
#  ifndef YYFREE
#   define YYFREE free
#   if ! defined free && ! defined EXIT_SUCCESS
void free (void *); /* INFRINGES ON USER NAME SPACE */
#   endif
#  endif
# endif
#endif /* ! defined yyoverflow || YYERROR_VERBOSE */


#if (! defined yyoverflow \
     && (! defined __cplusplus \
         || (defined YYLTYPE_IS_TRIVIAL && YYLTYPE_IS_TRIVIAL \
             && defined YYSTYPE_IS_TRIVIAL && YYSTYPE_IS_TRIVIAL)))

/* A type that is properly aligned for any stack member.  */
union yyalloc
{
  yytype_int16 yyss_alloc;
  YYSTYPE yyvs_alloc;
  YYLTYPE yyls_alloc;
};

/* The size of the maximum gap between one aligned stack and the next.  */
# define YYSTACK_GAP_MAXIMUM (sizeof (union yyalloc) - 1)

/* The size of an array large to enough to hold all stacks, each with
   N elements.  */
# define YYSTACK_BYTES(N) \
     ((N) * (sizeof (yytype_int16) + sizeof (YYSTYPE) + sizeof (YYLTYPE)) \
      + 2 * YYSTACK_GAP_MAXIMUM)

# define YYCOPY_NEEDED 1

/* Relocate STACK from its old location to the new one.  The
   local variables YYSIZE and YYSTACKSIZE give the old and new number of
   elements in the stack, and YYPTR gives the new location of the
   stack.  Advance YYPTR to a properly aligned location for the next
   stack.  */
# define YYSTACK_RELOCATE(Stack_alloc, Stack)                           \
    do                                                                  \
      {                                                                 \
        YYSIZE_T yynewbytes;                                            \
        YYCOPY (&yyptr->Stack_alloc, Stack, yysize);                    \
        Stack = &yyptr->Stack_alloc;                                    \
        yynewbytes = yystacksize * sizeof (*Stack) + YYSTACK_GAP_MAXIMUM; \
        yyptr += yynewbytes / sizeof (*yyptr);                          \
      }                                                                 \
    while (0)

#endif

#if defined YYCOPY_NEEDED && YYCOPY_NEEDED
/* Copy COUNT objects from SRC to DST.  The source and destination do
   not overlap.  */
# ifndef YYCOPY
#  if defined __GNUC__ && 1 < __GNUC__
#   define YYCOPY(Dst, Src, Count) \
      __builtin_memcpy (Dst, Src, (Count) * sizeof (*(Src)))
#  else
#   define YYCOPY(Dst, Src, Count)              \
      do                                        \
        {                                       \
          YYSIZE_T yyi;                         \
          for (yyi = 0; yyi < (Count); yyi++)   \
            (Dst)[yyi] = (Src)[yyi];            \
        }                                       \
      while (0)
#  endif
# endif
#endif /* !YYCOPY_NEEDED */

/* YYFINAL -- State number of the termination state.  */
#define YYFINAL  34
/* YYLAST -- Last index in YYTABLE.  */
#define YYLAST   872

/* YYNTOKENS -- Number of terminals.  */
#define YYNTOKENS  46
/* YYNNTS -- Number of nonterminals.  */
#define YYNNTS  23
/* YYNRULES -- Number of rules.  */
#define YYNRULES  84
/* YYNSTATES -- Number of states.  */
#define YYNSTATES  221

/* YYTRANSLATE[YYX] -- Symbol number corresponding to YYX as returned
   by yylex, with out-of-bounds checking.  */
#define YYUNDEFTOK  2
#define YYMAXUTOK   300

#define YYTRANSLATE(YYX)                                                \
  ((unsigned int) (YYX) <= YYMAXUTOK ? yytranslate[YYX] : YYUNDEFTOK)

/* YYTRANSLATE[TOKEN-NUM] -- Symbol number corresponding to TOKEN-NUM
   as returned by yylex, without out-of-bounds checking.  */
static const yytype_uint8 yytranslate[] =
{
       0,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     1,     2,     3,     4,
       5,     6,     7,     8,     9,    10,    11,    12,    13,    14,
      15,    16,    17,    18,    19,    20,    21,    22,    23,    24,
      25,    26,    27,    28,    29,    30,    31,    32,    33,    34,
      35,    36,    37,    38,    39,    40,    41,    42,    43,    44,
      45
};

#if YYDEBUG
  /* YYRLINE[YYN] -- Source line where rule number YYN was defined.  */
static const yytype_uint16 yyrline[] =
{
       0,    63,    63,    65,    69,    77,    80,    83,    86,    89,
      92,    97,   100,   105,   111,   117,   123,   129,   135,   144,
     148,   152,   156,   160,   166,   173,   180,   189,   198,   211,
     216,   222,   231,   235,   242,   246,   253,   257,   264,   270,
     279,   287,   296,   305,   318,   333,   336,   341,   346,   353,
     361,   369,   377,   387,   394,   400,   410,   411,   412,   413,
     414,   415,   416,   417,   418,   419,   420,   421,   422,   423,
     424,   425,   426,   427,   428,   429,   430,   431,   432,   433,
     434,   436,   441,   447,   456
};
#endif

#if YYDEBUG || YYERROR_VERBOSE || 1
/* YYTNAME[SYMBOL-NUM] -- String name of the symbol SYMBOL-NUM.
   First, the terminals, then, starting at YYNTOKENS, nonterminals.  */
static const char *const yytname[] =
{
  "$end", "error", "$undefined", "r_int", "r_string", "r_double",
  "r_char", "r_bool", "r_arreglo", "r_imprimir", "r_show", "r_si",
  "r_sino", "r_para", "r_repetir", "entero", "decimal", "caracter",
  "booleano", "cadena", "igual_logico", "desigual", "mayor", "mayor_igual",
  "menor", "menor_igual", "oor", "aand", "noot", "aumento", "decremento",
  "mas", "menos", "por", "entre", "potencia", "par_i", "par_d",
  "puntocoma", "coma", "igual", "cor_i", "cor_d", "llave_i", "llave_d",
  "id", "$accept", "INICIO", "L", "BLOCK", "B_VAR", "DECLARA_VAR",
  "TIPO_VAR", "ASIGNA_VAR", "L_ARRAY", "L_PROF", "L_FILA", "L_COLUMNA",
  "B_SHOW", "B_IMPRIMIR", "B_REPETIR", "B_PARA", "IN_PARA", "TIPO_UN",
  "L_ID", "B_IF", "B_ELSE", "L_SINOSI", "E", YY_NULLPTR
};
#endif

# ifdef YYPRINT
/* YYTOKNUM[NUM] -- (External) token number corresponding to the
   (internal) symbol number NUM (which must be that of a token).  */
static const yytype_uint16 yytoknum[] =
{
       0,   256,   257,   258,   259,   260,   261,   262,   263,   264,
     265,   266,   267,   268,   269,   270,   271,   272,   273,   274,
     275,   276,   277,   278,   279,   280,   281,   282,   283,   284,
     285,   286,   287,   288,   289,   290,   291,   292,   293,   294,
     295,   296,   297,   298,   299,   300
};
# endif

#define YYPACT_NINF -141

#define yypact_value_is_default(Yystate) \
  (!!((Yystate) == (-141)))

#define YYTABLE_NINF -1

#define yytable_value_is_error(Yytable_value) \
  0

  /* YYPACT[STATE-NUM] -- Index in YYTABLE of the portion describing
     STATE-NUM.  */
static const yytype_int16 yypact[] =
{
     231,  -141,  -141,  -141,  -141,  -141,   -30,   -19,   -11,    -7,
      50,   -10,    69,   231,  -141,  -141,  -141,    18,  -141,  -141,
    -141,  -141,  -141,  -141,   271,   271,   271,     1,   271,  -141,
    -141,   271,   271,    57,  -141,  -141,    66,  -141,   -24,    91,
      91,    91,  -141,  -141,   271,   271,   271,    37,   695,   504,
     713,    71,    60,   100,   731,   524,   297,  -141,    55,  -141,
     101,   271,  -141,  -141,  -141,   837,    56,   749,   271,  -141,
     271,   271,   271,   271,   271,   271,   271,   271,   271,   271,
     271,   271,   271,   115,   119,   271,   121,   122,   271,   129,
     131,  -141,    84,   271,    61,  -141,   543,  -141,   320,   136,
     136,   136,   136,   136,   136,   821,   837,    56,    56,   140,
     140,  -141,  -141,  -141,   767,   231,   271,   562,   231,   231,
     271,   271,   343,  -141,   133,  -141,   137,   142,    30,   581,
     271,    99,   145,   600,   366,   141,   235,   271,  -141,   171,
     271,   619,  -141,  -141,  -141,   120,   271,   266,    29,    32,
      33,   107,   389,    21,  -141,   172,   638,   143,   271,   271,
     412,   271,    40,    41,   144,   147,   153,   148,   271,   162,
     163,   169,   231,    49,  -141,   167,    91,   657,   435,   175,
    -141,  -141,   181,  -141,   271,  -141,   107,  -141,   271,   271,
     188,   192,    91,  -141,  -141,   191,   271,    43,    44,   458,
     785,  -141,   271,  -141,   271,   481,  -141,  -141,  -141,   196,
     803,   676,  -141,   231,   200,  -141,   204,   231,  -141,   216,
    -141
};

  /* YYDEFACT[STATE-NUM] -- Default reduction number in state STATE-NUM.
     Performed when YYTABLE does not specify something else to do.  Zero
     means the default is an error.  */
static const yytype_uint8 yydefact[] =
{
       0,    19,    20,    21,    22,    23,     0,     0,     0,     0,
       0,     0,     0,     2,     4,     5,    11,     0,    12,    10,
       9,     8,     7,     6,     0,     0,     0,     0,     0,    45,
      46,     0,     0,     0,     1,     3,     0,    48,     0,    75,
      77,    76,    78,    79,     0,     0,     0,    80,     0,     0,
       0,     0,     0,     0,     0,     0,     0,    25,     0,    13,
       0,     0,    70,    72,    71,    58,    74,     0,     0,    73,
       0,     0,     0,     0,     0,     0,     0,     0,     0,     0,
       0,     0,     0,     0,     0,     0,     0,     0,     0,     0,
       0,    24,     0,     0,     0,    47,     0,    84,     0,    59,
      60,    61,    62,    63,    64,    57,    56,    65,    66,    67,
      68,    69,    40,    38,     0,     0,     0,     0,     0,     0,
       0,     0,     0,    15,     0,    14,    81,     0,     0,     0,
       0,     0,     0,     0,     0,    29,     0,     0,    39,    49,
       0,     0,    42,    41,    26,     0,     0,     0,     0,     0,
       0,    37,     0,     0,    50,    51,     0,     0,     0,     0,
       0,     0,     0,     0,     0,     0,     0,     0,     0,     0,
      82,     0,     0,     0,    52,     0,     0,     0,     0,    30,
      33,    35,     0,    18,     0,    16,    36,    17,     0,     0,
       0,     0,     0,    44,    27,     0,     0,     0,     0,     0,
       0,    53,     0,    43,     0,     0,    32,    34,    83,     0,
       0,     0,    31,     0,     0,    28,     0,     0,    55,     0,
      54
};

  /* YYPGOTO[NTERM-NUM].  */
static const yytype_int16 yypgoto[] =
{
    -141,  -141,   -91,   -13,  -141,  -141,  -141,  -141,  -141,  -141,
    -140,  -123,  -141,  -141,  -141,  -141,  -141,   -29,   210,  -141,
      92,  -141,   -23
};

  /* YYDEFGOTO[NTERM-NUM].  */
static const yytype_int16 yydefgoto[] =
{
      -1,    12,    13,    14,    15,    16,    17,    18,    94,   148,
     149,   163,    19,    20,    21,    22,    53,    33,    38,    23,
     154,   155,   151
};

  /* YYTABLE[YYPACT[STATE-NUM]] -- What to do in state STATE-NUM.  If
     positive, shift that token.  If negative, reduce the rule whose
     number is the opposite.  If YYTABLE_NINF, syntax error.  */
static const yytype_uint8 yytable[] =
{
      35,    48,    49,    50,    51,    54,    24,   162,    55,    56,
      62,    63,    64,   150,    59,    60,    61,    25,    69,    29,
      30,    65,    66,    67,   128,    26,    36,   131,   132,    27,
      31,    32,   171,     1,     2,     3,     4,     5,    96,     6,
       7,     8,   197,     9,    10,    98,    52,    99,   100,   101,
     102,   103,   104,   105,   106,   107,   108,   109,   110,   111,
     191,   198,   114,    37,   172,   117,    29,    30,   164,    34,
     122,   166,   168,   165,   139,    11,   167,   169,    68,   166,
     168,   190,   166,   168,   180,   181,    28,   206,   207,    80,
      81,    82,   172,   129,    60,    57,    93,   133,   134,   123,
      88,   124,     1,     2,     3,     4,     5,   141,     6,     7,
       8,    37,     9,    10,   152,    35,    87,   156,    35,    35,
      29,    30,   216,   160,   120,   121,   219,    70,    71,    72,
      73,    74,    75,    76,    77,   177,   178,    89,    78,    79,
      80,    81,    82,   142,    11,   186,    95,   193,     1,     2,
       3,     4,     5,   112,     6,     7,     8,   113,     9,    10,
     158,   159,   116,   203,   115,   199,   200,    78,    79,    80,
      81,    82,   118,   205,   119,    82,   136,    35,   137,   210,
     138,   211,   146,   153,   173,   183,   185,   182,   176,   143,
      11,     1,     2,     3,     4,     5,   184,     6,     7,     8,
     187,     9,    10,    35,   188,   189,    35,     1,     2,     3,
       4,     5,   192,     6,     7,     8,   196,     9,    10,     1,
       2,     3,     4,     5,   161,     6,     7,     8,   202,     9,
      10,   204,   201,    11,     1,     2,     3,     4,     5,   213,
       6,     7,     8,   217,     9,    10,    58,   174,   218,    11,
      39,    40,    41,    42,    43,     0,     0,     0,     0,     0,
     220,    11,     0,    44,     0,     0,     0,    45,     0,     0,
       0,    46,     0,     0,     0,     0,    11,     0,   147,     0,
      47,    39,    40,    41,    42,    43,    39,    40,    41,    42,
      43,     0,     0,     0,    44,     0,     0,     0,    45,    44,
       0,     0,    46,    45,     0,     0,     0,    46,     0,   161,
       0,    47,     0,     0,     0,     0,    47,    70,    71,    72,
      73,    74,    75,    76,    77,     0,     0,     0,    78,    79,
      80,    81,    82,     0,     0,     0,     0,     0,     0,    92,
      70,    71,    72,    73,    74,    75,    76,    77,     0,     0,
       0,    78,    79,    80,    81,    82,     0,     0,     0,     0,
       0,     0,   126,    70,    71,    72,    73,    74,    75,    76,
      77,     0,     0,     0,    78,    79,    80,    81,    82,     0,
       0,     0,     0,     0,     0,   135,    70,    71,    72,    73,
      74,    75,    76,    77,     0,     0,     0,    78,    79,    80,
      81,    82,     0,     0,     0,     0,     0,     0,   145,    70,
      71,    72,    73,    74,    75,    76,    77,     0,     0,     0,
      78,    79,    80,    81,    82,     0,     0,     0,     0,     0,
       0,   170,    70,    71,    72,    73,    74,    75,    76,    77,
       0,     0,     0,    78,    79,    80,    81,    82,     0,     0,
       0,     0,     0,     0,   179,    70,    71,    72,    73,    74,
      75,    76,    77,     0,     0,     0,    78,    79,    80,    81,
      82,     0,     0,     0,     0,     0,     0,   195,    70,    71,
      72,    73,    74,    75,    76,    77,     0,     0,     0,    78,
      79,    80,    81,    82,     0,     0,     0,     0,     0,     0,
     208,    70,    71,    72,    73,    74,    75,    76,    77,     0,
       0,     0,    78,    79,    80,    81,    82,     0,     0,     0,
       0,     0,     0,   212,    70,    71,    72,    73,    74,    75,
      76,    77,     0,     0,     0,    78,    79,    80,    81,    82,
       0,    84,     0,    85,    70,    71,    72,    73,    74,    75,
      76,    77,     0,     0,     0,    78,    79,    80,    81,    82,
       0,     0,    91,    70,    71,    72,    73,    74,    75,    76,
      77,     0,     0,     0,    78,    79,    80,    81,    82,     0,
       0,   125,    70,    71,    72,    73,    74,    75,    76,    77,
       0,     0,     0,    78,    79,    80,    81,    82,     0,     0,
     130,    70,    71,    72,    73,    74,    75,    76,    77,     0,
       0,     0,    78,    79,    80,    81,    82,     0,     0,   140,
      70,    71,    72,    73,    74,    75,    76,    77,     0,     0,
       0,    78,    79,    80,    81,    82,     0,     0,   144,    70,
      71,    72,    73,    74,    75,    76,    77,     0,     0,     0,
      78,    79,    80,    81,    82,     0,     0,   157,    70,    71,
      72,    73,    74,    75,    76,    77,     0,     0,     0,    78,
      79,    80,    81,    82,     0,     0,   175,    70,    71,    72,
      73,    74,    75,    76,    77,     0,     0,     0,    78,    79,
      80,    81,    82,     0,     0,   194,    70,    71,    72,    73,
      74,    75,    76,    77,     0,     0,     0,    78,    79,    80,
      81,    82,     0,     0,   215,    70,    71,    72,    73,    74,
      75,    76,    77,     0,     0,     0,    78,    79,    80,    81,
      82,     0,    83,    70,    71,    72,    73,    74,    75,    76,
      77,     0,     0,     0,    78,    79,    80,    81,    82,     0,
      86,    70,    71,    72,    73,    74,    75,    76,    77,     0,
       0,     0,    78,    79,    80,    81,    82,     0,    90,    70,
      71,    72,    73,    74,    75,    76,    77,     0,     0,     0,
      78,    79,    80,    81,    82,     0,    97,    70,    71,    72,
      73,    74,    75,    76,    77,     0,     0,     0,    78,    79,
      80,    81,    82,     0,   127,    70,    71,    72,    73,    74,
      75,    76,    77,     0,     0,     0,    78,    79,    80,    81,
      82,     0,   209,    70,    71,    72,    73,    74,    75,    76,
      77,     0,     0,     0,    78,    79,    80,    81,    82,     0,
     214,    70,    71,    72,    73,    74,    75,     0,    77,     0,
       0,     0,    78,    79,    80,    81,    82,    70,    71,    72,
      73,    74,    75,     0,     0,     0,     0,     0,    78,    79,
      80,    81,    82
};

static const yytype_int16 yycheck[] =
{
      13,    24,    25,    26,     3,    28,    36,   147,    31,    32,
      39,    40,    41,   136,    38,    39,    40,    36,    47,    29,
      30,    44,    45,    46,   115,    36,     8,   118,   119,    36,
      40,    41,    11,     3,     4,     5,     6,     7,    61,     9,
      10,    11,   182,    13,    14,    68,    45,    70,    71,    72,
      73,    74,    75,    76,    77,    78,    79,    80,    81,    82,
      11,   184,    85,    45,    43,    88,    29,    30,    39,     0,
      93,    39,    39,    44,    44,    45,    44,    44,    41,    39,
      39,   172,    39,    39,    44,    44,    36,    44,    44,    33,
      34,    35,    43,   116,    39,    38,    41,   120,   121,    38,
      40,    40,     3,     4,     5,     6,     7,   130,     9,    10,
      11,    45,    13,    14,   137,   128,    45,   140,   131,   132,
      29,    30,   213,   146,    40,    41,   217,    20,    21,    22,
      23,    24,    25,    26,    27,   158,   159,    37,    31,    32,
      33,    34,    35,    44,    45,   168,    45,   176,     3,     4,
       5,     6,     7,    38,     9,    10,    11,    38,    13,    14,
      40,    41,    40,   192,    43,   188,   189,    31,    32,    33,
      34,    35,    43,   196,    43,    35,    43,   190,    41,   202,
      38,   204,    41,    12,    12,    38,    38,    43,    45,    44,
      45,     3,     4,     5,     6,     7,    43,     9,    10,    11,
      38,    13,    14,   216,    41,    36,   219,     3,     4,     5,
       6,     7,    45,     9,    10,    11,    41,    13,    14,     3,
       4,     5,     6,     7,    43,     9,    10,    11,    36,    13,
      14,    40,    44,    45,     3,     4,     5,     6,     7,    43,
       9,    10,    11,    43,    13,    14,    36,   155,    44,    45,
      15,    16,    17,    18,    19,    -1,    -1,    -1,    -1,    -1,
      44,    45,    -1,    28,    -1,    -1,    -1,    32,    -1,    -1,
      -1,    36,    -1,    -1,    -1,    -1,    45,    -1,    43,    -1,
      45,    15,    16,    17,    18,    19,    15,    16,    17,    18,
      19,    -1,    -1,    -1,    28,    -1,    -1,    -1,    32,    28,
      -1,    -1,    36,    32,    -1,    -1,    -1,    36,    -1,    43,
      -1,    45,    -1,    -1,    -1,    -1,    45,    20,    21,    22,
      23,    24,    25,    26,    27,    -1,    -1,    -1,    31,    32,
      33,    34,    35,    -1,    -1,    -1,    -1,    -1,    -1,    42,
      20,    21,    22,    23,    24,    25,    26,    27,    -1,    -1,
      -1,    31,    32,    33,    34,    35,    -1,    -1,    -1,    -1,
      -1,    -1,    42,    20,    21,    22,    23,    24,    25,    26,
      27,    -1,    -1,    -1,    31,    32,    33,    34,    35,    -1,
      -1,    -1,    -1,    -1,    -1,    42,    20,    21,    22,    23,
      24,    25,    26,    27,    -1,    -1,    -1,    31,    32,    33,
      34,    35,    -1,    -1,    -1,    -1,    -1,    -1,    42,    20,
      21,    22,    23,    24,    25,    26,    27,    -1,    -1,    -1,
      31,    32,    33,    34,    35,    -1,    -1,    -1,    -1,    -1,
      -1,    42,    20,    21,    22,    23,    24,    25,    26,    27,
      -1,    -1,    -1,    31,    32,    33,    34,    35,    -1,    -1,
      -1,    -1,    -1,    -1,    42,    20,    21,    22,    23,    24,
      25,    26,    27,    -1,    -1,    -1,    31,    32,    33,    34,
      35,    -1,    -1,    -1,    -1,    -1,    -1,    42,    20,    21,
      22,    23,    24,    25,    26,    27,    -1,    -1,    -1,    31,
      32,    33,    34,    35,    -1,    -1,    -1,    -1,    -1,    -1,
      42,    20,    21,    22,    23,    24,    25,    26,    27,    -1,
      -1,    -1,    31,    32,    33,    34,    35,    -1,    -1,    -1,
      -1,    -1,    -1,    42,    20,    21,    22,    23,    24,    25,
      26,    27,    -1,    -1,    -1,    31,    32,    33,    34,    35,
      -1,    37,    -1,    39,    20,    21,    22,    23,    24,    25,
      26,    27,    -1,    -1,    -1,    31,    32,    33,    34,    35,
      -1,    -1,    38,    20,    21,    22,    23,    24,    25,    26,
      27,    -1,    -1,    -1,    31,    32,    33,    34,    35,    -1,
      -1,    38,    20,    21,    22,    23,    24,    25,    26,    27,
      -1,    -1,    -1,    31,    32,    33,    34,    35,    -1,    -1,
      38,    20,    21,    22,    23,    24,    25,    26,    27,    -1,
      -1,    -1,    31,    32,    33,    34,    35,    -1,    -1,    38,
      20,    21,    22,    23,    24,    25,    26,    27,    -1,    -1,
      -1,    31,    32,    33,    34,    35,    -1,    -1,    38,    20,
      21,    22,    23,    24,    25,    26,    27,    -1,    -1,    -1,
      31,    32,    33,    34,    35,    -1,    -1,    38,    20,    21,
      22,    23,    24,    25,    26,    27,    -1,    -1,    -1,    31,
      32,    33,    34,    35,    -1,    -1,    38,    20,    21,    22,
      23,    24,    25,    26,    27,    -1,    -1,    -1,    31,    32,
      33,    34,    35,    -1,    -1,    38,    20,    21,    22,    23,
      24,    25,    26,    27,    -1,    -1,    -1,    31,    32,    33,
      34,    35,    -1,    -1,    38,    20,    21,    22,    23,    24,
      25,    26,    27,    -1,    -1,    -1,    31,    32,    33,    34,
      35,    -1,    37,    20,    21,    22,    23,    24,    25,    26,
      27,    -1,    -1,    -1,    31,    32,    33,    34,    35,    -1,
      37,    20,    21,    22,    23,    24,    25,    26,    27,    -1,
      -1,    -1,    31,    32,    33,    34,    35,    -1,    37,    20,
      21,    22,    23,    24,    25,    26,    27,    -1,    -1,    -1,
      31,    32,    33,    34,    35,    -1,    37,    20,    21,    22,
      23,    24,    25,    26,    27,    -1,    -1,    -1,    31,    32,
      33,    34,    35,    -1,    37,    20,    21,    22,    23,    24,
      25,    26,    27,    -1,    -1,    -1,    31,    32,    33,    34,
      35,    -1,    37,    20,    21,    22,    23,    24,    25,    26,
      27,    -1,    -1,    -1,    31,    32,    33,    34,    35,    -1,
      37,    20,    21,    22,    23,    24,    25,    -1,    27,    -1,
      -1,    -1,    31,    32,    33,    34,    35,    20,    21,    22,
      23,    24,    25,    -1,    -1,    -1,    -1,    -1,    31,    32,
      33,    34,    35
};

  /* YYSTOS[STATE-NUM] -- The (internal number of the) accessing
     symbol of state STATE-NUM.  */
static const yytype_uint8 yystos[] =
{
       0,     3,     4,     5,     6,     7,     9,    10,    11,    13,
      14,    45,    47,    48,    49,    50,    51,    52,    53,    58,
      59,    60,    61,    65,    36,    36,    36,    36,    36,    29,
      30,    40,    41,    63,     0,    49,     8,    45,    64,    15,
      16,    17,    18,    19,    28,    32,    36,    45,    68,    68,
      68,     3,    45,    62,    68,    68,    68,    38,    64,    38,
      39,    40,    63,    63,    63,    68,    68,    68,    41,    63,
      20,    21,    22,    23,    24,    25,    26,    27,    31,    32,
      33,    34,    35,    37,    37,    39,    37,    45,    40,    37,
      37,    38,    42,    41,    54,    45,    68,    37,    68,    68,
      68,    68,    68,    68,    68,    68,    68,    68,    68,    68,
      68,    68,    38,    38,    68,    43,    40,    68,    43,    43,
      40,    41,    68,    38,    40,    38,    42,    37,    48,    68,
      38,    48,    48,    68,    68,    42,    43,    41,    38,    44,
      38,    68,    44,    44,    38,    42,    41,    43,    55,    56,
      57,    68,    68,    12,    66,    67,    68,    38,    40,    41,
      68,    43,    56,    57,    39,    44,    39,    44,    39,    44,
      42,    11,    43,    12,    66,    38,    45,    68,    68,    42,
      44,    44,    43,    38,    43,    38,    68,    38,    41,    36,
      48,    11,    45,    63,    38,    42,    41,    56,    57,    68,
      68,    44,    36,    63,    40,    68,    44,    44,    42,    37,
      68,    68,    42,    43,    37,    38,    48,    43,    44,    48,
      44
};

  /* YYR1[YYN] -- Symbol number of symbol that rule YYN derives.  */
static const yytype_uint8 yyr1[] =
{
       0,    46,    47,    48,    48,    49,    49,    49,    49,    49,
      49,    50,    50,    51,    51,    51,    51,    51,    51,    52,
      52,    52,    52,    52,    53,    53,    53,    53,    53,    54,
      54,    54,    55,    55,    56,    56,    57,    57,    58,    58,
      59,    60,    61,    62,    62,    63,    63,    64,    64,    65,
      65,    65,    65,    66,    67,    67,    68,    68,    68,    68,
      68,    68,    68,    68,    68,    68,    68,    68,    68,    68,
      68,    68,    68,    68,    68,    68,    68,    68,    68,    68,
      68,    68,    68,    68,    68
};

  /* YYR2[YYN] -- Number of symbols on the right hand side of rule YYN.  */
static const yytype_uint8 yyr2[] =
{
       0,     2,     1,     2,     1,     1,     1,     1,     1,     1,
       1,     1,     1,     3,     5,     5,     9,     9,     9,     1,
       1,     1,     1,     1,     4,     3,     7,    10,    13,     3,
       6,     9,     5,     3,     5,     3,     3,     1,     5,     7,
       5,     7,     7,     9,     8,     1,     1,     3,     1,     7,
       8,     8,     9,     4,     9,     8,     3,     3,     2,     3,
       3,     3,     3,     3,     3,     3,     3,     3,     3,     3,
       2,     2,     2,     2,     2,     1,     1,     1,     1,     1,
       1,     4,     7,    10,     3
};


#define yyerrok         (yyerrstatus = 0)
#define yyclearin       (yychar = YYEMPTY)
#define YYEMPTY         (-2)
#define YYEOF           0

#define YYACCEPT        goto yyacceptlab
#define YYABORT         goto yyabortlab
#define YYERROR         goto yyerrorlab


#define YYRECOVERING()  (!!yyerrstatus)

#define YYBACKUP(Token, Value)                                  \
do                                                              \
  if (yychar == YYEMPTY)                                        \
    {                                                           \
      yychar = (Token);                                         \
      yylval = (Value);                                         \
      YYPOPSTACK (yylen);                                       \
      yystate = *yyssp;                                         \
      goto yybackup;                                            \
    }                                                           \
  else                                                          \
    {                                                           \
      yyerror (YY_("syntax error: cannot back up")); \
      YYERROR;                                                  \
    }                                                           \
while (0)

/* Error token number */
#define YYTERROR        1
#define YYERRCODE       256


/* YYLLOC_DEFAULT -- Set CURRENT to span from RHS[1] to RHS[N].
   If N is 0, then set CURRENT to the empty location which ends
   the previous symbol: RHS[0] (always defined).  */

#ifndef YYLLOC_DEFAULT
# define YYLLOC_DEFAULT(Current, Rhs, N)                                \
    do                                                                  \
      if (N)                                                            \
        {                                                               \
          (Current).first_line   = YYRHSLOC (Rhs, 1).first_line;        \
          (Current).first_column = YYRHSLOC (Rhs, 1).first_column;      \
          (Current).last_line    = YYRHSLOC (Rhs, N).last_line;         \
          (Current).last_column  = YYRHSLOC (Rhs, N).last_column;       \
        }                                                               \
      else                                                              \
        {                                                               \
          (Current).first_line   = (Current).last_line   =              \
            YYRHSLOC (Rhs, 0).last_line;                                \
          (Current).first_column = (Current).last_column =              \
            YYRHSLOC (Rhs, 0).last_column;                              \
        }                                                               \
    while (0)
#endif

#define YYRHSLOC(Rhs, K) ((Rhs)[K])


/* Enable debugging if requested.  */
#if YYDEBUG

# ifndef YYFPRINTF
#  include <stdio.h> /* INFRINGES ON USER NAME SPACE */
#  define YYFPRINTF fprintf
# endif

# define YYDPRINTF(Args)                        \
do {                                            \
  if (yydebug)                                  \
    YYFPRINTF Args;                             \
} while (0)


/* YY_LOCATION_PRINT -- Print the location on the stream.
   This macro was not mandated originally: define only if we know
   we won't break user code: when these are the locations we know.  */

#ifndef YY_LOCATION_PRINT
# if defined YYLTYPE_IS_TRIVIAL && YYLTYPE_IS_TRIVIAL

/* Print *YYLOCP on YYO.  Private, do not rely on its existence. */

YY_ATTRIBUTE_UNUSED
static unsigned
yy_location_print_ (FILE *yyo, YYLTYPE const * const yylocp)
{
  unsigned res = 0;
  int end_col = 0 != yylocp->last_column ? yylocp->last_column - 1 : 0;
  if (0 <= yylocp->first_line)
    {
      res += YYFPRINTF (yyo, "%d", yylocp->first_line);
      if (0 <= yylocp->first_column)
        res += YYFPRINTF (yyo, ".%d", yylocp->first_column);
    }
  if (0 <= yylocp->last_line)
    {
      if (yylocp->first_line < yylocp->last_line)
        {
          res += YYFPRINTF (yyo, "-%d", yylocp->last_line);
          if (0 <= end_col)
            res += YYFPRINTF (yyo, ".%d", end_col);
        }
      else if (0 <= end_col && yylocp->first_column < end_col)
        res += YYFPRINTF (yyo, "-%d", end_col);
    }
  return res;
 }

#  define YY_LOCATION_PRINT(File, Loc)          \
  yy_location_print_ (File, &(Loc))

# else
#  define YY_LOCATION_PRINT(File, Loc) ((void) 0)
# endif
#endif


# define YY_SYMBOL_PRINT(Title, Type, Value, Location)                    \
do {                                                                      \
  if (yydebug)                                                            \
    {                                                                     \
      YYFPRINTF (stderr, "%s ", Title);                                   \
      yy_symbol_print (stderr,                                            \
                  Type, Value, Location); \
      YYFPRINTF (stderr, "\n");                                           \
    }                                                                     \
} while (0)


/*----------------------------------------.
| Print this symbol's value on YYOUTPUT.  |
`----------------------------------------*/

static void
yy_symbol_value_print (FILE *yyoutput, int yytype, YYSTYPE const * const yyvaluep, YYLTYPE const * const yylocationp)
{
  FILE *yyo = yyoutput;
  YYUSE (yyo);
  YYUSE (yylocationp);
  if (!yyvaluep)
    return;
# ifdef YYPRINT
  if (yytype < YYNTOKENS)
    YYPRINT (yyoutput, yytoknum[yytype], *yyvaluep);
# endif
  YYUSE (yytype);
}


/*--------------------------------.
| Print this symbol on YYOUTPUT.  |
`--------------------------------*/

static void
yy_symbol_print (FILE *yyoutput, int yytype, YYSTYPE const * const yyvaluep, YYLTYPE const * const yylocationp)
{
  YYFPRINTF (yyoutput, "%s %s (",
             yytype < YYNTOKENS ? "token" : "nterm", yytname[yytype]);

  YY_LOCATION_PRINT (yyoutput, *yylocationp);
  YYFPRINTF (yyoutput, ": ");
  yy_symbol_value_print (yyoutput, yytype, yyvaluep, yylocationp);
  YYFPRINTF (yyoutput, ")");
}

/*------------------------------------------------------------------.
| yy_stack_print -- Print the state stack from its BOTTOM up to its |
| TOP (included).                                                   |
`------------------------------------------------------------------*/

static void
yy_stack_print (yytype_int16 *yybottom, yytype_int16 *yytop)
{
  YYFPRINTF (stderr, "Stack now");
  for (; yybottom <= yytop; yybottom++)
    {
      int yybot = *yybottom;
      YYFPRINTF (stderr, " %d", yybot);
    }
  YYFPRINTF (stderr, "\n");
}

# define YY_STACK_PRINT(Bottom, Top)                            \
do {                                                            \
  if (yydebug)                                                  \
    yy_stack_print ((Bottom), (Top));                           \
} while (0)


/*------------------------------------------------.
| Report that the YYRULE is going to be reduced.  |
`------------------------------------------------*/

static void
yy_reduce_print (yytype_int16 *yyssp, YYSTYPE *yyvsp, YYLTYPE *yylsp, int yyrule)
{
  unsigned long int yylno = yyrline[yyrule];
  int yynrhs = yyr2[yyrule];
  int yyi;
  YYFPRINTF (stderr, "Reducing stack by rule %d (line %lu):\n",
             yyrule - 1, yylno);
  /* The symbols being reduced.  */
  for (yyi = 0; yyi < yynrhs; yyi++)
    {
      YYFPRINTF (stderr, "   $%d = ", yyi + 1);
      yy_symbol_print (stderr,
                       yystos[yyssp[yyi + 1 - yynrhs]],
                       &(yyvsp[(yyi + 1) - (yynrhs)])
                       , &(yylsp[(yyi + 1) - (yynrhs)])                       );
      YYFPRINTF (stderr, "\n");
    }
}

# define YY_REDUCE_PRINT(Rule)          \
do {                                    \
  if (yydebug)                          \
    yy_reduce_print (yyssp, yyvsp, yylsp, Rule); \
} while (0)

/* Nonzero means print parse trace.  It is left uninitialized so that
   multiple parsers can coexist.  */
int yydebug;
#else /* !YYDEBUG */
# define YYDPRINTF(Args)
# define YY_SYMBOL_PRINT(Title, Type, Value, Location)
# define YY_STACK_PRINT(Bottom, Top)
# define YY_REDUCE_PRINT(Rule)
#endif /* !YYDEBUG */


/* YYINITDEPTH -- initial size of the parser's stacks.  */
#ifndef YYINITDEPTH
# define YYINITDEPTH 200
#endif

/* YYMAXDEPTH -- maximum size the stacks can grow to (effective only
   if the built-in stack extension method is used).

   Do not make this value too large; the results are undefined if
   YYSTACK_ALLOC_MAXIMUM < YYSTACK_BYTES (YYMAXDEPTH)
   evaluated with infinite-precision integer arithmetic.  */

#ifndef YYMAXDEPTH
# define YYMAXDEPTH 10000
#endif


#if YYERROR_VERBOSE

# ifndef yystrlen
#  if defined __GLIBC__ && defined _STRING_H
#   define yystrlen strlen
#  else
/* Return the length of YYSTR.  */
static YYSIZE_T
yystrlen (const char *yystr)
{
  YYSIZE_T yylen;
  for (yylen = 0; yystr[yylen]; yylen++)
    continue;
  return yylen;
}
#  endif
# endif

# ifndef yystpcpy
#  if defined __GLIBC__ && defined _STRING_H && defined _GNU_SOURCE
#   define yystpcpy stpcpy
#  else
/* Copy YYSRC to YYDEST, returning the address of the terminating '\0' in
   YYDEST.  */
static char *
yystpcpy (char *yydest, const char *yysrc)
{
  char *yyd = yydest;
  const char *yys = yysrc;

  while ((*yyd++ = *yys++) != '\0')
    continue;

  return yyd - 1;
}
#  endif
# endif

# ifndef yytnamerr
/* Copy to YYRES the contents of YYSTR after stripping away unnecessary
   quotes and backslashes, so that it's suitable for yyerror.  The
   heuristic is that double-quoting is unnecessary unless the string
   contains an apostrophe, a comma, or backslash (other than
   backslash-backslash).  YYSTR is taken from yytname.  If YYRES is
   null, do not copy; instead, return the length of what the result
   would have been.  */
static YYSIZE_T
yytnamerr (char *yyres, const char *yystr)
{
  if (*yystr == '"')
    {
      YYSIZE_T yyn = 0;
      char const *yyp = yystr;

      for (;;)
        switch (*++yyp)
          {
          case '\'':
          case ',':
            goto do_not_strip_quotes;

          case '\\':
            if (*++yyp != '\\')
              goto do_not_strip_quotes;
            /* Fall through.  */
          default:
            if (yyres)
              yyres[yyn] = *yyp;
            yyn++;
            break;

          case '"':
            if (yyres)
              yyres[yyn] = '\0';
            return yyn;
          }
    do_not_strip_quotes: ;
    }

  if (! yyres)
    return yystrlen (yystr);

  return yystpcpy (yyres, yystr) - yyres;
}
# endif

/* Copy into *YYMSG, which is of size *YYMSG_ALLOC, an error message
   about the unexpected token YYTOKEN for the state stack whose top is
   YYSSP.

   Return 0 if *YYMSG was successfully written.  Return 1 if *YYMSG is
   not large enough to hold the message.  In that case, also set
   *YYMSG_ALLOC to the required number of bytes.  Return 2 if the
   required number of bytes is too large to store.  */
static int
yysyntax_error (YYSIZE_T *yymsg_alloc, char **yymsg,
                yytype_int16 *yyssp, int yytoken)
{
  YYSIZE_T yysize0 = yytnamerr (YY_NULLPTR, yytname[yytoken]);
  YYSIZE_T yysize = yysize0;
  enum { YYERROR_VERBOSE_ARGS_MAXIMUM = 5 };
  /* Internationalized format string. */
  const char *yyformat = YY_NULLPTR;
  /* Arguments of yyformat. */
  char const *yyarg[YYERROR_VERBOSE_ARGS_MAXIMUM];
  /* Number of reported tokens (one for the "unexpected", one per
     "expected"). */
  int yycount = 0;

  /* There are many possibilities here to consider:
     - If this state is a consistent state with a default action, then
       the only way this function was invoked is if the default action
       is an error action.  In that case, don't check for expected
       tokens because there are none.
     - The only way there can be no lookahead present (in yychar) is if
       this state is a consistent state with a default action.  Thus,
       detecting the absence of a lookahead is sufficient to determine
       that there is no unexpected or expected token to report.  In that
       case, just report a simple "syntax error".
     - Don't assume there isn't a lookahead just because this state is a
       consistent state with a default action.  There might have been a
       previous inconsistent state, consistent state with a non-default
       action, or user semantic action that manipulated yychar.
     - Of course, the expected token list depends on states to have
       correct lookahead information, and it depends on the parser not
       to perform extra reductions after fetching a lookahead from the
       scanner and before detecting a syntax error.  Thus, state merging
       (from LALR or IELR) and default reductions corrupt the expected
       token list.  However, the list is correct for canonical LR with
       one exception: it will still contain any token that will not be
       accepted due to an error action in a later state.
  */
  if (yytoken != YYEMPTY)
    {
      int yyn = yypact[*yyssp];
      yyarg[yycount++] = yytname[yytoken];
      if (!yypact_value_is_default (yyn))
        {
          /* Start YYX at -YYN if negative to avoid negative indexes in
             YYCHECK.  In other words, skip the first -YYN actions for
             this state because they are default actions.  */
          int yyxbegin = yyn < 0 ? -yyn : 0;
          /* Stay within bounds of both yycheck and yytname.  */
          int yychecklim = YYLAST - yyn + 1;
          int yyxend = yychecklim < YYNTOKENS ? yychecklim : YYNTOKENS;
          int yyx;

          for (yyx = yyxbegin; yyx < yyxend; ++yyx)
            if (yycheck[yyx + yyn] == yyx && yyx != YYTERROR
                && !yytable_value_is_error (yytable[yyx + yyn]))
              {
                if (yycount == YYERROR_VERBOSE_ARGS_MAXIMUM)
                  {
                    yycount = 1;
                    yysize = yysize0;
                    break;
                  }
                yyarg[yycount++] = yytname[yyx];
                {
                  YYSIZE_T yysize1 = yysize + yytnamerr (YY_NULLPTR, yytname[yyx]);
                  if (! (yysize <= yysize1
                         && yysize1 <= YYSTACK_ALLOC_MAXIMUM))
                    return 2;
                  yysize = yysize1;
                }
              }
        }
    }

  switch (yycount)
    {
# define YYCASE_(N, S)                      \
      case N:                               \
        yyformat = S;                       \
      break
      YYCASE_(0, YY_("syntax error"));
      YYCASE_(1, YY_("syntax error, unexpected %s"));
      YYCASE_(2, YY_("syntax error, unexpected %s, expecting %s"));
      YYCASE_(3, YY_("syntax error, unexpected %s, expecting %s or %s"));
      YYCASE_(4, YY_("syntax error, unexpected %s, expecting %s or %s or %s"));
      YYCASE_(5, YY_("syntax error, unexpected %s, expecting %s or %s or %s or %s"));
# undef YYCASE_
    }

  {
    YYSIZE_T yysize1 = yysize + yystrlen (yyformat);
    if (! (yysize <= yysize1 && yysize1 <= YYSTACK_ALLOC_MAXIMUM))
      return 2;
    yysize = yysize1;
  }

  if (*yymsg_alloc < yysize)
    {
      *yymsg_alloc = 2 * yysize;
      if (! (yysize <= *yymsg_alloc
             && *yymsg_alloc <= YYSTACK_ALLOC_MAXIMUM))
        *yymsg_alloc = YYSTACK_ALLOC_MAXIMUM;
      return 1;
    }

  /* Avoid sprintf, as that infringes on the user's name space.
     Don't have undefined behavior even if the translation
     produced a string with the wrong number of "%s"s.  */
  {
    char *yyp = *yymsg;
    int yyi = 0;
    while ((*yyp = *yyformat) != '\0')
      if (*yyp == '%' && yyformat[1] == 's' && yyi < yycount)
        {
          yyp += yytnamerr (yyp, yyarg[yyi++]);
          yyformat += 2;
        }
      else
        {
          yyp++;
          yyformat++;
        }
  }
  return 0;
}
#endif /* YYERROR_VERBOSE */

/*-----------------------------------------------.
| Release the memory associated to this symbol.  |
`-----------------------------------------------*/

static void
yydestruct (const char *yymsg, int yytype, YYSTYPE *yyvaluep, YYLTYPE *yylocationp)
{
  YYUSE (yyvaluep);
  YYUSE (yylocationp);
  if (!yymsg)
    yymsg = "Deleting";
  YY_SYMBOL_PRINT (yymsg, yytype, yyvaluep, yylocationp);

  YY_IGNORE_MAYBE_UNINITIALIZED_BEGIN
  YYUSE (yytype);
  YY_IGNORE_MAYBE_UNINITIALIZED_END
}




/* The lookahead symbol.  */
int yychar;

/* The semantic value of the lookahead symbol.  */
YYSTYPE yylval;
/* Location data for the lookahead symbol.  */
YYLTYPE yylloc
# if defined YYLTYPE_IS_TRIVIAL && YYLTYPE_IS_TRIVIAL
  = { 1, 1, 1, 1 }
# endif
;
/* Number of syntax errors so far.  */
int yynerrs;


/*----------.
| yyparse.  |
`----------*/

int
yyparse (void)
{
    int yystate;
    /* Number of tokens to shift before error messages enabled.  */
    int yyerrstatus;

    /* The stacks and their tools:
       'yyss': related to states.
       'yyvs': related to semantic values.
       'yyls': related to locations.

       Refer to the stacks through separate pointers, to allow yyoverflow
       to reallocate them elsewhere.  */

    /* The state stack.  */
    yytype_int16 yyssa[YYINITDEPTH];
    yytype_int16 *yyss;
    yytype_int16 *yyssp;

    /* The semantic value stack.  */
    YYSTYPE yyvsa[YYINITDEPTH];
    YYSTYPE *yyvs;
    YYSTYPE *yyvsp;

    /* The location stack.  */
    YYLTYPE yylsa[YYINITDEPTH];
    YYLTYPE *yyls;
    YYLTYPE *yylsp;

    /* The locations where the error started and ended.  */
    YYLTYPE yyerror_range[3];

    YYSIZE_T yystacksize;

  int yyn;
  int yyresult;
  /* Lookahead token as an internal (translated) token number.  */
  int yytoken = 0;
  /* The variables used to return semantic value and location from the
     action routines.  */
  YYSTYPE yyval;
  YYLTYPE yyloc;

#if YYERROR_VERBOSE
  /* Buffer for error messages, and its allocated size.  */
  char yymsgbuf[128];
  char *yymsg = yymsgbuf;
  YYSIZE_T yymsg_alloc = sizeof yymsgbuf;
#endif

#define YYPOPSTACK(N)   (yyvsp -= (N), yyssp -= (N), yylsp -= (N))

  /* The number of symbols on the RHS of the reduced rule.
     Keep to zero when no symbol should be popped.  */
  int yylen = 0;

  yyssp = yyss = yyssa;
  yyvsp = yyvs = yyvsa;
  yylsp = yyls = yylsa;
  yystacksize = YYINITDEPTH;

  YYDPRINTF ((stderr, "Starting parse\n"));

  yystate = 0;
  yyerrstatus = 0;
  yynerrs = 0;
  yychar = YYEMPTY; /* Cause a token to be read.  */
  yylsp[0] = yylloc;
  goto yysetstate;

/*------------------------------------------------------------.
| yynewstate -- Push a new state, which is found in yystate.  |
`------------------------------------------------------------*/
 yynewstate:
  /* In all cases, when you get here, the value and location stacks
     have just been pushed.  So pushing a state here evens the stacks.  */
  yyssp++;

 yysetstate:
  *yyssp = yystate;

  if (yyss + yystacksize - 1 <= yyssp)
    {
      /* Get the current used size of the three stacks, in elements.  */
      YYSIZE_T yysize = yyssp - yyss + 1;

#ifdef yyoverflow
      {
        /* Give user a chance to reallocate the stack.  Use copies of
           these so that the &'s don't force the real ones into
           memory.  */
        YYSTYPE *yyvs1 = yyvs;
        yytype_int16 *yyss1 = yyss;
        YYLTYPE *yyls1 = yyls;

        /* Each stack pointer address is followed by the size of the
           data in use in that stack, in bytes.  This used to be a
           conditional around just the two extra args, but that might
           be undefined if yyoverflow is a macro.  */
        yyoverflow (YY_("memory exhausted"),
                    &yyss1, yysize * sizeof (*yyssp),
                    &yyvs1, yysize * sizeof (*yyvsp),
                    &yyls1, yysize * sizeof (*yylsp),
                    &yystacksize);

        yyls = yyls1;
        yyss = yyss1;
        yyvs = yyvs1;
      }
#else /* no yyoverflow */
# ifndef YYSTACK_RELOCATE
      goto yyexhaustedlab;
# else
      /* Extend the stack our own way.  */
      if (YYMAXDEPTH <= yystacksize)
        goto yyexhaustedlab;
      yystacksize *= 2;
      if (YYMAXDEPTH < yystacksize)
        yystacksize = YYMAXDEPTH;

      {
        yytype_int16 *yyss1 = yyss;
        union yyalloc *yyptr =
          (union yyalloc *) YYSTACK_ALLOC (YYSTACK_BYTES (yystacksize));
        if (! yyptr)
          goto yyexhaustedlab;
        YYSTACK_RELOCATE (yyss_alloc, yyss);
        YYSTACK_RELOCATE (yyvs_alloc, yyvs);
        YYSTACK_RELOCATE (yyls_alloc, yyls);
#  undef YYSTACK_RELOCATE
        if (yyss1 != yyssa)
          YYSTACK_FREE (yyss1);
      }
# endif
#endif /* no yyoverflow */

      yyssp = yyss + yysize - 1;
      yyvsp = yyvs + yysize - 1;
      yylsp = yyls + yysize - 1;

      YYDPRINTF ((stderr, "Stack size increased to %lu\n",
                  (unsigned long int) yystacksize));

      if (yyss + yystacksize - 1 <= yyssp)
        YYABORT;
    }

  YYDPRINTF ((stderr, "Entering state %d\n", yystate));

  if (yystate == YYFINAL)
    YYACCEPT;

  goto yybackup;

/*-----------.
| yybackup.  |
`-----------*/
yybackup:

  /* Do appropriate processing given the current state.  Read a
     lookahead token if we need one and don't already have one.  */

  /* First try to decide what to do without reference to lookahead token.  */
  yyn = yypact[yystate];
  if (yypact_value_is_default (yyn))
    goto yydefault;

  /* Not known => get a lookahead token if don't already have one.  */

  /* YYCHAR is either YYEMPTY or YYEOF or a valid lookahead symbol.  */
  if (yychar == YYEMPTY)
    {
      YYDPRINTF ((stderr, "Reading a token: "));
      yychar = yylex ();
    }

  if (yychar <= YYEOF)
    {
      yychar = yytoken = YYEOF;
      YYDPRINTF ((stderr, "Now at end of input.\n"));
    }
  else
    {
      yytoken = YYTRANSLATE (yychar);
      YY_SYMBOL_PRINT ("Next token is", yytoken, &yylval, &yylloc);
    }

  /* If the proper action on seeing token YYTOKEN is to reduce or to
     detect an error, take that action.  */
  yyn += yytoken;
  if (yyn < 0 || YYLAST < yyn || yycheck[yyn] != yytoken)
    goto yydefault;
  yyn = yytable[yyn];
  if (yyn <= 0)
    {
      if (yytable_value_is_error (yyn))
        goto yyerrlab;
      yyn = -yyn;
      goto yyreduce;
    }

  /* Count tokens shifted since error; after three, turn off error
     status.  */
  if (yyerrstatus)
    yyerrstatus--;

  /* Shift the lookahead token.  */
  YY_SYMBOL_PRINT ("Shifting", yytoken, &yylval, &yylloc);

  /* Discard the shifted token.  */
  yychar = YYEMPTY;

  yystate = yyn;
  YY_IGNORE_MAYBE_UNINITIALIZED_BEGIN
  *++yyvsp = yylval;
  YY_IGNORE_MAYBE_UNINITIALIZED_END
  *++yylsp = yylloc;
  goto yynewstate;


/*-----------------------------------------------------------.
| yydefault -- do the default action for the current state.  |
`-----------------------------------------------------------*/
yydefault:
  yyn = yydefact[yystate];
  if (yyn == 0)
    goto yyerrlab;
  goto yyreduce;


/*-----------------------------.
| yyreduce -- Do a reduction.  |
`-----------------------------*/
yyreduce:
  /* yyn is the number of a rule to reduce with.  */
  yylen = yyr2[yyn];

  /* If YYLEN is nonzero, implement the default value of the action:
     '$$ = $1'.

     Otherwise, the following line sets YYVAL to garbage.
     This behavior is undocumented and Bison
     users should not rely upon it.  Assigning to YYVAL
     unconditionally makes the parser a bit smaller, and it avoids a
     GCC warning that YYVAL may be used uninitialized.  */
  yyval = yyvsp[1-yylen];

  /* Default location.  */
  YYLLOC_DEFAULT (yyloc, (yylsp - yylen), yylen);
  YY_REDUCE_PRINT (yyn);
  switch (yyn)
    {
        case 2:
#line 63 "parser.y" /* yacc.c:1646  */
    { raiz = (yyval.nodo); }
#line 1655 "parser.cpp" /* yacc.c:1646  */
    break;

  case 3:
#line 66 "parser.y" /* yacc.c:1646  */
    {
          (yyval.nodo) = (yyvsp[-1].nodo); (yyval.nodo)->add(*(yyvsp[0].nodo));
        }
#line 1663 "parser.cpp" /* yacc.c:1646  */
    break;

  case 4:
#line 70 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = PRINCIPAL;
          (yyval.nodo) = new NodoAst(yylineno,columna,t,"principal","principal");
          (yyval.nodo)->add(*(yyvsp[0].nodo));
        }
#line 1673 "parser.cpp" /* yacc.c:1646  */
    break;

  case 5:
#line 77 "parser.y" /* yacc.c:1646  */
    {
          (yyval.nodo) = (yyvsp[0].nodo);
        }
#line 1681 "parser.cpp" /* yacc.c:1646  */
    break;

  case 6:
#line 80 "parser.y" /* yacc.c:1646  */
    {
          (yyval.nodo) = (yyvsp[0].nodo);
        }
#line 1689 "parser.cpp" /* yacc.c:1646  */
    break;

  case 7:
#line 83 "parser.y" /* yacc.c:1646  */
    {
          (yyval.nodo) = (yyvsp[0].nodo);
        }
#line 1697 "parser.cpp" /* yacc.c:1646  */
    break;

  case 8:
#line 86 "parser.y" /* yacc.c:1646  */
    {
          (yyval.nodo) = (yyvsp[0].nodo);
        }
#line 1705 "parser.cpp" /* yacc.c:1646  */
    break;

  case 9:
#line 89 "parser.y" /* yacc.c:1646  */
    {
          (yyval.nodo) = (yyvsp[0].nodo);
        }
#line 1713 "parser.cpp" /* yacc.c:1646  */
    break;

  case 10:
#line 92 "parser.y" /* yacc.c:1646  */
    {
          (yyval.nodo) = (yyvsp[0].nodo);
        }
#line 1721 "parser.cpp" /* yacc.c:1646  */
    break;

  case 11:
#line 97 "parser.y" /* yacc.c:1646  */
    {
          (yyval.nodo) = (yyvsp[0].nodo);
        }
#line 1729 "parser.cpp" /* yacc.c:1646  */
    break;

  case 12:
#line 100 "parser.y" /* yacc.c:1646  */
    {
          (yyval.nodo) = (yyvsp[0].nodo);
        }
#line 1737 "parser.cpp" /* yacc.c:1646  */
    break;

  case 13:
#line 105 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = LID;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"LID","LID");
          nuevo->add(*(yyvsp[-2].nodo)); nuevo->add(*(yyvsp[-1].nodo));
          (yyval.nodo) = nuevo;
        }
#line 1748 "parser.cpp" /* yacc.c:1646  */
    break;

  case 14:
#line 111 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = LID;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"LID","LID");
          nuevo->add(*(yyvsp[-4].nodo)); nuevo->add(*(yyvsp[-3].nodo)); nuevo->add(*(yyvsp[-1].nodo));
          (yyval.nodo) = nuevo;
        }
#line 1759 "parser.cpp" /* yacc.c:1646  */
    break;

  case 15:
#line 117 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = LARRAY;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"Larray","Larray");
          nuevo->add(*(yyvsp[-4].nodo)); nuevo->add(*(yyvsp[-2].nodo)); nuevo->add(*(yyvsp[-1].nodo));
          (yyval.nodo) = nuevo;
        }
#line 1770 "parser.cpp" /* yacc.c:1646  */
    break;

  case 16:
#line 123 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = LARRAY;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"Larray","Larray");
          nuevo->add(*(yyvsp[-8].nodo)); nuevo->add(*(yyvsp[-6].nodo)); nuevo->add(*(yyvsp[-5].nodo)); nuevo->add(*(yyvsp[-2].nodo));
          (yyval.nodo) = nuevo;
        }
#line 1781 "parser.cpp" /* yacc.c:1646  */
    break;

  case 17:
#line 129 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = LARRAY;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"Larray","Larray");
          nuevo->add(*(yyvsp[-8].nodo)); nuevo->add(*(yyvsp[-6].nodo)); nuevo->add(*(yyvsp[-5].nodo)); nuevo->add(*(yyvsp[-2].nodo));
          (yyval.nodo) = nuevo;
        }
#line 1792 "parser.cpp" /* yacc.c:1646  */
    break;

  case 18:
#line 135 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = LARRAY;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"Larray","Larray");
          nuevo->add(*(yyvsp[-8].nodo)); nuevo->add(*(yyvsp[-6].nodo)); nuevo->add(*(yyvsp[-5].nodo)); nuevo->add(*(yyvsp[-2].nodo));
          (yyval.nodo) = nuevo;
        }
#line 1803 "parser.cpp" /* yacc.c:1646  */
    break;

  case 19:
#line 144 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = RINT;
          (yyval.nodo) = new NodoAst(yylineno,columna,t,"int","int");
        }
#line 1812 "parser.cpp" /* yacc.c:1646  */
    break;

  case 20:
#line 148 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = RSTRING;
          (yyval.nodo) = new NodoAst(yylineno,columna,t,"string","string");
        }
#line 1821 "parser.cpp" /* yacc.c:1646  */
    break;

  case 21:
#line 152 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = RDOUBLE;
          (yyval.nodo) = new NodoAst(yylineno,columna,t,"double","double");
        }
#line 1830 "parser.cpp" /* yacc.c:1646  */
    break;

  case 22:
#line 156 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = RCHAR;
          (yyval.nodo) = new NodoAst(yylineno,columna,t,"char","char");
        }
#line 1839 "parser.cpp" /* yacc.c:1646  */
    break;

  case 23:
#line 160 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = RBOOL;
          (yyval.nodo) = new NodoAst(yylineno,columna,t,"bool","bool");
        }
#line 1848 "parser.cpp" /* yacc.c:1646  */
    break;

  case 24:
#line 166 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = ASID;
          (yyval.nodo) = new NodoAst(yylineno,columna,t,"Asignacion","Asignacion");
          Tipo t1 = RID; NodoAst *nq = new NodoAst(yylineno,columna,t1,(yyvsp[-3].TEXT),"id");
          (yyval.nodo)->add(*nq);
          (yyval.nodo)->add(*(yyvsp[-1].nodo));
        }
#line 1860 "parser.cpp" /* yacc.c:1646  */
    break;

  case 25:
#line 173 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = ASID;
          (yyval.nodo) = new NodoAst(yylineno,columna,t,"Asignacion","Asignacion");
          Tipo t1 = RID; NodoAst *nq = new NodoAst(yylineno,columna,t1,(yyvsp[-2].TEXT),"id");
          (yyval.nodo)->add(*nq);
          (yyval.nodo)->add(*(yyvsp[-1].nodo));
        }
#line 1872 "parser.cpp" /* yacc.c:1646  */
    break;

  case 26:
#line 180 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = ASARR;
          (yyval.nodo) = new NodoAst(yylineno,columna,t,"AsignacionArray","AsignacionArray");
          Tipo t1 = RID; NodoAst *nq = new NodoAst(yylineno,columna,t1,(yyvsp[-6].TEXT),"id");
          (yyval.nodo)->add(*nq);
          (yyval.nodo)->add(*(yyvsp[-4].nodo));
          (yyval.nodo)->add(*(yyvsp[-1].nodo));
        }
#line 1885 "parser.cpp" /* yacc.c:1646  */
    break;

  case 27:
#line 189 "parser.y" /* yacc.c:1646  */
    {
            Tipo t = ASARR;
            (yyval.nodo) = new NodoAst(yylineno,columna,t,"AsignacionArray","AsignacionArray");
            Tipo t1 = RID; NodoAst *nq = new NodoAst(yylineno,columna,t1,(yyvsp[-9].TEXT),"id");
            (yyval.nodo)->add(*nq);
            (yyval.nodo)->add(*(yyvsp[-7].nodo));
            (yyval.nodo)->add(*(yyvsp[-4].nodo));
            (yyval.nodo)->add(*(yyvsp[-1].nodo));
          }
#line 1899 "parser.cpp" /* yacc.c:1646  */
    break;

  case 28:
#line 198 "parser.y" /* yacc.c:1646  */
    {
              Tipo t = ASARR;
              (yyval.nodo) = new NodoAst(yylineno,columna,t,"AsignacionArray","AsignacionArray");
              Tipo t1 = RID; NodoAst *nq = new NodoAst(yylineno,columna,t1,(yyvsp[-12].TEXT),"id");
              (yyval.nodo)->add(*nq);
              (yyval.nodo)->add(*(yyvsp[-10].nodo));
              (yyval.nodo)->add(*(yyvsp[-7].nodo));
              (yyval.nodo)->add(*(yyvsp[-4].nodo));
              (yyval.nodo)->add(*(yyvsp[-1].nodo));

        }
#line 1915 "parser.cpp" /* yacc.c:1646  */
    break;

  case 29:
#line 211 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = DIM_ARRAY;
          (yyval.nodo) = new NodoAst(yylineno,columna,t,"LArray","LArray");
          (yyval.nodo)->add(*(yyvsp[-1].nodo));
        }
#line 1925 "parser.cpp" /* yacc.c:1646  */
    break;

  case 30:
#line 216 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = DIM_ARRAY;
          (yyval.nodo) = new NodoAst(yylineno,columna,t,"LArray","LArray");
          (yyval.nodo)->add(*(yyvsp[-4].nodo));
          (yyval.nodo)->add(*(yyvsp[-1].nodo));
        }
#line 1936 "parser.cpp" /* yacc.c:1646  */
    break;

  case 31:
#line 222 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = DIM_ARRAY;
          (yyval.nodo) = new NodoAst(yylineno,columna,t,"LArray","LArray");
          (yyval.nodo)->add(*(yyvsp[-7].nodo));
          (yyval.nodo)->add(*(yyvsp[-4].nodo));
          (yyval.nodo)->add(*(yyvsp[-1].nodo));
        }
#line 1948 "parser.cpp" /* yacc.c:1646  */
    break;

  case 32:
#line 231 "parser.y" /* yacc.c:1646  */
    {
          (yyval.nodo) = (yyvsp[-4].nodo);
          (yyval.nodo)->add(*(yyvsp[-1].nodo));
        }
#line 1957 "parser.cpp" /* yacc.c:1646  */
    break;

  case 33:
#line 235 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = LISTPROF;
          (yyval.nodo) = new NodoAst(yylineno,columna,t,"LProf","LProf");
          (yyval.nodo)->add(*(yyvsp[-1].nodo));
        }
#line 1967 "parser.cpp" /* yacc.c:1646  */
    break;

  case 34:
#line 242 "parser.y" /* yacc.c:1646  */
    {
          (yyval.nodo) = (yyvsp[-4].nodo);
          (yyval.nodo)->add(*(yyvsp[-1].nodo));
        }
#line 1976 "parser.cpp" /* yacc.c:1646  */
    break;

  case 35:
#line 246 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = LISTROW;
          (yyval.nodo) = new NodoAst(yylineno,columna,t,"LRow","LRow");
          (yyval.nodo)->add(*(yyvsp[-1].nodo));
        }
#line 1986 "parser.cpp" /* yacc.c:1646  */
    break;

  case 36:
#line 253 "parser.y" /* yacc.c:1646  */
    {
          (yyval.nodo) = (yyvsp[-2].nodo);
          (yyval.nodo)->add(*(yyvsp[0].nodo));
        }
#line 1995 "parser.cpp" /* yacc.c:1646  */
    break;

  case 37:
#line 257 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = LISTCOLUMN;
          (yyval.nodo) = new NodoAst(yylineno,columna,t,"LCol","LCol");
          (yyval.nodo)->add(*(yyvsp[0].nodo));
        }
#line 2005 "parser.cpp" /* yacc.c:1646  */
    break;

  case 38:
#line 264 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = RSHOW;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"show","show");
          nuevo->add(*(yyvsp[-2].nodo));
          (yyval.nodo) = nuevo;
        }
#line 2016 "parser.cpp" /* yacc.c:1646  */
    break;

  case 39:
#line 270 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = RSHOW;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"show","show");
          nuevo->add(*(yyvsp[-4].nodo));
          nuevo->add(*(yyvsp[-2].nodo));
          (yyval.nodo) = nuevo;
        }
#line 2028 "parser.cpp" /* yacc.c:1646  */
    break;

  case 40:
#line 279 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = RIMPRIMIR;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"imprimir","imprimir");
          nuevo->add(*(yyvsp[-2].nodo));
          (yyval.nodo) = nuevo;
        }
#line 2039 "parser.cpp" /* yacc.c:1646  */
    break;

  case 41:
#line 287 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = RREPETIR;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"repetir","repetir");
          nuevo->add(*(yyvsp[-4].nodo));
          nuevo->add(*(yyvsp[-1].nodo));
          (yyval.nodo) = nuevo;
        }
#line 2051 "parser.cpp" /* yacc.c:1646  */
    break;

  case 42:
#line 296 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = RPARA;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"para","para");
          nuevo->add(*(yyvsp[-4].nodo));
          nuevo->add(*(yyvsp[-1].nodo));
          (yyval.nodo) = nuevo;
        }
#line 2063 "parser.cpp" /* yacc.c:1646  */
    break;

  case 43:
#line 305 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = INPARA;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"inpara","inpara");
          (yyval.nodo) = nuevo;

          Tipo t1 = VID; NodoAst *n1 = new NodoAst(yylineno,columna,t1,(yyvsp[-7].TEXT),"id");
          NodoAst *n2 = new NodoAst(yylineno,columna,t1,(yyvsp[-1].TEXT),"id");

          (yyval.nodo)->add(*n1); (yyval.nodo)->add(*(yyvsp[-5].nodo));
          (yyval.nodo)->add(*(yyvsp[-3].nodo));
          (yyval.nodo)->add(*n2); (yyval.nodo)->add(*(yyvsp[0].nodo));

        }
#line 2081 "parser.cpp" /* yacc.c:1646  */
    break;

  case 44:
#line 318 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = INPARA2;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"inpara2","inpara2");
          (yyval.nodo) = nuevo;

          Tipo t1 = VID; NodoAst *n1 = new NodoAst(yylineno,columna,t1,(yyvsp[-7].TEXT),"id");
          NodoAst *n2 = new NodoAst(yylineno,columna,t1,(yyvsp[-1].TEXT),"id");

          (yyval.nodo)->add(*n1); (yyval.nodo)->add(*(yyvsp[-5].nodo));
          (yyval.nodo)->add(*(yyvsp[-3].nodo));
          (yyval.nodo)->add(*n2); (yyval.nodo)->add(*(yyvsp[0].nodo));
        }
#line 2098 "parser.cpp" /* yacc.c:1646  */
    break;

  case 45:
#line 333 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = AUMENTO; (yyval.nodo) = new NodoAst(yylineno,columna,t,(yyvsp[0].TEXT),"++");
        }
#line 2106 "parser.cpp" /* yacc.c:1646  */
    break;

  case 46:
#line 336 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = DECREMENTO; (yyval.nodo) = new NodoAst(yylineno,columna,t,(yyvsp[0].TEXT),"--");
        }
#line 2114 "parser.cpp" /* yacc.c:1646  */
    break;

  case 47:
#line 341 "parser.y" /* yacc.c:1646  */
    {
          (yyval.nodo) = (yyvsp[-2].nodo);
          Tipo t = RID; NodoAst *nuevo = new NodoAst(yylineno,columna,t,(yyvsp[0].TEXT),"id");
          (yyval.nodo)->add(*nuevo);
        }
#line 2124 "parser.cpp" /* yacc.c:1646  */
    break;

  case 48:
#line 346 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = LISTID; (yyval.nodo) = new NodoAst(yylineno,columna,t,"listd","listd");
          Tipo t1 = RID; NodoAst *nuevo = new NodoAst(yylineno,columna,t1,(yyvsp[0].TEXT),"id");
          (yyval.nodo)->add(*nuevo);
        }
#line 2134 "parser.cpp" /* yacc.c:1646  */
    break;

  case 49:
#line 353 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = LISTIF;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"listaif","listaif");
          Tipo t1 = RSINO; NodoAst *nuevo1 = new NodoAst(yylineno,columna,t1,"si","si");
          nuevo1->add(*(yyvsp[-4].nodo)); nuevo1->add(*(yyvsp[-1].nodo));
          nuevo->add(*nuevo1);
          (yyval.nodo) = nuevo;
        }
#line 2147 "parser.cpp" /* yacc.c:1646  */
    break;

  case 50:
#line 361 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = LISTIF;
          NodoAst *nuevo = new NodoAst(yylineno,yylineno,t,"listaif","listaif");
          Tipo t1 = RSINO; NodoAst *nuevo1 = new NodoAst(yylineno,columna,t1,"si","si");
          nuevo1->add(*(yyvsp[-5].nodo)); nuevo1->add(*(yyvsp[-2].nodo));
          nuevo->add(*nuevo1); nuevo->add(*(yyvsp[0].nodo));
          (yyval.nodo) = nuevo;
        }
#line 2160 "parser.cpp" /* yacc.c:1646  */
    break;

  case 51:
#line 369 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = LISTIF;
          NodoAst *nuevo = new NodoAst(yylineno,yylineno,t,"listaif","listaif");
          Tipo t1 = RSINO; NodoAst *nuevo1 = new NodoAst(yylineno,columna,t1,"si","si");
          nuevo1->add(*(yyvsp[-5].nodo)); nuevo1->add(*(yyvsp[-2].nodo));
          nuevo->add(*nuevo1); nuevo->add(*(yyvsp[0].nodo));
          (yyval.nodo) = nuevo;
        }
#line 2173 "parser.cpp" /* yacc.c:1646  */
    break;

  case 52:
#line 377 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = LISTIF;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"listaif","listaif");
          Tipo t1 = RSINO; NodoAst *nuevo1 = new NodoAst(yylineno,columna,t1,"si","si");
          nuevo1->add(*(yyvsp[-6].nodo)); nuevo1->add(*(yyvsp[-3].nodo));
          nuevo->add(*nuevo1); nuevo->add(*(yyvsp[-1].nodo)); nuevo->add(*(yyvsp[0].nodo));
          (yyval.nodo) = nuevo;
        }
#line 2186 "parser.cpp" /* yacc.c:1646  */
    break;

  case 53:
#line 387 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = RSINO;
          (yyval.nodo) = new NodoAst(yylineno,columna,t,"sino","sino");
          (yyval.nodo)->add(*(yyvsp[-1].nodo));
        }
#line 2196 "parser.cpp" /* yacc.c:1646  */
    break;

  case 54:
#line 394 "parser.y" /* yacc.c:1646  */
    {
          (yyval.nodo) = (yyvsp[-8].nodo);
          Tipo t = RSINOSI; NodoAst *nuevo = new NodoAst(yylineno,columna,t,"sinosi","sinosi");
          nuevo->add(*(yyvsp[-4].nodo)); nuevo->add(*(yyvsp[-1].nodo));
          (yyval.nodo)->add(*nuevo);
        }
#line 2207 "parser.cpp" /* yacc.c:1646  */
    break;

  case 55:
#line 400 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = LISTEIF;
          NodoAst *nuevo = new NodoAst(yylineno,columna,t,"listaelseif","listaelseif");
          Tipo t1 = RSINOSI; NodoAst *nuevo1 = new NodoAst(yylineno,columna,t1,"sinosi","sinosi");
          nuevo1->add(*(yyvsp[-4].nodo)); nuevo1->add(*(yyvsp[-1].nodo));
          nuevo->add(*nuevo1);
          (yyval.nodo) = nuevo;
        }
#line 2220 "parser.cpp" /* yacc.c:1646  */
    break;

  case 56:
#line 410 "parser.y" /* yacc.c:1646  */
    { Tipo t = AND; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"and"); nodo->add(*(yyvsp[-2].nodo)); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo;}
#line 2226 "parser.cpp" /* yacc.c:1646  */
    break;

  case 57:
#line 411 "parser.y" /* yacc.c:1646  */
    {Tipo t = OR; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"or"); nodo->add(*(yyvsp[-2].nodo)); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo; }
#line 2232 "parser.cpp" /* yacc.c:1646  */
    break;

  case 58:
#line 412 "parser.y" /* yacc.c:1646  */
    {Tipo t = NOT; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"not"); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo; }
#line 2238 "parser.cpp" /* yacc.c:1646  */
    break;

  case 59:
#line 413 "parser.y" /* yacc.c:1646  */
    {Tipo t = IGUALLOGICO; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"iguallogico"); nodo->add(*(yyvsp[-2].nodo)); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo;}
#line 2244 "parser.cpp" /* yacc.c:1646  */
    break;

  case 60:
#line 414 "parser.y" /* yacc.c:1646  */
    {Tipo t = DESIGUAL; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"desigual"); nodo->add(*(yyvsp[-2].nodo)); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo;}
#line 2250 "parser.cpp" /* yacc.c:1646  */
    break;

  case 61:
#line 415 "parser.y" /* yacc.c:1646  */
    {Tipo t = MAYOR; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"mayor"); nodo->add(*(yyvsp[-2].nodo)); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo;}
#line 2256 "parser.cpp" /* yacc.c:1646  */
    break;

  case 62:
#line 416 "parser.y" /* yacc.c:1646  */
    {Tipo t = MAYORQUE; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"mayorigual"); nodo->add(*(yyvsp[-2].nodo)); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo;}
#line 2262 "parser.cpp" /* yacc.c:1646  */
    break;

  case 63:
#line 417 "parser.y" /* yacc.c:1646  */
    {Tipo t = MENOR; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"menor"); nodo->add(*(yyvsp[-2].nodo)); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo;}
#line 2268 "parser.cpp" /* yacc.c:1646  */
    break;

  case 64:
#line 418 "parser.y" /* yacc.c:1646  */
    {Tipo t = MENORQUE; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"menorigual"); nodo->add(*(yyvsp[-2].nodo)); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo;}
#line 2274 "parser.cpp" /* yacc.c:1646  */
    break;

  case 65:
#line 419 "parser.y" /* yacc.c:1646  */
    { Tipo t = MAS; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"suma"); nodo->add(*(yyvsp[-2].nodo)); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo;}
#line 2280 "parser.cpp" /* yacc.c:1646  */
    break;

  case 66:
#line 420 "parser.y" /* yacc.c:1646  */
    {Tipo t = MENOS; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"resta"); nodo->add(*(yyvsp[-2].nodo)); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo; }
#line 2286 "parser.cpp" /* yacc.c:1646  */
    break;

  case 67:
#line 421 "parser.y" /* yacc.c:1646  */
    {Tipo t = POR; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"multiplicacion"); nodo->add(*(yyvsp[-2].nodo)); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo; }
#line 2292 "parser.cpp" /* yacc.c:1646  */
    break;

  case 68:
#line 422 "parser.y" /* yacc.c:1646  */
    {Tipo t = DIV; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"division"); nodo->add(*(yyvsp[-2].nodo)); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo; }
#line 2298 "parser.cpp" /* yacc.c:1646  */
    break;

  case 69:
#line 423 "parser.y" /* yacc.c:1646  */
    {Tipo t = POTENCIA; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"potencia"); nodo->add(*(yyvsp[-2].nodo)); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo; }
#line 2304 "parser.cpp" /* yacc.c:1646  */
    break;

  case 70:
#line 424 "parser.y" /* yacc.c:1646  */
    {Tipo t = NUMERO; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"entero"); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo; }
#line 2310 "parser.cpp" /* yacc.c:1646  */
    break;

  case 71:
#line 425 "parser.y" /* yacc.c:1646  */
    {Tipo t = CARACTER; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"caracter"); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo; }
#line 2316 "parser.cpp" /* yacc.c:1646  */
    break;

  case 72:
#line 426 "parser.y" /* yacc.c:1646  */
    {Tipo t = DECIMAL; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"decimal"); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo; }
#line 2322 "parser.cpp" /* yacc.c:1646  */
    break;

  case 73:
#line 427 "parser.y" /* yacc.c:1646  */
    {Tipo t = RID; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"id"); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo; }
#line 2328 "parser.cpp" /* yacc.c:1646  */
    break;

  case 74:
#line 428 "parser.y" /* yacc.c:1646  */
    {Tipo t = MENOS; NodoAst *nodo = new NodoAst(yylineno,columna,t,(yyvsp[-1].TEXT),"menos"); nodo->add(*(yyvsp[0].nodo)); (yyval.nodo)=nodo; }
#line 2334 "parser.cpp" /* yacc.c:1646  */
    break;

  case 75:
#line 429 "parser.y" /* yacc.c:1646  */
    {Tipo t = NUMERO; (yyval.nodo) = new NodoAst(yylineno,columna,t,(yyvsp[0].TEXT),"entero"); }
#line 2340 "parser.cpp" /* yacc.c:1646  */
    break;

  case 76:
#line 430 "parser.y" /* yacc.c:1646  */
    {Tipo t = CARACTER; (yyval.nodo) = new NodoAst(yylineno,columna,t,(yyvsp[0].TEXT),"caracter"); }
#line 2346 "parser.cpp" /* yacc.c:1646  */
    break;

  case 77:
#line 431 "parser.y" /* yacc.c:1646  */
    {Tipo t = DECIMAL; (yyval.nodo) = new NodoAst(yylineno,columna,t,(yyvsp[0].TEXT),"decimal"); }
#line 2352 "parser.cpp" /* yacc.c:1646  */
    break;

  case 78:
#line 432 "parser.y" /* yacc.c:1646  */
    { Tipo t = BOOLEANO; (yyval.nodo) = new NodoAst(yylineno,columna,t,(yyvsp[0].TEXT),"booleano");}
#line 2358 "parser.cpp" /* yacc.c:1646  */
    break;

  case 79:
#line 433 "parser.y" /* yacc.c:1646  */
    { Tipo t = CADENA; (yyval.nodo) = new NodoAst(yylineno,columna,t,(yyvsp[0].TEXT),"cadena");}
#line 2364 "parser.cpp" /* yacc.c:1646  */
    break;

  case 80:
#line 434 "parser.y" /* yacc.c:1646  */
    { Tipo t = VID; (yyval.nodo) = new NodoAst(yylineno,columna,t,(yyvsp[0].TEXT),"id");}
#line 2370 "parser.cpp" /* yacc.c:1646  */
    break;

  case 81:
#line 436 "parser.y" /* yacc.c:1646  */
    {Tipo t = DATOMATRIZ; (yyval.nodo) = new NodoAst(yylineno,columna,t,"datoarray","datoarray");
          Tipo t1 = VID; NodoAst *nuevo = new NodoAst(yylineno,columna,t1,(yyvsp[-3].TEXT),"id");
          Tipo t2 = POSARR; NodoAst *nuevo2 = new NodoAst(yylineno,columna,t2,"posarr","posarr"); nuevo2->add(*(yyvsp[-1].nodo));
          (yyval.nodo)->add(*nuevo); (yyval.nodo)->add(*nuevo2);
        }
#line 2380 "parser.cpp" /* yacc.c:1646  */
    break;

  case 82:
#line 441 "parser.y" /* yacc.c:1646  */
    {Tipo t = DATOMATRIZ; (yyval.nodo) = new NodoAst(yylineno,columna,t,"datoarray","datoarray");
          Tipo t1 = VID; NodoAst *nuevo = new NodoAst(yylineno,columna,t1,(yyvsp[-6].TEXT),"id");
          Tipo t2 = POSARR; NodoAst *nuevo2 = new NodoAst(yylineno,columna,t2,"posarr","posarr"); nuevo2->add(*(yyvsp[-4].nodo));
          NodoAst *nuevo3 = new NodoAst(yylineno,columna,t2,"posarr","posarr"); nuevo3->add(*(yyvsp[-1].nodo));
          (yyval.nodo)->add(*nuevo); (yyval.nodo)->add(*nuevo2); (yyval.nodo)->add(*nuevo3);
        }
#line 2391 "parser.cpp" /* yacc.c:1646  */
    break;

  case 83:
#line 447 "parser.y" /* yacc.c:1646  */
    {
          Tipo t = DATOMATRIZ; (yyval.nodo) = new NodoAst(yylineno,columna,t,"datoarray","datoarray");

          Tipo t1 = VID; NodoAst *nuevo = new NodoAst(yylineno,columna,t1,(yyvsp[-9].TEXT),"id");
          Tipo t2 = POSARR; NodoAst *nuevo2 = new NodoAst(yylineno,columna,t2,"posarr","posarr"); nuevo2->add(*(yyvsp[-7].nodo));
          NodoAst *nuevo3 = new NodoAst(yylineno,columna,t2,"posarr","posarr"); nuevo3->add(*(yyvsp[-4].nodo));
          NodoAst *nuevo4 = new NodoAst(yylineno,columna,t2,"posarr","posarr"); nuevo4->add(*(yyvsp[-1].nodo));
          (yyval.nodo)->add(*nuevo); (yyval.nodo)->add(*nuevo2); (yyval.nodo)->add(*nuevo3); (yyval.nodo)->add(*nuevo4);
        }
#line 2405 "parser.cpp" /* yacc.c:1646  */
    break;

  case 84:
#line 456 "parser.y" /* yacc.c:1646  */
    {(yyval.nodo)=(yyvsp[-1].nodo);}
#line 2411 "parser.cpp" /* yacc.c:1646  */
    break;


#line 2415 "parser.cpp" /* yacc.c:1646  */
      default: break;
    }
  /* User semantic actions sometimes alter yychar, and that requires
     that yytoken be updated with the new translation.  We take the
     approach of translating immediately before every use of yytoken.
     One alternative is translating here after every semantic action,
     but that translation would be missed if the semantic action invokes
     YYABORT, YYACCEPT, or YYERROR immediately after altering yychar or
     if it invokes YYBACKUP.  In the case of YYABORT or YYACCEPT, an
     incorrect destructor might then be invoked immediately.  In the
     case of YYERROR or YYBACKUP, subsequent parser actions might lead
     to an incorrect destructor call or verbose syntax error message
     before the lookahead is translated.  */
  YY_SYMBOL_PRINT ("-> $$ =", yyr1[yyn], &yyval, &yyloc);

  YYPOPSTACK (yylen);
  yylen = 0;
  YY_STACK_PRINT (yyss, yyssp);

  *++yyvsp = yyval;
  *++yylsp = yyloc;

  /* Now 'shift' the result of the reduction.  Determine what state
     that goes to, based on the state we popped back to and the rule
     number reduced by.  */

  yyn = yyr1[yyn];

  yystate = yypgoto[yyn - YYNTOKENS] + *yyssp;
  if (0 <= yystate && yystate <= YYLAST && yycheck[yystate] == *yyssp)
    yystate = yytable[yystate];
  else
    yystate = yydefgoto[yyn - YYNTOKENS];

  goto yynewstate;


/*--------------------------------------.
| yyerrlab -- here on detecting error.  |
`--------------------------------------*/
yyerrlab:
  /* Make sure we have latest lookahead translation.  See comments at
     user semantic actions for why this is necessary.  */
  yytoken = yychar == YYEMPTY ? YYEMPTY : YYTRANSLATE (yychar);

  /* If not already recovering from an error, report this error.  */
  if (!yyerrstatus)
    {
      ++yynerrs;
#if ! YYERROR_VERBOSE
      yyerror (YY_("syntax error"));
#else
# define YYSYNTAX_ERROR yysyntax_error (&yymsg_alloc, &yymsg, \
                                        yyssp, yytoken)
      {
        char const *yymsgp = YY_("syntax error");
        int yysyntax_error_status;
        yysyntax_error_status = YYSYNTAX_ERROR;
        if (yysyntax_error_status == 0)
          yymsgp = yymsg;
        else if (yysyntax_error_status == 1)
          {
            if (yymsg != yymsgbuf)
              YYSTACK_FREE (yymsg);
            yymsg = (char *) YYSTACK_ALLOC (yymsg_alloc);
            if (!yymsg)
              {
                yymsg = yymsgbuf;
                yymsg_alloc = sizeof yymsgbuf;
                yysyntax_error_status = 2;
              }
            else
              {
                yysyntax_error_status = YYSYNTAX_ERROR;
                yymsgp = yymsg;
              }
          }
        yyerror (yymsgp);
        if (yysyntax_error_status == 2)
          goto yyexhaustedlab;
      }
# undef YYSYNTAX_ERROR
#endif
    }

  yyerror_range[1] = yylloc;

  if (yyerrstatus == 3)
    {
      /* If just tried and failed to reuse lookahead token after an
         error, discard it.  */

      if (yychar <= YYEOF)
        {
          /* Return failure if at end of input.  */
          if (yychar == YYEOF)
            YYABORT;
        }
      else
        {
          yydestruct ("Error: discarding",
                      yytoken, &yylval, &yylloc);
          yychar = YYEMPTY;
        }
    }

  /* Else will try to reuse lookahead token after shifting the error
     token.  */
  goto yyerrlab1;


/*---------------------------------------------------.
| yyerrorlab -- error raised explicitly by YYERROR.  |
`---------------------------------------------------*/
yyerrorlab:

  /* Pacify compilers like GCC when the user code never invokes
     YYERROR and the label yyerrorlab therefore never appears in user
     code.  */
  if (/*CONSTCOND*/ 0)
     goto yyerrorlab;

  yyerror_range[1] = yylsp[1-yylen];
  /* Do not reclaim the symbols of the rule whose action triggered
     this YYERROR.  */
  YYPOPSTACK (yylen);
  yylen = 0;
  YY_STACK_PRINT (yyss, yyssp);
  yystate = *yyssp;
  goto yyerrlab1;


/*-------------------------------------------------------------.
| yyerrlab1 -- common code for both syntax error and YYERROR.  |
`-------------------------------------------------------------*/
yyerrlab1:
  yyerrstatus = 3;      /* Each real token shifted decrements this.  */

  for (;;)
    {
      yyn = yypact[yystate];
      if (!yypact_value_is_default (yyn))
        {
          yyn += YYTERROR;
          if (0 <= yyn && yyn <= YYLAST && yycheck[yyn] == YYTERROR)
            {
              yyn = yytable[yyn];
              if (0 < yyn)
                break;
            }
        }

      /* Pop the current state because it cannot handle the error token.  */
      if (yyssp == yyss)
        YYABORT;

      yyerror_range[1] = *yylsp;
      yydestruct ("Error: popping",
                  yystos[yystate], yyvsp, yylsp);
      YYPOPSTACK (1);
      yystate = *yyssp;
      YY_STACK_PRINT (yyss, yyssp);
    }

  YY_IGNORE_MAYBE_UNINITIALIZED_BEGIN
  *++yyvsp = yylval;
  YY_IGNORE_MAYBE_UNINITIALIZED_END

  yyerror_range[2] = yylloc;
  /* Using YYLLOC is tempting, but would change the location of
     the lookahead.  YYLOC is available though.  */
  YYLLOC_DEFAULT (yyloc, yyerror_range, 2);
  *++yylsp = yyloc;

  /* Shift the error token.  */
  YY_SYMBOL_PRINT ("Shifting", yystos[yyn], yyvsp, yylsp);

  yystate = yyn;
  goto yynewstate;


/*-------------------------------------.
| yyacceptlab -- YYACCEPT comes here.  |
`-------------------------------------*/
yyacceptlab:
  yyresult = 0;
  goto yyreturn;

/*-----------------------------------.
| yyabortlab -- YYABORT comes here.  |
`-----------------------------------*/
yyabortlab:
  yyresult = 1;
  goto yyreturn;

#if !defined yyoverflow || YYERROR_VERBOSE
/*-------------------------------------------------.
| yyexhaustedlab -- memory exhaustion comes here.  |
`-------------------------------------------------*/
yyexhaustedlab:
  yyerror (YY_("memory exhausted"));
  yyresult = 2;
  /* Fall through.  */
#endif

yyreturn:
  if (yychar != YYEMPTY)
    {
      /* Make sure we have latest lookahead translation.  See comments at
         user semantic actions for why this is necessary.  */
      yytoken = YYTRANSLATE (yychar);
      yydestruct ("Cleanup: discarding lookahead",
                  yytoken, &yylval, &yylloc);
    }
  /* Do not reclaim the symbols of the rule whose action triggered
     this YYABORT or YYACCEPT.  */
  YYPOPSTACK (yylen);
  YY_STACK_PRINT (yyss, yyssp);
  while (yyssp != yyss)
    {
      yydestruct ("Cleanup: popping",
                  yystos[*yyssp], yyvsp, yylsp);
      YYPOPSTACK (1);
    }
#ifndef yyoverflow
  if (yyss != yyssa)
    YYSTACK_FREE (yyss);
#endif
#if YYERROR_VERBOSE
  if (yymsg != yymsgbuf)
    YYSTACK_FREE (yymsg);
#endif
  return yyresult;
}
#line 458 "parser.y" /* yacc.c:1906  */

