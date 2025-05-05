🧾 Tiny Compiler (C#)

A simple, educational Tiny Compiler built using C#, designed to help understand the core phases of compilation including lexical analysis, parsing, and syntax tree generation.

🚀 Features

Lexical Analyzer (Tokenizer)

Recursive Descent Parser

Syntax Tree Generation

Error Handling and Reporting

Clean separation of compiler phases

Educational and well-commented code structure

🧱 Tech Stack

Language: C#

IDE: Visual Studio

Platform: .NET Framework / .NET Core

📂 Compiler Phases Implemented

Lexical Analysis

Converts source code into a stream of tokens

Handles identifiers, numbers, keywords, symbols, etc.

Syntax Analysis (Parsing)

Uses recursive descent parsing techniques

Constructs a syntax tree

Validates grammar rules

Error Reporting

Detailed syntax error messages

Token position and type tracking

⚙️ How to Run

Clone the Repository

git clone https://github.com/your-username/Tiny_compiler.git
cd tiny-compiler

Open in Visual Studio

Open the TINY_Compiler.sln file in Visual Studio.

Build and Run

Press Ctrl + F5 to build and run the project.

Enter sample source code and view output tokens and parse results.

💡 Example Input

int x = 5 + 3;

Output:

Token Stream:

int, identifier(x), =, number(5), +, number(3), ;

Parse Tree:

Visual or printed tree depending on implementation

📜 License

This project is open-source and available under the MIT License.
