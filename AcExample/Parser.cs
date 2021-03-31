
using System;
using System.Collections.Generic;

namespace cocor_compiler {



public class Parser {
	public const int _EOF = 0;
	public const int _plus = 1;
	public const int _minus = 2;
	public const int _floatdcl = 3;
	public const int _intdcl = 4;
	public const int _print = 5;
	public const int _inum = 6;
	public const int _fnum = 7;
	public const int _id = 8;
	public const int maxT = 10;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public Prog ProgramAST;



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void AC() {
		List<AST> liste = new List<AST>(); SymDeclaring e; AST e2;
		dcl(out e);
		liste.Add(e); 
		while (la.kind == 3 || la.kind == 4) {
			dcl(out e);
			liste.Add(e); 
		}
		while (la.kind == 5 || la.kind == 8) {
			stmt(out e2);
			liste.Add(e2); 
		}
		ProgramAST = new Prog(liste);
	}

	void dcl(out SymDeclaring e) {
		e = null; 
		if (la.kind == 3) {
			Get();
			Expect(8);
			e = new FloatDcl(t.val); 
		} else if (la.kind == 4) {
			Get();
			Expect(8);
			e = new IntDcl(t.val); 
		} else SynErr(11);
	}

	void stmt(out AST e) {
		e = null; AST v = null; 
		if (la.kind == 8) {
			Get();
			String symref = t.val; 
			Expect(9);
			val(out v);
			e = new Assigning(symref, v); 
			if (la.kind == 1 || la.kind == 2) {
				Expression e2; 
				expr(out e2, v);
				e = new Assigning(symref, e2); 
			}
		} else if (la.kind == 5) {
			Get();
			Expect(8);
			e = new Printing(t.val); 
		} else SynErr(12);
	}

	void val(out AST e) {
		e = null; 
		if (la.kind == 6) {
			Get();
			e = new IntConsting(t.val); 
		} else if (la.kind == 7) {
			Get();
			e = new FloatConsting(t.val); 
		} else if (la.kind == 8) {
			Get();
			e = new SymReferencing(t.val); 
		} else SynErr(13);
	}

	void expr(out Expression e, AST v) {
		string op = null; AST e2; Expression e3 = null; 
		if (la.kind == 1) {
			Get();
			op = "+"; 
		} else if (la.kind == 2) {
			Get();
			op = "-"; 
		} else SynErr(14);
		val(out e2);
		e = new Expression(op, v, e2); 
		if (la.kind == 1 || la.kind == 2) {
			expr(out e3, e2);
			e = new Expression(op, v, e3); 
		}
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		AC();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "plus expected"; break;
			case 2: s = "minus expected"; break;
			case 3: s = "floatdcl expected"; break;
			case 4: s = "intdcl expected"; break;
			case 5: s = "print expected"; break;
			case 6: s = "inum expected"; break;
			case 7: s = "fnum expected"; break;
			case 8: s = "id expected"; break;
			case 9: s = "\"=\" expected"; break;
			case 10: s = "??? expected"; break;
			case 11: s = "invalid dcl"; break;
			case 12: s = "invalid stmt"; break;
			case 13: s = "invalid val"; break;
			case 14: s = "invalid expr"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}