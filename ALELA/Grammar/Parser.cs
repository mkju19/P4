
using System;
using System.Collections.Generic;

namespace ALELA_Compiler {



public class Parser {
	public const int _EOF = 0;
	public const int _intnumber = 1;
	public const int _floatnumber = 2;
	public const int _id = 3;
	public const int _true = 4;
	public const int _false = 5;
	public const int _stringtext = 6;
	public const int _newline = 7;
	public const int maxT = 48;

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
		List<AST> astList = new List<AST>(); List<AST> nodeList = new List<AST>();
		AST e; SymDeclaring e1 = null; AST idval; 
		DECL(out e);
		astList.Add(e); 
		while (StartOf(1)) {
			DECL(out e);
			astList.Add(e); 
		}
		Expect(8);
		BLOCK(out nodeList);
		astList.Add(new ProgSetup(nodeList)); 
		Expect(9);
		BLOCK(out nodeList);
		astList.Add(new ProgLoop(nodeList)); 
		while (StartOf(1)) {
			DCL(out e1, out idval);
			FUNCDECL(out e, e1);
			astList.Add(e); 
		}
		ProgramAST = new Prog(astList); 
	}

	void DECL(out AST e) {
		SymDeclaring e1; AST e2 = null; StructDecl es = null;
		AST idval; List<AST> el = new List<AST>(); 
		DCL(out e1, out idval);
		if (la.kind == 3 || la.kind == 10 || la.kind == 12) {
			if (la.kind == 10 || la.kind == 12) {
				ASSIG(out e2, idval);
			} else {
				Get();
				AST structid = new SymReferencing(t.val); string idofstruct = t.val; 
				if (la.kind == 10) {
					Get();
					STRUCTDECL(out el);
				}
				es = new StructDecl(idval, el); es.structId = structid; 
				e2 = new Assigning(structid, es); e1.id = idofstruct;
			}
		}
		e = new Decl(e1, e2); 
		Expect(11);
	}

	void BLOCK(out List<AST> eo ) {
		AST e; List<AST> e2 = new List<AST>(); 
		Expect(10);
		while (StartOf(2)) {
			STMT(out e);
			e2.Add(e); 
		}
		Expect(13);
		eo = e2; 
	}

	void DCL(out SymDeclaring e, out AST idval) {
		TYPE(out e);
		Expect(3);
		idval = new SymReferencing(t.val); e.id = t.val; 
	}

	void FUNCDECL(out AST eo, SymDeclaring e1) {
		AST e = null; AST rv = null; SymDeclaring e11; List<SymDeclaring> e2 = new List<SymDeclaring>();  List<AST> e3 = new List<AST>(); 
		Expect(17);
		if (StartOf(1)) {
			TYPE(out e11);
			Expect(3);
			e11.id = t.val; e2.Add(e11); 
			while (la.kind == 14) {
				Get();
				TYPE(out e11);
				Expect(3);
				e11.id = t.val;  e2.Add(e11);
			}
		}
		Expect(18);
		Expect(10);
		while (StartOf(2)) {
			STMT(out e);
			e3.Add(e); 
		}
		if (la.kind == 26) {
			Get();
			EXPR(out rv);
			Expect(11);
		}
		Expect(13);
		eo = new FuncDecl(e1, e2, e3, rv); 
	}

	void ASSIG(out AST e, AST idval) {
		e = null; 
		if (la.kind == 12) {
			Get();
			EXPR(out e);
		} else if (la.kind == 10) {
			Get();
			STRUCTDEF(out e, idval);
		} else SynErr(49);
		e = new Assigning(idval, e); 
	}

	void STRUCTDECL(out List<AST> eo) {
		AST e; List<AST> e2 = new List<AST>(); 
		Expect(3);
		AST idval = new SymReferencing(t.val); 
		ASSIG(out e, idval);
		e2.Add(e); 
		while (la.kind == 14) {
			Get();
			Expect(3);
			idval = new SymReferencing(t.val); 
			ASSIG(out e, idval);
			e2.Add(e); 
		}
		Expect(13);
		eo = e2; 
	}

	void TYPE(out SymDeclaring e) {
		e = null; SymDeclaring lt = null; 
		switch (la.kind) {
		case 41: {
			Get();
			e = new VoidDcl(); 
			break;
		}
		case 42: {
			Get();
			e = new IntDcl(); 
			break;
		}
		case 43: {
			Get();
			e = new FloatDcl(); 
			break;
		}
		case 44: {
			Get();
			e = new StringDcl(); 
			break;
		}
		case 45: {
			Get();
			e = new BooleanDcl(); 
			break;
		}
		case 46: {
			Get();
			e = new StructDcl(); 
			break;
		}
		case 47: {
			Get();
			Expect(34);
			TYPE(out lt);
			Expect(32);
			e = new ListDcl(lt); 
			break;
		}
		default: SynErr(50); break;
		}
	}

	void EXPR(out AST e) {
		multExpr(out e);
		if (la.kind == 36 || la.kind == 37) {
			addExpr(e, out e);
		}
	}

	void STRUCTDEF(out AST eo, AST idval) {
		List<AST> e2 = new List<AST>(); AST e; StructDef ed; 
		STRUCTFIELD(out e);
		e2.Add(e); 
		while (StartOf(1)) {
			STRUCTFIELD(out e);
			e2.Add(e); 
		}
		Expect(13);
		ed = new StructDef(e2); ed.structType = idval; eo = ed; 
	}

	void STRUCTFIELD(out AST eo) {
		AST idval; SymDeclaring e; eo = null; 
		DCL(out e, out idval);
		if (la.kind == 11) {
			Get();
			eo = e; 
		} else if (la.kind == 17) {
			FUNCDECL(out eo, e);
		} else SynErr(51);
	}

	void STMT(out AST e) {
		AST logi, n1, n2; List<AST> stm = new List<AST>();
		List<AST> stm2 = new List<AST>(); string id; AST astid; e = null; 
		switch (la.kind) {
		case 15: {
			Get();
			IFELSTMT(out e);
			break;
		}
		case 16: {
			Get();
			Expect(17);
			LOGI_EXPR(out logi);
			Expect(18);
			BLOCK(out stm);
			e = new WhileStmt(logi, stm); 
			break;
		}
		case 19: {
			Get();
			Expect(17);
			n1 = null; 
			if (StartOf(1)) {
				DECL(out n1);
			} else if (la.kind == 3) {
				Get();
				n1 = new SymReferencing(t.val); 
				Expect(11);
			} else SynErr(52);
			LOGI_EXPR(out logi);
			Expect(11);
			Expect(3);
			astid = new SymReferencing(t.val); 
			ASSIG(out n2, astid);
			Expect(18);
			BLOCK(out stm);
			e = new ForStmt(n1, logi, n2, stm); 
			break;
		}
		case 20: {
			Get();
			Expect(17);
			Expect(3);
			id = t.val; 
			Expect(18);
			Expect(10);
			Expect(21);
			Expect(3);
			id = t.val; 
			Expect(22);
			BLOCK(out stm2);
			stm.Add(new SwitchCase(id, stm2)); 
			while (la.kind == 21) {
				Get();
				Expect(3);
				id = t.val; 
				Expect(22);
				BLOCK(out stm2);
				stm.Add(new SwitchCase(id, stm2)); 
			}
			Expect(23);
			Expect(22);
			BLOCK(out stm2);
			stm.Add(new SwitchDefault(stm2)); 
			Expect(13);
			e = new SwitchStmt(id, stm); 
			break;
		}
		case 3: {
			IDENTIFIER(out e);
			CALL(e, out e);
			Expect(11);
			break;
		}
		case 41: case 42: case 43: case 44: case 45: case 46: case 47: {
			DECL(out e);
			break;
		}
		default: SynErr(53); break;
		}
	}

	void IFELSTMT(out AST e) {
		List<AST> stm, stm2 = new List<AST>(); AST els = null; 
		Expect(17);
		LOGI_EXPR(out AST logi);
		Expect(18);
		BLOCK(out stm);
		if (la.kind == 24) {
			Get();
			if (la.kind == 10) {
				BLOCK(out stm2);
				els = new ElseStmt(stm2); 
			} else if (la.kind == 15) {
				Get();
				IFELSTMT(out els);
			} else SynErr(54);
		}
		e = new IfStmt(logi, stm, els); 
	}

	void LOGI_EXPR(out AST e) {
		LOGI_AND(out e);
		if (la.kind == 27) {
			LOGI_OR(e, out e);
		}
	}

	void IDENTIFIER(out AST e) {
		e = null; List<AST> elist = new List<AST>(); AST ei = null; 
		Expect(3);
		string id = t.val; e = new SymReferencing(id); 
		if (la.kind == 25) {
			Get();
			IDENTIFIER(out ei);
			e = new DotReferencing(e, ei); 
		}
	}

	void CALL(AST id, out AST e) {
		e = null; List<AST> stm = new List<AST>(); 
		if (la.kind == 17) {
			FUNC(out stm);
			e = new FunctionStmt(id, stm); 
		} else if (la.kind == 10 || la.kind == 12) {
			ASSIG(out e, id);
		} else SynErr(55);
	}

	void FUNC(out List<AST> eo) {
		AST v; List<AST> e = new List<AST>(); 
		Expect(17);
		if (StartOf(3)) {
			EXPR(out v);
			e.Add(v); 
			while (la.kind == 14) {
				Get();
				EXPR(out v);
				e.Add(v); 
			}
		}
		Expect(18);
		eo = e; 
	}

	void VALUE(out AST e) {
		e = null; List<AST> elist = new List<AST>(); 
		if (la.kind == 4 || la.kind == 5) {
			BOOL();
			e = new BooleanConst(t.val); 
		} else if (la.kind == 1) {
			Get();
			e = new IntConst(t.val); 
		} else if (la.kind == 2) {
			Get();
			e = new FloatConst(t.val); 
		} else if (la.kind == 6) {
			Get();
			e = new StringConst(t.val); 
		} else if (la.kind == 3) {
			IDENTIFIER(out e);
			if (la.kind == 17) {
				FUNC(out elist);
				e = new FunctionStmt(e, elist); 
			}
		} else SynErr(56);
	}

	void BOOL() {
		if (la.kind == 4) {
			Get();
		} else if (la.kind == 5) {
			Get();
		} else SynErr(57);
	}

	void LOGI_AND(out AST e) {
		LOGI_EQUAL(out e);
		if (la.kind == 28) {
			LOGI_ANDOp(e, out e);
		}
	}

	void LOGI_OR(AST e, out AST eo) {
		string op; AST e2; 
		Expect(27);
		op = t.val; 
		LOGI_EXPR(out e2);
		eo = new LogiExpression(op, e, e2); 
	}

	void LOGI_EQUAL(out AST e) {
		LOGI_LG(out e);
		if (la.kind == 29 || la.kind == 30) {
			LOGI_EQUALOp(e, out e);
		}
	}

	void LOGI_ANDOp(AST e, out AST eo) {
		string op; AST e2; 
		Expect(28);
		op = t.val; 
		LOGI_EQUAL(out e2);
		eo = new LogiExpression(op, e, e2); 
	}

	void LOGI_LG(out AST e) {
		LOGI_NOT(out e);
		if (StartOf(4)) {
			LOGI_LGOp(e, out e);
		}
	}

	void LOGI_EQUALOp(AST e, out AST eo) {
		string op; AST e2; 
		if (la.kind == 29) {
			Get();
		} else if (la.kind == 30) {
			Get();
		} else SynErr(58);
		op = t.val; 
		LOGI_LG(out e2);
		eo = new LogiExpression(op, e, e2); 
	}

	void LOGI_NOT(out AST e) {
		AST e2; e = null; 
		if (la.kind == 35) {
			Get();
			LOGI_NOT(out e2);
			e = new NotExpression(e2); 
		} else if (StartOf(3)) {
			LOGI_TERM(out e);
		} else SynErr(59);
	}

	void LOGI_LGOp(AST e, out AST eo) {
		string op; AST e2; 
		if (la.kind == 31) {
			Get();
		} else if (la.kind == 32) {
			Get();
		} else if (la.kind == 33) {
			Get();
		} else if (la.kind == 34) {
			Get();
		} else SynErr(60);
		op = t.val; 
		LOGI_NOT(out e2);
		eo = new LogiExpression(op, e, e2); 
	}

	void LOGI_TERM(out AST e) {
		e = null; 
		EXPR(out e);
	}

	void multExpr(out AST e) {
		terminalExpr(out e);
		if (la.kind == 38 || la.kind == 39 || la.kind == 40) {
			multExprOp(e, out e);
		}
	}

	void addExpr(AST e, out AST eo) {
		string op; AST e2; 
		if (la.kind == 36) {
			Get();
		} else if (la.kind == 37) {
			Get();
		} else SynErr(61);
		op = t.val; 
		EXPR(out e2);
		eo = new Expression(op, e, e2); 
	}

	void terminalExpr(out AST e) {
		e = null; 
		if (StartOf(5)) {
			VALUE(out e);
		} else if (la.kind == 17) {
			Get();
			LOGI_EXPR(out e);
			Expect(18);
		} else SynErr(62);
	}

	void multExprOp(AST e, out AST eo) {
		string op; AST e2; 
		if (la.kind == 38) {
			Get();
		} else if (la.kind == 39) {
			Get();
		} else if (la.kind == 40) {
			Get();
		} else SynErr(63);
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
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _x,_x},
		{_x,_x,_x,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_x,_x,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x}

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
			case 4: s = "true expected"; break;
			case 5: s = "false expected"; break;
			case 6: s = "stringtext expected"; break;
			case 7: s = "newline expected"; break;
			case 8: s = "\"setup\" expected"; break;
			case 9: s = "\"loop\" expected"; break;
			case 10: s = "\"{\" expected"; break;
			case 11: s = "\";\" expected"; break;
			case 12: s = "\"=\" expected"; break;
			case 13: s = "\"}\" expected"; break;
			case 14: s = "\",\" expected"; break;
			case 15: s = "\"if\" expected"; break;
			case 16: s = "\"while\" expected"; break;
			case 17: s = "\"(\" expected"; break;
			case 18: s = "\")\" expected"; break;
			case 19: s = "\"for\" expected"; break;
			case 20: s = "\"switch\" expected"; break;
			case 21: s = "\"case\" expected"; break;
			case 22: s = "\":\" expected"; break;
			case 23: s = "\"default\" expected"; break;
			case 24: s = "\"else\" expected"; break;
			case 25: s = "\".\" expected"; break;
			case 26: s = "\"return\" expected"; break;
			case 27: s = "\"||\" expected"; break;
			case 28: s = "\"&&\" expected"; break;
			case 29: s = "\"==\" expected"; break;
			case 30: s = "\"!=\" expected"; break;
			case 31: s = "\">=\" expected"; break;
			case 32: s = "\">\" expected"; break;
			case 33: s = "\"<=\" expected"; break;
			case 34: s = "\"<\" expected"; break;
			case 35: s = "\"!\" expected"; break;
			case 36: s = "\"+\" expected"; break;
			case 37: s = "\"-\" expected"; break;
			case 38: s = "\"*\" expected"; break;
			case 39: s = "\"/\" expected"; break;
			case 40: s = "\"%\" expected"; break;
			case 41: s = "\"void\" expected"; break;
			case 42: s = "\"int\" expected"; break;
			case 43: s = "\"float\" expected"; break;
			case 44: s = "\"string\" expected"; break;
			case 45: s = "\"boolean\" expected"; break;
			case 46: s = "\"struct\" expected"; break;
			case 47: s = "\"List\" expected"; break;
			case 48: s = "??? expected"; break;
			case 49: s = "invalid ASSIG"; break;
			case 50: s = "invalid TYPE"; break;
			case 51: s = "invalid STRUCTFIELD"; break;
			case 52: s = "invalid STMT"; break;
			case 53: s = "invalid STMT"; break;
			case 54: s = "invalid IFELSTMT"; break;
			case 55: s = "invalid CALL"; break;
			case 56: s = "invalid VALUE"; break;
			case 57: s = "invalid BOOL"; break;
			case 58: s = "invalid LOGI_EQUALOp"; break;
			case 59: s = "invalid LOGI_NOT"; break;
			case 60: s = "invalid LOGI_LGOp"; break;
			case 61: s = "invalid addExpr"; break;
			case 62: s = "invalid terminalExpr"; break;
			case 63: s = "invalid multExprOp"; break;

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