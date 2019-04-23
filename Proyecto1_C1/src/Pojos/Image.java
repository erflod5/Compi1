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
public class Image {

    private String path;
    private int alto,ancho;

    public Image() {
    }

    public Image(String path, int alto, int ancho) {
        this.path = path;
        this.alto = alto;
        this.ancho = ancho;
    }

    public String getPath() {
        return path;
    }

    public void setPath(String path) {
        this.path = path;
    }

    public int getAlto() {
        return alto;
    }

    public void setAlto(int alto) {
        this.alto = alto;
    }

    public int getAncho() {
        return ancho;
    }

    public void setAncho(int ancho) {
        this.ancho = ancho;
    }

    public String getHtml(){
        path = path.replaceAll("\""," ");
        return "<image src= \"" + this.path +"\" width=" + this.ancho  + " height=" + this.alto  +  " >" + "</image>";
      }

}
