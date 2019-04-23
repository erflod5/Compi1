/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package Pojos;

/**
 *
 * @author Erik
 */
public class Token {

    private int fila,columna;
    private String lexema,token;

    public Token(){
        this(0,0,"","");
    }

    public Token(int fila, int columna, String lexema, String token) {
        this.fila = fila;
        this.columna = columna;
        this.lexema = lexema;
        this.token = token;
    }

    public int getFila() {
        return fila;
    }

    public void setFila(int fila) {
        this.fila = fila;
    }

    public int getColumna() {
        return columna;
    }

    public void setColumna(int columna) {
        this.columna = columna;
    }

    public String getLexema() {
        return lexema;
    }

    public void setLexema(String lexema) {
        this.lexema = lexema;
    }

    public String getToken() {
        return token;
    }

    public void setToken(String token) {
        this.token = token;
    }


    public String toString(){
        return fila + " " + columna + " " + lexema + " " + token;

    }
}
