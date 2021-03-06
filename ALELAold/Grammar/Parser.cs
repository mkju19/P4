
using System;
using Alela.AST

namespace alela {



public class Parser {
	public const int _EOF = 0;
	public const int _ID = 1;
	public const int _NUM = 2;
	public const int maxT = 17;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public ProgAST AST




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

	
	void Alela() {
		dcls();
		lines();
	}

	void dcls() {
		dcl();
		Expect(3);
		if (StartOf(1)) {
			dcls();
		}
	}

	void lines() {
		while (StartOf(2)) {
			line();
			Expect(3);
		}
	}

	void line() {
		if (StartOf(1)) {
			dcl();
		} else if (la.kind == 1) {
			Get();
			assig();
		} else SynErr(18);
	}

	void dcl() {
		type();
		Expect(1);
		if (la.kind == 4) {
			assig();
		}
	}

	void assig() {
		Expr e; 
		Expect(4);
		expr(out e);
	}

	void type() {
		if (la.kind == 12) {
			Get();
		} else if (la.kind == 13) {
			Get();
		} else if (la.kind == 14) {
			Get();
		} else if (la.kind == 15) {
			Get();
		} else if (la.kind == 16) {
			Get();
		} else SynErr(19);
	}

	void expr(out Expr e) {
		addExpr(out e);
	}

	void addExpr(out Expr e) {
		Operator op; Expr e2; 
		multExpr(out e);
		if (la.kind == 5 || la.kind == 6) {
			addExprOp(out op, out e2);
			e = new BinExpr(e, op, e2); 
		}
	}

	void multExpr(out Expr e) {
		Operator op; Expr e2; 
		terminalExpr(out e);
		if (la.kind == 7 || la.kind == 8 || la.kind == 9) {
			multExprOp(out op, out e2);
			e = new BinExpr(e, op, e2); 
		}
	}

	void addExprOp(out Operator op, out Expr e2) {
		if (la.kind == 5) {
			Get();
			addExpr(out e2);
			op = Operator.PLUS; 
		} else if (la.kind == 6) {
			Get();
			addExpr(out e2);
			op = Operator.MINUS; 
		} else SynErr(20);
	}

	void terminalExpr(out Expr e) {
		if (la.kind == 2) {
			value(out e);
		} else if (la.kind == 10) {
			Get();
			expr(out e);
			Expect(11);
		} else SynErr(21);
	}

	void multExprOp(out Operator op, out Expr e2) {
		if (la.kind == 7) {
			Get();
			multExpr(out e2);
			op = Operator.MULT; 
		} else if (la.kind == 8) {
			Get();
			multExpr(out e2);
			op = Operator.DIV; 
		} else if (la.kind == 9) {
			Get();
			multExpr(out e2);
			op = Operator.MOD; 
		} else SynErr(22);
	}

	void value(out Expr e) {
		Expect(2);
		e = new Expr(t.value); 
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Alela();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_T, _T,_x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_T, _T,_x,_x}

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
			case 1: s = "ID expected"; break;
			case 2: s = "NUM expected"; break;
			case 3: s = "\";\" expected"; break;
			case 4: s = "\"=\" expected"; break;
			case 5: s = "\"+\" expected"; break;
			case 6: s = "\"-\" expected"; break;
			case 7: s = "\"*\" expected"; break;
			case 8: s = "\"/\" expected"; break;
			case 9: s = "\"%\" expected"; break;
			case 10: s = "\"(\" expected"; break;
			case 11: s = "\")\" expected"; break;
			case 12: s = "\"void\" expected"; break;
			case 13: s = "\"int\" expected"; break;
			case 14: s = "\"float\" expected"; break;
			case 15: s = "\"string\" expected"; break;
			case 16: s = "\"boolean\" expected"; break;
			case 17: s = "??? expected"; break;
			case 18: s = "invalid line"; break;
			case 19: s = "invalid type"; break;
			case 20: s = "invalid addExprOp"; break;
			case 21: s = "invalid terminalExpr"; break;
			case 22: s = "invalid multExprOp"; break;

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