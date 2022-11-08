//Alondra Yocelin Osornio Vega
using System;
using System.Collections.Generic;
//Requerimiento 1: Actualizacion:
//                 a)agregar el residuo de la division en porfactor       <3
//                 b)agregar en instruccion los incrementos de termino y los incrementos de factor
//                   a++, a--, a+=1, a-=1, a*=1, a/=1, a%=1
//                   en donde el uno puede ser una expresion              <3
//                 c)programar el destructor para ejecutar el metodo cerrarArchivo()
//                  #libreria especial
//Requerimiento 2:    
//                 a)Marcar errores semanticos cuando los incrementos de termino o incrementos de factor
//                   superen el rango de la variable                      <3
//                 b)Considerar el inciso b y c para el for
//                 c)Que funcione el do y el while                        <3
//Requerimiento 3:
//                 a)Considerar las variables y los casteos de las expresiones matematicas en ensamblador 
//                 b)Considerar el residuo de la division en assembler    <3    
//                 c)Programar el printf y scanf en ensamblador           <3
//Requerimiento 4: 
//                 a) Programar el else en ensamblador
//                 b) Programar el for en ensamblador
//Requerimiento 5: 
//                 a) Programar el do en ensamblador
//                 b) Programar el do while en ensamblador

namespace SEMANTICA
{
    public class Lenguaje : Sintaxis
    {
        List <Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.TipoDato dominante;
        int cIf;
        int cFor;
        public Lenguaje()
        {
            cIf = 0;
            cIf = cFor = 0;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            cIf = 0;
            cIf = cFor = 0;
        }

        ~Lenguaje()
        {
            Console.WriteLine("Destructor");
            cerrar();
        }

        private void addVariable(String nombre,Variable.TipoDato tipo)
        {
            variables.Add(new Variable(nombre, tipo));
        }

        private void displayVariables()
        {
            log.WriteLine();
            log.WriteLine("Variables: ");
            foreach(Variable v in variables)
            {
                log.WriteLine(v.getNombre()+" "+v.getTipo()+" "+v.getValor());
            }
        }
        private void variablesAsm(){
            asm.WriteLine("Variables: ");
            foreach(Variable v in variables){
                asm.WriteLine("\t" + v.getNombre()+" DW ? ");
            }

        }

        private bool existeVariable(string nombre)
        {
            foreach(Variable v in variables)
            {
                if(v.getNombre().Equals(nombre))
                {
                    return true;
                }
            }
            return false;
        }

        private void modVariable(string nombre, float nuevoValor)
        {
            foreach(Variable v in variables)
            {
                if(v.getNombre().Equals(nombre))
                {
                    v.setValor(nuevoValor);
                }
            }
        }
        
        private float convert(float valor, Variable.TipoDato tipo)
        {
            if(dominante == Variable.TipoDato.Char)
            {
                valor = (char)(valor)%256;
                return valor;
            } else if(dominante == Variable.TipoDato.Int)
            {
                valor = (int)(valor)%65536;
                return valor;
            }
            else
            {
                return valor;
            }
        }

        private float getValor(string nombreVariable)
        {
            foreach(Variable v in variables)
            {
                if(v.getNombre().Equals(nombreVariable))
                {
                    return v.getValor();
                }
            }
            return 0;
        }

        private Variable.TipoDato getTipo(string nombreVariable)
        {
            foreach(Variable v in variables)
            {
                if(v.getNombre().Equals(nombreVariable))
                {
                    return v.getTipo();
                }
            }
            return Variable.TipoDato.Char;
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        { 
            asm.WriteLine("#make COM");
            asm.WriteLine("include 'emu8086.inc'");
            asm.WriteLine("ORG 100h");
            Libreria();
            Variables();
            variablesAsm();
            Main();
            displayVariables();
            asm.WriteLine("RET");
            asm.WriteLine("DEFINE_SCAN_NUM");
        }

        //Librerias -> #include<identificador(.h)?> Librerias?
        private void Libreria()
        {
            if(getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(Tipos.Identificador);
                if(getContenido() == ".")
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
            if(getClasificacion() == Tipos.TipoDato)
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
            if(getClasificacion() == Tipos.Identificador)
            {
                if(!existeVariable(getContenido()))
                {
                    addVariable(getContenido(), tipo);
                }
                else
                {
                    throw new Error("Error de sintaxis, variable duplicada <" +getContenido()+"> en linea: "+linea, log);
                }
            }
            match(Tipos.Identificador);
            if(getContenido() == ",")
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
            BloqueInstrucciones(true, true);
        }

        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion, bool imprime)
        {
            match("{");
            if(getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, imprime);
            }    
            match("}"); 
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion, bool imprime)
        {
            Instruccion(evaluacion, imprime);;
            if(getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, imprime);
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion, bool imprime)
        {
            Instruccion(evaluacion, imprime);
            if(getContenido() != "case" && getContenido() !=  "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion, imprime);
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion, bool imprime)
        {
            if(getContenido() == "printf")
            {
                Printf(evaluacion, imprime);
            }
            else if(getContenido() == "scanf")
            {
                Scanf(evaluacion, imprime);
            }
            else if(getContenido() == "if")
            {
                If(evaluacion, imprime);
            }
            else if(getContenido() == "while")
            {
                While(evaluacion, imprime);
            }
            else if(getContenido() == "do")
            {
                Do(evaluacion, imprime);
            }
            else if(getContenido() == "for")
            {
                For(evaluacion, imprime);
            }
            else if(getContenido() == "switch")
            {
                Switch(evaluacion, imprime);
            }
            else
            {
                Asignacion(evaluacion, imprime);
            }
        }

        private Variable.TipoDato evaluaNumero(float resultado)
        {
            if(resultado%1 != 0)
            {
                return Variable.TipoDato.Float;
            }
            if(resultado<=255)
            {
                return Variable.TipoDato.Char;
            }
            else if(resultado<=65535)
            {
                return Variable.TipoDato.Int;
            }
            return Variable.TipoDato.Float;
        }

        private bool evaluaSemantica(string variable, float resultado)
        {
            Variable.TipoDato tipoDato = getTipo(variable);
            return false;
        }

        //Asignacion -> identificador = cadena | Expresion;
        private void Asignacion(bool evaluacion, bool imprime)
        {
            if(!existeVariable(getContenido()))
            {
                throw new Error("Error de sintaxis, variable inexistente <" +getContenido()+"> en linea: "+linea, log);
            }
            log.WriteLine();
            log.Write(getContenido() + " = ");
            string nombre = getContenido();
            match(Tipos.Identificador); 
            dominante = Variable.TipoDato.Char;
            if(getClasificacion() == Tipos.IncrementoTermino || getClasificacion() == Tipos.IncrementoFactor)
            {
                //Requerimiento 1.b
                float resultado = getValor(nombre);
                match(";");
                if(imprime)
                {
                    asm.WriteLine("Pop AX");
                }
                log.Write("= " + resultado);
                log.WriteLine();
                if(dominante < evaluaNumero(resultado))
                {
                    dominante = evaluaNumero(resultado);
                }
                if(dominante <= getTipo(nombre))
                {
                    if(evaluacion)
                    {
                        modVariable(nombre, resultado);
                    }
                }
                else
                {
                    throw new Error("Error de semantica, no podemos asignar un <" + dominante + "> a un <" + getTipo(nombre) + "> en la linea " + linea, log);
                }
                if(imprime)
                {
                    asm.WriteLine("Mov " + nombre + ", AX");
                }
            }
            else
            {
                match(Tipos.Asignacion);
                Expresion(imprime);
                match(";");
                float resultado = stack.Pop();
                if(imprime)
                {
                    asm.WriteLine("Pop AX");
                }
                log.Write("= "+resultado);
                log.WriteLine();
                if(dominante < evaluaNumero(resultado))
                {
                    dominante = evaluaNumero(resultado);
                }
                if(dominante <= getTipo(nombre))
                {
                    if(evaluacion)
                    {
                        modVariable(nombre, resultado);
                    }
                }
                else
                {
                    throw new Error("Error de semantica, no podemos asignar un <" + dominante + "> a un <" + getTipo(nombre) + "> en la linea " + linea, log);
                }
                if(imprime)
                {
                    asm.WriteLine("Mov " + nombre + ", AX");
                }
            }
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion, bool imprime)
        {
            match("while");
            match("(");
            string variable = getContenido();
            if(!existeVariable(getContenido()))
                throw new Error("Error de sintaxis, variable inexistente <" +getContenido()+"> en linea: "+linea, log);
            bool validarWhile;
            int pos = posicion;
            int lin = linea;
            do
            {
                validarWhile = Condicion("", imprime);            
                if(!evaluacion)
                {
                    validarWhile = false;
                }
                match(")");
                if(getContenido() == "{") 
                {
                    BloqueInstrucciones(validarWhile, imprime);
                }
                else
                {
                    Instruccion(validarWhile, imprime);
                }
                if(validarWhile)
                {
                    posicion = pos - variable.Length;
                    linea = lin;
                    setPosicion(posicion);
                    NextToken();
                }
            }while(validarWhile);
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion, bool imprime)
        {
            match("do");
            
            bool validarDo = evaluacion;
            int pos = posicion;
            int lin = linea;
            do
            {
                if(getContenido() == "{")
                {
                    BloqueInstrucciones(validarDo, imprime);
                }
                else
                {
                    Instruccion(validarDo, imprime);
                } 
                match("while");
                match("(");
                string variable = getContenido();
                if(!existeVariable(getContenido()))
                    throw new Error("Error de sintaxis, variable inexistente <" +getContenido()+"> en linea: "+linea, log);
                validarDo = Condicion("", imprime);
                if(!evaluacion)
                {
                    validarDo = false;
                }
                match(")");
                match(";");
                if(validarDo)
                {
                    posicion = pos - variable.Length;
                    linea = lin;
                    setPosicion(posicion);
                    NextToken();
                }
            }while(validarDo);
        }

        public void setPosicion(long posicion)
        {
            archivo.DiscardBufferedData();
            archivo.BaseStream.Seek(posicion, SeekOrigin.Begin);
        }

        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion, bool imprime)
        {
            string etiquetaInicioFor = "inicioFor" + cFor;
            string etiquetaFinFor = "finFor" + cFor++;
            
            match("for");
            match("(");
            Asignacion(evaluacion, imprime);
            string variable = getContenido();
            if(!existeVariable(getContenido()))
                throw new Error("Error de sintaxis, variable inexistente <" +getContenido()+"> en linea: "+linea, log);
            bool validarFor;
            int pos = posicion;
            int lin = linea;
            do
            {
                if(imprime)
                {
                    asm.WriteLine(etiquetaInicioFor + ":");
                }
                validarFor = Condicion("", imprime);
                if(!evaluacion)
                {
                    validarFor = false;
                }
                match(";");
                //Requerimiento 1.d
                match(Tipos.Identificador);
                float resultado = Incremento(variable, imprime);
                match(")");
                if(getContenido() == "{")
                {
                    BloqueInstrucciones(validarFor, imprime);  
                }
                else
                {
                    Instruccion(validarFor, imprime);
                }
                if(validarFor)
                {
                    posicion = pos - variable.Length;
                    linea = lin;
                    setPosicion(posicion);
                    NextToken();
                }
                if(evaluacion)
                {
                    modVariable(variable, resultado);
                }
            }while(validarFor);
            if(imprime)
            {
                asm.WriteLine(etiquetaFinFor + ":");
            }
        }

        //Incremento -> Identificador ++ | --
        private float Incremento(string nombre, bool imprime)
        {
            float resultado = getValor(nombre);
            switch(getContenido())
            {
                case "++":
                    match(Tipos.IncrementoTermino);
                    resultado++;
                    if(imprime)
                    {
                        asm.WriteLine("Inc " + nombre);
                    }
                    break;
                case "--":
                    match(Tipos.IncrementoTermino);
                    if(imprime)
                    {
                        asm.WriteLine("Dec " + nombre);
                    }
                    resultado--;
                    break;
                case "+=":
                    match(Tipos.IncrementoTermino);
                    Expresion(imprime);
                    resultado += stack.Pop();
                    if(imprime)
                    {
                        asm.WriteLine("POP AX");
                    }
                    break;
                case "-=":
                    match(Tipos.IncrementoTermino);
                    Expresion(imprime);
                    resultado -= stack.Pop();
                    break;
                case "*=":
                    match(Tipos.IncrementoFactor);
                    Expresion(imprime);
                    resultado *= stack.Pop();
                    break;
                case "/=":
                    match(Tipos.IncrementoFactor);
                    Expresion(imprime);
                    resultado /= stack.Pop();
                    break;
                case "%=":
                    match(Tipos.IncrementoFactor);
                    Expresion(imprime);
                    resultado %= stack.Pop();
                    break;
            }
            return resultado;
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion, bool imprime)
        {
            match("switch");
            match("(");
            Expresion(imprime);
            stack.Pop();
            if(imprime)
            {
                asm.WriteLine("Pop AX");
            }
            match(")");
            match("{");
            ListaDeCasos(evaluacion, imprime);
            if(getContenido() == "default")
            {
                match("default");
                match(":");
                if(getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion, imprime);   
                }
                else
                {
                    Instruccion(evaluacion, imprime);
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion, bool imprime)
        {
            match("case");
            Expresion(imprime);
            stack.Pop();
            if(imprime)
            {
                asm.WriteLine("Pop AX");
            }
            match(":");
            ListaInstruccionesCase(evaluacion, imprime);
            if(getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if(getContenido() == "case")
            {
                ListaDeCasos(evaluacion, imprime);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion(string etiqueta, bool imprime)
        {
            Expresion(imprime);
            String operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion(imprime);
            float e2 = stack.Pop();
            if(imprime)
            {
                asm.WriteLine("POP AX");
            }
            float e1 = stack.Pop();
            if(imprime)
            {
                asm.WriteLine("POP BX");
            }
            switch(operador)
            {
                case "==":
                    if(imprime)
                    {
                        asm.WriteLine("JNE" + etiqueta);
                    }
                    return e1 == e2;
                case "<":
                    if(imprime)
                    {
                        asm.WriteLine("JGE" + etiqueta);
                    }
                    return e1 < e2;
                case "<=":
                    if(imprime)
                    {
                        asm.WriteLine("JG" + etiqueta);
                    }
                    return e1 <= e2;
                case ">":
                    if(imprime)
                    {
                        asm.WriteLine("JLE" + etiqueta);
                    }
                    return e1 > e2;
                case ">=":
                    if(imprime)
                    {
                        asm.WriteLine("JL" + etiqueta);
                    }
                    return e1 >= e2;
                default:
                    if(imprime)
                    {
                        asm.WriteLine("JE" + etiqueta);
                    }
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion, bool imprime)
        {
            String etiquetaIf = "if" + ++cIf;
            string finIf = "else" + cIf;
            match("if");
            match("(");
            bool validarIf = Condicion("", imprime);
            if(!evaluacion)
            {
                validarIf = false;
            }
            match(")");
            if(getContenido() == "{")
            {
                BloqueInstrucciones(validarIf, imprime);          
            }
            else
            {
                Instruccion(validarIf, imprime);  
            }
            if(imprime)
            {
                asm.WriteLine("JMP " + finIf);
                asm.WriteLine(etiquetaIf + ":");  
            }
            if(getContenido() == "else")
            {
                match("else");
                if(getContenido() == "{")
                {
                    if(evaluacion)
                    {
                        BloqueInstrucciones(!validarIf, imprime);
                    }
                    else
                    {
                        BloqueInstrucciones(evaluacion, imprime);
                    }
                }
                else
                {
                    if(evaluacion)
                    {
                        Instruccion(!validarIf, imprime);
                    }
                    else
                    {
                        Instruccion(evaluacion, imprime);
                    }
                }
            }
            if(imprime)
            {
                asm.WriteLine(finIf + ":");
            }
        }

        //Printf -> printf(cadena o expresion);
        private void Printf(bool evaluacion, bool imprime)
        {
            match("printf");
            match("(");
            if(getClasificacion()==Tipos.Cadena)
            {
                if(evaluacion){
                    string cadena = getContenido();
                    if(cadena.Contains("\\n"))
                    {
                        cadena=cadena.Replace("\\n", "\n");
                    }
                    if(cadena.Contains("\\t"))
                    {
                       cadena=cadena.Replace("\\t", "\t");
                    }
                    for(int i=1; i<cadena.Length-1; i++)
                    {
                        Console.Write(cadena[i]);
                    }
                }
                if(imprime)
                {
                    asm.WriteLine("PRINTN " + getContenido());
                }
                match(Tipos.Cadena);
            }
            else
            {
                Expresion(imprime);
                float resultado = stack.Pop();
                if(imprime)
                {
                    asm.WriteLine("Pop AX");
                }
                if(evaluacion)
                {
                    Console.Write(resultado);
                }
            }
            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena, &identificador);
        private void Scanf(bool evaluacion, bool imprime)    
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            if(!existeVariable(getContenido()))
            {
                throw new Error("Error de sintaxis, variable inexistente <" +getContenido()+"> en linea: "+linea, log);
            } 
            if(evaluacion)
            {
                string val = ""+Console.ReadLine(); 
                //Requerimiento 5
                double validaVal;
                if(!double.TryParse(val, out validaVal))
                {
                    throw new Error("Error de sintaxis, se espera un numero en linea: "+linea, log);
                }
                float valorFloat = float.Parse(val);
                modVariable(getContenido(), valorFloat);
            }         
            if(imprime)
            {
                asm.WriteLine("CALL SCAN_NUM");
                asm.WriteLine("MOV " + getContenido() + ", CX");
            } 
            match(Tipos.Identificador);
            match(")");
            match(";");
        }

        //Expresion -> Termino MasTermino
        private void Expresion(bool imprime)
        {
            Termino(imprime);
            MasTermino(imprime);
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino(bool imprime)
        {
            if(getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino(imprime);
                log.Write(operador+" ");
                float n1=stack.Pop();
                if(imprime)
                {
                    asm.WriteLine("Pop AX");
                }
                float n2=stack.Pop();
                if(imprime)
                {
                    asm.WriteLine("Pop BX");
                    asm.WriteLine("CMP AX, BX");
                }
                switch(operador)
                {
                    case "+":
                        stack.Push(n2+n1);
                        if(imprime)
                        {
                            asm.WriteLine("ADD AX, BX");
                            asm.WriteLine("Push AX");
                        }
                        break;
                    case "-":
                        stack.Push(n2-n1);
                        if(imprime)
                        {
                            asm.WriteLine("SUB AX, BX");
                            asm.WriteLine("Push AX");
                        }
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino(bool imprime)
        {
            Factor(imprime);
            PorFactor(imprime);
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor(bool imprime)
        {
            if(getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor(imprime);
                log.Write(operador+" ");
                float n1=stack.Pop();
                if(imprime)
                {
                    asm.WriteLine("Pop AX");
                }
                float n2=stack.Pop();
                if(imprime)
                {
                    asm.WriteLine("Pop BX");
                }
                //Requerimiento 1.a
                switch(operador)
                {
                    case "*":
                        stack.Push(n2*n1);
                        if(imprime)
                        {
                            asm.WriteLine("MUL BX");
                            asm.WriteLine("Push AX");
                        }
                        break;
                    case "/":
                        stack.Push(n2/n1);
                        if(imprime)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("Push AX");
                        }
                        break;
                    case "%":
                        stack.Push(n2%n1);
                        if(imprime)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("Push DX");
                        }
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor(bool imprime)
        {
            if(getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido() + " ");
                if(dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    dominante = evaluaNumero(float.Parse(getContenido()));
                }       
                stack.Push(float.Parse(getContenido()));
                if(imprime)
                {
                    asm.WriteLine("Mov AX, "+ getContenido());
                    asm.WriteLine("Push AX");
                }
                match(Tipos.Numero);
            }
            else if(getClasificacion() == Tipos.Identificador)
            {
                if(!existeVariable(getContenido()))
                {
                    throw new Error("Error de sintaxis, variable inexistente <" +getContenido()+"> en linea: "+linea, log);
                }
                log.Write(getContenido() + " ");
                if(dominante < getTipo(getContenido()))
                {
                    dominante = getTipo(getContenido());
                }
                stack.Push(getValor(getContenido()));
                //Requerimiento 3.a
                if(imprime)
                {
                    asm.WriteLine("Mov AX, " + getContenido());
                    asm.WriteLine("Push AX");
                }
                match(Tipos.Identificador);
            }
            else
            {
                bool huboCasteo = false;
                Variable.TipoDato casteo = Variable.TipoDato.Char;
                match("(");
                if(getClasificacion() == Tipos.TipoDato)
                {
                    huboCasteo = true;
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
                Expresion(imprime);
                match(")");
                if(huboCasteo)
                {
                    dominante = casteo;
                    float valor = stack.Pop();
                    if(imprime)
                    {
                        asm.WriteLine("Pop AX");
                    }
                    valor = convert(valor, dominante);
                    stack.Push(valor);
                    //Requerimiento 3.a
                    if(imprime)
                    {
                        asm.WriteLine("Mov AX, " + valor);
                        asm.WriteLine("Push AX");
                    }
                }
            }
        }
    }
}