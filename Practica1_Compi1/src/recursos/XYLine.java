
package recursos;

import java.awt.Color;
import java.util.ArrayList;

public class XYLine {

    private String nombre;
    private Color color;
    private int grosor;
    private ArrayList<Punto> punto;
    private ArrayList<Object> caracteristica;

    public XYLine(ArrayList<Object> caracteristica) {
        this.caracteristica = caracteristica;
    }
    
    public XYLine(String nombre, Color color, int grosor, ArrayList<Punto> punto) {
        this.nombre = nombre;
        this.color = color;
        this.grosor = grosor;
        this.punto = punto;
    }
    
    public XYLine(){
        this("",null,0,null);
    }

    public String getNombre() {
        return nombre;
    }

    public void setNombre(String nombre) {
        this.nombre = nombre;
    }

    public Color getColor() {
        return color;
    }

    public void setColor(Color color) {
        this.color = color;
    }

    public int getGrosor() {
        return grosor;
    }

    public void setGrosor(int grosor) {
        this.grosor = grosor;
    }

    public ArrayList<Punto> getPunto() {
        return punto;
    }

    public void setPunto(ArrayList<Punto> punto) {
        this.punto = punto;
    }

    public ArrayList<Object> getCaracteristica() {
        return caracteristica;
    }

    public void setCaracteristica(ArrayList<Object> caracteristica) {
        this.caracteristica = caracteristica;
    }    
}
