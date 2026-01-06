open PlanEditor.Types
open PlanEditor.DSL

let примерДисциплины =
    дисциплина {
        название "Программирование на F#"
        аудиторнаяРабота { лекции 30 }
    }

printfn "%A" примерДисциплины
