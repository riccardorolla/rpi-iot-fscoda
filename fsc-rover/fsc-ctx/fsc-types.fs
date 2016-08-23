
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
let request(m:int,s:string) =
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
let response(m:int,s:string) =
  if true then
    failwith "Solved by JIT"
  else
    Fsc.FscContext.response(m,s)

[<TypedPred>]
[<Code>]
let stop(m:bool) =
  if true then
    failwith "Solved by JIT"
  else
    Fsc.FscContext.stop(m)