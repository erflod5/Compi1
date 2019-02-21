package recursos;
import java.util.ArrayList;
import org.jfree.chart.ChartFactory;
import org.jfree.chart.JFreeChart;
import org.jfree.chart.plot.PlotOrientation;
import org.jfree.data.category.DefaultCategoryDataset;
import static practica1_compi1.Principal.error;
import static practica1_compi1.Principal.errorR;
import static practica1_compi1.Principal.vGlobal;

public class GraficaBarras extends Grafica{
    
    private ArrayList<String> ejeX;
    private ArrayList<Integer> ejeY;
    private ArrayList<Punto> puntos;
    private ArrayList<Object> caracteristica;
    
    public GraficaBarras(ArrayList<Object> caracteristica){
        this.caracteristica = caracteristica;
        
    }
    
    public GraficaBarras(ArrayList<Object> caracteristica,ArrayList<String> ejeX, ArrayList<Integer> ejeY,ArrayList<Punto> puntos){
        this.caracteristica = caracteristica;
        this.ejeX = ejeX;
        this.ejeY = ejeY;
        this.puntos = puntos;
    }
            
    public GraficaBarras(String id, String titulo, String tituloX, String tituloY, ArrayList<String> ejeX, ArrayList<Integer> ejeY,ArrayList<Punto> puntos){
        super(id,titulo,tituloX,tituloY);
        this.ejeX = ejeX;
        this.ejeY = ejeY;
        this.puntos = puntos;
    }
    
    public GraficaBarras(){
        this("","","","",null,null,null);
    }

    public ArrayList<String> getEjeX() {
        return ejeX;
    }

    public void setEjeX(ArrayList<String> ejeX) {
        this.ejeX = ejeX;
    }

    public ArrayList<Integer> getEjeY() {
        return ejeY;
    }

    public void setEjeY(ArrayList<Integer> ejeY) {
        this.ejeY = ejeY;
    }
    
    public ArrayList<Punto> getPuntos() {
        return puntos;
    }

    public void setPuntos(ArrayList<Punto> puntos) {
        this.puntos = puntos;
    }

    public ArrayList<Object> getCaracteristica() {
        return caracteristica;
    }

    public void setCaracteristica(ArrayList<Object> caracteristica) {
        this.caracteristica = caracteristica;
    }
    
    @Override
    public void display() {
        System.out.println("Grafica de Barras");
    }
    
    public String buscarGlobalS(String id,int fila, int columna){
        for(Variable v: vGlobal){
            if(v.getNombreVariable().equalsIgnoreCase(id)){
                return v.getValorString();
            }
        }
        error.add(new Error(fila,columna,"Sintactico","Variable: " + id + " no existe"));
        return "";
    }
    
    public int buscarGlobalI(String id,int fila,int columna){
        for(Variable v: vGlobal){
            if(v.getNombreVariable().equalsIgnoreCase(id)){
                return v.getValorEntero();
            }
        }
        error.add(new Error(fila,columna,"Sintactico","Variable: " + id + " no existe"));
        return 0;
    }
    
    public void validar_caracteristicas() {
        for (Object o : caracteristica) {
            if(o instanceof Caracteristica){
                Caracteristica c = (Caracteristica)o;
                if(c.getGlobal()==1){
                    c.setValor(buscarGlobalS(c.getValor(),c.getFila(),c.getColumna()));
                }
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
                try{
                    if(((ArrayList<?>)o).get(0) instanceof Integer)
                    {
                        this.ejeY = (ArrayList<Integer>)o;
                    }
                    else if(((ArrayList<?>)o).get(0) instanceof String)
                    {
                        this.ejeX = (ArrayList<String>)o;
                    }
                    else if(((ArrayList<?>)o).get(0) instanceof Punto)
                    {
                        this.puntos = (ArrayList<Punto>)o;
                    }
                }
                catch(NullPointerException ex){
                    error.add(new recursos.Error(0,0,"Sintactico","Lista vacia"));
                }
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
    }
    
    public JFreeChart graficar(){
        final DefaultCategoryDataset dataset = new DefaultCategoryDataset( );
        try{
            puntos.stream().forEach((p) -> {
                dataset.addValue(ejeY.get(p.getPuntoy()), ejeX.get(p.getPuntox()), ejeX.get(p.getPuntox()));
            });
        }
        catch(NullPointerException ex){
            error.add(new Error(0,0,"Sintactico","Valor fuera del intervalo"));
            return null;
        }
        JFreeChart barChart = ChartFactory.createBarChart(
           this.getTitulo(), 
           this.getTituloX(), this.getTituloY(), 
           dataset,PlotOrientation.VERTICAL, 
           true, true, false); 
        return barChart;
    }
}
