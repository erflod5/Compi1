
package recursos;

import java.util.ArrayList;
import org.jfree.chart.ChartFactory;
import org.jfree.chart.JFreeChart;
import org.jfree.chart.plot.PlotOrientation;
import org.jfree.data.xy.XYSeries;
import org.jfree.data.xy.XYSeriesCollection;
import static practica1_compi1.Principal.error;


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
                        error.add(new recursos.Error(c.getFila(),c.getColumna(),"Sintactico","Caracteristia: " + c.getNombre() + " no reconocida"));
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
                                x.setColor(c.getValor());
                                break;
                            case "grosor":
                                x.setGrosor(Integer.parseInt(c.getValor()));
                                break;
                            default:
                                error.add(new recursos.Error(c.getFila(),c.getColumna(),"Sintactico","Caracteristia: " + c.getNombre() + " no reconocida"));
                                break;
                        }
                    }
                    else if (p instanceof ArrayList<?>){
                        x.setPunto((ArrayList<Punto>) p);
                        /*for(Object o1: (ArrayList<Object>)p){
                            Punto pt = (Punto)o1;
                        }*/
                    }
                });
            }
        }  
    }
    
    public JFreeChart graficar(){
        final XYSeries firefox = new XYSeries( "Firefox" );
        firefox.add( 1.0 , 1.0 );
        firefox.add( 2.0 , 4.0 );
        firefox.add( 3.0 , 3.0 );

        final XYSeries chrome = new XYSeries( "Chrome" );
        chrome.add( 1.0 , 4.0 );
        chrome.add( 2.0 , 5.0 );
        chrome.add( 3.0 , 6.0 );

        final XYSeries iexplorer = new XYSeries( "InternetExplorer" );
        iexplorer.add( 3.0 , 4.0 );
        iexplorer.add( 4.0 , 5.0 );
        iexplorer.add( 5.0 , 4.0 );

        final XYSeriesCollection dataset1 = new XYSeriesCollection( );
        dataset1.addSeries( firefox );
        dataset1.addSeries( chrome );
        dataset1.addSeries( iexplorer );

        JFreeChart xylineChart = ChartFactory.createXYLineChart(
           "titulo", 
           "titulo x",
           "titulo y", 
           dataset1,
           PlotOrientation.VERTICAL, 
           true, true, false);

        return xylineChart;
    }
}
