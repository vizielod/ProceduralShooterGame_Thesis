using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorFunction : MonoBehaviour
{
    public static double Erf(double x)
    {
        // constants
        double a1 = 0.254829592;
        double a2 = -0.284496736;
        double a3 = 1.421413741;
        double a4 = -1.453152027;
        double a5 = 1.061405429;
        double p = 0.3275911;
 
        // Save the sign of x
        int sign = 1;
        if (x < 0)
            sign = -1;
        x = Math.Abs(x);
 
        // A&S formula 7.1.26
        double t = 1.0 / (1.0 + p*x);
        double y = 1.0 - (((((a5*t + a4)*t) + a3)*t + a2)*t + a1)*t*Math.Exp(-x*x);
 
        return sign*y;
    }
 
    public static double TestErf()
    {
        // Select a few input values
        double[] x = 
        {
            -3, 
            -1, 
            0.0, 
            0.5, 
            2.1 
        };
 
        // Output computed by Mathematica
        // y = Erf[x]
        double[] y = 
        { 
            -0.999977909503, 
            -0.842700792950, 
            0.0, 
            0.520499877813, 
            0.997020533344 
        };
 
        double maxError = 0.0;
        for (int i = 0; i < x.Length; ++i)
        {
            double error = Math.Abs(y[i] - Erf(x[i]));
            if (error > maxError)
                maxError = error;
        }
 
        //Console.WriteLine("Maximum error: {0}", maxError);
        //Debug.Log("Maximum error: {0}", maxError);

        return maxError;
    }    
}
