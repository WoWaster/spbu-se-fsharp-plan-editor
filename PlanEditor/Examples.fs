module PlanEditor.Examples

open PlanEditor.Types
open PlanEditor.Builders

let примерДисциплины =
    дисциплина {
        название "Программирование на F#"
        аудиторнаяРабота { лекции 30 }
    }

printfn "%A" примерДисциплины
