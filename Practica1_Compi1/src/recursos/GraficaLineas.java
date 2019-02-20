
package recursos;

import java.util.ArrayList;

public class GraficaLineas extends Grafica{
    
    private ArrayList<XYLine> graph;
    private ArrayList<Object> caracteristica;

    public GraficaLineas(ArrayList<Object> caracteristica) {
        this.caracteristica = caracteristica;
    }
    
    public GraficaLineas(String id, String titulo, String tituloX, String tituloY, ArrayList<XYLine> graph){
        super(id,titulo,tituloX,tituloY);
        this.graph = graph;
    }
    
    public GraficaLineas(){
        this("","","","",null);
    }

    public ArrayList<XYLine> getGraph() {
        return graph;
    }

    public void setGraph(ArrayList<XYLine> graph) {
        this.graph = graph;
    }

    public ArrayList<Object> getCaracteristica() {
        return caracteristica;
    }

    public void setCaracteristica(ArrayList<Object> caracteristica) {
        this.caracteristica = caracteristica;
    }
    
    public void display() {
        System.out.println("Grafica de Lineas");
    }
    
    
}
