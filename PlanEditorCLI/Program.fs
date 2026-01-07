open PlanEditor.Types
open PlanEditor.DSL

[<EntryPoint>]
let main argv =
    let impl1 =
        implementation {
            semester 1
            laborIntensity 3
            realization ""
            trajectory ""
            monitoring [Зачет]
            workHoursFromString "32 0 0 14 0 2 0 0 2 0 0 54 0 4 10"
            competences [
                "ОПК-2 - Способен применять современный математический аппарат..."
                "ПКП-1-ИП-ПК-1 - Способен разрабатывать и отлаживать программный код"
            ]
        }

    let impl2 =
        implementation {
            semester 2
            laborIntensity 3
            realization ""
            trajectory ""
            monitoring [Экзамен]
            workHoursFromString "30 0 2 16 0 0 0 0 2 0 0 30 0 28 10"
            competences []
        }

    let discreteMath =
        course {
            code "002180"
            russianName "Дискретная математика"
            englishName "Discrete Mathematics"
            courseType Base
            implementations [ impl1; impl2 ]
        }

    printfn "=== Курс Дискретная математика ==="
    printfn "%A" discreteMath

    0 // возвращаем код выхода