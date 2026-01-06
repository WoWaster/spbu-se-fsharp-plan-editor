module PlanEditor.DSL

open PlanEditor.Types

// type АудиторнаяРаботаBuilder() =
//     let mutable аудиторнаяРабота = базоваяАудиторнаяРабота

//     member _.Лекции(часы: int) =
//         аудиторнаяРабота <- { аудиторнаяРабота with Лекции = часы }


//     member _.Семинары(часы: int) =
//         аудиторнаяРабота <-
//             { аудиторнаяРабота with
//                 Семинары = часы }


//     member _.Консультации(часы: int) =
//         аудиторнаяРабота <-
//             { аудиторнаяРабота with
//                 Консультации = часы }


//     member _.ПрактическиеЗанятия(часы: int) =
//         аудиторнаяРабота <-
//             { аудиторнаяРабота with
//                 ПрактическиеЗанятия = часы }


//     member _.ЛабораторныеРаботы(часы: int) =
//         аудиторнаяРабота <-
//             { аудиторнаяРабота with
//                 ЛабораторныеРаботы = часы }


//     member _.Коллоквиумы(часы: int) =
//         аудиторнаяРабота <-
//             { аудиторнаяРабота with
//                 Коллоквиумы = часы }


//     member _.ТекущийКонтрольАудиторный(часы: int) =
//         аудиторнаяРабота <-
//             { аудиторнаяРабота with
//                 ТекущийКонтрольАудиторный = часы }


//     member _.ПромежуточнаяАттестацияАудиторная(часы: int) =
//         аудиторнаяРабота <-
//             { аудиторнаяРабота with
//                 ПромежуточнаяАттестацияАудиторная = часы }

//     member _.Zero() = базоваяАудиторнаяРабота

// let аудиторнаяРабота = new АудиторнаяРаботаBuilder()

// type ДисциплинаBuilder(номер: int, название: string, английскоеНазвание: string) =
//     let mutable дисциплина = базоваяДисциплина

//     member _.КодБлокаПоФГОС(код: КодБлокаПоФГОС) =
//         дисциплина <- { дисциплина with КодБлокаПоФГОС = код }

type АудиторнаяРаботаBuilder() =
    member _.Yield(()) = ()

    [<CustomOperation("лекции")>]
    member _.Лекции((), лекции: int) : АудиторнаяРабота = { Лекции = лекции }

let аудиторнаяРабота = АудиторнаяРаботаBuilder()

[<RequireQualifiedAccess>]
type DisciplineProperty =
    | Название of string
    | АудиторнаяРабота of АудиторнаяРабота

type DisciplineBuilder() =
    member _.Yield(()) = ()

    member _.Yield(auditoryWork: АудиторнаяРабота) =
        [ DisciplineProperty.АудиторнаяРабота auditoryWork ]

    member _.Delay(f: unit -> DisciplineProperty list) = f ()
    member _.Delay(f: unit -> DisciplineProperty) = [ f () ]

    member _.Combine(newProp: DisciplineProperty, previousProps: DisciplineProperty list) = newProp :: previousProps

    member this.For(prop: DisciplineProperty, f: unit -> DisciplineProperty list) = this.Combine(prop, f ())

    member _.Run(props) : Дисциплина =
        props
        |> List.fold
            (fun discipline prop ->
                match prop with
                | DisciplineProperty.Название название -> { discipline with Название = название }
                | DisciplineProperty.АудиторнаяРабота аудиторнаяРабота ->
                    { discipline with
                        АудиторнаяРабота = аудиторнаяРабота })
            базоваяДисциплина


    [<CustomOperation("название")>]
    member _.Название((), название: string) = DisciplineProperty.Название название

let дисциплина = DisciplineBuilder()
