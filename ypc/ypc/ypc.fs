module YPCompiler

open YieldProlog
open System
open System.IO

 
[<EntryPoint>]
let main args =
  if args.Length<3 then
   printfn "Usage: ypc [namespace] [.P file] [.cs file]"
   1
  else
   let outFile = new StreamWriter(args.[2])
   fprintf outFile "// Compiler output follows.\nnamespace %s { \nusing System; \nusing System.Collections.Generic;\nusing YieldProlog;\n" args.[0] 
   fprintf outFile "public class %sContext {\n" args.[0] 
  
   YP.tell(outFile)
  
   YP.see(new StreamReader(args.[1]))
   let TermList = new Variable()
   let PseudoCode = new Variable()
   for l1 in Parser.parseInput(TermList) do
     for l2 in Compiler.makeFunctionPseudoCode(TermList, PseudoCode) do
      Compiler.convertFunctionCSharp(PseudoCode)
   YP.seen()
   fprintf outFile "}}" 
   outFile.Close()
   0
