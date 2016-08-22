
module Fsc.Types

open CoDa


[<TypedPred>]
[<Code>]
let rover_command(s:string,t:string) =
  if true then
    failwith "Solved by JIT"
  else
    Fsc.FscContext.rover_command(s,t)


[<TypedPred>]
[<Code>]
let rover_request(m:int,s:string) =
  if true then
    failwith "Solved by JIT"
  else
    Fsc.FscContext.rover_request(m,s)
  
[<TypedPred>]
[<Code>]
let rover_validate(s:string,m:bool) =
  if true then
    failwith "Solved by JIT"
  else
    Fsc.FscContext.rover_validate(s,m)

[<TypedPred>]
[<Code>]
let stop(m:bool) =
  if true then
    failwith "Solved by JIT"
  else
    Fsc.FscContext.stop(m)