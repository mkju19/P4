
using System;

namespace alela {



public class Parser {
	public const int _EOF = 0;
	public const int _ID = 1;
	public const int _NUM = 2;
	public const int maxT = 16;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;



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
	}

	void dcls() {
		dcl();
		if (StartOf(1)) {
			dcls();
		}
	}

	void dcl() {
		type();
		assig();
	}

	void type() {
		if (la.kind == 11) {
			Get();
		} else if (la.kind == 12) {
			Get();
		} else if (la.kind == 13) {
			Get();
		} else if (la.kind == 14) {
			Get();
		} else if (la.kind == 15) {
			Get();
		} else SynErr(17);
	}

	void assig() {
		Expect(1);
		Expect(3);
		expr();
	}

	void expr() {
		addExpr();
	}

	void addExpr() {
		multExpr();
		if (la.kind == 4 || la.kind == 5) {
			addExprOp();
		}
	}

	void multExpr() {
		terminalExpr();
		if (la.kind == 6 || la.kind == 7 || la.kind == 8) {
			multExprOp();
		}
	}

	void addExprOp() {
		if (la.kind == 4) {
			Get();
			addExpr();
		} else if (la.kind == 5) {
			Get();
			addExpr();
		} else SynErr(18);
	}

	void terminalExpr() {
		if (la.kind == 1 || la.kind == 2) {
			value();
		} else if (la.kind == 9) {
			Get();
			expr();
			Expect(10);
		} else SynErr(19);
	}

	void multExprOp() {
		if (la.kind == 6) {
			Get();
			multExpr();
		} else if (la.kind == 7) {
			Get();
			multExpr();
		} else if (la.kind == 8) {
			Get();
			multExpr();
		} else SynErr(20);
	}

	void value() {
		if (la.kind == 2) {
			Get();
		} else if (la.kind == 1) {
			Get();
		} else SynErr(21);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Alela();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_T, _x,_x}

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
			case 3: s = "\"=\" expected"; break;
			case 4: s = "\"+\" expected"; break;
			case 5: s = "\"-\" expected"; break;
			case 6: s = "\"*\" expected"; break;
			case 7: s = "\"/\" expected"; break;
			case 8: s = "\"%\" expected"; break;
			case 9: s = "\"(\" expected"; break;
			case 10: s = "\")\" expected"; break;
			case 11: s = "\"void\" expected"; break;
			case 12: s = "\"int\" expected"; break;
			case 13: s = "\"float\" expected"; break;
			case 14: s = "\"string\" expected"; break;
			case 15: s = "\"boolean\" expected"; break;
			case 16: s = "??? expected"; break;
			case 17: s = "invalid type"; break;
			case 18: s = "invalid addExprOp"; break;
			case 19: s = "invalid terminalExpr"; break;
			case 20: s = "invalid multExprOp"; break;
			case 21: s = "invalid value"; break;

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