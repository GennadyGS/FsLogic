﻿namespace FsLogic

module NumericLiteralZ =
    open System
    open Goal

    let FromZero() = prim 0
    let FromOne() = prim 1
    let FromInt32 (a:int) = prim a
    let FromInt64 (a:int64) = prim (int a)
    let FromString (s:string) = prim (Int32.Parse s)

module Relations =

    open Goal

    ///Tries goal an unbounded number of times.
    let rec anyo goal =
        recurse (fun () -> goal ||| anyo goal)

    ///Goal that succeeds an unbounded number of times.
    let alwayso = anyo succeed

    ///Goal that fails an unbounded number of times.
    let nevero = anyo fail

    ///Non-relational. The given goal succeeds at most once.
    let onceo goal = condu [ [ goal ] ]

    //---lists----

    ///Relates l with the empty lst.
    let emptyo l = l *=* nil

    ///Relates h and t with the list l such that (h::t) = l.
    let conso h t l = cons h t *=* l

    ///Relates h with the list l such that (h::_) = l.
    let heado h l =
        let t = fresh()
        conso h t l

    ///Relates t with the list l such that (_::t) = l.
    let tailo t l =
        let h = fresh()
        conso h t l

    ///Relates l,s and out so that l @ s = out.
    let rec appendo l s out = 
        let a,d,res = fresh()
        matche l
            [ nil       ->> [ s *=* out ]
              cons a d  ->> [ conso a res out
                              recurse (fun () -> appendo d s res) ]
            ]

    /// Relates x,l and out such that out is l with 
    /// the first occurence of x in l removed.
    let rec removeo x l out =
        let a,d,res = fresh()
        matche l
            [ nil       ->> [ l *=* out ]
              cons a d  ->> [ (a *=* x &&& d *=* out)
                              ||| (a *<>* x &&& conso a res out &&& recurse (fun () -> removeo x d res))]
            ]