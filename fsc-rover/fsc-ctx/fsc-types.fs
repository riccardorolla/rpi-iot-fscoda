
module Fsc.Types

open CoDa


[<TypedPred>]
[<Code>]
let result(s:string,t:string) =
  if true then
    failwith "Solved by JIT"
  else
    Fsc.FscContext.result(s,t)


[<TypedPred>]
[<Code>]
let request(m:string,s:string,p:string) =
  if true then
    failwith "Solved by JIT"
  else
    Fsc.FscContext.request(m,s,p)
  
[<TypedPred>]
[<Code>]
let found(s:string,m:string) =
  if true then
    failwith "Solved by JIT"
  else
    Fsc.FscContext.found(s,m)

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

[<TypedPred>]
[<Code>]
let user_command(x:string,y:string) =
    if true then
      failwith "Solved by JIT"
    else
      Fsc.FscContext.user_command(x,y)
  