
using System;

namespace ALELA_Compiler {



public class Parser {
	public const int _EOF = 0;
	public const int _number = 1;
	public const int _id = 2;
	public const int _newline = 3;
	public const int maxT = 45;

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

	
	void ALELA() {
		DECL();
		while (StartOf(1)) {
			DECL();
		}
		SETUP();
		LOOP();
		if (StartOf(1)) {
			FUNCDECLS();
		}
	}

	void DECL() {
		TYPE();
		Expect(2);
		ASSIG();
	}

	void SETUP() {
		Expect(4);
		BLOCK();
	}

	void LOOP() {
		Expect(7);
		BLOCK();
	}

	void FUNCDECLS() {
		FUNCDECL();
		if (StartOf(1)) {
			FUNCDECLS();
		}
	}

	void TYPE() {
		switch (la.kind) {
		case 39: {
			Get();
			break;
		}
		case 40: {
			Get();
			break;
		}
		case 41: {
			Get();
			break;
		}
		case 42: {
			Get();
			break;
		}
		case 43: {
			Get();
			break;
		}
		case 44: {
			Get();
			break;
		}
		default: SynErr(46); break;
		}
	}

	void ASSIG() {
		Expect(5);
		EXPR();
		Expect(6);
	}

	void BLOCK() {
		Expect(8);
		STMTS();
		Expect(9);
	}

	void EXPR() {
		multExpr();
		if (la.kind == 34 || la.kind == 35) {
			addExpr();
		}
	}

	void STMTS() {
		STMT();
		if (StartOf(2)) {
			STMTS();
		}
	}

	void STMT() {
		switch (la.kind) {
		case 10: {
			Get();
			LOGI_EXPR();
			Expect(11);
			BLOCK();
			ELSESTMT();
			break;
		}
		case 12: {
			Get();
			LOGI_EXPR();
			Expect(11);
			BLOCK();
			break;
		}
		case 13: {
			Get();
			VALUE();
			Expect(14);
			VALUE();
			Expect(11);
			BLOCK();
			break;
		}
		case 15: {
			Get();
			Expect(2);
			Expect(11);
			Expect(8);
			CASELIST();
			Expect(9);
			break;
		}
		case 16: {
			Get();
			VALUE();
			Expect(11);
			break;
		}
		case 17: {
			Get();
			VALUE();
			Expect(11);
			break;
		}
		case 2: {
			Get();
			CALL();
			break;
		}
		case 39: case 40: case 41: case 42: case 43: case 44: {
			DECL();
			break;
		}
		default: SynErr(47); break;
		}
	}

	void LOGI_EXPR() {
		LOGI_AND();
		if (la.kind == 25) {
			LOGI_OR();
		}
	}

	void ELSESTMT() {
		Expect(20);
		ELSEIF();
	}

	void VALUE() {
		if (la.kind == 1) {
			Get();
		} else if (la.kind == 2) {
			Get();
		} else SynErr(48);
	}

	void CASELIST() {
		if (la.kind == 22) {
			Get();
			Expect(2);
			Expect(23);
			BLOCK();
			if (la.kind == 22 || la.kind == 24) {
				CASELIST();
			}
		} else if (la.kind == 24) {
			Get();
			Expect(23);
			BLOCK();
		} else SynErr(49);
	}

	void CALL() {
		if (la.kind == 18) {
			FUNC();
		} else if (la.kind == 5) {
			ASSIG();
		} else SynErr(50);
	}

	void FUNC() {
		Expect(18);
		if (la.kind == 1 || la.kind == 2) {
			INPARAM_LIST();
		}
		Expect(11);
		Expect(6);
	}

	void INPARAM_LIST() {
		VALUE();
		if (la.kind == 19) {
			Get();
			INPARAM_LIST();
		}
	}

	void FUNCDECL() {
		TYPE();
		Expect(2);
		Expect(18);
		if (StartOf(1)) {
			FUNCARG_LIST();
		}
		Expect(11);
		BLOCK();
	}

	void FUNCARG_LIST() {
		FUNCARG();
		if (la.kind == 19) {
			Get();
			FUNCARG_LIST();
		}
	}

	void FUNCARG() {
		TYPE();
		Expect(2);
	}

	void ELSEIF() {
		if (la.kind == 21) {
			Get();
			Expect(18);
			LOGI_EXPR();
			Expect(11);
			BLOCK();
		} else if (la.kind == 8) {
			BLOCK();
		} else SynErr(51);
	}

	void LOGI_AND() {
		LOGI_EQUAL();
		if (la.kind == 26) {
			LOGI_ANDOp();
		}
	}

	void LOGI_OR() {
		Expect(25);
		LOGI_EXPR();
	}

	void LOGI_EQUAL() {
		LOGI_LG();
		if (la.kind == 27 || la.kind == 28) {
			LOGI_EQUALOp();
		}
	}

	void LOGI_ANDOp() {
		Expect(26);
		LOGI_EQUAL();
	}

	void LOGI_LG() {
		LOGI_NOT();
		if (StartOf(3)) {
			LOGI_LGOp();
		}
	}

	void LOGI_EQUALOp() {
		if (la.kind == 27) {
			Get();
		} else if (la.kind == 28) {
			Get();
		} else SynErr(52);
		LOGI_LG();
	}

	void LOGI_NOT() {
		if (la.kind == 33) {
			Get();
			LOGI_NOT();
		} else if (la.kind == 1 || la.kind == 2 || la.kind == 18) {
			LOGI_TERM();
		} else SynErr(53);
	}

	void LOGI_LGOp() {
		if (la.kind == 29) {
			Get();
		} else if (la.kind == 30) {
			Get();
		} else if (la.kind == 31) {
			Get();
		} else if (la.kind == 32) {
			Get();
		} else SynErr(54);
		LOGI_NOT();
	}

	void LOGI_TERM() {
		if (la.kind == 18) {
			Get();
			LOGI_EXPR();
			Expect(11);
		} else if (la.kind == 1 || la.kind == 2) {
			VALUE();
		} else SynErr(55);
	}

	void multExpr() {
		terminalExpr();
		if (la.kind == 36 || la.kind == 37 || la.kind == 38) {
			multExprOp();
		}
	}

	void addExpr() {
		if (la.kind == 34) {
			Get();
		} else if (la.kind == 35) {
			Get();
		} else SynErr(56);
		EXPR();
	}

	void terminalExpr() {
		if (la.kind == 1 || la.kind == 2) {
			VALUE();
		} else if (la.kind == 18) {
			Get();
			EXPR();
			Expect(11);
		} else SynErr(57);
	}

	void multExprOp() {
		if (la.kind == 36) {
			Get();
		} else if (la.kind == 37) {
			Get();
		} else if (la.kind == 38) {
			Get();
		} else SynErr(58);
		multExpr();
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		ALELA();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_T, _T,_x,_x},
		{_x,_x,_T,_x, _x,_x,_x,_x, _x,_x,_T,_x, _T,_T,_x,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_T, _T,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x}

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
			case 1: s = "number expected"; break;
			case 2: s = "id expected"; break;
			case 3: s = "newline expected"; break;
			case 4: s = "\"setup\" expected"; break;
			case 5: s = "\"=\" expected"; break;
			case 6: s = "\";\" expected"; break;
			case 7: s = "\"loop\" expected"; break;
			case 8: s = "\"{\" expected"; break;
			case 9: s = "\"}\" expected"; break;
			case 10: s = "\"if(\" expected"; break;
			case 11: s = "\")\" expected"; break;
			case 12: s = "\"while(\" expected"; break;
			case 13: s = "\"for(\" expected"; break;
			case 14: s = "\"to\" expected"; break;
			case 15: s = "\"switch(\" expected"; break;
			case 16: s = "\"Delay(\" expected"; break;
			case 17: s = "\"Timer(\" expected"; break;
			case 18: s = "\"(\" expected"; break;
			case 19: s = "\",\" expected"; break;
			case 20: s = "\"else\" expected"; break;
			case 21: s = "\"if\" expected"; break;
			case 22: s = "\"case\" expected"; break;
			case 23: s = "\":\" expected"; break;
			case 24: s = "\"default\" expected"; break;
			case 25: s = "\"||\" expected"; break;
			case 26: s = "\"&\" expected"; break;
			case 27: s = "\"==\" expected"; break;
			case 28: s = "\"!=\" expected"; break;
			case 29: s = "\">=\" expected"; break;
			case 30: s = "\">\" expected"; break;
			case 31: s = "\"<=\" expected"; break;
			case 32: s = "\"<\" expected"; break;
			case 33: s = "\"!\" expected"; break;
			case 34: s = "\"+\" expected"; break;
			case 35: s = "\"-\" expected"; break;
			case 36: s = "\"*\" expected"; break;
			case 37: s = "\"/\" expected"; break;
			case 38: s = "\"%\" expected"; break;
			case 39: s = "\"void\" expected"; break;
			case 40: s = "\"int\" expected"; break;
			case 41: s = "\"float\" expected"; break;
			case 42: s = "\"double\" expected"; break;
			case 43: s = "\"string\" expected"; break;
			case 44: s = "\"boolean\" expected"; break;
			case 45: s = "??? expected"; break;
			case 46: s = "invalid TYPE"; break;
			case 47: s = "invalid STMT"; break;
			case 48: s = "invalid VALUE"; break;
			case 49: s = "invalid CASELIST"; break;
			case 50: s = "invalid CALL"; break;
			case 51: s = "invalid ELSEIF"; break;
			case 52: s = "invalid LOGI_EQUALOp"; break;
			case 53: s = "invalid LOGI_NOT"; break;
			case 54: s = "invalid LOGI_LGOp"; break;
			case 55: s = "invalid LOGI_TERM"; break;
			case 56: s = "invalid addExpr"; break;
			case 57: s = "invalid terminalExpr"; break;
			case 58: s = "invalid multExprOp"; break;

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