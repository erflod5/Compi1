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
public class TextoA {

    private String texto;

    public TextoA() {
    }

    public TextoA(String texto) {
        this.texto = texto;
    }

    public String getTexto() {
        return texto;
    }

    public void setTexto(String texto) {
        this.texto = texto;
    }

    public String getHtml(){
        return "<h1>" + this.texto + "</h1>";
    }

}
