package practica1_compi1;

import recursos.GraficaBarras;
import java.io.FileInputStream;
import java.io.*;
import java.util.Iterator;
import java.util.LinkedList;

import org.jfree.chart.ChartFactory;
import org.jfree.chart.JFreeChart;
import org.jfree.chart.plot.PlotOrientation;
import org.jfree.data.category.DefaultCategoryDataset;
import org.jfree.chart.ChartUtilities;
import org.jfree.data.xy.XYSeries;
import org.jfree.data.xy.XYSeriesCollection;
import recursos.Variable;

public class Practica1_Compi1 {

    public static void main(String[] args) throws IOException {
      Principal p = new Principal();
      p.setVisible(true);
    }
    
}

/*            Iterator<Variable> it  = analizador.global.iterator();
            Integer valor = null;
            while(it.hasNext()){
                Variable n = it.next();
                if(n.getNombreVariable().equals("a"))
                    valor = n.getValorEntero();
            }
            if(valor!=null){
                            
            }*/

/*
        final String clase1 = "COMPI 1";
        final String clase2 = "ORGA";
        final String clase3 = "TEO 1";

        final DefaultCategoryDataset dataset = new DefaultCategoryDataset( );
        dataset.addValue( 1.0 , clase1,clase1);
        dataset.addValue( 5.0 , clase2,clase2);
        dataset.addValue( 4.0 , clase3,clase3);

        JFreeChart barChart = ChartFactory.createBarChart(
           "TITULO", 
           "Titulo Eje x", "Titulo Eje y", 
           dataset,PlotOrientation.VERTICAL, 
           true, true, false);
        int width = 640; 
        int height = 480; 
        File BarChart = new File( "nombreArchivo.jpeg" ); 
        ChartUtilities.saveChartAsJPEG( BarChart , barChart , width , height );






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

        File XYChart = new File( "XYLineChart.jpeg" ); 
        ChartUtilities.saveChartAsJPEG( XYChart, xylineChart, width, height);*/