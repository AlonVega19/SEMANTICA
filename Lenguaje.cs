//Alondra Yocelin Osornio Vega
using System;
using System.Collections.Generic;
//Requerimiento 1: Actualizar el dominante para variables en la expersion.
//                 Ejemplo: float x; char y; y=x
//Requerimiento 2: Actualizar el dominante para el casteo y el valor de la subexpression
//Requerimiento 3: Programar un metodo de conversion de un valor a un tipo de dato 
//                 private float convert(float valor, string TipoDato)
//                 deberan usar el residuo de la division %255, %65535
//Requerimiento 4: Evaluar nuevamente la condicion del if, while, for, do while, con respecto al parametro que reciben
//Requerimiento 5: Levantar una excepcion cuando la captura no sea un numero 
//Requerimiento 6: Ejecutar el For(); 
namespace SEMANTICA
{
    public class Lenguaje : Sintaxis
    {
        List <Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.TipoDato dominante;
        public Lenguaje()
        {

        }
        public Lenguaje(string nombre) : base(nombre)
        {

        }
        private void addVariable(String nombre,Variable.TipoDato tipo)
        {
            variables.Add(new Variable(nombre, tipo));
        }
        private void displayVariables()
        {   
            log.WriteLine();
            log.WriteLine("variable: ");
            foreach (Variable v in variables)
            {
                log.WriteLine(v.getNombre()+" "+v.getTipo()+" "+v.getValor());
            }
        }
        private bool existeVariable(string nombre)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    return true;
                }
            }
            return false;
        }
        private void modificaValor(string nombre, float nuevoValor)
        {
            foreach (Variable v in variables)
            {
                if(v.getNombre().Equals(nombre))
                {
                    v.setValor(nuevoValor);
                }
            }
        }
        private float convert(float valor, Variable.TipoDato tipo)
        {
            if(dominante == Variable.TipoDato.Char && valor > 255)
            {
                valor = valor%256;
                return valor;
            } else if(dominante == Variable.TipoDato.Int && valor > 65535)
            {
                valor = valor%65536;
                return valor;
            }
            else
            {
                return valor;
            }
        }
        private float getValor(string nombreVariable)
        {
            //Requerimiento 4.- Obtener el valor de la variable cuando se requiera  y programar el método getValor
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombreVariable))
                {
                    return v.getValor();
                }
            }
            return 0;
        }
        private Variable.TipoDato getTipo(string nombreVariable)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombreVariable))
                {
                    return v.getTipo();
                }
            }
            return Variable.TipoDato.Char;
        }
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            Libreria();
            Variables();
            Main();
            displayVariables();
        }
        //Librerias -> #include<identificador(.h)?> Librerias?
        private void Libreria()
        {
            if (getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(Tipos.Identificador);
                if (getContenido() == ".")
                {
                    match(".");
                    match("h");
                }
                match(">");
                Libreria();
            }
        }
        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variable.TipoDato tipo = Variable.TipoDato.Char; 
                switch (getContenido())
                {
                    case "int": tipo = Variable.TipoDato.Int; break;
                    case "float": tipo = Variable.TipoDato.Float; break;
                }
                match(Tipos.TipoDato);
                Lista_identificadores(tipo);
                match(Tipos.FinSentencia);
                Variables();
            }
        }
        //Lista_identificadores -> identificador (,Lista_identificadores)?
        private void Lista_identificadores(Variable.TipoDato tipo)
        {
            if (getClasificacion() == Token.Tipos.Identificador)
            {
                if (!existeVariable(getContenido()))
                {
                    addVariable(getContenido(), tipo);
                }
                else
                {
                    throw new Error("Error, variable duplicada" +getContenido()+" en la linea: "+linea, log);
                }
            }
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                Lista_identificadores(tipo);
            }
        }
        //Main      -> void main() Bloque de instrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones(true);
        }
        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }    
            match("}"); 
        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
        }
        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "case" && getContenido() !=  "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion);
            }
        }
        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion)
        {
            if (getContenido() == "printf")
                Printf(evaluacion);
            else if (getContenido() == "scanf")
                Scanf(evaluacion);
            else if (getContenido() == "if")
                If(evaluacion);
            else if (getContenido() == "while")
                While(evaluacion);
            else if (getContenido() == "do")
                Do(evaluacion);
            else if (getContenido() == "for")
                For(evaluacion);
            else if (getContenido() == "switch")
                Switch(evaluacion);
            else
            {
                Asignacion(evaluacion);
            }
        }
        private Variable.TipoDato evaluanumero(float resultado)
        {
            if(resultado % 1 != 0)
            {
                return Variable.TipoDato.Float;
            }
            if(resultado <= 255)
            {
                return Variable.TipoDato.Char;
            }
            else if(resultado <= 65535)
            {
                return Variable.TipoDato.Int;
            }
            return Variable.TipoDato.Float;
        }
        private bool evaluasemantica(string variable, float resultado)
        {
            Variable.TipoDato tipoDato = getTipo(variable);
            return false;
        }
        //Asignacion -> identificador = cadena | Expresion;
        private void Asignacion(bool evaluacion)
        {
            //Requerimiento 2.- Si no existe la variable levanta la excepcion
            if(getClasificacion()== Tipos.Identificador)
            {
                if(!existeVariable(getContenido()))
                {
                    throw new Error("Error de sintais, la variable no existe: <"+ getContenido() + "> es inexistente en linea:"+ linea, log);
                }
            }
            log.WriteLine();
            log.Write(getContenido()+" = ");
            string nombreVariable = getContenido();
            match(Tipos.Identificador);
            match(Tipos.Asignacion);
            dominante = Variable.TipoDato.Char;
            Expresion();
            match(";");
            float resultado = stack.Pop();
            log.Write("= " + resultado);
            log.WriteLine();
            if (dominante < evaluanumero(resultado))
            {
                dominante = evaluanumero(resultado);
            }
            if(dominante <= getTipo(nombreVariable))
            {
                if (evaluacion)
                {
                    modificaValor(nombreVariable, resultado);
                }
            }
            else 
            {
                 throw new Error("Error de semantica: no podemos asignar un: <" +dominante + "> a un <" + getTipo(nombreVariable) +  "> en linea  " + linea, log);
            }
        }
        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion)
        {
            match("while");
            match("(");
            bool validarWhile = Condicion();
            //Requerimiento 4
            if(!evaluacion)
            {
                validarWhile = false;
            }
            match(")");
            if(getContenido() == "{") 
            {
                BloqueInstrucciones(validarWhile);
            }
            else
            {
                Instruccion(validarWhile);
            }
        }
        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion)
        {
            bool validarDo = evaluacion;
            match("do");
            if(getContenido() == "{")
            {
                BloqueInstrucciones(validarDo);
            }
            else
            {
                Instruccion(validarDo);
            } 
            match("while");
            match("(");
            //Requerimiento 4
            validarDo = Condicion();
            if(!evaluacion)
            {
                validarDo = false;
            }
            match(")");
            match(";");
        }

        public void setPosicion(long posicion)
        {
            archivo.DiscardBufferedData();
            archivo.BaseStream.Seek(posicion, SeekOrigin.Begin);
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion)
        {
            match("for");
            match("(");
            Asignacion(evaluacion);
            //Requerimiento 4:
            //Requerimiento 6:
            //                  a)Necesito guardar la posicion del archivo de texto
            string variable= "";
            bool validarFor;
            int pos = posicion;
            int lin = linea;
            //                  b)Agregar un ciclo while
            do
            {
                
                validarFor = Condicion();
                if(!evaluacion)
                {
                    validarFor = false;
                }
                match(";");
                variable = getContenido();
                float valor = Incremento1(validarFor);
                match(")");
                if(getContenido() == "{")
                {
                    BloqueInstrucciones(validarFor);  
                }
                else
                {
                    Instruccion(validarFor);
                }
                if(validarFor)
                {
                    posicion = pos - variable.Length;
                    linea = lin;
                    setPosicion(posicion);
                    NextToken();
                }
                //              c)Regresar a la posicion de lectura del archivo
                //              d)Sacar otro token
                modificaValor(variable, valor);
            }while(validarFor);
            
        }
        //Incremento -> Identificador ++ | --
        private void Incremento(bool evaluacion)
        {
            string variable = getContenido();
            //Requerimiento 2.- Si no existe la variable levanta la excepcion
            if(!existeVariable(getContenido()))
            {
                throw new Error("Error de sintais, variable no existe: <"+ getContenido() + "> es inexistente en linea:"+ linea, log);
            }
            match(Tipos.Identificador);
            if (getClasificacion() == Tipos.IncrementoTermino)
            {
                if (getContenido()[0] == '+')
                {
                    match("++");
                    if (evaluacion)
                    {
                        modificaValor(variable, getValor(variable) + 1);
                    }
                }
                else
                {
                    match("--");
                    if (evaluacion)
                    {
                        modificaValor(variable, getValor(variable) - 1);
                    }
                }
            }
            else
            {
                match(Tipos.IncrementoTermino);
            }
        }
        private float Incremento1(bool evaluacion)
        {
            string variable = getContenido();
            //Requerimiento 2.- Si no existe la variable levanta la excepcion
            if(!existeVariable(getContenido()))
            {
                throw new Error("Error de sintais, variable no existe: <"+ getContenido() + "> es inexistente en linea:"+ linea, log);
            }
            match(Tipos.Identificador);
            if (getClasificacion() == Tipos.IncrementoTermino)
            {
                if (getContenido()[0] == '+')
                {
                    match("++");
                    if (evaluacion)
                    {
                        return getValor(variable) + 1;
                    }
                }
                else
                {
                    match("--");
                    if (evaluacion)
                    {
                       return getValor(variable) - 1;
                    }
                }
            }
            else
            {
                match(Tipos.IncrementoTermino);
            }
            return 0;
        }
        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion)
        {
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            match(")");
            match("{");
            ListaDeCasos(evaluacion);
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() != "}" && getContenido() != "{")
                    ListaInstruccionesCase(evaluacion);
                else if (getContenido() == "{")
                    BloqueInstrucciones(evaluacion);
                if (getContenido() == "break")
                {
                    match("break");
                    match(";");
                }
            }
            match("}");
        }
        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion)
        {
            if (getContenido() != "}" && getContenido() != "default")
            {
                match("case");
                Expresion();
                stack.Pop();
                match(":");
                if (getContenido() != "case" && getContenido() != "{")
                    ListaInstruccionesCase(evaluacion);
                else if (getContenido() == "{")
                    ListaInstruccionesCase(evaluacion);
                if (getContenido() == "break")
                {
                    match("break");
                    match(";");
                }
                ListaDeCasos(evaluacion);
            }
        }
        // Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion()
        {
            Expresion();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float e2 = stack.Pop();
            float e1 = stack.Pop();
            switch(operador)
            {
                case "==":
                    return e1 == e2;
                case ">":
                    return e1 > e2;
                case ">=":
                    return e1 >= e2;
                case "<":
                    return e1 < e2;
                case "<=":
                    return e1 <= e2;
                default: 
                return e1 != e2;
                    
            }   
        }
        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion)
        {
            match("if");
            match("(");
            //Requerimiento 4
            bool validarIf = Condicion();
            if(!evaluacion)
            {
                validarIf = false;
            }
            match(")");
            if(getContenido() == "{")
            {
                BloqueInstrucciones(validarIf);          
            }
            else
            {
                Instruccion(validarIf);  
            }
            if(getContenido() == "else")
            {
                match("else");
                //Requerimiento 4 Se debe comportar alravez
                if(getContenido() == "{")
                {
                    if(evaluacion)
                    {
                        BloqueInstrucciones(!validarIf);
                    }
                    else
                    {
                        BloqueInstrucciones(evaluacion);
                    }
                }
                else
                {
                    if(evaluacion)
                    {
                        Instruccion(!validarIf);
                    }
                    else
                    {
                        Instruccion(evaluacion);
                    }
                }
            }
        }
        //Printf -> printf(cadena | expresion);
        private void Printf(bool evaluacion)
        {
            match("printf");
            match("(");
            if (getClasificacion() == Tipos.Cadena)
            {
                string comilla = getContenido();
                if ((evaluacion))
                {
                    comilla = comilla.Replace("\\n" , "\n");
                    comilla = comilla.Replace("\\t" , "\t");
                    Console.Write(comilla.Substring(1, comilla.Length - 2));
                    //Console.Write(comilla);
                }
                match(Tipos.Cadena);
            }
            else
            {
                Expresion();
                float resultado = stack.Pop();
                if (evaluacion)
                {
                    Console.Write(resultado);
                }
            }
            match(")");
            match(";");
        }
        //Scanf -> scanf(cadena, &Identificador);
        private void Scanf(bool evaluacion)    
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            if(!existeVariable(getContenido()))
            {
                throw new Error("Error de sintais, variable solicitada: <"+ getContenido() + "> es inexistente en linea:"+ linea, log);
            }
            if(evaluacion)
            {   
                //Requerimiento 2.- Si no existe la variable levanta la excepcion
                string val = ""+Console.ReadLine(); 
                //modificaValor (nombreVariable, valorFloat);
                //Requerimiento 5._ MOdificar el valor de la variable
                double validaVal;
                if(!double.TryParse(val, out validaVal))
                {
                    throw new Error("Error de sintaxis, se espera un numero en linea: "+linea, log);
                }
                float valorFloat = float.Parse(val);
                modificaValor(getContenido(), valorFloat);
            }         
            match(Tipos.Identificador);
            match(")");
            match(";");
        }
        
        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                switch (operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                switch (operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido() + " ");
                if(dominante < evaluanumero(float.Parse(getContenido())))
                {
                    dominante = evaluanumero(float.Parse(getContenido()));
                }
                stack.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                //Requerimiento 2. Si no existe la variable, se levanta la excepción.
                if (!existeVariable(getContenido()))
                {
                    throw new Error("Error de sintáxis: Variable no existe \"" + getContenido() + "\" en la línea " + linea + ".", log);
                }
                log.Write(getContenido() + " ");
                //Requerimiento 1 : Es con un if como ese
                if(dominante < getTipo(getContenido()))
                {
                    dominante = getTipo(getContenido());
                }
                stack.Push(getValor(getContenido()));
                match(Tipos.Identificador);
            }
            else
            {
                bool hubocasteo = false;
                Variable.TipoDato casteo = Variable.TipoDato.Char;
                match("(");
                if (getClasificacion() == Tipos.TipoDato)
                {
                    hubocasteo = true;
                    switch(getContenido())
                    {
                        case "char":
                            casteo = Variable.TipoDato.Char;
                            break;
                        case "int":
                            casteo = Variable.TipoDato.Int;
                            break;
                        case "float":
                            casteo = Variable.TipoDato.Float;
                            break;
                    }
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
                if (hubocasteo)
                {
                    //Requerimiento 2: Actualizar dominande en base a casteo
                    //Saco un elemnto del satck
                    //Convierto ese valor al equivalente en casteo
                    //Requerimiento 3:
                    //Ejemplo: si el casteo es char y el Pop regresa un 256
                    //el valor equivalente en casteo es 0
                    dominante = casteo;
                    float valor = stack.Pop();
                    if(valor%1 !=0 && dominante != Variable.TipoDato.Float)
                    {
                        valor = MathF.Truncate(valor);
                    }
                    valor = convert(valor, dominante);
                    stack.Push(valor);
                }
            }
        }
    }
}