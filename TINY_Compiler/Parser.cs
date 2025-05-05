using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TINY_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = Program();
            return root;
        }

        Node FunctionCall()
        {
            Node functioncall = new Node("FunctionCall");
            functioncall.Children.Add(match(Token_Class.Identifier));
            functioncall.Children.Add(match(Token_Class.LBracket));
            functioncall.Children.Add(Arguments());
            functioncall.Children.Add(match(Token_Class.RBracket));
            return functioncall;
        }

        Node Argument()
        {
            Node argument = new Node("Argument");
            if (InputPointer < TokenStream.Count && Token_Class.Number == TokenStream[InputPointer].token_type)
            {
                argument.Children.Add(match(Token_Class.Number));
            }
            else
            {
                argument.Children.Add(match(Token_Class.Identifier));
            }
            return argument;
        }

        Node Arguments()
        {
            Node arguments = new Node("Arguments");
            arguments.Children.Add(Argument());
            arguments.Children.Add(ArgumentList());
            return arguments;
        }

        void ArgumentListAux(Node root)
        {
            if (InputPointer >= TokenStream.Count || Token_Class.Comma != TokenStream[InputPointer].token_type)
            {
                return;
            }
            root.Children.Add(match(Token_Class.Comma));
            root.Children.Add(Argument());
            ArgumentListAux(root);
        }

        Node ArgumentList()
        {
            Node argumentlist = new Node("ArgumentList");
            ArgumentListAux(argumentlist);
            return argumentlist;
        }

        Node Term()
        {
            Node term = new Node("Term");
            if (InputPointer < TokenStream.Count && Token_Class.Number == TokenStream[InputPointer].token_type)
            {
                term.Children.Add(match(Token_Class.Number));
            }
            else
            {
                if (InputPointer+1 < TokenStream.Count && Token_Class.LBracket == TokenStream[InputPointer+1].token_type)
                {
                    term.Children.Add(FunctionCall());
                }
                else
                {
                    term.Children.Add(match(Token_Class.Identifier));
                }
            }
            return term;
        }

        Node ArithmeticOperator()
        {
            Node arithmeticoperator = new Node("ArithmeticOperator");
            if (InputPointer < TokenStream.Count && Token_Class.PlusOp == TokenStream[InputPointer].token_type)
            {
                arithmeticoperator.Children.Add(match(Token_Class.PlusOp));
            }
            else if (InputPointer < TokenStream.Count && Token_Class.MinusOp == TokenStream[InputPointer].token_type)
            {
                arithmeticoperator.Children.Add(match(Token_Class.MinusOp));
            }
            else if (InputPointer < TokenStream.Count && Token_Class.MultiplyOp == TokenStream[InputPointer].token_type)
            {
                arithmeticoperator.Children.Add(match(Token_Class.MultiplyOp));
            }
            else
            {
                arithmeticoperator.Children.Add(match(Token_Class.DivideOp));
            }
            return arithmeticoperator;
        }

        Node Equation()
        {
            Node equation = new Node("Equation");
            equation.Children.Add(EquationTerm());
            equation.Children.Add(EquationExpression());
            return equation;
        }

        void EquationExpressionAux(Node root)
        {
            if (InputPointer >= TokenStream.Count || (Token_Class.PlusOp != TokenStream[InputPointer].token_type && Token_Class.MinusOp != TokenStream[InputPointer].token_type))
            {
                return;
            }
            root.Children.Add(AddOp());
            root.Children.Add(EquationTerm());
            EquationExpressionAux(root);
        }
        Node EquationExpression()
        {
            Node equationexpression = new Node("EquationExpression");
            EquationExpressionAux(equationexpression);
            return equationexpression;
        }

        Node EquationTerm()
        {
            Node equationterm = new Node("EquationTerm");
            equationterm.Children.Add(EquationFactor());
            equationterm.Children.Add(EquationTerms());
            return equationterm;
        }

        void EquationTermsAux(Node root)
        {
            if (InputPointer >= TokenStream.Count || (Token_Class.MultiplyOp != TokenStream[InputPointer].token_type && Token_Class.DivideOp != TokenStream[InputPointer].token_type))
            {
                return;
            }
            root.Children.Add(MulOp());
            root.Children.Add(EquationFactor());
            EquationTermsAux(root);
        }

        Node EquationTerms()
        {
            Node equationterms = new Node("EquationTerms");
            EquationTermsAux(equationterms);
            return equationterms;
        }

        Node EquationFactor()
        {
            Node equationfactor = new Node("EquationFactor");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LBracket)
            {
                equationfactor.Children.Add(match(Token_Class.LBracket));
                equationfactor.Children.Add(Equation());
                equationfactor.Children.Add(match(Token_Class.RBracket));
            }
            else
            {
                equationfactor.Children.Add(Term());
            }
                
            return equationfactor;
        }

        Node AddOp()
        {
            Node addop = new Node("addop");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.PlusOp)
                addop.Children.Add(match(Token_Class.PlusOp));
            else
                addop.Children.Add(match(Token_Class.MinusOp));


            return addop;
        }

        Node MulOp()
        {
            Node mulop = new Node("mulop");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
                mulop.Children.Add(match(Token_Class.MultiplyOp));
            else
                mulop.Children.Add(match(Token_Class.DivideOp));

            return mulop;
        }

        Node Expression()
        {
            Node expression = new Node("Expression");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.StringConstant)
            {
                expression.Children.Add(match(Token_Class.StringConstant));
            }
            else
            {
                if ((InputPointer < TokenStream.Count && Token_Class.LBracket == TokenStream[InputPointer].token_type) ||(InputPointer+1 < TokenStream.Count && (Token_Class.PlusOp == TokenStream[InputPointer+1].token_type || Token_Class.MinusOp == TokenStream[InputPointer+1].token_type || Token_Class.MultiplyOp == TokenStream[InputPointer+1].token_type || Token_Class.DivideOp == TokenStream[InputPointer + 1].token_type)))
                {
                    expression.Children.Add(Equation());
                }
                else
                {
                    expression.Children.Add(Term());
                }
            }
            return expression;
        }

        Node SingleAssignmentStatement()
        {
            Node singleassignmentstatement = new Node("SingleAssignmentStatement");
            singleassignmentstatement.Children.Add(AssignmentStatement());
            singleassignmentstatement.Children.Add(match(Token_Class.Semicolon));
            return singleassignmentstatement;
        }

        Node AssignmentStatement()
        {
            Node assignmentstatement = new Node("AssignmentStatement");
            assignmentstatement.Children.Add(match(Token_Class.Identifier));
            assignmentstatement.Children.Add(match(Token_Class.AssignmentOp));
            assignmentstatement.Children.Add(Expression());
            return assignmentstatement;
        }

        Node DataType()
        {
            Node datatype = new Node("DataType");

            if (InputPointer < TokenStream.Count && Token_Class.Int == TokenStream[InputPointer].token_type)
            {
                datatype.Children.Add(match(Token_Class.Int));
            }
            else if (InputPointer < TokenStream.Count && Token_Class.Float == TokenStream[InputPointer].token_type)
            {
                datatype.Children.Add(match(Token_Class.Float));

            }
            else
            {
                datatype.Children.Add(match(Token_Class.String));
            }

            return datatype;
        }


        Node DeclarationStatement()
        {
            Node declarationstatement = new Node("DeclarationStatement");
            declarationstatement.Children.Add(DataType());
            if (InputPointer + 1 < TokenStream.Count && Token_Class.AssignmentOp == TokenStream[InputPointer + 1].token_type)
            {
                declarationstatement.Children.Add(AssignmentStatement());
            }
            else
            {
                declarationstatement.Children.Add(match(Token_Class.Identifier));
            }
            declarationstatement.Children.Add(DeclarationVariables());
            declarationstatement.Children.Add(match(Token_Class.Semicolon));
            return declarationstatement;
        }

        void DeclarationVariablesAux(Node root)
        {
            if (InputPointer >= TokenStream.Count || Token_Class.Comma != TokenStream[InputPointer].token_type)
            {
                return;
            }

            root.Children.Add(match(Token_Class.Comma));
            if (InputPointer+1 < TokenStream.Count && Token_Class.AssignmentOp == TokenStream[InputPointer+1].token_type)
            {
                root.Children.Add(AssignmentStatement());
            }
            else
            {
                root.Children.Add(match(Token_Class.Identifier));
            }
        
            DeclarationVariablesAux(root);
        }

        Node DeclarationVariables()
        {
            Node declarationvariables = new Node("DeclarationVariables");
            DeclarationVariablesAux(declarationvariables);
            return declarationvariables;
        }

        Node WriteStatemrnt()
        {
            Node writestatement = new Node("WriteStatement");
            writestatement.Children.Add(match(Token_Class.Write));
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Endl)
            {
                writestatement.Children.Add(match(Token_Class.Endl));
            }
            else
            {
                writestatement.Children.Add(Expression());
            }
            writestatement.Children.Add(match(Token_Class.Semicolon));
            return writestatement;
        }

        Node ReadStatement()
        {
            Node readstatement = new Node("ReadStatement");
            readstatement.Children.Add(match(Token_Class.Read));
            readstatement.Children.Add(match(Token_Class.Identifier));
            readstatement.Children.Add(match(Token_Class.Semicolon));
            return readstatement;
        }

        Node ReturnStatement()
        {
            Node returnstatement = new Node("ReturnStatement");
            returnstatement.Children.Add(match(Token_Class.Return));
            returnstatement.Children.Add(Expression());
            returnstatement.Children.Add(match(Token_Class.Semicolon));
            return returnstatement;
        }

        Node ConditionOperator()
        {
            Node conditionoperator = new Node("ConditionOperator");
            if (InputPointer < TokenStream.Count && Token_Class.EqualOp == TokenStream[InputPointer].token_type)
            {
                conditionoperator.Children.Add(match(Token_Class.EqualOp));
            }
            else if (InputPointer < TokenStream.Count && Token_Class.LessThanOp == TokenStream[InputPointer].token_type)
            {
                conditionoperator.Children.Add(match(Token_Class.LessThanOp));
            }
            else if (InputPointer < TokenStream.Count && Token_Class.GreaterThanOp == TokenStream[InputPointer].token_type)
            {
                conditionoperator.Children.Add(match(Token_Class.GreaterThanOp));
            }
            else
            {
                conditionoperator.Children.Add(match(Token_Class.NotEqualOp));
            }

            return conditionoperator;
        }

        Node Condition()
        {
            Node condition = new Node("Condition");
            condition.Children.Add(match(Token_Class.Identifier));
            condition.Children.Add(ConditionOperator());
            condition.Children.Add(Term());
            return condition;
        }

        Node BooleanOperator()
        {
            Node booleanoperator = new Node("BooleanOperator");
            if (InputPointer < TokenStream.Count && Token_Class.AndOp == TokenStream[InputPointer].token_type)
            {
                booleanoperator.Children.Add(match(Token_Class.AndOp));
            }
            else
            {
                booleanoperator.Children.Add(match(Token_Class.OrOp));
            }
            return booleanoperator;
        }

        void ConditionAux(Node root)
        {
            if (InputPointer >= TokenStream.Count || Token_Class.AndOp != TokenStream[InputPointer].token_type && Token_Class.OrOp != TokenStream[InputPointer].token_type)
            {
                return;
            }
            root.Children.Add(BooleanOperator());
            root.Children.Add(Condition());
            ConditionAux(root);
        }

        Node ConditionExpression()
        {
            Node conditionexpression = new Node("ConditionExpression");
            ConditionAux(conditionexpression);
            return conditionexpression;
        }

        Node ConditionStatement()
        {
            Node condition = new Node("ConditionStatement");
            condition.Children.Add(Condition());
            condition.Children.Add(ConditionExpression());
            return condition;
        }

        Node IfStatement()
        {
            Node ifstatement = new Node("IfStatement");
            ifstatement.Children.Add(match(Token_Class.If));
            ifstatement.Children.Add(ConditionStatement());
            ifstatement.Children.Add(match(Token_Class.Then));
            ifstatement.Children.Add(Statements(true));
            if (InputPointer < TokenStream.Count && Token_Class.ElseIf == TokenStream[InputPointer].token_type)
            {
                ifstatement.Children.Add(ElseIfStatement());
            }
            else if (InputPointer < TokenStream.Count && Token_Class.Else == TokenStream[InputPointer].token_type)
            {
                ifstatement.Children.Add(ElseStatement());
            }
            else
            {
                ifstatement.Children.Add(match(Token_Class.End));
            }
            return ifstatement;
        }

        Node ElseIfStatement()
        {
            Node elseifstatement = new Node("ElseIfStatement");
            elseifstatement.Children.Add(match(Token_Class.ElseIf));
            elseifstatement.Children.Add(ConditionStatement());
            elseifstatement.Children.Add(match(Token_Class.Then));
            elseifstatement.Children.Add(Statements(true));
            if (InputPointer < TokenStream.Count && Token_Class.ElseIf == TokenStream[InputPointer].token_type)
            {
                elseifstatement.Children.Add(ElseIfStatement());
            }
            else if (InputPointer < TokenStream.Count && Token_Class.Else == TokenStream[InputPointer].token_type)
            {
                elseifstatement.Children.Add(ElseStatement());
            }
            else
            {
                elseifstatement.Children.Add(match(Token_Class.End));
            }
            return elseifstatement;
        }

        Node ElseStatement()
        {
            Node elsestatement = new Node("ElseStatement");
            elsestatement.Children.Add(match(Token_Class.Else));
            elsestatement.Children.Add(Statements(true));
            elsestatement.Children.Add(match(Token_Class.End));
            return elsestatement;
        }

        Node Statements(bool has_return)
        {
            Node statements = new Node("Statements");
            if (InputPointer < TokenStream.Count && Token_Class.Identifier == TokenStream[InputPointer].token_type)
            {
                statements.Children.Add(SingleAssignmentStatement());
            }
            else if (InputPointer < TokenStream.Count && Token_Class.Write == TokenStream[InputPointer].token_type)
            {
                statements.Children.Add(WriteStatemrnt());
            }
            else if (InputPointer < TokenStream.Count && Token_Class.Read == TokenStream[InputPointer].token_type)
            {
                statements.Children.Add(ReadStatement());
            }
            else if (InputPointer < TokenStream.Count && Token_Class.If == TokenStream[InputPointer].token_type)
            {
                statements.Children.Add(IfStatement());
            }
            else if (InputPointer < TokenStream.Count && Token_Class.Repeat == TokenStream[InputPointer].token_type)
            {
                statements.Children.Add(ReapeatStatement());
            }
            else if (InputPointer < TokenStream.Count && Token_Class.Return == TokenStream[InputPointer].token_type && has_return)
            {
                statements.Children.Add(ReturnStatement());
            }
            else
            {
                statements.Children.Add(DeclarationStatement());
            }
            statements.Children.Add(StatementsList(has_return));
            return statements;
        }

        Node SatatemsntsListAux(Node root, bool has_return)
        {
            if (InputPointer >= TokenStream.Count || (Token_Class.Identifier != TokenStream[InputPointer].token_type && Token_Class.Write != TokenStream[InputPointer].token_type && Token_Class.Read != TokenStream[InputPointer].token_type && Token_Class.If != TokenStream[InputPointer].token_type && Token_Class.Repeat != TokenStream[InputPointer].token_type && Token_Class.Int != TokenStream[InputPointer].token_type && Token_Class.Float != TokenStream[InputPointer].token_type && Token_Class.String != TokenStream[InputPointer].token_type) && (!has_return || Token_Class.Return != TokenStream[InputPointer].token_type))
            {
                return null;
            }

            if (InputPointer < TokenStream.Count && Token_Class.Identifier == TokenStream[InputPointer].token_type)
            {
                root.Children.Add(SingleAssignmentStatement());
            }
            else if (InputPointer < TokenStream.Count && Token_Class.Write == TokenStream[InputPointer].token_type)
            {
                root.Children.Add(WriteStatemrnt());
            }
            else if (InputPointer < TokenStream.Count && Token_Class.Read == TokenStream[InputPointer].token_type)
            {
                root.Children.Add(ReadStatement());
            }
            else if (InputPointer < TokenStream.Count && Token_Class.If == TokenStream[InputPointer].token_type)
            {
                root.Children.Add(IfStatement());
            }
            else if (InputPointer < TokenStream.Count && Token_Class.Repeat == TokenStream[InputPointer].token_type)
            {
                root.Children.Add(ReapeatStatement());
            }
            else if (InputPointer < TokenStream.Count && Token_Class.Return == TokenStream[InputPointer].token_type && has_return)
            {
                root.Children.Add(ReturnStatement());
            }
            else
            {
                root.Children.Add(DeclarationStatement());
            }
            SatatemsntsListAux(root, has_return);
            return root;
        }

        Node StatementsList(bool has_return)
        {
            Node statementslist = new Node("StatementsList");
            SatatemsntsListAux(statementslist, has_return);
            return statementslist;
        }

        Node ReapeatStatement()
        {
            Node repeatstatement = new Node("RepeatStatement");
            repeatstatement.Children.Add(match(Token_Class.Repeat));
            repeatstatement.Children.Add(Statements(true));
            repeatstatement.Children.Add(match(Token_Class.Until));
            repeatstatement.Children.Add(ConditionStatement());
            return repeatstatement;
        }

        Node FunctionName()
        {
            Node functionname = new Node("FunctionName");
            functionname.Children.Add(match(Token_Class.Identifier));
            return functionname;
        }

        Node Parameter()
        {
            Node parameter = new Node("Parameter");
            parameter.Children.Add(DataType());
            parameter.Children.Add(match(Token_Class.Identifier));
            return parameter;
        }

        Node Parameters()
        {
            Node parameter = new Node("Parameters");
            parameter.Children.Add(Parameter());
            parameter.Children.Add(ParameterList());
            return parameter;
        }

        void ParameterListAux(Node root)
        {
            if (InputPointer >= TokenStream.Count || Token_Class.Comma != TokenStream[InputPointer].token_type)
            {
                return;
            }
            root.Children.Add(match(Token_Class.Comma));
            root.Children.Add(Parameter());
            ParameterListAux(root);
        }

        Node ParameterList()
        {
            Node parameterlist = new Node("ParameterList");
            ParameterListAux(parameterlist);
            return parameterlist;
        }

        Node FunctionDecleration()
        {
            Node functiondecleration = new Node("FunctionDecleration");
            functiondecleration.Children.Add(DataType());
            functiondecleration.Children.Add(FunctionName());
            functiondecleration.Children.Add(match(Token_Class.LBracket));
            functiondecleration.Children.Add(Parameters());
            functiondecleration.Children.Add(match(Token_Class.RBracket));
            return functiondecleration;
        }

        Node FunctionBody()
        {
            Node functionbody = new Node("FunctionBody");
            functionbody.Children.Add(match(Token_Class.LCurlyBracket));
            if (InputPointer < TokenStream.Count && Token_Class.Return != TokenStream[InputPointer].token_type)
            {
                functionbody.Children.Add(Statements(false));
            }
            functionbody.Children.Add(ReturnStatement());
            functionbody.Children.Add(match(Token_Class.RCurlyBracket));
            return functionbody;
        }

        Node FunctionStatement()
        {
            Node functionstatement = new Node("FunctionStatement");
            functionstatement.Children.Add(FunctionDecleration());
            functionstatement.Children.Add(FunctionBody());
            return functionstatement;
        }

        Node FunctionStatements()
        {
            Node functionstatements = new Node("FunctionStatements");
            functionstatements.Children.Add(FunctionStatement());
            functionstatements.Children.Add(FunctionStatementsList());
            return functionstatements;
        }

        void FunctionStatementsListAux(Node root)
        {
            if (InputPointer >= TokenStream.Count || (Token_Class.Int != TokenStream[InputPointer].token_type && Token_Class.Float != TokenStream[InputPointer].token_type && Token_Class.String != TokenStream[InputPointer].token_type) || InputPointer+1 >= TokenStream.Count || Token_Class.Main == TokenStream[InputPointer+1].token_type)
            {
                return;
            }
            root.Children.Add(FunctionStatement());
            FunctionStatementsListAux(root);
        }

        Node FunctionStatementsList()
        {
            Node functionstatementslist = new Node("FunctionStatementsList");
            FunctionStatementsListAux(functionstatementslist);
            return functionstatementslist;
        }

        Node MainFunction()
        {
            Node mainfunction = new Node("MainFunction");
            mainfunction.Children.Add(DataType());
            mainfunction.Children.Add(match(Token_Class.Main));
            mainfunction.Children.Add(match(Token_Class.LBracket));
            mainfunction.Children.Add(match(Token_Class.RBracket));
            mainfunction.Children.Add(FunctionBody());
            return mainfunction;
        }

        Node Program()
        {
            Node program = new Node("Program");
            if (InputPointer+1 < TokenStream.Count && Token_Class.Main != TokenStream[InputPointer+1].token_type)
            {
                program.Children.Add(FunctionStatements());
            }
            program.Children.Add(MainFunction());
            return program;
        }

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
