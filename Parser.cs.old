
using System;
using System.Collections.Generic;

namespace ALELA_Compiler {



public class Parser {
	public const int _EOF = 0;
	public const int _intnumber = 1;
	public const int _floatnumber = 2;
	public const int _id = 3;
	public const int _stringtext = 4;
	public const int _newline = 5;
	public const int maxT = 43;

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

	
	void ALELA() {
		List<AST> liste = new List<AST>(); List<AST> elist = new List<AST>(); AST e;
		DECL(out e);
		liste.Add(e); 
		while (StartOf(1)) {
			DECL(out e);
			liste.Add(e); 
		}
		Expect(6);
		BLOCK(out elist);
		liste.Add(new ProgSetup(elist)); 
		Expect(7);
		BLOCK(out elist);
		liste.Add(new ProgLoop(elist)); 
		while (StartOf(1)) {
			FUNCDECL(out e);
			liste.Add(e); 
		}
		ProgramAST = new Prog(liste); 
	}

	void DECL(out AST e) {
		SymDeclaring e1; AST e2 = null; 
		TYPE(out e1);
		Expect(3);
		string idval = t.val; e1.id = idval; 
		if (la.kind == 9) {
			ASSIG(out e2, idval);
		}
		e = new Decl(e1, e2); 
		Expect(8);
	}

	void BLOCK(out List<AST> eo ) {
		AST e; List<AST> e2 = new List<AST>(); 
		Expect(10);
		while (StartOf(2)) {
			STMT(out e);
			e2.Add(e); 
		}
		Expect(11);
		eo = e2; 
	}

	void FUNCDECL(out AST e) {
		SymDeclaring e1 ,e11; List<SymDeclaring> e2 = new List<SymDeclaring>();  List<AST> e3 = new List<AST>(); 
		TYPE(out e1);
		Expect(3);
		string idval = t.val; e1.id = idval; 
		Expect(14);
		if (StartOf(1)) {
			TYPE(out e11);
			Expect(3);
			e11.id = t.val; 
			while (la.kind == 23) {
				Get();
				TYPE(out e11);
				Expect(3);
				e11.id = t.val; 
			}
		}
		Expect(15);
		BLOCK(out e3);
		e = new FuncDecl(e1, e2, e3); 
	}

	void TYPE(out SymDeclaring e) {
		e = null; 
		if (la.kind == 38) {
			Get();
			e = new VoidDcl(); 
		} else if (la.kind == 39) {
			Get();
			e = new IntDcl(); 
		} else if (la.kind == 40) {
			Get();
			e = new FloatDcl(); 
		} else if (la.kind == 41) {
			Get();
			e = new StringDcl(); 
		} else if (la.kind == 42) {
			Get();
			e = new BooleanDcl(); 
		} else SynErr(44);
	}

	void ASSIG(out AST e, string idval) {
		e = null; 
		Expect(9);
		if (StartOf(3)) {
			EXPR(out e);
		} else if (la.kind == 4) {
			Get();
			e = new StringConst(t.val); 
		} else SynErr(45);
		e = new Assigning(idval, e); 
	}

	void EXPR(out AST e) {
		multExpr(out e);
		if (la.kind == 33 || la.kind == 34) {
			addExpr(e, out e);
		}
	}

	void STMT(out AST e) {
		AST logi, n1, n2; List<AST> stm = new List<AST>();
		List<AST> stm2 = new List<AST>(); string id; e = null; 
		switch (la.kind) {
		case 12: {
			Get();
			IFELSTMT(out e);
			break;
		}
		case 13: {
			Get();
			Expect(14);
			LOGI_EXPR(out logi);
			Expect(15);
			BLOCK(out stm);
			e = new WhileStmt(logi, stm); 
			break;
		}
		case 16: {
			Get();
			Expect(14);
			VALUE(out n1);
			Expect(17);
			VALUE(out n2);
			Expect(15);
			BLOCK(out stm);
			e = new ForStmt(n1, n2, stm); 
			break;
		}
		case 18: {
			Get();
			Expect(14);
			Expect(3);
			id = t.val; 
			Expect(15);
			Expect(10);
			Expect(19);
			Expect(3);
			id = t.val; 
			Expect(20);
			BLOCK(out stm2);
			stm.Add(new SwitchCase(id, stm2)); 
			while (la.kind == 19) {
				Get();
				Expect(3);
				id = t.val; 
				Expect(20);
				BLOCK(out stm2);
				stm.Add(new SwitchCase(id, stm2)); 
			}
			Expect(21);
			Expect(20);
			BLOCK(out stm2);
			stm.Add(new SwitchDefault(stm2)); 
			Expect(11);
			e = new SwitchStmt(id, stm); 
			break;
		}
		case 3: {
			Get();
			id = t.val; 
			CALL(id, out e);
			Expect(8);
			break;
		}
		case 38: case 39: case 40: case 41: case 42: {
			DECL(out e);
			break;
		}
		default: SynErr(46); break;
		}
	}

	void IFELSTMT(out AST e) {
		List<AST> stm, stm2 = new List<AST>(); AST els = null; 
		Expect(14);
		LOGI_EXPR(out AST logi);
		Expect(15);
		BLOCK(out stm);
		if (la.kind == 22) {
			Get();
			if (la.kind == 10) {
				BLOCK(out stm2);
				els = new ElseStmt(stm2); 
			} else if (la.kind == 12) {
				Get();
				IFELSTMT(out els);
			} else SynErr(47);
		}
		e = new IfStmt(logi, stm, els); 
	}

	void LOGI_EXPR(out AST e) {
		LOGI_AND(out e);
		if (la.kind == 24) {
			LOGI_OR(e, out e);
		}
	}

	void VALUE(out AST e) {
		e = null; 
		if (la.kind == 1) {
			Get();
			e = new IntConst(t.val); 
		} else if (la.kind == 2) {
			Get();
			e = new FloatConst(t.val); 
		} else if (la.kind == 3) {
			Get();
			e = new SymReferencing(t.val); 
		} else SynErr(48);
	}

	void CALL(string id, out AST e) {
		e = null; List<AST> stm = new List<AST>(); 
		if (la.kind == 14) {
			FUNC(out stm);
			e = new FunctionStmt(id, stm); 
		} else if (la.kind == 9) {
			ASSIG(out e, id);
		} else SynErr(49);
	}

	void FUNC(out List<AST> eo) {
		AST v; List<AST> e = new List<AST>(); 
		Expect(14);
		if (la.kind == 1 || la.kind == 2 || la.kind == 3) {
			VALUE(out v);
			e.Add(v); 
			while (la.kind == 23) {
				Get();
				VALUE(out v);
				e.Add(v); 
			}
		}
		Expect(15);
		eo= e; 
	}

	void LOGI_AND(out AST e) {
		LOGI_EQUAL(out e);
		if (la.kind == 25) {
			LOGI_ANDOp(e, out e);
		}
	}

	void LOGI_OR(AST e, out AST eo) {
		string op; AST e2; 
		Expect(24);
		op = t.val; 
		LOGI_EXPR(out e2);
		eo = new Expression(op, e, e2); 
	}

	void LOGI_EQUAL(out AST e) {
		LOGI_LG(out e);
		if (la.kind == 26 || la.kind == 27) {
			LOGI_EQUALOp(e, out e);
		}
	}

	void LOGI_ANDOp(AST e, out AST eo) {
		string op; AST e2; 
		Expect(25);
		op = t.val; 
		LOGI_EQUAL(out e2);
		eo = new Expression(op, e, e2); 
	}

	void LOGI_LG(out AST e) {
		LOGI_NOT(out e);
		if (StartOf(4)) {
			LOGI_LGOp(e, out e);
		}
	}

	void LOGI_EQUALOp(AST e, out AST eo) {
		string op; AST e2; 
		if (la.kind == 26) {
			Get();
		} else if (la.kind == 27) {
			Get();
		} else SynErr(50);
		op = t.val; 
		LOGI_LG(out e2);
		eo = new Expression(op, e, e2); 
	}

	void LOGI_NOT(out AST e) {
		AST e2; e = null; 
		if (la.kind == 32) {
			Get();
			LOGI_NOT(out e2);
			e = new NotExpression(e2); 
		} else if (StartOf(3)) {
			LOGI_TERM(out e);
		} else SynErr(51);
	}

	void LOGI_LGOp(AST e, out AST eo) {
		string op; AST e2; 
		if (la.kind == 28) {
			Get();
		} else if (la.kind == 29) {
			Get();
		} else if (la.kind == 30) {
			Get();
		} else if (la.kind == 31) {
			Get();
		} else SynErr(52);
		op = t.val; 
		LOGI_NOT(out e2);
		eo = new Expression(op, e, e2); 
	}

	void LOGI_TERM(out AST e) {
		e = null; 
		if (la.kind == 1 || la.kind == 2 || la.kind == 3) {
			VALUE(out e);
		} else if (la.kind == 14) {
			Get();
			LOGI_EXPR(out e);
			Expect(15);
		} else SynErr(53);
	}

	void multExpr(out AST e) {
		terminalExpr(out e);
		if (la.kind == 35 || la.kind == 36 || la.kind == 37) {
			multExprOp(e, out e);
		}
	}

	void addExpr(AST e, out AST eo) {
		string op; AST e2; 
		if (la.kind == 33) {
			Get();
		} else if (la.kind == 34) {
			Get();
		} else SynErr(54);
		op = t.val; 
		EXPR(out e2);
		eo = new Expression(op, e, e2); 
	}

	void terminalExpr(out AST e) {
		e = null; 
		if (la.kind == 1 || la.kind == 2 || la.kind == 3) {
			VALUE(out e);
		} else if (la.kind == 14) {
			Get();
			EXPR(out e);
			Expect(15);
		} else SynErr(55);
	}

	void multExprOp(AST e, out AST eo) {
		string op; AST e2; 
		if (la.kind == 35) {
			Get();
		} else if (la.kind == 36) {
			Get();
		} else if (la.kind == 37) {
			Get();
		} else SynErr(56);
		op = t.val; 
		multExpr(out e2);
		eo = new Expression(op, e, e2); 
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		ALELA();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_x, _x},
		{_x,_x,_x,_T, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_x,_x, _T,_x,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_x, _x},
		{_x,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x}

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
			case 1: s = "intnumber expected"; break;
			case 2: s = "floatnumber expected"; break;
			case 3: s = "id expected"; break;
			case 4: s = "stringtext expected"; break;
			case 5: s = "newline expected"; break;
			case 6: s = "\"setup\" expected"; break;
			case 7: s = "\"loop\" expected"; break;
			case 8: s = "\";\" expected"; break;
			case 9: s = "\"=\" expected"; break;
			case 10: s = "\"{\" expected"; break;
			case 11: s = "\"}\" expected"; break;
			case 12: s = "\"if\" expected"; break;
			case 13: s = "\"while\" expected"; break;
			case 14: s = "\"(\" expected"; break;
			case 15: s = "\")\" expected"; break;
			case 16: s = "\"for\" expected"; break;
			case 17: s = "\"to\" expected"; break;
			case 18: s = "\"switch\" expected"; break;
			case 19: s = "\"case\" expected"; break;
			case 20: s = "\":\" expected"; break;
			case 21: s = "\"default\" expected"; break;
			case 22: s = "\"else\" expected"; break;
			case 23: s = "\",\" expected"; break;
			case 24: s = "\"||\" expected"; break;
			case 25: s = "\"&\" expected"; break;
			case 26: s = "\"==\" expected"; break;
			case 27: s = "\"!=\" expected"; break;
			case 28: s = "\">=\" expected"; break;
			case 29: s = "\">\" expected"; break;
			case 30: s = "\"<=\" expected"; break;
			case 31: s = "\"<\" expected"; break;
			case 32: s = "\"!\" expected"; break;
			case 33: s = "\"+\" expected"; break;
			case 34: s = "\"-\" expected"; break;
			case 35: s = "\"*\" expected"; break;
			case 36: s = "\"/\" expected"; break;
			case 37: s = "\"%\" expected"; break;
			case 38: s = "\"void\" expected"; break;
			case 39: s = "\"int\" expected"; break;
			case 40: s = "\"float\" expected"; break;
			case 41: s = "\"string\" expected"; break;
			case 42: s = "\"boolean\" expected"; break;
			case 43: s = "??? expected"; break;
			case 44: s = "invalid TYPE"; break;
			case 45: s = "invalid ASSIG"; break;
			case 46: s = "invalid STMT"; break;
			case 47: s = "invalid IFELSTMT"; break;
			case 48: s = "invalid VALUE"; break;
			case 49: s = "invalid CALL"; break;
			case 50: s = "invalid LOGI_EQUALOp"; break;
			case 51: s = "invalid LOGI_NOT"; break;
			case 52: s = "invalid LOGI_LGOp"; break;
			case 53: s = "invalid LOGI_TERM"; break;
			case 54: s = "invalid addExpr"; break;
			case 55: s = "invalid terminalExpr"; break;
			case 56: s = "invalid multExprOp"; break;

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