package recursos;
import java.util.ArrayList;

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
                        System.err.println("ERROR: la característica '" + c.getNombre() + "' es inválida");
                        break;
                }
            }
            else if(o instanceof ArrayList<?>){
                try{
                    if(((ArrayList<?>)o).get(0) instanceof Integer)
                    {
                        
                    }
                    else if(((ArrayList<?>)o).get(0) instanceof String)
                    {
                    
                    }
                    if(((ArrayList<?>)o).get(0) instanceof Punto)
                    {
                    
                    }
                }
                catch(NullPointerException ex){
                    System.out.println("NO METAS UN ARRAYLIST VACIO");
                }
                System.out.println(o);
            }
        }
    }

    public void mostrar_caracteristicas() {
        System.out.println("********** " + this.getTitulo() + " **********");
    }
    
}
