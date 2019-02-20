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
      p.setLocationRelativeTo(null);
      p.setVisible(true);
    }
    
}

