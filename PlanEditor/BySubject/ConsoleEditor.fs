module PlanEditor.ConsoleEditor

open PlanEditor.Types
open PlanEditor.DSL
open System
open System.Text.Json
open System.Text.Json.Serialization

type CourseTypeConverter() =
    inherit JsonConverter<CourseType>()

    override _.Read(reader, _, _) =
        match reader.GetString() with
        | null | "" -> Base
        | s ->
            match s with
            | _ when s.Equals("Base", StringComparison.OrdinalIgnoreCase)       -> Base
            | _ when s.Equals("Elective", StringComparison.OrdinalIgnoreCase)   -> Elective
            | _ when s.Equals("Facultative", StringComparison.OrdinalIgnoreCase)-> Facultative
            | _ -> failwithf "Неизвестный тип курса: %s" s

    override _.Write(writer, value, _) =
        let str = 
            match value with
            | Base        -> "Base"
            | Elective    -> "Elective"
            | Facultative -> "Facultative"
        writer.WriteStringValue(str)

// 2. WorkHours → строка с 15 числами
type WorkHoursStringConverter() =
    inherit JsonConverter<WorkHoursDistribution>()

    override _.Read(reader, _, _) =
        parseWorkHours (reader.GetString())

    override _.Write(writer, value, _) =
        writer.WriteStringValue(
            $"{value.Lecture} {value.Seminar} {value.Consultation} {value.Practical} " +
            $"{value.Lab} {value.Colloquium} {value.CurrentControl} {value.InterimAssessment} " +
            $"{value.ControlWorks} {value.WithTeacherPresence} {value.WithTeacher} " +
            $"{value.WithMethodologicalMaterials} {value.CurrentControlIndependent} " +
            $"{value.MidtermAssessment} {value.TotalIndependentWork}")

let jsonOptions = JsonSerializerOptions(
    WriteIndented = true,
    PropertyNamingPolicy = null,
    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
)

do
    jsonOptions.Converters.Add(CourseTypeConverter())
    jsonOptions.Converters.Add(WorkHoursStringConverter())

let serializeCourse (course: Course) : string =
    JsonSerializer.Serialize(course, jsonOptions)

let serializeCourseList (courses: Course list) : string =
    JsonSerializer.Serialize(courses, jsonOptions)

let deserializeCourse (json: string) : Course =
    JsonSerializer.Deserialize<Course>(json, jsonOptions)

let deserializeCourseList (json: string) : Course list =
    JsonSerializer.Deserialize<Course list>(json, jsonOptions)

let saveToFile (filename: string) (courses: Course list) =
    try
        let json = serializeCourseList courses
        use sw = new System.IO.StreamWriter(filename, false, System.Text.Encoding.UTF8)
        sw.Write(json)
        printfn "✓ Сохранено"
        true
    with _ -> false


let loadFromFile (filename: string) =
    try
        if System.IO.File.Exists(filename) then
            use sr = new System.IO.StreamReader(filename, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks = false)
            let json = sr.ReadToEnd()
            deserializeCourseList json
        else []
    with _ -> []

// Вспомогательные функции для парсинга
let parseBlockCode (s: string) : FgosBlockCode option =
    match s.ToLower().Trim() with
    | "дисциплины" | "disciplines" | "1" -> Some Disciplines
    | "практика" | "practicaltraining" | "2" -> Some PracticalTraining
    | "гиа" | "gia" | "3" -> Some Gia
    | _ -> None

let parseCourseType (s: string) : CourseType option =
    match s.ToLower().Trim() with
    | "базовая" | "base" | "1" -> Some Base
    | "электив" | "elective" | "2" -> Some Elective
    | "факультатив" | "facultative" | "3" -> Some Facultative
    | _ -> None

let parseMonitoringTypeList (input: string) : string =
    if String.IsNullOrWhiteSpace(input) then ""
    else
        let parts = input.Split([|','; ' '|], StringSplitOptions.RemoveEmptyEntries)
        let converted = parts |> Array.map (fun s ->
            match s.ToLower().Trim() with
            | "1" | "экзамен" -> "Экзамен"
            | "2" | "зачет" | "зачёт" -> "Зачет"
            | "3" | "аттестационноеиспытание" | "аттестация" -> "АттестационноеИспытание"
            | other -> other
        )
        String.Join(", ", converted)

let parseWorkHours (s: string) : WorkHoursDistribution =
    let nums =
        s.Split(' ', System.StringSplitOptions.RemoveEmptyEntries)
        |> Array.map System.Int32.Parse
        |> Array.toList

    match nums with
    | [l; se; c; p; lb; cw; col; cc; ia; wtp; wt; wmm; cci; ma; tiw] ->
        { Lecture = l
          Seminar = se
          Consultation = c
          Practical = p
          Lab = lb
          ControlWorks = cw
          Colloquium = col
          CurrentControl = cc
          InterimAssessment = ia
          WithTeacherPresence = wtp
          WithTeacher = wt
          WithMethodologicalMaterials = wmm
          CurrentControlIndependent = cci
          MidtermAssessment = ma
          TotalIndependentWork = tiw }
    | _ -> failwith "WorkHours string must contain exactly 15 integers"

module ConsoleDSL =
    let createImplementationFromConsole() : Implementation =
        printfn "\n=== Создание реализации ==="
        
        printf "Семестр: "
        let semester = Int32.Parse(Console.ReadLine())
        
        printf "Трудоёмкость (з.е.): "
        let laborIntensity = Int32.Parse(Console.ReadLine())
        
        printfn "Тип блока:"
        printfn "  1. Дисциплины"
        printfn "  2. Практика"
        printfn "  3. ГИА"
        printf "Выбор [1]: "
        let blockChoice = Console.ReadLine()
        let blockCode = 
            match parseBlockCode (if String.IsNullOrEmpty(blockChoice) then "1" else blockChoice) with
            | Some bc -> bc
            | None -> Disciplines
        
        printf "Компетенции (через запятую): "
        let competences = 
            Console.ReadLine().Split(',', StringSplitOptions.RemoveEmptyEntries)
            |> Array.map (fun s -> s.Trim())
            |> Array.toList
        
        printfn "Формы контроля (через запятую или пробел):"
        printfn "  1. Экзамен"
        printfn "  2. Зачет"
        printfn "  3. Аттестационное испытание"
        printfn "  4. Текущий контроль"
        printf "Выбор: "
        let monitoringTypes = parseMonitoringTypeList (Console.ReadLine())
        
        printf "Реализация (опционально): "
        let realization = Console.ReadLine()
        
        printf "Траектория (опционально): "
        let trajectory = Console.ReadLine()
        
        printf "Распределение часов (15 чисел через пробел): "
        let workHoursStr = Console.ReadLine()
        
        { emptyImplementation with
            Semester = semester
            LaborIntensity = laborIntensity
            BlockCode = blockCode
            Competences = competences
            MonitoringTypes = monitoringTypes
            Realization = realization
            Trajectory = trajectory
            WorkHours = parseWorkHours workHoursStr }

    let createCourseFromConsole() : Course =
        printfn "\n=== Создание нового курса ==="
        
        printf "Код курса: "
        let code = Console.ReadLine()
        
        printf "Русское название: "
        let russianName = Console.ReadLine()
        
        printf "Английское название: "
        let englishName = Console.ReadLine()
        
        printfn "Тип курса:"
        printfn "  1. Базовая"
        printfn "  2. Электив"
        printfn "  3. Факультатив"
        printf "Выбор [1]: "
        let typeChoice = Console.ReadLine()
        let courseType = 
            match parseCourseType (if String.IsNullOrEmpty(typeChoice) then "1" else typeChoice) with
            | Some ct -> ct
            | None -> Base
        
        printf "Добавить реализации сейчас? (y/n): "
        let addImplsNow = Console.ReadLine().ToLower() = "y"
        
        let implementations = 
            if addImplsNow then
                let rec collectImplementations acc =
                    let impl = createImplementationFromConsole()
                    printf "Добавить ещё одну реализацию? (y/n): "
                    if Console.ReadLine().ToLower() = "y" then
                        collectImplementations (impl :: acc)
                    else
                        impl :: acc |> List.rev
                collectImplementations []
            else
                []
        
        { emptyCourse with
            Code = code
            RussianName = russianName
            EnglishName = englishName
            Type = courseType
            Implementations = implementations }
    let editCourseWithDSL (originalCourse: Course) : Course =
        let rec editLoop (current: Course) =
            printfn "\n=== Редактирование курса ==="
            printfn "Текущие значения:"
            printfn "  1. Код: %s" current.Code
            printfn "  2. Русское название: %s" current.RussianName
            printfn "  3. Английское название: %s" current.EnglishName
            printfn "  4. Тип: %A" current.Type
            printfn "  5. Реализации: %d шт." current.Implementations.Length
            printfn "  6. Блоки выбора: %d шт." current.ElectivesBlock.Length
            printfn "  0. Сохранить и выйти"
            
            printf "\nЧто редактировать? "
            match Console.ReadLine() with
            | "1" -> 
                printf "Новый код: "
                let newCode = Console.ReadLine()
                editLoop { current with Code = newCode }
            | "2" -> 
                printf "Новое русское название: "
                let newName = Console.ReadLine()
                editLoop { current with RussianName = newName }
            | "3" -> 
                printf "Новое английское название: "
                let newName = Console.ReadLine()
                editLoop { current with EnglishName = newName }
            | "4" -> 
                printfn "Тип курса:"
                printfn "  1. Базовая"
                printfn "  2. Электив"
                printfn "  3. Факультатив"
                printf "Выбор: "
                match parseCourseType (Console.ReadLine()) with
                | Some ct -> editLoop { current with Type = ct }
                | None -> 
                    printfn "Неверный тип!"
                    editLoop current
            | "5" -> 
                let editedImpls = editImplementations current.Implementations
                editLoop { current with Implementations = editedImpls }
            | "6" -> 
                let editedBlocks = editElectiveBlocks current.ElectivesBlock
                editLoop { current with ElectivesBlock = editedBlocks }
            | "0" -> current
            | _ -> 
                printfn "Неверный выбор!"
                editLoop current
        
        and editImplementations (impls: Implementation list) : Implementation list =
            printfn "\n=== Реализации (%d шт.) ===" impls.Length
            impls |> List.iteri (fun i impl -> 
                printfn "%d. Семестр %d, %d з.е., %A" (i+1) impl.Semester impl.LaborIntensity impl.BlockCode)
            
            printfn "\nКоманды:"
            printfn "  номер - редактировать реализацию"
            printfn "  a - добавить новую"
            printfn "  d номер - удалить"
            printfn "  s - сохранить"
            
            printf "\nВыбор: "
            let input = Console.ReadLine()
            
            match input with
            | "a" -> 
                let newImpl = createImplementationFromConsole()
                newImpl :: impls |> List.rev
            | cmd when cmd.StartsWith("d ") ->
                let idxStr = cmd.Substring(2).Trim()
                match Int32.TryParse(idxStr) with
                | true, idx when idx > 0 && idx <= impls.Length ->
                    impls 
                    |> List.indexed 
                    |> List.filter (fun (i, _) -> i <> idx-1)
                    |> List.map snd
                | _ ->
                    printfn "Неверный индекс!"
                    impls
            | "s" -> impls
            | num when System.Int32.TryParse(num) |> fst ->
                let idx = Int32.Parse(num) - 1
                if idx >= 0 && idx < impls.Length then
                    let editedImpl = editImplementation impls.[idx]
                    impls 
                    |> List.mapi (fun i existing -> if i = idx then editedImpl else existing)
                else
                    printfn "Неверный индекс!"
                    impls
            | _ -> impls
        
        and editImplementation (impl: Implementation) : Implementation =
            printfn "\n=== Редактирование реализации ==="
            
            let rec editLoop (current: Implementation) =
                printfn "\nТекущие значения:"
                printfn "  1. Семестр: %d" current.Semester
                printfn "  2. Трудоёмкость: %d" current.LaborIntensity
                printfn "  3. Блок: %A" current.BlockCode
                printfn "  4. Компетенции: %A" current.Competences
                printfn "  5. Формы контроля: %s" current.MonitoringTypes
                printfn "  6. Распределение часов"
                printfn "  7. Реализация"
                printfn "  8. Траектория"
                printfn "  0. Готово"
                
                printf "\nЧто редактировать? "
                match Console.ReadLine() with
                | "1" -> 
                    printf "Новый семестр: "
                    let newSem = Int32.Parse(Console.ReadLine())
                    editLoop { current with Semester = newSem }
                | "2" -> 
                    printf "Новая трудоёмкость: "
                    let newLab = Int32.Parse(Console.ReadLine())
                    editLoop { current with LaborIntensity = newLab }
                | "3" -> 
                    printfn "Тип блока:"
                    printfn "  1. Дисциплины"
                    printfn "  2. Практика"
                    printfn "  3. ГИА"
                    printf "Выбор: "
                    match parseBlockCode (Console.ReadLine()) with
                    | Some bc -> editLoop { current with BlockCode = bc }
                    | None -> editLoop current
                | "4" -> 
                    printf "Новые компетенции (через запятую): "
                    let newComps = 
                        Console.ReadLine().Split(',', StringSplitOptions.RemoveEmptyEntries)
                        |> Array.map (fun s -> s.Trim())
                        |> Array.toList
                    editLoop { current with Competences = newComps }
                | "5" -> 
                    printf "Новые формы контроля (через запятую): "
                    let newMonitoring = parseMonitoringTypeList (Console.ReadLine())
                    editLoop { current with MonitoringTypes = newMonitoring }
                | "6" -> 
                    let editedWH = editWorkHours current.WorkHours
                    editLoop { current with WorkHours = editedWH }
                | "7" ->
                    printf "Реализация [%s]: " current.Realization
                    let newRealization = Console.ReadLine()
                    if not (String.IsNullOrWhiteSpace(newRealization)) then
                        editLoop { current with Realization = newRealization }
                    else
                        editLoop current
                | "8" ->
                    printf "Траектория [%s]: " current.Trajectory
                    let newTrajectory = Console.ReadLine()
                    if not (String.IsNullOrWhiteSpace(newTrajectory)) then
                        editLoop { current with Trajectory = newTrajectory }
                    else
                        editLoop current
                | "0" -> current
                | _ -> editLoop current
            
            and editWorkHours (wh: WorkHoursDistribution) : WorkHoursDistribution =
                printfn "\n=== Редактирование распределения часов ==="
                printfn "Текущие значения:"
                printfn "  Лекции: %d" wh.Lecture
                printfn "  Семинары: %d" wh.Seminar
                printfn "  Консультации: %d" wh.Consultation
                printfn "  Практические: %d" wh.Practical
                printfn "  Лабораторные: %d" wh.Lab
                printfn "  И т.д..."
                
                printf "\nВведите 15 чисел через пробел или нажмите Enter для пропуска: "
                let input = Console.ReadLine()
                if String.IsNullOrWhiteSpace(input) then
                    wh
                else
                    try
                        parseWorkHours input
                    with _ ->
                        printfn "Неверный формат! Оставляем текущие значения."
                        wh
            
            editLoop impl
        
        and editElectiveBlocks (blocks: ElectivesBlockEntry list) : ElectivesBlockEntry list =
            printfn "\n=== Блоки выбора (%d шт.) ===" blocks.Length
            blocks |> List.iteri (fun i block -> 
                printfn "%d. Семестр %d, №%d, Специализация: %s" 
                    (i+1) block.Semester block.Number block.Specialization)
            
            printfn "\nКоманды:"
            printfn "  a - добавить"
            printfn "  d номер - удалить"
            printfn "  s - сохранить"
            
            printf "\nВыбор: "
            match Console.ReadLine() with
            | "a" -> 
                printf "Семестр: "
                let sem = Int32.Parse(Console.ReadLine())
                printf "Номер: "
                let num = Int32.Parse(Console.ReadLine())
                printf "Специализация: "
                let spec = Console.ReadLine()
                let newBlock = { Semester = sem; Number = num; Specialization = spec }
                newBlock :: blocks
            | cmd when cmd.StartsWith("d ") ->
                let idxStr = cmd.Substring(2).Trim()
                match Int32.TryParse(idxStr) with
                | true, idx when idx > 0 && idx <= blocks.Length ->
                    blocks 
                    |> List.indexed 
                    |> List.filter (fun (i, _) -> i <> idx-1)
                    |> List.map snd
                | _ ->
                    printfn "Неверный индекс!"
                    blocks
            | "s" -> blocks
            | _ -> blocks
        
        editLoop originalCourse

// Основной интерфейс редактора
type InteractiveEditor() =
    
    member this.StartEditor (initialCourses: Course list) =
        printfn "=== РЕДАКТОР УЧЕБНОГО ПЛАНА ==="
        
        let rec mainLoop (courses: Course list) =
            printfn "\n=== СПИСОК КУРСОВ (%d шт.) ===" courses.Length
            courses |> List.iteri (fun i c -> 
                printfn "%d. [%s] %s" (i+1) c.Code c.RussianName)
            
            printfn "\nОсновные команды:"
            printfn "  номер    - редактировать курс"
            printfn "  new      - создать новый курс"
            printfn "  add      - быстро добавить курс"
            printfn "  del номер - удалить курс"
            printfn "  save     - сохранить в JSON"
            printfn "  load     - загрузить из JSON"
            printfn "  export   - экспортировать один курс"
            printfn "  import   - импортировать курс из файла"
            printfn "  show     - показать json"
            printfn "  exit     - выход"
            
            printf "\n> "
            match Console.ReadLine().ToLower().Trim() with
            | "new" ->
                printfn "\nСоздаем новый курс..."
                let newCourse = ConsoleDSL.createCourseFromConsole()
                mainLoop (newCourse :: courses)
            
            | "add" ->
                // Быстрое добавление
                printf "Введите через запятую: код, рус.название, англ.название: "
                let parts = Console.ReadLine().Split(',')
                if parts.Length >= 3 then
                    let quickCourse = 
                        { emptyCourse with
                            Code = parts.[0].Trim()
                            RussianName = parts.[1].Trim()
                            EnglishName = parts.[2].Trim()
                            Type = Base
                            Implementations = [] }
                    mainLoop (quickCourse :: courses)
                else
                    printfn "Неверный формат!"
                    mainLoop courses
            
            | cmd when cmd.StartsWith("del ") ->
                let idxStr = cmd.Substring(4).Trim()
                match Int32.TryParse(idxStr) with
                | true, idx when idx > 0 && idx <= courses.Length ->
                    let newCourses = 
                        courses 
                        |> List.indexed 
                        |> List.filter (fun (i, _) -> i <> idx-1)
                        |> List.map snd
                    mainLoop newCourses
                | _ ->
                    printfn "Неверный индекс!"
                    mainLoop courses
            
            | "save" ->
                printf "Имя файла (например, courses.json): "
                let filename = Console.ReadLine()
                let result = saveToFile filename courses
                if not result then
                    printfn "Не удалось сохранить файл"
                mainLoop courses
            
            | "load" ->
                printf "Имя файла для загрузки: "
                let filename = Console.ReadLine()
                let loadedCourses = loadFromFile filename
                if not (List.isEmpty loadedCourses) then
                    printf "Заменить текущие курсы или добавить к ним? (replace/add): "
                    match Console.ReadLine().ToLower() with
                    | "replace" | "r" ->
                        mainLoop loadedCourses
                    | "add" | "a" ->
                        let combined = loadedCourses @ courses
                        printfn "Теперь всего %d курсов" combined.Length
                        mainLoop combined
                    | _ ->
                        printfn "Неверный выбор, оставляем текущие курсы"
                        mainLoop courses
                else
                    mainLoop courses
            
            | "export" ->
                if List.isEmpty courses then
                    printfn "Нет курсов для экспорта"
                else
                    printf "Номер курса для экспорта: "
                    match Int32.TryParse(Console.ReadLine()) with
                    | true, idx when idx > 0 && idx <= courses.Length ->
                        let courseToExport = courses.[idx - 1]
                        printf "Имя файла (например, course_[код].json): "
                        let filename = Console.ReadLine()
                        let json = serializeCourse courseToExport
                        try
                            System.IO.File.WriteAllText(filename, json)
                            printfn "Курс '%s' экспортирован в: %s" courseToExport.RussianName filename
                        with ex ->
                            printfn "Ошибка при экспорте: %s" ex.Message
                    | _ ->
                        printfn "Неверный номер курса"
                mainLoop courses
            
            | "import" ->
                printf "Имя файла с курсом для импорта: "
                let filename = Console.ReadLine()
                try
                    if System.IO.File.Exists(filename) then
                        let json = System.IO.File.ReadAllText(filename)
                        let importedCourse = deserializeCourse json
                        printfn "Импортирован курс: %s" importedCourse.RussianName
                        
                        printf "Добавить этот курс? (y/n): "
                        if Console.ReadLine().ToLower() = "y" then
                            mainLoop (importedCourse :: courses)
                        else
                            mainLoop courses
                    else
                        printfn "Файл не найден: %s" filename
                        mainLoop courses
                with ex ->
                    printfn "Ошибка при импорте: %s" ex.Message
                    mainLoop courses
            
            | "exit" -> 
                printfn "Выход из редактора."
                
                if not (List.isEmpty courses) then
                    printf "Сохранить перед выходом? (y/n): "
                    if Console.ReadLine().ToLower() = "y" then
                        printf "Имя файла [autosave.json]: "
                        let filename = 
                            let input = Console.ReadLine()
                            if String.IsNullOrWhiteSpace(input) then "autosave.json" else input
                        saveToFile filename courses |> ignore
                
                courses
            
            | num when System.Int32.TryParse(num) |> fst ->
                let idx = Int32.Parse(num) - 1
                if idx >= 0 && idx < courses.Length then
                    printfn "\nРедактируем курс #%d..." (idx+1)
                    let editedCourse = ConsoleDSL.editCourseWithDSL courses.[idx]
                    let newCourses = 
                        courses 
                        |> List.mapi (fun i c -> if i = idx then editedCourse else c)
                    mainLoop newCourses
                else
                    printfn "Неверный номер курса!"
                    mainLoop courses
            
            | "show" ->
                if List.isEmpty courses then
                    printfn "Нет курсов для отображения"
                else
                    printf "Номер курса для просмотра JSON: "
                    match Int32.TryParse(Console.ReadLine()) with
                    | true, idx when idx > 0 && idx <= courses.Length ->
                        let courseToShow = courses.[idx - 1]
                        let json = serializeCourse courseToShow
                        printfn "\n=== JSON курс #%d ===" idx
                        printfn "%s" json
                        printfn "================================="
                    | _ ->
                        printfn "Неверный номер курса"
                mainLoop courses

            | "help" | "?" ->
                printfn "\nДополнительные команды для работы с файлами:"
                printfn "  save - сохранить ВСЕ курсы в JSON файл"
                printfn "  load - загрузить курсы из JSON файла (можно добавить или заменить)"
                printfn "  export - экспортировать ОДИН курс в отдельный JSON файл"
                printfn "  import - импортировать курс из JSON файла"
                printfn "  show - показать json"
                printfn "  exit - выход с возможностью автосохранения"
                mainLoop courses
            
            | _ ->
                printfn "Неизвестная команда! Введите 'help' для списка команд."
                mainLoop courses
        
        mainLoop initialCourses