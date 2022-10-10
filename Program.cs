//Alondra Yocelin Osornio Vega
using System;
using System.IO;

namespace SEMANTICA
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Byte x = 255;
                Console.WriteLine(x);
                x++;
                Console.WriteLine(x);
                Lenguaje a = new Lenguaje();

                a.Programa();



                /*a.match("#");
                a.match("include");
                a.match("<");
                a.match(Token.Tipos.Identificador);
                a.match(".");
                a.match("h");
                a.match(">"); */

                //while(!a.FinArchivo())
                //{
                //  a.NextToken();
                //}
                a.cerrar();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}