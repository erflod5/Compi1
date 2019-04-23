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
public class Parrafo {

    private String texto, alineacion;

    public Parrafo(String texto, String alineacion) {
        this.texto = texto;
        this.alineacion = alineacion.replaceAll("\"","");
    }

    public Parrafo(String texto){
        this.texto = texto;
        this.alineacion = "justificado";
    }

    public Parrafo() {
    }

    public String getTexto() {
        return texto;
    }

    public void setTexto(String texto) {
        this.texto = texto;
    }

    public String getAlineacion() {
        return alineacion;
    }

    public void setAlineacion(String alineacion) {
        this.alineacion = alineacion;
    }

    public String getHtml(){
        if(this.alineacion.equalsIgnoreCase("justificado")){
          return "<p align = \"justify\">" + this.texto + "</p>";
        }
        else if(this.alineacion.equalsIgnoreCase("centrado")){
          return "<p align = \"center\">" + this.texto + "</p>";
        }
        else if(this.alineacion.equalsIgnoreCase("derecha")){
          return "<p align = \"right\">" + this.texto + "</p>";
        }
        else if(this.alineacion.equalsIgnoreCase("izquierda")){
          return "<p align = \"left\">" + this.texto + "</p>";
        }
        else{
          return "<p align = \"left\">" + this.texto + "</p>";
        }
    }

}
