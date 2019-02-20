package recursos;

import java.util.LinkedList;

public class Galeria {
    private LinkedList<String> listaGraph;
    private String name;

    public Galeria(LinkedList<String> listaGraph, String name) {
        this.listaGraph = listaGraph;
        this.name = name;
    }
    
    public Galeria(){
        this(null,"");
    }

    public LinkedList<String> getListaGraph() {
        return listaGraph;
    }

    public void setListaGraph(LinkedList<String> listaGraph) {
        this.listaGraph = listaGraph;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }
    
    
}
