using System;
using System.IO;

namespace SEMANTICA
{
    public class Error : Exception
    {
        public Error(string mensaje, StreamWriter log) : base(mensaje)
        {
            Console.WriteLine(mensaje);
            log.WriteLine(mensaje);
        }
    }
}