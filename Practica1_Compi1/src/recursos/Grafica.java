package recursos;

public abstract class Grafica {

    private String id, titulo, tituloX, tituloY;
    
    public Grafica(){
        this("","","","");
    }
    
    
    public Grafica(String id, String titulo, String tituloX, String tituloY){
        this.id = id;
        this.titulo = titulo;
        this.tituloX = tituloX;
        this.tituloY = tituloY;
    }
   
    public abstract void display();

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getTitulo() {
        return titulo;
    }

    public void setTitulo(String titulo) {
        this.titulo = titulo;
    }

    public String getTituloX() {
        return tituloX;
    }

    public void setTituloX(String tituloX) {
        this.tituloX = tituloX;
    }

    public String getTituloY() {
        return tituloY;
    }

    public void setTituloY(String tituloY) {
        this.tituloY = tituloY;
    }


}
