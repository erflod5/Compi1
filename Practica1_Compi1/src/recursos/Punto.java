package recursos;

public class Punto {
    private int puntox,puntoy;

    public Punto(int puntox, int puntoy) {
        this.puntox = puntox;
        this.puntoy = puntoy;
    }
    
    public Punto()
    {
        this.puntox = 0;
        this.puntoy = 0;
    }

    public int getPuntox() {
        return puntox;
    }

    public void setPuntox(int puntox) {
        this.puntox = puntox;
    }

    public int getPuntoy() {
        return puntoy;
    }

    public void setPuntoy(int puntoy) {
        this.puntoy = puntoy;
    }
    
    
}
