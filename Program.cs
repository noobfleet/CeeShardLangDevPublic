// BIG TO DO: ADD THIS PACKAGE!!!!!!! https://github.com/nrother/dynamiclua

using CeeShard.Runtime;

using System;
using System.Net.Mime;
using System.Reflection;
using CeeShard.Parsing;

namespace CeeShard;

class Program
{
    static void Main(string[] args)
    {
        if(args.Length == 54543)
        {
            Console.WriteLine("CeeShard CLI (v0.02)");

            Interpreter interpreter = new();

            // basic CLI
            while(true)
            {
                Console.Write("> ");

                Lexer lexer = new Lexer();
                Parser parser = new Parser();
                Token[] tokens = lexer.Tokenize(Console.ReadLine());
                LangProgram program = parser.produceAST(tokens);

                interpreter.Run(program, false);
            }
        }
        else
        {
            StreamReader sr = File.OpenText("/Users/admin/RiderProjects/CeeShard/CeeShard/code.shart");
            
            Lexer lexer = new Lexer();
            Parser parser = new Parser();
            Token[] tokens = lexer.Tokenize(sr.ReadToEnd());
            LangProgram program = parser.produceAST(tokens);
            Interpreter interpreter = new Interpreter();
            interpreter.Run(program, false);
        }
    }
}
