
module Fsc.Types

open CoDa


[<TypedPred>]
[<Code>]
let execute(s:string,t:string) =
  if true then
    failwith "Solved by JIT"
  else
    Fsc.FscContext.execute(s,t)


[<TypedPred>]
[<Code>]
let request(m:string,s:string) =
  if true then
    failwith "Solved by JIT"
  else
    Fsc.FscContext.request(m,s)
  
[<TypedPred>]
[<Code>]
let observe(s:string,m:string) =
  if true then
    failwith "Solved by JIT"
  else
    Fsc.FscContext.observe(s,m)

[<TypedPred>]
[<Code>]
let response(m:string,s:string) =
  if true then
    failwith "Solved by JIT"
  else
    Fsc.FscContext.response(m,s)

[<TypedPred>]
[<Code>]
let rule(x:string,y:string,z:string) =
  if true then
    failwith "Solved by JIT"
  else
    Fsc.FscContext.rule(x,y,z)

[<TypedPred>]
[<Code>]
let next(x:string,y:string) =
   if true then
     failwith "Solved by JIT"
   else
     Fsc.FscContext.next(x,y)
 