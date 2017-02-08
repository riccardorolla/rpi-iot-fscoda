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
let request(m:string,s:string) =
  if true then
    failwith "Solved by JIT"
  else
    Fsc.FscContext.request(m,s)
  

[<TypedPred>]
[<Code>]
let response(m:string,s:string) =
  if true then
    failwith "Solved by JIT"
  else
    Fsc.FscContext.response(m,s)

[<TypedPred>]
[<Code>]
let action(x:string,y:string,z:string) =
  if true then
    failwith "Solved by JIT"
  else
    Fsc.FscContext.action(x,y,z)

[<TypedPred>]
[<Code>]
let next(x:string) =
   if true then
     failwith "Solved by JIT"
   else
     Fsc.FscContext.next(x)

[<TypedPred>]
[<Code>]
let usrcmd(x:string,y:string) =
    if true then
      failwith "Solved by JIT"
    else
      Fsc.FscContext.usrcmd(x,y)
[<TypedPred>]
[<Code>]
let usrcmddesc(x:string,y:string) =
    if true then
      failwith "Solved by JIT"
    else
      Fsc.FscContext.usrcmddesc(x,y)


[<TypedPred>]
[<Code>]
let confidence(x:string,y:float,z:float) =
    if true then
      failwith "Solved by JIT"
    else
      Fsc.FscContext.confidence(x,y,z)

[<TypedPred>]
[<Code>]
let recognition(x:string,y:float) =
    if true then
      failwith "Solved by JIT"
    else
      Fsc.FscContext.recognition(x,y)


[<TypedPred>]
[<Code>]
let cmddesc(x:string,y:string) =
    if true then
      failwith "Solved by JIT"
    else
      Fsc.FscContext.cmddesc(x,y)

[<TypedPred>]
[<Code>]
let detected(x:string) =
    if true then
      failwith "Solved by JIT"
    else
      Fsc.FscContext.detected(x)

[<TypedPred>]
[<Code>]
let undetected(x:string) =
    if true then
      failwith "Solved by JIT"
    else
      Fsc.FscContext.detected(x)