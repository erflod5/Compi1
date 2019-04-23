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
public class TextoB {

    private String texto;

    public TextoB() {
    }

    public TextoB(String texto) {
        this.texto = texto;
    }

    public String getTexto() {
        return texto;
    }

    public void setTexto(String texto) {
        this.texto = texto;
    }

    public String getHtml(){
        return "<h3>" + this.texto + "</h3>";
    }

}
