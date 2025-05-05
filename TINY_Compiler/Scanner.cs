using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum Token_Class
{
    Identifier, Number, StringConstant,
    Int, Float, String, Read, Write, Repeat, Until, If, ElseIf, Else, End, Then, Return, Endl, Main, 
    LBracket, RBracket, LCurlyBracket, RCurlyBracket, Comma, Semicolon, 
    PlusOp, MinusOp, MultiplyOp, DivideOp, AssignmentOp, EqualOp, LessThanOp, GreaterThanOp, NotEqualOp, AndOp, OrOp,
}
namespace TINY_Compiler
{
    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("elseif", Token_Class.ElseIf);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("end", Token_Class.End);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("main", Token_Class.Main);

            Operators.Add("(", Token_Class.LBracket);
            Operators.Add(")", Token_Class.RBracket);
            Operators.Add("{", Token_Class.LCurlyBracket);
            Operators.Add("}", Token_Class.RCurlyBracket);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add(":=", Token_Class.AssignmentOp);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);
        }
        
        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if (CurrentChar >= 'A' && CurrentChar <= 'z') //if you read a character
                {
                    for (j++; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();

                        if (!isIdentifier(CurrentLexeme))
                        {
                            CurrentLexeme = CurrentLexeme.Remove(CurrentLexeme.Length - 1);
                            break;
                        }
                    }
                    i = j - 1;
                }

                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    for (j++; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();

                        if (CurrentChar == '.')
                        {
                            continue;
                        }

                        if (!isNumber(CurrentLexeme))
                        {
                            CurrentLexeme = CurrentLexeme.Remove(CurrentLexeme.Length - 1);
                            break;
                        }
                    }
                    i = j - 1;
                }
                else if (CurrentChar == '"')
                {
                    for (j++; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();
                        if (CurrentChar == '"')
                        {
                            break;
                        }
                    }
                    i = j;
                }
                else if (CurrentChar == '/' && j+1 < SourceCode.Length && SourceCode[j+1]=='*')
                {
                    for (j++; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();
                        if (CurrentChar == '/')
                        {
                            break;
                        }
                    }
                    i = j;
                }
                else if (CurrentChar == ':' || CurrentChar == '<' || CurrentChar == '&' || CurrentChar == '|')
                {
                    if (j + 1 < SourceCode.Length && Operators.ContainsKey(CurrentLexeme + SourceCode[j + 1].ToString()))
                    {
                        CurrentLexeme += SourceCode[++j].ToString();
                    }
                    i = j;
                }

                FindTokenClass(CurrentLexeme);
            }

            TINY_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (ReservedWords.TryGetValue(Lex, out TC)){
                Tok.token_type = TC;
            }

            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Identifier;
            }

            //Is it a Number?
            else if (isNumber(Lex))
            {
                Tok.token_type = Token_Class.Number;
            }

            //Is it a String Constant?
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.StringConstant;
            }

            //Is it a String Constant?
            else if (isComment(Lex))
            {
                return;
            }

            //Is it an operator?
            else if (Operators.TryGetValue(Lex, out TC))
            {
                Tok.token_type = TC;
            }

            //Is it an undefined?
            else
            {
                Errors.Error_List.Add(Lex);
                return;
            }

            Tokens.Add(Tok);
        }



        bool isIdentifier(string lex)
        {
            Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
            return regex.IsMatch(lex);
        }
        bool isNumber(string lex)
        {
            Regex regex = new Regex(@"[0-9]+(\.[0-9]+)?$");
            return regex.IsMatch(lex);
        }

        bool isString(string lex)
        {
            Regex regex = new Regex("^\".*\"$");
            return regex.IsMatch(lex);
        }

        bool isComment(string lex)
        {
            Regex regex = new Regex(@"^/\*.*\*/$");
            return regex.IsMatch(lex);
        }

    }
}
