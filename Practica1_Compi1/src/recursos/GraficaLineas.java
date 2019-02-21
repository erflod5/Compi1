
package recursos;

import java.awt.BasicStroke;
import java.awt.Color;
import java.util.ArrayList;
import org.jfree.chart.ChartFactory;
import org.jfree.chart.JFreeChart;
import org.jfree.chart.plot.PlotOrientation;
import org.jfree.chart.plot.XYPlot;
import org.jfree.chart.renderer.xy.XYLineAndShapeRenderer;
import org.jfree.data.xy.XYSeries;
import org.jfree.data.xy.XYSeriesCollection;
import static practica1_compi1.Principal.error;
import static practica1_compi1.Principal.errorR;


public class GraficaLineas extends Grafica{
    
    private ArrayList<XYLine> graph;
    private ArrayList<Object> caracteristica;

    public GraficaLineas(ArrayList<Object> caracteristica) {
        this.caracteristica = caracteristica;
        graph = new ArrayList<>();
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
    
    public void validar_caracteristicas() {
        for (Object o : caracteristica) {
            if(o instanceof Caracteristica){
                Caracteristica c = (Caracteristica)o;
                switch (c.getNombre().toLowerCase()) {    
                    case "id":
                        this.setId(c.getValor());
                        break;
                    case "titulo":
                        this.setTitulo(c.getValor());
                        break;
                    case "titulox":
                        this.setTituloX(c.getValor());
                        break;
                    case "tituloy":
                        this.setTituloY(c.getValor());
                        break;
                    default:
                        errorR.add(new recursos.Error(c.getFila(),c.getColumna(),"Sintactico","Caracteristica: " + c.getNombre() + " no reconocida"));
                        break;
                }
            }
            else if(o instanceof ArrayList<?>){
                ArrayList<Object> n = (ArrayList<Object>)o;
                XYLine x = new XYLine();
                n.stream().forEach((Object p) -> {
                    if(p instanceof Caracteristica){
                        Caracteristica c = (Caracteristica)p;
                        switch (c.getNombre().toLowerCase()){
                            case "nombre":
                                x.setNombre(c.getValor());
                                break;
                            case "color":
                                x.setColor(getColor(c.getValor()));
                                break;
                            case "grosor":
                                x.setGrosor(Integer.parseInt(c.getValor()));
                                break;
                            default:
                                errorR.add(new recursos.Error(c.getFila(),c.getColumna(),"Sintactico","Caracteristica: " + c.getNombre() + " no reconocida"));
                                break;
                        }
                    }
                    else if (p instanceof ArrayList<?>){
                        x.setPunto((ArrayList<Punto>) p);
                    }
                });
                this.graph.add(x);
            }
        }
        validarC();
    }
    
    private void validarC(){
        if(this.getTitulo().equals("")){
            errorR.add(new Error(0,0,"Sintactico","Titulo no definido"));
            this.setTitulo("Default");
        }
        if(this.getTituloX().equals("")){                        
            errorR.add(new Error(0,0,"Sintactico","Titulo X no definido"));
            this.setTituloX("Default");
        }
        if(this.getTituloY().equals("")){
            errorR.add(new Error(0,0,"Sintactico","Titulo Y no definido"));
            this.setTituloY("Default");
        }
        if(this.getId().equals("")){
            errorR.add(new Error(0,0,"Sintactico","Id no definido"));            
            this.setId("Default");
        }
        for(XYLine xyl: graph){
            if(xyl.getNombre().equals("")){
                errorR.add(new Error(0,0,"Sintactico","Nombre no definido"));
                xyl.setNombre("Default");
            }
            if(xyl.getGrosor()==0){
                errorR.add(new Error(0,0,"Sintactico","Grosor no definido"));
                xyl.setGrosor(1);
            }
            if(xyl.getColor()==null){
                errorR.add(new Error(0,0,"sintactico","color no definido"));
                xyl.setColor(Color.WHITE);
            }
        }
    }
    
    private Color getColor(String nombre){
        switch(nombre.toLowerCase()){
            case "rojo":
                return Color.RED;
            case "amarillo":
                return Color.YELLOW;
            case "naranja":
                return Color.ORANGE;
            case "azul":
                return Color.BLUE;
            case "negro":
                return Color.BLACK;
            case "verde":
                return Color.GREEN;
            default:
                errorR.add(new Error(0,0,"Sintactico","Color no reconocido"));
                return Color.WHITE;
        }
    }
    
    public JFreeChart graficar(){
        
        final XYSeriesCollection ds = new XYSeriesCollection();
        XYLineAndShapeRenderer renderer = new XYLineAndShapeRenderer();
        int c=0;
        for(XYLine xyl: graph){
            final XYSeries n = new XYSeries(xyl.getNombre());
            for(Punto p: xyl.getPunto()){
                n.add(p.getPuntox(),p.getPuntoy());
            }
            renderer.setSeriesPaint(c, xyl.getColor());
            renderer.setSeriesStroke(c, new BasicStroke(xyl.getGrosor()));
            c++;
            ds.addSeries(n);
        }
        
        JFreeChart xylineChart = ChartFactory.createXYLineChart(
           this.getTitulo(), 
           this.getTituloX(),
           this.getTituloY(), 
           ds,
           PlotOrientation.VERTICAL, 
           true, true, false);
        XYPlot plot = xylineChart.getXYPlot();
        plot.setRenderer(renderer);
        return xylineChart;
    }
}
